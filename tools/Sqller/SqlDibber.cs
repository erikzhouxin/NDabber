﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Cobber;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.Sqller
{
    /// <summary>
    /// Sql挖坑器
    /// </summary>
    public static partial class SqlDibber
    {
        #region // 外部属性
        private static IEqualityComparer<string> _connStringComparer = StringComparer.Ordinal;
        /// <summary>
        /// 连接字符串比较
        /// </summary>
        public static IEqualityComparer<string> ConnStringComparer
        {
            get { return _connStringComparer; }
            set { _connStringComparer = value ?? StringComparer.Ordinal; }
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        public static int? CommandTimeout { get; set; }
        #endregion 外部属性
        #region // 外部方法
        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <param name="cnn">数据库连接</param>
        /// <param name="sql">需要执行的SQL</param>
        /// <param name="param">需要执行的参数</param>
        /// <param name="transaction">执行事务</param>
        /// <param name="commandTimeout">命令超时时间(秒)</param>
        /// <param name="commandType">命令类型(SQL语句/存储过程/表)</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var command = new CmdDefinition(sql, param, transaction, commandTimeout, commandType, CmdFlags.Buffered);
            return ExecuteImpl(cnn, command);
        }
        #endregion 外部方法
        #region // 公开方法
        /// <summary>
        /// Called if the query cache is purged via PurgeQueryCache
        /// </summary>
        public static event EventHandler QueryCachePurged;
        /// <summary>
        /// Purge the query cache
        /// </summary>
        public static void PurgeQueryCache()
        {
            _queryCache.Clear();
            TypeDeserializerCache.Purge();
            OnQueryCachePurged();
        }

        /// <summary>
        /// Return a count of all the cached queries by Dapper
        /// </summary>
        /// <returns></returns>
        public static int GetCachedSQLCount()
        {
            return _queryCache.Count;
        }

        /// <summary>
        /// Return a list of all the queries cached by Dapper
        /// </summary>
        /// <param name="ignoreHitCountAbove"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, int>> GetCachedSQL(int ignoreHitCountAbove = int.MaxValue)
        {
            var data = _queryCache.Select(pair => Tuple.Create(pair.Key.connectionString, pair.Key.sql, pair.Value.GetHitCount()));
            return (ignoreHitCountAbove < int.MaxValue)
                    ? data.Where(tuple => tuple.Item3 <= ignoreHitCountAbove)
                    : data;
        }

        /// <summary>
        /// Deep diagnostics only: find any hash collisions in the cache
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuple<int, int>> GetHashCollissions()
        {
            var counts = new Dictionary<int, int>();
            foreach (var key in _queryCache.Keys)
            {
                if (!counts.TryGetValue(key.hashCode, out int count))
                {
                    counts.Add(key.hashCode, 1);
                }
                else
                {
                    counts[key.hashCode] = count + 1;
                }
            }
            return from pair in counts
                   where pair.Value > 1
                   select Tuple.Create(pair.Key, pair.Value);
        }
        /// <summary>
        /// Get the DbType that maps to a given value.
        /// </summary>
        /// <param name="value">The object to get a corresponding database type for.</param>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DbType GetDbType(object value)
        {
            if (value == null || value is DBNull) return DbType.Object;

            return LookupDbType(value.GetType(), "n/a", false, out ITypeHandler _);
        }

        /// <summary>
        /// OBSOLETE: For internal usage only. Lookup the DbType and handler for a given Type and member
        /// </summary>
        /// <param name="type">The type to lookup.</param>
        /// <param name="name">The name (for error messages).</param>
        /// <param name="demand">Whether to demand a value (throw if missing).</param>
        /// <param name="handler">The handler for <paramref name="type"/>.</param>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DbType LookupDbType(Type type, string name, bool demand, out ITypeHandler handler)
        {
            handler = null;
            var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullUnderlyingType != null) type = nullUnderlyingType;
            if (type.IsEnum && !typeMap.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            if (typeMap.TryGetValue(type, out DbType dbType))
            {
                return dbType;
            }
            if (type.FullName == LinqBinary)
            {
                return DbType.Binary;
            }
            if (typeHandlers.TryGetValue(type, out handler))
            {
                return DbType.Object;
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // auto-detect things like IEnumerable<SqlDataRecord> as a family
                if (type.IsInterface && type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && typeof(IEnumerable<IDataRecord>).IsAssignableFrom(type))
                {
                    var argTypes = type.GetGenericArguments();
                    if (typeof(IDataRecord).IsAssignableFrom(argTypes[0]))
                    {
                        try
                        {
                            handler = (ITypeHandler)Activator.CreateInstance(
                                typeof(DataRecordHandler<>).MakeGenericType(argTypes));
                            AddTypeHandlerImpl(type, handler, true);
                            return DbType.Object;
                        }
                        catch
                        {
                            handler = null;
                        }
                    }
                }
                return DynamicArgs.EnumerableMultiParameter;
            }

            switch (type.FullName)
            {
                case "Microsoft.SqlServer.Types.SqlGeography":
                    AddTypeHandler(type, handler = new UdtTypeHandler("geography"));
                    return DbType.Object;
                case "Microsoft.SqlServer.Types.SqlGeometry":
                    AddTypeHandler(type, handler = new UdtTypeHandler("geometry"));
                    return DbType.Object;
                case "Microsoft.SqlServer.Types.SqlHierarchyId":
                    AddTypeHandler(type, handler = new UdtTypeHandler("hierarchyid"));
                    return DbType.Object;
            }

            if (demand)
                throw new NotSupportedException($"The member {name} of type {type.FullName} cannot be used as a parameter value");
            return DbType.Object;
        }
        /// <summary>
        /// OBSOLETE: For internal usage only. Sanitizes the parameter value with proper type casting.
        /// </summary>
        /// <param name="value">The value to sanitize.</param>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static object SanitizeParameterValue(object value)
        {
            if (value == null) return DBNull.Value;
            if (value is Enum)
            {
                TypeCode typeCode = value is IConvertible convertible
                    ? convertible.GetTypeCode()
                    : Type.GetTypeCode(Enum.GetUnderlyingType(value.GetType()));

                switch (typeCode)
                {
                    case TypeCode.Byte: return (byte)value;
                    case TypeCode.SByte: return (sbyte)value;
                    case TypeCode.Int16: return (short)value;
                    case TypeCode.Int32: return (int)value;
                    case TypeCode.Int64: return (long)value;
                    case TypeCode.UInt16: return (ushort)value;
                    case TypeCode.UInt32: return (uint)value;
                    case TypeCode.UInt64: return (ulong)value;
                }
            }
            return value;
        }

        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <param name="type">The type to handle.</param>
        /// <param name="handler">The handler to process the <paramref name="type"/>.</param>
        public static void AddTypeHandler(Type type, ITypeHandler handler) => AddTypeHandlerImpl(type, handler, true);

        internal static bool HasTypeHandler(Type type) => typeHandlers.ContainsKey(type);

        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <param name="type">The type to handle.</param>
        /// <param name="handler">The handler to process the <paramref name="type"/>.</param>
        /// <param name="clone">Whether to clone the current type handler map.</param>
        public static void AddTypeHandlerImpl(Type type, ITypeHandler handler, bool clone)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            Type secondary = null;
            if (type.IsValueType)
            {
                var underlying = Nullable.GetUnderlyingType(type);
                if (underlying == null)
                {
                    secondary = typeof(Nullable<>).MakeGenericType(type); // the Nullable<T>
                    // type is already the T
                }
                else
                {
                    secondary = type; // the Nullable<T>
                    type = underlying; // the T
                }
            }

            var snapshot = typeHandlers;
            if (snapshot.TryGetValue(type, out ITypeHandler oldValue) && handler == oldValue) return; // nothing to do

            var newCopy = clone ? new Dictionary<Type, ITypeHandler>(snapshot) : snapshot;

#pragma warning disable 618
            typeof(TypeHandlerCache<>).MakeGenericType(type).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
            if (secondary != null)
            {
                typeof(TypeHandlerCache<>).MakeGenericType(secondary).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
            }
#pragma warning restore 618
            if (handler == null)
            {
                newCopy.Remove(type);
                if (secondary != null) newCopy.Remove(secondary);
            }
            else
            {
                newCopy[type] = handler;
                if (secondary != null) newCopy[secondary] = handler;
            }
            typeHandlers = newCopy;
        }
        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="command">The command to pack parameters for.</param>
        /// <param name="namePrefix">The name prefix for these parameters.</param>
        /// <param name="value">The parameter value can be an <see cref="IEnumerable{T}"/></param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static void PackListParameters(IDbCommand command, string namePrefix, object value)
        {
            // initially we tried TVP, however it performs quite poorly.
            // keep in mind SQL support up to 2000 params easily in sp_executesql, needing more is rare

            if (FeatureSupport.Get(command.Connection).Arrays)
            {
                var arrayParm = command.CreateParameter();
                arrayParm.Value = SanitizeParameterValue(value);
                arrayParm.ParameterName = namePrefix;
                command.Parameters.Add(arrayParm);
            }
            else
            {
                bool byPosition = ShouldPassByPosition(command.CommandText);
                var list = value as IEnumerable;
                var count = 0;
                bool isString = value is IEnumerable<string>;
                bool isDbString = value is IEnumerable<DbString>;
                DbType dbType = 0;

                int splitAt = SqlMapper.Settings.InListStringSplitCount;
                bool viaSplit = splitAt >= 0
                    && TryStringSplit(ref list, splitAt, namePrefix, command, byPosition);

                if (list != null && !viaSplit)
                {
                    object lastValue = null;
                    foreach (var item in list)
                    {
                        if (++count == 1) // first item: fetch some type info
                        {
                            if (item == null)
                            {
                                throw new NotSupportedException("The first item in a list-expansion cannot be null");
                            }
                            if (!isDbString)
                            {
                                dbType = LookupDbType(item.GetType(), "", true, out ITypeHandler handler);
                            }
                        }
                        var nextName = namePrefix + count.ToString();
                        if (isDbString && item is DbString)
                        {
                            var str = item as DbString;
                            str.AddParameter(command, nextName);
                        }
                        else
                        {
                            var listParam = command.CreateParameter();
                            listParam.ParameterName = nextName;
                            if (isString)
                            {
                                listParam.Size = DbString.DefaultLength;
                                if (item != null && ((string)item).Length > DbString.DefaultLength)
                                {
                                    listParam.Size = -1;
                                }
                            }

                            var tmp = listParam.Value = SanitizeParameterValue(item);
                            if (tmp != null && !(tmp is DBNull))
                                lastValue = tmp; // only interested in non-trivial values for padding

                            if (listParam.DbType != dbType)
                            {
                                listParam.DbType = dbType;
                            }
                            command.Parameters.Add(listParam);
                        }
                    }
                    if (Settings.PadListExpansions && !isDbString && lastValue != null)
                    {
                        int padCount = GetListPaddingExtraCount(count);
                        for (int i = 0; i < padCount; i++)
                        {
                            count++;
                            var padParam = command.CreateParameter();
                            padParam.ParameterName = namePrefix + count.ToString();
                            if (isString) padParam.Size = DbString.DefaultLength;
                            padParam.DbType = dbType;
                            padParam.Value = lastValue;
                            command.Parameters.Add(padParam);
                        }
                    }
                }

                if (viaSplit)
                {
                    // already done
                }
                else
                {
                    var regexIncludingUnknown = GetInListRegex(namePrefix, byPosition);
                    if (count == 0)
                    {
                        command.CommandText = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
                        {
                            var variableName = match.Groups[1].Value;
                            if (match.Groups[2].Success)
                            {
                                // looks like an optimize hint; leave it alone!
                                return match.Value;
                            }
                            else
                            {
                                return "(SELECT " + variableName + " WHERE 1 = 0)";
                            }
                        }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                        var dummyParam = command.CreateParameter();
                        dummyParam.ParameterName = namePrefix;
                        dummyParam.Value = DBNull.Value;
                        command.Parameters.Add(dummyParam);
                    }
                    else
                    {
                        command.CommandText = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
                        {
                            var variableName = match.Groups[1].Value;
                            if (match.Groups[2].Success)
                            {
                                // looks like an optimize hint; expand it
                                var suffix = match.Groups[2].Value;

                                var sb = GetStringBuilder().Append(variableName).Append(1).Append(suffix);
                                for (int i = 2; i <= count; i++)
                                {
                                    sb.Append(',').Append(variableName).Append(i).Append(suffix);
                                }
                                return sb.ToStringRecycle();
                            }
                            else
                            {
                                var sb = GetStringBuilder().Append('(').Append(variableName);
                                if (!byPosition) sb.Append(1); else sb.Append(namePrefix).Append(1).Append(variableName);
                                for (int i = 2; i <= count; i++)
                                {
                                    sb.Append(',').Append(variableName);
                                    if (!byPosition) sb.Append(i); else sb.Append(namePrefix).Append(i).Append(variableName);
                                }
                                return sb.Append(')').ToStringRecycle();
                            }
                        }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                    }
                }
            }
        }
        /// <summary>
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <typeparam name="T">The type to handle.</typeparam>
        /// <param name="handler">The handler for the type <typeparamref name="T"/>.</param>
        public static void AddTypeHandler<T>(TypeHandler<T> handler) => AddTypeHandlerImpl(typeof(T), handler, true);
        #endregion 公开方法
        #region // 外部类型
        /// <summary>
        /// 控制命令行为的附加状态标志
        /// </summary>
        [Flags]
        [EDisplay("命令特征")]
        public enum CmdFlags
        {
            /// <summary>
            /// 无
            /// </summary>
            [EDisplay("无")]
            None = 0,
            /// <summary>
            /// 数据是否应该在返回之前进行缓冲?
            /// </summary>
            [EDisplay("缓冲")]
            Buffered = 1,
            /// <summary>
            /// 异步查询可以流水线化吗?
            /// </summary>
            [EDisplay("流水线")]
            Pipelined = 2,
            /// <summary>
            /// 计划缓存应该被绕过吗?
            /// </summary>
            [EDisplay("无缓存")]
            NoCache = 4,
        }
        /// <summary>
        /// This class represents a SQL string, it can be used if you need to denote your parameter is a Char vs VarChar vs nVarChar vs nChar
        /// </summary>
        public sealed class DbString : ICustomQueryParameter
        {
            /// <summary>
            /// Default value for IsAnsi.
            /// </summary>
            public static bool IsAnsiDefault { get; set; }

            /// <summary>
            /// A value to set the default value of strings
            /// going through Dapper. Default is 4000, any value larger than this
            /// field will not have the default value applied.
            /// </summary>
            public const int DefaultLength = 4000;

            /// <summary>
            /// Create a new DbString
            /// </summary>
            public DbString()
            {
                Length = -1;
                IsAnsi = IsAnsiDefault;
            }
            /// <summary>
            /// Ansi vs Unicode 
            /// </summary>
            public bool IsAnsi { get; set; }
            /// <summary>
            /// Fixed length 
            /// </summary>
            public bool IsFixedLength { get; set; }
            /// <summary>
            /// Length of the string -1 for max
            /// </summary>
            public int Length { get; set; }
            /// <summary>
            /// The value of the string
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// Add the parameter to the command... internal use only
            /// </summary>
            /// <param name="command"></param>
            /// <param name="name"></param>
            public void AddParameter(IDbCommand command, string name)
            {
                if (IsFixedLength && Length == -1)
                {
                    throw new InvalidOperationException("If specifying IsFixedLength,  a Length must also be specified");
                }
                bool add = !command.Parameters.Contains(name);
                IDbDataParameter param;
                if (add)
                {
                    param = command.CreateParameter();
                    param.ParameterName = name;
                }
                else
                {
                    param = (IDbDataParameter)command.Parameters[name];
                }
#pragma warning disable 0618
                param.Value = SanitizeParameterValue(Value);
#pragma warning restore 0618
                if (Length == -1 && Value != null && Value.Length <= DefaultLength)
                {
                    param.Size = DefaultLength;
                }
                else
                {
                    param.Size = Length;
                }
                param.DbType = IsAnsi ? (IsFixedLength ? DbType.AnsiStringFixedLength : DbType.AnsiString) : (IsFixedLength ? DbType.StringFixedLength : DbType.String);
                if (add)
                {
                    command.Parameters.Add(param);
                }
            }
        }
        /// <summary>
        /// 命令定义
        /// <see cref="Dabber.CommandDefinition"/>
        /// </summary>
        public class CmdDefinition
        {
            /// <summary>
            /// 执行文本
            /// </summary>
            public string Text { get; }
            /// <summary>
            /// 参数值
            /// </summary>
            public object Args { get; }
            /// <summary>
            /// 事务
            /// </summary>
            public IDbTransaction Transaction { get; }
            /// <summary>
            /// 超时时间
            /// </summary>
            public int? Timeout { get; }
            /// <summary>
            /// 命令类型
            /// </summary>
            public CommandType Type { get; }
            /// <summary>
            /// 状态标识
            /// </summary>
            public CmdFlags Flags { get; }
            /// <summary>
            /// 是缓冲
            /// </summary>
            public bool IsBuffered => Flags.HasFlag(CmdFlags.Buffered);
            /// <summary>
            /// 是流水线
            /// </summary>
            public bool IsPipelined => Flags.HasFlag(CmdFlags.Pipelined);
            /// <summary>
            /// 需要添加缓存
            /// </summary>
            internal bool IsAddToCache => !Flags.HasFlag(CmdFlags.NoCache);
            /// <summary>
            /// 对于异步线程的取消令牌
            /// </summary>
            public CancellationToken CancelToken { get; }
            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="commandText">执行命令</param>
            /// <param name="parameters">参数</param>
            /// <param name="transaction">事务</param>
            /// <param name="commandTimeout">命令超时(秒)</param>
            /// <param name="commandType"><see cref="CommandType"/> 命令类型</param>
            /// <param name="flags">命令的行为状态标识</param>
            /// <param name="cancellationToken">取消令牌</param>
            public CmdDefinition(string commandText, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text, CmdFlags flags = CmdFlags.Buffered, CancellationToken cancellationToken = default)
            {
                Text = commandText;
                Args = parameters;
                Transaction = transaction;
                Timeout = commandTimeout;
                Type = commandType;
                Flags = flags;
                CancelToken = cancellationToken;
            }
        }
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

        /// <summary>
        /// Not intended for direct usage
        /// </summary>
        /// <typeparam name="T">The type to have a cache for.</typeparam>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static class TypeHandlerCache<T>
        {
            /// <summary>
            /// Not intended for direct usage.
            /// </summary>
            /// <param name="value">The object to parse.</param>
            [Obsolete(ObsoleteInternalUsageOnly, true)]
            public static T Parse(object value) => (T)handler.Parse(typeof(T), value);

            /// <summary>
            /// Not intended for direct usage.
            /// </summary>
            /// <param name="parameter">The parameter to set a value for.</param>
            /// <param name="value">The value to set.</param>
            [Obsolete(ObsoleteInternalUsageOnly, true)]
            public static void SetValue(IDbDataParameter parameter, object value) => handler.SetValue(parameter, value);

            internal static void SetHandler(ITypeHandler handler)
            {
#pragma warning disable 618
                TypeHandlerCache<T>.handler = handler;
#pragma warning restore 618
            }

            private static ITypeHandler handler;
        }
        /// <summary>
        /// Implement this interface to pass an arbitrary db specific parameter to Dapper
        /// </summary>
        public interface ICustomQueryParameter
        {
            /// <summary>
            /// Add the parameter needed to the command before it executes
            /// </summary>
            /// <param name="command">The raw command prior to execution</param>
            /// <param name="name">Parameter name</param>
            void AddParameter(IDbCommand command, string name);
        }
        /// <summary>
        /// 动态参数回调
        /// </summary>
        public interface IDynamicArgCallbacks : IDynamicArgs
        {
            /// <summary>
            /// Invoked when the command has executed
            /// </summary>
            void OnCompleted();
        }
        /// <summary>
        /// Extends IDynamicParameters providing by-name lookup of parameter values
        /// </summary>
        public interface IDynamicArgLookup : IDynamicArgs
        {
            /// <summary>
            /// Get the value of the specified parameter (return null if not found)
            /// </summary>
            /// <param name="name">The name of the parameter to get.</param>
            object this[string name] { get; }
        }
        /// <summary>
        /// 动态参数接口
        /// </summary>
        /// <see cref="Dabber.SqlMapper.IDynamicParameters"/>
        public interface IDynamicArgs
        {
            /// <summary>
            /// Add all the parameters needed to the command just before it executes
            /// </summary>
            /// <param name="command">The raw command prior to execution</param>
            /// <param name="identity">Information about the query</param>
            void AddParameters(IDbCommand command, CmdIdentity identity);
        }
        /// <summary>
        /// A bag of parameters that can be passed to the Dapper Query and Execute methods
        /// </summary>
        public class DynamicArgs : IDynamicArgs, IDynamicArgLookup, IDynamicArgCallbacks
        {
            internal const DbType EnumerableMultiParameter = (DbType)(-1);
            private static readonly Dictionary<CmdIdentity, Action<IDbCommand, object>> paramReaderCache = new Dictionary<CmdIdentity, Action<IDbCommand, object>>();
            private readonly Dictionary<string, ParamInfo> parameters = new Dictionary<string, ParamInfo>();
            private List<object> templates;

            object IDynamicArgLookup.this[string name] =>
                parameters.TryGetValue(name, out ParamInfo param) ? param.Value : null;

            /// <summary>
            /// construct a dynamic parameter bag
            /// </summary>
            public DynamicArgs()
            {
                RemoveUnused = true;
            }

            /// <summary>
            /// construct a dynamic parameter bag
            /// </summary>
            /// <param name="template">can be an anonymous type or a DynamicParameters bag</param>
            public DynamicArgs(object template)
            {
                RemoveUnused = true;
                AddDynamicParams(template);
            }

            /// <summary>
            /// Append a whole object full of params to the dynamic
            /// EG: AddDynamicParams(new {A = 1, B = 2}) // will add property A and B to the dynamic
            /// </summary>
            /// <param name="param"></param>
            public void AddDynamicParams(object param)
            {
                var obj = param;
                if (obj != null)
                {
                    if (obj is DynamicArgs subDynamic)
                    {
                        if (subDynamic.parameters != null)
                        {
                            foreach (var kvp in subDynamic.parameters)
                            {
                                parameters.Add(kvp.Key, kvp.Value);
                            }
                        }

                        if (subDynamic.templates != null)
                        {
                            templates ??= new List<object>();
                            foreach (var t in subDynamic.templates)
                            {
                                templates.Add(t);
                            }
                        }
                    }
                    else
                    {
                        if (obj is IEnumerable<KeyValuePair<string, object>> dictionary)
                        {
                            foreach (var kvp in dictionary)
                            {
                                Add(kvp.Key, kvp.Value, null, null, null);
                            }
                        }
                        else
                        {
                            templates ??= new List<object>();
                            templates.Add(obj);
                        }
                    }
                }
            }

            /// <summary>
            /// Add a parameter to this dynamic parameter list.
            /// </summary>
            /// <param name="name">The name of the parameter.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <param name="dbType">The type of the parameter.</param>
            /// <param name="direction">The in or out direction of the parameter.</param>
            /// <param name="size">The size of the parameter.</param>
            public void Add(string name, object value, DbType? dbType, ParameterDirection? direction, int? size)
            {
                parameters[Clean(name)] = new ParamInfo
                {
                    Name = name,
                    Value = value,
                    ParameterDirection = direction ?? ParameterDirection.Input,
                    DbType = dbType,
                    Size = size
                };
            }

            /// <summary>
            /// Add a parameter to this dynamic parameter list.
            /// </summary>
            /// <param name="name">The name of the parameter.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <param name="dbType">The type of the parameter.</param>
            /// <param name="direction">The in or out direction of the parameter.</param>
            /// <param name="size">The size of the parameter.</param>
            /// <param name="precision">The precision of the parameter.</param>
            /// <param name="scale">The scale of the parameter.</param>
            public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
            {
                parameters[Clean(name)] = new ParamInfo
                {
                    Name = name,
                    Value = value,
                    ParameterDirection = direction ?? ParameterDirection.Input,
                    DbType = dbType,
                    Size = size,
                    Precision = precision,
                    Scale = scale
                };
            }

            private static string Clean(string name)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    switch (name[0])
                    {
                        case '@':
                        case ':':
                        case '?':
                            return name.Substring(1);
                    }
                }
                return name;
            }

            void IDynamicArgs.AddParameters(IDbCommand command, CmdIdentity identity)
            {
                AddParameters(command, identity);
            }

            /// <summary>
            /// If true, the command-text is inspected and only values that are clearly used are included on the connection
            /// </summary>
            public bool RemoveUnused { get; set; }

            /// <summary>
            /// Add all the parameters needed to the command just before it executes
            /// </summary>
            /// <param name="command">The raw command prior to execution</param>
            /// <param name="identity">Information about the query</param>
            protected void AddParameters(IDbCommand command, CmdIdentity identity)
            {
                var literals = GetLiteralTokens(identity.sql);

                if (templates != null)
                {
                    foreach (var template in templates)
                    {
                        var newIdent = identity.ForDynamicParameters(template.GetType());
                        Action<IDbCommand, object> appender;

                        lock (paramReaderCache)
                        {
                            if (!paramReaderCache.TryGetValue(newIdent, out appender))
                            {
                                appender = CreateParamInfoGenerator(newIdent, true, RemoveUnused, literals);
                                paramReaderCache[newIdent] = appender;
                            }
                        }

                        appender(command, template);
                    }

                    // The parameters were added to the command, but not the
                    // DynamicParameters until now.
                    foreach (IDbDataParameter param in command.Parameters)
                    {
                        // If someone makes a DynamicParameters with a template,
                        // then explicitly adds a parameter of a matching name,
                        // it will already exist in 'parameters'.
                        if (!parameters.ContainsKey(param.ParameterName))
                        {
                            parameters.Add(param.ParameterName, new ParamInfo
                            {
                                AttachedParam = param,
                                CameFromTemplate = true,
                                DbType = param.DbType,
                                Name = param.ParameterName,
                                ParameterDirection = param.Direction,
                                Size = param.Size,
                                Value = param.Value
                            });
                        }
                    }

                    // Now that the parameters are added to the command, let's place our output callbacks
                    var tmp = outputCallbacks;
                    if (tmp != null)
                    {
                        foreach (var generator in tmp)
                        {
                            generator();
                        }
                    }
                }

                foreach (var param in parameters.Values)
                {
                    if (param.CameFromTemplate) continue;

                    var dbType = param.DbType;
                    var val = param.Value;
                    string name = Clean(param.Name);
                    var isCustomQueryParameter = val is ICustomQueryParameter;

                    ITypeHandler handler = null;
                    if (dbType == null && val != null && !isCustomQueryParameter)
                    {
#pragma warning disable 618
                        dbType = LookupDbType(val.GetType(), name, true, out handler);
#pragma warning disable 618
                    }
                    if (isCustomQueryParameter)
                    {
                        ((ICustomQueryParameter)val).AddParameter(command, name);
                    }
                    else if (dbType == EnumerableMultiParameter)
                    {
#pragma warning disable 612, 618
                        PackListParameters(command, name, val);
#pragma warning restore 612, 618
                    }
                    else
                    {
                        bool add = !command.Parameters.Contains(name);
                        IDbDataParameter p;
                        if (add)
                        {
                            p = command.CreateParameter();
                            p.ParameterName = name;
                        }
                        else
                        {
                            p = (IDbDataParameter)command.Parameters[name];
                        }

                        p.Direction = param.ParameterDirection;
                        if (handler == null)
                        {
#pragma warning disable 0618
                            p.Value = SqlMapper.SanitizeParameterValue(val);
#pragma warning restore 0618
                            if (dbType != null && p.DbType != dbType)
                            {
                                p.DbType = dbType.Value;
                            }
                            var s = val as string;
                            if (s?.Length <= DbString.DefaultLength)
                            {
                                p.Size = DbString.DefaultLength;
                            }
                            if (param.Size != null) p.Size = param.Size.Value;
                            if (param.Precision != null) p.Precision = param.Precision.Value;
                            if (param.Scale != null) p.Scale = param.Scale.Value;
                        }
                        else
                        {
                            if (dbType != null) p.DbType = dbType.Value;
                            if (param.Size != null) p.Size = param.Size.Value;
                            if (param.Precision != null) p.Precision = param.Precision.Value;
                            if (param.Scale != null) p.Scale = param.Scale.Value;
                            handler.SetValue(p, val ?? DBNull.Value);
                        }

                        if (add)
                        {
                            command.Parameters.Add(p);
                        }
                        param.AttachedParam = p;
                    }
                }

                // note: most non-privileged implementations would use: this.ReplaceLiterals(command);
                if (literals.Count != 0) SqlMapper.ReplaceLiterals(this, command, literals);
            }

            /// <summary>
            /// All the names of the param in the bag, use Get to yank them out
            /// </summary>
            public IEnumerable<string> ParameterNames => parameters.Select(p => p.Key);

            /// <summary>
            /// Get the value of a parameter
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="name"></param>
            /// <returns>The value, note DBNull.Value is not returned, instead the value is returned as null</returns>
            public T Get<T>(string name)
            {
                var paramInfo = parameters[Clean(name)];
                var attachedParam = paramInfo.AttachedParam;
                object val = attachedParam == null ? paramInfo.Value : attachedParam.Value;
                if (val == DBNull.Value)
                {
                    if (default(T) != null)
                    {
                        throw new ApplicationException("Attempting to cast a DBNull to a non nullable type! Note that out/return parameters will not have updated values until the data stream completes (after the 'foreach' for Query(..., buffered: false), or after the GridReader has been disposed for QueryMultiple)");
                    }
                    return default;
                }
                return (T)val;
            }

            /// <summary>
            /// Allows you to automatically populate a target property/field from output parameters. It actually
            /// creates an InputOutput parameter, so you can still pass data in.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="target">The object whose property/field you wish to populate.</param>
            /// <param name="expression">A MemberExpression targeting a property/field of the target (or descendant thereof.)</param>
            /// <param name="dbType"></param>
            /// <param name="size">The size to set on the parameter. Defaults to 0, or DbString.DefaultLength in case of strings.</param>
            /// <returns>The DynamicParameters instance</returns>
            public DynamicArgs Output<T>(T target, Expression<Func<T, object>> expression, DbType? dbType = null, int? size = null)
            {
                static void ThrowInvalidChain()
                    => throw new InvalidOperationException($"Expression must be a property/field chain off of a(n) {typeof(T).Name} instance");

                // Is it even a MemberExpression?
#pragma warning disable IDE0019 // Use pattern matching - already complex enough
                var lastMemberAccess = expression.Body as MemberExpression;
#pragma warning restore IDE0019 // Use pattern matching

                if (lastMemberAccess == null
                    || (!(lastMemberAccess.Member is PropertyInfo)
                        && !(lastMemberAccess.Member is FieldInfo)))
                {
                    if (expression.Body.NodeType == ExpressionType.Convert
                        && expression.Body.Type == typeof(object)
                        && ((UnaryExpression)expression.Body).Operand is MemberExpression member)
                    {
                        // It's got to be unboxed
                        lastMemberAccess = member;
                    }
                    else
                    {
                        ThrowInvalidChain();
                    }
                }

                // Does the chain consist of MemberExpressions leading to a ParameterExpression of type T?
                MemberExpression diving = lastMemberAccess;
                // Retain a list of member names and the member expressions so we can rebuild the chain.
                List<string> names = new List<string>();
                List<MemberExpression> chain = new List<MemberExpression>();

                do
                {
                    // Insert the names in the right order so expression
                    // "Post.Author.Name" becomes parameter "PostAuthorName"
                    names.Insert(0, diving?.Member.Name);
                    chain.Insert(0, diving);

#pragma warning disable IDE0019 // use pattern matching; this is fine!
                    var constant = diving?.Expression as ParameterExpression;
                    diving = diving?.Expression as MemberExpression;
#pragma warning restore IDE0019 // use pattern matching

                    if (constant is object && constant.Type == typeof(T))
                    {
                        break;
                    }
                    else if (diving == null
                        || (!(diving.Member is PropertyInfo)
                            && !(diving.Member is FieldInfo)))
                    {
                        ThrowInvalidChain();
                    }
                }
                while (diving != null);

                var dynamicParamName = string.Concat(names.ToArray());

                // Before we get all emitty...
                var lookup = string.Join("|", names.ToArray());

                var cache = CachedOutputSetters<T>.Cache;
                var setter = (Action<object, DynamicArgs>)cache[lookup];
                if (setter != null) goto MAKECALLBACK;

                // Come on let's build a method, let's build it, let's build it now!
                var dm = new DynamicMethod("ExpressionParam" + Guid.NewGuid().ToString(), null, new[] { typeof(object), GetType() }, true);
                var il = dm.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // [object]
                il.Emit(OpCodes.Castclass, typeof(T));    // [T]

                // Count - 1 to skip the last member access
                for (var i = 0; i < chain.Count - 1; i++)
                {
                    var member = chain[i].Member;

                    if (member is PropertyInfo info)
                    {
                        var get = info.GetGetMethod(true);
                        il.Emit(OpCodes.Callvirt, get); // [Member{i}]
                    }
                    else // Else it must be a field!
                    {
                        il.Emit(OpCodes.Ldfld, (FieldInfo)member); // [Member{i}]
                    }
                }

                var paramGetter = GetType().GetMethod("Get", new Type[] { typeof(string) }).MakeGenericMethod(lastMemberAccess.Type);

                il.Emit(OpCodes.Ldarg_1); // [target] [DynamicParameters]
                il.Emit(OpCodes.Ldstr, dynamicParamName); // [target] [DynamicParameters] [ParamName]
                il.Emit(OpCodes.Callvirt, paramGetter); // [target] [value], it's already typed thanks to generic method

                // GET READY
                var lastMember = lastMemberAccess.Member;
                if (lastMember is PropertyInfo property)
                {
                    var set = property.GetSetMethod(true);
                    il.Emit(OpCodes.Callvirt, set); // SET
                }
                else
                {
                    il.Emit(OpCodes.Stfld, (FieldInfo)lastMember); // SET
                }

                il.Emit(OpCodes.Ret); // GO

                setter = (Action<object, DynamicArgs>)dm.CreateDelegate(typeof(Action<object, DynamicArgs>));
                lock (cache)
                {
                    cache[lookup] = setter;
                }

            // Queue the preparation to be fired off when adding parameters to the DbCommand
            MAKECALLBACK:
                (outputCallbacks ??= new List<Action>()).Add(() =>
                {
                    // Finally, prep the parameter and attach the callback to it
                    var targetMemberType = lastMemberAccess?.Type;
                    int sizeToSet = (!size.HasValue && targetMemberType == typeof(string)) ? DbString.DefaultLength : size ?? 0;

                    if (parameters.TryGetValue(dynamicParamName, out ParamInfo parameter))
                    {
                        parameter.ParameterDirection = parameter.AttachedParam.Direction = ParameterDirection.InputOutput;

                        if (parameter.AttachedParam.Size == 0)
                        {
                            parameter.Size = parameter.AttachedParam.Size = sizeToSet;
                        }
                    }
                    else
                    {
                        dbType = (!dbType.HasValue)
#pragma warning disable 618
                    ? SqlMapper.LookupDbType(targetMemberType, targetMemberType?.Name, true, out SqlMapper.ITypeHandler handler)
#pragma warning restore 618
                    : dbType;

                        // CameFromTemplate property would not apply here because this new param
                        // Still needs to be added to the command
                        Add(dynamicParamName, expression.Compile().Invoke(target), null, ParameterDirection.InputOutput, sizeToSet);
                    }

                    parameter = parameters[dynamicParamName];
                    parameter.OutputCallback = setter;
                    parameter.OutputTarget = target;
                });

                return this;
            }

            private List<Action> outputCallbacks;

            void IDynamicArgCallbacks.OnCompleted()
            {
                foreach (var param in from p in parameters select p.Value)
                {
                    param.OutputCallback?.Invoke(param.OutputTarget, this);
                }
            }
            #region // 内部类
            private sealed class ParamInfo
            {
                public string Name { get; set; }
                public object Value { get; set; }
                public ParameterDirection ParameterDirection { get; set; }
                public DbType? DbType { get; set; }
                public int? Size { get; set; }
                public IDbDataParameter AttachedParam { get; set; }
                internal Action<object, DynamicArgs> OutputCallback { get; set; }
                internal object OutputTarget { get; set; }
                internal bool CameFromTemplate { get; set; }

                public byte? Precision { get; set; }
                public byte? Scale { get; set; }
            }
            // The type here is used to differentiate the cache by type via generics
            // ReSharper disable once UnusedTypeParameter
            internal static class CachedOutputSetters<T>
            {
                // Intentional, abusing generics to get our cache splits
                // ReSharper disable once StaticMemberInGenericType
                public static readonly Hashtable Cache = new Hashtable();
            }
            #endregion
        }
        /// <summary>
        /// 缓存查询的标识，用于可扩展性
        /// </summary>
        public class CmdIdentity : IEquatable<CmdIdentity>
        {
            internal static DontMap DontMapper => new DontMap();
            internal Type[] _types = Type.EmptyTypes;
            internal virtual int TypeCount => _types.Length;
            internal virtual Type GetType(int index) => _types[index];
            internal CmdIdentity ForGrid<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(Type primaryType, int gridIndex) =>
                new CmdIdentity(sql, commandType, connectionString, primaryType, parametersType, FillterTypes<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(), gridIndex);

            internal CmdIdentity ForGrid(Type primaryType, int gridIndex) =>
                new CmdIdentity(sql, commandType, connectionString, primaryType, parametersType, 0, gridIndex);

            internal CmdIdentity ForGrid(Type primaryType, Type[] otherTypes, int gridIndex) =>
                (otherTypes == null || otherTypes.Length == 0)
                ? new CmdIdentity(sql, commandType, connectionString, primaryType, parametersType, 0, gridIndex)
                : new CmdIdentity(sql, commandType, connectionString, primaryType, parametersType, otherTypes, gridIndex);

            /// <summary>
            /// Create an identity for use with DynamicParameters, internal use only.
            /// </summary>
            /// <param name="type">The parameters type to create an <see cref="CmdIdentity"/> for.</param>
            /// <returns></returns>
            public CmdIdentity ForDynamicParameters(Type type) =>
                new CmdIdentity(sql, commandType, connectionString, this.type, type, 0, -1);

            internal CmdIdentity(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType)
                : this(sql, commandType, connection.ConnectionString, type, parametersType, 0, 0) { /* base call */ }

            internal CmdIdentity(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, Type[] otherTypes, int gridIndex = 0)
                : this(sql, commandType, connectionString, type, parametersType, HashTypes(otherTypes), gridIndex)
            {
                _types = otherTypes ?? Type.EmptyTypes;
            }
            internal CmdIdentity(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType, Type[] otherTypes, int gridIndex = 0)
                : this(sql, commandType, connection.ConnectionString, type, parametersType, HashTypes(otherTypes), gridIndex)
            {
                _types = otherTypes ?? Type.EmptyTypes;
            }
            private protected CmdIdentity(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, int otherTypesHash, int gridIndex)
            {
                this.sql = sql;
                this.commandType = commandType;
                this.connectionString = connectionString;
                this.type = type;
                this.parametersType = parametersType;
                this.gridIndex = gridIndex;
                unchecked
                {
                    hashCode = 17; // we *know* we are using this in a dictionary, so pre-compute this
                    hashCode = (hashCode * 23) + commandType.GetHashCode();
                    hashCode = (hashCode * 23) + gridIndex.GetHashCode();
                    hashCode = (hashCode * 23) + (sql?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 23) + (type?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 23) + otherTypesHash;
                    hashCode = (hashCode * 23) + (connectionString == null ? 0 : _connStringComparer.GetHashCode(connectionString));
                    hashCode = (hashCode * 23) + (parametersType?.GetHashCode() ?? 0);
                }
            }

            /// <summary>
            /// Whether this <see cref="CmdIdentity"/> equals another.
            /// </summary>
            /// <param name="obj">The other <see cref="object"/> to compare to.</param>
            public override bool Equals(object obj) => Equals(obj as CmdIdentity);

            /// <summary>
            /// The raw SQL command.
            /// </summary>
            public readonly string sql;

            /// <summary>
            /// The SQL command type.
            /// </summary>
            public readonly CommandType? commandType;

            /// <summary>
            /// The hash code of this Identity.
            /// </summary>
            public readonly int hashCode;

            /// <summary>
            /// The grid index (position in the reader) of this Identity.
            /// </summary>
            public readonly int gridIndex;

            /// <summary>
            /// This <see cref="Type"/> of this Identity.
            /// </summary>
            public readonly Type type;

            /// <summary>
            /// The connection string for this Identity.
            /// </summary>
            public readonly string connectionString;

            /// <summary>
            /// The type of the parameters object for this Identity.
            /// </summary>
            public readonly Type parametersType;

            /// <summary>
            /// Gets the hash code for this identity.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() => hashCode;

            /// <summary>
            /// See object.ToString()
            /// </summary>
            public override string ToString() => sql;

            /// <summary>
            /// Compare 2 Identity objects
            /// </summary>
            /// <param name="other">The other <see cref="CmdIdentity"/> object to compare.</param>
            /// <returns>Whether the two are equal</returns>
            public bool Equals(CmdIdentity other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (other is null) return false;

                int typeCount;
                return gridIndex == other.gridIndex
                    && type == other.type
                    && sql == other.sql
                    && commandType == other.commandType
                    && _connStringComparer.Equals(connectionString, other.connectionString)
                    && parametersType == other.parametersType
                    && (typeCount = TypeCount) == other.TypeCount
                    && (typeCount == 0 || TypesEqual(this, other, typeCount));
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static bool TypesEqual(CmdIdentity x, CmdIdentity y, int count)
            {
                if (y.TypeCount != count) return false;
                for (int i = 0; i < count; i++)
                {
                    if (x.GetType(i) != y.GetType(i))
                        return false;
                }
                return true;
            }
            internal static int HashTypes(Type[] types)
            {
                var hashCode = 0;
                if (types != null)
                {
                    foreach (var t in types)
                    {
                        hashCode = (hashCode * 23) + (t?.GetHashCode() ?? 0);
                    }
                }
                return hashCode;
            }
            internal static Type[] FillterTypes<T1, T2, T3, T4, T5, T6, T7>()
            {
                List<Type> types = new List<Type>();
                bool Map<T>()
                {
                    if (typeof(T) != typeof(DontMap))
                    {
                        types.Add(typeof(T));
                        return true;
                    }
                    return false;
                }
                _ = Map<T1>() && Map<T2>() && Map<T3>() && Map<T4>() && Map<T5>() && Map<T6>() && Map<T7>();
                return types.ToArray();
            }
            /// <summary>
            /// 不映射类
            /// </summary>
            internal class DontMap { }
        }
        #endregion 外部类型
        #region // 内部属性
        internal static Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>(37)
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(object)] = DbType.Object
        };
        #endregion 内部属性
        #region // 内部方法
        internal static int GetColumnHash(IDataReader reader, int startBound = 0, int length = -1)
        {
            unchecked
            {
                int max = length < 0 ? reader.FieldCount : startBound + length;
                int hash = (-37 * startBound) + max;
                for (int i = startBound; i < max; i++)
                {
                    object tmp = reader.GetName(i);
                    hash = (-79 * ((hash * 31) + (tmp?.GetHashCode() ?? 0))) + (reader.GetFieldType(i)?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
        internal static IList<LiteralToken> GetLiteralTokens(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return LiteralToken.None;
            if (!literalTokens.IsMatch(sql)) return LiteralToken.None;

            var matches = literalTokens.Matches(sql);
            var found = new HashSet<string>(StringComparer.Ordinal);
            List<LiteralToken> list = new List<LiteralToken>(matches.Count);
            foreach (Match match in matches)
            {
                string token = match.Value;
                if (found.Add(match.Value))
                {
                    list.Add(new LiteralToken(token, match.Groups[1].Value));
                }
            }
            return list.Count == 0 ? LiteralToken.None : list;
        }
        internal static Action<IDbCommand, object> CreateParamInfoGenerator(CmdIdentity identity, bool checkForDuplicates, bool removeUnused, IList<LiteralToken> literals)
        {
            Type type = identity.parametersType;

            if (IsValueTuple(type))
            {
                throw new NotSupportedException("ValueTuple should not be used for parameters - the language-level names are not available to use as parameter names, and it adds unnecessary boxing");
            }

            bool filterParams = false;
            if (removeUnused && identity.commandType.GetValueOrDefault(CommandType.Text) == CommandType.Text)
            {
                filterParams = !smellsLikeOleDb.IsMatch(identity.sql);
            }
            var dm = new DynamicMethod("ParamInfo" + Guid.NewGuid().ToString(), null, new[] { typeof(IDbCommand), typeof(object) }, type, true);

            var il = dm.GetILGenerator();

            bool isStruct = type.IsValueType;
            var _sizeLocal = (LocalBuilder)null;
            LocalBuilder GetSizeLocal() => _sizeLocal ??= il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldarg_1); // stack is now [untyped-param]

            LocalBuilder typedParameterLocal;
            if (isStruct)
            {
                typedParameterLocal = il.DeclareLocal(type.MakeByRefType()); // note: ref-local
                il.Emit(OpCodes.Unbox, type); // stack is now [typed-param]
            }
            else
            {
                typedParameterLocal = il.DeclareLocal(type);
                il.Emit(OpCodes.Castclass, type); // stack is now [typed-param]
            }
            il.Emit(OpCodes.Stloc, typedParameterLocal); // stack is now empty

            il.Emit(OpCodes.Ldarg_0); // stack is now [command]
            il.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetProperty(nameof(IDbCommand.Parameters)).GetGetMethod(), null); // stack is now [parameters]

            var allTypeProps = type.GetProperties();
            var propsList = new List<PropertyInfo>(allTypeProps.Length);
            for (int i = 0; i < allTypeProps.Length; ++i)
            {
                var p = allTypeProps[i];
                if (p.GetIndexParameters().Length == 0)
                    propsList.Add(p);
            }

            var ctors = type.GetConstructors();
            ParameterInfo[] ctorParams;
            IEnumerable<PropertyInfo> props = null;
            // try to detect tuple patterns, e.g. anon-types, and use that to choose the order
            // otherwise: alphabetical
            if (ctors.Length == 1 && propsList.Count == (ctorParams = ctors[0].GetParameters()).Length)
            {
                // check if reflection was kind enough to put everything in the right order for us
                bool ok = true;
                for (int i = 0; i < propsList.Count; i++)
                {
                    if (!string.Equals(propsList[i].Name, ctorParams[i].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    // pre-sorted; the reflection gods have smiled upon us
                    props = propsList;
                }
                else
                { // might still all be accounted for; check the hard way
                    var positionByName = new Dictionary<string, int>(ctorParams.Length, StringComparer.OrdinalIgnoreCase);
                    foreach (var param in ctorParams)
                    {
                        positionByName[param.Name] = param.Position;
                    }
                    if (positionByName.Count == propsList.Count)
                    {
                        int[] positions = new int[propsList.Count];
                        ok = true;
                        for (int i = 0; i < propsList.Count; i++)
                        {
                            if (!positionByName.TryGetValue(propsList[i].Name, out int pos))
                            {
                                ok = false;
                                break;
                            }
                            positions[i] = pos;
                        }
                        if (ok)
                        {
                            props = propsList.ToArray();
                            Array.Sort(positions, (PropertyInfo[])props);
                        }
                    }
                }
            }
            if (props == null)
            {
                propsList.Sort(new PropertyInfoByNameComparer());
                props = propsList;
            }
            if (filterParams)
            {
                props = FilterParameters(props, identity.sql);
            }

            var callOpCode = isStruct ? OpCodes.Call : OpCodes.Callvirt;
            foreach (var prop in props)
            {
                if (typeof(ICustomQueryParameter).IsAssignableFrom(prop.PropertyType))
                {
                    il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [typed-param]
                    il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [custom]
                    il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [custom] [command]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [custom] [command] [name]
                    il.EmitCall(OpCodes.Callvirt, prop.PropertyType.GetMethod(nameof(ICustomQueryParameter.AddParameter)), null); // stack is now [parameters]
                    continue;
                }
#pragma warning disable 618
                DbType dbType = LookupDbType(prop.PropertyType, prop.Name, true, out ITypeHandler handler);
#pragma warning restore 618
                if (dbType == DynamicParameters.EnumerableMultiParameter)
                {
                    // this actually represents special handling for list types;
                    il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [command]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [command] [name]
                    il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [command] [name] [typed-param]
                    il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [command] [name] [typed-value]
                    if (prop.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, prop.PropertyType); // stack is [parameters] [command] [name] [boxed-value]
                    }
                    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.PackListParameters)), null); // stack is [parameters]
                    continue;
                }
                il.Emit(OpCodes.Dup); // stack is now [parameters] [parameters]

                il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [parameters] [command]

                if (checkForDuplicates)
                {
                    // need to be a little careful about adding; use a utility method
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [parameters] [command] [name]
                    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.FindOrAddParameter)), null); // stack is [parameters] [parameter]
                }
                else
                {
                    // no risk of duplicates; just blindly add
                    il.EmitCall(OpCodes.Callvirt, typeof(IDbCommand).GetMethod(nameof(IDbCommand.CreateParameter)), null);// stack is now [parameters] [parameters] [parameter]

                    il.Emit(OpCodes.Dup);// stack is now [parameters] [parameters] [parameter] [parameter]
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [parameters] [parameter] [parameter] [name]
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.ParameterName)).GetSetMethod(), null);// stack is now [parameters] [parameters] [parameter]
                }
                if (dbType != DbType.Time && handler == null) // https://connect.microsoft.com/VisualStudio/feedback/details/381934/sqlparameter-dbtype-dbtype-time-sets-the-parameter-to-sqldbtype-datetime-instead-of-sqldbtype-time
                {
                    il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                    if (dbType == DbType.Object && prop.PropertyType == typeof(object)) // includes dynamic
                    {
                        // look it up from the param value
                        il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [typed-param]
                        il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [[parameters]] [parameter] [parameter] [object-value]
                        il.Emit(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.GetDbType), BindingFlags.Static | BindingFlags.Public)); // stack is now [parameters] [[parameters]] [parameter] [parameter] [db-type]
                    }
                    else
                    {
                        // constant value; nice and simple
                        EmitInt32(il, (int)dbType);// stack is now [parameters] [[parameters]] [parameter] [parameter] [db-type]
                    }
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.DbType)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]
                }

                il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                EmitInt32(il, (int)ParameterDirection.Input);// stack is now [parameters] [[parameters]] [parameter] [parameter] [dir]
                il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.Direction)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]

                il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                il.Emit(OpCodes.Ldloc, typedParameterLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [typed-param]
                il.Emit(callOpCode, prop.GetGetMethod()); // stack is [parameters] [[parameters]] [parameter] [parameter] [typed-value]
                bool checkForNull;
                if (prop.PropertyType.IsValueType)
                {
                    var propType = prop.PropertyType;
                    var nullType = Nullable.GetUnderlyingType(propType);
                    bool callSanitize = false;

                    if ((nullType ?? propType).IsEnum)
                    {
                        if (nullType != null)
                        {
                            // Nullable<SomeEnum>; we want to box as the underlying type; that's just *hard*; for
                            // simplicity, box as Nullable<SomeEnum> and call SanitizeParameterValue
                            callSanitize = checkForNull = true;
                        }
                        else
                        {
                            checkForNull = false;
                            // non-nullable enum; we can do that! just box to the wrong type! (no, really)
                            switch (Type.GetTypeCode(Enum.GetUnderlyingType(propType)))
                            {
                                case TypeCode.Byte: propType = typeof(byte); break;
                                case TypeCode.SByte: propType = typeof(sbyte); break;
                                case TypeCode.Int16: propType = typeof(short); break;
                                case TypeCode.Int32: propType = typeof(int); break;
                                case TypeCode.Int64: propType = typeof(long); break;
                                case TypeCode.UInt16: propType = typeof(ushort); break;
                                case TypeCode.UInt32: propType = typeof(uint); break;
                                case TypeCode.UInt64: propType = typeof(ulong); break;
                            }
                        }
                    }
                    else
                    {
                        checkForNull = nullType != null;
                    }
                    il.Emit(OpCodes.Box, propType); // stack is [parameters] [[parameters]] [parameter] [parameter] [boxed-value]
                    if (callSanitize)
                    {
                        checkForNull = false; // handled by sanitize
                        il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SanitizeParameterValue)), null);
                        // stack is [parameters] [[parameters]] [parameter] [parameter] [boxed-value]
                    }
                }
                else
                {
                    checkForNull = true; // if not a value-type, need to check
                }
                if (checkForNull)
                {
                    // relative stack: [boxed value]
                    il.Emit(OpCodes.Dup);// relative stack: [boxed value] [boxed value]
                    Label notNull = il.DefineLabel();
                    Label? allDone = (dbType == DbType.String || dbType == DbType.AnsiString) ? il.DefineLabel() : (Label?)null;
                    il.Emit(OpCodes.Brtrue_S, notNull);
                    // relative stack [boxed value = null]
                    il.Emit(OpCodes.Pop); // relative stack empty
                    il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField(nameof(DBNull.Value))); // relative stack [DBNull]
                    if (dbType == DbType.String || dbType == DbType.AnsiString)
                    {
                        EmitInt32(il, 0);
                        il.Emit(OpCodes.Stloc, GetSizeLocal());
                    }
                    if (allDone != null) il.Emit(OpCodes.Br_S, allDone.Value);
                    il.MarkLabel(notNull);
                    if (prop.PropertyType == typeof(string))
                    {
                        il.Emit(OpCodes.Dup); // [string] [string]
                        il.EmitCall(OpCodes.Callvirt, typeof(string).GetProperty(nameof(string.Length)).GetGetMethod(), null); // [string] [length]
                        EmitInt32(il, DbString.DefaultLength); // [string] [length] [4000]
                        il.Emit(OpCodes.Cgt); // [string] [0 or 1]
                        Label isLong = il.DefineLabel(), lenDone = il.DefineLabel();
                        il.Emit(OpCodes.Brtrue_S, isLong);
                        EmitInt32(il, DbString.DefaultLength); // [string] [4000]
                        il.Emit(OpCodes.Br_S, lenDone);
                        il.MarkLabel(isLong);
                        EmitInt32(il, -1); // [string] [-1]
                        il.MarkLabel(lenDone);
                        il.Emit(OpCodes.Stloc, GetSizeLocal()); // [string]
                    }
                    if (prop.PropertyType.FullName == LinqBinary)
                    {
                        il.EmitCall(OpCodes.Callvirt, prop.PropertyType.GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance), null);
                    }
                    if (allDone != null) il.MarkLabel(allDone.Value);
                    // relative stack [boxed value or DBNull]
                }

                if (handler != null)
                {
#pragma warning disable 618
                    il.Emit(OpCodes.Call, typeof(TypeHandlerCache<>).MakeGenericType(prop.PropertyType).GetMethod(nameof(TypeHandlerCache<int>.SetValue))); // stack is now [parameters] [[parameters]] [parameter]
#pragma warning restore 618
                }
                else
                {
                    il.EmitCall(OpCodes.Callvirt, typeof(IDataParameter).GetProperty(nameof(IDataParameter.Value)).GetSetMethod(), null);// stack is now [parameters] [[parameters]] [parameter]
                }

                if (prop.PropertyType == typeof(string))
                {
                    var endOfSize = il.DefineLabel();
                    var sizeLocal = GetSizeLocal();
                    // don't set if 0
                    il.Emit(OpCodes.Ldloc, sizeLocal); // [parameters] [[parameters]] [parameter] [size]
                    il.Emit(OpCodes.Brfalse_S, endOfSize); // [parameters] [[parameters]] [parameter]

                    il.Emit(OpCodes.Dup);// stack is now [parameters] [[parameters]] [parameter] [parameter]
                    il.Emit(OpCodes.Ldloc, sizeLocal); // stack is now [parameters] [[parameters]] [parameter] [parameter] [size]
                    il.EmitCall(OpCodes.Callvirt, typeof(IDbDataParameter).GetProperty(nameof(IDbDataParameter.Size)).GetSetMethod(), null); // stack is now [parameters] [[parameters]] [parameter]

                    il.MarkLabel(endOfSize);
                }
                if (checkForDuplicates)
                {
                    // stack is now [parameters] [parameter]
                    il.Emit(OpCodes.Pop); // don't need parameter any more
                }
                else
                {
                    // stack is now [parameters] [parameters] [parameter]
                    // blindly add
                    il.EmitCall(OpCodes.Callvirt, typeof(IList).GetMethod(nameof(IList.Add)), null); // stack is now [parameters]
                    il.Emit(OpCodes.Pop); // IList.Add returns the new index (int); we don't care
                }
            }

            // stack is currently [parameters]
            il.Emit(OpCodes.Pop); // stack is now empty

            if (literals.Count != 0 && propsList != null)
            {
                il.Emit(OpCodes.Ldarg_0); // command
                il.Emit(OpCodes.Ldarg_0); // command, command
                var cmdText = typeof(IDbCommand).GetProperty(nameof(IDbCommand.CommandText));
                il.EmitCall(OpCodes.Callvirt, cmdText.GetGetMethod(), null); // command, sql
                Dictionary<Type, LocalBuilder> locals = null;
                LocalBuilder local = null;
                foreach (var literal in literals)
                {
                    // find the best member, preferring case-sensitive
                    PropertyInfo exact = null, fallback = null;
                    string huntName = literal.Member;
                    for (int i = 0; i < propsList.Count; i++)
                    {
                        string thisName = propsList[i].Name;
                        if (string.Equals(thisName, huntName, StringComparison.OrdinalIgnoreCase))
                        {
                            fallback = propsList[i];
                            if (string.Equals(thisName, huntName, StringComparison.Ordinal))
                            {
                                exact = fallback;
                                break;
                            }
                        }
                    }
                    var prop = exact ?? fallback;

                    if (prop != null)
                    {
                        il.Emit(OpCodes.Ldstr, literal.Token);
                        il.Emit(OpCodes.Ldloc, typedParameterLocal); // command, sql, typed parameter
                        il.EmitCall(callOpCode, prop.GetGetMethod(), null); // command, sql, typed value
                        Type propType = prop.PropertyType;
                        var typeCode = Type.GetTypeCode(propType);
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                Label ifTrue = il.DefineLabel(), allDone = il.DefineLabel();
                                il.Emit(OpCodes.Brtrue_S, ifTrue);
                                il.Emit(OpCodes.Ldstr, "0");
                                il.Emit(OpCodes.Br_S, allDone);
                                il.MarkLabel(ifTrue);
                                il.Emit(OpCodes.Ldstr, "1");
                                il.MarkLabel(allDone);
                                break;
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.UInt16:
                            case TypeCode.Int16:
                            case TypeCode.UInt32:
                            case TypeCode.Int32:
                            case TypeCode.UInt64:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                // need to stloc, ldloca, call
                                // re-use existing locals (both the last known, and via a dictionary)
                                var convert = GetToString(typeCode);
                                if (local == null || local.LocalType != propType)
                                {
                                    if (locals == null)
                                    {
                                        locals = new Dictionary<Type, LocalBuilder>();
                                        local = null;
                                    }
                                    else
                                    {
                                        if (!locals.TryGetValue(propType, out local)) local = null;
                                    }
                                    if (local == null)
                                    {
                                        local = il.DeclareLocal(propType);
                                        locals.Add(propType, local);
                                    }
                                }
                                il.Emit(OpCodes.Stloc, local); // command, sql
                                il.Emit(OpCodes.Ldloca, local); // command, sql, ref-to-value
                                il.EmitCall(OpCodes.Call, InvariantCulture, null); // command, sql, ref-to-value, culture
                                il.EmitCall(OpCodes.Call, convert, null); // command, sql, string value
                                break;
                            default:
                                if (propType.IsValueType) il.Emit(OpCodes.Box, propType); // command, sql, object value
                                il.EmitCall(OpCodes.Call, format, null); // command, sql, string value
                                break;
                        }
                        il.EmitCall(OpCodes.Callvirt, StringReplace, null);
                    }
                }
                il.EmitCall(OpCodes.Callvirt, cmdText.GetSetMethod(), null); // empty
            }

            il.Emit(OpCodes.Ret);
            return (Action<IDbCommand, object>)dm.CreateDelegate(typeof(Action<IDbCommand, object>));
        }

        #endregion 内部方法
        #region // 内部类型
        internal sealed class DataRecordHandler<T> : ITypeHandler
            where T : IDataRecord
        {
            public object Parse(Type destinationType, object value)
            {
                throw new NotSupportedException();
            }

            public void SetValue(IDbDataParameter parameter, object value)
            {
                SqlDataRecordListTVPParameter<T>.Set(parameter, value as IEnumerable<T>, null);
            }
        }
        /// <summary>
        /// Used to pass a IEnumerable&lt;SqlDataRecord&gt; as a SqlDataRecordListTVPParameter
        /// </summary>
        internal sealed class SqlDataRecordListTVPParameter<T> : ICustomQueryParameter
            where T : IDataRecord
        {
            private readonly IEnumerable<T> data;
            private readonly string typeName;
            /// <summary>
            /// Create a new instance of <see cref="SqlDataRecordListTVPParameter&lt;T&gt;"/>.
            /// </summary>
            /// <param name="data">The data records to convert into TVPs.</param>
            /// <param name="typeName">The parameter type name.</param>
            public SqlDataRecordListTVPParameter(IEnumerable<T> data, string typeName)
            {
                this.data = data;
                this.typeName = typeName;
            }

            void ICustomQueryParameter.AddParameter(IDbCommand command, string name)
            {
                var param = command.CreateParameter();
                param.ParameterName = name;
                Set(param, data, typeName);
                command.Parameters.Add(param);
            }

            internal static void Set(IDbDataParameter parameter, IEnumerable<T> data, string typeName)
            {
                parameter.Value = data != null && data.Any() ? data : null;
                StructuredHelper.ConfigureTVP(parameter, typeName);
            }
        }
        static class StructuredHelper
        {
            private static readonly Hashtable s_udt = new Hashtable(), s_tvp = new Hashtable();

            private static Action<IDbDataParameter, string> GetUDT(Type type)
                => (Action<IDbDataParameter, string>)s_udt[type] ?? SlowGetHelper(type, s_udt, "UdtTypeName", 29); // 29 = SqlDbType.Udt (avoiding ref)
            private static Action<IDbDataParameter, string> GetTVP(Type type)
                => (Action<IDbDataParameter, string>)s_tvp[type] ?? SlowGetHelper(type, s_tvp, "TypeName", 30); // 30 = SqlDbType.Structured (avoiding ref)

            static Action<IDbDataParameter, string> SlowGetHelper(Type type, Hashtable hashtable, string nameProperty, int sqlDbType)
            {
                lock (hashtable)
                {
                    var helper = (Action<IDbDataParameter, string>)hashtable[type];
                    if (helper == null)
                    {
                        helper = CreateFor(type, nameProperty, sqlDbType);
                        hashtable.Add(type, helper);
                    }
                    return helper;
                }
            }

            static Action<IDbDataParameter, string> CreateFor(Type type, string nameProperty, int sqlDbType)
            {
                var name = type.GetProperty(nameProperty, BindingFlags.Public | BindingFlags.Instance);
                if (name == null || !name.CanWrite)
                {
                    return (p, n) => { };
                }

                var dbType = type.GetProperty("SqlDbType", BindingFlags.Public | BindingFlags.Instance);
                if (dbType != null && !dbType.CanWrite) dbType = null;

                var dm = new DynamicMethod(nameof(CreateFor) + "_" + type.Name, null,
                    new[] { typeof(IDbDataParameter), typeof(string) }, true);
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Callvirt, name.GetSetMethod(), null);

                if (dbType != null)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, type);
                    il.Emit(OpCodes.Ldc_I4, sqlDbType);
                    il.EmitCall(OpCodes.Callvirt, dbType.GetSetMethod(), null);
                }

                il.Emit(OpCodes.Ret);
                return (Action<IDbDataParameter, string>)dm.CreateDelegate(typeof(Action<IDbDataParameter, string>));

            }

            // this needs to be done per-provider; "dynamic" doesn't work well on all runtimes, although that
            // would be a fair option otherwise
            internal static void ConfigureUDT(IDbDataParameter parameter, string typeName)
                => GetUDT(parameter.GetType())(parameter, typeName);
            internal static void ConfigureTVP(IDbDataParameter parameter, string typeName)
                => GetTVP(parameter.GetType())(parameter, typeName);
        }
        /// <summary>
        /// Represents a placeholder for a value that should be replaced as a literal value in the resulting sql
        /// </summary>
        internal struct LiteralToken
        {
            /// <summary>
            /// The text in the original command that should be replaced
            /// </summary>
            public string Token { get; }

            /// <summary>
            /// The name of the member referred to by the token
            /// </summary>
            public string Member { get; }

            internal LiteralToken(string token, string member)
            {
                Token = token;
                Member = member;
            }
#if NETFrame
            internal static IList<LiteralToken> None => new LiteralToken[0];
#else
            internal static IList<LiteralToken> None => Array.Empty<LiteralToken>();
#endif
        }
        /// <summary>
        /// 这是一个微缓存; 适用于数量是可控的(例如几百个)，并且严格只添加; 不能更改现有的值。 
        /// 所有键匹配在**REFERENCE**相等。 
        /// 该类型是完全线程安全的。
        /// </summary>
        /// <typeparam name="TKey">缓存键类型</typeparam>
        /// <typeparam name="TValue">缓存值类型</typeparam>
        internal class MapperLink<TKey, TValue> where TKey : class
        {
            public static bool TryGet(MapperLink<TKey, TValue> link, TKey key, out TValue value)
            {
                while (link != null)
                {
                    if ((object)key == (object)link.Key)
                    {
                        value = link.Value;
                        return true;
                    }
                    link = link.Tail;
                }
                value = default;
                return false;
            }

            public static bool TryAdd(ref MapperLink<TKey, TValue> head, TKey key, ref TValue value)
            {
                bool tryAgain;
                do
                {
                    var snapshot = Interlocked.CompareExchange(ref head, null, null);
                    if (TryGet(snapshot, key, out TValue found))
                    { // existing match; report the existing value instead
                        value = found;
                        return false;
                    }
                    var newNode = new MapperLink<TKey, TValue>(key, value, snapshot);
                    // did somebody move our cheese?
                    tryAgain = Interlocked.CompareExchange(ref head, newNode, snapshot) != snapshot;
                } while (tryAgain);
                return true;
            }

            private MapperLink(TKey key, TValue value, MapperLink<TKey, TValue> tail)
            {
                Key = key;
                Value = value;
                Tail = tail;
            }
            public TKey Key { get; }
            public TValue Value { get; }
            public MapperLink<TKey, TValue> Tail { get; }
        }
        /// <summary>
        /// Handles variances in features per DBMS
        /// </summary>
        internal class FeatureSupport
        {
            private static readonly FeatureSupport
                Default = new FeatureSupport(false),
                Postgres = new FeatureSupport(true),
                ClickHouse = new FeatureSupport(true);

            /// <summary>
            /// Gets the feature set based on the passed connection
            /// </summary>
            /// <param name="connection">The connection to get supported features for.</param>
            public static FeatureSupport Get(IDbConnection connection)
            {
                string name = connection?.GetType().Name;
                if (string.Equals(name, "npgsqlconnection", StringComparison.OrdinalIgnoreCase)) return Postgres;
                if (string.Equals(name, "clickhouseconnection", StringComparison.OrdinalIgnoreCase)) return ClickHouse;
                return Default;
            }

            private FeatureSupport(bool arrays)
            {
                Arrays = arrays;
            }

            /// <summary>
            /// True if the db supports array columns e.g. Postgresql
            /// </summary>
            public bool Arrays { get; }
        }
        internal class CmdCacheInfo
        {
            public DeserializerState Deserializer { get; set; }
            public Func<IDataReader, object>[] OtherDeserializers { get; set; }
            public Action<IDbCommand, object> ParamReader { get; set; }
            private int hitCount;
            public int GetHitCount() { return Interlocked.CompareExchange(ref hitCount, 0, 0); }
            public void RecordHit() { Interlocked.Increment(ref hitCount); }
        }
        internal struct DeserializerState
        {
            public readonly int Hash;
            public readonly Func<IDataReader, object> Func;

            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }
        internal class TypeDeserializerCache
        {
            private static readonly Hashtable byType = new Hashtable();
            private readonly Type type;
            private TypeDeserializerCache(Type type)
            {
                this.type = type;
            }

            internal static void Purge(Type type)
            {
                lock (byType)
                {
                    byType.Remove(type);
                }
            }

            internal static void Purge()
            {
                lock (byType)
                {
                    byType.Clear();
                }
            }

            internal static Func<IDataReader, object> GetReader(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
            {
                var found = (TypeDeserializerCache)byType[type];
                if (found == null)
                {
                    lock (byType)
                    {
                        found = (TypeDeserializerCache)byType[type];
                        if (found == null)
                        {
                            byType[type] = found = new TypeDeserializerCache(type);
                        }
                    }
                }
                return found.GetReader(reader, startBound, length, returnNullIfFirstMissing);
            }

            private readonly Dictionary<DeserializerKey, Func<IDataReader, object>> readers = new Dictionary<DeserializerKey, Func<IDataReader, object>>();

            private struct DeserializerKey : IEquatable<DeserializerKey>
            {
                private readonly int startBound, length;
                private readonly bool returnNullIfFirstMissing;
                private readonly IDataReader reader;
                private readonly string[] names;
                private readonly Type[] types;
                private readonly int hashCode;

                public DeserializerKey(int hashCode, int startBound, int length, bool returnNullIfFirstMissing, IDataReader reader, bool copyDown)
                {
                    this.hashCode = hashCode;
                    this.startBound = startBound;
                    this.length = length;
                    this.returnNullIfFirstMissing = returnNullIfFirstMissing;

                    if (copyDown)
                    {
                        this.reader = null;
                        names = new string[length];
                        types = new Type[length];
                        int index = startBound;
                        for (int i = 0; i < length; i++)
                        {
                            names[i] = reader.GetName(index);
                            types[i] = reader.GetFieldType(index++);
                        }
                    }
                    else
                    {
                        this.reader = reader;
                        names = null;
                        types = null;
                    }
                }

                public override int GetHashCode() => hashCode;

                public override string ToString()
                { // only used in the debugger
                    if (names != null)
                    {
                        return string.Join(", ", names);
                    }
                    if (reader != null)
                    {
                        var sb = new StringBuilder();
                        int index = startBound;
                        for (int i = 0; i < length; i++)
                        {
                            if (i != 0) sb.Append(", ");
                            sb.Append(reader.GetName(index++));
                        }
                        return sb.ToString();
                    }
                    return base.ToString();
                }

                public override bool Equals(object obj)
                    => obj is DeserializerKey key && Equals(key);

                public bool Equals(DeserializerKey other)
                {
                    if (hashCode != other.hashCode
                        || startBound != other.startBound
                        || length != other.length
                        || returnNullIfFirstMissing != other.returnNullIfFirstMissing)
                    {
                        return false; // clearly different
                    }
                    for (int i = 0; i < length; i++)
                    {
                        if ((names?[i] ?? reader?.GetName(startBound + i)) != (other.names?[i] ?? other.reader?.GetName(startBound + i))
                            ||
                            (types?[i] ?? reader?.GetFieldType(startBound + i)) != (other.types?[i] ?? other.reader?.GetFieldType(startBound + i))
                            )
                        {
                            return false; // different column name or type
                        }
                    }
                    return true;
                }
            }

            private Func<IDataReader, object> GetReader(IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
            {
                if (length < 0) length = reader.FieldCount - startBound;
                int hash = GetColumnHash(reader, startBound, length);
                if (returnNullIfFirstMissing) hash *= -27;
                // get a cheap key first: false means don't copy the values down
                var key = new DeserializerKey(hash, startBound, length, returnNullIfFirstMissing, reader, false);
                Func<IDataReader, object> deser;
                lock (readers)
                {
                    if (readers.TryGetValue(key, out deser)) return deser;
                }
                deser = GetTypeDeserializerImpl(type, reader, startBound, length, returnNullIfFirstMissing);
                // get a more expensive key: true means copy the values down so it can be used as a key later
                key = new DeserializerKey(hash, startBound, length, returnNullIfFirstMissing, reader, true);
                lock (readers)
                {
                    return readers[key] = deser;
                }
            }
        }
        internal abstract class XmlTypeHandler<T> : StringTypeHandler<T>
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
        #endregion 内部类型
        #region // 隐私属性
        private static readonly ConcurrentDictionary<CmdIdentity, CmdCacheInfo> _queryCache = new();
        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        private static int collect;
        private static Dictionary<Type, ITypeHandler> typeHandlers;
        private const string LinqBinary = "System.Data.Linq.Binary";
        private const string ObsoleteInternalUsageOnly = "This method is for internal use only";
        #endregion 隐私属性
        #region // 隐私方法
        private static readonly Regex smellsLikeOleDb = new Regex(@"(?<![\p{L}\p{N}@_])[?@:](?![\p{L}\p{N}@_])", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex literalTokens = new Regex(@"(?<![\p{L}\p{N}_])\{=([\p{L}\p{N}_]+)\}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex pseudoPositional = new Regex(@"\?([\p{L}_][\p{L}\p{N}_]*)\?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static void SetQueryCache(CmdIdentity key, CmdCacheInfo value)
        {
            if (Interlocked.Increment(ref collect) == COLLECT_PER_ITEMS)
            {
                CollectCacheGarbage();
            }
            _queryCache[key] = value;
        }
        private static void CollectCacheGarbage()
        {
            try
            {
                foreach (var pair in _queryCache)
                {
                    if (pair.Value.GetHitCount() <= COLLECT_HIT_COUNT_MIN)
                    {
                        _queryCache.TryRemove(pair.Key, out CmdCacheInfo _);
                    }
                }
            }

            finally
            {
                Interlocked.Exchange(ref collect, 0);
            }
        }
        private static bool TryGetQueryCache(CmdIdentity key, out CmdCacheInfo value)
        {
            if (_queryCache.TryGetValue(key, out value))
            {
                value.RecordHit();
                return true;
            }
            value = null;
            return false;
        }
        private static void PurgeQueryCacheByType(Type type)
        {
            foreach (var entry in _queryCache)
            {
                if (entry.Key.type == type)
                    _queryCache.TryRemove(entry.Key, out CmdCacheInfo _);
            }
            TypeDeserializerCache.Purge(type);
        }
        private static void OnQueryCachePurged()
        {
            var handler = QueryCachePurged;
            handler?.Invoke(null, EventArgs.Empty);
        }
        private static int ExecuteImpl(this IDbConnection cnn, CmdDefinition command)
        {
            object param = command.Args;
            IEnumerable multiExec = GetMultiExec(param);
            CmdIdentity identity;
            CmdCacheInfo info = null;
            if (multiExec != null)
            {
                if (command.IsPipelined)
                {
                    // this includes all the code for concurrent/overlapped query
                    return ExecuteMultiImplAsync(cnn, command, multiExec).Result;
                }
                bool isFirst = true;
                int total = 0;
                bool wasClosed = cnn.State == ConnectionState.Closed;
                try
                {
                    if (wasClosed) cnn.Open();
                    using (var cmd = command.SetupCommand(cnn, null))
                    {
                        string masterSql = null;
                        foreach (var obj in multiExec)
                        {
                            if (isFirst)
                            {
                                masterSql = cmd.CommandText;
                                isFirst = false;
                                identity = new CmdIdentity(command.Text, cmd.CommandType, cnn, null, obj.GetType());
                                info = GetCacheInfo(identity, obj, command.IsAddToCache);
                            }
                            else
                            {
                                cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                                cmd.Parameters.Clear(); // current code is Add-tastic
                            }
                            info.ParamReader(cmd, obj);
                            total += cmd.ExecuteNonQuery();
                        }
                    }
                    command.OnCompleted();
                }
                finally
                {
                    if (wasClosed) cnn.Close();
                }
                return total;
            }

            // nice and simple
            if (param != null)
            {
                identity = new CmdIdentity(command.Text, command.Type, cnn, null, param.GetType());
                info = GetCacheInfo(identity, param, command.IsAddToCache);
            }
            return ExecuteCommand(cnn, ref command, param == null ? null : info.ParamReader);
        }

        private static CmdCacheInfo GetCacheInfo(CmdIdentity identity, object exampleParameters, bool addToCache)
        {
            if (!TryGetQueryCache(identity, out CmdCacheInfo info))
            {
                if (GetMultiExec(exampleParameters) != null)
                {
                    throw new InvalidOperationException("An enumerable sequence of parameters (arrays, lists, etc) is not allowed in this context");
                }
                info = new CacheInfo();
                if (identity.parametersType != null)
                {
                    Action<IDbCommand, object> reader;
                    if (exampleParameters is IDynamicArgs)
                    {
                        reader = (cmd, obj) => ((IDynamicParameters)obj).AddParameters(cmd, identity);
                    }
                    else if (exampleParameters is IEnumerable<KeyValuePair<string, object>>)
                    {
                        reader = (cmd, obj) =>
                        {
                            IDynamicParameters mapped = new DynamicParameters(obj);
                            mapped.AddParameters(cmd, identity);
                        };
                    }
                    else
                    {
                        var literals = GetLiteralTokens(identity.sql);
                        reader = CreateParamInfoGenerator(identity, false, true, literals);
                    }
                    if ((identity.commandType == null || identity.commandType == CommandType.Text) && ShouldPassByPosition(identity.sql))
                    {
                        var tail = reader;
                        reader = (cmd, obj) =>
                        {
                            tail(cmd, obj);
                            PassByPosition(cmd);
                        };
                    }
                    info.ParamReader = reader;
                }
                if (addToCache) SetQueryCache(identity, info);
            }
            return info;
        }
        private static bool ShouldPassByPosition(string sql)
        {
            return sql?.IndexOf('?') >= 0 && pseudoPositional.IsMatch(sql);
        }

        private static void PassByPosition(IDbCommand cmd)
        {
            if (cmd.Parameters.Count == 0) return;

            Dictionary<string, IDbDataParameter> parameters = new Dictionary<string, IDbDataParameter>(StringComparer.Ordinal);

            foreach (IDbDataParameter param in cmd.Parameters)
            {
                if (!string.IsNullOrEmpty(param.ParameterName)) parameters[param.ParameterName] = param;
            }
            HashSet<string> consumed = new HashSet<string>(StringComparer.Ordinal);
            bool firstMatch = true;
            cmd.CommandText = pseudoPositional.Replace(cmd.CommandText, match =>
            {
                string key = match.Groups[1].Value;
                if (!consumed.Add(key))
                {
                    throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
                }
                else if (parameters.TryGetValue(key, out IDbDataParameter param))
                {
                    if (firstMatch)
                    {
                        firstMatch = false;
                        cmd.Parameters.Clear(); // only clear if we are pretty positive that we've found this pattern successfully
                    }
                    // if found, return the anonymous token "?"
                    cmd.Parameters.Add(param);
                    parameters.Remove(key);
                    consumed.Add(key);
                    return "?";
                }
                else
                {
                    // otherwise, leave alone for simple debugging
                    return match.Value;
                }
            });
        }

        private static IEnumerable GetMultiExec(object param)
        {
#pragma warning disable IDE0038 // Use pattern matching - complicated enough!
            return (param is IEnumerable
#pragma warning restore IDE0038 // Use pattern matching
                    && !(param is string
                      || param is IEnumerable<KeyValuePair<string, object>>
                      || param is IDynamicArgs)
                ) ? (IEnumerable)param : null;
        }

        private static IDbCommand SetupCommand(this CmdDefinition cmdDef, IDbConnection cnn, Action<IDbCommand, object> paramReader)
        {
            var cmd = cnn.CreateCommand();
            var init = GetInit(cmd.GetType());
            init?.Invoke(cmd);
            if (cmdDef.Transaction != null) { cmd.Transaction = cmdDef.Transaction; }
            cmd.CommandText = cmdDef.Text;
            if (cmdDef.Timeout.HasValue)
            {
                cmd.CommandTimeout = cmdDef.Timeout.Value;
            }
            else if (CommandTimeout.HasValue)
            {
                cmd.CommandTimeout = CommandTimeout.Value;
            }
            cmd.CommandType = cmdDef.Type;
            paramReader?.Invoke(cmd, cmdDef.Args);
            return cmd;
        }

        private static void OnCompleted(this CmdDefinition cmdDef)
        {
            (cmdDef.Args as IDynamicArgCallbacks)?.OnCompleted();
        }

        private static MapperLink<Type, Action<IDbCommand>> commandInitCache;

        private static Action<IDbCommand> GetInit(Type commandType)
        {
            if (commandType == null) { return null; } // GIGO
            if (MapperLink<Type, Action<IDbCommand>>.TryGet(commandInitCache, commandType, out Action<IDbCommand> action))
            {
                return action;
            }

            static MethodInfo GetBasicPropertySetter(Type declaringType, string name, Type expectedType)
            {
                var prop = declaringType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.CanWrite == true && prop.PropertyType == expectedType && prop.GetIndexParameters().Length == 0)
                {
                    return prop.GetSetMethod();
                }
                return null;
            }
            var bindByName = GetBasicPropertySetter(commandType, "BindByName", typeof(bool));
            var initialLongFetchSize = GetBasicPropertySetter(commandType, "InitialLONGFetchSize", typeof(int));

            action = null;
            if (bindByName != null || initialLongFetchSize != null)
            {
                var method = new DynamicMethod(commandType.Name + "_init", null, new Type[] { typeof(IDbCommand) });
                var il = method.GetILGenerator();
                if (bindByName != null)
                {
                    // .BindByName = true
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, commandType);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.EmitCall(OpCodes.Callvirt, bindByName, null);
                }
                if (initialLongFetchSize != null)
                {
                    // .InitialLONGFetchSize = -1
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, commandType);
                    il.Emit(OpCodes.Ldc_I4_M1);
                    il.EmitCall(OpCodes.Callvirt, initialLongFetchSize, null);
                }
                il.Emit(OpCodes.Ret);
                action = (Action<IDbCommand>)method.CreateDelegate(typeof(Action<IDbCommand>));
            }
            // cache it
            MapperLink<Type, Action<IDbCommand>>.TryAdd(ref commandInitCache, commandType, ref action);
            return action;
        }
        #endregion 隐私方法
        #region // 隐私类型
        #endregion 隐私类型
    }
}
