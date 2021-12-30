using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Data.Cobber
{
    internal class ClassSampleTypeBuilder : IClassCreateBuilder
    {
        /*
        {
            var baseType = typeof(T);
            var poolType = typeof(DbConnectionPool<T>);
            var baseCtor = baseType.GetConstructor(new Type[] { typeof(String) });
            if (baseCtor == null) { throw new Exception($"不支持没有连接字符串构造的[{baseType.Name}]"); }
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            var getPoolMethod = poolType.GetMethod("GetPool", new Type[] { typeof(String) });
            var recycleMethod = poolType.GetMethod("Recycle", new Type[] { baseType });
            var disposeMethod = baseType.GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance);
#if NETFrame
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType($"DbConnectionPool{baseType.Name}", TypeAttributes.Public | TypeAttributes.Class, baseType, new Type[] { typeof(InnerPool) });

            // 创建构造
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(String) });
            ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Ldarg_1);
            ilOfCtor.Emit(OpCodes.Call, baseCtor);
            ilOfCtor.Emit(OpCodes.Ret);

            // 添加方法
            var methodRelease = typeBuilder.DefineMethod("Release", MethodAttributes.Virtual | MethodAttributes.Public);
            var ilOfRelease = methodRelease.GetILGenerator();
            ilOfRelease.Emit(OpCodes.Ldarg_0);
            ilOfRelease.Emit(OpCodes.Ldarg_1);
            ilOfRelease.Emit(OpCodes.Call, disposeMethod);
            ilOfRelease.Emit(OpCodes.Ret);

            var methodDispose = typeBuilder.DefineMethod("Dispose", MethodAttributes.Virtual | MethodAttributes.Family, null, new Type[] { typeof(bool) });
            var ilOfDispose = methodDispose.GetILGenerator();
            ilOfDispose.Emit(OpCodes.Ldarg_0);
            ilOfDispose.Emit(OpCodes.Callvirt, baseType.GetMethod("get_ConnectionString"));
            ilOfDispose.Emit(OpCodes.Call, getPoolMethod);
            ilOfDispose.Emit(OpCodes.Ldarg_0);
            ilOfDispose.Emit(OpCodes.Callvirt, recycleMethod);
            ilOfDispose.Emit(OpCodes.Ret);

            var newType = typeBuilder.CreateType();
#if NETFrame
            // assemblyBuilder.Save(assemblyName.Name + ".dll");
#endif
            CreateInstance = (connString) => (T)Activator.CreateInstance(newType, connString);
        }*/
    }
    /// <summary>
    /// 简单类创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SampleClassBuilder<T>
    {
        /// <summary>
        /// 创建实体
        /// </summary>
        public static Func<T> CreateInstance { get; }
        static SampleClassBuilder()
        {
            var baseType = typeof(T);
            if (!baseType.IsInterface) { throw new NotSupportedException("简单类创建者(SampleClassBuilder)只支持接口类型创建"); }
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
#if NETFrame
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType($"{baseType.FullName}CodeGenerImpl", TypeAttributes.Public | TypeAttributes.Class, null, new Type[] { baseType });
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
    }
}
