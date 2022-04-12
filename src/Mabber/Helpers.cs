using System;
using System.Reflection.Emit;
#if NETFx
using System.Reflection;
#endif

namespace System.Data.Mabber
{
    internal static class Helpers
    {
        internal static bool IsValueType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        internal static bool IsPrimitive(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        internal static bool IsEnum(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        internal static bool IsGenericType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        internal static Type CreateType(TypeBuilder typeBuilder)
        {
#if NETFx
            return typeBuilder.CreateTypeInfo().AsType();
#else
            return typeBuilder.CreateType();
#endif
        }

        internal static Type BaseType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

    }
}