using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Cobber
{
    /// <summary>
    /// Redis数据库序号功能枚举
    /// </summary>
    public enum RedisDbEnum : int
    {
        /// <summary>
        /// 全局
        /// </summary>
        Global = 0,
        /// <summary>
        /// 定义
        /// </summary>
        Definition = 1,
        /// <summary>
        /// 模型
        /// </summary>
        Model = 2,
        /// <summary>
        /// 实体
        /// </summary>
        Entity = 3,
        /// <summary>
        /// 服务
        /// </summary>
        Service = 4,
        /// <summary>
        /// 组件
        /// </summary>
        Component = 5,
        /// <summary>
        /// 区域
        /// </summary>
        Region = 6,
        /// <summary>
        /// 领域
        /// </summary>
        Domain = 7,
        /// <summary>
        /// 环境
        /// </summary>
        Environment = 8,
        /// <summary>
        /// 设置
        /// </summary>
        Setting = 9,
        /// <summary>
        /// 分享
        /// </summary>
        Share = 10,
        /// <summary>
        /// 订阅
        /// </summary>
        Subscribe = 11,
        /// <summary>
        /// 转换
        /// </summary>
        Transform = 12,
        /// <summary>
        /// 内存
        /// </summary>
        Memory = 13,
        /// <summary>
        /// 公共
        /// </summary>
        Common = 14,
        /// <summary>
        /// 最后
        /// </summary>
        Demon = 15,
    }
}
