using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace System.Data.Dabber.Cobber
{
    /// <summary>
    /// 连接创建者
    /// </summary>
    public class ConnBuilder : DbConnectionStringBuilder
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ConnBuilder() : this(StoreType.SQLite, $"DataSource={nameof(ConnBuilder)}")
        {

        }
        /// <summary>
        /// 存储模型创建者
        /// </summary>
        /// <param name="storeModel"></param>
        public ConnBuilder(StoreModel storeModel) : this(storeModel.DbType, storeModel.ConnString)
        {

        }
        /// <summary>
        /// 使用数据卡类型及连接字符串的构造
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="connString"></param>
        public ConnBuilder(StoreType storeType, string connString)
        {
            StoreType = storeType;
            ConnectionString = connString;
            SetDefault(storeType);
        }
        /// <summary>
        /// 存储类型
        /// </summary>
        public StoreType StoreType { get; set; }
        /// <summary>
        /// 连接池数量
        /// </summary>
        public Int32 PoolSize { get; set; }
        /// <summary>
        /// 连接池最小数量
        /// </summary>
        public Int32 MinPoolSize { get; set; }
        /// <summary>
        /// 连接池最大数量
        /// </summary>
        public Int32 MaxPoolSize { get; set; }
        /// <summary>
        /// 连接池
        /// </summary>
        public bool Pooling { get; set; }

        private void SetDefault(StoreType storeType)
        {
            switch (storeType)
            {
                case StoreType.SQLite:
                    GetOrAdd(nameof(Pooling), true);
                    GetOrAdd(nameof(PoolSize), 5);
                    GetOrAdd(nameof(MinPoolSize), 5);
                    GetOrAdd(nameof(MaxPoolSize), 10);
                    break;
                case StoreType.SqlServer:
                    GetOrAdd(nameof(Pooling), true);
                    GetOrAdd(nameof(MinPoolSize), 5);
                    GetOrAdd(nameof(MaxPoolSize), 50);
                    break;
                case StoreType.MySQL:
                    GetOrAdd(nameof(Pooling), true);
                    GetOrAdd(nameof(MinPoolSize), 5);
                    GetOrAdd(nameof(MaxPoolSize), 50);
                    break;
                case StoreType.Oracle:
                    break;
                case StoreType.PostgreSQL:
                    break;
                case StoreType.Redis:
                    break;
                case StoreType.Access:
                    break;
                case StoreType.Excel:
                    break;
                case StoreType.Xml:
                    break;
                case StoreType.Memory:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object GetOrAdd(string key, object value) 
            => ContainsKey(key) ? this[key] : (this[key] = value);
        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="GetValue"></param>
        /// <returns></returns>
        public object GetOrAdd(string key, Func<object> GetValue) 
            => ContainsKey(key) ? this[key] : (this[key] = GetValue());
    }
}
