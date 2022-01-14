using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
            get => _connStringComparer;
            set => _connStringComparer = value ?? StringComparer.Ordinal;
        }
        /// <summary>
        /// 为所有查询指定默认的命令超时时间
        /// </summary>
        public static int? CommandTimeout { get; set; }
        /// <summary>
        /// 指示数据中的空值是被静默忽略(默认)还是主动应用并分配给成员
        /// </summary>
        public static bool ApplyNullValues { get; set; }
        /// <summary>
        /// 列表扩展是否应该用空值参数填充，以防止查询计划饱和? 例如，一个'in @foo'扩展有7、8或9个值，将被发送为一个包含10个值的列表，其中3、2或1个值为空。  
        /// 填充的大小是相对于列表的大小; 150以下的“下10”，500以下的“下50”，1500以下的“下100”，等等。
        /// 注意:如果你的数据库提供程序(或特定的配置)允许null相等(aka "ansi nulls off")，这应该小心处理，因为这可能会改变你的查询的意图; 
        /// 因此，这在默认情况下是禁用的，必须启用。 
        /// </summary>
        public static bool PadListExpansions { get; set; }
        /// <summary>
        /// 如果设置(非负)，当执行整数类型的列表扩展(“where id in @ids”等)时，切换到基于string_split的  
        /// 如果有这么多或更多的元素。 请注意，此特性需要SQL Server 2016 /兼容性级别130(或以上)。
        /// </summary>
        public static int InListStringSplitCount { get; set; } = -1;
        /// <summary>
        /// 默认情况下禁用单个结果; 在正确检测到选择后防止错误
        /// </summary>
        public static CommandBehavior AllowedCommandBehaviors { get; set; } = ~CommandBehavior.SingleResult;
        /// <summary>
        /// 获取或设置Dapper是否应该使用命令行为。 SingleResult优化
        /// 注意，启用此选项的结果是，可能不会报告第一次选择之后发生的错误  
        /// </summary>
        public static bool UseSingleResultOptimization
        {
            get => (AllowedCommandBehaviors & CommandBehavior.SingleResult) != 0;
            set => SetAllowedCommandBehaviors(CommandBehavior.SingleResult, value);
        }
        /// <summary>
        /// 获取或设置Dapper是否应该使用命令行为。 回转支承的优化 
        /// 注意，在某些DB提供上，这种优化可能会对性能产生不利影响  
        /// </summary>
        public static bool UseSingleRowOptimization
        {
            get => (AllowedCommandBehaviors & CommandBehavior.SingleRow) != 0;
            set => SetAllowedCommandBehaviors(CommandBehavior.SingleRow, value);
        }

        private static CommandBehavior SetAllowedCommandBehaviors(CommandBehavior behavior, bool enabled)
        {
            return enabled ? (AllowedCommandBehaviors |= behavior) : (AllowedCommandBehaviors &= (~behavior));
        }
        internal static bool DisableCommandBehaviorOptimizations(CommandBehavior behavior, Exception ex)
        {
            if (AllowedCommandBehaviors == (~CommandBehavior.SingleResult) && (behavior & (CommandBehavior.SingleResult | CommandBehavior.SingleRow)) != 0)
            {
                if (ex.Message.Contains(nameof(CommandBehavior.SingleResult)) || ex.Message.Contains(nameof(CommandBehavior.SingleRow)))
                {
                    // 有些提供只是允许这些，所以: 在没有它们的情况下再试一次，并停止发布它们
                    SetAllowedCommandBehaviors(CommandBehavior.SingleResult | CommandBehavior.SingleRow, false);
                    return true;
                }
            }
            return false;
        }
        #endregion 外部属性
        #region // CommandDefinition
        /// <summary>
        /// 命令定义
        /// </summary>
        public struct CommandDefinition
        {
            internal static CommandDefinition ForCallback(object parameters)
            {
                if (parameters is DynamicParameters)
                {
                    return new CommandDefinition(parameters);
                }
                else
                {
                    return default;
                }
            }

            internal void OnCompleted()
            {
                (Parameters as IParameterCallbacks)?.OnCompleted();
            }

            /// <summary>
            /// The command (sql or a stored-procedure name) to execute
            /// </summary>
            public string CommandText { get; }

            /// <summary>
            /// The parameters associated with the command
            /// </summary>
            public object Parameters { get; }

            /// <summary>
            /// The active transaction for the command
            /// </summary>
            public IDbTransaction Transaction { get; }

            /// <summary>
            /// The effective timeout for the command
            /// </summary>
            public int? CommandTimeout { get; }

            /// <summary>
            /// The type of command that the command-text represents
            /// </summary>
            public CommandType? CommandType { get; }

            /// <summary>
            /// Should data be buffered before returning?
            /// </summary>
            public bool Buffered => (Flags & CommandFlags.Buffered) != 0;

            /// <summary>
            /// Should the plan for this query be cached?
            /// </summary>
            internal bool AddToCache => (Flags & CommandFlags.NoCache) == 0;

            /// <summary>
            /// Additional state flags against this command
            /// </summary>
            public CommandFlags Flags { get; }

            /// <summary>
            /// Can async queries be pipelined?
            /// </summary>
            public bool Pipelined => (Flags & CommandFlags.Pipelined) != 0;

            /// <summary>
            /// Initialize the command definition
            /// </summary>
            /// <param name="commandText">The text for this command.</param>
            /// <param name="parameters">The parameters for this command.</param>
            /// <param name="transaction">The transaction for this command to participate in.</param>
            /// <param name="commandTimeout">The timeout (in seconds) for this command.</param>
            /// <param name="commandType">The <see cref="CommandType"/> for this command.</param>
            /// <param name="flags">The behavior flags for this command.</param>
            /// <param name="cancellationToken">The cancellation token for this command.</param>
            public CommandDefinition(string commandText, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null,
                                     CommandType? commandType = null, CommandFlags flags = CommandFlags.Buffered
                                     , CancellationToken cancellationToken = default
                )
            {
                CommandText = commandText;
                Parameters = parameters;
                Transaction = transaction;
                CommandTimeout = commandTimeout;
                CommandType = commandType;
                Flags = flags;
                CancellationToken = cancellationToken;
            }

            private CommandDefinition(object parameters) : this()
            {
                Parameters = parameters;
            }

            /// <summary>
            /// For asynchronous operations, the cancellation-token
            /// </summary>
            public CancellationToken CancellationToken { get; }

            internal IDbCommand SetupCommand(IDbConnection cnn, Action<IDbCommand, object> paramReader)
            {
                var cmd = cnn.CreateCommand();
                var init = GetInit(cmd.GetType());
                init?.Invoke(cmd);
                if (Transaction != null)
                    cmd.Transaction = Transaction;
                cmd.CommandText = CommandText;
                if (CommandTimeout.HasValue)
                {
                    cmd.CommandTimeout = CommandTimeout.Value;
                }
                else if (CommandTimeout.HasValue)
                {
                    cmd.CommandTimeout = CommandTimeout.Value;
                }
                if (CommandType.HasValue)
                    cmd.CommandType = CommandType.Value;
                paramReader?.Invoke(cmd, Parameters);
                return cmd;
            }

            private static Link<Type, Action<IDbCommand>> commandInitCache;

            private static Action<IDbCommand> GetInit(Type commandType)
            {
                if (commandType == null) { return null; } // GIGO
                if (Link<Type, Action<IDbCommand>>.TryGet(commandInitCache, commandType, out Action<IDbCommand> action))
                {
                    return action;
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
                Link<Type, Action<IDbCommand>>.TryAdd(ref commandInitCache, commandType, ref action);
                return action;
            }

            private static MethodInfo GetBasicPropertySetter(Type declaringType, string name, Type expectedType)
            {
                var prop = declaringType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.CanWrite == true && prop.PropertyType == expectedType && prop.GetIndexParameters().Length == 0)
                {
                    return prop.GetSetMethod();
                }
                return null;
            }
        }
        #endregion
        #region // CommandFlags
        /// <summary>
        /// 控制命令行为的附加状态标志
        /// </summary>
        [Flags]
        [Cobber.EDisplay("命令特征")]
        public enum CommandFlags
        {
            /// <summary>
            /// 无
            /// </summary>
            [Cobber.EDisplay("无")]
            None = 0,
            /// <summary>
            /// 数据是否应该在返回之前进行缓冲?
            /// </summary>
            [Cobber.EDisplay("缓冲")]
            Buffered = 1,
            /// <summary>
            /// 异步查询可以流水线化吗?
            /// </summary>
            [Cobber.EDisplay("流水线")]
            Pipelined = 2,
            /// <summary>
            /// 计划缓存应该被绕过吗?
            /// </summary>
            [Cobber.EDisplay("无缓存")]
            NoCache = 4,
        }
        #endregion
        #region // CompactAssist
        /// <summary>
        /// 从结果来任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static Task<T> FromResult<T>(T result)
        {
#if NET40
            return new Task<T>(() => result);
#else
            return Task.FromResult(result);
#endif
        }
#if NET40
        internal static T GetFieldValue<T>(this DbDataReader _reader, int ordinal) => (T)_reader[ordinal];
        internal static Task<T> GetFieldValueAsync<T>(this DbDataReader _reader, int ordinal, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<T>();
            }
            return new Task<T>(() => GetFieldValue<T>(_reader, ordinal));
        }
        internal static Stream GetStream(this DbDataReader _reader, int ordinal)
        {
            using (MemoryStream bufferStream = new MemoryStream())
            {
                long bytesRead = 0;
                long bytesReadTotal = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    bytesRead = _reader.GetBytes(ordinal, bytesReadTotal, buffer, 0, buffer.Length);
                    bufferStream.Write(buffer, 0, (int)bytesRead);
                    bytesReadTotal += bytesRead;
                } while (bytesRead > 0);

                return new MemoryStream(bufferStream.ToArray(), false);
            }
        }
        internal static TextReader GetTextReader(this DbDataReader _reader, int ordinal)
        {
            return _reader.IsDBNull(ordinal) ? new StringReader(String.Empty) : new StringReader(_reader.GetString(ordinal));
        }
        internal static Task<bool> IsDBNullAsync(this DbDataReader _reader, int ordinal, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.IsDBNull(ordinal) ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }
        internal static Task<bool> NextResultAsync(this DbDataReader _reader, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.NextResult() ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }
        internal static Task<bool> ReadAsync(this DbDataReader _reader, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.Read() ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }

        internal static Task<bool> TrueTask { get; } = new Task<bool>(() => true);
        internal static Task<bool> FalseTask { get; } = new Task<bool>(() => false);
        internal static Task<T> CreatedTaskWithCancellation<T>()
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetCanceled();
            return completion.Task;
        }
        internal static Task<T> CreatedTaskWithException<T>(Exception ex)
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetException(ex);
            return completion.Task;
        }

        internal static Task<int> ExecuteNonQueryAsync(this DbCommand cmd) => ExecuteNonQueryAsync(cmd, CancellationToken.None);

        internal static Task<int> ExecuteNonQueryAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<int>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return FromResult<int>(cmd.ExecuteNonQuery());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<int>(e);
                }
            }
        }

        internal static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd)
        {
            return ExecuteReaderAsync(cmd, CommandBehavior.Default, CancellationToken.None);
        }

        internal static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(cmd, CommandBehavior.Default, cancellationToken);
        }

        internal static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CommandBehavior behavior)
        {
            return ExecuteReaderAsync(cmd, behavior, CancellationToken.None);
        }

        internal static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<DbDataReader>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return FromResult<DbDataReader>(cmd.ExecuteReader(behavior));
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<DbDataReader>(e);
                }
            }
        }
        internal static Task<object> ExecuteScalarAsync(this DbCommand cmd)
        {
            return ExecuteScalarAsync(cmd, CancellationToken.None);
        }

        internal static Task<object> ExecuteScalarAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<object>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return FromResult<object>(cmd.ExecuteScalar());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<object>(e);
                }
            }
        }
        internal static Task OpenAsync(this DbConnection conn)
        {
            return OpenAsync(conn, CancellationToken.None);
        }

        internal static Task OpenAsync(this DbConnection conn, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.SetCanceled();
            }
            else
            {
                try
                {
                    conn.Open();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            }

            return taskCompletionSource.Task;
        }
