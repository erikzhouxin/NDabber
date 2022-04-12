using System;
#if !NETFx
using System.Runtime.Serialization;
#endif

namespace System.Data.Mabber
{
    /// <summary>
    ///     Exception during mapping or binding
    /// </summary>
#if !NETFx
    [Serializable]
#endif
    public class TinyMapperException : Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TinyMapperException()
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public TinyMapperException(string message) : base(message)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TinyMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !NETFx
        protected TinyMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
