using System;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.Dabber
{
    public static partial class SqlMapper
    {
        /// <summary>
        /// Implement this interface to perform custom type-based parameter handling and value parsing
        /// </summary>
        public interface ITypeHandler
        {
            /// <summary>
            /// Assign the value of a parameter before a command executes
            /// </summary>
            /// <param name="parameter">The parameter to configure</param>
            /// <param name="value">Parameter value</param>
            void SetValue(IDbDataParameter parameter, object value);

            /// <summary>
            /// Parse a database value back to a typed value
            /// </summary>
            /// <param name="value">The value from the database</param>
            /// <param name="destinationType">The type to parse to</param>
            /// <returns>The typed value</returns>
            object Parse(Type destinationType, object value);
        }
        /// <summary>
        /// Base-class for simple type-handlers
        /// </summary>
        /// <typeparam name="T">This <see cref="Type"/> this handler is for.</typeparam>
        public abstract class TypeHandler<T> : ITypeHandler
        {
            /// <summary>
            /// Assign the value of a parameter before a command executes
            /// </summary>
            /// <param name="parameter">The parameter to configure</param>
            /// <param name="value">Parameter value</param>
            public abstract void SetValue(IDbDataParameter parameter, T value);

            /// <summary>
            /// Parse a database value back to a typed value
            /// </summary>
            /// <param name="value">The value from the database</param>
            /// <returns>The typed value</returns>
            public abstract T Parse(object value);

            void ITypeHandler.SetValue(IDbDataParameter parameter, object value)
            {
                if (value is DBNull)
                {
                    parameter.Value = value;
                }
                else
                {
                    SetValue(parameter, (T)value);
                }
            }

            object ITypeHandler.Parse(Type destinationType, object value)
            {
                return Parse(value);
            }
        }

        /// <summary>
        /// Base-class for simple type-handlers that are based around strings
        /// </summary>
        /// <typeparam name="T">This <see cref="Type"/> this handler is for.</typeparam>
        public abstract class StringTypeHandler<T> : TypeHandler<T>
        {
            /// <summary>
            /// Parse a string into the expected type (the string will never be null)
            /// </summary>
            /// <param name="xml">The string to parse.</param>
            protected abstract T Parse(string xml);

            /// <summary>
            /// Format an instance into a string (the instance will never be null)
            /// </summary>
            /// <param name="xml">The string to format.</param>
            protected abstract string Format(T xml);

            /// <summary>
            /// Assign the value of a parameter before a command executes
            /// </summary>
            /// <param name="parameter">The parameter to configure</param>
            /// <param name="value">Parameter value</param>
            public override void SetValue(IDbDataParameter parameter, T value)
            {
                parameter.Value = value == null ? (object)DBNull.Value : Format(value);
            }

            /// <summary>
            /// Parse a database value back to a typed value
            /// </summary>
            /// <param name="value">The value from the database</param>
            /// <returns>The typed value</returns>
            public override T Parse(object value)
            {
                if (value == null || value is DBNull) return default;
                return Parse((string)value);
            }
        }

        /// <summary>
        /// A type handler for data-types that are supported by the underlying provider, but which need
        /// a well-known UdtTypeName to be specified
        /// </summary>
        public class UdtTypeHandler : ITypeHandler
        {
            private readonly string udtTypeName;
            /// <summary>
            /// Creates a new instance of UdtTypeHandler with the specified <see cref="UdtTypeHandler"/>.
            /// </summary>
            /// <param name="udtTypeName">The user defined type name.</param>
            public UdtTypeHandler(string udtTypeName)
            {
                if (string.IsNullOrEmpty(udtTypeName)) throw new ArgumentException("Cannot be null or empty", udtTypeName);
                this.udtTypeName = udtTypeName;
            }

            object ITypeHandler.Parse(Type destinationType, object value)
            {
                return value is DBNull ? null : value;
            }

            void ITypeHandler.SetValue(IDbDataParameter parameter, object value)
            {
#pragma warning disable 0618
                parameter.Value = SanitizeParameterValue(value);
#pragma warning restore 0618
                if (!(value is DBNull)) StructuredHelper.ConfigureUDT(parameter, udtTypeName);
            }
        }
    }
    internal abstract class XmlTypeHandler<T> : SqlMapper.StringTypeHandler<T>
    {
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            base.SetValue(parameter, value);
            parameter.DbType = DbType.Xml;
        }
    }

    internal sealed class XmlDocumentHandler : XmlTypeHandler<XmlDocument>
    {
        protected override XmlDocument Parse(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        protected override string Format(XmlDocument xml) => xml.OuterXml;
    }

    internal sealed class XDocumentHandler : XmlTypeHandler<XDocument>
    {
        protected override XDocument Parse(string xml) => XDocument.Parse(xml);
        protected override string Format(XDocument xml) => xml.ToString();
    }

    internal sealed class XElementHandler : XmlTypeHandler<XElement>
    {
        protected override XElement Parse(string xml) => XElement.Parse(xml);
        protected override string Format(XElement xml) => xml.ToString();
    }
}