#endif
        #endregion
        #region // CustomPropertyTypeMap
        /// <summary>
        /// Implements custom property mapping by user provided criteria (usually presence of some custom attribute with column to member mapping)
        /// </summary>
        public sealed class CustomPropertyTypeMap : ITypeMap
        {
            private readonly Type _type;
            private readonly Func<Type, string, PropertyInfo> _propertySelector;

            /// <summary>
            /// Creates custom property mapping
            /// </summary>
            /// <param name="type">Target entity type</param>
            /// <param name="propertySelector">Property selector based on target type and DataReader column name</param>
            public CustomPropertyTypeMap(Type type, Func<Type, string, PropertyInfo> propertySelector)
            {
                _type = type ?? throw new ArgumentNullException(nameof(type));
                _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
            }

            /// <summary>
            /// Always returns default constructor
            /// </summary>
            /// <param name="names">DataReader column names</param>
            /// <param name="types">DataReader column types</param>
            /// <returns>Default constructor</returns>
            public ConstructorInfo FindConstructor(string[] names, Type[] types) =>
#if NETFx
            _type.GetConstructor(Array.Empty<Type>());
#else
            _type.GetConstructor(new Type[0]);
#endif

            /// <summary>
            /// Always returns null
            /// </summary>
            /// <returns></returns>
            public ConstructorInfo FindExplicitConstructor() => null;

            /// <summary>
            /// Not implemented as far as default constructor used for all cases
            /// </summary>
            /// <param name="constructor"></param>
            /// <param name="columnName"></param>
            /// <returns></returns>
            public IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Returns property based on selector strategy
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <returns>Property member map</returns>
            public IMemberMap GetMember(string columnName)
            {
                var prop = _propertySelector(_type, columnName);
                return prop != null ? new SimpleMemberMap(columnName, prop) : null;
            }
        }
        #endregion
        #region // DataTableHandler
        internal sealed class DataTableHandler : ITypeHandler
        {
            public object Parse(Type destinationType, object value)
            {
                throw new NotImplementedException();
            }

            public void SetValue(IDbDataParameter parameter, object value)
            {
                TableValuedParameter.Set(parameter, value as DataTable, null);
            }
        }
        #endregion
        #region // DbString
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
        #endregion
        #region // DefaultTypeMap
        /// <summary>
        /// Represents default type mapping strategy used by Dapper
        /// </summary>
        public sealed class DefaultTypeMap : ITypeMap
        {
            private readonly List<FieldInfo> _fields;
            private readonly Type _type;

            /// <summary>
            /// Creates default type map
            /// </summary>
            /// <param name="type">Entity type</param>
            public DefaultTypeMap(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                _fields = GetSettableFields(type);
                Properties = GetSettableProps(type);
                _type = type;
            }

            internal static MethodInfo GetPropertySetter(PropertyInfo propertyInfo, Type type)
            {
                if (propertyInfo.DeclaringType == type) return propertyInfo.GetSetMethod(true);

                return propertyInfo.DeclaringType.GetProperty(
                       propertyInfo.Name,
                       BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                       Type.DefaultBinder,
                       propertyInfo.PropertyType,
                       propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray(),
                       null).GetSetMethod(true);
            }

            internal static List<PropertyInfo> GetSettableProps(Type t)
            {
                return t
                      .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                      .Where(p => GetPropertySetter(p, t) != null)
                      .ToList();
            }

            internal static List<FieldInfo> GetSettableFields(Type t)
            {
                return t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            }

            /// <summary>
            /// Finds best constructor
            /// </summary>
            /// <param name="names">DataReader column names</param>
            /// <param name="types">DataReader column types</param>
            /// <returns>Matching constructor or default one</returns>
            public ConstructorInfo FindConstructor(string[] names, Type[] types)
            {
                var constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (ConstructorInfo ctor in constructors.OrderBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1)).ThenBy(c => c.GetParameters().Length))
                {
                    ParameterInfo[] ctorParameters = ctor.GetParameters();
                    if (ctorParameters.Length == 0) { return ctor; }

                    if (ctorParameters.Length != types.Length) { continue; }

                    int i = 0;
                    for (; i < ctorParameters.Length; i++)
                    {
                        if (!string.Equals(ctorParameters[i].Name, names[i], StringComparison.OrdinalIgnoreCase)) { break; }
                        if (types[i] == typeof(byte[]) && ctorParameters[i].ParameterType.FullName == LinqBinary) { continue; }
                        var unboxedType = Nullable.GetUnderlyingType(ctorParameters[i].ParameterType) ?? ctorParameters[i].ParameterType;
                        if ((unboxedType != types[i] && !HasTypeHandler(unboxedType))
                            && !(unboxedType.IsEnum && Enum.GetUnderlyingType(unboxedType) == types[i])
                            && !(unboxedType == typeof(char) && types[i] == typeof(string))
                            && !(unboxedType.IsEnum && types[i] == typeof(string)))
                        {
                            break;
                        }
                    }

                    if (i == ctorParameters.Length) { return ctor; }
                }

                return null;
            }

            /// <summary>
            /// Returns the constructor, if any, that has the ExplicitConstructorAttribute on it.
            /// </summary>
            public ConstructorInfo FindExplicitConstructor()
            {
                var constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var withAttr = constructors.Where(c => c.GetCustomAttributes(typeof(ExplicitConstructorAttribute), true).Length > 0).ToList();

                if (withAttr.Count == 1)
                {
                    return withAttr[0];
                }

                return null;
            }

            /// <summary>
            /// Gets mapping for constructor parameter
            /// </summary>
            /// <param name="constructor">Constructor to resolve</param>
            /// <param name="columnName">DataReader column name</param>
            /// <returns>Mapping implementation</returns>
            public IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
            {
                var parameters = constructor.GetParameters();

                return new SimpleMemberMap(columnName, parameters.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase)));
            }

            /// <summary>
            /// Gets member mapping for column
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <returns>Mapping implementation</returns>
            public IMemberMap GetMember(string columnName)
            {
                var property = Properties.Find(p => string.Equals(p.Name, columnName, StringComparison.Ordinal))
                   ?? Properties.Find(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

                if (property == null && MatchNamesWithUnderscores)
                {
                    property = Properties.Find(p => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.Ordinal))
                        ?? Properties.Find(p => string.Equals(p.Name, columnName.Replace("_", ""), StringComparison.OrdinalIgnoreCase));
                }

                if (property != null)
                    return new SimpleMemberMap(columnName, property);

                // roslyn automatically implemented properties, in particular for get-only properties: <{Name}>k__BackingField;
                var backingFieldName = "<" + columnName + ">k__BackingField";

                // preference order is:
                // exact match over underscore match, exact case over wrong case, backing fields over regular fields, match-inc-underscores over match-exc-underscores
                var field = _fields.Find(p => string.Equals(p.Name, columnName, StringComparison.Ordinal))
                    ?? _fields.Find(p => string.Equals(p.Name, backingFieldName, StringComparison.Ordinal))
                    ?? _fields.Find(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase))
                    ?? _fields.Find(p => string.Equals(p.Name, backingFieldName, StringComparison.OrdinalIgnoreCase));

                if (field == null && MatchNamesWithUnderscores)
                {
                    var effectiveColumnName = columnName.Replace("_", "");
                    backingFieldName = "<" + effectiveColumnName + ">k__BackingField";

                    field = _fields.Find(p => string.Equals(p.Name, effectiveColumnName, StringComparison.Ordinal))
                        ?? _fields.Find(p => string.Equals(p.Name, backingFieldName, StringComparison.Ordinal))
                        ?? _fields.Find(p => string.Equals(p.Name, effectiveColumnName, StringComparison.OrdinalIgnoreCase))
                        ?? _fields.Find(p => string.Equals(p.Name, backingFieldName, StringComparison.OrdinalIgnoreCase));
                }

                if (field != null)
                    return new SimpleMemberMap(columnName, field);

                return null;
            }
            /// <summary>
            /// Should column names like User_Id be allowed to match properties/fields like UserId ?
            /// </summary>
            public static bool MatchNamesWithUnderscores { get; set; }

            /// <summary>
            /// The settable properties for this typemap
            /// </summary>
            public List<PropertyInfo> Properties { get; }
        }
        #endregion
        #region // DynamicParameters
        /// <summary>
        /// Implement this interface to pass an arbitrary db specific set of parameters to Dapper
        /// </summary>
        public interface IDynamicParameters
        {
            /// <summary>
            /// Add all the parameters needed to the command just before it executes
            /// </summary>
            /// <param name="command">The raw command prior to execution</param>
            /// <param name="identity">Information about the query</param>
            void AddParameters(IDbCommand command, Identity identity);
        }
        /// <summary>
        /// Extends IDynamicParameters providing by-name lookup of parameter values
        /// </summary>
        public interface IParameterLookup : IDynamicParameters
        {
            /// <summary>
            /// Get the value of the specified parameter (return null if not found)
            /// </summary>
            /// <param name="name">The name of the parameter to get.</param>
            object this[string name] { get; }
        }
        /// <summary>
        /// Extends IDynamicParameters with facilities for executing callbacks after commands have completed
        /// </summary>
        public interface IParameterCallbacks : IDynamicParameters
        {
            /// <summary>
            /// Invoked when the command has executed
            /// </summary>
            void OnCompleted();
        }
        /// <summary>
        /// A bag of parameters that can be passed to the Dapper Query and Execute methods
        /// </summary>
        public class DynamicParameters : IDynamicParameters, IParameterLookup, IParameterCallbacks
        {
            internal const DbType EnumerableMultiParameter = (DbType)(-1);
            private static readonly Dictionary<Identity, Action<IDbCommand, object>> paramReaderCache = new Dictionary<Identity, Action<IDbCommand, object>>();
            private readonly Dictionary<string, ParamInfo> parameters = new Dictionary<string, ParamInfo>();
            private List<object> templates;

            object IParameterLookup.this[string name] =>
                parameters.TryGetValue(name, out ParamInfo param) ? param.Value : null;

            /// <summary>
            /// construct a dynamic parameter bag
            /// </summary>
            public DynamicParameters()
            {
                RemoveUnused = true;
            }

            /// <summary>
            /// construct a dynamic parameter bag
            /// </summary>
            /// <param name="template">can be an anonymous type or a DynamicParameters bag</param>
            public DynamicParameters(object template)
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
                    if (obj is DynamicParameters subDynamic)
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

            void IDynamicParameters.AddParameters(IDbCommand command, Identity identity)
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
            protected void AddParameters(IDbCommand command, Identity identity)
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
                            p.Value = SanitizeParameterValue(val);
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
                if (literals.Count != 0) ReplaceLiterals(this, command, literals);
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
            public DynamicParameters Output<T>(T target, Expression<Func<T, object>> expression, DbType? dbType = null, int? size = null)
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
                var setter = (Action<object, DynamicParameters>)cache[lookup];
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

                setter = (Action<object, DynamicParameters>)dm.CreateDelegate(typeof(Action<object, DynamicParameters>));
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
                    ? LookupDbType(targetMemberType, targetMemberType?.Name, true, out ITypeHandler handler)
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

            void IParameterCallbacks.OnCompleted()
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
                internal Action<object, DynamicParameters> OutputCallback { get; set; }
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
        #endregion
        #region // ExplicitConstructorAttribute
        /// <summary>
        /// Tell Dapper to use an explicit constructor, passing nulls or 0s for all parameters
        /// </summary>
        [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
        public sealed class ExplicitConstructorAttribute : Attribute
        {
        }
        #endregion
        #region // Extensions
        /// <summary>
        /// Creates a <see cref="Task{TResult}"/> with a less specific generic parameter that perfectly mirrors the
        /// state of the specified <paramref name="task"/>.
        /// </summary>
        internal static Task<TTo> CastResult<TFrom, TTo>(this Task<TFrom> task)
            where TFrom : TTo
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return FromResult((TTo)task.Result);
            }
            var source = new TaskCompletionSource<TTo>();
#if NET40
            task.ContinueWith((s) => OnTaskCompleted<TFrom, TTo>(s, source), TaskContinuationOptions.ExecuteSynchronously);
#else
            task.ContinueWith(OnTaskCompleted<TFrom, TTo>, state: source, TaskContinuationOptions.ExecuteSynchronously);
#endif
            return source.Task;
        }

        private static void OnTaskCompleted<TFrom, TTo>(Task<TFrom> completedTask, object state)
            where TFrom : TTo
        {
            var source = (TaskCompletionSource<TTo>)state;

            switch (completedTask.Status)
            {
                case TaskStatus.RanToCompletion:
                    source.SetResult(completedTask.Result);
                    break;
                case TaskStatus.Canceled:
                    source.SetCanceled();
                    break;
                case TaskStatus.Faulted:
                    source.SetException(completedTask.Exception.InnerExceptions);
                    break;
            }
        }
        #endregion
        #region // FeatureSupport
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
        #endregion
        #region // SimpleMemberMap
        /// <summary>
        /// Represents simple member map for one of target parameter or property or field to source DataReader column
        /// </summary>
        internal sealed class SimpleMemberMap : IMemberMap
        {
            /// <summary>
            /// Creates instance for simple property mapping
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <param name="property">Target property</param>
            public SimpleMemberMap(string columnName, PropertyInfo property)
            {
                ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
                Property = property ?? throw new ArgumentNullException(nameof(property));
            }

            /// <summary>
            /// Creates instance for simple field mapping
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <param name="field">Target property</param>
            public SimpleMemberMap(string columnName, FieldInfo field)
            {
                ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
                Field = field ?? throw new ArgumentNullException(nameof(field));
            }

            /// <summary>
            /// Creates instance for simple constructor parameter mapping
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <param name="parameter">Target constructor parameter</param>
            public SimpleMemberMap(string columnName, ParameterInfo parameter)
            {
                ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
                Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            }

            /// <summary>
            /// DataReader column name
            /// </summary>
            public string ColumnName { get; }

            /// <summary>
            /// Target member type
            /// </summary>
            public Type MemberType => Field?.FieldType ?? Property?.PropertyType ?? Parameter?.ParameterType;

            /// <summary>
            /// Target property
            /// </summary>
            public PropertyInfo Property { get; }

            /// <summary>
            /// Target field
            /// </summary>
            public FieldInfo Field { get; }

            /// <summary>
            /// Target constructor parameter
            /// </summary>
            public ParameterInfo Parameter { get; }
        }
        #endregion
        #region // SqlDataRecordHandler
        internal sealed class SqlDataRecordHandler<T> : ITypeHandler
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
        #endregion
        #region // SqlDataRecordListTVPParameter
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
        #endregion
        #region // CacheInfo
        private class CacheInfo
        {
            public DeserializerState Deserializer { get; set; }
            public Func<IDataReader, object>[] OtherDeserializers { get; set; }
            public Action<IDbCommand, object> ParamReader { get; set; }
            private int hitCount;
            public int GetHitCount() { return Interlocked.CompareExchange(ref hitCount, 0, 0); }
            public void RecordHit() { Interlocked.Increment(ref hitCount); }
        }
        #endregion
        #region // SqlMapper
        private class PropertyInfoByNameComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y) => string.CompareOrdinal(x.Name, y.Name);
        }
        private static int GetColumnHash(IDataReader reader, int startBound = 0, int length = -1)
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

        /// <summary>
        /// Called if the query cache is purged via PurgeQueryCache
        /// </summary>
        public static event EventHandler QueryCachePurged;
        private static void OnQueryCachePurged()
        {
            var handler = QueryCachePurged;
            handler?.Invoke(null, EventArgs.Empty);
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Identity, CacheInfo> _queryCache = new System.Collections.Concurrent.ConcurrentDictionary<Identity, CacheInfo>();
        private static void SetQueryCache(Identity key, CacheInfo value)
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
                        _queryCache.TryRemove(pair.Key, out CacheInfo _);
                    }
                }
            }

            finally
            {
                Interlocked.Exchange(ref collect, 0);
            }
        }

        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        private static int collect;
        private static bool TryGetQueryCache(Identity key, out CacheInfo value)
        {
            if (_queryCache.TryGetValue(key, out value))
            {
                value.RecordHit();
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Purge the query cache
        /// </summary>
        public static void PurgeQueryCache()
        {
            _queryCache.Clear();
            TypeDeserializerCache.Purge();
            OnQueryCachePurged();
        }

        private static void PurgeQueryCacheByType(Type type)
        {
            foreach (var entry in _queryCache)
            {
                if (entry.Key.type == type)
                    _queryCache.TryRemove(entry.Key, out CacheInfo _);
            }
            TypeDeserializerCache.Purge(type);
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

        private static Dictionary<Type, DbType> typeMap;

        static SqlDibber()
        {
            typeMap = new Dictionary<Type, DbType>(37)
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
            ResetTypeHandlers(false);
        }

        /// <summary>
        /// Clear the registered type handlers.
        /// </summary>
        public static void ResetTypeHandlers() => ResetTypeHandlers(true);

        private static void ResetTypeHandlers(bool clone)
        {
            typeHandlers = new Dictionary<Type, ITypeHandler>();
            AddTypeHandlerImpl(typeof(DataTable), new DataTableHandler(), clone);
            AddTypeHandlerImpl(typeof(XmlDocument), new XmlDocumentHandler(), clone);
            AddTypeHandlerImpl(typeof(XDocument), new XDocumentHandler(), clone);
            AddTypeHandlerImpl(typeof(XElement), new XElementHandler(), clone);
        }

        /// <summary>
        /// Configure the specified type to be mapped to a given db-type.
        /// </summary>
        /// <param name="type">The type to map from.</param>
        /// <param name="dbType">The database type to map to.</param>
        public static void AddTypeMap(Type type, DbType dbType)
        {
            // use clone, mutate, replace to avoid threading issues
            var snapshot = typeMap;

            if (snapshot.TryGetValue(type, out DbType oldValue) && oldValue == dbType) return; // nothing to do

            typeMap = new Dictionary<Type, DbType>(snapshot) { [type] = dbType };
        }

        /// <summary>
        /// Removes the specified type from the Type/DbType mapping table.
        /// </summary>
        /// <param name="type">The type to remove from the current map.</param>
        public static void RemoveTypeMap(Type type)
        {
            // use clone, mutate, replace to avoid threading issues
            var snapshot = typeMap;

            if (!snapshot.ContainsKey(type)) return; // nothing to do

            var newCopy = new Dictionary<Type, DbType>(snapshot);
            newCopy.Remove(type);

            typeMap = newCopy;
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
        /// Configure the specified type to be processed by a custom handler.
        /// </summary>
        /// <typeparam name="T">The type to handle.</typeparam>
        /// <param name="handler">The handler for the type <typeparamref name="T"/>.</param>
        public static void AddTypeHandler<T>(TypeHandler<T> handler) => AddTypeHandlerImpl(typeof(T), handler, true);

        private static Dictionary<Type, ITypeHandler> typeHandlers;

        internal const string LinqBinary = "System.Data.Linq.Binary";

        private const string ObsoleteInternalUsageOnly = "This method is for internal use only";

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
                                typeof(SqlDataRecordHandler<>).MakeGenericType(argTypes));
                            AddTypeHandlerImpl(type, handler, true);
                            return DbType.Object;
                        }
                        catch
                        {
                            handler = null;
                        }
                    }
                }
                return DynamicParameters.EnumerableMultiParameter;
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
        /// Obtains the data as a list; if it is *already* a list, the original object is returned without
        /// any duplication; otherwise, ToList() is invoked.
        /// </summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The enumerable to return as a list.</param>
        public static List<T> AsList<T>(this IEnumerable<T> source) =>
            (source == null || source is List<T>) ? (List<T>)source : source.ToList();

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteImpl(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute on this connection.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection cnn, CommandDefinition command) => ExecuteImpl(cnn, ref command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell selected as <see cref="object"/>.</returns>
        public static object ExecuteScalar(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteScalarImpl<object>(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
        public static T ExecuteScalar<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return ExecuteScalarImpl<T>(cnn, ref command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <see cref="object"/>.</returns>
        public static object ExecuteScalar(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImpl<object>(cnn, ref command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <typeparamref name="T"/>.</returns>
        public static T ExecuteScalar<T>(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImpl<T>(cnn, ref command);

        private static IEnumerable GetMultiExec(object param)
        {
#pragma warning disable IDE0038 // Use pattern matching - complicated enough!
            return (param is IEnumerable
#pragma warning restore IDE0038 // Use pattern matching
                    && !(param is string
                      || param is IEnumerable<KeyValuePair<string, object>>
                      || param is IDynamicParameters)
                ) ? (IEnumerable)param : null;
        }

        private static int ExecuteImpl(this IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            Identity identity;
            CacheInfo info = null;
            if (multiExec != null)
            {
                if ((command.Flags & CommandFlags.Pipelined) != 0)
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
                                identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType());
                                info = GetCacheInfo(identity, obj, command.AddToCache);
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
                identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                info = GetCacheInfo(identity, param, command.AddToCache);
            }
            return ExecuteCommand(cnn, ref command, param == null ? null : info.ParamReader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DataTable table = new DataTable("MyTable");
        /// using (var reader = ExecuteReader(cnn, sql, param))
        /// {
        ///     table.Load(reader);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IDataReader ExecuteReader(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            var reader = ExecuteReaderImpl(cnn, ref command, CommandBehavior.Default, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command)
        {
            var reader = ExecuteReaderImpl(cnn, ref command, CommandBehavior.Default, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The <see cref="CommandBehavior"/> flags for this reader.</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        public static IDataReader ExecuteReader(this IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        {
            var reader = ExecuteReaderImpl(cnn, ref command, commandBehavior, out IDbCommand dbcmd);
            return WrappedReader.Create(dbcmd, reader);
        }

        /// <summary>
        /// Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
            Query<DapperRow>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QueryFirst(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryFirst<DapperRow>(cnn, sql, param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QueryFirstOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryFirstOrDefault<DapperRow>(cnn, sql, param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QuerySingle(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QuerySingle<DapperRow>(cnn, sql, param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static dynamic QuerySingleOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QuerySingleOrDefault<DapperRow>(cnn, sql, param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var data = QueryImpl<T>(cnn, command, typeof(T));
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QueryFirst<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.First, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.FirstOrDefault, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QuerySingle<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.Single, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QuerySingleOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<T>(cnn, Row.SingleOrDefault, ref command, typeof(T));
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static IEnumerable<object> Query(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var data = QueryImpl<object>(cnn, command, type);
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static object QueryFirst(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.First, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static object QueryFirstOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.FirstOrDefault, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static object QuerySingle(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.Single, ref command, type);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as <paramref name="type"/>.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static object QuerySingleOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None);
            return QueryRowImpl<object>(cnn, Row.SingleOrDefault, ref command, type);
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, CommandDefinition command)
        {
            var data = QueryImpl<T>(cnn, command, typeof(T));
            return command.Buffered ? data.ToList() : data;
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A single instance or null of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QueryFirst<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.First, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A single or null instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.FirstOrDefault, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QuerySingle<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.Single, ref command, typeof(T));

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static T QuerySingleOrDefault<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowImpl<T>(cnn, Row.SingleOrDefault, ref command, typeof(T));

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        public static GridReader QueryMultiple(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered);
            return QueryMultipleImpl(cnn, ref command);
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command to execute for this query.</param>
        public static GridReader QueryMultiple(this IDbConnection cnn, CommandDefinition command) =>
            QueryMultipleImpl(cnn, ref command);

        private static GridReader QueryMultipleImpl(this IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, typeof(GridReader), param?.GetType());
            CacheInfo info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) cnn.Open();
                cmd = command.SetupCommand(cnn, info.ParamReader);
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, CommandBehavior.SequentialAccess);

                var result = new GridReader(cmd, reader, identity, command.Parameters as DynamicParameters, command.AddToCache);
                cmd = null; // now owned by result
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                return result;
            }
            catch
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd?.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                cmd?.Dispose();
                if (wasClosed) cnn.Close();
                throw;
            }
        }

        private static IDataReader ExecuteReaderWithFlagsFallback(IDbCommand cmd, bool wasClosed, CommandBehavior behavior)
        {
            try
            {
                return cmd.ExecuteReader(GetBehavior(wasClosed, behavior));
            }
            catch (ArgumentException ex)
            { // thanks, Sqlite!
                if (DisableCommandBehaviorOptimizations(behavior, ex))
                {
                    // we can retry; this time it will have different flags
                    return cmd.ExecuteReader(GetBehavior(wasClosed, behavior));
                }
                throw;
            }
        }

        private static IEnumerable<T> QueryImpl<T>(this IDbConnection cnn, CommandDefinition command, Type effectiveType)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                var tuple = info.Deserializer;
                int hash = GetColumnHash(reader);
                if (tuple.Func == null || tuple.Hash != hash)
                {
                    if (reader.FieldCount == 0) //https://code.google.com/p/dapper-dot-net/issues/detail?id=57
                        yield break;
                    tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                    if (command.AddToCache) SetQueryCache(identity, info);
                }

                var func = tuple.Func;
                var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                while (reader.Read())
                {
                    object val = func(reader);
                    yield return GetValue<T>(reader, effectiveType, val);
                }
                while (reader.NextResult()) { /* ignore subsequent result sets */ }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        [Flags]
        internal enum Row
        {
            First = 0,
            FirstOrDefault = 1, //  & FirstOrDefault != 0: allow zero rows
            Single = 2, // & Single != 0: demand at least one row
            SingleOrDefault = 3
        }
#if NETFrame
        private static readonly int[] ErrTwoRows = new int[2], ErrZeroRows = new int[0];
#else
        private static readonly int[] ErrTwoRows = new int[2], ErrZeroRows = Array.Empty<int>();
#endif
        private static void ThrowMultipleRows(Row row)
        {
            _ = row switch
            {
                Row.Single => ErrTwoRows.Single(),
                Row.SingleOrDefault => ErrTwoRows.SingleOrDefault(),
                _ => throw new InvalidOperationException(),
            };
        }

        private static void ThrowZeroRows(Row row)
        {
            _ = row switch
            {   // get the standard exception from the runtime
                Row.First => ErrZeroRows.First(),
                Row.Single => ErrZeroRows.Single(),
                _ => throw new InvalidOperationException(),
            };
        }

        private static T QueryRowImpl<T>(IDbConnection cnn, Row row, ref CommandDefinition command, Type effectiveType)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand cmd = null;
            IDataReader reader = null;

            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, info.ParamReader);

                if (wasClosed) cnn.Open();
                reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, (row & Row.Single) != 0
                    ? CommandBehavior.SequentialAccess | CommandBehavior.SingleResult // need to allow multiple rows, to check fail condition
                    : CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader

                T result = default;
                if (reader.Read() && reader.FieldCount != 0)
                {
                    // with the CloseConnection flag, so the reader will deal with the connection; we
                    // still need something in the "finally" to ensure that broken SQL still results
                    // in the connection closing itself
                    result = ReadRow<T>(info, identity, ref command, effectiveType, reader);

                    if ((row & Row.Single) != 0 && reader.Read()) ThrowMultipleRows(row);
                    while (reader.Read()) { /* ignore subsequent rows */ }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                while (reader.NextResult()) { /* ignore subsequent result sets */ }
                // happy path; close the reader cleanly - no
                // need for "Cancel" etc
                reader.Dispose();
                reader = null;

                command.OnCompleted();
                return result;
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd.Cancel(); }
                        catch { /* don't spoil the existing exception */ }
                    }
                    reader.Dispose();
                }
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        /// <summary>
        /// Shared value deserialization path for QueryRowImpl and QueryRowAsync
        /// </summary>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T ReadRow<T>(CacheInfo info, Identity identity, ref CommandDefinition command, Type effectiveType, IDataReader reader)
        {
            var tuple = info.Deserializer;
            int hash = GetColumnHash(reader);
            if (tuple.Func == null || tuple.Hash != hash)
            {
                tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                if (command.AddToCache) SetQueryCache(identity, info);
            }

            var func = tuple.Func;
            object val = func(reader);
            return GetValue<T>(reader, effectiveType, val);
        }
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T GetValue<T>(IDataReader reader, Type effectiveType, object val)
        {
            if (val is T tVal)
            {
                return tVal;
            }
            else if (val == null && (!effectiveType.IsValueType || Nullable.GetUnderlyingType(effectiveType) != null))
            {
                return default;
            }
            else
            {
                try
                {
                    var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                    return (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    ThrowDataException(ex, 0, reader, val);
#pragma warning restore CS0618 // Type or member is obsolete
                    return default; // For the compiler - we've already thrown
                }
            }
        }

        /// <summary>
        /// Perform a multi-mapping query with 2 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 3 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 4 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 5 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 6 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with 7 input types. If you need more types -> use Query with Type[] parameter.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        /// <summary>
        /// Perform a multi-mapping query with an arbitrary number of input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="types">Array of types in the recordset.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static IEnumerable<TReturn> Query<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var results = MultiMapImpl(cnn, command, types, map, splitOn, null, null, true);
            return buffered ? results.ToList() : results;
        }

        private static IEnumerable<TReturn> MultiMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            this IDbConnection cnn, string sql, Delegate map, object param, IDbTransaction transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var results = MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, command, map, splitOn, null, null, true);
            return buffered ? results.ToList() : results;
        }

        private static IEnumerable<TReturn> MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Delegate map, string splitOn, IDataReader reader, Identity identity, bool finalize)
        {
            object param = command.Parameters;
            identity ??= new Identity<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(command.CommandText, command.CommandType, cnn, typeof(TFirst), param?.GetType());
            CacheInfo cinfo = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand ownedCommand = null;
            IDataReader ownedReader = null;

            bool wasClosed = cnn?.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cinfo.ParamReader);
                    if (wasClosed) cnn.Open();
                    ownedReader = ExecuteReaderWithFlagsFallback(ownedCommand, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    reader = ownedReader;
                }
                var deserializer = default(DeserializerState);
                Func<IDataReader, object>[] otherDeserializers;

                int hash = GetColumnHash(reader);
                if ((deserializer = cinfo.Deserializer).Func == null || (otherDeserializers = cinfo.OtherDeserializers) == null || hash != deserializer.Hash)
                {
                    var deserializers = GenerateDeserializers(identity, splitOn, reader);
                    deserializer = cinfo.Deserializer = new DeserializerState(hash, deserializers[0]);
                    otherDeserializers = cinfo.OtherDeserializers = deserializers.Skip(1).ToArray();
                    if (command.AddToCache) SetQueryCache(identity, cinfo);
                }

                Func<IDataReader, TReturn> mapIt = GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(deserializer.Func, otherDeserializers, map);

                if (mapIt != null)
                {
                    while (reader.Read())
                    {
                        yield return mapIt(reader);
                    }
                    if (finalize)
                    {
                        while (reader.NextResult()) { /* ignore remaining result sets */ }
                        command.OnCompleted();
                    }
                }
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed) cnn.Close();
                }
            }
        }

        private static CommandBehavior GetBehavior(bool close, CommandBehavior @default)
        {
            return (close ? (@default | CommandBehavior.CloseConnection) : @default) & AllowedCommandBehaviors;
        }

        private static IEnumerable<TReturn> MultiMapImpl<TReturn>(this IDbConnection cnn, CommandDefinition command, Type[] types, Func<object[], TReturn> map, string splitOn, IDataReader reader, Identity identity, bool finalize)
        {
            if (types.Length < 1)
            {
                throw new ArgumentException("you must provide at least one type to deserialize");
            }

            object param = command.Parameters;
            identity ??= new IdentityWithTypes(command.CommandText, command.CommandType, cnn, types[0], param?.GetType(), types);
            CacheInfo cinfo = GetCacheInfo(identity, param, command.AddToCache);

            IDbCommand ownedCommand = null;
            IDataReader ownedReader = null;

            bool wasClosed = cnn?.State == ConnectionState.Closed;
            try
            {
                if (reader == null)
                {
                    ownedCommand = command.SetupCommand(cnn, cinfo.ParamReader);
                    if (wasClosed) cnn.Open();
                    ownedReader = ExecuteReaderWithFlagsFallback(ownedCommand, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult);
                    reader = ownedReader;
                }
                DeserializerState deserializer;
                Func<IDataReader, object>[] otherDeserializers;

                int hash = GetColumnHash(reader);
                if ((deserializer = cinfo.Deserializer).Func == null || (otherDeserializers = cinfo.OtherDeserializers) == null || hash != deserializer.Hash)
                {
                    var deserializers = GenerateDeserializers(identity, splitOn, reader);
                    deserializer = cinfo.Deserializer = new DeserializerState(hash, deserializers[0]);
                    otherDeserializers = cinfo.OtherDeserializers = deserializers.Skip(1).ToArray();
                    SetQueryCache(identity, cinfo);
                }

                Func<IDataReader, TReturn> mapIt = GenerateMapper(types.Length, deserializer.Func, otherDeserializers, map);

                if (mapIt != null)
                {
                    while (reader.Read())
                    {
                        yield return mapIt(reader);
                    }
                    if (finalize)
                    {
                        while (reader.NextResult()) { /* ignore subsequent result sets */ }
                        command.OnCompleted();
                    }
                }
            }
            finally
            {
                try
                {
                    ownedReader?.Dispose();
                }
                finally
                {
                    ownedCommand?.Dispose();
                    if (wasClosed) cnn.Close();
                }
            }
        }

        private static Func<IDataReader, TReturn> GenerateMapper<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, object map)
            => otherDeserializers.Length switch
            {
                1 => r => ((Func<TFirst, TSecond, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r)),
                2 => r => ((Func<TFirst, TSecond, TThird, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r)),
                3 => r => ((Func<TFirst, TSecond, TThird, TFourth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r)),
                4 => r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r)),
                5 => r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r), (TSixth)otherDeserializers[4](r)),
                6 => r => ((Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)map)((TFirst)deserializer(r), (TSecond)otherDeserializers[0](r), (TThird)otherDeserializers[1](r), (TFourth)otherDeserializers[2](r), (TFifth)otherDeserializers[3](r), (TSixth)otherDeserializers[4](r), (TSeventh)otherDeserializers[5](r)),
                _ => throw new NotSupportedException(),
            };

        private static Func<IDataReader, TReturn> GenerateMapper<TReturn>(int length, Func<IDataReader, object> deserializer, Func<IDataReader, object>[] otherDeserializers, Func<object[], TReturn> map)
        {
            return r =>
            {
                var objects = new object[length];
                objects[0] = deserializer(r);

                for (var i = 1; i < length; ++i)
                {
                    objects[i] = otherDeserializers[i - 1](r);
                }

                return map(objects);
            };
        }

        private static Func<IDataReader, object>[] GenerateDeserializers(Identity identity, string splitOn, IDataReader reader)
        {
            var deserializers = new List<Func<IDataReader, object>>();
            var splits = splitOn.Split(',').Select(s => s.Trim()).ToArray();
            bool isMultiSplit = splits.Length > 1;

            int typeCount = identity.TypeCount;
            if (identity.GetType(0) == typeof(object))
            {
                // we go left to right for dynamic multi-mapping so that the madness of TestMultiMappingVariations
                // is supported
                bool first = true;
                int currentPos = 0;
                int splitIdx = 0;
                string currentSplit = splits[splitIdx];

                for (int i = 0; i < typeCount; i++)
                {
                    Type type = identity.GetType(i);
                    if (type == typeof(DontMap))
                    {
                        break;
                    }

                    int splitPoint = GetNextSplitDynamic(currentPos, currentSplit, reader);
                    if (isMultiSplit && splitIdx < splits.Length - 1)
                    {
                        currentSplit = splits[++splitIdx];
                    }
                    deserializers.Add(GetDeserializer(type, reader, currentPos, splitPoint - currentPos, !first));
                    currentPos = splitPoint;
                    first = false;
                }
            }
            else
            {
                // in this we go right to left through the data reader in order to cope with properties that are
                // named the same as a subsequent primary key that we split on
                int currentPos = reader.FieldCount;
                int splitIdx = splits.Length - 1;
                var currentSplit = splits[splitIdx];
                for (var typeIdx = typeCount - 1; typeIdx >= 0; --typeIdx)
                {
                    var type = identity.GetType(typeIdx);
                    if (type == typeof(DontMap))
                    {
                        continue;
                    }

                    int splitPoint = 0;
                    if (typeIdx > 0)
                    {
                        splitPoint = GetNextSplit(currentPos, currentSplit, reader);
                        if (isMultiSplit && splitIdx > 0)
                        {
                            currentSplit = splits[--splitIdx];
                        }
                    }

                    deserializers.Add(GetDeserializer(type, reader, splitPoint, currentPos - splitPoint, typeIdx > 0));
                    currentPos = splitPoint;
                }

                deserializers.Reverse();
            }

            return deserializers.ToArray();
        }

        private static int GetNextSplitDynamic(int startIdx, string splitOn, IDataReader reader)
        {
            if (startIdx == reader.FieldCount)
            {
                throw MultiMapException(reader);
            }

            if (splitOn == "*")
            {
                return ++startIdx;
            }

            for (var i = startIdx + 1; i < reader.FieldCount; ++i)
            {
                if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return reader.FieldCount;
        }

        private static int GetNextSplit(int startIdx, string splitOn, IDataReader reader)
        {
            if (splitOn == "*")
            {
                return --startIdx;
            }

            for (var i = startIdx - 1; i > 0; --i)
            {
                if (string.Equals(splitOn, reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            throw MultiMapException(reader);
        }

        private static CacheInfo GetCacheInfo(Identity identity, object exampleParameters, bool addToCache)
        {
            if (!TryGetQueryCache(identity, out CacheInfo info))
            {
                if (GetMultiExec(exampleParameters) != null)
                {
                    throw new InvalidOperationException("An enumerable sequence of parameters (arrays, lists, etc) is not allowed in this context");
                }
                info = new CacheInfo();
                if (identity.parametersType != null)
                {
                    Action<IDbCommand, object> reader;
                    if (exampleParameters is IDynamicParameters)
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

        private static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            // dynamic is passed in as Object ... by c# design
            if (type == typeof(object) || type == typeof(DapperRow))
            {
                return GetDapperRowDeserializer(reader, startBound, length, returnNullIfFirstMissing);
            }
            Type underlyingType = null;
            if (!(typeMap.ContainsKey(type) || type.IsEnum || type.FullName == LinqBinary
                || (type.IsValueType && (underlyingType = Nullable.GetUnderlyingType(type)) != null && underlyingType.IsEnum)))
            {
                if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
                {
                    return GetHandlerDeserializer(handler, type, startBound);
                }
                return GetTypeDeserializer(type, reader, startBound, length, returnNullIfFirstMissing);
            }
            return GetStructDeserializer(type, underlyingType ?? type, startBound);
        }

        private static Func<IDataReader, object> GetHandlerDeserializer(ITypeHandler handler, Type type, int startBound)
        {
            return reader => handler.Parse(type, reader.GetValue(startBound));
        }

        private static Exception MultiMapException(IDataRecord reader)
        {
            bool hasFields = false;
            try { hasFields = reader != null && reader.FieldCount != 0; }
            catch { /* don't throw when trying to throw */ }
            if (hasFields)
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                return new ArgumentException("When using the multi-mapping APIs ensure you set the splitOn param if you have keys other than Id", "splitOn");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }
            else
            {
                return new InvalidOperationException("No columns were selected");
            }
        }

        internal static Func<IDataReader, object> GetDapperRowDeserializer(IDataRecord reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            var fieldCount = reader.FieldCount;
            if (length == -1)
            {
                length = fieldCount - startBound;
            }

            if (fieldCount <= startBound)
            {
                throw MultiMapException(reader);
            }

            var effectiveFieldCount = Math.Min(fieldCount - startBound, length);

            DapperTable table = null;

            return
                r =>
                {
                    if (table == null)
                    {
                        string[] names = new string[effectiveFieldCount];
                        for (int i = 0; i < effectiveFieldCount; i++)
                        {
                            names[i] = r.GetName(i + startBound);
                        }
                        table = new DapperTable(names);
                    }

                    var values = new object[effectiveFieldCount];

                    if (returnNullIfFirstMissing)
                    {
                        values[0] = r.GetValue(startBound);
                        if (values[0] is DBNull)
                        {
                            return null;
                        }
                    }

                    if (startBound == 0)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            object val = r.GetValue(i);
                            values[i] = val is DBNull ? null : val;
                        }
                    }
                    else
                    {
                        var begin = returnNullIfFirstMissing ? 1 : 0;
                        for (var iter = begin; iter < effectiveFieldCount; ++iter)
                        {
                            object obj = r.GetValue(iter + startBound);
                            values[iter] = obj is DBNull ? null : obj;
                        }
                    }
                    return new DapperRow(table, values);
                };
        }
        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="value">The object to convert to a character.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static char ReadChar(object value)
        {
            if (value == null || value is DBNull) throw new ArgumentNullException(nameof(value));
            if (value is string s && s.Length == 1) return s[0];
            if (value is char c) return c;
            throw new ArgumentException("A single-character was expected", nameof(value));
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="value">The object to convert to a character.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static char? ReadNullableChar(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is string s && s.Length == 1) return s[0];
            if (value is char c) return c;
            throw new ArgumentException("A single-character was expected", nameof(value));
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="parameters">The parameter collection to search in.</param>
        /// <param name="command">The command for this fetch.</param>
        /// <param name="name">The name of the parameter to get.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteInternalUsageOnly, true)]
        public static IDbDataParameter FindOrAddParameter(IDataParameterCollection parameters, IDbCommand command, string name)
        {
            IDbDataParameter result;
            if (parameters.Contains(name))
            {
                result = (IDbDataParameter)parameters[name];
            }
            else
            {
                result = command.CreateParameter();
                result.ParameterName = name;
                parameters.Add(result);
            }
            return result;
        }

        internal static int GetListPaddingExtraCount(int count)
        {
            switch (count)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return 0; // no padding
            }
            if (count < 0) return 0;

            int padFactor;
            if (count <= 150) padFactor = 10;
            else if (count <= 750) padFactor = 50;
            else if (count <= 2000) padFactor = 100; // note: max param count for SQL Server
            else if (count <= 2070) padFactor = 10; // try not to over-pad as we approach that limit
            else if (count <= 2100) return 0; // just don't pad between 2070 and 2100, to minimize the crazy
            else padFactor = 200; // above that, all bets are off!

            // if we have 17, factor = 10; 17 % 10 = 7, we need 3 more
            int intoBlock = count % padFactor;
            return intoBlock == 0 ? 0 : (padFactor - intoBlock);
        }

        private static string GetInListRegex(string name, bool byPosition) => byPosition
            ? (@"(\?)" + Regex.Escape(name) + @"\?(?!\w)(\s+(?i)unknown(?-i))?")
            : ("([?@:]" + Regex.Escape(name) + @")(?!\w)(\s+(?i)unknown(?-i))?");

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

                int splitAt = InListStringSplitCount;
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
                    if (PadListExpansions && !isDbString && lastValue != null)
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

        private static bool TryStringSplit(ref IEnumerable list, int splitAt, string namePrefix, IDbCommand command, bool byPosition)
        {
            if (list == null || splitAt < 0) return false;
            return list switch
            {
                IEnumerable<int> l => TryStringSplit(ref l, splitAt, namePrefix, command, "int", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture))),
                IEnumerable<long> l => TryStringSplit(ref l, splitAt, namePrefix, command, "bigint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture))),
                IEnumerable<short> l => TryStringSplit(ref l, splitAt, namePrefix, command, "smallint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture))),
                IEnumerable<byte> l => TryStringSplit(ref l, splitAt, namePrefix, command, "tinyint", byPosition, (sb, i) => sb.Append(i.ToString(CultureInfo.InvariantCulture))),
                _ => false,
            };
        }

        private static bool TryStringSplit<T>(ref IEnumerable<T> list, int splitAt, string namePrefix, IDbCommand command, string colType, bool byPosition,
            Action<StringBuilder, T> append)
        {
            if (!(list is ICollection<T> typed))
            {
                typed = list.ToList();
                list = typed; // because we still need to be able to iterate it, even if we fail here
            }
            if (typed.Count < splitAt) return false;

            string varName = null;
            var regexIncludingUnknown = GetInListRegex(namePrefix, byPosition);
            var sql = Regex.Replace(command.CommandText, regexIncludingUnknown, match =>
            {
                var variableName = match.Groups[1].Value;
                if (match.Groups[2].Success)
                {
                    // looks like an optimize hint; leave it alone!
                    return match.Value;
                }
                else
                {
                    varName = variableName;
                    return "(select cast([value] as " + colType + ") from string_split(" + variableName + ",','))";
                }
            }, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
            if (varName == null) return false; // couldn't resolve the var!

            command.CommandText = sql;
            var concatenatedParam = command.CreateParameter();
            concatenatedParam.ParameterName = namePrefix;
            concatenatedParam.DbType = DbType.AnsiString;
            concatenatedParam.Size = -1;
            string val;
            using (var iter = typed.GetEnumerator())
            {
                if (iter.MoveNext())
                {
                    var sb = GetStringBuilder();
                    append(sb, iter.Current);
                    while (iter.MoveNext())
                    {
                        append(sb.Append(','), iter.Current);
                    }
                    val = sb.ToString();
                }
                else
                {
                    val = "";
                }
            }
            concatenatedParam.Value = val;
            command.Parameters.Add(concatenatedParam);
            return true;
        }

        /// <summary>
        /// OBSOLETE:仅供内部使用。 使用适当的类型铸造对参数值进行消毒。  
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

        private static IEnumerable<PropertyInfo> FilterParameters(IEnumerable<PropertyInfo> parameters, string sql)
        {
            var list = new List<PropertyInfo>(16);
            foreach (var p in parameters)
            {
                if (Regex.IsMatch(sql, @"[?@:]" + p.Name + @"([^\p{L}\p{N}_]+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant))
                    list.Add(p);
            }
            return list;
        }

        // look for ? / @ / : *by itself*
        private static readonly Regex smellsLikeOleDb = new Regex(@"(?<![\p{L}\p{N}@_])[?@:](?![\p{L}\p{N}@_])", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled),
            literalTokens = new Regex(@"(?<![\p{L}\p{N}_])\{=([\p{L}\p{N}_]+)\}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled),
            pseudoPositional = new Regex(@"\?([\p{L}_][\p{L}\p{N}_]*)\?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Replace all literal tokens with their text form.
        /// </summary>
        /// <param name="parameters">The parameter lookup to do replacements with.</param>
        /// <param name="command">The command to replace parameters in.</param>
        public static void ReplaceLiterals(this IParameterLookup parameters, IDbCommand command)
        {
            var tokens = GetLiteralTokens(command.CommandText);
            if (tokens.Count != 0) { ReplaceLiterals(parameters, command, tokens); }
        }

        internal static readonly MethodInfo format = typeof(SqlDibber).GetMethod("Format", BindingFlags.Public | BindingFlags.Static);

        /// <summary>
        /// Convert numeric values to their string form for SQL literal purposes.
        /// </summary>
        /// <param name="value">The value to get a string for.</param>
        [Obsolete(ObsoleteInternalUsageOnly)]
        public static string Format(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.DBNull:
                        return "null";
                    case TypeCode.Boolean:
                        return ((bool)value) ? "1" : "0";
                    case TypeCode.Byte:
                        return ((byte)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.SByte:
                        return ((sbyte)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt16:
                        return ((ushort)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int16:
                        return ((short)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt32:
                        return ((uint)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int32:
                        return ((int)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.UInt64:
                        return ((ulong)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Int64:
                        return ((long)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Single:
                        return ((float)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Double:
                        return ((double)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Decimal:
                        return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                    default:
                        var multiExec = GetMultiExec(value);
                        if (multiExec != null)
                        {
                            StringBuilder sb = null;
                            bool first = true;
                            foreach (object subval in multiExec)
                            {
                                if (first)
                                {
                                    sb = GetStringBuilder().Append('(');
                                    first = false;
                                }
                                else
                                {
                                    sb.Append(',');
                                }
                                sb.Append(Format(subval));
                            }
                            if (first)
                            {
                                return "(select null where 1=0)";
                            }
                            else
                            {
                                return sb.Append(')').ToStringRecycle();
                            }
                        }
                        throw new NotSupportedException($"The type '{value.GetType().Name}' is not supported for SQL literals.");
                }
            }
        }

        internal static void ReplaceLiterals(IParameterLookup parameters, IDbCommand command, IList<LiteralToken> tokens)
        {
            var sql = command.CommandText;
            foreach (var token in tokens)
            {
                object value = parameters[token.Member];
#pragma warning disable 0618
                string text = Format(value);
#pragma warning restore 0618
                sql = sql.Replace(token.Token, text);
            }
            command.CommandText = sql;
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

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="identity">The identity of the generator.</param>
        /// <param name="checkForDuplicates">Whether to check for duplicates.</param>
        /// <param name="removeUnused">Whether to remove unused parameters.</param>
        public static Action<IDbCommand, object> CreateParamInfoGenerator(Identity identity, bool checkForDuplicates, bool removeUnused) =>
            CreateParamInfoGenerator(identity, checkForDuplicates, removeUnused, GetLiteralTokens(identity.sql));

        private static bool IsValueTuple(Type type) => (type?.IsValueType == true
                                                       && type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
                                                       || (type != null && IsValueTuple(Nullable.GetUnderlyingType(type)));

        internal static Action<IDbCommand, object> CreateParamInfoGenerator(Identity identity, bool checkForDuplicates, bool removeUnused, IList<LiteralToken> literals)
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
                    il.EmitCall(OpCodes.Call, typeof(SqlDibber).GetMethod(nameof(PackListParameters)), null); // stack is [parameters]
                    continue;
                }
                il.Emit(OpCodes.Dup); // stack is now [parameters] [parameters]

                il.Emit(OpCodes.Ldarg_0); // stack is now [parameters] [parameters] [command]

                if (checkForDuplicates)
                {
                    // need to be a little careful about adding; use a utility method
                    il.Emit(OpCodes.Ldstr, prop.Name); // stack is now [parameters] [parameters] [command] [name]
                    il.EmitCall(OpCodes.Call, typeof(SqlDibber).GetMethod(nameof(FindOrAddParameter)), null); // stack is [parameters] [parameter]
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
                        il.Emit(OpCodes.Call, typeof(SqlDibber).GetMethod(nameof(GetDbType), BindingFlags.Static | BindingFlags.Public)); // stack is now [parameters] [[parameters]] [parameter] [parameter] [db-type]
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
                        il.EmitCall(OpCodes.Call, typeof(SqlDibber).GetMethod(nameof(SanitizeParameterValue)), null);
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

        private static readonly Dictionary<TypeCode, MethodInfo> toStrings = new[]
        {
            typeof(bool), typeof(sbyte), typeof(byte), typeof(ushort), typeof(short),
            typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal)
        }.ToDictionary(x => Type.GetTypeCode(x), x => x.GetPublicInstanceMethod(nameof(object.ToString), new[] { typeof(IFormatProvider) }));

        private static MethodInfo GetToString(TypeCode typeCode)
        {
            return toStrings.TryGetValue(typeCode, out MethodInfo method) ? method : null;
        }

        private static readonly MethodInfo StringReplace = typeof(string).GetPublicInstanceMethod(nameof(string.Replace), new Type[] { typeof(string), typeof(string) }),
            InvariantCulture = typeof(CultureInfo).GetProperty(nameof(CultureInfo.InvariantCulture), BindingFlags.Public | BindingFlags.Static).GetGetMethod();

        private static int ExecuteCommand(IDbConnection cnn, ref CommandDefinition command, Action<IDbCommand, object> paramReader)
        {
            IDbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                int result = cmd.ExecuteNonQuery();
                command.OnCompleted();
                return result;
            }
            finally
            {
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
        }

        private static T ExecuteScalarImpl<T>(IDbConnection cnn, ref CommandDefinition command)
        {
            Action<IDbCommand, object> paramReader = null;
            object param = command.Parameters;
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                paramReader = GetCacheInfo(identity, command.Parameters, command.AddToCache).ParamReader;
            }

            IDbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            object result;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                result = cmd.ExecuteScalar();
                command.OnCompleted();
            }
            finally
            {
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
            return Parse<T>(result);
        }

        private static IDataReader ExecuteReaderImpl(IDbConnection cnn, ref CommandDefinition command, CommandBehavior commandBehavior, out IDbCommand cmd)
        {
            Action<IDbCommand, object> paramReader = GetParameterReader(cnn, ref command);
            cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed, disposeCommand = true;
            try
            {
                cmd = command.SetupCommand(cnn, paramReader);
                if (wasClosed) cnn.Open();
                var reader = ExecuteReaderWithFlagsFallback(cmd, wasClosed, commandBehavior);
                wasClosed = false; // don't dispose before giving it to them!
                disposeCommand = false;
                // note: command.FireOutputCallbacks(); would be useless here; parameters come at the **end** of the TDS stream
                return reader;
            }
            finally
            {
                if (wasClosed) cnn.Close();
                if (cmd != null && disposeCommand) cmd.Dispose();
            }
        }

        private static Action<IDbCommand, object> GetParameterReader(IDbConnection cnn, ref CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            CacheInfo info = null;
            if (multiExec != null)
            {
                throw new NotSupportedException("MultiExec is not supported by ExecuteReader");
            }

            // nice and simple
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                info = GetCacheInfo(identity, param, command.AddToCache);
            }
            var paramReader = info?.ParamReader;
            return paramReader;
        }

        private static Func<IDataReader, object> GetStructDeserializer(Type type, Type effectiveType, int index)
        {
            // no point using special per-type handling here; it boils down to the same, plus not all are supported anyway (see: SqlDataReader.GetChar - not supported!)
#pragma warning disable 618
            if (type == typeof(char))
            { // this *does* need special handling, though
                return r => ReadChar(r.GetValue(index));
            }
            if (type == typeof(char?))
            {
                return r => ReadNullableChar(r.GetValue(index));
            }
            if (type.FullName == LinqBinary)
            {
                return r => Activator.CreateInstance(type, r.GetValue(index));
            }
#pragma warning restore 618

            if (effectiveType.IsEnum)
            {   // assume the value is returned as the correct type (int/byte/etc), but box back to the typed enum
                return r =>
                {
                    var val = r.GetValue(index);
                    if (val is float || val is double || val is decimal)
                    {
                        val = Convert.ChangeType(val, Enum.GetUnderlyingType(effectiveType), CultureInfo.InvariantCulture);
                    }
                    return val is DBNull ? null : Enum.ToObject(effectiveType, val);
                };
            }
            if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val is DBNull ? null : handler.Parse(type, val);
                };
            }
            return r =>
            {
                var val = r.GetValue(index);
                return val is DBNull ? null : val;
            };
        }

        private static T Parse<T>(object value)
        {
            if (value is null || value is DBNull) return default;
            if (value is T t) return t;
            var type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                if (value is float || value is double || value is decimal)
                {
                    value = Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
                }
                return (T)Enum.ToObject(type, value);
            }
            if (typeHandlers.TryGetValue(type, out ITypeHandler handler))
            {
                return (T)handler.Parse(type, value);
            }
            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static readonly MethodInfo
                    enumParse = typeof(Enum).GetMethod(nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                    getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length > 0 && p.GetIndexParameters()[0].ParameterType == typeof(int))
                        .Select(p => p.GetGetMethod()).First();

        /// <summary>
        /// Gets type-map for the given type
        /// </summary>
        /// <returns>Type map instance, default is to create new instance of DefaultTypeMap</returns>
#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
        public static Func<Type, ITypeMap> TypeMapProvider = (Type type) => new DefaultTypeMap(type);
#pragma warning restore CA2211 // Non-constant fields should not be visible

        /// <summary>
        /// Gets type-map for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to get a map for.</param>
        /// <returns>Type map implementation, DefaultTypeMap instance if no override present</returns>
        public static ITypeMap GetTypeMap(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var map = (ITypeMap)_typeMaps[type];
            if (map == null)
            {
                lock (_typeMaps)
                {   // double-checked; store this to avoid reflection next time we see this type
                    // since multiple queries commonly use the same domain-entity/DTO/view-model type
                    map = (ITypeMap)_typeMaps[type];

                    if (map == null)
                    {
                        map = TypeMapProvider(type);
                        _typeMaps[type] = map;
                    }
                }
            }
            return map;
        }

        // use Hashtable to get free lockless reading
        private static readonly Hashtable _typeMaps = new Hashtable();

        /// <summary>
        /// Set custom mapping for type deserializers
        /// </summary>
        /// <param name="type">Entity type to override</param>
        /// <param name="map">Mapping rules implementation, null to remove custom map</param>
        public static void SetTypeMap(Type type, ITypeMap map)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (map == null || map is DefaultTypeMap)
            {
                lock (_typeMaps)
                {
                    _typeMaps.Remove(type);
                }
            }
            else
            {
                lock (_typeMaps)
                {
                    _typeMaps[type] = map;
                }
            }

            PurgeQueryCacheByType(type);
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <param name="startBound"></param>
        /// <param name="length"></param>
        /// <param name="returnNullIfFirstMissing"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetTypeDeserializer(
            Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false
        )
        {
            return TypeDeserializerCache.GetReader(type, reader, startBound, length, returnNullIfFirstMissing);
        }

        private static LocalBuilder GetTempLocal(ILGenerator il, ref Dictionary<Type, LocalBuilder> locals, Type type, bool initAndLoad)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            locals ??= new Dictionary<Type, LocalBuilder>();
            if (!locals.TryGetValue(type, out LocalBuilder found))
            {
                found = il.DeclareLocal(type);
                locals.Add(type, found);
            }
            if (initAndLoad)
            {
                il.Emit(OpCodes.Ldloca, found);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloca, found);
                il.Emit(OpCodes.Ldobj, type);
            }
            return found;
        }

        private static Func<IDataReader, object> GetTypeDeserializerImpl(
            Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false
        )
        {
            if (length == -1)
            {
                length = reader.FieldCount - startBound;
            }

            if (reader.FieldCount <= startBound)
            {
                throw MultiMapException(reader);
            }

            var returnType = type.IsValueType ? typeof(object) : type;
            var dm = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString(), returnType, new[] { typeof(IDataReader) }, type, true);
            var il = dm.GetILGenerator();

            if (IsValueTuple(type))
            {
                GenerateValueTupleDeserializer(type, reader, startBound, length, il);
            }
            else
            {
                GenerateDeserializerFromMap(type, reader, startBound, length, returnNullIfFirstMissing, il);
            }

            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(IDataReader), returnType);
            return (Func<IDataReader, object>)dm.CreateDelegate(funcType);
        }

        private static void GenerateValueTupleDeserializer(Type valueTupleType, IDataReader reader, int startBound, int length, ILGenerator il)
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(valueTupleType);
            var currentValueTupleType = nullableUnderlyingType ?? valueTupleType;

            var constructors = new List<ConstructorInfo>();
            var languageTupleElementTypes = new List<Type>();

            while (true)
            {
                var arity = int.Parse(currentValueTupleType.Name.Substring("ValueTuple`".Length), CultureInfo.InvariantCulture);
                var constructorParameterTypes = new Type[arity];
                var restField = (FieldInfo)null;

                foreach (var field in currentValueTupleType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (field.Name == "Rest")
                    {
                        restField = field;
                    }
                    else if (field.Name.StartsWith("Item", StringComparison.Ordinal))
                    {
                        var elementNumber = int.Parse(field.Name.Substring("Item".Length), CultureInfo.InvariantCulture);
                        constructorParameterTypes[elementNumber - 1] = field.FieldType;
                    }
                }

                var itemFieldCount = constructorParameterTypes.Length;
                if (restField != null) itemFieldCount--;

                for (var i = 0; i < itemFieldCount; i++)
                {
                    languageTupleElementTypes.Add(constructorParameterTypes[i]);
                }

                if (restField != null)
                {
                    constructorParameterTypes[constructorParameterTypes.Length - 1] = restField.FieldType;
                }

                constructors.Add(currentValueTupleType.GetConstructor(constructorParameterTypes));

                if (restField is null) break;

                currentValueTupleType = restField.FieldType;
                if (!IsValueTuple(currentValueTupleType))
                {
                    throw new InvalidOperationException("The Rest field of a ValueTuple must contain a nested ValueTuple of arity 1 or greater.");
                }
            }

            var stringEnumLocal = (LocalBuilder)null;

            for (var i = 0; i < languageTupleElementTypes.Count; i++)
            {
                var targetType = languageTupleElementTypes[i];

                if (i < length)
                {
                    LoadReaderValueOrBranchToDBNullLabel(
                        il,
                        startBound + i,
                        ref stringEnumLocal,
                        valueCopyLocal: null,
                        reader.GetFieldType(startBound + i),
                        targetType,
                        out var isDbNullLabel);

                    var finishLabel = il.DefineLabel();
                    il.Emit(OpCodes.Br_S, finishLabel);
                    il.MarkLabel(isDbNullLabel);
                    il.Emit(OpCodes.Pop);

                    LoadDefaultValue(il, targetType);

                    il.MarkLabel(finishLabel);
                }
                else
                {
                    LoadDefaultValue(il, targetType);
                }
            }

            for (var i = constructors.Count - 1; i >= 0; i--)
            {
                il.Emit(OpCodes.Newobj, constructors[i]);
            }

            if (nullableUnderlyingType != null)
            {
                var nullableTupleConstructor = valueTupleType.GetConstructor(new[] { nullableUnderlyingType });

                il.Emit(OpCodes.Newobj, nullableTupleConstructor);
            }

            il.Emit(OpCodes.Box, valueTupleType);
            il.Emit(OpCodes.Ret);
        }

        private static void GenerateDeserializerFromMap(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing, ILGenerator il)
        {
            var currentIndexDiagnosticLocal = il.DeclareLocal(typeof(int));
            var returnValueLocal = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, currentIndexDiagnosticLocal);

            var names = Enumerable.Range(startBound, length).Select(i => reader.GetName(i)).ToArray();

            ITypeMap typeMap = GetTypeMap(type);

            int index = startBound;
            ConstructorInfo specializedConstructor = null;

            bool supportInitialize = false;
            Dictionary<Type, LocalBuilder> structLocals = null;
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, returnValueLocal);
                il.Emit(OpCodes.Initobj, type);
            }
            else
            {
                var types = new Type[length];
                for (int i = startBound; i < startBound + length; i++)
                {
                    types[i - startBound] = reader.GetFieldType(i);
                }

                var explicitConstr = typeMap.FindExplicitConstructor();
                if (explicitConstr != null)
                {
                    var consPs = explicitConstr.GetParameters();
                    foreach (var p in consPs)
                    {
                        if (!p.ParameterType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldnull);
                        }
                        else
                        {
                            GetTempLocal(il, ref structLocals, p.ParameterType, true);
                        }
                    }

                    il.Emit(OpCodes.Newobj, explicitConstr);
                    il.Emit(OpCodes.Stloc, returnValueLocal);
                    supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(type);
                    if (supportInitialize)
                    {
                        il.Emit(OpCodes.Ldloc, returnValueLocal);
                        il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.BeginInit)), null);
                    }
                }
                else
                {
                    var ctor = typeMap.FindConstructor(names, types);
                    if (ctor == null)
                    {
                        string proposedTypes = "(" + string.Join(", ", types.Select((t, i) => t.FullName + " " + names[i]).ToArray()) + ")";
                        throw new InvalidOperationException($"A parameterless default constructor or one matching signature {proposedTypes} is required for {type.FullName} materialization");
                    }

                    if (ctor.GetParameters().Length == 0)
                    {
                        il.Emit(OpCodes.Newobj, ctor);
                        il.Emit(OpCodes.Stloc, returnValueLocal);
                        supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(type);
                        if (supportInitialize)
                        {
                            il.Emit(OpCodes.Ldloc, returnValueLocal);
                            il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.BeginInit)), null);
                        }
                    }
                    else
                    {
                        specializedConstructor = ctor;
                    }
                }
            }

            il.BeginExceptionBlock();
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca, returnValueLocal); // [target]
            }
            else if (specializedConstructor == null)
            {
                il.Emit(OpCodes.Ldloc, returnValueLocal); // [target]
            }

            var members = (specializedConstructor != null
                ? names.Select(n => typeMap.GetConstructorParameter(specializedConstructor, n))
                : names.Select(n => typeMap.GetMember(n))).ToList();

            // stack is now [target]
            bool first = true;
            var allDone = il.DefineLabel();
            var stringEnumLocal = (LocalBuilder)null;
            var valueCopyDiagnosticLocal = il.DeclareLocal(typeof(object));
            bool applyNullSetting = ApplyNullValues;
            foreach (var item in members)
            {
                if (item != null)
                {
                    if (specializedConstructor == null)
                        il.Emit(OpCodes.Dup); // stack is now [target][target]
                    Label finishLabel = il.DefineLabel();
                    Type memberType = item.MemberType;

                    // Save off the current index for access if an exception is thrown
                    EmitInt32(il, index);
                    il.Emit(OpCodes.Stloc, currentIndexDiagnosticLocal);

                    LoadReaderValueOrBranchToDBNullLabel(il, index, ref stringEnumLocal, valueCopyDiagnosticLocal, reader.GetFieldType(index), memberType, out var isDbNullLabel);

                    if (specializedConstructor == null)
                    {
                        // Store the value in the property/field
                        if (item.Property != null)
                        {
                            il.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
                        }
                        else
                        {
                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
                        }
                    }

                    il.Emit(OpCodes.Br_S, finishLabel); // stack is now [target]

                    il.MarkLabel(isDbNullLabel); // incoming stack: [target][target][value]
                    if (specializedConstructor != null)
                    {
                        il.Emit(OpCodes.Pop);
                        LoadDefaultValue(il, item.MemberType);
                    }
                    else if (applyNullSetting && (!memberType.IsValueType || Nullable.GetUnderlyingType(memberType) != null))
                    {
                        il.Emit(OpCodes.Pop); // stack is now [target][target]
                        // can load a null with this value
                        if (memberType.IsValueType)
                        { // must be Nullable<T> for some T
                            GetTempLocal(il, ref structLocals, memberType, true); // stack is now [target][target][null]
                        }
                        else
                        { // regular reference-type
                            il.Emit(OpCodes.Ldnull); // stack is now [target][target][null]
                        }

                        // Store the value in the property/field
                        if (item.Property != null)
                        {
                            il.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
                            // stack is now [target]
                        }
                        else
                        {
                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Pop); // stack is now [target][target]
                        il.Emit(OpCodes.Pop); // stack is now [target]
                    }

                    if (first && returnNullIfFirstMissing)
                    {
                        il.Emit(OpCodes.Pop);
                        il.Emit(OpCodes.Ldnull); // stack is now [null]
                        il.Emit(OpCodes.Stloc, returnValueLocal);
                        il.Emit(OpCodes.Br, allDone);
                    }

                    il.MarkLabel(finishLabel);
                }
                first = false;
                index++;
            }
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                if (specializedConstructor != null)
                {
                    il.Emit(OpCodes.Newobj, specializedConstructor);
                }
                il.Emit(OpCodes.Stloc, returnValueLocal); // stack is empty
                if (supportInitialize)
                {
                    il.Emit(OpCodes.Ldloc, returnValueLocal);
                    il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.EndInit)), null);
                }
            }
            il.MarkLabel(allDone);
            il.BeginCatchBlock(typeof(Exception)); // stack is Exception
            il.Emit(OpCodes.Ldloc, currentIndexDiagnosticLocal); // stack is Exception, index
            il.Emit(OpCodes.Ldarg_0); // stack is Exception, index, reader
            il.Emit(OpCodes.Ldloc, valueCopyDiagnosticLocal); // stack is Exception, index, reader, value
            il.EmitCall(OpCodes.Call, typeof(SqlDibber).GetMethod(nameof(ThrowDataException)), null);
            il.EndExceptionBlock();

            il.Emit(OpCodes.Ldloc, returnValueLocal); // stack is [rval]
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            il.Emit(OpCodes.Ret);
        }

        private static void LoadDefaultValue(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                var local = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, local);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
        }

        private static void LoadReaderValueOrBranchToDBNullLabel(ILGenerator il, int index, ref LocalBuilder stringEnumLocal, LocalBuilder valueCopyLocal, Type colType, Type memberType, out Label isDbNullLabel)
        {
            isDbNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0); // stack is now [...][reader]
            EmitInt32(il, index); // stack is now [...][reader][index]
            il.Emit(OpCodes.Callvirt, getItem); // stack is now [...][value-as-object]

            if (valueCopyLocal != null)
            {
                il.Emit(OpCodes.Dup); // stack is now [...][value-as-object][value-as-object]
                il.Emit(OpCodes.Stloc, valueCopyLocal); // stack is now [...][value-as-object]
            }

            if (memberType == typeof(char) || memberType == typeof(char?))
            {
                il.EmitCall(OpCodes.Call, typeof(SqlDibber).GetMethod(
                    memberType == typeof(char) ? nameof(ReadChar) : nameof(ReadNullableChar), BindingFlags.Static | BindingFlags.Public), null); // stack is now [...][typed-value]
            }
            else
            {
                il.Emit(OpCodes.Dup); // stack is now [...][value-as-object][value-as-object]
                il.Emit(OpCodes.Isinst, typeof(DBNull)); // stack is now [...][value-as-object][DBNull or null]
                il.Emit(OpCodes.Brtrue_S, isDbNullLabel); // stack is now [...][value-as-object]

                // unbox nullable enums as the primitive, i.e. byte etc

                var nullUnderlyingType = Nullable.GetUnderlyingType(memberType);
                var unboxType = nullUnderlyingType?.IsEnum == true ? nullUnderlyingType : memberType;

                if (unboxType.IsEnum)
                {
                    Type numericType = Enum.GetUnderlyingType(unboxType);
                    if (colType == typeof(string))
                    {
                        if (stringEnumLocal == null)
                        {
                            stringEnumLocal = il.DeclareLocal(typeof(string));
                        }
                        il.Emit(OpCodes.Castclass, typeof(string)); // stack is now [...][string]
                        il.Emit(OpCodes.Stloc, stringEnumLocal); // stack is now [...]
                        il.Emit(OpCodes.Ldtoken, unboxType); // stack is now [...][enum-type-token]
                        il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null);// stack is now [...][enum-type]
                        il.Emit(OpCodes.Ldloc, stringEnumLocal); // stack is now [...][enum-type][string]
                        il.Emit(OpCodes.Ldc_I4_1); // stack is now [...][enum-type][string][true]
                        il.EmitCall(OpCodes.Call, enumParse, null); // stack is now [...][enum-as-object]
                        il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [...][typed-value]
                    }
                    else
                    {
                        FlexibleConvertBoxedFromHeadOfStack(il, colType, unboxType, numericType);
                    }

                    if (nullUnderlyingType != null)
                    {
                        il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [...][typed-value]
                    }
                }
                else if (memberType.FullName == LinqBinary)
                {
                    il.Emit(OpCodes.Unbox_Any, typeof(byte[])); // stack is now [...][byte-array]
                    il.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[] { typeof(byte[]) }));// stack is now [...][binary]
                }
                else
                {
                    TypeCode dataTypeCode = Type.GetTypeCode(colType), unboxTypeCode = Type.GetTypeCode(unboxType);
                    bool hasTypeHandler;
                    if ((hasTypeHandler = typeHandlers.ContainsKey(unboxType)) || colType == unboxType || dataTypeCode == unboxTypeCode || dataTypeCode == Type.GetTypeCode(nullUnderlyingType))
                    {
                        if (hasTypeHandler)
                        {
#pragma warning disable 618
                            il.EmitCall(OpCodes.Call, typeof(TypeHandlerCache<>).MakeGenericType(unboxType).GetMethod(nameof(TypeHandlerCache<int>.Parse)), null); // stack is now [...][typed-value]
#pragma warning restore 618
                        }
                        else
                        {
                            il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [...][typed-value]
                        }
                    }
                    else
                    {
                        // not a direct match; need to tweak the unbox
                        FlexibleConvertBoxedFromHeadOfStack(il, colType, nullUnderlyingType ?? unboxType, null);
                        if (nullUnderlyingType != null)
                        {
                            il.Emit(OpCodes.Newobj, unboxType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [...][typed-value]
                        }
                    }
                }
            }
        }

        private static void FlexibleConvertBoxedFromHeadOfStack(ILGenerator il, Type from, Type to, Type via)
        {
            MethodInfo op;
            if (from == (via ?? to))
            {
                il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
            }
            else if ((op = GetOperator(from, to)) != null)
            {
                // this is handy for things like decimal <===> double
                il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][data-typed-value]
                il.Emit(OpCodes.Call, op); // stack is now [target][target][typed-value]
            }
            else
            {
                bool handled = false;
                OpCode opCode = default;
                switch (Type.GetTypeCode(from))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        handled = true;
                        switch (Type.GetTypeCode(via ?? to))
                        {
                            case TypeCode.Byte:
                                opCode = OpCodes.Conv_Ovf_I1_Un; break;
                            case TypeCode.SByte:
                                opCode = OpCodes.Conv_Ovf_I1; break;
                            case TypeCode.UInt16:
                                opCode = OpCodes.Conv_Ovf_I2_Un; break;
                            case TypeCode.Int16:
                                opCode = OpCodes.Conv_Ovf_I2; break;
                            case TypeCode.UInt32:
                                opCode = OpCodes.Conv_Ovf_I4_Un; break;
                            case TypeCode.Boolean: // boolean is basically an int, at least at this level
                            case TypeCode.Int32:
                                opCode = OpCodes.Conv_Ovf_I4; break;
                            case TypeCode.UInt64:
                                opCode = OpCodes.Conv_Ovf_I8_Un; break;
                            case TypeCode.Int64:
                                opCode = OpCodes.Conv_Ovf_I8; break;
                            case TypeCode.Single:
                                opCode = OpCodes.Conv_R4; break;
                            case TypeCode.Double:
                                opCode = OpCodes.Conv_R8; break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                }
                if (handled)
                {
                    il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][col-typed-value]
                    il.Emit(opCode); // stack is now [target][target][typed-value]
                    if (to == typeof(bool))
                    { // compare to zero; I checked "csc" - this is the trick it uses; nice
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldtoken, via ?? to); // stack is now [target][target][value][member-type-token]
                    il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null); // stack is now [target][target][value][member-type]
                    il.EmitCall(OpCodes.Call, InvariantCulture, null); // stack is now [target][target][value][member-type][culture]
                    il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod(nameof(Convert.ChangeType), new Type[] { typeof(object), typeof(Type), typeof(IFormatProvider) }), null); // stack is now [target][target][boxed-member-type-value]
                    il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
                }
            }
        }

        private static MethodInfo GetOperator(Type from, Type to)
        {
            if (to == null) return null;
            MethodInfo[] fromMethods, toMethods;
            return ResolveOperator(fromMethods = from.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(toMethods = to.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(fromMethods, from, to, "op_Explicit")
                ?? ResolveOperator(toMethods, from, to, "op_Explicit");
        }

        private static MethodInfo ResolveOperator(MethodInfo[] methods, Type from, Type to, string name)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != name || methods[i].ReturnType != to) continue;
                var args = methods[i].GetParameters();
                if (args.Length != 1 || args[0].ParameterType != from) continue;
                return methods[i];
            }
            return null;
        }

        /// <summary>
        /// Throws a data exception, only used internally
        /// </summary>
        /// <param name="ex">The exception to throw.</param>
        /// <param name="index">The index the exception occurred at.</param>
        /// <param name="reader">The reader the exception occurred in.</param>
        /// <param name="value">The value that caused the exception.</param>
        [Obsolete(ObsoleteInternalUsageOnly, false)]
        public static void ThrowDataException(Exception ex, int index, IDataReader reader, object value)
        {
            Exception toThrow;
            try
            {
                string name = "(n/a)", formattedValue = "(n/a)";
                if (reader != null && index >= 0 && index < reader.FieldCount)
                {
                    name = reader.GetName(index);
                    if (name == string.Empty)
                    {
                        // Otherwise we throw (=value) below, which isn't intuitive
                        name = "(Unnamed Column)";
                    }
                    try
                    {
                        if (value == null || value is DBNull)
                        {
                            formattedValue = "<null>";
                        }
                        else
                        {
                            formattedValue = Convert.ToString(value) + " - " + Type.GetTypeCode(value.GetType());
                        }
                    }
                    catch (Exception valEx)
                    {
                        formattedValue = valEx.Message;
                    }
                }
                toThrow = new DataException($"Error parsing column {index} ({name}={formattedValue})", ex);
            }
            catch
            { // throw the **original** exception, wrapped as DataException
                toThrow = new DataException(ex.Message, ex);
            }
            throw toThrow;
        }

        private static void EmitInt32(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        /// <summary>
        /// How should connection strings be compared for equivalence? Defaults to StringComparer.Ordinal.
        /// Providing a custom implementation can be useful for allowing multi-tenancy databases with identical
        /// schema to share strategies. Note that usual equivalence rules apply: any equivalent connection strings
        /// <b>MUST</b> yield the same hash-code.
        /// </summary>
        public static IEqualityComparer<string> ConnectionStringComparer
        {
            get { return connectionStringComparer; }
            set { connectionStringComparer = value ?? StringComparer.Ordinal; }
        }

        private static IEqualityComparer<string> connectionStringComparer = StringComparer.Ordinal;

        /// <summary>
        /// Key used to indicate the type name associated with a DataTable.
        /// </summary>
        private const string DataTableTypeNameKey = "dapper:TypeName";

        /// <summary>
        /// Used to pass a DataTable as a <see cref="TableValuedParameter"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to create this parameter for.</param>
        /// <param name="typeName">The name of the type this parameter is for.</param>
        public static ICustomQueryParameter AsTableValuedParameter(this DataTable table, string typeName = null) =>
            new TableValuedParameter(table, typeName);

        /// <summary>
        /// Associate a DataTable with a type name.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> that does with the <paramref name="typeName"/>.</param>
        /// <param name="typeName">The name of the type this table is for.</param>
        public static void SetTypeName(this DataTable table, string typeName)
        {
            if (table != null)
            {
                if (string.IsNullOrEmpty(typeName))
                    table.ExtendedProperties.Remove(DataTableTypeNameKey);
                else
                    table.ExtendedProperties[DataTableTypeNameKey] = typeName;
            }
        }

        /// <summary>
        /// Fetch the type name associated with a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> that has a type name associated with it.</param>
        public static string GetTypeName(this DataTable table) =>
            table?.ExtendedProperties[DataTableTypeNameKey] as string;

        /// <summary>
        /// Used to pass a IEnumerable&lt;SqlDataRecord&gt; as a TableValuedParameter.
        /// </summary>
        /// <param name="list">The list of records to convert to TVPs.</param>
        /// <param name="typeName">The sql parameter type name.</param>
        public static ICustomQueryParameter AsTableValuedParameter<T>(this IEnumerable<T> list, string typeName = null) where T : IDataRecord =>
            new SqlDataRecordListTVPParameter<T>(list, typeName);

        // one per thread
        [ThreadStatic]
        private static StringBuilder perThreadStringBuilderCache;
        private static StringBuilder GetStringBuilder()
        {
            var tmp = perThreadStringBuilderCache;
            if (tmp != null)
            {
                perThreadStringBuilderCache = null;
                tmp.Length = 0;
                return tmp;
            }
            return new StringBuilder();
        }

        private static string ToStringRecycle(this StringBuilder obj)
        {
            if (obj == null) return "";
            var s = obj.ToString();
            perThreadStringBuilderCache ??= obj;
            return s;
        }
        #endregion
        #region // SqlMapper.Async
        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryAsync<dynamic>(cnn, typeof(DapperRow), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered, default));

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, CommandDefinition command) =>
            QueryAsync<dynamic>(cnn, typeof(DapperRow), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<dynamic> QueryFirstAsync(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<dynamic>(cnn, Row.First, typeof(DapperRow), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<dynamic> QueryFirstOrDefaultAsync(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<dynamic>(cnn, Row.FirstOrDefault, typeof(DapperRow), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<dynamic> QuerySingleAsync(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<dynamic>(cnn, Row.Single, typeof(DapperRow), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static Task<dynamic> QuerySingleOrDefaultAsync(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<dynamic>(cnn, Row.SingleOrDefault, typeof(DapperRow), command);

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryAsync<T>(cnn, typeof(T), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> QueryFirstAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<T>(cnn, Row.First, typeof(T), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<T>(cnn, Row.FirstOrDefault, typeof(T), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> QuerySingleAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<T>(cnn, Row.Single, typeof(T), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<T> QuerySingleOrDefaultAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<T>(cnn, Row.SingleOrDefault, typeof(T), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<dynamic> QueryFirstAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<dynamic>(cnn, Row.First, typeof(DapperRow), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<dynamic> QueryFirstOrDefaultAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<dynamic>(cnn, Row.FirstOrDefault, typeof(DapperRow), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<dynamic> QuerySingleAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<dynamic>(cnn, Row.Single, typeof(DapperRow), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<dynamic> QuerySingleOrDefaultAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryRowAsync<dynamic>(cnn, Row.SingleOrDefault, typeof(DapperRow), new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static Task<IEnumerable<object>> QueryAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return QueryAsync<object>(cnn, type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered, default));
        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static Task<object> QueryFirstAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return QueryRowAsync<object>(cnn, Row.First, type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static Task<object> QueryFirstOrDefaultAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return QueryRowAsync<object>(cnn, Row.FirstOrDefault, type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static Task<object> QuerySingleAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return QueryRowAsync<object>(cnn, Row.Single, type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static Task<object> QuerySingleOrDefaultAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return QueryRowAsync<object>(cnn, Row.SingleOrDefault, type, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.None, default));
        }

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryAsync<T>(cnn, typeof(T), command);

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<IEnumerable<object>> QueryAsync(this IDbConnection cnn, Type type, CommandDefinition command) =>
            QueryAsync<object>(cnn, type, command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<object> QueryFirstAsync(this IDbConnection cnn, Type type, CommandDefinition command) =>
            QueryRowAsync<object>(cnn, Row.First, type, command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<T> QueryFirstAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<T>(cnn, Row.First, typeof(T), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<object> QueryFirstOrDefaultAsync(this IDbConnection cnn, Type type, CommandDefinition command) =>
            QueryRowAsync<object>(cnn, Row.FirstOrDefault, type, command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<T>(cnn, Row.FirstOrDefault, typeof(T), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<object> QuerySingleAsync(this IDbConnection cnn, Type type, CommandDefinition command) =>
            QueryRowAsync<object>(cnn, Row.Single, type, command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<T> QuerySingleAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<T>(cnn, Row.Single, typeof(T), command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="type">The type to return.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<object> QuerySingleOrDefaultAsync(this IDbConnection cnn, Type type, CommandDefinition command) =>
            QueryRowAsync<object>(cnn, Row.SingleOrDefault, type, command);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command used to query on this connection.</param>
        public static Task<T> QuerySingleOrDefaultAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            QueryRowAsync<T>(cnn, Row.SingleOrDefault, typeof(T), command);

        private static Task<DbDataReader> ExecuteReaderWithFlagsFallbackAsync(DbCommand cmd, bool wasClosed, CommandBehavior behavior, CancellationToken cancellationToken)
        {
            var task = cmd.ExecuteReaderAsync(GetBehavior(wasClosed, behavior), cancellationToken);
            if (task.Status == TaskStatus.Faulted && DisableCommandBehaviorOptimizations(behavior, task.Exception.InnerException))
            { // we can retry; this time it will have different flags
                return cmd.ExecuteReaderAsync(GetBehavior(wasClosed, behavior), cancellationToken);
            }
            return task;
        }

        /// <summary>
        /// Attempts to open a connection asynchronously, with a better error message for unsupported usages.
        /// </summary>
        private static Task TryOpenAsync(this IDbConnection cnn, CancellationToken cancel)
        {
            if (cnn is DbConnection dbConn)
            {
                return dbConn.OpenAsync(cancel);
            }
            else
            {
                throw new InvalidOperationException("Async operations require use of a DbConnection or an already-open IDbConnection");
            }
        }

        /// <summary>
        /// Attempts setup a <see cref="DbCommand"/> on a <see cref="DbConnection"/>, with a better error message for unsupported usages.
        /// </summary>
        private static DbCommand TrySetupAsyncCommand(this CommandDefinition command, IDbConnection cnn, Action<IDbCommand, object> paramReader)
        {
            if (command.SetupCommand(cnn, paramReader) is DbCommand dbCommand)
            {
                return dbCommand;
            }
            else
            {
                throw new InvalidOperationException("Async operations require use of a DbConnection or an IDbConnection where .CreateCommand() returns a DbCommand");
            }
        }

        private static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, Type effectiveType, CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            var cancel = command.CancellationToken;
            using var cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
            DbDataReader reader = null;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(cancel).ConfigureAwait(false);
                reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, cancel).ConfigureAwait(false);

                var tuple = info.Deserializer;
                int hash = GetColumnHash(reader);
                if (tuple.Func == null || tuple.Hash != hash)
                {
                    if (reader.FieldCount == 0)
                        return Enumerable.Empty<T>();
                    tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                    if (command.AddToCache) SetQueryCache(identity, info);
                }

                var func = tuple.Func;

                if (command.Buffered)
                {
                    var buffer = new List<T>();
                    var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                    while (await reader.ReadAsync(cancel).ConfigureAwait(false))
                    {
                        object val = func(reader);
                        buffer.Add(GetValue<T>(reader, effectiveType, val));
                    }
                    while (await reader.NextResultAsync(cancel).ConfigureAwait(false)) { /* ignore subsequent result sets */ }
                    command.OnCompleted();
                    return buffer;
                }
                else
                {
                    // can't use ReadAsync / cancellation; but this will have to do
                    wasClosed = false; // don't close if handing back an open reader; rely on the command-behavior
                    var deferred = ExecuteReaderSync<T>(reader, func, command.Parameters);
                    reader = null; // to prevent it being disposed before the caller gets to see it
                    return deferred;
                }
            }
            finally
            {
                using (reader) { /* dispose if non-null */ }
                if (wasClosed) cnn.Close();
            }
        }

        private static async Task<T> QueryRowAsync<T>(this IDbConnection cnn, Row row, Type effectiveType, CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            var cancel = command.CancellationToken;
            using var cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
            DbDataReader reader = null;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(cancel).ConfigureAwait(false);
                reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, (row & Row.Single) != 0
                ? CommandBehavior.SequentialAccess | CommandBehavior.SingleResult // need to allow multiple rows, to check fail condition
                : CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow, cancel).ConfigureAwait(false);

                T result = default;
                if (await reader.ReadAsync(cancel).ConfigureAwait(false) && reader.FieldCount != 0)
                {
                    result = ReadRow<T>(info, identity, ref command, effectiveType, reader);

                    if ((row & Row.Single) != 0 && await reader.ReadAsync(cancel).ConfigureAwait(false)) ThrowMultipleRows(row);
                    while (await reader.ReadAsync(cancel).ConfigureAwait(false)) { /* ignore rows after the first */ }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                while (await reader.NextResultAsync(cancel).ConfigureAwait(false)) { /* ignore result sets after the first */ }
                return result;
            }
            finally
            {
                using (reader) { /* dispose if non-null */ }
                if (wasClosed) cnn.Close();
            }
        }

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            ExecuteAsync(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered, default));

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute on this connection.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> ExecuteAsync(this IDbConnection cnn, CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            if (multiExec != null)
            {
                return ExecuteMultiImplAsync(cnn, command, multiExec);
            }
            else
            {
                return ExecuteImplAsync(cnn, command, param);
            }
        }

        private struct AsyncExecState
        {
            public readonly DbCommand Command;
            public readonly Task<int> Task;
            public AsyncExecState(DbCommand command, Task<int> task)
            {
                Command = command;
                Task = task;
            }
        }

        private static async Task<int> ExecuteMultiImplAsync(IDbConnection cnn, CommandDefinition command, IEnumerable multiExec)
        {
            bool isFirst = true;
            int total = 0;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);

                CacheInfo info = null;
                string masterSql = null;
                if ((command.Flags & CommandFlags.Pipelined) != 0)
                {
                    const int MAX_PENDING = 100;
                    var pending = new Queue<AsyncExecState>(MAX_PENDING);
                    DbCommand cmd = null;
                    try
                    {
                        foreach (var obj in multiExec)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                                cmd = command.TrySetupAsyncCommand(cnn, null);
                                masterSql = cmd.CommandText;
                                var identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType());
                                info = GetCacheInfo(identity, obj, command.AddToCache);
                            }
                            else if (pending.Count >= MAX_PENDING)
                            {
                                var recycled = pending.Dequeue();
                                total += await recycled.Task.ConfigureAwait(false);
                                cmd = recycled.Command;
                                cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                                cmd.Parameters.Clear(); // current code is Add-tastic
                            }
                            else
                            {
                                cmd = command.TrySetupAsyncCommand(cnn, null);
                            }
                            info.ParamReader(cmd, obj);

                            var task = cmd.ExecuteNonQueryAsync(command.CancellationToken);
                            pending.Enqueue(new AsyncExecState(cmd, task));
                            cmd = null; // note the using in the finally: this avoids a double-dispose
                        }
                        while (pending.Count != 0)
                        {
                            var pair = pending.Dequeue();
                            using (pair.Command) { /* dispose commands */ }
                            total += await pair.Task.ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        // this only has interesting work to do if there are failures
                        using (cmd) { /* dispose commands */ }
                        while (pending.Count != 0)
                        { // dispose tasks even in failure
                            using (pending.Dequeue().Command) { /* dispose commands */ }
                        }
                    }
                }
                else
                {
                    using var cmd = command.TrySetupAsyncCommand(cnn, null);
                    foreach (var obj in multiExec)
                    {
                        if (isFirst)
                        {
                            masterSql = cmd.CommandText;
                            isFirst = false;
                            var identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType());
                            info = GetCacheInfo(identity, obj, command.AddToCache);
                        }
                        else
                        {
                            cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                            cmd.Parameters.Clear(); // current code is Add-tastic
                        }
                        info.ParamReader(cmd, obj);
                        total += await cmd.ExecuteNonQueryAsync(command.CancellationToken).ConfigureAwait(false);
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

        private static async Task<int> ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, object param)
        {
            var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            using var cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                var result = await cmd.ExecuteNonQueryAsync(command.CancellationToken).ConfigureAwait(false);
                command.OnCompleted();
                return result;
            }
            finally
            {
                if (wasClosed) cnn.Close();
            }
        }

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 2 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 2 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id") =>
            MultiMapAsync<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 3 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 3 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id") =>
            MultiMapAsync<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 4 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 4 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id") =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 5 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 5 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id") =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 6 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 6 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id") =>
             MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn, command, map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 7 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn,
                new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default), map, splitOn);

        /// <summary>
        /// Perform an asynchronous multi-mapping query with 7 input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
        /// <typeparam name="TThird">The third type in the recordset.</typeparam>
        /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
        /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
        /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
        /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id") =>
            MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, command, map, splitOn);

        private static async Task<IEnumerable<TReturn>> MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Delegate map, string splitOn)
        {
            object param = command.Parameters;
            var identity = new Identity<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(command.CommandText, command.CommandType, cnn, typeof(TFirst), param?.GetType());
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                using var cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
                using var reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, command.CancellationToken).ConfigureAwait(false);
                if (!command.Buffered) wasClosed = false; // handing back open reader; rely on command-behavior
                var results = MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(null, CommandDefinition.ForCallback(command.Parameters), map, splitOn, reader, identity, true);
                return command.Buffered ? results.ToList() : results;
            }
            finally
            {
                if (wasClosed) cnn.Close();
            }
        }

        /// <summary>
        /// Perform an asynchronous multi-mapping query with an arbitrary number of input types.
        /// This returns a single type, combined from the raw types via <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TReturn">The combined type to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="types">Array of types in the recordset.</param>
        /// <param name="map">The function to map row types to the return type.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An enumerable of <typeparamref name="TReturn"/>.</returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, default);
            return MultiMapAsync(cnn, command, types, map, splitOn);
        }

        private static async Task<IEnumerable<TReturn>> MultiMapAsync<TReturn>(this IDbConnection cnn, CommandDefinition command, Type[] types, Func<object[], TReturn> map, string splitOn)
        {
            if (types.Length < 1)
            {
                throw new ArgumentException("you must provide at least one type to deserialize");
            }

            object param = command.Parameters;
            var identity = new IdentityWithTypes(command.CommandText, command.CommandType, cnn, types[0], param?.GetType(), types);
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                using var cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
                using var reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, command.CancellationToken).ConfigureAwait(false);
                var results = MultiMapImpl(null, default, types, map, splitOn, reader, identity, true);
                return command.Buffered ? results.ToList() : results;
            }
            finally
            {
                if (wasClosed) cnn.Close();
            }
        }

        private static IEnumerable<T> ExecuteReaderSync<T>(IDataReader reader, Func<IDataReader, object> func, object parameters)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return (T)func(reader);
                }
                while (reader.NextResult()) { /* ignore subsequent result sets */ }
                (parameters as IParameterCallbacks)?.OnCompleted();
            }
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        public static Task<GridReader> QueryMultipleAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            QueryMultipleAsync(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered));

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="command">The command to execute for this query.</param>
        public static async Task<GridReader> QueryMultipleAsync(this IDbConnection cnn, CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, typeof(GridReader), param?.GetType());
            CacheInfo info = GetCacheInfo(identity, param, command.AddToCache);

            DbCommand cmd = null;
            IDataReader reader = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                cmd = command.TrySetupAsyncCommand(cnn, info.ParamReader);
                reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, CommandBehavior.SequentialAccess, command.CancellationToken).ConfigureAwait(false);

                var result = new GridReader(cmd, reader, identity, command.Parameters as DynamicParameters, command.AddToCache, command.CancellationToken);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                return result;
            }
            catch
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        try { cmd.Cancel(); }
                        catch
                        { /* don't spoil the existing exception */
                        }
                    }
                    reader.Dispose();
                }
                cmd?.Dispose();
                if (wasClosed) cnn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DataTable table = new DataTable("MyTable");
        /// using (var reader = ExecuteReader(cnn, sql, param))
        /// {
        ///     table.Load(reader);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            ExecuteWrappedReaderImplAsync(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered), CommandBehavior.Default).CastResult<DbDataReader, IDataReader>();

        /// <summary>
        /// Execute parameterized SQL and return a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            ExecuteWrappedReaderImplAsync(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered), CommandBehavior.Default);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteWrappedReaderImplAsync(cnn, command, CommandBehavior.Default).CastResult<DbDataReader, IDataReader>();

        /// <summary>
        /// Execute parameterized SQL and return a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection cnn, CommandDefinition command) =>
            ExecuteWrappedReaderImplAsync(cnn, command, CommandBehavior.Default);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The <see cref="CommandBehavior"/> flags for this reader.</param>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="T:DataSet"/>.
        /// </remarks>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior) =>
            ExecuteWrappedReaderImplAsync(cnn, command, commandBehavior).CastResult<DbDataReader, IDataReader>();

        /// <summary>
        /// Execute parameterized SQL and return a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The <see cref="CommandBehavior"/> flags for this reader.</param>
        public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior) =>
            ExecuteWrappedReaderImplAsync(cnn, command, commandBehavior);

        private static async Task<DbDataReader> ExecuteWrappedReaderImplAsync(IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        {
            Action<IDbCommand, object> paramReader = GetParameterReader(cnn, ref command);

            DbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed, disposeCommand = true;
            try
            {
                cmd = command.TrySetupAsyncCommand(cnn, paramReader);
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                var reader = await ExecuteReaderWithFlagsFallbackAsync(cmd, wasClosed, commandBehavior, command.CancellationToken).ConfigureAwait(false);
                wasClosed = false;
                disposeCommand = false;
                return WrappedReader.Create(cmd, reader);
            }
            finally
            {
                if (wasClosed) cnn.Close();
                if (cmd != null && disposeCommand) cmd.Dispose();
            }
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <see cref="object"/>.</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            ExecuteScalarImplAsync<object>(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered));

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
        public static Task<T> ExecuteScalarAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            ExecuteScalarImplAsync<T>(cnn, new CommandDefinition(sql, param, transaction, commandTimeout, commandType, CommandFlags.Buffered));

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <see cref="object"/>.</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImplAsync<object>(cnn, command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="cnn">The connection to execute on.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first cell selected as <typeparamref name="T"/>.</returns>
        public static Task<T> ExecuteScalarAsync<T>(this IDbConnection cnn, CommandDefinition command) =>
            ExecuteScalarImplAsync<T>(cnn, command);

        private static async Task<T> ExecuteScalarImplAsync<T>(IDbConnection cnn, CommandDefinition command)
        {
            Action<IDbCommand, object> paramReader = null;
            object param = command.Parameters;
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType());
                paramReader = GetCacheInfo(identity, command.Parameters, command.AddToCache).ParamReader;
            }

            DbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            object result;
            try
            {
                cmd = command.TrySetupAsyncCommand(cnn, paramReader);
                if (wasClosed) await cnn.TryOpenAsync(command.CancellationToken).ConfigureAwait(false);
                result = await cmd.ExecuteScalarAsync(command.CancellationToken).ConfigureAwait(false);
                command.OnCompleted();
            }
            finally
            {
                if (wasClosed) cnn.Close();
                cmd?.Dispose();
            }
            return Parse<T>(result);
        }
        #endregion
        #region // DapperRow

        private sealed partial class DapperRow
            : IDictionary<string, object>
            , IReadOnlyDictionary<string, object>
        {
            private readonly DapperTable table;
            private object[] values;

            public DapperRow(DapperTable table, object[] values)
            {
                this.table = table ?? throw new ArgumentNullException(nameof(table));
                this.values = values ?? throw new ArgumentNullException(nameof(values));
            }

            private sealed class DeadValue
            {
                public static readonly DeadValue Default = new DeadValue();
                private DeadValue() { /* hiding constructor */ }
            }

            int ICollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    int count = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!(values[i] is DeadValue)) count++;
                    }
                    return count;
                }
            }

            public bool TryGetValue(string key, out object value)
                => TryGetValue(table.IndexOfName(key), out value);

            internal bool TryGetValue(int index, out object value)
            {
                if (index < 0)
                { // doesn't exist
                    value = null;
                    return false;
                }
                // exists, **even if** we don't have a value; consider table rows heterogeneous
                value = index < values.Length ? values[index] : null;
                if (value is DeadValue)
                { // pretend it isn't here
                    value = null;
                    return false;
                }
                return true;
            }

            public override string ToString()
            {
                var sb = GetStringBuilder().Append("{DapperRow");
                foreach (var kv in this)
                {
                    var value = kv.Value;
                    sb.Append(", ").Append(kv.Key);
                    if (value != null)
                    {
                        sb.Append(" = '").Append(kv.Value).Append('\'');
                    }
                    else
                    {
                        sb.Append(" = NULL");
                    }
                }

                return sb.Append('}').ToStringRecycle();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                var names = table.FieldNames;
                for (var i = 0; i < names.Length; i++)
                {
                    object value = i < values.Length ? values[i] : null;
                    if (!(value is DeadValue))
                    {
                        yield return new KeyValuePair<string, object>(names[i], value);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Implementation of ICollection<KeyValuePair<string,object>>

            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                dic.Add(item.Key, item.Value);
            }

            void ICollection<KeyValuePair<string, object>>.Clear()
            { // removes values for **this row**, but doesn't change the fundamental table
                for (int i = 0; i < values.Length; i++)
                    values[i] = DeadValue.Default;
            }

            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return TryGetValue(item.Key, out object value) && Equals(value, item.Value);
            }

            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                foreach (var kv in this)
                {
                    array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
                }
            }

            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                return dic.Remove(item.Key);
            }

            bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
            #endregion

            #region Implementation of IDictionary<string,object>

            bool IDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                return true;
            }

            void IDictionary<string, object>.Add(string key, object value)
            {
                SetValue(key, value, true);
            }

            bool IDictionary<string, object>.Remove(string key)
                => Remove(table.IndexOfName(key));

            internal bool Remove(int index)
            {
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                values[index] = DeadValue.Default;
                return true;
            }

            object IDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
                set { SetValue(key, value, false); }
            }

            public object SetValue(string key, object value)
            {
                return SetValue(key, value, false);
            }

            private object SetValue(string key, object value, bool isAdd)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                int index = table.IndexOfName(key);
                if (index < 0)
                {
                    index = table.AddField(key);
                }
                else if (isAdd && index < values.Length && !(values[index] is DeadValue))
                {
                    // then semantically, this value already exists
                    throw new ArgumentException("An item with the same key has already been added", nameof(key));
                }
                return SetValue(index, value);
            }
            internal object SetValue(int index, object value)
            {
                int oldLength = values.Length;
                if (oldLength <= index)
                {
                    // we'll assume they're doing lots of things, and
                    // grow it to the full width of the table
                    Array.Resize(ref values, table.FieldCount);
                    for (int i = oldLength; i < values.Length; i++)
                    {
                        values[i] = DeadValue.Default;
                    }
                }
                return values[index] = value;
            }

            ICollection<string> IDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key).ToArray(); }
            }

            ICollection<object> IDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value).ToArray(); }
            }

            #endregion


            #region Implementation of IReadOnlyDictionary<string,object>


            int IReadOnlyCollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    return values.Count(t => !(t is DeadValue));
                }
            }

            bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                return index >= 0 && index < values.Length && !(values[index] is DeadValue);
            }

            object IReadOnlyDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
            }

            IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key); }
            }

            IEnumerable<object> IReadOnlyDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value); }
            }

            #endregion
        }
        [TypeDescriptionProvider(typeof(DapperRowTypeDescriptionProvider))]
        private sealed partial class DapperRow
        {
            private sealed class DapperRowTypeDescriptionProvider : TypeDescriptionProvider
            {
                public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
                    => new DapperRowTypeDescriptor(instance);
                public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
                    => new DapperRowTypeDescriptor(instance);
            }

            //// in theory we could implement this for zero-length results to bind; would require
            //// additional changes, though, to capture a table even when no rows - so not currently provided
            //internal sealed class DapperRowList : List<DapperRow>, ITypedList
            //{
            //    private readonly DapperTable _table;
            //    public DapperRowList(DapperTable table) { _table = table; }
            //    PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
            //    {
            //        if (listAccessors != null && listAccessors.Length != 0) return PropertyDescriptorCollection.Empty;

            //        return DapperRowTypeDescriptor.GetProperties(_table);
            //    }

            //    string ITypedList.GetListName(PropertyDescriptor[] listAccessors) => null;
            //}

            private sealed class DapperRowTypeDescriptor : ICustomTypeDescriptor
            {
                private readonly DapperRow _row;
                public DapperRowTypeDescriptor(object instance)
                    => _row = (DapperRow)instance;

                AttributeCollection ICustomTypeDescriptor.GetAttributes()
                    => AttributeCollection.Empty;

                string ICustomTypeDescriptor.GetClassName() => typeof(DapperRow).FullName;

                string ICustomTypeDescriptor.GetComponentName() => null;

                private static readonly TypeConverter s_converter = new ExpandableObjectConverter();
                TypeConverter ICustomTypeDescriptor.GetConverter() => s_converter;

                EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => null;

                PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => null;

                object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => null;

                EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => EventDescriptorCollection.Empty;

                EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;

                internal static PropertyDescriptorCollection GetProperties(DapperRow row) => GetProperties(row?.table, row);
                internal static PropertyDescriptorCollection GetProperties(DapperTable table, IDictionary<string, object> row = null)
                {
                    string[] names = table?.FieldNames;
                    if (names == null || names.Length == 0) return PropertyDescriptorCollection.Empty;
                    var arr = new PropertyDescriptor[names.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var type = row != null && row.TryGetValue(names[i], out var value) && value != null
                            ? value.GetType() : typeof(object);
                        arr[i] = new RowBoundPropertyDescriptor(type, names[i], i);
                    }
                    return new PropertyDescriptorCollection(arr, true);
                }
                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => GetProperties(_row);

                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => GetProperties(_row);

                object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => _row;
            }

            private sealed class RowBoundPropertyDescriptor : PropertyDescriptor
            {
                private readonly Type _type;
                private readonly int _index;
                public RowBoundPropertyDescriptor(Type type, string name, int index) : base(name, null)
                {
                    _type = type;
                    _index = index;
                }
                public override bool CanResetValue(object component) => true;
                public override void ResetValue(object component) => ((DapperRow)component).Remove(_index);
                public override bool IsReadOnly => false;
                public override bool ShouldSerializeValue(object component) => ((DapperRow)component).TryGetValue(_index, out _);
                public override Type ComponentType => typeof(DapperRow);
                public override Type PropertyType => _type;
                public override object GetValue(object component)
                    => ((DapperRow)component).TryGetValue(_index, out var val) ? (val ?? DBNull.Value) : DBNull.Value;
                public override void SetValue(object component, object value)
                    => ((DapperRow)component).SetValue(_index, value is DBNull ? null : value);
            }
        }
        private sealed partial class DapperRow : System.Dynamic.IDynamicMetaObjectProvider
        {
            System.Dynamic.DynamicMetaObject System.Dynamic.IDynamicMetaObjectProvider.GetMetaObject(
    System.Linq.Expressions.Expression parameter)
            {
                return new DapperRowMetaObject(parameter, System.Dynamic.BindingRestrictions.Empty, this);
            }
        }

        private sealed class DapperRowMetaObject : System.Dynamic.DynamicMetaObject
        {
            private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
            private static readonly MethodInfo setValueMethod = typeof(DapperRow).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });

            public DapperRowMetaObject(
                System.Linq.Expressions.Expression expression,
                System.Dynamic.BindingRestrictions restrictions
                )
                : base(expression, restrictions)
            {
            }

            public DapperRowMetaObject(
                System.Linq.Expressions.Expression expression,
                System.Dynamic.BindingRestrictions restrictions,
                object value
                )
                : base(expression, restrictions, value)
            {
            }

            private System.Dynamic.DynamicMetaObject CallMethod(
                MethodInfo method,
                System.Linq.Expressions.Expression[] parameters
                )
            {
                var callMethod = new System.Dynamic.DynamicMetaObject(
                    System.Linq.Expressions.Expression.Call(
                        System.Linq.Expressions.Expression.Convert(Expression, LimitType),
                        method,
                        parameters),
                    System.Dynamic.BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                    );
                return callMethod;
            }

            public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder)
            {
                var parameters = new System.Linq.Expressions.Expression[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                     };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            // Needed for Visual basic dynamic support
            public override System.Dynamic.DynamicMetaObject BindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject[] args)
            {
                var parameters = new System.Linq.Expressions.Expression[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                     };

                var callMethod = CallMethod(getValueMethod, parameters);

                return callMethod;
            }

            public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value)
            {
                var parameters = new System.Linq.Expressions.Expression[]
                                     {
                                         System.Linq.Expressions.Expression.Constant(binder.Name),
                                         value.Expression,
                                     };

                var callMethod = CallMethod(setValueMethod, parameters);

                return callMethod;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                if (HasValue && Value is IDictionary<string, object> lookup) return lookup.Keys;
#if NETFrame
                return new string[0];
#else
                return Array.Empty<string>();
#endif
            }
        }
        #endregion
        #region // DapperTable

        private sealed class DapperTable
        {
            private string[] fieldNames;
            private readonly Dictionary<string, int> fieldNameLookup;

            internal string[] FieldNames => fieldNames;

            public DapperTable(string[] fieldNames)
            {
                this.fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));

                fieldNameLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
                // if there are dups, we want the **first** key to be the "winner" - so iterate backwards
                for (int i = fieldNames.Length - 1; i >= 0; i--)
                {
                    string key = fieldNames[i];
                    if (key != null) fieldNameLookup[key] = i;
                }
            }

            internal int IndexOfName(string name)
            {
                return (name != null && fieldNameLookup.TryGetValue(name, out int result)) ? result : -1;
            }

            internal int AddField(string name)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (fieldNameLookup.ContainsKey(name)) throw new InvalidOperationException("Field already exists: " + name);
                int oldLen = fieldNames.Length;
                Array.Resize(ref fieldNames, oldLen + 1); // yes, this is sub-optimal, but this is not the expected common case
                fieldNames[oldLen] = name;
                fieldNameLookup[name] = oldLen;
                return oldLen;
            }

            internal bool FieldExists(string key) => key != null && fieldNameLookup.ContainsKey(key);

            public int FieldCount => fieldNames.Length;
        }
        #endregion
        #region // DeserializerState
        private struct DeserializerState
        {
            public readonly int Hash;
            public readonly Func<IDataReader, object> Func;

            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }
        #endregion
        #region // DontMap
        /// <summary>
        /// Dummy type for excluding from multi-map
        /// </summary>
        private class DontMap { /* hiding constructor */ }
        #endregion
        #region // GridReader

        /// <summary>
        /// The grid reader provides interfaces for reading multiple result sets from a Dapper query
        /// </summary>
        public partial class GridReader : IDisposable
        {
            private IDataReader reader;
            private readonly Identity identity;
            private readonly bool addToCache;

            internal GridReader(IDbCommand command, IDataReader reader, Identity identity, IParameterCallbacks callbacks, bool addToCache)
            {
                Command = command;
                this.reader = reader;
                this.identity = identity;
                this.callbacks = callbacks;
                this.addToCache = addToCache;
            }

            /// <summary>
            /// Read the next grid of results, returned as a dynamic object.
            /// </summary>
            /// <param name="buffered">Whether the results should be buffered in memory.</param>
            /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public IEnumerable<dynamic> Read(bool buffered = true) => ReadImpl<dynamic>(typeof(DapperRow), buffered);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object.
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public dynamic ReadFirst() => ReadRow<dynamic>(typeof(DapperRow), Row.First);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object.
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public dynamic ReadFirstOrDefault() => ReadRow<dynamic>(typeof(DapperRow), Row.FirstOrDefault);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object.
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public dynamic ReadSingle() => ReadRow<dynamic>(typeof(DapperRow), Row.Single);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object.
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public dynamic ReadSingleOrDefault() => ReadRow<dynamic>(typeof(DapperRow), Row.SingleOrDefault);

            /// <summary>
            /// Read the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            /// <param name="buffered">Whether the results should be buffered in memory.</param>
            public IEnumerable<T> Read<T>(bool buffered = true) => ReadImpl<T>(typeof(T), buffered);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public T ReadFirst<T>() => ReadRow<T>(typeof(T), Row.First);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public T ReadFirstOrDefault<T>() => ReadRow<T>(typeof(T), Row.FirstOrDefault);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public T ReadSingle<T>() => ReadRow<T>(typeof(T), Row.Single);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public T ReadSingleOrDefault<T>() => ReadRow<T>(typeof(T), Row.SingleOrDefault);

            /// <summary>
            /// Read the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <param name="buffered">Whether to buffer the results.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public IEnumerable<object> Read(Type type, bool buffered = true)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadImpl<object>(type, buffered);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public object ReadFirst(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRow<object>(type, Row.First);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public object ReadFirstOrDefault(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRow<object>(type, Row.FirstOrDefault);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public object ReadSingle(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRow<object>(type, Row.Single);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public object ReadSingleOrDefault(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRow<object>(type, Row.SingleOrDefault);
            }

            private IEnumerable<T> ReadImpl<T>(Type type, bool buffered)
            {
                if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
                var typedIdentity = identity.ForGrid(type, gridIndex);
                CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                var deserializer = cache.Deserializer;

                int hash = GetColumnHash(reader);
                if (deserializer.Func == null || deserializer.Hash != hash)
                {
                    deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                    cache.Deserializer = deserializer;
                }
                IsConsumed = true;
                var result = ReadDeferred<T>(gridIndex, deserializer.Func, type);
                return buffered ? result.ToList() : result;
            }

            private T ReadRow<T>(Type type, Row row)
            {
                if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
                IsConsumed = true;

                T result = default;
                if (reader.Read() && reader.FieldCount != 0)
                {
                    var typedIdentity = identity.ForGrid(type, gridIndex);
                    CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                    var deserializer = cache.Deserializer;

                    int hash = GetColumnHash(reader);
                    if (deserializer.Func == null || deserializer.Hash != hash)
                    {
                        deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                        cache.Deserializer = deserializer;
                    }
                    object val = deserializer.Func(reader);
                    if (val == null || val is T)
                    {
                        result = (T)val;
                    }
                    else
                    {
                        var convertToType = Nullable.GetUnderlyingType(type) ?? type;
                        result = (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                    }
                    if ((row & Row.Single) != 0 && reader.Read()) ThrowMultipleRows(row);
                    while (reader.Read()) { /* ignore subsequent rows */ }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                NextResult();
                return result;
            }

            private IEnumerable<TReturn> MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Delegate func, string splitOn)
            {
                var identity = this.identity.ForGrid<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(typeof(TReturn), gridIndex);

                IsConsumed = true;

                try
                {
                    foreach (var r in MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(null, default, func, splitOn, reader, identity, false))
                    {
                        yield return r;
                    }
                }
                finally
                {
                    NextResult();
                }
            }

            private IEnumerable<TReturn> MultiReadInternal<TReturn>(Type[] types, Func<object[], TReturn> map, string splitOn)
            {
                var identity = this.identity.ForGrid(typeof(TReturn), types, gridIndex);
                try
                {
                    foreach (var r in MultiMapImpl<TReturn>(null, default, types, map, splitOn, reader, identity, false))
                    {
                        yield return r;
                    }
                }
                finally
                {
                    NextResult();
                }
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid.
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid.
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TThird">The third type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TThird">The third type in the record set.</typeparam>
            /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TThird">The third type in the record set.</typeparam>
            /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
            /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TThird">The third type in the record set.</typeparam>
            /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
            /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
            /// <typeparam name="TSixth">The sixth type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid
            /// </summary>
            /// <typeparam name="TFirst">The first type in the record set.</typeparam>
            /// <typeparam name="TSecond">The second type in the record set.</typeparam>
            /// <typeparam name="TThird">The third type in the record set.</typeparam>
            /// <typeparam name="TFourth">The fourth type in the record set.</typeparam>
            /// <typeparam name="TFifth">The fifth type in the record set.</typeparam>
            /// <typeparam name="TSixth">The sixth type in the record set.</typeparam>
            /// <typeparam name="TSeventh">The seventh type in the record set.</typeparam>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="func">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> func, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(func, splitOn);
                return buffered ? result.ToList() : result;
            }

            /// <summary>
            /// Read multiple objects from a single record set on the grid
            /// </summary>
            /// <typeparam name="TReturn">The type to return from the record set.</typeparam>
            /// <param name="types">The types to read from the result set.</param>
            /// <param name="map">The mapping function from the read types to the return type.</param>
            /// <param name="splitOn">The field(s) we should split and read the second object from (defaults to "id")</param>
            /// <param name="buffered">Whether to buffer results in memory.</param>
            public IEnumerable<TReturn> Read<TReturn>(Type[] types, Func<object[], TReturn> map, string splitOn = "id", bool buffered = true)
            {
                var result = MultiReadInternal(types, map, splitOn);
                return buffered ? result.ToList() : result;
            }

            private IEnumerable<T> ReadDeferred<T>(int index, Func<IDataReader, object> deserializer, Type effectiveType)
            {
                try
                {
                    var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                    while (index == gridIndex && reader.Read())
                    {
                        object val = deserializer(reader);
                        if (val == null || val is T)
                        {
                            yield return (T)val;
                        }
                        else
                        {
                            yield return (T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture);
                        }
                    }
                }
                finally // finally so that First etc progresses things even when multiple rows
                {
                    if (index == gridIndex)
                    {
                        NextResult();
                    }
                }
            }

            private int gridIndex; //, readCount;
            private readonly IParameterCallbacks callbacks;

            /// <summary>
            /// Has the underlying reader been consumed?
            /// </summary>
            public bool IsConsumed { get; private set; }

            /// <summary>
            /// The command associated with the reader
            /// </summary>
            public IDbCommand Command { get; set; }

            private void NextResult()
            {
                if (reader.NextResult())
                {
                    // readCount++;
                    gridIndex++;
                    IsConsumed = false;
                }
                else
                {
                    // happy path; close the reader cleanly - no
                    // need for "Cancel" etc
                    reader.Dispose();
                    reader = null;
                    callbacks?.OnCompleted();
                    Dispose();
                }
            }

            /// <summary>
            /// Dispose the grid, closing and disposing both the underlying reader and command.
            /// </summary>
            public void Dispose()
            {
                if (reader != null)
                {
                    if (!reader.IsClosed) Command?.Cancel();
                    reader.Dispose();
                    reader = null;
                }
                if (Command != null)
                {
                    Command.Dispose();
                    Command = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        public partial class GridReader
        {
            private readonly CancellationToken cancel;
            internal GridReader(IDbCommand command, IDataReader reader, Identity identity, DynamicParameters dynamicParams, bool addToCache, CancellationToken cancel)
                : this(command, reader, identity, dynamicParams, addToCache)
            {
                this.cancel = cancel;
            }

            /// <summary>
            /// Read the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            /// <param name="buffered">Whether to buffer the results.</param>
            public Task<IEnumerable<dynamic>> ReadAsync(bool buffered = true) => ReadAsyncImpl<dynamic>(typeof(DapperRow), buffered);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public Task<dynamic> ReadFirstAsync() => ReadRowAsyncImpl<dynamic>(typeof(DapperRow), Row.First);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public Task<dynamic> ReadFirstOrDefaultAsync() => ReadRowAsyncImpl<dynamic>(typeof(DapperRow), Row.FirstOrDefault);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public Task<dynamic> ReadSingleAsync() => ReadRowAsyncImpl<dynamic>(typeof(DapperRow), Row.Single);

            /// <summary>
            /// Read an individual row of the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public Task<dynamic> ReadSingleOrDefaultAsync() => ReadRowAsyncImpl<dynamic>(typeof(DapperRow), Row.SingleOrDefault);

            /// <summary>
            /// Read the next grid of results
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <param name="buffered">Whether to buffer the results.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public Task<IEnumerable<object>> ReadAsync(Type type, bool buffered = true)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadAsyncImpl<object>(type, buffered);
            }

            /// <summary>
            /// Read an individual row of the next grid of results
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public Task<object> ReadFirstAsync(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRowAsyncImpl<object>(type, Row.First);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public Task<object> ReadFirstOrDefaultAsync(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRowAsyncImpl<object>(type, Row.FirstOrDefault);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public Task<object> ReadSingleAsync(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRowAsyncImpl<object>(type, Row.Single);
            }

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <param name="type">The type to read.</param>
            /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
            public Task<object> ReadSingleOrDefaultAsync(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                return ReadRowAsyncImpl<object>(type, Row.SingleOrDefault);
            }

            /// <summary>
            /// Read the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            /// <param name="buffered">Whether the results should be buffered in memory.</param>
            public Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true) => ReadAsyncImpl<T>(typeof(T), buffered);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public Task<T> ReadFirstAsync<T>() => ReadRowAsyncImpl<T>(typeof(T), Row.First);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public Task<T> ReadFirstOrDefaultAsync<T>() => ReadRowAsyncImpl<T>(typeof(T), Row.FirstOrDefault);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public Task<T> ReadSingleAsync<T>() => ReadRowAsyncImpl<T>(typeof(T), Row.Single);

            /// <summary>
            /// Read an individual row of the next grid of results.
            /// </summary>
            /// <typeparam name="T">The type to read.</typeparam>
            public Task<T> ReadSingleOrDefaultAsync<T>() => ReadRowAsyncImpl<T>(typeof(T), Row.SingleOrDefault);

            private async Task NextResultAsync()
            {
                if (await ((DbDataReader)reader).NextResultAsync(cancel).ConfigureAwait(false))
                {
                    // readCount++;
                    gridIndex++;
                    IsConsumed = false;
                }
                else
                {
                    // happy path; close the reader cleanly - no
                    // need for "Cancel" etc
                    reader.Dispose();
                    reader = null;
                    callbacks?.OnCompleted();
                    Dispose();
                }
            }

            private Task<IEnumerable<T>> ReadAsyncImpl<T>(Type type, bool buffered)
            {
                if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
                var typedIdentity = identity.ForGrid(type, gridIndex);
                CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                var deserializer = cache.Deserializer;

                int hash = GetColumnHash(reader);
                if (deserializer.Func == null || deserializer.Hash != hash)
                {
                    deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                    cache.Deserializer = deserializer;
                }
                IsConsumed = true;
                if (buffered && reader is DbDataReader)
                {
                    return ReadBufferedAsync<T>(gridIndex, deserializer.Func);
                }
                else
                {
                    var result = ReadDeferred<T>(gridIndex, deserializer.Func, type);
                    if (buffered) result = result.ToList(); // for the "not a DbDataReader" scenario
                    return FromResult(result);
                }
            }

            private Task<T> ReadRowAsyncImpl<T>(Type type, Row row)
            {
                if (reader is DbDataReader dbReader) return ReadRowAsyncImplViaDbReader<T>(dbReader, type, row);

                // no async API available; use non-async and fake it
                return FromResult(ReadRow<T>(type, row));
            }

            private async Task<T> ReadRowAsyncImplViaDbReader<T>(DbDataReader reader, Type type, Row row)
            {
                if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (IsConsumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");

                IsConsumed = true;
                T result = default;
                if (await reader.ReadAsync(cancel).ConfigureAwait(false) && reader.FieldCount != 0)
                {
                    var typedIdentity = identity.ForGrid(type, gridIndex);
                    CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                    var deserializer = cache.Deserializer;

                    int hash = GetColumnHash(reader);
                    if (deserializer.Func == null || deserializer.Hash != hash)
                    {
                        deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                        cache.Deserializer = deserializer;
                    }
                    result = (T)deserializer.Func(reader);
                    if ((row & Row.Single) != 0 && await reader.ReadAsync(cancel).ConfigureAwait(false)) ThrowMultipleRows(row);
                    while (await reader.ReadAsync(cancel).ConfigureAwait(false)) { /* ignore subsequent rows */ }
                }
                else if ((row & Row.FirstOrDefault) == 0) // demanding a row, and don't have one
                {
                    ThrowZeroRows(row);
                }
                await NextResultAsync().ConfigureAwait(false);
                return result;
            }

            private async Task<IEnumerable<T>> ReadBufferedAsync<T>(int index, Func<IDataReader, object> deserializer)
            {
                try
                {
                    var reader = (DbDataReader)this.reader;
                    var buffer = new List<T>();
                    while (index == gridIndex && await reader.ReadAsync(cancel).ConfigureAwait(false))
                    {
                        buffer.Add((T)deserializer(reader));
                    }
                    return buffer;
                }
                finally // finally so that First etc progresses things even when multiple rows
                {
                    if (index == gridIndex)
                    {
                        await NextResultAsync().ConfigureAwait(false);
                    }
                }
            }
        }
        #endregion
        #region // ICustomQueryParameter
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
        #endregion
        #region // IDataReader
        /// <summary>
        /// Parses a data reader to a sequence of data of the supplied type. Used for deserializing a reader without a connection, etc.
        /// </summary>
        /// <typeparam name="T">The type to parse from the <paramref name="reader"/>.</typeparam>
        /// <param name="reader">The data reader to parse results from.</param>
        public static IEnumerable<T> Parse<T>(this IDataReader reader)
        {
            if (reader.Read())
            {
                var effectiveType = typeof(T);
                var deser = GetDeserializer(effectiveType, reader, 0, -1, false);
                var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                do
                {
                    object val = deser(reader);
                    if (val == null || val is T)
                    {
                        yield return (T)val;
                    }
                    else
                    {
                        yield return (T)Convert.ChangeType(val, convertToType, System.Globalization.CultureInfo.InvariantCulture);
                    }
                } while (reader.Read());
            }
        }

        /// <summary>
        /// Parses a data reader to a sequence of data of the supplied type (as object). Used for deserializing a reader without a connection, etc.
        /// </summary>
        /// <param name="reader">The data reader to parse results from.</param>
        /// <param name="type">The type to parse from the <paramref name="reader"/>.</param>
        public static IEnumerable<object> Parse(this IDataReader reader, Type type)
        {
            if (reader.Read())
            {
                var deser = GetDeserializer(type, reader, 0, -1, false);
                do
                {
                    yield return deser(reader);
                } while (reader.Read());
            }
        }

        /// <summary>
        /// Parses a data reader to a sequence of dynamic. Used for deserializing a reader without a connection, etc.
        /// </summary>
        /// <param name="reader">The data reader to parse results from.</param>
        public static IEnumerable<dynamic> Parse(this IDataReader reader)
        {
            if (reader.Read())
            {
                var deser = GetDapperRowDeserializer(reader, 0, -1, false);
                do
                {
                    yield return deser(reader);
                } while (reader.Read());
            }
        }

        /// <summary>
        /// Gets the row parser for a specific row on a data reader. This allows for type switching every row based on, for example, a TypeId column.
        /// You could return a collection of the base type but have each more specific.
        /// </summary>
        /// <param name="reader">The data reader to get the parser for the current row from</param>
        /// <param name="type">The type to get the parser for</param>
        /// <param name="startIndex">The start column index of the object (default 0)</param>
        /// <param name="length">The length of columns to read (default -1 = all fields following startIndex)</param>
        /// <param name="returnNullIfFirstMissing">Return null if we can't find the first column? (default false)</param>
        /// <returns>A parser for this specific object from this row.</returns>
        public static Func<IDataReader, object> GetRowParser(this IDataReader reader, Type type,
            int startIndex = 0, int length = -1, bool returnNullIfFirstMissing = false)
        {
            return GetDeserializer(type, reader, startIndex, length, returnNullIfFirstMissing);
        }

        /// <summary>
        /// Gets the row parser for a specific row on a data reader. This allows for type switching every row based on, for example, a TypeId column.
        /// You could return a collection of the base type but have each more specific.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="reader">The data reader to get the parser for the current row from.</param>
        /// <param name="concreteType">The type to get the parser for.</param>
        /// <param name="startIndex">The start column index of the object (default: 0).</param>
        /// <param name="length">The length of columns to read (default: -1 = all fields following startIndex).</param>
        /// <param name="returnNullIfFirstMissing">Return null if we can't find the first column? (default: false).</param>
        /// <returns>A parser for this specific object from this row.</returns>
        /// <example>
        /// var result = new List&lt;BaseType&gt;();
        /// using (var reader = connection.ExecuteReader(@"
        ///   select 'abc' as Name, 1 as Type, 3.0 as Value
        ///   union all
        ///   select 'def' as Name, 2 as Type, 4.0 as Value"))
        /// {
        ///     if (reader.Read())
        ///     {
        ///         var toFoo = reader.GetRowParser&lt;BaseType&gt;(typeof(Foo));
        ///         var toBar = reader.GetRowParser&lt;BaseType&gt;(typeof(Bar));
        ///         var col = reader.GetOrdinal("Type");
        ///         do
        ///         {
        ///             switch (reader.GetInt32(col))
        ///             {
        ///                 case 1:
        ///                     result.Add(toFoo(reader));
        ///                     break;
        ///                 case 2:
        ///                     result.Add(toBar(reader));
        ///                     break;
        ///             }
        ///         } while (reader.Read());
        ///     }
        /// }
        ///  
        /// abstract class BaseType
        /// {
        ///     public abstract int Type { get; }
        /// }
        /// class Foo : BaseType
        /// {
        ///     public string Name { get; set; }
        ///     public override int Type =&gt; 1;
        /// }
        /// class Bar : BaseType
        /// {
        ///     public float Value { get; set; }
        ///     public override int Type =&gt; 2;
        /// }
        /// </example>
        public static Func<IDataReader, T> GetRowParser<T>(this IDataReader reader, Type concreteType = null,
            int startIndex = 0, int length = -1, bool returnNullIfFirstMissing = false)
        {
            concreteType ??= typeof(T);
            var func = GetDeserializer(concreteType, reader, startIndex, length, returnNullIfFirstMissing);
            if (concreteType.IsValueType)
            {
                return _ => (T)func(_);
            }
            else
            {
                return (Func<IDataReader, T>)(Delegate)func;
            }
        }
        #endregion
        #region // Identity
        internal sealed class Identity<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> : Identity
        {
            private static readonly int s_typeHash;
            private static readonly int s_typeCount = CountNonTrivial(out s_typeHash);

            internal Identity(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, int gridIndex = 0)
                : base(sql, commandType, connectionString, type, parametersType, s_typeHash, gridIndex)
            { }
            internal Identity(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType, int gridIndex = 0)
                : base(sql, commandType, connection.ConnectionString, type, parametersType, s_typeHash, gridIndex)
            { }

            static int CountNonTrivial(out int hashCode)
            {
                int hashCodeLocal = 0;
                int count = 0;
                bool Map<T>()
                {
                    if (typeof(T) != typeof(DontMap))
                    {
                        count++;
                        hashCodeLocal = (hashCodeLocal * 23) + (typeof(T).GetHashCode());
                        return true;
                    }
                    return false;
                }
                _ = Map<TFirst>() && Map<TSecond>() && Map<TThird>()
                    && Map<TFourth>() && Map<TFifth>() && Map<TSixth>()
                    && Map<TSeventh>();
                hashCode = hashCodeLocal;
                return count;
            }
            internal override int TypeCount => s_typeCount;
            internal override Type GetType(int index) => index switch
            {
                0 => typeof(TFirst),
                1 => typeof(TSecond),
                2 => typeof(TThird),
                3 => typeof(TFourth),
                4 => typeof(TFifth),
                5 => typeof(TSixth),
                6 => typeof(TSeventh),
                _ => base.GetType(index),
            };
        }
        internal sealed class IdentityWithTypes : Identity
        {
            private readonly Type[] _types;

            internal IdentityWithTypes(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, Type[] otherTypes, int gridIndex = 0)
                : base(sql, commandType, connectionString, type, parametersType, HashTypes(otherTypes), gridIndex)
            {
                _types = otherTypes ?? Type.EmptyTypes;
            }
            internal IdentityWithTypes(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType, Type[] otherTypes, int gridIndex = 0)
                : base(sql, commandType, connection.ConnectionString, type, parametersType, HashTypes(otherTypes), gridIndex)
            {
                _types = otherTypes ?? Type.EmptyTypes;
            }

            internal override int TypeCount => _types.Length;

            internal override Type GetType(int index) => _types[index];

            static int HashTypes(Type[] types)
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
        }

        /// <summary>
        /// Identity of a cached query in Dapper, used for extensibility.
        /// </summary>
        public class Identity : IEquatable<Identity>
        {
            internal virtual int TypeCount => 0;

            internal virtual Type GetType(int index) => throw new IndexOutOfRangeException(nameof(index));

            internal Identity ForGrid<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(Type primaryType, int gridIndex) =>
                new Identity<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(sql, commandType, connectionString, primaryType, parametersType, gridIndex);

            internal Identity ForGrid(Type primaryType, int gridIndex) =>
                new Identity(sql, commandType, connectionString, primaryType, parametersType, 0, gridIndex);

            internal Identity ForGrid(Type primaryType, Type[] otherTypes, int gridIndex) =>
                (otherTypes == null || otherTypes.Length == 0)
                ? new Identity(sql, commandType, connectionString, primaryType, parametersType, 0, gridIndex)
                : new IdentityWithTypes(sql, commandType, connectionString, primaryType, parametersType, otherTypes, gridIndex);

            /// <summary>
            /// Create an identity for use with DynamicParameters, internal use only.
            /// </summary>
            /// <param name="type">The parameters type to create an <see cref="Identity"/> for.</param>
            /// <returns></returns>
            public Identity ForDynamicParameters(Type type) =>
                new Identity(sql, commandType, connectionString, this.type, type, 0, -1);

            internal Identity(string sql, CommandType? commandType, IDbConnection connection, Type type, Type parametersType)
                : this(sql, commandType, connection.ConnectionString, type, parametersType, 0, 0) { /* base call */ }

            private protected Identity(string sql, CommandType? commandType, string connectionString, Type type, Type parametersType, int otherTypesHash, int gridIndex)
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
                    hashCode = (hashCode * 23) + (connectionString == null ? 0 : connectionStringComparer.GetHashCode(connectionString));
                    hashCode = (hashCode * 23) + (parametersType?.GetHashCode() ?? 0);
                }
            }

            /// <summary>
            /// Whether this <see cref="Identity"/> equals another.
            /// </summary>
            /// <param name="obj">The other <see cref="object"/> to compare to.</param>
            public override bool Equals(object obj) => Equals(obj as Identity);

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
            /// <param name="other">The other <see cref="Identity"/> object to compare.</param>
            /// <returns>Whether the two are equal</returns>
            public bool Equals(Identity other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (other is null) return false;

                int typeCount;
                return gridIndex == other.gridIndex
                    && type == other.type
                    && sql == other.sql
                    && commandType == other.commandType
                    && connectionStringComparer.Equals(connectionString, other.connectionString)
                    && parametersType == other.parametersType
                    && (typeCount = TypeCount) == other.TypeCount
                    && (typeCount == 0 || TypesEqual(this, other, typeCount));
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static bool TypesEqual(Identity x, Identity y, int count)
            {
                if (y.TypeCount != count) return false;
                for (int i = 0; i < count; i++)
                {
                    if (x.GetType(i) != y.GetType(i))
                        return false;
                }
                return true;
            }
        }
        #endregion
        #region // IMemberMap
        /// <summary>
        /// Implements this interface to provide custom member mapping
        /// </summary>
        public interface IMemberMap
        {
            /// <summary>
            /// Source DataReader column name
            /// </summary>
            string ColumnName { get; }

            /// <summary>
            ///  Target member type
            /// </summary>
            Type MemberType { get; }

            /// <summary>
            /// Target property
            /// </summary>
            PropertyInfo Property { get; }

            /// <summary>
            /// Target field
            /// </summary>
            FieldInfo Field { get; }

            /// <summary>
            /// Target constructor parameter
            /// </summary>
            ParameterInfo Parameter { get; }
        }
        #endregion
        #region // ITypeMap
        /// <summary>
        /// Implement this interface to change default mapping of reader columns to type members
        /// </summary>
        public interface ITypeMap
        {
            /// <summary>
            /// Finds best constructor
            /// </summary>
            /// <param name="names">DataReader column names</param>
            /// <param name="types">DataReader column types</param>
            /// <returns>Matching constructor or default one</returns>
            ConstructorInfo FindConstructor(string[] names, Type[] types);

            /// <summary>
            /// Returns a constructor which should *always* be used.
            /// 
            /// Parameters will be default values, nulls for reference types and zero'd for value types.
            /// 
            /// Use this class to force object creation away from parameterless constructors you don't control.
            /// </summary>
            ConstructorInfo FindExplicitConstructor();

            /// <summary>
            /// Gets mapping for constructor parameter
            /// </summary>
            /// <param name="constructor">Constructor to resolve</param>
            /// <param name="columnName">DataReader column name</param>
            /// <returns>Mapping implementation</returns>
            IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName);

            /// <summary>
            /// Gets member mapping for column
            /// </summary>
            /// <param name="columnName">DataReader column name</param>
            /// <returns>Mapping implementation</returns>
            IMemberMap GetMember(string columnName);
        }
        #endregion
        #region // Link
        /// <summary>
        /// This is a micro-cache; suitable when the number of terms is controllable (a few hundred, for example),
        /// and strictly append-only; you cannot change existing values. All key matches are on **REFERENCE**
        /// equality. The type is fully thread-safe.
        /// </summary>
        /// <typeparam name="TKey">The type to cache.</typeparam>
        /// <typeparam name="TValue">The value type of the cache.</typeparam>
        internal class Link<TKey, TValue> where TKey : class
        {
            public static bool TryGet(Link<TKey, TValue> link, TKey key, out TValue value)
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

            public static bool TryAdd(ref Link<TKey, TValue> head, TKey key, ref TValue value)
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
                    var newNode = new Link<TKey, TValue>(key, value, snapshot);
                    // did somebody move our cheese?
                    tryAgain = Interlocked.CompareExchange(ref head, newNode, snapshot) != snapshot;
                } while (tryAgain);
                return true;
            }

            private Link(TKey key, TValue value, Link<TKey, TValue> tail)
            {
                Key = key;
                Value = value;
                Tail = tail;
            }

            public TKey Key { get; }
            public TValue Value { get; }
            public Link<TKey, TValue> Tail { get; }
        }
        #endregion
        #region // LiteralToken
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
        #endregion
        #region // TypeDeserializerCache
        private class TypeDeserializerCache
        {
            private TypeDeserializerCache(Type type)
            {
                this.type = type;
            }

            private static readonly Hashtable byType = new Hashtable();
            private readonly Type type;
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
        #endregion
        #region // ITypeHandler
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
        #endregion
        #region // TypeHandlerCache
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
        #endregion
        #region // TableValuedParameter
        /// <summary>
        /// Used to pass a DataTable as a TableValuedParameter
        /// </summary>
        internal sealed class TableValuedParameter : ICustomQueryParameter
        {
            private readonly DataTable table;
            private readonly string typeName;

            /// <summary>
            /// Create a new instance of <see cref="TableValuedParameter"/>.
            /// </summary>
            /// <param name="table">The <see cref="DataTable"/> to create this parameter for</param>
            public TableValuedParameter(DataTable table) : this(table, null) { /* run base */ }

            /// <summary>
            /// Create a new instance of <see cref="TableValuedParameter"/>.
            /// </summary>
            /// <param name="table">The <see cref="DataTable"/> to create this parameter for.</param>
            /// <param name="typeName">The name of the type this parameter is for.</param>
            public TableValuedParameter(DataTable table, string typeName)
            {
                this.table = table;
                this.typeName = typeName;
            }

            void ICustomQueryParameter.AddParameter(IDbCommand command, string name)
            {
                var param = command.CreateParameter();
                param.ParameterName = name;
                Set(param, table, typeName);
                command.Parameters.Add(param);
            }

            internal static void Set(IDbDataParameter parameter, DataTable table, string typeName)
            {
#pragma warning disable 0618
                parameter.Value = SanitizeParameterValue(table);
#pragma warning restore 0618
                if (string.IsNullOrEmpty(typeName) && table != null)
                {
                    typeName = table.GetTypeName();
                }
                if (!string.IsNullOrEmpty(typeName)) StructuredHelper.ConfigureTVP(parameter, typeName);
            }
        }
        #endregion
        #region // TypeExtentions
        internal static string Name(this Type type)
        {
#if !NET40
            return type.GetTypeInfo().Name;
#else
            return type.Name;
#endif
        }

        internal static bool IsValueType(this Type type)
        {
#if !NET40
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }
        internal static bool IsEnum(this Type type)
        {
#if !NET40
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }
        internal static bool IsGenericType(this Type type)
        {
#if !NET40
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }
        internal static bool IsInterface(this Type type)
        {
#if !NET40
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }
#if !NET40
        internal static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit) as IEnumerable<Attribute>;
        }

        internal static TypeCode GetTypeCode(Type type)
        {
            if (type == null) return TypeCode.Empty;
            TypeCode result;
            if (typeCodeLookup.TryGetValue(type, out result)) return result;

            if (type.IsEnum())
            {
                type = Enum.GetUnderlyingType(type);
                if (typeCodeLookup.TryGetValue(type, out result)) return result;
            }
            return TypeCode.Object;
        }
        static readonly Dictionary<Type, TypeCode> typeCodeLookup = new Dictionary<Type, TypeCode>
        {
            {typeof(bool), TypeCode.Boolean },
            {typeof(byte), TypeCode.Byte },
            {typeof(char), TypeCode.Char},
            {typeof(DateTime), TypeCode.DateTime},
            {typeof(decimal), TypeCode.Decimal},
            {typeof(double), TypeCode.Double },
            {typeof(short), TypeCode.Int16 },
            {typeof(int), TypeCode.Int32 },
            {typeof(long), TypeCode.Int64 },
            {typeof(object), TypeCode.Object},
            {typeof(sbyte), TypeCode.SByte },
            {typeof(float), TypeCode.Single },
            {typeof(string), TypeCode.String },
            {typeof(ushort), TypeCode.UInt16 },
            {typeof(uint), TypeCode.UInt32 },
            {typeof(ulong), TypeCode.UInt64 },
        };
