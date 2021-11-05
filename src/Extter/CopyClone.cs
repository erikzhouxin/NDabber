using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 复制克隆
    /// </summary>
    public static class CopyClone
    {
        /// <summary>
        /// 深度表达式树复制
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="original">Object to copy.</param>
        /// <param name="copiedReferencesDict">Dictionary of already copied objects (Keys: original objects, Values: their copies).</param>
        /// <returns></returns>
        public static T DeepExpressionCopy<T>(this T original, Dictionary<object, object> copiedReferencesDict = null)
        {
            return (T)DeepExpressionTreeObjCopy(original, false, copiedReferencesDict ?? new Dictionary<object, object>(new CopyClone.ReferenceEqualityComparer()));
        }
        /// <summary>
        /// 深度Memory的Serialize复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepMemoryCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// 深度反射复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepReflectionCopy<T>(this T original)
        {
            return (T)ReflectionInternalCopy((object)original, new Dictionary<Object, object>(new CopyClone.ReferenceEqualityComparer()));
        }
        #region // 内部方法及类
        internal class ReferenceEqualityComparer : EqualityComparer<Object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }
        internal static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        #region // 表达式树
        private static readonly object ExpressionTreeStructTypeDeepCopyLocker = new object();
        private static Dictionary<Type, bool> ExpressionTreeStructTypeToDeepCopyDic = new Dictionary<Type, bool>();

        private static readonly object ExpressionTreeCompiledCopyFuncDicLocker = new object();
        private static Dictionary<Type, Func<object, Dictionary<object, object>, object>> ExpressionTreeCompiledCopyFuncDic = new Dictionary<Type, Func<object, Dictionary<object, object>, object>>();

        private static readonly Type ExpressionTreeObjectType = typeof(Object);
        private static readonly Type ExpressionTreeObjectDictionaryType = typeof(Dictionary<object, object>);
        private static readonly Type ExpressionTreeFieldInfoType = typeof(FieldInfo);
        private static readonly MethodInfo SetValueMethod = ExpressionTreeFieldInfoType.GetMethod("SetValue", new[] { ExpressionTreeObjectType, ExpressionTreeObjectType });
        private static readonly Type ThisType = typeof(CopyClone);
        private static readonly MethodInfo DeepCopyByExpressionTreeObjMethod = ThisType.GetMethod(nameof(DeepExpressionTreeObjCopy), BindingFlags.NonPublic | BindingFlags.Static);

        private static object DeepExpressionTreeObjCopy(object original, bool forceDeepCopy, Dictionary<object, object> copiedReferencesDict)
        {
            if (original == null)
            {
                return null;
            }

            var type = original.GetType();

            if (ExpressionTreeIsDelegate(type))
            {
                return null;
            }

            if (!forceDeepCopy && !ExpressionTreeIsTypeToDeepCopy(type))
            {
                return original;
            }

            object alreadyCopiedObject;

            if (copiedReferencesDict.TryGetValue(original, out alreadyCopiedObject))
            {
                return alreadyCopiedObject;
            }

            if (type == ExpressionTreeObjectType)
            {
                return new object();
            }

            var compiledCopyFunction = ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(type);

            object copy = compiledCopyFunction(original, copiedReferencesDict);

            return copy;
        }

        private static Func<object, Dictionary<object, object>, object> ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            Func<object, Dictionary<object, object>, object> compiledCopyFunction;

            if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
            {
                lock (ExpressionTreeCompiledCopyFuncDicLocker)
                {
                    if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
                    {
                        var uncompiledCopyFunction = ExpressionTreeCreateCompiledLambdaCopyFuncForType(type);

                        compiledCopyFunction = uncompiledCopyFunction.Compile();

                        var dictionaryCopy = ExpressionTreeCompiledCopyFuncDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        dictionaryCopy.Add(type, compiledCopyFunction);

                        ExpressionTreeCompiledCopyFuncDic = dictionaryCopy;
                    }
                }
            }

            return compiledCopyFunction;
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCreateCompiledLambdaCopyFuncForType(Type type)
        {
            ParameterExpression inputParameter;
            ParameterExpression inputDictionary;
            ParameterExpression outputVariable;
            ParameterExpression boxingVariable;
            LabelTarget endLabel;
            List<ParameterExpression> variables;
            List<Expression> expressions;

            ///// INITIALIZATION OF EXPRESSIONS AND VARIABLES

            ExpressionTreeInitializeExpressions(type,
                                  out inputParameter,
                                  out inputDictionary,
                                  out outputVariable,
                                  out boxingVariable,
                                  out endLabel,
                                  out variables,
                                  out expressions);

            ///// RETURN NULL IF ORIGINAL IS NULL

            ExpressionTreeIfNullThenReturnNull(inputParameter, endLabel, expressions);

            ///// MEMBERWISE CLONE ORIGINAL OBJECT

            ExpressionTreeMemberwiseCloneInputToOutput(type, inputParameter, outputVariable, expressions);

            ///// STORE COPIED OBJECT TO REFERENCES DICTIONARY

            if (ExpressionTreeIsClassOtherThanString(type))
            {
                ExpressionTreeStoreReferencesIntoDictionary(inputParameter, inputDictionary, outputVariable, expressions);
            }

            ///// COPY ALL NONVALUE OR NONPRIMITIVE FIELDS

            ExpressionTreeFieldsCopy(type,
                                  inputParameter,
                                  inputDictionary,
                                  outputVariable,
                                  boxingVariable,
                                  expressions);

            ///// COPY ELEMENTS OF ARRAY

            if (type.IsArray && ExpressionTreeIsTypeToDeepCopy(type.GetElementType()))
            {
                ExpressionTreeCreateArrayCopyLoop(type,
                                              inputParameter,
                                              inputDictionary,
                                              outputVariable,
                                              variables,
                                              expressions);
            }

            ///// COMBINE ALL EXPRESSIONS INTO LAMBDA FUNCTION

            var lambda = ExpressionTreeCombineAllIntoLambdaFunc(inputParameter, inputDictionary, outputVariable, endLabel, variables, expressions);

            return lambda;
        }

        private static void ExpressionTreeInitializeExpressions(Type type,
                                                  out ParameterExpression inputParameter,
                                                  out ParameterExpression inputDictionary,
                                                  out ParameterExpression outputVariable,
                                                  out ParameterExpression boxingVariable,
                                                  out LabelTarget endLabel,
                                                  out List<ParameterExpression> variables,
                                                  out List<Expression> expressions)
        {

            inputParameter = Expression.Parameter(ExpressionTreeObjectType);

            inputDictionary = Expression.Parameter(ExpressionTreeObjectDictionaryType);

            outputVariable = Expression.Variable(type);

            boxingVariable = Expression.Variable(ExpressionTreeObjectType);

            endLabel = Expression.Label();

            variables = new List<ParameterExpression>();

            expressions = new List<Expression>();

            variables.Add(outputVariable);
            variables.Add(boxingVariable);
        }

        private static void ExpressionTreeIfNullThenReturnNull(ParameterExpression inputParameter, LabelTarget endLabel, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// if (input == null)
            ///// {
            /////     return null;
            ///// }

            var ifNullThenReturnNullExpression =
                Expression.IfThen(
                    Expression.Equal(
                        inputParameter,
                        Expression.Constant(null, ExpressionTreeObjectType)),
                    Expression.Return(endLabel));

            expressions.Add(ifNullThenReturnNullExpression);
        }

        private static void ExpressionTreeMemberwiseCloneInputToOutput(
            Type type,
            ParameterExpression inputParameter,
            ParameterExpression outputVariable,
            List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// var output = (<type>)input.MemberwiseClone();

            var memberwiseCloneMethod = ExpressionTreeObjectType.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

            var memberwiseCloneInputExpression =
                Expression.Assign(
                    outputVariable,
                    Expression.Convert(
                        Expression.Call(
                            inputParameter,
                            memberwiseCloneMethod),
                        type));

            expressions.Add(memberwiseCloneInputExpression);
        }

        private static void ExpressionTreeStoreReferencesIntoDictionary(ParameterExpression inputParameter,
                                                                          ParameterExpression inputDictionary,
                                                                          ParameterExpression outputVariable,
                                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// inputDictionary[(Object)input] = (Object)output;

            var storeReferencesExpression =
                Expression.Assign(
                    Expression.Property(
                        inputDictionary,
                        ExpressionTreeObjectDictionaryType.GetProperty("Item"),
                        inputParameter),
                    Expression.Convert(outputVariable, ExpressionTreeObjectType));

            expressions.Add(storeReferencesExpression);
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCombineAllIntoLambdaFunc(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            LabelTarget endLabel,
            List<ParameterExpression> variables,
            List<Expression> expressions)
        {
            expressions.Add(Expression.Label(endLabel));

            expressions.Add(Expression.Convert(outputVariable, ExpressionTreeObjectType));

            var finalBody = Expression.Block(variables, expressions);

            var lambda = Expression.Lambda<Func<object, Dictionary<object, object>, object>>(finalBody, inputParameter, inputDictionary);

            return lambda;
        }

        private static void ExpressionTreeCreateArrayCopyLoop(Type type,
                                                          ParameterExpression inputParameter,
                                                          ParameterExpression inputDictionary,
                                                          ParameterExpression outputVariable,
                                                          List<ParameterExpression> variables,
                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 
            ///// 
            ///// int length1 = inputarray.GetLength(0); 
            ///// i1 = 0; 
            ///// while (true)
            ///// {
            /////     if (i1 >= length1)
            /////     {
            /////         goto ENDLABELFORLOOP1;
            /////     }
            /////     int length2 = inputarray.GetLength(1); 
            /////     i2 = 0; 
            /////     while (true)
            /////     {
            /////         if (i2 >= length2)
            /////         {
            /////             goto ENDLABELFORLOOP2;
            /////         }
            /////         ...
            /////         ...
            /////         ...
            /////         int lengthn = inputarray.GetLength(n); 
            /////         in = 0; 
            /////         while (true)
            /////         {
            /////             if (in >= lengthn)
            /////             {
            /////                 goto ENDLABELFORLOOPn;
            /////             }
            /////             outputarray[i1, i2, ..., in] 
            /////                 = (<elementType>)DeepCopyByExpressionTreeObj(
            /////                        (Object)inputarray[i1, i2, ..., in])
            /////             in++; 
            /////         }
            /////         ENDLABELFORLOOPn:
            /////         ...
            /////         ...  
            /////         ...
            /////         i2++; 
            /////     }
            /////     ENDLABELFORLOOP2:
            /////     i1++; 
            ///// }
            ///// ENDLABELFORLOOP1:

            var rank = type.GetArrayRank();

            var indices = ExpressionTreeGenerateIndices(rank);

            variables.AddRange(indices);

            var elementType = type.GetElementType();

            var assignExpression = ExpressionTreeArrayFieldToArrayFieldAssign(inputParameter, inputDictionary, outputVariable, elementType, type, indices);

            Expression forExpression = assignExpression;

            for (int dimension = 0; dimension < rank; dimension++)
            {
                var indexVariable = indices[dimension];

                forExpression = ExpressionTreeLoopIntoLoop(inputParameter, indexVariable, forExpression, dimension);
            }

            expressions.Add(forExpression);
        }

        private static List<ParameterExpression> ExpressionTreeGenerateIndices(int arrayRank)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 

            var indices = new List<ParameterExpression>();

            for (int i = 0; i < arrayRank; i++)
            {
                var indexVariable = Expression.Variable(typeof(Int32));

                indices.Add(indexVariable);
            }

            return indices;
        }

        private static BinaryExpression ExpressionTreeArrayFieldToArrayFieldAssign(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            Type elementType,
            Type arrayType,
            List<ParameterExpression> indices)
        {
            ///// Intended code:
            /////
            ///// outputarray[i1, i2, ..., in] 
            /////     = (<elementType>)DeepCopyByExpressionTreeObj(
            /////            (Object)inputarray[i1, i2, ..., in]);

            var indexTo = Expression.ArrayAccess(outputVariable, indices);

            var indexFrom = Expression.ArrayIndex(Expression.Convert(inputParameter, arrayType), indices);

            var forceDeepCopy = elementType != ExpressionTreeObjectType;

            var rightSide =
                Expression.Convert(
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(indexFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary),
                    elementType);

            var assignExpression = Expression.Assign(indexTo, rightSide);

            return assignExpression;
        }

        private static BlockExpression ExpressionTreeLoopIntoLoop(
            ParameterExpression inputParameter,
            ParameterExpression indexVariable,
            Expression loopToEncapsulate,
            int dimension)
        {
            ///// Intended code:
            /////
            ///// int length = inputarray.GetLength(dimension); 
            ///// i = 0; 
            ///// while (true)
            ///// {
            /////     if (i >= length)
            /////     {
            /////         goto ENDLABELFORLOOP;
            /////     }
            /////     loopToEncapsulate;
            /////     i++; 
            ///// }
            ///// ENDLABELFORLOOP:

            var lengthVariable = Expression.Variable(typeof(Int32));

            var endLabelForThisLoop = Expression.Label();

            var newLoop =
                Expression.Loop(
                    Expression.Block(
                        new ParameterExpression[0],
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(indexVariable, lengthVariable),
                            Expression.Break(endLabelForThisLoop)),
                        loopToEncapsulate,
                        Expression.PostIncrementAssign(indexVariable)),
                    endLabelForThisLoop);

            var lengthAssignment = ExpressionTreeGetLengthForDimension(lengthVariable, inputParameter, dimension);

            var indexAssignment = Expression.Assign(indexVariable, Expression.Constant(0));

            return Expression.Block(
                new[] { lengthVariable },
                lengthAssignment,
                indexAssignment,
                newLoop);
        }

        private static BinaryExpression ExpressionTreeGetLengthForDimension(
            ParameterExpression lengthVariable,
            ParameterExpression inputParameter,
            int i)
        {
            ///// Intended code:
            /////
            ///// length = ((Array)input).GetLength(i); 

            var getLengthMethod = typeof(Array).GetMethod("GetLength", BindingFlags.Public | BindingFlags.Instance);

            var dimensionConstant = Expression.Constant(i);

            return Expression.Assign(
                lengthVariable,
                Expression.Call(
                    Expression.Convert(inputParameter, typeof(Array)),
                    getLengthMethod,
                    new[] { dimensionConstant }));
        }

        private static void ExpressionTreeFieldsCopy(Type type,
                                                  ParameterExpression inputParameter,
                                                  ParameterExpression inputDictionary,
                                                  ParameterExpression outputVariable,
                                                  ParameterExpression boxingVariable,
                                                  List<Expression> expressions)
        {
            var fields = ExpressionTreeGetAllRelevantFields(type);

            var readonlyFields = fields.Where(f => f.IsInitOnly).ToList();
            var writableFields = fields.Where(f => !f.IsInitOnly).ToList();

            ///// READONLY FIELDS COPY (with boxing)

            bool shouldUseBoxing = readonlyFields.Any();

            if (shouldUseBoxing)
            {
                var boxingExpression = Expression.Assign(boxingVariable, Expression.Convert(outputVariable, ExpressionTreeObjectType));

                expressions.Add(boxingExpression);
            }

            foreach (var field in readonlyFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeReadonlyFieldToNull(field, boxingVariable, expressions);
                }
                else
                {
                    ExpressionTreeReadonlyFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                boxingVariable,
                                                expressions);
                }
            }

            if (shouldUseBoxing)
            {
                var unboxingExpression = Expression.Assign(outputVariable, Expression.Convert(boxingVariable, type));

                expressions.Add(unboxingExpression);
            }

            ///// NOT-READONLY FIELDS COPY

            foreach (var field in writableFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeWritableFieldToNull(field, outputVariable, expressions);
                }
                else
                {
                    ExpressionTreeWritableFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                outputVariable,
                                                expressions);
                }
            }
        }

        private static FieldInfo[] ExpressionTreeGetAllRelevantFields(Type type, bool forceAllFields = false)
        {
            var fieldsList = new List<FieldInfo>();

            var typeCache = type;

            while (typeCache != null)
            {
                fieldsList.AddRange(
                    typeCache
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                        .Where(field => forceAllFields || ExpressionTreeIsTypeToDeepCopy(field.FieldType)));

                typeCache = typeCache.BaseType;
            }

            return fieldsList.ToArray();
        }

        private static void ExpressionTreeReadonlyFieldToNull(FieldInfo field, ParameterExpression boxingVariable, List<Expression> expressions)
        {
            // This option must be implemented by Reflection because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, <fieldtype>null);

            var fieldToNullExpression =
                    Expression.Call(
                        Expression.Constant(field),
                        SetValueMethod,
                        boxingVariable,
                        Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeReadonlyFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression boxingVariable,
                                                        List<Expression> expressions)
        {
            // This option must be implemented by Reflection (SetValueMethod) because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>))

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Call(
                    Expression.Constant(field, ExpressionTreeFieldInfoType),
                    SetValueMethod,
                    boxingVariable,
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static void ExpressionTreeWritableFieldToNull(FieldInfo field, ParameterExpression outputVariable, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<type>)null;

            var fieldTo = Expression.Field(outputVariable, field);

            var fieldToNullExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeWritableFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression outputVariable,
                                                        List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<fieldType>)DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>);

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var fieldType = field.FieldType;

            var fieldTo = Expression.Field(outputVariable, field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Convert(
                        Expression.Call(
                            DeepCopyByExpressionTreeObjMethod,
                            Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                            Expression.Constant(forceDeepCopy, typeof(Boolean)),
                            inputDictionary),
                        fieldType));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static bool ExpressionTreeIsDelegate(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        private static bool ExpressionTreeIsTypeToDeepCopy(Type type)
        {
            return ExpressionTreeIsClassOtherThanString(type) || ExpressionTreeIsStructWhichNeedsDeepCopy(type);
        }

        private static bool ExpressionTreeIsClassOtherThanString(Type type)
        {
            return !type.IsValueType && type != typeof(String);
        }

        private static bool ExpressionTreeIsStructWhichNeedsDeepCopy(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            bool isStructTypeToDeepCopy;

            if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
            {
                lock (ExpressionTreeStructTypeDeepCopyLocker)
                {
                    if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
                    {
                        isStructTypeToDeepCopy = ExpressionTreeIsStructOtherThanBasicValueTypes(type) && ExpressionTreeHasInItsHierarchyFieldsWithClasses(type);

                        var newDictionary = ExpressionTreeStructTypeToDeepCopyDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        newDictionary[type] = isStructTypeToDeepCopy;

                        ExpressionTreeStructTypeToDeepCopyDic = newDictionary;
                    }
                }
            }

            return isStructTypeToDeepCopy;
        }

        private static bool ExpressionTreeIsStructOtherThanBasicValueTypes(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type != typeof(Decimal);
        }

        private static bool ExpressionTreeHasInItsHierarchyFieldsWithClasses(Type type, HashSet<Type> alreadyCheckedTypes = null)
        {
            alreadyCheckedTypes = alreadyCheckedTypes ?? new HashSet<Type>();

            alreadyCheckedTypes.Add(type);

            var allFields = ExpressionTreeGetAllRelevantFields(type, forceAllFields: true);

            var allFieldTypes = allFields.Select(f => f.FieldType).Distinct().ToList();

            var hasFieldsWithClasses = allFieldTypes.Any(ExpressionTreeIsClassOtherThanString);

            if (hasFieldsWithClasses)
            {
                return true;
            }

            var notBasicStructsTypes = allFieldTypes.Where(ExpressionTreeIsStructOtherThanBasicValueTypes).ToList();

            var typesToCheck = notBasicStructsTypes.Where(t => !alreadyCheckedTypes.Contains(t)).ToList();

            foreach (var typeToCheck in typesToCheck)
            {
                if (ExpressionTreeHasInItsHierarchyFieldsWithClasses(typeToCheck, alreadyCheckedTypes))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region // 反射方法
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static bool IsPrimitive(Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        private static Object ReflectionInternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(ReflectionInternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect);
            ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void ReflectionRecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void ReflectionCopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (typeToReflect.Name == "Entry" && fieldInfo.Name == "value")
                { }

                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = ReflectionInternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        #endregion
        #endregion
    }
}
