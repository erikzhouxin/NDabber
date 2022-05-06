using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 简单接口类创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISampleClassBuilder<T>
    {
        /// <summary>
        /// 是合法的
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// 单例
        /// </summary>
        T Instance { get; }
        /// <summary>
        /// 多例
        /// </summary>
        T Current { get; }
        /// <summary>
        /// 默认值
        /// </summary>
        T Default { get; }
    }
    /// <summary>
    /// 简单接口类创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SampleClassBuilder<T> : ISampleClassBuilder<T>
    {
        #region // 实例对象属性
        /// <summary>
        /// 是合法的
        /// </summary>
        public bool IsValid { get => IsValider; }
        /// <summary>
        /// 单例
        /// </summary>
        public T Instance { get => Instancer; }
        /// <summary>
        /// 多例
        /// </summary>
        public T Current { get => CreateInstance(); }
        /// <summary>
        /// 默认值
        /// </summary>
        public T Default { get => Defaulter; }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public T GetInstance() => CreateInstance();
        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        public T GetDefault() => CreateDefault();
        #endregion
        /// <summary>
        /// 创建实体
        /// </summary>
        public static Func<T> CreateInstance { get; }
        /// <summary>
        /// 创建默认
        /// </summary>
        public static Func<T> CreateDefault { get; internal set; }
        /// <summary>
        /// 是合法的
        /// </summary>
        public static bool IsValider { get; }
        /// <summary>
        /// 静态实例单例
        /// </summary>
        public static T Instancer { get; }
        /// <summary>
        /// 静态实时多例
        /// </summary>
        public static T Currenter { get => CreateInstance(); }
        /// <summary>
        /// 静态默认值
        /// </summary>
        public static T Defaulter { get; set; }
        static SampleClassBuilder()
        {
            var baseType = typeof(T);
            try
            {
                if (!baseType.IsInterface)
                {
                    throw new NotSupportedException($"简单类创建者({nameof(SampleClassBuilder<T>)})只支持接口类型创建");
                }
                var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
#if NETFrame
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                var typeBuilder = moduleBuilder.DefineType($"{baseType.FullName}SampleCIModel", TypeAttributes.Public | TypeAttributes.Class, null, new Type[] { baseType });
                typeBuilder.AddInterfaceImplementation(baseType);

                ConstructorInfo objCtor = typeof(object).GetConstructor(new Type[0]);
                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
                ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Call, objCtor);
                ilOfCtor.Emit(OpCodes.Ret);

                foreach (var item in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var field = typeBuilder.DefineField($"_{item.Name}", item.PropertyType, FieldAttributes.Private);
                    var getter = typeBuilder.DefineMethod($"get_{item.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, item.PropertyType, null);
                    var setter = typeBuilder.DefineMethod($"set_{item.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, null, new Type[] { item.PropertyType });

                    var ilOfGetId = getter.GetILGenerator();
                    ilOfGetId.Emit(OpCodes.Ldarg_0); // this
                    ilOfGetId.Emit(OpCodes.Ldfld, field);
                    ilOfGetId.Emit(OpCodes.Ret);

                    var ilOfSetId = setter.GetILGenerator();
                    ilOfSetId.Emit(OpCodes.Ldarg_0); // this
                    ilOfSetId.Emit(OpCodes.Ldarg_1); // the first one in arguments list
                    ilOfSetId.Emit(OpCodes.Stfld, field);
                    ilOfSetId.Emit(OpCodes.Ret);

                    var propertyId = typeBuilder.DefineProperty(item.Name, Reflection.PropertyAttributes.None, item.PropertyType, null);
                    propertyId.SetGetMethod(getter);
                    propertyId.SetSetMethod(setter);
                }

                var newType = typeBuilder.CreateType();

                CreateInstance = () => (T)Activator.CreateInstance(newType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                IsValider = false;
                CreateInstance = () => default(T);
            }
        }
    }
    /// <summary>
    /// 简单数据库接口类创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SampleDbClassBuilder<T>
    {
        #region // 实例对象属性
        /// <summary>
        /// 是合法的
        /// </summary>
        public bool IsValid { get => IsValider; }
        /// <summary>
        /// 单例
        /// </summary>
        public T Instance { get => Instancer; }
        /// <summary>
        /// 多例
        /// </summary>
        public T Current { get => CreateInstance(); }
        /// <summary>
        /// 默认值
        /// </summary>
        public T Default { get => Defaulter; }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public T GetInstance() => CreateInstance();
        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        public T GetDefault() => CreateDefault();
        #endregion
        /// <summary>
        /// 创建实体
        /// </summary>
        public static Func<T> CreateInstance { get; }
        /// <summary>
        /// 创建默认
        /// </summary>
        public static Func<T> CreateDefault { get; internal set; }
        /// <summary>
        /// 是合法的
        /// </summary>
        public static bool IsValider { get; }
        /// <summary>
        /// 静态实例单例
        /// </summary>
        public static T Instancer { get; }
        /// <summary>
        /// 静态实时多例
        /// </summary>
        public static T Currenter { get => CreateInstance(); }
        /// <summary>
        /// 静态默认值
        /// </summary>
        public static T Defaulter { get; set; }
        static SampleDbClassBuilder()
        {
            var baseType = typeof(T);
            try
            {
                if (!baseType.IsInterface)
                {
                    throw new NotSupportedException($"简单类创建者({nameof(SampleClassBuilder<T>)})只支持接口类型创建");
                }
                var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
#if NETFrame
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                var typeBuilder = moduleBuilder.DefineType($"{baseType.FullName}SampleDIModel", TypeAttributes.Public | TypeAttributes.Class, null, new Type[] { baseType });
                typeBuilder.AddInterfaceImplementation(baseType);

                ConstructorInfo objCtor = typeof(object).GetConstructor(new Type[0]);
                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
                ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Call, objCtor);
                ilOfCtor.Emit(OpCodes.Ret);

                //定义构造器参数
                Type[] attrCtorParma = new Type[] { typeof(string) };


                foreach (var item in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var field = typeBuilder.DefineField($"_{item.Name}", item.PropertyType, FieldAttributes.Private);
                    var getter = typeBuilder.DefineMethod($"get_{item.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, item.PropertyType, null);
                    var setter = typeBuilder.DefineMethod($"set_{item.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, null, new Type[] { item.PropertyType });

                    //CustomAttributeBuilder cab = new CustomAttributeBuilder()

                    var ilOfGetId = getter.GetILGenerator();
                    ilOfGetId.Emit(OpCodes.Ldarg_0); // this
                    ilOfGetId.Emit(OpCodes.Ldfld, field);
                    ilOfGetId.Emit(OpCodes.Ret);

                    var ilOfSetId = setter.GetILGenerator();
                    ilOfSetId.Emit(OpCodes.Ldarg_0); // this
                    ilOfSetId.Emit(OpCodes.Ldarg_1); // the first one in arguments list
                    ilOfSetId.Emit(OpCodes.Stfld, field);
                    ilOfSetId.Emit(OpCodes.Ret);

                    var propertyId = typeBuilder.DefineProperty(item.Name, Reflection.PropertyAttributes.None, item.PropertyType, null);
                    propertyId.SetGetMethod(getter);
                    propertyId.SetSetMethod(setter);
                }

                var newType = typeBuilder.CreateType();

                CreateInstance = () => (T)Activator.CreateInstance(newType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                IsValider = false;
                CreateInstance = () => default(T);
            }
        }
    }
}