#else
        internal static TypeCode GetTypeCode(Type type)
        {
            return Type.GetTypeCode(type);
        }
#endif
        internal static MethodInfo GetPublicInstanceMethod(this Type type, string name, Type[] types)
        {
#if NETFx
            var method = type.GetMethod(name, types);
            return (method != null && method.IsPublic && !method.IsStatic) ? method : null;
#else
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public, null, types, null);
#endif
        }
        #endregion
        #region // IWrappedDataReader
        /// <summary>
        /// Describes a reader that controls the lifetime of both a command and a reader,
        /// exposing the downstream command/reader as properties.
        /// </summary>
        public interface IWrappedDataReader : IDataReader
        {
            /// <summary>
            /// Obtain the underlying reader
            /// </summary>
            IDataReader Reader { get; }
            /// <summary>
            /// Obtain the underlying command
            /// </summary>
            IDbCommand Command { get; }
        }
        #endregion
        #region // WrappedReader
        internal sealed class DisposedReader : DbDataReader
        {
            internal static readonly DisposedReader Instance = new DisposedReader();
            private DisposedReader() { }
            public override int Depth => 0;
            public override int FieldCount => 0;
            public override bool IsClosed => true;
            public override bool HasRows => false;
            public override int RecordsAffected => -1;
            public override int VisibleFieldCount => 0;

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static T ThrowDisposed<T>() => throw new ObjectDisposedException(nameof(DbDataReader));
#if NET40
            [MethodImpl(MethodImplOptions.NoInlining)]
            private static Task<T> ThrowDisposedAsync<T>()
            {
                return new Task<T>(() => ThrowDisposed<T>());
            }
#else
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async static Task<T> ThrowDisposedAsync<T>()
        {
            var result = ThrowDisposed<T>();
            await Task.Yield(); // will never hit this - already thrown and handled
            return result;
        }
#endif

            public override void Close() { }
            public override DataTable GetSchemaTable() => ThrowDisposed<DataTable>();

#if NET50
        [Obsolete("This Remoting API is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0010", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
            public override object InitializeLifetimeService() => ThrowDisposed<object>();
            protected override void Dispose(bool disposing) { }
            public override bool GetBoolean(int ordinal) => ThrowDisposed<bool>();
            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => ThrowDisposed<long>();
            public override float GetFloat(int ordinal) => ThrowDisposed<float>();
            public override short GetInt16(int ordinal) => ThrowDisposed<short>();
            public override byte GetByte(int ordinal) => ThrowDisposed<byte>();
            public override char GetChar(int ordinal) => ThrowDisposed<char>();
            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => ThrowDisposed<long>();
            public override string GetDataTypeName(int ordinal) => ThrowDisposed<string>();
            public override DateTime GetDateTime(int ordinal) => ThrowDisposed<DateTime>();
            protected override DbDataReader GetDbDataReader(int ordinal) => ThrowDisposed<DbDataReader>();
            public override decimal GetDecimal(int ordinal) => ThrowDisposed<decimal>();
            public override double GetDouble(int ordinal) => ThrowDisposed<double>();
            public override IEnumerator GetEnumerator() => ThrowDisposed<IEnumerator>();
            public override Type GetFieldType(int ordinal) => ThrowDisposed<Type>();
            public override Guid GetGuid(int ordinal) => ThrowDisposed<Guid>();
            public override int GetInt32(int ordinal) => ThrowDisposed<int>();
            public override long GetInt64(int ordinal) => ThrowDisposed<long>();
            public override string GetName(int ordinal) => ThrowDisposed<string>();
            public override int GetOrdinal(string name) => ThrowDisposed<int>();
            public override Type GetProviderSpecificFieldType(int ordinal) => ThrowDisposed<Type>();
            public override object GetProviderSpecificValue(int ordinal) => ThrowDisposed<object>();
            public override int GetProviderSpecificValues(object[] values) => ThrowDisposed<int>();
            public override string GetString(int ordinal) => ThrowDisposed<string>();
            public override object GetValue(int ordinal) => ThrowDisposed<object>();
            public override int GetValues(object[] values) => ThrowDisposed<int>();
            public override bool IsDBNull(int ordinal) => ThrowDisposed<bool>();
            public override bool NextResult() => ThrowDisposed<bool>();
            public override bool Read() => ThrowDisposed<bool>();
            public override object this[int ordinal] => ThrowDisposed<object>();
            public override object this[string name] => ThrowDisposed<object>();
#if NET40
            public T GetFieldValue<T>(int ordinal) => ThrowDisposed<T>();
            public Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => ThrowDisposedAsync<T>();
            public Stream GetStream(int ordinal) => ThrowDisposed<Stream>();
            public TextReader GetTextReader(int ordinal) => ThrowDisposed<TextReader>();
            public Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
            public Task<bool> NextResultAsync(CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
            public Task<bool> ReadAsync(CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
#else
        public override T GetFieldValue<T>(int ordinal) => ThrowDisposed<T>();
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => ThrowDisposedAsync<T>();
        public override Stream GetStream(int ordinal) => ThrowDisposed<Stream>();
        public override TextReader GetTextReader(int ordinal) => ThrowDisposed<TextReader>();
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => ThrowDisposedAsync<bool>();
#endif
        }

        internal static class WrappedReader
        {
            // the purpose of wrapping here is to allow closing a reader to *also* close
            // the command, without having to explicitly hand the command back to the
            // caller; what that actually looks like depends on what we get: if we are
            // given a DbDataReader, we will surface a DbDataReader; if we are given
            // a raw IDataReader, we will surface that; and if null: null
            public static IDataReader Create(IDbCommand cmd, IDataReader reader)
            {
                if (cmd == null) return reader; // no need to wrap if no command

                if (reader is DbDataReader dbr) return new DbWrappedReader(cmd, dbr);
                if (reader != null) return new BasicWrappedReader(cmd, reader);
                cmd.Dispose();
                return null; // GIGO
            }
            public static DbDataReader Create(IDbCommand cmd, DbDataReader reader)
            {
                if (cmd == null) return reader; // no need to wrap if no command

                if (reader != null) return new DbWrappedReader(cmd, reader);
                cmd.Dispose();
                return null; // GIGO
            }
        }
        internal sealed class DbWrappedReader : DbDataReader, IWrappedDataReader
        {
            private DbDataReader _reader;
            private IDbCommand _cmd;

            IDataReader IWrappedDataReader.Reader => _reader;

            IDbCommand IWrappedDataReader.Command => _cmd;

            public DbWrappedReader(IDbCommand cmd, DbDataReader reader)
            {
                _cmd = cmd;
                _reader = reader;
            }

            public DbWrappedReader(IDbCommand cmd, DbWrappedReader reader)
            {
                _cmd = cmd;
                _reader = reader;
            }

            public override bool HasRows => _reader.HasRows;

            public override void Close() => _reader.Close();
            public override DataTable GetSchemaTable() => _reader.GetSchemaTable();

#if NET50
        [Obsolete("This Remoting API is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0010", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
            public override object InitializeLifetimeService() => _reader.InitializeLifetimeService();

            public override int Depth => _reader.Depth;

            public override bool IsClosed => _reader.IsClosed;

            public override bool NextResult() => _reader.NextResult();

            public override bool Read() => _reader.Read();

            public override int RecordsAffected => _reader.RecordsAffected;

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _reader.Close();
                    _reader.Dispose();
                    _reader = DisposedReader.Instance; // all future ops are no-ops
                    _cmd?.Dispose();
                    _cmd = null;
                }
            }

            public override int FieldCount => _reader.FieldCount;

            public override bool GetBoolean(int i) => _reader.GetBoolean(i);

            public override byte GetByte(int i) => _reader.GetByte(i);

            public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
                _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

            public override char GetChar(int i) => _reader.GetChar(i);

            public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
                _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

            public override string GetDataTypeName(int i) => _reader.GetDataTypeName(i);

            public override DateTime GetDateTime(int i) => _reader.GetDateTime(i);

            public override decimal GetDecimal(int i) => _reader.GetDecimal(i);

            public override double GetDouble(int i) => _reader.GetDouble(i);

            public override Type GetFieldType(int i) => _reader.GetFieldType(i);

            public override float GetFloat(int i) => _reader.GetFloat(i);

            public override Guid GetGuid(int i) => _reader.GetGuid(i);

            public override short GetInt16(int i) => _reader.GetInt16(i);

            public override int GetInt32(int i) => _reader.GetInt32(i);

            public override long GetInt64(int i) => _reader.GetInt64(i);

            public override string GetName(int i) => _reader.GetName(i);

            public override int GetOrdinal(string name) => _reader.GetOrdinal(name);

            public override string GetString(int i) => _reader.GetString(i);

            public override object GetValue(int i) => _reader.GetValue(i);

            public override int GetValues(object[] values) => _reader.GetValues(values);

            public override bool IsDBNull(int i) => _reader.IsDBNull(i);

            public override object this[string name] => _reader[name];

            public override object this[int i] => _reader[i];
            public override IEnumerator GetEnumerator() => _reader.GetEnumerator();
            public override Type GetProviderSpecificFieldType(int ordinal) => _reader.GetProviderSpecificFieldType(ordinal);
            public override object GetProviderSpecificValue(int ordinal) => _reader.GetProviderSpecificValue(ordinal);
            public override int GetProviderSpecificValues(object[] values) => _reader.GetProviderSpecificValues(values);
            public override int VisibleFieldCount => _reader.VisibleFieldCount;
            protected override DbDataReader GetDbDataReader(int ordinal) => (((IDataReader)_reader).GetData(ordinal) as DbDataReader) ?? throw new NotSupportedException();
#if NET40
            public T GetFieldValue<T>(int ordinal) => _reader.GetFieldValue<T>(ordinal);
            public Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => _reader.GetFieldValueAsync<T>(ordinal, cancellationToken);
            public Stream GetStream(int ordinal) => _reader.GetStream(ordinal);
            public TextReader GetTextReader(int ordinal) => _reader.GetTextReader(ordinal);
            public Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => _reader.IsDBNullAsync(ordinal, cancellationToken);
            public Task<bool> NextResultAsync(CancellationToken cancellationToken) => _reader.NextResultAsync(cancellationToken);
            public Task<bool> ReadAsync(CancellationToken cancellationToken) => _reader.ReadAsync(cancellationToken);
#else
        public override T GetFieldValue<T>(int ordinal) => _reader.GetFieldValue<T>(ordinal);
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => _reader.GetFieldValueAsync<T>(ordinal, cancellationToken);
        public override Stream GetStream(int ordinal) => _reader.GetStream(ordinal);
        public override TextReader GetTextReader(int ordinal) => _reader.GetTextReader(ordinal);
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => _reader.IsDBNullAsync(ordinal, cancellationToken);
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => _reader.NextResultAsync(cancellationToken);
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => _reader.ReadAsync(cancellationToken);
#endif
        }

        internal class BasicWrappedReader : IWrappedDataReader
        {
            private IDataReader _reader;
            private IDbCommand _cmd;

            IDataReader IWrappedDataReader.Reader => _reader;

            IDbCommand IWrappedDataReader.Command => _cmd;

            public BasicWrappedReader(IDbCommand cmd, IDataReader reader)
            {
                _cmd = cmd;
                _reader = reader;
            }

            void IDataReader.Close() => _reader.Close();

            int IDataReader.Depth => _reader.Depth;

            DataTable IDataReader.GetSchemaTable() => _reader.GetSchemaTable();

            bool IDataReader.IsClosed => _reader.IsClosed;

            bool IDataReader.NextResult() => _reader.NextResult();

            bool IDataReader.Read() => _reader.Read();

            int IDataReader.RecordsAffected => _reader.RecordsAffected;

            void IDisposable.Dispose()
            {
                _reader.Close();
                _reader.Dispose();
                _reader = DisposedReader.Instance;
                _cmd?.Dispose();
                _cmd = null;
            }

            int IDataRecord.FieldCount => _reader.FieldCount;

            bool IDataRecord.GetBoolean(int i) => _reader.GetBoolean(i);

            byte IDataRecord.GetByte(int i) => _reader.GetByte(i);

            long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
                _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

            char IDataRecord.GetChar(int i) => _reader.GetChar(i);

            long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
                _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

            IDataReader IDataRecord.GetData(int i) => _reader.GetData(i);

            string IDataRecord.GetDataTypeName(int i) => _reader.GetDataTypeName(i);

            DateTime IDataRecord.GetDateTime(int i) => _reader.GetDateTime(i);

            decimal IDataRecord.GetDecimal(int i) => _reader.GetDecimal(i);

            double IDataRecord.GetDouble(int i) => _reader.GetDouble(i);

            Type IDataRecord.GetFieldType(int i) => _reader.GetFieldType(i);

            float IDataRecord.GetFloat(int i) => _reader.GetFloat(i);

            Guid IDataRecord.GetGuid(int i) => _reader.GetGuid(i);

            short IDataRecord.GetInt16(int i) => _reader.GetInt16(i);

            int IDataRecord.GetInt32(int i) => _reader.GetInt32(i);

            long IDataRecord.GetInt64(int i) => _reader.GetInt64(i);

            string IDataRecord.GetName(int i) => _reader.GetName(i);

            int IDataRecord.GetOrdinal(string name) => _reader.GetOrdinal(name);

            string IDataRecord.GetString(int i) => _reader.GetString(i);

            object IDataRecord.GetValue(int i) => _reader.GetValue(i);

            int IDataRecord.GetValues(object[] values) => _reader.GetValues(values);

            bool IDataRecord.IsDBNull(int i) => _reader.IsDBNull(i);

            object IDataRecord.this[string name] => _reader[name];

            object IDataRecord.this[int i] => _reader[i];
        }
        #endregion
    }
}
