using System;
using System.Reflection.Emit;

namespace System.Data.Mabber
{
    internal interface IDynamicAssembly
    {
        TypeBuilder DefineType(string typeName, Type parentType);
        void Save();
    }
}
