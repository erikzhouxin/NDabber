using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Cobber;
using System.Data.Common;
using System.Data.Dabber;
using System.Data.SQLiteCipher;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    public class DbConnUT
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var conn = @"DataSource=C:\Users\Admin\Documents\CenIdea\Qualimetry\CenIdea.Qualimetry.NEamsUI.sqlite;Version=4;Password=Eams2020!@#;";
            var conner = SqliteConnectionPool.GetConnection(conn);
            conner.Open();
            var res = conner.Query("select * from sqlite_master");
            var count = res.Count();
            Console.WriteLine(count);
        }

        [Test]
        public void TestInitDatabase()
        {
            var connString = @"DataSource=|DataDirectory|CenIdea.Qualimetry.NEamsUI.sqlite;Version=4;Password=Eams2020!@#;";
            DbAutoMigration.CreateLocal().RegistAttribute(Initialize)
                .Start(StoreType.SQLite, connString, () => SqliteConnectionPool.GetConnection(connString));
        }

        [DbMigrationAttribute(20200110101022)]
        private IAlertMsg Initialize(DbConnection conn, DbTransaction trans)
        {
            SQLiteDbMigration.CheckTable<TDeviceBackups>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceEquipments>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceInstruments>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceLoggers>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceMenus>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceParams>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceSettings>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceResources>(conn, trans);
            SQLiteDbMigration.CheckTable<TDeviceDicts>(conn, trans);
            return AlertMsg.OperSuccess;
        }

        [Test]
        public void TestYield()
        {
            foreach (var item in GetMsgs())
            {
                Console.WriteLine(item.Message);
            }
        }
        private IEnumerable<IAlertMsg> GetMsgs()
        {
            IAlertMsg success = AlertMsg.OperSuccess;
            IAlertMsg error = null;
            yield return new AlertMsg(false, $"success:{success?.Message},error:{error?.Message}");
            error = AlertMsg.OperError;
            yield return new AlertMsg(false, $"success:{success?.Message},error:{error?.Message}");
        }
    }
    #region // 实体类
    /// <summary>
    /// 版本号接口
    /// </summary>
    public interface ILocalVersion
    {
        /// <summary>
        /// 版本号
        /// </summary>
        Int32 Version { get; set; }
    }
    /// <summary>
    /// 本地备份数据表
    /// </summary>
    [DbCol("本地备份数据表", Name = "t_device_backups")]
    public class TDeviceBackups
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "标识")]
        [Column("id")]
        [DbCol("标识", Name = "id", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        [Column("type")]
        [DbCol("类型", Name = "type", Len = 1024)]
        public virtual String Type { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Column("content")]
        [DbCol("内容", Name = "content", Type = DbColType.StringMax)]
        public virtual String Content { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [Display(Name = "时间")]
        [Column("ctime")]
        [DbCol("时间", Name = "ctime", Type = DbColType.DateTime)]
        public virtual DateTime CTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 客户端标识
        /// </summary>
        [Display(Name = "客户端标识")]
        [Column("deviceid")]
        [DbCol("客户端标识", Name = "deviceid", Len = 255)]
        public virtual String DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 客户端类型
        /// </summary>
        [Display(Name = "客户端类型")]
        [Column("clienttype")]
        [DbCol("客户端类型", Name = "clienttype", Len = 255)]
        public virtual String ClientType { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        [Column("status")]
        [DbCol("状态", Name = "status", Type = DbColType.Int32)]
        public virtual Int32 Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [Column("memo")]
        [DbCol("备注", Name = "memo", Len = 255, Default = "备份")]
        public virtual String Memo { get; set; } = "备份";
    }
    /// <summary>
    /// 终端仪器设备表
    /// </summary>
    [Table("t_device_equipments")]
    [DbCol("终端仪器设备表", Name = "t_device_equipments")]
    public class TDeviceEquipments
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Key = DbIxType.PK)]
        public String ID { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [DbCol("编号")]
        public String Code { get; set; } = string.Empty;
        /// <summary>
        /// 设备型号
        /// </summary>
        [DbCol("设备型号", Len = 255)]
        public virtual String Name { get; set; } = string.Empty;
        /// <summary>
        /// 设备类型
        /// </summary>
        [DbCol("设备类型", Type = DbColType.Int32)]
        public virtual Int32 Type { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        [DbCol("波特率", Type = DbColType.Int32)]
        public virtual Int32 BaudRate { get; set; } = 2400;
        /// <summary>
        /// 数据位
        /// </summary>
        [DbCol("数据位", Type = DbColType.Int32)]
        public virtual Int32 DataBits { get; set; } = 8;
        /// <summary>
        /// 停止位
        /// </summary>
        [DbCol("停止位", Type = DbColType.Int32)]
        public virtual Int32 StopBit { get; set; } = 1;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        [DbCol("奇偶校验", Type = DbColType.Int32)]
        public virtual Int32 Parity { get; set; } = 0;
        /// <summary>
        /// 字节长度
        /// </summary>
        [DbCol("字节长度", Type = DbColType.Int32)]
        public virtual Int32 ByteSize { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [DbCol("IP地址")]
        public virtual String IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        [DbCol("端口", Type = DbColType.Int32)]
        public virtual Int32 Port { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        [DbCol("附加信息", Type = DbColType.StringNormal)]
        public virtual String Extra { get; set; } = "{}";
        /// <summary>
        /// 编辑人
        /// </summary>
        [DbCol("编辑人")]
        public virtual String Editor { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DbCol("编辑时间", Type = DbColType.DateTime)]
        public virtual DateTime EditTime { get; set; }
        /// <summary>
        /// 删除标识
        /// </summary>
        [DbCol("删除标识", Type = DbColType.Int32)]
        public virtual Int32 Flag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Remarks { get; set; } = string.Empty;
        /// <summary>
        /// 版本号
        /// </summary>
        [DbCol("版本号", Type = DbColType.Int32)]
        public virtual Int32 Version { get; set; }
        /// <summary>
        /// 终端类型
        /// </summary>
        [DbCol("终端类型", Type = DbColType.Int32)]
        public virtual Int32 Mode { get; set; }
    }
    /// <summary>
    /// 终端仪器设备的配置
    /// </summary>
    [Table("t_device_instruments")]
    [DbCol("终端仪器设备的配置", Name = "t_device_instruments")]
    public class TDeviceInstruments
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Key = DbIxType.PK)]
        public virtual String ID { get; set; }
        /// <summary>
        /// 设备标识
        /// </summary>
        [DbCol("设备标识")]
        public virtual string EquipID { get; set; }
        /// <summary>
        /// 是网口
        /// </summary>
        [DbCol("是网口", Type = DbColType.Boolean)]
        public virtual Boolean IsNet { get; set; }
        /// <summary>
        /// 串口号
        /// </summary>
        [DbCol("串口号")]
        public String PortName { get; set; } = "COM1";
        /// <summary>
        /// 安装位置
        /// </summary>
        [DbCol("安装位置", Len = 255)]
        public String Location { get; set; } = "";
        /// <summary>
        /// 波特率
        /// </summary>
        [DbCol("波特率", Type = DbColType.Int32)]
        public virtual Int32 BaudRate { get; set; } = 2400;
        /// <summary>
        /// 数据位
        /// </summary>
        [DbCol("数据位", Type = DbColType.Int32)]
        public virtual Int32 DataBits { get; set; } = 8;
        /// <summary>
        /// 编码格式
        /// </summary>
        [DbCol("编码格式", Type = DbColType.Int32)]
        public virtual Int32 Encoding { get; set; } = System.Text.Encoding.UTF8.CodePage;
        /// <summary>
        /// 停止位
        /// </summary>
        [DbCol("停止位", Type = DbColType.Int32)]
        public virtual Int32 StopBit { get; set; } = 1;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        [DbCol("奇偶校验", Type = DbColType.Int32)]
        public virtual Int32 Parity { get; set; } = 0;
        /// <summary>
        /// IP地址
        /// </summary>
        [DbCol("IP地址", Len = 64)]
        public virtual string IPAddress { get; set; } = "127.0.0.1";
        /// <summary>
        /// 端口
        /// </summary>
        [DbCol("端口", Type = DbColType.Int32)]
        public virtual Int32 Port { get; set; }
        /// <summary>
        /// 天线
        /// </summary>
        [DbCol("天线", Type = DbColType.Int32)]
        public virtual Int32 Antenna { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        [DbCol("附加信息", Type = DbColType.StringMax)]
        public virtual String Extra { get; set; } = "{}";
        /// <summary>
        /// 安装部门标识
        /// </summary>
        [DbCol("安装部门标识", Type = DbColType.Int32)]
        public virtual Int32 DeptID { get; set; }
        /// <summary>
        /// 安装部门名称
        /// </summary>
        [DbCol("安装部门名称")]
        public virtual String DeptName { get; set; } = "";
        /// <summary>
        /// 安装位置
        /// </summary>
        [DbCol("安装位置")]
        public virtual String Position { get; set; } = string.Empty;
        /// <summary>
        /// 设备账号
        /// </summary>
        [DbCol("设备账号")]
        public virtual String Account { get; set; } = string.Empty;
        /// <summary>
        /// 设备密码
        /// </summary>
        [DbCol("设备密码")]
        public virtual String Password { get; set; } = string.Empty;
        /// <summary>
        /// 创建人
        /// </summary>
        [DbCol("创建人")]
        public virtual string Editor { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DbCol("创建时间", Type = DbColType.DateTime)]
        public virtual DateTime EditTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 删除标识
        /// </summary>
        [DbCol("删除标识", Type = DbColType.Int32)]
        public virtual Int32 Flag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Remarks { get; set; } = string.Empty;
    }
    /// <summary>
    /// 本地日志
    /// </summary>
    [Table("t_device_loggers")]
    [DbCol("日志记录表", Name = "t_device_loggers")]
    public partial class TDeviceLoggers
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("唯一标识", Len = 64, Key = DbIxType.PK)]
        public virtual String ID { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [DbCol("日志类型", Type = DbColType.Int32)]
        public virtual Int32 Type { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        [DbCol("日志内容", Type = DbColType.StringNormal)]
        public virtual String Content { get; set; }
        /// <summary>
        /// 操作的数据集
        /// </summary>
        [DbCol("操作的数据集", Type = DbColType.StringNormal, IsReq = false)]
        public virtual String Data { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        [DbCol("用户标识", Len = 64)]
        public virtual String UserID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [DbCol("用户名称", Len = 255)]
        public virtual String UserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DbCol("创建时间", Type = DbColType.DateTime)]
        public virtual DateTime CTime { get; set; }
        /// <summary>
        /// 终端Mac/客户端IP
        /// </summary>
        [DbCol("终端Mac/客户端IP", Len = 255)]
        public virtual String DeviceID { get; set; }
        /// <summary>
        /// 客户端类型
        /// </summary>
        [DbCol("客户端类型", Len = 255)]
        public virtual String CType { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        [DbCol("标记", Type = DbColType.Int32)]
        public virtual Int32 Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255, Default = "操作日志")]
        public virtual String Memo { get; set; } = string.Empty;
    }
    /// <summary>
    /// 系统菜单列表
    /// </summary>
    [Table("t_device_menus")]
    [DbCol("系统菜单列表", Name = "t_device_menus")]
    public partial class TDeviceMenus : ILocalVersion
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 菜单标识
        /// </summary>
        [DbCol("菜单标识", Key = DbIxType.UIX, Index = nameof(MID))]
        public virtual String MID { get; set; }
        /// <summary>
        /// 上级菜单标识
        /// </summary>
        [DbCol("上级菜单标识")]
        public virtual String PID { get; set; }
        /// <summary>
        /// 分组类型
        /// </summary>
        [DbCol("分组类型", Type = DbColType.Int32)]
        public virtual int Group { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [DbCol("参数值", Len = 255)]
        public virtual String Refer { get; set; }
        /// <summary>
        /// 类名标识
        /// </summary>
        [DbCol("类名标识")]
        public virtual String Clazz { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        [DbCol("菜单名称")]
        public virtual String Name { get; set; }
        /// <summary>
        /// 菜单显示
        /// </summary>
        [DbCol("菜单显示")]
        public virtual String Display { get; set; }
        /// <summary>
        /// 定位连接
        /// </summary>
        [DbCol("定位连接", Len = 255)]
        public virtual String Url { get; set; }
        /// <summary>
        /// 显示图标
        /// </summary>
        [DbCol("显示图标", Type = DbColType.Int32)]
        public virtual Int32 Icon { get; set; }
        /// <summary>
        /// 按钮列表
        /// </summary>
        [DbCol("按钮列表", Type = DbColType.StringNormal)]
        public virtual String Buttons { get; set; }
        /// <summary>
        /// 附加内容
        /// </summary>
        [DbCol("附加内容", Type = DbColType.StringNormal)]
        public virtual String Extra { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DbCol("排序", Type = DbColType.Int32)]
        public virtual Int32 Order { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [DbCol("类型", Type = DbColType.Int32)]
        public virtual Int32 Type { get; set; }
        /// <summary>
        /// 标记(0:正常,1:禁用,2:删除)
        /// </summary>
        [DbCol("标记(0:正常,1:禁用,2:删除)", Type = DbColType.Int32)]
        public virtual Int32 Flag { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [DbCol("修改人", Type = DbColType.Int32)]
        public virtual String Editor { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DbCol("修改时间", Type = DbColType.DateTime)]
        public virtual DateTime ETime { get; set; }
        /// <summary>
        /// 终端类型[DeviceType]
        /// </summary>
        [DbCol("终端类型", Type = DbColType.Int32)]
        public virtual Int32 Mode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 1024)]
        public virtual String Remarks { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [DbCol("版本号", Type = DbColType.Int32)]
        public virtual Int32 Version { get; set; }
    }
    /// <summary>
    /// 本地参数表
    /// </summary>
    [Table("t_device_params")]
    [DbCol("本地参数表", Name = "t_device_params")]
    public partial class TDeviceParams
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 键名
        /// </summary>
        [DbCol("键名", Len = 255)]
        public virtual String Key { get; set; }
        /// <summary>
        /// 键值
        /// </summary>
        [DbCol("键值", Type = DbColType.StringMax)]
        public virtual String Value { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Memo { get; set; }
    }
    /// <summary>
    /// 本地资源表
    /// </summary>
    [Table("t_device_resources")]
    [DbCol("本地资源表", Name = "t_device_resources")]
    public partial class TDeviceResources
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        [DbCol("区域代码", Len = 255, Key = DbIxType.UIX, Index = nameof(Region) + "|" + nameof(Category) + "|" + nameof(Catalog))]
        public virtual String Region { get; set; }
        /// <summary>
        /// 大类
        /// </summary>
        [DbCol("大类", Type = DbColType.Int32)]
        public virtual Int32 Category { get; set; }
        /// <summary>
        /// 小类
        /// </summary>
        [DbCol("小类", Len = 255)]
        public virtual String Catalog { get; set; }
        /// <summary>
        /// 键值
        /// </summary>
        [DbCol("键值", Type = DbColType.StringMax)]
        public virtual String Value { get; set; }
        /// <summary>
        /// 默认参考值
        /// </summary>
        [DbCol("默认参考值", Type = DbColType.StringMax)]
        public virtual String Refer { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Memo { get; set; }
    }
    /// <summary>
    /// 系统设置
    /// </summary>
    [Table("t_device_settings")]
    [DbCol("系统设置", Name = "t_device_settings")]
    public class TDeviceSettings
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Key]
        [DbCol("唯一标识", Len = 64, Key = DbIxType.PK)]
        public virtual String ID { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [DbCol("分类", Type = DbColType.Int32, Key = DbIxType.UIX, Index = nameof(Cate) + "|" + nameof(Key))]
        public virtual Int32 Cate { get; set; }
        /// <summary>
        /// 键名
        /// </summary>
        [DbCol("键名", Len = 255)]
        public virtual String Key { get; set; }
        /// <summary>
        /// 字符键值
        /// </summary>
        [DbCol("字符键值", Type = DbColType.StringMax)]
        public virtual String Value { get; set; } = "{}";
        /// <summary>
        /// 类型
        /// </summary>
        [DbCol("类型", Type = DbColType.StringNormal)]
        public virtual String Type { get; set; } = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Memo { get; set; } = string.Empty;
    }
    /// <summary>
    /// 终端字典表
    /// </summary>
    [Table("t_device_dicts")]
    [DbCol("终端字典表", Name = "t_device_dicts")]
    public class TDeviceDicts : ILocalVersion
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "标识")]
        [Column("id")]
        [DbCol("标识", Name = "id", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 分类编码
        /// </summary>
        [Display(Name = "分类编码")]
        [Column("code")]
        [DbCol("分类编码", Name = "code", Len = 64, Key = DbIxType.UIX, Index = nameof(Code) + "|" + nameof(Key))]
        public virtual String Code { get; set; } = String.Empty;
        /// <summary>
        /// 项值编码
        /// </summary>
        [Display(Name = "项值编码")]
        [Column("key")]
        [DbCol("项值编码", Name = "key", Len = 255)]
        public virtual String Key { get; set; } = String.Empty;
        /// <summary>
        /// 字典项名称
        /// </summary>
        [Display(Name = "字典项名称")]
        [Column("name")]
        [DbCol("字典项名称", Name = "name", Len = 255)]
        public virtual String Name { get; set; } = String.Empty;
        /// <summary>
        /// 字典键值
        /// </summary>
        [Display(Name = "字典键值")]
        [Column("value")]
        [DbCol("字典键值", Name = "value", Type = DbColType.Int64)]
        public virtual Int64 Value { get; set; }
        /// <summary>
        /// 扩展1
        /// </summary>
        [Display(Name = "扩展1")]
        [Column("ext1")]
        [DbCol("扩展1", Name = "ext1", Len = 255)]
        public virtual String Ext1 { get; set; } = String.Empty;
        /// <summary>
        /// 扩展2
        /// </summary>
        [Display(Name = "扩展2")]
        [Column("ext2")]
        [DbCol("扩展2", Name = "ext2", Len = 512)]
        public virtual String Ext2 { get; set; } = String.Empty;
        /// <summary>
        /// 扩展3
        /// </summary>
        [Display(Name = "扩展3")]
        [Column("ext3")]
        [DbCol("扩展3", Name = "ext3", Len = 1024)]
        public virtual String Ext3 { get; set; } = String.Empty;
        /// <summary>
        /// 字典排序
        /// </summary>
        [Display(Name = "字典排序")]
        [Column("order")]
        [DbCol("字典排序", Name = "order", Type = DbColType.Int32)]
        public virtual Int32 Order { get; set; }
        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "编辑人")]
        [Column("editor")]
        [DbCol("编辑人", Name = "editor", Len = 64)]
        public virtual String Editor { get; set; } = String.Empty;
        /// <summary>
        /// 编辑时间
        /// </summary>
        [Display(Name = "编辑时间")]
        [Column("etime")]
        [DbCol("编辑时间", Name = "etime", Type = DbColType.DateTime)]
        public virtual DateTime ETime { get; set; } = DateTime.Now;
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        [Column("status")]
        [DbCol("状态", Name = "status", Type = DbColType.Int32)]
        public virtual Int32 Status { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [Display(Name = "版本")]
        [Column("version")]
        [DbCol("版本", Name = "version", Type = DbColType.Int32)]
        public virtual Int32 Version { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [Column("remark")]
        [DbCol("备注", Name = "remark", Len = 255)]
        public virtual String Remark { get; set; } = String.Empty;
    }
    /// <summary>
    /// 终端字典模型转换接口
    /// </summary>
    public interface IDeviceDictModel
    {
        /// <summary>
        /// 获取代码分类
        /// </summary>
        /// <returns></returns>
        string GetCategory();
        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="dicts"></param>
        void LoadEntities(IEnumerable<TDeviceDicts> dicts);
        /// <summary>
        /// 获取保存内容
        /// </summary>
        /// <returns></returns>
        IEnumerable<TDeviceDicts> GetEntities();
    }
    #endregion
}
