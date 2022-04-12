using System;

namespace System.Data.Mabber
{
    internal interface IEmitter
    {
        void Emit(CodeGenerator generator);
    }
}
