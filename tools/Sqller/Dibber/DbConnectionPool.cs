using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace System.Data.Dibber
{
    /// <summary>
    /// 连接池
    /// </summary>
    public class DbConnectionPool
    {
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static T GetConnection<T>(string connString) where T : DbConnection
        {
            return DbConnectionPool<T>.GetConnection(connString);
        }
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="CreateConn"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(string connString, Func<string, DbConnection> CreateConn)
        {
            return DbConnPool<DbConnection>.GetConnection(connString, CreateConn);
        }
    }
    /// <summary>
    /// 自动生成泛型继承类的通用数据库连接池(推荐使用)
    /// 当数据库连接为sealed class时请使用DbConnPool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbConnectionPool<T> where T : DbConnection
    {
        private static ConcurrentDictionary<string, DbConnectionPool<T>> _poolDic = new();
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static T GetConnection(string connString)
        {
            return GetPool(connString).GetInstance();
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static DbConnectionPool<T> GetPool(string connString)
        {
            DbConnectionPool<T> pool;
            if (Monitor.TryEnter(_poolDic, TimeSpan.FromSeconds(5)))
            {
                pool = _poolDic.GetOrAdd(connString, (k) => new DbConnectionPool<T>(connString));
                Monitor.Exit(_poolDic);
            }
            else
            {
                pool = new DbConnectionPool<T>(connString);
            }
            return pool;
        }
        #region // 内部实现
        /// <summary>
        /// 连接池队列
        /// </summary>
        private ConcurrentQueue<T> _connPool = new();
        private Timer _clearTimer = new Timer
        {
            Enabled = true, // 启用执行
            Interval = 1800000, // 半小时执行一次
            AutoReset = true, // 一直执行
        };
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; }
        /// <summary>
        /// 私有构造
        /// </summary>
        /// <param name="connString"></param>
        private DbConnectionPool(string connString)
        {
            ConnString = connString;
            // 定时清理连接池
            _clearTimer.Elapsed += (s, e) =>
            {
                if (_connPool.TryDequeue(out T curr)) // 池子里还有东西就继续清理
                {
                    if (curr is InnerPool ip)
                    {
                        ip.Release();
                    }
                }
            };
            _clearTimer.Start();
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public T GetInstance()
        {
            if (_connPool.IsEmpty || !_connPool.TryDequeue(out T result))
            {
                result = CreateInstance(ConnString);
            }
            return result;
        }
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="instance"></param>
        public void Recycle(T instance)
        {
            if (instance != null)
            {
                _connPool.Enqueue(instance);
            }
        }
        #endregion
        /// <summary>
        /// 创建实例
        /// </summary>
        public static Func<string, T> CreateInstance { get; }
        /// <summary>
        /// 静态构造
        /// </summary>
        static DbConnectionPool()
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
        }
        /// <summary>
        /// 内部池
        /// </summary>
        public interface InnerPool
        {
            /// <summary>
            /// 释放
            /// </summary>
            void Release();
        }
    }
    /// <summary>
    /// 再次封装的数据库代理连接池(不推荐使用)
    /// 如果数据库连接为sealed class时推荐使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbConnPool<T> where T : DbConnection
    {
        private static ConcurrentDictionary<string, DbConnPool<T>> _poolDic = new();
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static DbConnection GetConnection(string connString, Func<String, T> CreateConn)
        {
            return GetPool(connString).GetInstance(CreateConn);
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static DbConnPool<T> GetPool(string connString)
        {
            DbConnPool<T> pool;
            if (Monitor.TryEnter(_poolDic, TimeSpan.FromSeconds(5)))
            {
                pool = _poolDic.GetOrAdd(connString, (k) => new DbConnPool<T>(connString));
                Monitor.Exit(_poolDic);
            }
            else
            {
                pool = new DbConnPool<T>(connString);
            }
            return pool;
        }
        #region // 内部实现
        /// <summary>
        /// 连接池队列
        /// </summary>
        private ConcurrentQueue<DbConnProxy> _connPool = new();
        private Timer _clearTimer = new Timer
        {
            Enabled = true, // 启用执行
            Interval = 1800000, // 半小时执行一次
            AutoReset = true, // 一直执行
        };
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; }
        /// <summary>
        /// 私有构造
        /// </summary>
        /// <param name="connString"></param>
        private DbConnPool(string connString)
        {
            ConnString = connString;
            // 定时清理连接池
            _clearTimer.Elapsed += (s, e) =>
            {
                if (_connPool.TryDequeue(out DbConnProxy curr)) // 池子里还有东西就继续清理
                {
                    curr.Release();
                }
            };
            _clearTimer.Start();
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public DbConnection GetInstance(Func<string, T> CreateConn)
        {
            if (_connPool.IsEmpty || !_connPool.TryDequeue(out DbConnProxy result))
            {
                result = new DbConnProxy(CreateConn(ConnString));
            }
            return result;
        }
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="instance"></param>
        public void Recycle(DbConnection instance)
        {
            if (instance is DbConnProxy conn)
            {
                _connPool.Enqueue(conn);
            }
        }
        #endregion
        #region // 内部类
        /// <summary>
        /// 带有连接池的连接
        /// </summary>
        internal class DbConnProxy : DbConnection
        {
            public T RealConnection { get; }
            /// <summary>
            /// 构造
            /// </summary>
            public DbConnProxy(T model)
            {
                RealConnection = model;
            }
            /// <summary>
            /// 不释放资源
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    GetPool(ConnectionString).Recycle(this);
                }
                else
                {
                    RealConnection.Dispose();
                    base.Dispose(true);
                }
            }
            /// <summary>
            /// 释放资源
            /// </summary>
            public virtual void Release()
            {
                Dispose(false);
            }
            #region // 抽象方法实现
            public override string ConnectionString { get => RealConnection.ConnectionString; set => RealConnection.ConnectionString = value; }

            public override string Database => RealConnection.Database;

            public override string DataSource => RealConnection.DataSource;

            public override string ServerVersion => RealConnection.ServerVersion;

            public override ConnectionState State => RealConnection.State;

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                return RealConnection.BeginTransaction(isolationLevel);
            }

            public override void ChangeDatabase(string databaseName)
            {
                RealConnection.ChangeDatabase(databaseName);
            }

            public override void Close()
            {
                RealConnection.Close();
            }

            protected override DbCommand CreateDbCommand()
            {
                return RealConnection.CreateCommand();
            }

            public override void Open()
            {
                RealConnection.Open();
            }
            #endregion
            #region // 重写方法
            //protected override bool CanRaiseEvents => RealConnection.CanRaiseEvents;
            public override int ConnectionTimeout => RealConnection.ConnectionTimeout;
            //protected override DbProviderFactory DbProviderFactory => RealConnection.DbProviderFactory;
#if NETFx
            public override void EnlistTransaction(System.Transactions.Transaction transaction)
            {
                RealConnection.EnlistTransaction(transaction);
            }
#endif
            public override bool Equals(object obj)
            {
                return RealConnection.Equals(obj);
            }
            public override int GetHashCode()
            {
                return RealConnection.GetHashCode();
            }
            public override DataTable GetSchema()
            {
                return RealConnection.GetSchema();
            }
            public override DataTable GetSchema(string collectionName)
            {
                return RealConnection.GetSchema(collectionName);
            }
            public override DataTable GetSchema(string collectionName, string[] restrictionValues)
            {
                return RealConnection.GetSchema(collectionName, restrictionValues);
            }
            //protected override object GetService(Type service)
            //{
            //    return RealConnection.GetService(service);
            //}
#if NET50
            [Obsolete]
#endif
            public override object InitializeLifetimeService()
            {
                return RealConnection.InitializeLifetimeService();
            }
            //protected override void OnStateChange(StateChangeEventArgs stateChange)
            //{
            //    RealConnection.OnStateChange(stateChange);
            //}
#if NETFx
            public override Task OpenAsync(CancellationToken cancellationToken)
            {
                return RealConnection.OpenAsync(cancellationToken);
            }
#endif
            public override ISite Site { get => RealConnection.Site; set => RealConnection.Site = value; }
            public override string ToString()
            {
                return RealConnection.ToString();
            }
#endregion
        }
#endregion
    }
}
