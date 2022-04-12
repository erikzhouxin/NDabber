using System;

namespace System.Data.Mabber
{
    internal interface IEmitterType : IEmitter
    {
        Type ObjectType { get; }
    }
}
