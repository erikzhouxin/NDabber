using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SysSslProtocols = System.Security.Authentication.SslProtocols;
using HashAlgos = System.Data.Dubber.HashAlgorithms;

namespace System.Data.Dubber
{
    /// <summary>
    /// Interface for the FtpClient class.
    /// For detailed documentation of the methods, please see the FtpClient class or check the Wiki on the FluentFTP Github project.
    /// </summary>
    public interface IFtpClient : IDisposable
    {

        // PROPERTIES (From FtpClient_Properties)
        /// <summary>
        /// 已释放
        /// </summary>
        bool IsDisposed { get; }
        /// <summary>
        /// 协议版本
        /// </summary>
        FtpIpVersion InternetProtocolVersions { get; set; }
        /// <summary>
        /// 轮询间隔
        /// </summary>
        int SocketPollInterval { get; set; }
        /// <summary>
        /// 旧数据检查
        /// </summary>
        bool StaleDataCheck { get; set; }
        /// <summary>
        /// 已连接
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 开启线程安全数据连接
        /// </summary>
        bool EnableThreadSafeDataConnections { get; set; }
        /// <summary>
        /// 循环间隔
        /// </summary>
        int NoopInterval { get; set; }
        /// <summary>
        /// 检查功能
        /// </summary>
        bool CheckCapabilities { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// 网站
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// 认证
        /// </summary>
        NetworkCredential Credentials { get; set; }
        /// <summary>
        /// 最大废弃计数
        /// </summary>
        int MaximumDereferenceCount { get; set; }
        /// <summary>
        /// 客户认证
        /// </summary>
        X509CertificateCollection ClientCertificates { get; }
        /// <summary>
        /// 地址分解
        /// </summary>
        Func<string> AddressResolver { get; set; }
        /// <summary>
        /// 激活端口
        /// </summary>
        IEnumerable<int> ActivePorts { get; set; }
        /// <summary>
        /// 数据连接类型
        /// </summary>
        FtpDataConnectionType DataConnectionType { get; set; }
        /// <summary>
        /// 未允许断开
        /// </summary>
        bool UngracefullDisconnection { get; set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        int ConnectTimeout { get; set; }
        /// <summary>
        /// 读超时
        /// </summary>
        int ReadTimeout { get; set; }
        /// <summary>
        /// 数据连接连接超时
        /// </summary>
        int DataConnectionConnectTimeout { get; set; }
        /// <summary>
        /// 数据连接读取超时
        /// </summary>
        int DataConnectionReadTimeout { get; set; }
        /// <summary>
        /// 保持对话
        /// </summary>
        bool SocketKeepAlive { get; set; }
        /// <summary>
        /// 功能
        /// </summary>
        List<FtpCapability> Capabilities { get; }
        /// <summary>
        /// 加密方式
        /// </summary>
        FtpHashAlgorithm HashAlgorithms { get; }
        /// <summary>
        /// 解密模型
        /// </summary>
        FtpEncryptionMode EncryptionMode { get; set; }
        /// <summary>
        /// 数据连接加密
        /// </summary>
        bool DataConnectionEncryption { get; set; }
#if !NETFx
        /// <summary>
        /// 解释器加密
        /// </summary>
        bool PlainTextEncryption { get; set; }
#endif
        /// <summary>
        /// SSL协议
        /// </summary>
        SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// SSL缓存
        /// </summary>
        FtpsBuffering SslBuffering { get; set; }
        /// <summary>
        /// 验证证书
        /// </summary>
        event FtpSslValidation ValidateCertificate;
        /// <summary>
        /// 验证任意一个证书
        /// </summary>
        bool ValidateAnyCertificate { get; set; }
        /// <summary>
        /// 验证证书取消
        /// </summary>
        bool ValidateCertificateRevocation { get; set; }
        /// <summary>
        /// 系统类型
        /// </summary>
        string SystemType { get; }
        /// <summary>
        /// 服务类型
        /// </summary>
        FtpServer ServerType { get; }
        /// <summary>
        /// 服务事件
        /// </summary>
        FtpBaseServer ServerHandler { get; set; }
        /// <summary>
        /// 服务系统
        /// </summary>
        FtpOperatingSystem ServerOS { get; }
        /// <summary>
        /// 连接类型
        /// </summary>
        string ConnectionType { get; }
        /// <summary>
        /// 上一次回复
        /// </summary>
        FtpReply LastReply { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        FtpDataType ListingDataType { get; set; }
        /// <summary>
        /// 监听转换
        /// </summary>
        FtpParser ListingParser { get; set; }
        /// <summary>
        /// 列表语言
        /// </summary>
        CultureInfo ListingCulture { get; set; }
        /// <summary>
        /// 覆盖列表
        /// </summary>
        bool RecursiveList { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        double TimeZone { get; set; }
#if NETFx
        /// <summary>
        /// 本地时区
        /// </summary>
        double LocalTimeZone { get; set; }
#endif
        /// <summary>
        /// 事件转换
        /// </summary>
        FtpDate TimeConversion { get; set; }
        /// <summary>
        /// 批量列表
        /// </summary>
        bool BulkListing { get; set; }
        /// <summary>
        /// 批量长度
        /// </summary>
        int BulkListingLength { get; set; }
        /// <summary>
        /// 传输块长度
        /// </summary>
        int TransferChunkSize { get; set; }
        /// <summary>
        /// 本地文件缓存大小
        /// </summary>
        int LocalFileBufferSize { get; set; }
        /// <summary>
        /// 重试块
        /// </summary>
        int RetryAttempts { get; set; }
        /// <summary>
        /// 上传速率限制
        /// </summary>
        uint UploadRateLimit { get; set; }
        /// <summary>
        /// 下载速率限制
        /// </summary>
        uint DownloadRateLimit { get; set; }
        /// <summary>
        /// 下载0字节文件
        /// </summary>
        bool DownloadZeroByteFiles { get; set; }
        /// <summary>
        /// 上传数据类型
        /// </summary>
        FtpDataType UploadDataType { get; set; }
        /// <summary>
        /// 下载数据类型
        /// </summary>
        FtpDataType DownloadDataType { get; set; }
        /// <summary>
        /// 排除上传目录删除
        /// </summary>
        bool UploadDirectoryDeleteExcluded { get; set; }
        /// <summary>
        /// 排除下载目录删除
        /// </summary>
        bool DownloadDirectoryDeleteExcluded { get; set; }
        /// <summary>
        /// FXP数据类型
        /// </summary>
        FtpDataType FXPDataType { get; set; }
        /// <summary>
        /// FXP处理间隔
        /// </summary>
        int FXPProgressInterval { get; set; }
        /// <summary>
        /// 发送宿主
        /// </summary>
        bool SendHost { get; set; }
        /// <summary>
        /// 发送宿主域
        /// </summary>
        string SendHostDomain { get; set; }

        // METHODS
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        FtpReply Execute(string command);
        /// <summary>
        /// 获取回复
        /// </summary>
        /// <returns></returns>
        FtpReply GetReply();
        /// <summary>
        /// 连接
        /// </summary>
        void Connect();
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="profile"></param>
        void Connect(FtpProfile profile);
        /// <summary>
        /// 自动检测
        /// </summary>
        /// <param name="firstOnly"></param>
        /// <param name="cloneConnection"></param>
        /// <returns></returns>
        List<FtpProfile> AutoDetect(bool firstOnly = true, bool cloneConnection = true);
        /// <summary>
        /// 自动连接
        /// </summary>
        /// <returns></returns>
        FtpProfile AutoConnect();
        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 有失败
        /// </summary>
        /// <param name="cap"></param>
        /// <returns></returns>
        bool HasFeature(FtpCapability cap);
        /// <summary>
        /// 禁用UTF-8
        /// </summary>
        void DisableUTF8();

#if !NET40
        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpReply> ExecuteAsync(string command, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取回复
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpReply> GetReplyAsync(CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ConnectAsync(CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ConnectAsync(FtpProfile profile, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步自动检测
        /// </summary>
        /// <param name="firstOnly"></param>
        /// <param name="cloneConnection"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<FtpProfile>> AutoDetectAsync(bool firstOnly = true, bool cloneConnection = true, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步自动连接
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpProfile> AutoConnectAsync(CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken token = default(CancellationToken));
#endif

        // MANAGEMENT
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        void DeleteFile(string path);
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path"></param>
        void DeleteDirectory(string path);
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        void DeleteDirectory(string path, FtpListOption options);
        /// <summary>
        /// 目录存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool DirectoryExists(string path);
        /// <summary>
        /// 文件存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FileExists(string path);
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool CreateDirectory(string path);
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        bool CreateDirectory(string path, bool force);
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        void Rename(string path, string dest);
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        /// <param name="existsMode"></param>
        /// <returns></returns>
        bool MoveFile(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite);
        /// <summary>
        /// 移动目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        /// <param name="existsMode"></param>
        /// <returns></returns>
        bool MoveDirectory(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite);
        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        void SetFilePermissions(string path, int permissions);
        /// <summary>
        /// 修改文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        void Chmod(string path, int permissions);
        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="owner"></param>
        /// <param name="group"></param>
        /// <param name="other"></param>
        void SetFilePermissions(string path, FtpPermission owner, FtpPermission group, FtpPermission other);
        /// <summary>
        /// 修改文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="owner"></param>
        /// <param name="group"></param>
        /// <param name="other"></param>
        void Chmod(string path, FtpPermission owner, FtpPermission group, FtpPermission other);
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        FtpListItem GetFilePermissions(string path);
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        int GetChmod(string path);
        /// <summary>
        /// 废弃的链接
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        FtpListItem DereferenceLink(FtpListItem item);
        /// <summary>
        /// 废弃的链接
        /// </summary>
        /// <param name="item"></param>
        /// <param name="recMax"></param>
        /// <returns></returns>
        FtpListItem DereferenceLink(FtpListItem item, int recMax);
        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="path"></param>
        void SetWorkingDirectory(string path);
        /// <summary>
        /// 获取工作目录
        /// </summary>
        /// <returns></returns>
        string GetWorkingDirectory();
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        long GetFileSize(string path, long defaultValue = -1);
        /// <summary>
        /// 获取修改时间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        DateTime GetModifiedTime(string path);
        /// <summary>
        /// 设置修改时间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="date"></param>
        void SetModifiedTime(string path, DateTime date);
#if !NET40
        /// <summary>
        /// 异步删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteFileAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步删除目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteDirectoryAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步删除目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteDirectoryAsync(string path, FtpListOption options, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步目录存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> DirectoryExistsAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步文件存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> FileExistsAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步创建目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="force"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> CreateDirectoryAsync(string path, bool force, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步创建目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> CreateDirectoryAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步重命名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task RenameAsync(string path, string dest, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步移动文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        /// <param name="existsMode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> MoveFileAsync(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步移动文件目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dest"></param>
        /// <param name="existsMode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> MoveDirectoryAsync(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步设置文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetFilePermissionsAsync(string path, int permissions, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步修改文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ChmodAsync(string path, int permissions, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步设置文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="owner"></param>
        /// <param name="group"></param>
        /// <param name="other"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetFilePermissionsAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步修改文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="owner"></param>
        /// <param name="group"></param>
        /// <param name="other"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ChmodAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem> GetFilePermissionsAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取文件权限
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> GetChmodAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步废弃链接
        /// </summary>
        /// <param name="item"></param>
        /// <param name="recMax"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, int recMax, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步废弃链接
        /// </summary>
        /// <param name="item"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步设置工作目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetWorkingDirectoryAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取工作目录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetWorkingDirectoryAsync(CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取文件大小
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<long> GetFileSizeAsync(string path, long defaultValue = -1, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取修改时间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DateTime> GetModifiedTimeAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步设置修改时间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="date"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetModifiedTimeAsync(string path, DateTime date, CancellationToken token = default(CancellationToken));
#endif

        // LISTING
        /// <summary>
        /// 获取对象信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dateModified"></param>
        /// <returns></returns>
        FtpListItem GetObjectInfo(string path, bool dateModified = false);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        FtpListItem[] GetListing();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        FtpListItem[] GetListing(string path);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        FtpListItem[] GetListing(string path, FtpListOption options);
        /// <summary>
        /// 获取名称列表
        /// </summary>
        /// <returns></returns>
        string[] GetNameListing();
        /// <summary>
        /// 获取名称列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string[] GetNameListing(string path);

#if !NET40
        /// <summary>
        /// 异步获取对象信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dateModified"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem> GetObjectInfoAsync(string path, bool dateModified = false, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem[]> GetListingAsync(string path, FtpListOption options, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem[]> GetListingAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpListItem[]> GetListingAsync(CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取文件名列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string[]> GetNameListingAsync(string path, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步获取文件名列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string[]> GetNameListingAsync(CancellationToken token = default(CancellationToken));
#endif

#if NETFx
        /// <summary>
        /// 异步列表获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <param name="enumToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(string path, FtpListOption options, CancellationToken token = default(CancellationToken), CancellationToken enumToken = default(CancellationToken));
        /// <summary>
        /// 异步列表获取列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="enumToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(string path, CancellationToken token = default(CancellationToken), CancellationToken enumToken = default(CancellationToken));
        /// <summary>
        /// 异步列表获取列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(CancellationToken token = default(CancellationToken), CancellationToken enumToken = default(CancellationToken));
#endif

        // LOW LEVEL
        /// <summary>
        /// 打开读
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="restart"></param>
        /// <param name="checkIfFileExists"></param>
        /// <returns></returns>
        Stream OpenRead(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true);
        /// <summary>
        /// 打开读
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="restart"></param>
        /// <param name="fileLen"></param>
        /// <returns></returns>
        Stream OpenRead(string path, FtpDataType type, long restart, long fileLen);
        /// <summary>
        /// 打开写
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="checkIfFileExists"></param>
        /// <returns></returns>
        Stream OpenWrite(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true);
        /// <summary>
        /// 打开写
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fileLen"></param>
        /// <returns></returns>
        Stream OpenWrite(string path, FtpDataType type, long fileLen);
        /// <summary>
        /// 打开添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="checkIfFileExists"></param>
        /// <returns></returns>
        Stream OpenAppend(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true);
        /// <summary>
        /// 打开添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fileLen"></param>
        /// <returns></returns>
        Stream OpenAppend(string path, FtpDataType type, long fileLen);

#if !NET40
        /// <summary>
        /// 异步打开读
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="restart"></param>
        /// <param name="checkIfFileExists"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenReadAsync(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步打开读
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="restart"></param>
        /// <param name="fileLen"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenReadAsync(string path, FtpDataType type, long restart, long fileLen, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步打开写
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="checkIfFileExists"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenWriteAsync(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步打开写
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fileLen"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenWriteAsync(string path, FtpDataType type, long fileLen, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步打开添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="checkIfFileExists"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenAppendAsync(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步打开添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fileLen"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Stream> OpenAppendAsync(string path, FtpDataType type, long fileLen, CancellationToken token = default(CancellationToken));
#endif
        // HIGH LEVEL
        /// <summary>
        /// 上传多文件
        /// </summary>
        /// <param name="localPaths"></param>
        /// <param name="remoteDir"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="errorHandling"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        int UploadFiles(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null);
        /// <summary>
        /// 上传多文件
        /// </summary>
        /// <param name="localFiles"></param>
        /// <param name="remoteDir"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="errorHandling"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        int UploadFiles(IEnumerable<FileInfo> localFiles, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null);
        /// <summary>
        /// 下载多文件
        /// </summary>
        /// <param name="localDir"></param>
        /// <param name="remotePaths"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="errorHandling"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        int DownloadFiles(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        FtpStatus UploadFile(string localPath, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, FtpVerify verifyOptions = FtpVerify.None, Action<FtpProgress> progress = null);
        /// <summary>
        /// 上传流
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        FtpStatus Upload(Stream fileStream, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, Action<FtpProgress> progress = null);
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        FtpStatus Upload(byte[] fileData, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, Action<FtpProgress> progress = null);
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        FtpStatus DownloadFile(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, Action<FtpProgress> progress = null);
        /// <summary>
        /// 下载流
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="remotePath"></param>
        /// <param name="restartPosition"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        bool Download(Stream outStream, string remotePath, long restartPosition = 0, Action<FtpProgress> progress = null);
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="outBytes"></param>
        /// <param name="remotePath"></param>
        /// <param name="restartPosition"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        bool Download(out byte[] outBytes, string remotePath, long restartPosition = 0, Action<FtpProgress> progress = null);
        /// <summary>
        /// 下载目录
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="mode"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="rules"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        List<FtpResult> DownloadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpLocalExists existsMode = FtpLocalExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, Action<FtpProgress> progress = null);
        /// <summary>
        /// 上传目录
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="mode"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="rules"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        List<FtpResult> UploadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, Action<FtpProgress> progress = null);

#if !NET40
        /// <summary>
        /// 异步多文件上传
        /// </summary>
        /// <param name="localPaths"></param>
        /// <param name="remoteDir"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="errorHandling"></param>
        /// <param name="token"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<int> UploadFilesAsync(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default(CancellationToken), IProgress<FtpProgress> progress = null);
        /// <summary>
        /// 异步多文件下载
        /// </summary>
        /// <param name="localDir"></param>
        /// <param name="remotePaths"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="errorHandling"></param>
        /// <param name="token"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<int> DownloadFilesAsync(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default(CancellationToken), IProgress<FtpProgress> progress = null);
        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpStatus> UploadFileAsync(string localPath, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步上传流
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpStatus> UploadAsync(Stream fileStream, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步上传数据
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="createRemoteDir"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpStatus> UploadAsync(byte[] fileData, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpStatus> DownloadFileAsync(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步下载流
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="remotePath"></param>
        /// <param name="restartPosition"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> DownloadAsync(Stream outStream, string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步下载字节
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="restartPosition"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<byte[]> DownloadAsync(string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步下载文件目录
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="mode"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="rules"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<FtpResult>> DownloadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpLocalExists existsMode = FtpLocalExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 异步上传文件夹
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="mode"></param>
        /// <param name="existsMode"></param>
        /// <param name="verifyOptions"></param>
        /// <param name="rules"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<FtpResult>> UploadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken));
#endif

        // HASH
        /// <summary>
        /// 获取校验值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        FtpHash GetChecksum(string path, FtpHashAlgorithm algorithm = FtpHashAlgorithm.NONE);

#if !NET40
        /// <summary>
        /// 异步获取校验值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="algorithm"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<FtpHash> GetChecksumAsync(string path, FtpHashAlgorithm algorithm = FtpHashAlgorithm.NONE, CancellationToken token = default(CancellationToken));

#endif
    }
    /// <summary>
    /// 连接到单个FTP服务器。 与任何FTP/FTPS服务器交互，并提供一个高级和低级API来处理文件和文件夹。
    /// 启用日志记录后，调试FTP问题就容易得多了。 查看我们Github项目页面上的FAQ以获得更多信息。 
    /// </summary>
    public partial class FtpClient : IFtpClient, IDisposable
    {
        #region // FtpClient_AutoConnection.cs
        #region Auto Detect
        private static List<FtpEncryptionMode> autoConnectEncryption = new List<FtpEncryptionMode>
        {
            FtpEncryptionMode.Auto, FtpEncryptionMode.None, FtpEncryptionMode.Implicit,
        };

        private static List<SysSslProtocols> autoConnectProtocols = new List<SysSslProtocols>
        {
			//SysSslProtocols.None,
#if !NET40
			SysSslProtocols.Tls12 | SysSslProtocols.Tls11,
#else
            SysSslProtocols.Tls,
#endif
#if !NETFx
			SysSslProtocols.Default,
#endif
		};

        private static List<FtpDataConnectionType> autoConnectData = new List<FtpDataConnectionType>
        {
            FtpDataConnectionType.PASV,
            FtpDataConnectionType.EPSV,
            FtpDataConnectionType.PORT,
            FtpDataConnectionType.EPRT,
            FtpDataConnectionType.PASVEX
        };

        /// <summary>
        /// Automatic FTP and FTPS connection negotiation.
        /// This method tries every possible combination of the FTP connection properties, and returns the list of successful connection profiles.
        /// You can configure it to stop after finding the first successful profile, or to collect all successful profiles.
        /// You can then generate code for the profile using the FtpProfile.ToCode method.
        /// If no successful profiles are found, a blank list is returned.
        /// </summary>
        /// <param name="firstOnly">Find all successful profiles (false) or stop after finding the first successful profile (true)</param>
        /// <param name="cloneConnection">Use a new cloned FtpClient for testing connection profiles (true) or use the source FtpClient (false)</param>
        /// <returns></returns>
        public List<FtpProfile> AutoDetect(bool firstOnly = true, bool cloneConnection = true)
        {
            var results = new List<FtpProfile>();
            lock (m_lock)
            {
                LogFunc(nameof(AutoDetect), new object[] { firstOnly, cloneConnection });
                ValidateAutoDetect();

                // get known working connection profile based on the host (if any)
                var knownProfile = FtpServerSpecificHandler.GetWorkingProfileFromHost(Host, Port);
                if (knownProfile != null)
                {
                    results.Add(knownProfile);
                    return results;
                }

                var blacklistedEncryptions = new List<FtpEncryptionMode>();
                bool resetPort = m_port == 0;

                // clone this connection or use this connection
                var conn = cloneConnection ? CloneConnection() : this;

                // copy basic props if cloned connection
                if (cloneConnection)
                {
                    conn.Host = this.Host;
                    conn.Port = this.Port;
                    conn.Credentials = this.Credentials;
                }

                // disconnect if already connected
                if (conn.IsConnected)
                {
                    conn.Disconnect();
                }

                // try each encryption mode
                foreach (var encryption in autoConnectEncryption)
                {

                    // skip if FTPS was tried and failed
                    if (blacklistedEncryptions.Contains(encryption))
                    {
                        continue;
                    }

                    // try each SSL protocol
                    foreach (var protocol in autoConnectProtocols)
                    {

                        // skip plain protocols if testing secure FTPS -- disabled because 'None' is recommended by Microsoft
                        /*if (encryption != FtpEncryptionMode.None && protocol == SysSslProtocols.None) {
							continue;
						}*/

                        // skip secure protocols if testing plain FTP
                        if (encryption == FtpEncryptionMode.None && protocol != SysSslProtocols.None)
                        {
                            continue;
                        }

                        // reset port so it auto computes based on encryption type
                        if (resetPort)
                        {
                            conn.Port = 0;
                        }

                        // set rolled props
                        conn.EncryptionMode = encryption;
                        conn.SslProtocols = protocol;
                        conn.DataConnectionType = FtpDataConnectionType.AutoPassive;
                        conn.Encoding = Encoding.UTF8;

                        // try to connect
                        var connected = false;
                        var dataConn = FtpDataConnectionType.PASV;
                        try
                        {
                            conn.Connect();
                            connected = true;

                            // get data connection once connected
                            dataConn = AutoDataConnection(conn);

                            // if non-cloned connection, we want to remain connected if it works
                            if (cloneConnection)
                            {
                                conn.Disconnect();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is AuthenticationException)
                            {
                                throw new FtpInvalidCertificateException();
                            }
                            // since the connection failed, disconnect and retry
                            conn.Disconnect();

                            // if server does not support FTPS no point trying encryption again
                            if (IsFtpsFailure(blacklistedEncryptions, encryption, ex))
                            {
                                break;
                                // goto SkipEncryptionMode;
                            }

                            // catch error "no such host is known" and hard abort
                            if (IsPermanantConnectionFailure(ex))
                            {
                                if (cloneConnection)
                                {
                                    conn.Dispose();
                                }

                                // rethrow permanent failures so caller can be made aware of it
                                throw;
                            }
                        }

                        // if it worked
                        if (connected)
                        {

                            // if connected by explicit FTPS failed, no point trying encryption again
                            if (IsConnectedButFtpsFailure(blacklistedEncryptions, encryption, conn._ConnectionFTPSFailure))
                            {
                            }

                            results.Add(new FtpProfile
                            {
                                Host = Host,
                                Credentials = Credentials,
                                Encryption = blacklistedEncryptions.Contains(encryption) ? FtpEncryptionMode.None : encryption,
                                Protocols = protocol,
                                DataConnection = dataConn,
                                Encoding = Encoding.UTF8,
                                EncodingVerified = conn._ConnectionUTF8Success || conn.HasFeature(FtpCapability.UTF8)
                            });

                            // stop if only 1 wanted
                            if (firstOnly)
                            {
                                if (cloneConnection) { conn.Dispose(); }
                                return results;
                                // goto Exit;
                            }

                        }
                    }
                    // SkipEncryptionMode: var skip = true;
                }
                // Exit: if (cloneConnection) { conn.Dispose(); }
            }
            return results;
        }

#if !NET40
        /// <summary>
        /// Automatic FTP and FTPS connection negotiation.
        /// This method tries every possible combination of the FTP connection properties, and returns the list of successful connection profiles.
        /// You can configure it to stop after finding the first successful profile, or to collect all successful profiles.
        /// You can then generate code for the profile using the FtpProfile.ToCode method.
        /// If no successful profiles are found, a blank list is returned.
        /// </summary>
        /// <param name="firstOnly">Find all successful profiles (false) or stop after finding the first successful profile (true)</param>
        /// <param name="cloneConnection">Use a new cloned FtpClient for testing connection profiles (true) or use the source FtpClient (false)</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns></returns>
        public async Task<List<FtpProfile>> AutoDetectAsync(bool firstOnly, bool cloneConnection = true, CancellationToken token = default(CancellationToken))
        {
            var results = new List<FtpProfile>();

            LogFunc(nameof(AutoDetectAsync), new object[] { firstOnly, cloneConnection });
            ValidateAutoDetect();

            // get known working connection profile based on the host (if any)
            var knownProfile = FtpServerSpecificHandler.GetWorkingProfileFromHost(Host, Port);
            if (knownProfile != null)
            {
                results.Add(knownProfile);
                return results;
            }

            var blacklistedEncryptions = new List<FtpEncryptionMode>();
            bool resetPort = m_port == 0;

            // clone this connection or use this connection
            var conn = cloneConnection ? CloneConnection() : this;

            // copy basic props if cloned connection
            if (cloneConnection)
            {
                conn.Host = this.Host;
                conn.Port = this.Port;
                conn.Credentials = this.Credentials;
            }

            // disconnect if already connected
            if (conn.IsConnected)
            {
                await conn.DisconnectAsync(token);
            }

            // try each encryption mode
            foreach (var encryption in autoConnectEncryption)
            {

                // skip if FTPS was tried and failed
                if (blacklistedEncryptions.Contains(encryption))
                {
                    continue;
                }

                // try each SSL protocol
                foreach (var protocol in autoConnectProtocols)
                {

                    // skip plain protocols if testing secure FTPS -- disabled because 'None' is recommended by Microsoft
                    /*if (encryption != FtpEncryptionMode.None && protocol == SysSslProtocols.None) {
						continue;
					}*/

                    // skip secure protocols if testing plain FTP
                    if (encryption == FtpEncryptionMode.None && protocol != SysSslProtocols.None)
                    {
                        continue;
                    }

                    // reset port so it auto computes based on encryption type
                    if (resetPort)
                    {
                        conn.Port = 0;
                    }

                    // set rolled props
                    conn.EncryptionMode = encryption;
                    conn.SslProtocols = protocol;
                    conn.DataConnectionType = FtpDataConnectionType.AutoPassive;
                    conn.Encoding = Encoding.UTF8;

                    // try to connect
                    var connected = false;
                    var dataConn = FtpDataConnectionType.PASV;
                    try
                    {
                        await conn.ConnectAsync(token);
                        connected = true;

                        // get data connection once connected
                        dataConn = AutoDataConnection(conn);

                        // if non-cloned connection, we want to remain connected if it works
                        if (cloneConnection)
                        {
                            await conn.DisconnectAsync(token);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is AuthenticationException)
                        {
                            throw new FtpInvalidCertificateException();
                        }
                        // since the connection failed, disconnect and retry
                        await conn.DisconnectAsync(token);

                        // if server does not support FTPS no point trying encryption again
                        if (IsFtpsFailure(blacklistedEncryptions, encryption, ex))
                        {
                            break;//goto SkipEncryptionMode;
                        }

                        // catch error "no such host is known" and hard abort
                        if (IsPermanantConnectionFailure(ex))
                        {
                            if (cloneConnection)
                            {
                                conn.Dispose();
                            }

                            // rethrow permanent failures so caller can be made aware of it
                            throw;
                        }
                    }

                    // if it worked, add the profile
                    if (connected)
                    {

                        // if connected by explicit FTPS failed, no point trying encryption again
                        if (IsConnectedButFtpsFailure(blacklistedEncryptions, encryption, conn._ConnectionFTPSFailure))
                        {
                        }

                        results.Add(new FtpProfile
                        {
                            Host = Host,
                            Credentials = Credentials,
                            Encryption = encryption,
                            Protocols = protocol,
                            DataConnection = dataConn,
                            Encoding = Encoding.UTF8,
                            EncodingVerified = conn._ConnectionUTF8Success || conn.HasFeature(FtpCapability.UTF8)
                        });

                        // stop if only 1 wanted
                        if (firstOnly)
                        {
                            if (cloneConnection) { conn.Dispose(); }
                            return results;
                            // goto Exit;
                        }
                    }
                }
                // SkipEncryptionMode: var skip = true;
            }
            // Exit:
            if (cloneConnection) { conn.Dispose(); }
            return results;
        }
#endif

        private void ValidateAutoDetect()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("This FtpClient object has been disposed. It is no longer accessible.");
            }

            if (Host == null)
            {
                throw new FtpException("No host has been specified. Please set the 'Host' property before trying to auto connect.");
            }

            if (Credentials == null)
            {
                throw new FtpException("No username and password has been specified. Please set the 'Credentials' property before trying to auto connect.");
            }
        }

        private static bool IsFtpsFailure(List<FtpEncryptionMode> blacklistedEncryptions, FtpEncryptionMode encryption, Exception ex)
        {

            // catch error starting explicit FTPS and don't try any more secure connections
            if (encryption == FtpEncryptionMode.Auto || encryption == FtpEncryptionMode.Explicit)
            {
                if (ex is FtpSecurityNotAvailableException)
                {

                    // ban explicit FTPS
                    blacklistedEncryptions.Add(encryption);
                    return true;
                }
            }

            // catch error starting implicit FTPS and don't try any more secure connections
            if (encryption == FtpEncryptionMode.Implicit)
            {
                if ((ex is SocketException && (ex as SocketException).SocketErrorCode == SocketError.ConnectionRefused)
                    || ex is TimeoutException)
                {

                    // ban implicit FTPS
                    blacklistedEncryptions.Add(encryption);
                    return true;
                }
            }

            return false;
        }

        private static bool IsConnectedButFtpsFailure(List<FtpEncryptionMode> blacklistedEncryptions, FtpEncryptionMode encryption, bool failedFTPS)
        {

            // catch error starting explicit FTPS and don't try any more secure connections
            if (failedFTPS)
            {
                if (encryption == FtpEncryptionMode.Auto || encryption == FtpEncryptionMode.Explicit)
                {

                    // ban explicit FTPS
                    blacklistedEncryptions.Add(encryption);
                    return true;
                }
            }

            return false;
        }

        private static bool IsPermanantConnectionFailure(Exception ex)
        {

            // catch error "no such host is known" and hard abort
            if (ex is SocketException && ((SocketException)ex).SocketErrorCode == SocketError.HostNotFound)
            {
                return true;
            }

            // catch error "timed out trying to connect" and hard abort
            if (ex is TimeoutException)
            {
                return true;
            }

            // catch authentication error and hard abort (see issue #697)
            if (ex is FtpAuthenticationException)
            {

                // only catch auth error if the credentials have been rejected by the server
                // because the error is also thrown if connection drops due to TLS or EncryptionMode
                // (see issue #700 for more details)
                var authError = ex as FtpAuthenticationException;
                if (authError.CompletionCode != null && authError.CompletionCode.StartsWith("530"))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Auto Data Connection

        private FtpDataConnectionType AutoDataConnection(FtpClient conn)
        {

            // check socket protocol version
            if (conn.m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
            {

                // IPV4
                return FtpDataConnectionType.PASV;

            }
            else
            {

                // IPV6
                // always use enhanced passive (enhanced PORT is not recommended and no other types support IPV6)
                return FtpDataConnectionType.EPSV;
            }
        }

        #endregion

        #region Auto Connect

        /// <summary>
        /// Connect to the given server profile.
        /// </summary>
        public void Connect(FtpProfile profile)
        {

            // copy over the profile properties to this instance
            LoadProfile(profile);

            // begin connection
            Connect();
        }

#if !NET40
        /// <summary>
        /// Connect to the given server profile.
        /// </summary>
        public async Task ConnectAsync(FtpProfile profile, CancellationToken token = default(CancellationToken))
        {

            // copy over the profile properties to this instance
            LoadProfile(profile);

            // begin connection
            await ConnectAsync(token);
        }
#endif

        /// <summary>
        /// Load the given connection profile and configure the FTP client instance accordingly.
        /// </summary>
        /// <param name="profile">Connection profile. Not modified.</param>
        public void LoadProfile(FtpProfile profile)
        {

            // verify args
            if (profile == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "profile");
            }
            if (profile.Host.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "profile.Host");
            }
            if (profile.Credentials == null)
            {
                throw new ArgumentException("Required parameter is null.", "profile.Credentials");
            }
            if (profile.Encoding == null)
            {
                throw new ArgumentException("Required parameter is null.", "profile.Encoding");
            }

            // copy over the profile properties to this instance
            Host = profile.Host;
            Credentials = profile.Credentials;
            EncryptionMode = profile.Encryption;
            SslProtocols = profile.Protocols;
            DataConnectionType = profile.DataConnection;
            Encoding = profile.Encoding;
            if (profile.Timeout != 0)
            {
                ConnectTimeout = profile.Timeout;
                ReadTimeout = profile.Timeout;
                DataConnectionConnectTimeout = profile.Timeout;
                DataConnectionReadTimeout = profile.Timeout;
            }
            if (SocketPollInterval != 0)
            {
                SocketPollInterval = profile.SocketPollInterval;
            }
            if (RetryAttempts != 0)
            {
                RetryAttempts = profile.RetryAttempts;
            }
        }

        /// <summary>
        /// Automatic FTP and FTPS connection negotiation.
        /// This method tries every possible combination of the FTP connection properties, and connects to the first successful profile.
        /// Returns the FtpProfile if the connection succeeded, or null if it failed.
        /// It will throw exceptions for permanent failures like invalid host or invalid credentials.
        /// </summary>
        public FtpProfile AutoConnect()
        {
            LogFunc(nameof(AutoConnect));

            // connect to the first available connection profile
            var results = AutoDetect(true, false);
            if (results.Count > 0)
            {
                var profile = results[0];

                // load the profile so final property selections are
                // loaded into the current connection
                LoadProfile(profile);

                // if we are using SSL, set a basic server acceptance function
                SetDefaultCertificateValidation(profile);

                // return the working profile
                return profile;
            }

            return null;
        }

#if !NET40
        /// <summary>
        /// Automatic FTP and FTPS connection negotiation.
        /// This method tries every possible combination of the FTP connection properties, and connects to the first successful profile.
        /// Returns the FtpProfile if the connection succeeded, or null if it failed.
        /// It will throw exceptions for permanent failures like invalid host or invalid credentials.
        /// </summary>
        public async Task<FtpProfile> AutoConnectAsync(CancellationToken token = default(CancellationToken))
        {
            LogFunc(nameof(AutoConnectAsync));

            // connect to the first available connection profile
            var results = await AutoDetectAsync(true, false, token);
            if (results.Count > 0)
            {
                var profile = results[0];

                // load the profile so final property selections are
                // loaded into the current connection
                LoadProfile(profile);

                // if we are using SSL, set a basic server acceptance function
                SetDefaultCertificateValidation(profile);

                // return the working profile
                return profile;
            }

            return null;
        }
#endif
        private void SetDefaultCertificateValidation(FtpProfile profile)
        {
            if (profile.Encryption != FtpEncryptionMode.None)
            {
                ValidateCertificate += new FtpSslValidation(delegate (FtpClient c, FtpSslValidationEventArgs e)
                {
                    if (e.PolicyErrors != System.Net.Security.SslPolicyErrors.None)
                    {
                        e.Accept = false;
                    }
                    else
                    {
                        e.Accept = true;
                    }
                });
            }
        }

        #endregion
        #endregion // FtpClient_AutoConnection.cs
        #region // FtpClient_Connection.cs
        #region Constructor / Destructor

        /// <summary>
        /// Creates a new instance of an FTP Client.
        /// </summary>
        public FtpClient()
        {
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host.
        /// </summary>
        public FtpClient(string host)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host), "Host must be provided");
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host and credentials.
        /// </summary>
        public FtpClient(string host, NetworkCredential credentials)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host), "Host must be provided");
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials), "Credentials must be provided");
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, port and credentials.
        /// </summary>
        public FtpClient(string host, int port, NetworkCredential credentials)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host), "Host must be provided");
            Port = port;
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials), "Credentials must be provided");
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, username and password.
        /// </summary>
        public FtpClient(string host, string user, string pass)
        {
            Host = host;
            Credentials = new NetworkCredential(user, pass);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, username, password and account
        /// </summary>
        public FtpClient(string host, string user, string pass, string account)
        {
            Host = host;
            Credentials = new NetworkCredential(user, pass, account);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, port, username and password.
        /// </summary>
        public FtpClient(string host, int port, string user, string pass)
        {
            Host = host;
            Port = port;
            Credentials = new NetworkCredential(user, pass);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, port, username, password and account
        /// </summary>
        public FtpClient(string host, int port, string user, string pass, string account)
        {
            Host = host;
            Port = port;
            Credentials = new NetworkCredential(user, pass, account);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host.
        /// </summary>
        public FtpClient(Uri host)
        {
            Host = ValidateHost(host);
            Port = host.Port;
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host and credentials.
        /// </summary>
        public FtpClient(Uri host, NetworkCredential credentials)
        {
            Host = ValidateHost(host);
            Port = host.Port;
            Credentials = credentials;
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host and credentials.
        /// </summary>
        public FtpClient(Uri host, string user, string pass)
        {
            Host = ValidateHost(host);
            Port = host.Port;
            Credentials = new NetworkCredential(user, pass);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host and credentials.
        /// </summary>
        public FtpClient(Uri host, string user, string pass, string account)
        {
            Host = ValidateHost(host);
            Port = host.Port;
            Credentials = new NetworkCredential(user, pass, account);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, port and credentials.
        /// </summary>
        public FtpClient(Uri host, int port, string user, string pass)
        {
            Host = ValidateHost(host);
            Port = port;
            Credentials = new NetworkCredential(user, pass);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Creates a new instance of an FTP Client, with the given host, port and credentials.
        /// </summary>
        public FtpClient(Uri host, int port, string user, string pass, string account)
        {
            Host = ValidateHost(host);
            Port = port;
            Credentials = new NetworkCredential(user, pass, account);
            m_listParser = new FtpListParser(this);
        }

        /// <summary>
        /// Check if the host parameter is valid
        /// </summary>
        /// <param name="host"></param>
        private static string ValidateHost(Uri host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host), "Host is required");
            }
#if !NETFx
            if (host.Scheme != Uri.UriSchemeFtp)
            {
                throw new ArgumentException("Host is not a valid FTP path");
            }
#endif
            return host.Host;
        }

        /// <summary>
        /// Creates a new instance of this class. Useful in FTP proxy classes.
        /// </summary>
        /// <returns></returns>
        protected virtual FtpClient Create()
        {
            return new FtpClient();
        }

        /// <summary>
        /// Disconnects from the server, releases resources held by this
        /// object.
        /// </summary>
        public virtual void Dispose()
        {
            lock (m_lock)
            {
                if (IsDisposed)
                {
                    return;
                }

                // Fix: Hard catch and suppress all exceptions during disposing as there are constant issues with this method
                try
                {
                    LogFunc(nameof(Dispose));
                    LogStatus(FtpTraceLevel.Verbose, "Disposing FtpClient object...");
                }
                catch (Exception) { }

                try
                {
                    if (IsConnected)
                    {
                        Disconnect();
                    }
                }
                catch (Exception) { }

                if (m_stream != null)
                {
                    try
                    {
                        m_stream.Dispose();
                    }
                    catch (Exception) { }

                    m_stream = null;
                }

                try
                {
                    m_credentials = null;
                    m_textEncoding = null;
                    m_host = null;
                    m_asyncmethods.Clear();
                }
                catch (Exception) { }

                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~FtpClient()
        {
            Dispose();
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clones the control connection for opening multiple data streams
        /// </summary>
        /// <returns>A new control connection with the same property settings as this one</returns>
        protected FtpClient CloneConnection()
        {
            var conn = Create();

            conn.m_isClone = true;

            // configure new connection as clone of self
            conn.InternetProtocolVersions = InternetProtocolVersions;
            conn.SocketPollInterval = SocketPollInterval;
            conn.StaleDataCheck = StaleDataCheck;
            conn.EnableThreadSafeDataConnections = EnableThreadSafeDataConnections;
            conn.NoopInterval = NoopInterval;
            conn.Encoding = Encoding;
            conn.Host = Host;
            conn.Port = Port;
            conn.Credentials = Credentials;
            conn.MaximumDereferenceCount = MaximumDereferenceCount;
            conn.ClientCertificates = ClientCertificates;
            conn.DataConnectionType = DataConnectionType;
            conn.UngracefullDisconnection = UngracefullDisconnection;
            conn.ConnectTimeout = ConnectTimeout;
            conn.ReadTimeout = ReadTimeout;
            conn.DataConnectionConnectTimeout = DataConnectionConnectTimeout;
            conn.DataConnectionReadTimeout = DataConnectionReadTimeout;
            conn.SocketKeepAlive = SocketKeepAlive;
            conn.m_capabilities = m_capabilities;
            conn.EncryptionMode = EncryptionMode;
            conn.DataConnectionEncryption = DataConnectionEncryption;
            conn.SslProtocols = SslProtocols;
            conn.SslBuffering = SslBuffering;
            conn.TransferChunkSize = TransferChunkSize;
            conn.LocalFileBufferSize = LocalFileBufferSize;
            conn.ListingDataType = ListingDataType;
            conn.ListingParser = ListingParser;
            conn.ListingCulture = ListingCulture;
            conn.ListingCustomParser = ListingCustomParser;
            conn.TimeZone = TimeZone;
            conn.TimeConversion = TimeConversion;
            conn.RetryAttempts = RetryAttempts;
            conn.UploadRateLimit = UploadRateLimit;
            conn.DownloadZeroByteFiles = DownloadZeroByteFiles;
            conn.DownloadRateLimit = DownloadRateLimit;
            conn.DownloadDataType = DownloadDataType;
            conn.UploadDataType = UploadDataType;
            conn.ActivePorts = ActivePorts;
            conn.PassiveBlockedPorts = PassiveBlockedPorts;
            conn.PassiveMaxAttempts = PassiveMaxAttempts;
            conn.SendHost = SendHost;
            conn.SendHostDomain = SendHostDomain;
            conn.FXPDataType = FXPDataType;
            conn.FXPProgressInterval = FXPProgressInterval;
            conn.ServerHandler = ServerHandler;
            conn.UploadDirectoryDeleteExcluded = UploadDirectoryDeleteExcluded;
            conn.DownloadDirectoryDeleteExcluded = DownloadDirectoryDeleteExcluded;

            // configure new connection as clone of self (newer version .NET only)
#if !NET40
            conn.SocketLocalIp = SocketLocalIp;
#endif

            // configure new connection as clone of self (.NET core props only)
#if NETFx
            conn.LocalTimeZone = LocalTimeZone;
#endif

            // fix for #428: OpenRead with EnableThreadSafeDataConnections always uses ASCII
            conn.CurrentDataType = CurrentDataType;
            conn.ForceSetDataType = true;

            // configure new connection as clone of self (.NET framework props only)
#if !NETFx
            conn.PlainTextEncryption = PlainTextEncryption;
#endif

            // always accept certificate no matter what because if code execution ever
            // gets here it means the certificate on the control connection object being
            // cloned was already accepted.
            conn.ValidateCertificate += new FtpSslValidation(
                delegate (FtpClient obj, FtpSslValidationEventArgs e) { e.Accept = true; });

            return conn;
        }

        #endregion

        #region Connect

        private FtpListParser m_listParser;

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if this object has been disposed.</exception>
        public virtual void Connect()
        {
            FtpReply reply;
            lock (m_lock)
            {

                LogFunc(nameof(Connect));

                if (IsDisposed)
                {
                    throw new ObjectDisposedException("This FtpClient object has been disposed. It is no longer accessible.");
                }

                if (m_stream == null)
                {
                    m_stream = new FtpSocketStream(this);
                    m_stream.ValidateCertificate += new FtpSocketStreamSslValidation(FireValidateCertficate);
                }
                else
                {
                    if (IsConnected)
                    {
                        Disconnect();
                    }
                }

                if (Host == null)
                {
                    throw new FtpException("No host has been specified");
                }

                if (m_capabilities == null)
                {
                    m_capabilities = new List<FtpCapability>();
                }

                ResetStateFlags();

                m_hashAlgorithms = FtpHashAlgorithm.NONE;
                m_stream.ConnectTimeout = m_connectTimeout;
                m_stream.SocketPollInterval = m_socketPollInterval;
                Connect(m_stream);

                m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
                if (EncryptionMode == FtpEncryptionMode.Implicit)
                {
                    m_stream.ActivateEncryption(Host, m_clientCerts.Count > 0 ? m_clientCerts : null, m_SslProtocols);
                }
                Handshake();
                m_serverType = FtpServerSpecificHandler.DetectFtpServer(this, HandshakeReply);

                if (SendHost)
                {
                    if (!(reply = Execute("HOST " + (SendHostDomain != null ? SendHostDomain : Host))).Success)
                    {
                        throw new FtpException("HOST command failed.");
                    }
                }
                // try to upgrade this connection to SSL if supported by the server
                if (EncryptionMode == FtpEncryptionMode.Explicit || EncryptionMode == FtpEncryptionMode.Auto)
                {
                    reply = Execute("AUTH TLS");
                    if (!reply.Success)
                    {
                        _ConnectionFTPSFailure = true;
                        if (EncryptionMode == FtpEncryptionMode.Explicit)
                        {
                            throw new FtpSecurityNotAvailableException("AUTH TLS command failed.");
                        }
                    }
                    else if (reply.Success)
                    {
                        m_stream.ActivateEncryption(Host, m_clientCerts.Count > 0 ? m_clientCerts : null, m_SslProtocols);
                    }
                }
                if (m_credentials != null)
                {
                    Authenticate();
                }

                // configure the default FTPS settings
                if (IsEncrypted && DataConnectionEncryption)
                {
                    if (!(reply = Execute("PBSZ 0")).Success)
                    {
                        throw new FtpCommandException(reply);
                    }

                    if (!(reply = Execute("PROT P")).Success)
                    {
                        throw new FtpCommandException(reply);
                    }
                }

                // if this is a clone these values should have already been loaded
                // so save some bandwidth and CPU time and skip executing this again.
                // otherwise clear the capabilities in case connection is reused to 
                // a different server 
                if (!m_isClone && m_checkCapabilities)
                {
                    m_capabilities.Clear();
                }
                bool assumeCaps = false;
                if (m_capabilities.IsBlank() && m_checkCapabilities)
                {
                    if ((reply = Execute("FEAT")).Success && reply.InfoMessages != null)
                    {
                        GetFeatures(reply);
                    }
                    else
                    {
                        assumeCaps = true;
                    }
                }

                // Enable UTF8 if the encoding is ASCII and UTF8 is supported
                if (m_textEncodingAutoUTF && m_textEncoding == Encoding.ASCII && HasFeature(FtpCapability.UTF8))
                {
                    m_textEncoding = Encoding.UTF8;
                }

                LogStatus(FtpTraceLevel.Info, "Text encoding: " + m_textEncoding.ToString());

                if (m_textEncoding == Encoding.UTF8)
                {
                    // If the server supports UTF8 it should already be enabled and this
                    // command should not matter however there are conflicting drafts
                    // about this so we'll just execute it to be safe. 
                    if ((reply = Execute("OPTS UTF8 ON")).Success)
                    {
                        _ConnectionUTF8Success = true;
                    }
                }

                // Get the system type - Needed to auto-detect file listing parser
                if ((reply = Execute("SYST")).Success)
                {
                    m_systemType = reply.Message;
                    m_serverType = FtpServerSpecificHandler.DetectFtpServerBySyst(this);
                    m_serverOS = FtpServerSpecificHandler.DetectFtpOSBySyst(this);
                }

                // Set a FTP server handler if a custom handler has not already been set
                if (ServerHandler == null)
                {
                    ServerHandler = FtpServerSpecificHandler.GetServerHandler(m_serverType);
                }

                // Assume the system's capabilities if FEAT command not supported by the server
                if (assumeCaps)
                {
                    FtpServerSpecificHandler.AssumeCapabilities(this, ServerHandler, m_capabilities, ref m_hashAlgorithms);
                }

#if !NETFx
                if (IsEncrypted && PlainTextEncryption)
                {
                    if (!(reply = Execute("CCC")).Success)
                    {
                        throw new FtpSecurityNotAvailableException("Failed to disable encryption with CCC command. Perhaps your server does not support it or is not configured to allow it.");
                    }
                    else
                    {
                        // close the SslStream and send close_notify command to server
                        m_stream.DeactivateEncryption();

                        // read stale data (server's reply?)
                        ReadStaleData(false, true, false);
                    }
                }
#endif

                // Unless a custom list parser has been set,
                // Detect the listing parser and prefer machine listings over any other type
                // FIX : #739 prefer using machine listings to fix issues with GetListing and DeleteDirectory
                if (ListingParser != FtpParser.Custom)
                {
                    ListingParser = ServerHandler != null ? ServerHandler.GetParser() : FtpParser.Auto;
                    if (HasFeature(FtpCapability.MLSD))
                    {
                        ListingParser = FtpParser.Machine;
                    }
                }

                // Create the parser even if the auto-OS detection failed
                m_listParser.Init(m_serverOS, ListingParser);

                // FIX : #318 always set the type when we create a new connection
                ForceSetDataType = true;

                // Execute server-specific post-connection event
                if (ServerHandler != null)
                {
                    ServerHandler.AfterConnected(this);
                }
            }
        }

#if !NET40
        // TODO: add example
        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if this object has been disposed.</exception>
        public virtual async Task ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            LogFunc(nameof(ConnectAsync));

            if (IsDisposed)
            {
                throw new ObjectDisposedException("This FtpClient object has been disposed. It is no longer accessible.");
            }

            if (m_stream == null)
            {
                m_stream = new FtpSocketStream(this);
                m_stream.ValidateCertificate += new FtpSocketStreamSslValidation(FireValidateCertficate);
            }
            else
            {
                if (IsConnected)
                {
                    Disconnect();
                }
            }

            if (Host == null)
            {
                throw new FtpException("No host has been specified");
            }

            if (m_capabilities == null)
            {
                m_capabilities = new List<FtpCapability>();
            }

            ResetStateFlags();

            m_hashAlgorithms = FtpHashAlgorithm.NONE;
            m_stream.ConnectTimeout = m_connectTimeout;
            m_stream.SocketPollInterval = m_socketPollInterval;
            await ConnectAsync(m_stream, token);

            m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);

            if (EncryptionMode == FtpEncryptionMode.Implicit)
            {
                await m_stream.ActivateEncryptionAsync(Host, m_clientCerts.Count > 0 ? m_clientCerts : null, m_SslProtocols);
            }
            await HandshakeAsync(token);
            m_serverType = FtpServerSpecificHandler.DetectFtpServer(this, HandshakeReply);

            if (SendHost)
            {
                if (!(reply = await ExecuteAsync("HOST " + (SendHostDomain != null ? SendHostDomain : Host), token)).Success)
                {
                    throw new FtpException("HOST command failed.");
                }
            }
            // try to upgrade this connection to SSL if supported by the server
            if (EncryptionMode == FtpEncryptionMode.Explicit || EncryptionMode == FtpEncryptionMode.Auto)
            {
                reply = await ExecuteAsync("AUTH TLS", token);
                if (!reply.Success)
                {
                    _ConnectionFTPSFailure = true;
                    if (EncryptionMode == FtpEncryptionMode.Explicit)
                    {
                        throw new FtpSecurityNotAvailableException("AUTH TLS command failed.");
                    }
                }
                else if (reply.Success)
                {
                    await m_stream.ActivateEncryptionAsync(Host, m_clientCerts.Count > 0 ? m_clientCerts : null, m_SslProtocols);
                }
            }
            if (m_credentials != null)
            {
                await AuthenticateAsync(token);
            }

            // configure the default FTPS settings
            if (IsEncrypted && DataConnectionEncryption)
            {
                if (!(reply = await ExecuteAsync("PBSZ 0", token)).Success)
                {
                    throw new FtpCommandException(reply);
                }

                if (!(reply = await ExecuteAsync("PROT P", token)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }

            // if this is a clone these values should have already been loaded
            // so save some bandwidth and CPU time and skip executing this again.
            // otherwise clear the capabilities in case connection is reused to 
            // a different server 
            if (!m_isClone && m_checkCapabilities)
            {
                m_capabilities.Clear();
            }
            bool assumeCaps = false;
            if (m_capabilities.IsBlank() && m_checkCapabilities)
            {
                if ((reply = await ExecuteAsync("FEAT", token)).Success && reply.InfoMessages != null)
                {
                    GetFeatures(reply);
                }
                else
                {
                    assumeCaps = true;
                }
            }

            // Enable UTF8 if the encoding is ASCII and UTF8 is supported
            if (m_textEncodingAutoUTF && m_textEncoding == Encoding.ASCII && HasFeature(FtpCapability.UTF8))
            {
                m_textEncoding = Encoding.UTF8;
            }

            LogStatus(FtpTraceLevel.Info, "Text encoding: " + m_textEncoding.ToString());

            if (m_textEncoding == Encoding.UTF8)
            {
                // If the server supports UTF8 it should already be enabled and this
                // command should not matter however there are conflicting drafts
                // about this so we'll just execute it to be safe. 
                if ((reply = await ExecuteAsync("OPTS UTF8 ON", token)).Success)
                {
                    _ConnectionUTF8Success = true;
                }
            }

            // Get the system type - Needed to auto-detect file listing parser
            if ((reply = await ExecuteAsync("SYST", token)).Success)
            {
                m_systemType = reply.Message;
                m_serverType = FtpServerSpecificHandler.DetectFtpServerBySyst(this);
                m_serverOS = FtpServerSpecificHandler.DetectFtpOSBySyst(this);
            }

            // Set a FTP server handler if a custom handler has not already been set
            if (ServerHandler == null)
            {
                ServerHandler = FtpServerSpecificHandler.GetServerHandler(m_serverType);
            }
            // Assume the system's capabilities if FEAT command not supported by the server
            if (assumeCaps)
            {
                FtpServerSpecificHandler.AssumeCapabilities(this, ServerHandler, m_capabilities, ref m_hashAlgorithms);
            }

#if !NETFx
            if (IsEncrypted && PlainTextEncryption)
            {
                if (!(reply = await ExecuteAsync("CCC", token)).Success)
                {
                    throw new FtpSecurityNotAvailableException("Failed to disable encryption with CCC command. Perhaps your server does not support it or is not configured to allow it.");
                }
                else
                {
                    // close the SslStream and send close_notify command to server
                    m_stream.DeactivateEncryption();

                    // read stale data (server's reply?)
                    await ReadStaleDataAsync(false, true, false, token);
                }
            }
#endif

            // Unless a custom list parser has been set,
            // Detect the listing parser and prefer machine listings over any other type
            // FIX : #739 prefer using machine listings to fix issues with GetListing and DeleteDirectory
            if (ListingParser != FtpParser.Custom)
            {
                ListingParser = ServerHandler != null ? ServerHandler.GetParser() : FtpParser.Auto;
                if (HasFeature(FtpCapability.MLSD))
                {
                    ListingParser = FtpParser.Machine;
                }
            }

            // Create the parser even if the auto-OS detection failed
            m_listParser.Init(m_serverOS, ListingParser);

            // FIX : #318 always set the type when we create a new connection
            ForceSetDataType = true;

            // Execute server-specific post-connection event
            if (ServerHandler != null)
            {
                await ServerHandler.AfterConnectedAsync(this, token);
            }
        }

#endif

        /// <summary>
        /// Connect to the FTP server. Overridden in proxy classes.
        /// </summary>
        /// <param name="stream"></param>
        protected virtual void Connect(FtpSocketStream stream)
        {
            stream.Connect(Host, Port, InternetProtocolVersions);
        }

#if !NET40
        /// <summary>
        /// Connect to the FTP server. Overridden in proxy classes.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="token"></param>
        protected virtual async Task ConnectAsync(FtpSocketStream stream, CancellationToken token)
        {
            await stream.ConnectAsync(Host, Port, InternetProtocolVersions, token);
        }
#endif

        /// <summary>
        /// Connect to the FTP server. Overridden in proxy classes.
        /// </summary>
        protected virtual void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
        {
            stream.Connect(host, port, ipVersions);
        }

#if !NET40
        /// <summary>
        /// Connect to the FTP server. Overridden in proxy classes.
        /// </summary>
        protected virtual Task ConnectAsync(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions, CancellationToken token)
        {
            return stream.ConnectAsync(host, port, ipVersions, token);
        }
#endif
        /// <summary>
        /// 握手回复
        /// </summary>
        protected FtpReply HandshakeReply;

        /// <summary>
        /// Called during Connect(). Typically extended by FTP proxies.
        /// </summary>
        protected virtual void Handshake()
        {
            FtpReply reply;
            if (!(reply = GetReply()).Success)
            {
                if (reply.Code == null)
                {
                    throw new IOException("The connection was terminated before a greeting could be read.");
                }
                else
                {
                    throw new FtpCommandException(reply);
                }
            }

            HandshakeReply = reply;
        }

#if !NET40
        /// <summary>
        /// Called during <see cref="ConnectAsync(CancellationToken)"/>. Typically extended by FTP proxies.
        /// </summary>
        protected virtual async Task HandshakeAsync(CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;
            if (!(reply = await GetReplyAsync(token)).Success)
            {
                if (reply.Code == null)
                {
                    throw new IOException("The connection was terminated before a greeting could be read.");
                }
                else
                {
                    throw new FtpCommandException(reply);
                }
            }

            HandshakeReply = reply;
        }
#endif

        /// <summary>
        /// Populates the capabilities flags based on capabilities
        /// supported by this server. This method is overridable
        /// so that new features can be supported
        /// </summary>
        /// <param name="reply">The reply object from the FEAT command. The InfoMessages property will
        /// contain a list of the features the server supported delimited by a new line '\n' character.</param>
        protected virtual void GetFeatures(FtpReply reply)
        {
            FtpServerSpecificHandler.GetFeatures(this, m_capabilities, ref m_hashAlgorithms, reply.InfoMessages.Split('\n'));
        }

#if NET40
        private delegate void AsyncConnect();

        /// <summary>
        /// Initiates a connection to the server
        /// </summary>
        /// <param name="callback">AsyncCallback method</param>
        /// <param name="state">State object</param>
        /// <returns>IAsyncResult</returns>
        public IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {
            AsyncConnect func;
            IAsyncResult ar;

            lock (m_asyncmethods)
            {
                ar = (func = Connect).BeginInvoke(callback, state);
                m_asyncmethods.Add(ar, func);
            }

            return ar;
        }

        /// <summary>
        /// Ends an asynchronous connection attempt to the server from <see cref="BeginConnect"/>
        /// </summary>
        /// <param name="ar"><see cref="IAsyncResult"/> returned from <see cref="BeginConnect"/></param>
        public void EndConnect(IAsyncResult ar)
        {
            GetAsyncDelegate<AsyncConnect>(ar).EndInvoke(ar);
        }
#endif

        #endregion

        #region Login

        /// <summary>
        /// Performs a login on the server. This method is overridable so
        /// that the login procedure can be changed to support, for example,
        /// a FTP proxy.
        /// </summary>
        protected virtual void Authenticate()
        {
            Authenticate(Credentials.UserName, Credentials.Password, Credentials.Domain);
        }

#if !NET40
        /// <summary>
        /// Performs a login on the server. This method is overridable so
        /// that the login procedure can be changed to support, for example,
        /// a FTP proxy.
        /// </summary>
        protected virtual async Task AuthenticateAsync(CancellationToken token)
        {
            await AuthenticateAsync(Credentials.UserName, Credentials.Password, Credentials.Domain, token);
        }
#endif

        /// <summary>
        /// Performs a login on the server. This method is overridable so
        /// that the login procedure can be changed to support, for example,
        /// a FTP proxy.
        /// </summary>
        /// <exception cref="FtpAuthenticationException">On authentication failures</exception>
        /// <remarks>
        /// To handle authentication failures without retries, catch FtpAuthenticationException.
        /// </remarks>
        protected virtual void Authenticate(string userName, string password, string account)
        {

            // mark that we are not authenticated
            m_IsAuthenticated = false;

            // send the USER command along with the FTP username
            FtpReply reply = Execute("USER " + userName);

            // check the reply to the USER command
            if (!reply.Success)
            {
                throw new FtpAuthenticationException(reply);
            }

            // if it was accepted
            else if (reply.Type == FtpResponseType.PositiveIntermediate)
            {

                // send the PASS command along with the FTP password
                reply = Execute("PASS " + password);

                // fix for #620: some servers send multiple responses that must be read and decoded,
                // otherwise the connection is aborted and remade and it goes into an infinite loop
                var staleData = ReadStaleData(false, true, true);
                if (staleData != null)
                {
                    var staleReply = new FtpReply();
                    if (DecodeStringToReply(staleData, ref staleReply) && !staleReply.Success)
                    {
                        throw new FtpAuthenticationException(staleReply);
                    }
                }

                // check the first reply to the PASS command
                if (!reply.Success)
                {
                    throw new FtpAuthenticationException(reply);
                }

                // only possible 3** here is `332 Need account for login`
                if (reply.Type == FtpResponseType.PositiveIntermediate)
                {
                    reply = Execute("ACCT " + account);

                    if (!reply.Success)
                    {
                        throw new FtpAuthenticationException(reply);
                    }
                }

                // mark that we are authenticated
                m_IsAuthenticated = true;

            }
        }

#if !NET40
        /// <summary>
        /// Performs a login on the server. This method is overridable so
        /// that the login procedure can be changed to support, for example,
        /// a FTP proxy.
        /// </summary>
        /// <exception cref="FtpAuthenticationException">On authentication failures</exception>
        /// <remarks>
        /// To handle authentication failures without retries, catch FtpAuthenticationException.
        /// </remarks>
        protected virtual async Task AuthenticateAsync(string userName, string password, string account, CancellationToken token)
        {

            // send the USER command along with the FTP username
            FtpReply reply = await ExecuteAsync("USER " + userName, token);

            // check the reply to the USER command
            if (!reply.Success)
            {
                throw new FtpAuthenticationException(reply);
            }

            // if it was accepted
            else if (reply.Type == FtpResponseType.PositiveIntermediate)
            {

                // send the PASS command along with the FTP password
                reply = await ExecuteAsync("PASS " + password, token);

                // fix for #620: some servers send multiple responses that must be read and decoded,
                // otherwise the connection is aborted and remade and it goes into an infinite loop
                var staleData = await ReadStaleDataAsync(false, true, true, token);
                if (staleData != null)
                {
                    var staleReply = new FtpReply();
                    if (DecodeStringToReply(staleData, ref staleReply) && !staleReply.Success)
                    {
                        throw new FtpAuthenticationException(staleReply);
                    }
                }

                // check the first reply to the PASS command
                if (!reply.Success)
                {
                    throw new FtpAuthenticationException(reply);
                }

                // only possible 3** here is `332 Need account for login`
                if (reply.Type == FtpResponseType.PositiveIntermediate)
                {
                    reply = await ExecuteAsync("ACCT " + account, token);

                    if (!reply.Success)
                    {
                        throw new FtpAuthenticationException(reply);
                    }
                    else
                    {
                        m_IsAuthenticated = true;
                    }
                }
                else if (reply.Type == FtpResponseType.PositiveCompletion)
                {
                    m_IsAuthenticated = true;
                }
            }
        }
#endif

        #endregion

        #region Disconnect

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public virtual void Disconnect()
        {
            lock (m_lock)
            {
                if (m_stream != null && m_stream.IsConnected)
                {
                    try
                    {
                        if (!UngracefullDisconnection)
                        {
                            Execute("QUIT");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): Exception caught and discarded while closing control connection: " + ex.ToString());
                    }
                    finally
                    {
                        m_stream.Close();
                    }
                }
            }
        }

#if NET40
        private delegate void AsyncDisconnect();

        /// <summary>
        /// Initiates a disconnection on the server
        /// </summary>
        /// <param name="callback"><see cref="AsyncCallback"/> method</param>
        /// <param name="state">State object</param>
        /// <returns>IAsyncResult</returns>
        public IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
        {
            IAsyncResult ar;
            AsyncDisconnect func;

            lock (m_asyncmethods)
            {
                ar = (func = Disconnect).BeginInvoke(callback, state);
                m_asyncmethods.Add(ar, func);
            }

            return ar;
        }

        /// <summary>
        /// Ends a call to <see cref="BeginDisconnect"/>
        /// </summary>
        /// <param name="ar"><see cref="IAsyncResult"/> returned from <see cref="BeginDisconnect"/></param>
        public void EndDisconnect(IAsyncResult ar)
        {
            GetAsyncDelegate<AsyncDisconnect>(ar).EndInvoke(ar);
        }

#endif
#if !NET40
        /// <summary>
        /// Disconnects from the server asynchronously
        /// </summary>
        public async Task DisconnectAsync(CancellationToken token = default(CancellationToken))
        {
            if (m_stream != null && m_stream.IsConnected)
            {
                try
                {
                    if (!UngracefullDisconnection)
                    {
                        await ExecuteAsync("QUIT", token);
                    }
                }
                catch (Exception ex)
                {
                    LogStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): Exception caught and discarded while closing control connection: " + ex.ToString());
                }
                finally
                {
                    m_stream.Close();
                }
            }
        }
#endif

        #endregion

        #region FTPS

        /// <summary>
        /// Catches the socket stream ssl validation event and fires the event handlers
        /// attached to this object for validating SSL certificates
        /// </summary>
        /// <param name="stream">The stream that fired the event</param>
        /// <param name="e">The event args used to validate the certificate</param>
        private void FireValidateCertficate(FtpSocketStream stream, FtpSslValidationEventArgs e)
        {
            OnValidateCertficate(e);
        }

        /// <summary>
        /// Fires the SSL validation event
        /// </summary>
        /// <param name="e">Event Args</param>
        private void OnValidateCertficate(FtpSslValidationEventArgs e)
        {

            // automatically validate if ValidateAnyCertificate is set
            if (ValidateAnyCertificate)
            {
                e.Accept = true;
                return;
            }

            // fallback to manual validation using the ValidateCertificate event
            m_ValidateCertificate?.Invoke(this, e);

        }

        #endregion
        #endregion // FtpClient_Connection.cs
        #region // FtpClient_FileCompare.cs
        /// <summary>
        /// Compare the specified local file with the remote file on the FTP server using various kinds of quick equality checks.
        /// In Auto mode, the file size and checksum are compared.
        /// Comparing the checksum of a file is a quick way to check if the contents of the files are exactly equal without downloading a copy of the file.
        /// You can use the option flags to compare any combination of: file size, checksum, date modified.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="options">Types of equality checks to perform. Use Auto to compare file size and checksum.</param>
        /// <returns></returns>
        public FtpCompareResult CompareFile(string localPath, string remotePath, FtpCompareOption options = FtpCompareOption.Auto)
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(CompareFile), new object[] { localPath, remotePath, options });


            // ensure both files exists
            if (!File.Exists(localPath))
            {
                return FtpCompareResult.FileNotExisting;
            }
            if (!FileExists(remotePath))
            {
                return FtpCompareResult.FileNotExisting;
            }

            // if file size check enabled
            if (options == FtpCompareOption.Auto || options.HasFlag(FtpCompareOption.Size))
            {

                // check file size
                var localSize = FtpFileStream.GetFileSize(localPath, false);
                var remoteSize = GetFileSize(remotePath);
                if (localSize != remoteSize)
                {
                    return FtpCompareResult.NotEqual;
                }

            }

            // if date check enabled
            if (options.HasFlag(FtpCompareOption.DateModified))
            {

                // check file size
                var localDate = FtpFileStream.GetFileDateModifiedUtc(localPath);
                var remoteDate = GetModifiedTime(remotePath);
                if (!localDate.Equals(remoteDate))
                {
                    return FtpCompareResult.NotEqual;
                }

            }

            // if checksum check enabled
            if (options == FtpCompareOption.Auto || options.HasFlag(FtpCompareOption.Checksum))
            {

                // check file checksum
                if (SupportsChecksum())
                {
                    var hash = GetChecksum(remotePath);
                    if (hash.IsValid)
                    {
                        if (!hash.Verify(localPath))
                        {
                            return FtpCompareResult.NotEqual;
                        }
                    }
                    else
                    {
                        return FtpCompareResult.ChecksumNotSupported;
                    }
                }
                else
                {
                    return FtpCompareResult.ChecksumNotSupported;
                }

            }

            // all checks passed!
            return FtpCompareResult.Equal;
        }

#if !NET40
        /// <summary>
        /// Compare the specified local file with the remote file on the FTP server using various kinds of quick equality checks.
        /// In Auto mode, the file size and checksum are compared.
        /// Comparing the checksum of a file is a quick way to check if the contents of the files are exactly equal without downloading a copy of the file.
        /// You can use the option flags to compare any combination of: file size, checksum, date modified.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="options">Types of equality checks to perform. Use Auto to compare file size and checksum.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns></returns>
        public async Task<FtpCompareResult> CompareFileAsync(string localPath, string remotePath, FtpCompareOption options = FtpCompareOption.Auto,
            CancellationToken token = default(CancellationToken))
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(CompareFileAsync), new object[] { localPath, remotePath, options });


            // ensure both files exists
            if (!File.Exists(localPath))
            {
                return FtpCompareResult.FileNotExisting;
            }
            if (!await FileExistsAsync(remotePath, token))
            {
                return FtpCompareResult.FileNotExisting;
            }

            // if file size check enabled
            if (options == FtpCompareOption.Auto || options.HasFlag(FtpCompareOption.Size))
            {

                // check file size
                var localSize = await FtpFileStream.GetFileSizeAsync(localPath, false, token);
                var remoteSize = await GetFileSizeAsync(remotePath, -1, token);
                if (localSize != remoteSize)
                {
                    return FtpCompareResult.NotEqual;
                }

            }

            // if date check enabled
            if (options.HasFlag(FtpCompareOption.DateModified))
            {

                // check file size
                var localDate = await FtpFileStream.GetFileDateModifiedUtcAsync(localPath, token);
                var remoteDate = await GetModifiedTimeAsync(remotePath, token);
                if (!localDate.Equals(remoteDate))
                {
                    return FtpCompareResult.NotEqual;
                }

            }

            // if checksum check enabled
            if (options == FtpCompareOption.Auto || options.HasFlag(FtpCompareOption.Checksum))
            {

                // check file checksum
                if (SupportsChecksum())
                {
                    var hash = await GetChecksumAsync(remotePath, FtpHashAlgorithm.NONE, token);
                    if (hash.IsValid)
                    {
                        if (!hash.Verify(localPath))
                        {
                            return FtpCompareResult.NotEqual;
                        }
                    }
                    else
                    {
                        return FtpCompareResult.ChecksumNotSupported;
                    }
                }
                else
                {
                    return FtpCompareResult.ChecksumNotSupported;
                }

            }

            // all checks passed!
            return FtpCompareResult.Equal;
        }

#endif
        #endregion
        #region // FtpClient_FileDownload.cs
        #region Download Multiple Files

        /// <summary>
        /// Downloads the specified files into a local single directory.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// Same speed as <see cref="o:DownloadFile"/>.
        /// </summary>
        /// <param name="localDir">The full or relative path to the directory that files will be downloaded into.</param>
        /// <param name="remotePaths">The full or relative paths to the files on the server</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="errorHandling">Used to determine how errors are handled</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        /// <returns>The count of how many files were downloaded successfully. When existing files are skipped, they are not counted.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        public int DownloadFiles(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None,
            FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null)
        {

            // verify args
            if (!errorHandling.IsValidCombination())
            {
                throw new ArgumentException("Invalid combination of FtpError flags.  Throw & Stop cannot be combined");
            }

            if (localDir.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localDir");
            }

            LogFunc(nameof(DownloadFiles), new object[] { localDir, remotePaths, existsMode, verifyOptions });

            var errorEncountered = false;
            var successfulDownloads = new List<string>();

            // ensure ends with slash
            localDir = !localDir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? localDir + Path.DirectorySeparatorChar.ToString() : localDir;

            // per remote file
            var r = -1;
            foreach (var remotePath in remotePaths)
            {
                r++;

                // calc local path
                var localPath = localDir + remotePath.GetFtpFileName();

                // create meta progress to store the file progress
                var metaProgress = new FtpProgress(remotePaths.Count(), r);

                // try to download it
                try
                {
                    var ok = DownloadFileToFile(localPath, remotePath, existsMode, verifyOptions, progress, metaProgress);
                    if (ok.IsSuccess())
                    {
                        successfulDownloads.Add(localPath);
                    }
                    else if ((int)errorHandling > 1)
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogStatus(FtpTraceLevel.Error, "Failed to download " + remotePath + ". Error: " + ex);
                    if (errorHandling.HasFlag(FtpError.Stop))
                    {
                        errorEncountered = true;
                        break;
                    }

                    if (errorHandling.HasFlag(FtpError.Throw))
                    {
                        if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                        {
                            PurgeSuccessfulDownloads(successfulDownloads);
                        }

                        throw new FtpException("An error occurred downloading file(s).  See inner exception for more info.", ex);
                    }
                }
            }

            if (errorEncountered)
            {
                //Delete any successful uploads if needed
                if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                {
                    PurgeSuccessfulDownloads(successfulDownloads);
                    successfulDownloads.Clear(); //forces return of 0
                }

                //Throw generic error because requested
                if (errorHandling.HasFlag(FtpError.Throw))
                {
                    throw new FtpException("An error occurred downloading one or more files.  Refer to trace output if available.");
                }
            }

            return successfulDownloads.Count;
        }


        private void PurgeSuccessfulDownloads(IEnumerable<string> localFiles)
        {
            foreach (var localFile in localFiles)
            {
                // absorb any errors because we don't want this to throw more errors!
                try
                {
                    File.Delete(localFile);
                }
                catch (Exception ex)
                {
                    LogStatus(FtpTraceLevel.Warn, "FtpClient : Exception caught and discarded while attempting to delete file '" + localFile + "' : " + ex.ToString());
                }
            }
        }

#if !NET40
        /// <summary>
        /// Downloads the specified files into a local single directory.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// Same speed as <see cref="o:DownloadFile"/>.
        /// </summary>
        /// <param name="localDir">The full or relative path to the directory that files will be downloaded.</param>
        /// <param name="remotePaths">The full or relative paths to the files on the server</param>
        /// <param name="existsMode">Overwrite if you want the local file to be overwritten if it already exists. Append will also create a new file if it doesn't exists</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="errorHandling">Used to determine how errors are handled</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <returns>The count of how many files were downloaded successfully. When existing files are skipped, they are not counted.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically be set to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        public async Task<int> DownloadFilesAsync(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = FtpLocalExists.Overwrite,
            FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default(CancellationToken), IProgress<FtpProgress> progress = null)
        {

            // verify args
            if (!errorHandling.IsValidCombination())
            {
                throw new ArgumentException("Invalid combination of FtpError flags.  Throw & Stop cannot be combined");
            }

            if (localDir.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localDir");
            }

            LogFunc(nameof(DownloadFilesAsync), new object[] { localDir, remotePaths, existsMode, verifyOptions });

            //check if cancellation was requested and throw to set TaskStatus state to Canceled
            token.ThrowIfCancellationRequested();
            var errorEncountered = false;
            var successfulDownloads = new List<string>();

            // ensure ends with slash
            localDir = !localDir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? localDir + Path.DirectorySeparatorChar.ToString() : localDir;

            // per remote file
            var r = -1;
            foreach (var remotePath in remotePaths)
            {
                r++;

                //check if cancellation was requested and throw to set TaskStatus state to Canceled
                token.ThrowIfCancellationRequested();

                // calc local path
                var localPath = localDir + remotePath.GetFtpFileName();

                // create meta progress to store the file progress
                var metaProgress = new FtpProgress(remotePaths.Count(), r);

                // try to download it
                try
                {
                    var ok = await DownloadFileToFileAsync(localPath, remotePath, existsMode, verifyOptions, progress, token, metaProgress);
                    if (ok.IsSuccess())
                    {
                        successfulDownloads.Add(localPath);
                    }
                    else if ((int)errorHandling > 1)
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        LogStatus(FtpTraceLevel.Info, "Download cancellation requested");

                        //DO NOT SUPPRESS CANCELLATION REQUESTS -- BUBBLE UP!
                        throw;
                    }

                    if (errorHandling.HasFlag(FtpError.Stop))
                    {
                        errorEncountered = true;
                        break;
                    }

                    if (errorHandling.HasFlag(FtpError.Throw))
                    {
                        if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                        {
                            PurgeSuccessfulDownloads(successfulDownloads);
                        }

                        throw new FtpException("An error occurred downloading file(s).  See inner exception for more info.", ex);
                    }
                }
            }

            if (errorEncountered)
            {
                //Delete any successful uploads if needed
                if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                {
                    PurgeSuccessfulDownloads(successfulDownloads);
                    successfulDownloads.Clear(); //forces return of 0
                }

                //Throw generic error because requested
                if (errorHandling.HasFlag(FtpError.Throw))
                {
                    throw new FtpException("An error occurred downloading one or more files.  Refer to trace output if available.");
                }
            }

            return successfulDownloads.Count;
        }
#endif

        #endregion

        #region Download File

        /// <summary>
        /// Downloads the specified file onto the local file system.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <returns>FtpStatus flag indicating if the file was downloaded, skipped or failed to transfer.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically be set to true for subsequent attempts.
        /// </remarks>
        public FtpStatus DownloadFile(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, Action<FtpProgress> progress = null)
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            return DownloadFileToFile(localPath, remotePath, existsMode, verifyOptions, progress, new FtpProgress(1, 0));
        }

        private FtpStatus DownloadFileToFile(string localPath, string remotePath, FtpLocalExists existsMode, FtpVerify verifyOptions, Action<FtpProgress> progress, FtpProgress metaProgress)
        {
            bool isAppend = false;

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(DownloadFile), new object[] { localPath, remotePath, existsMode, verifyOptions });

            // skip downloading if the localPath is a folder
            if (LocalPaths.IsLocalFolderPath(localPath))
            {
                throw new ArgumentException("Local path must specify a file path and not a folder path.", "localPath");
            }

            // skip downloading if local file size matches
            long knownFileSize = 0;
            long restartPos = 0;
            if (existsMode == FtpLocalExists.Resume && File.Exists(localPath))
            {
                knownFileSize = GetFileSize(remotePath);
                restartPos = FtpFileStream.GetFileSize(localPath, false);
                if (knownFileSize.Equals(restartPos))
                {
                    LogStatus(FtpTraceLevel.Info, "Skipping file because Resume is enabled and file is fully downloaded (Remote: " + remotePath + ", Local: " + localPath + ")");
                    return FtpStatus.Skipped;
                }
                else
                {
                    isAppend = true;
                }
            }
            else if (existsMode == FtpLocalExists.Skip && File.Exists(localPath))
            {
                LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists locally (Remote: " + remotePath + ", Local: " + localPath + ")");
                return FtpStatus.Skipped;
            }

            try
            {
                // create the folders
                var dirPath = Path.GetDirectoryName(localPath);
                if (!Strings.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
            catch (Exception ex1)
            {
                // catch errors creating directory
                throw new FtpException("Error while creating directories. See InnerException for more info.", ex1);
            }

            // if not appending then fetch remote file size since mode is determined by that
            /*if (knownFileSize == 0 && !isAppend) {
				knownFileSize = GetFileSize(remotePath);
			}*/

            bool downloadSuccess;
            var verified = true;
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;
            do
            {

                // download the file from the server to a file stream or memory stream
                downloadSuccess = DownloadFileInternal(localPath, remotePath, null, restartPos, progress, metaProgress, knownFileSize, isAppend);
                attemptsLeft--;

                if (!downloadSuccess)
                {
                    LogStatus(FtpTraceLevel.Info, "Failed to download file.");

                    if (attemptsLeft > 0)
                        LogStatus(FtpTraceLevel.Info, "Retrying to download file.");
                }

                // if verification is needed
                if (downloadSuccess && verifyOptions != FtpVerify.None)
                {
                    verified = VerifyTransfer(localPath, remotePath);
                    LogLine(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                    if (!verified && attemptsLeft > 0)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode == FtpLocalExists.Overwrite ? "  Overwrite will occur." : "") + "  " + attemptsLeft + " attempts remaining");
                        // Force overwrite if a retry is required
                        existsMode = FtpLocalExists.Overwrite;
                    }
                }
            } while ((!downloadSuccess || !verified) && attemptsLeft > 0);

            if (downloadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                File.Delete(localPath);
            }

            if (downloadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Downloaded file checksum value does not match remote file");
            }

            return downloadSuccess && verified ? FtpStatus.Success : FtpStatus.Failed;
        }

#if !NET40
        /// <summary>
        /// Downloads the specified file onto the local file system asynchronously.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">Overwrite if you want the local file to be overwritten if it already exists. Append will also create a new file if it doesn't exists</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="progress">Provide an implementation of IProgress to track download progress.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>FtpStatus flag indicating if the file was downloaded, skipped or failed to transfer.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically be set to true for subsequent attempts.
        /// </remarks>
        public async Task<FtpStatus> DownloadFileAsync(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Resume, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            return await DownloadFileToFileAsync(localPath, remotePath, existsMode, verifyOptions, progress, token, new FtpProgress(1, 0));
        }

        private async Task<FtpStatus> DownloadFileToFileAsync(string localPath, string remotePath, FtpLocalExists existsMode, FtpVerify verifyOptions, IProgress<FtpProgress> progress, CancellationToken token, FtpProgress metaProgress)
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            // skip downloading if the localPath is a folder
            if (LocalPaths.IsLocalFolderPath(localPath))
            {
                throw new ArgumentException("Local path must specify a file path and not a folder path.", "localPath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(DownloadFileAsync), new object[] { localPath, remotePath, existsMode, verifyOptions });


            bool isAppend = false;

            // skip downloading if the local file exists
            long knownFileSize = 0;
            long restartPos = 0;
#if NETFx
            if (existsMode == FtpLocalExists.Resume && await Task.Run(() => File.Exists(localPath), token))
            {
                knownFileSize = (await GetFileSizeAsync(remotePath, -1, token));
                restartPos = await FtpFileStream.GetFileSizeAsync(localPath, false, token);
                if (knownFileSize.Equals(restartPos))
                {
#else
            if (existsMode == FtpLocalExists.Resume && File.Exists(localPath))
            {
                knownFileSize = (await GetFileSizeAsync(remotePath, -1, token));
                restartPos = FtpFileStream.GetFileSize(localPath, false);
                if (knownFileSize.Equals(restartPos))
                {
#endif
                    LogStatus(FtpTraceLevel.Info, "Skipping file because Resume is enabled and file is fully downloaded (Remote: " + remotePath + ", Local: " + localPath + ")");
                    return FtpStatus.Skipped;
                }
                else
                {
                    isAppend = true;
                }
            }
#if NETFx
            else if (existsMode == FtpLocalExists.Skip && await Task.Run(() => File.Exists(localPath), token))
            {
#else
            else if (existsMode == FtpLocalExists.Skip && File.Exists(localPath))
            {
#endif
                LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists locally (Remote: " + remotePath + ", Local: " + localPath + ")");
                return FtpStatus.Skipped;
            }

            try
            {
                // create the folders
                var dirPath = Path.GetDirectoryName(localPath);
#if NETFx
                if (!string.IsNullOrWhiteSpace(dirPath) && !await Task.Run(() => Directory.Exists(dirPath), token))
                {
#else
                if (!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
                {
#endif
                    Directory.CreateDirectory(dirPath);
                }
            }
            catch (Exception ex1)
            {
                // catch errors creating directory
                throw new FtpException("Error while crated directories. See InnerException for more info.", ex1);
            }

            // if not appending then fetch remote file size since mode is determined by that
            /*if (knownFileSize == 0 && !isAppend) {
				knownFileSize = GetFileSize(remotePath);
			}*/

            bool downloadSuccess;
            var verified = true;
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;
            do
            {

                // download the file from the server to a file stream or memory stream
                downloadSuccess = await DownloadFileInternalAsync(localPath, remotePath, null, restartPos, progress, token, metaProgress, knownFileSize, isAppend);
                attemptsLeft--;

                if (!downloadSuccess)
                {
                    LogStatus(FtpTraceLevel.Info, "Failed to download file.");

                    if (attemptsLeft > 0)
                        LogStatus(FtpTraceLevel.Info, "Retrying to download file.");
                }

                // if verification is needed
                if (downloadSuccess && verifyOptions != FtpVerify.None)
                {
                    verified = await VerifyTransferAsync(localPath, remotePath, token);
                    LogStatus(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                    if (!verified && attemptsLeft > 0)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode == FtpLocalExists.Resume ? "  Overwrite will occur." : "") + "  " + attemptsLeft + " attempts remaining");
                        // Force overwrite if a retry is required
                        existsMode = FtpLocalExists.Overwrite;
                    }
                }
            } while ((!downloadSuccess || !verified) && attemptsLeft > 0);

            if (downloadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                File.Delete(localPath);
            }

            if (downloadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Downloaded file checksum value does not match remote file");
            }

            return downloadSuccess && verified ? FtpStatus.Success : FtpStatus.Failed;
        }

#endif

        #endregion

        #region	Download Bytes/Stream

        /// <summary>
        /// Downloads the specified file into the specified stream.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="outStream">The stream that the file will be written to. Provide a new MemoryStream if you only want to read the file into memory.</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="restartPosition">The size of the existing file in bytes, or 0 if unknown. The download restarts from this byte index.</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <returns>If true then the file was downloaded, false otherwise.</returns>
        public bool Download(Stream outStream, string remotePath, long restartPosition = 0, Action<FtpProgress> progress = null)
        {
            // verify args
            if (outStream == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "outStream");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(Download), new object[] { remotePath });

            // download the file from the server
            return DownloadFileInternal(null, remotePath, outStream, restartPosition, progress, new FtpProgress(1, 0), 0, false);
        }

        /// <summary>
        /// Downloads the specified file and return the raw byte array.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="outBytes">The variable that will receive the bytes.</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="restartPosition">The size of the existing file in bytes, or 0 if unknown. The download restarts from this byte index.</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <returns>If true then the file was downloaded, false otherwise.</returns>
        public bool Download(out byte[] outBytes, string remotePath, long restartPosition = 0, Action<FtpProgress> progress = null)
        {
            // verify args
            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(Download), new object[] { remotePath });

            outBytes = null;

            // download the file from the server
            bool ok;
            using (var outStream = new MemoryStream())
            {
                ok = DownloadFileInternal(null, remotePath, outStream, restartPosition, progress, new FtpProgress(1, 0), 0, false);
                if (ok)
                {
                    outBytes = outStream.ToArray();
                }
            }

            return ok;
        }

#if !NET40
        /// <summary>
        /// Downloads the specified file into the specified stream asynchronously .
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="outStream">The stream that the file will be written to. Provide a new MemoryStream if you only want to read the file into memory.</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="restartPosition">The size of the existing file in bytes, or 0 if unknown. The download restarts from this byte index.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="progress">Provide an implementation of IProgress to track download progress.</param>
        /// <returns>If true then the file was downloaded, false otherwise.</returns>
        public async Task<bool> DownloadAsync(Stream outStream, string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (outStream == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "outStream");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(DownloadAsync), new object[] { remotePath });

            // download the file from the server
            return await DownloadFileInternalAsync(null, remotePath, outStream, restartPosition, progress, token, new FtpProgress(1, 0), 0, false);
        }

        /// <summary>
        /// Downloads the specified file and return the raw byte array.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="restartPosition">The size of the existing file in bytes, or 0 if unknown. The download restarts from this byte index.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="progress">Provide an implementation of IProgress to track download progress.</param>
        /// <returns>A byte array containing the contents of the downloaded file if successful, otherwise null.</returns>
        public async Task<byte[]> DownloadAsync(string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(DownloadAsync), new object[] { remotePath });

            // download the file from the server
            using (var outStream = new MemoryStream())
            {
                var ok = await DownloadFileInternalAsync(null, remotePath, outStream, restartPosition, progress, token, new FtpProgress(1, 0), 0, false);
                return ok ? outStream.ToArray() : null;
            }
        }

        /// <summary>
        /// Downloads the specified file into the specified stream asynchronously .
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it downloads data in chunks.
        /// </summary>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A byte array containing the contents of the downloaded file if successful, otherwise null.</returns>
        public async Task<byte[]> DownloadAsync(string remotePath, CancellationToken token = default(CancellationToken))
        {
            // download the file from the server
            return await DownloadAsync(remotePath, 0, null, token);
        }
#endif

        #endregion

        #region Download File Internal

        /// <summary>
        /// Download a file from the server and write the data into the given stream.
        /// Reads data in chunks. Retries if server disconnects midway.
        /// </summary>
        private bool DownloadFileInternal(string localPath, string remotePath, Stream outStream, long restartPosition,
            Action<FtpProgress> progress, FtpProgress metaProgress, long knownFileSize, bool isAppend)
        {

            Stream downStream = null;
            var disposeOutStream = false;

            try
            {
                // get file size if progress requested
                long fileLen = 0;

                if (progress != null)
                {
                    fileLen = knownFileSize > 0 ? knownFileSize : GetFileSize(remotePath);
                }

                // open the file for reading
                downStream = OpenRead(remotePath, DownloadDataType, restartPosition, fileLen);

                // if the server has not provided a length for this file or
                // if the mode is ASCII or
                // if the server is IBM z/OS
                // we read until EOF instead of reading a specific number of bytes
                var readToEnd = (fileLen <= 0) ||
                                (DownloadDataType == FtpDataType.ASCII) ||
                                (ServerType == FtpServer.IBMzOSFTP);

                const int rateControlResolution = 100;
                var rateLimitBytes = DownloadRateLimit != 0 ? (long)DownloadRateLimit * 1024 : 0;
                var chunkSize = CalculateTransferChunkSize(rateLimitBytes, rateControlResolution);

                // loop till entire file downloaded
                var buffer = new byte[chunkSize];
                var offset = restartPosition;

                var transferStarted = DateTime.Now;
                var sw = new Stopwatch();

                var anyNoop = false;

                // Fix #554: ability to download zero-byte files
                if (DownloadZeroByteFiles && outStream == null && localPath != null)
                {
                    outStream = FtpFileStream.GetFileWriteStream(this, localPath, false, QuickTransferLimit, knownFileSize, isAppend, restartPosition);
                    disposeOutStream = true;
                }

                while (offset < fileLen || readToEnd)
                {
                    try
                    {
                        // read a chunk of bytes from the FTP stream
                        var readBytes = 1;
                        long limitCheckBytes = 0;
                        long bytesProcessed = 0;

                        sw.Start();
                        while ((readBytes = downStream.Read(buffer, 0, buffer.Length)) > 0)
                        {

                            // Fix #552: only create outstream when first bytes downloaded
                            if (outStream == null && localPath != null)
                            {
                                outStream = FtpFileStream.GetFileWriteStream(this, localPath, false, QuickTransferLimit, knownFileSize, isAppend, restartPosition);
                                disposeOutStream = true;
                            }

                            // write chunk to output stream
                            outStream.Write(buffer, 0, readBytes);
                            offset += readBytes;
                            bytesProcessed += readBytes;
                            limitCheckBytes += readBytes;

                            // send progress reports
                            if (progress != null)
                            {
                                ReportProgress(progress, fileLen, offset, bytesProcessed, DateTime.Now - transferStarted, localPath, remotePath, metaProgress);
                            }

                            // Fix #387: keep alive with NOOP as configured and needed
                            if (!m_threadSafeDataChannels)
                            {
                                anyNoop = Noop() || anyNoop;
                            }

                            // honor the rate limit
                            var swTime = sw.ElapsedMilliseconds;
                            if (rateLimitBytes > 0)
                            {
                                var timeShouldTake = limitCheckBytes * 1000 / rateLimitBytes;
                                if (timeShouldTake > swTime)
                                {
                                    Thread.Sleep((int)(timeShouldTake - swTime)); // Task.Delay((int)(timeShouldTake - swTime)).Wait();
                                }
                                else if (swTime > timeShouldTake + rateControlResolution)
                                {
                                    limitCheckBytes = 0;
                                    sw.Restart();
                                }
                            }
                        }

                        // if we reach here means EOF encountered
                        // stop if we are in "read until EOF" mode
                        if (readToEnd || offset == fileLen)
                        {
                            break;
                        }

                        // zero return value (with no Exception) indicates EOS; so we should fail here and attempt to resume
                        throw new IOException($"Unexpected EOF for remote file {remotePath} [{offset}/{fileLen} bytes read]");
                    }
                    catch (IOException ex)
                    {

                        // resume if server disconnected midway, or throw if there is an exception doing that as well
                        if (!ResumeDownload(remotePath, ref downStream, offset, ex))
                        {
                            sw.Stop();
                            throw;
                        }
                    }
                    catch (TimeoutException ex)
                    {

                        // fix: attempting to download data after we reached the end of the stream
                        // often throws a timeout exception, so we silently absorb that here
                        if (offset >= fileLen && !readToEnd)
                        {
                            break;
                        }
                        else
                        {
                            sw.Stop();
                            throw ex;
                        }
                    }
                }

                sw.Stop();

                // disconnect FTP stream before exiting
                if (outStream != null)
                {
                    outStream.Flush();
                }
                downStream.Dispose();

                // Fix #552: close the filestream if it was created in this method
                if (disposeOutStream)
                {
                    outStream.Dispose();
                    disposeOutStream = false;
                }

                // send progress reports
                if (progress != null)
                {
                    progress(new FtpProgress(100.0, offset, 0, TimeSpan.Zero, localPath, remotePath, metaProgress));
                }

                // FIX : if this is not added, there appears to be "stale data" on the socket
                // listen for a success/failure reply
                try
                {
                    while (!m_threadSafeDataChannels)
                    {
                        var status = GetReply();

                        // Fix #387: exhaust any NOOP responses (not guaranteed during file transfers)
                        if (anyNoop && status.Message != null && status.Message.Contains("NOOP"))
                        {
                            continue;
                        }

                        // Fix #353: if server sends 550 or 5xx the transfer was received but could not be confirmed by the server
                        // Fix #509: if server sends 450 or 4xx the transfer was aborted or failed midway
                        if (status.Code != null && !status.Success)
                        {
                            return false;
                        }

                        // Fix #387: exhaust any NOOP responses also after "226 Transfer complete."
                        if (anyNoop)
                        {
                            ReadStaleData(false, true, true);
                        }

                        break;
                    }
                }

                // absorb "System.TimeoutException: Timed out trying to read data from the socket stream!" at GetReply()
                catch (Exception) { }

                return true;
            }
            catch (Exception ex1)
            {

                // close stream before throwing error
                try
                {
                    downStream.Dispose();
                }
                catch (Exception)
                {
                }

                // Fix #552: close the filestream if it was created in this method
                if (disposeOutStream)
                {
                    try
                    {
                        outStream.Dispose();
                        disposeOutStream = false;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (ex1 is IOException)
                {
                    LogStatus(FtpTraceLevel.Verbose, "IOException for file " + localPath + " : " + ex1.Message);
                    return false;
                }

                // absorb "file does not exist" exceptions and simply return false
                if (ex1.Message.IsKnownError(FtpServerStrings.fileNotFound))
                {
                    LogStatus(FtpTraceLevel.Error, "File does not exist: " + ex1.Message);
                    return false;
                }

                // catch errors during download
                throw new FtpException("Error while downloading the file from the server. See InnerException for more info.", ex1);
            }
        }

        /// <summary>
        /// Calculate transfer chunk size taking rate control into account
        /// </summary>
        private int CalculateTransferChunkSize(Int64 rateLimitBytes, int rateControlResolution)
        {
            int chunkSize = TransferChunkSize;

            // if user has not specified a TransferChunkSize and rate limiting is enabled
            if (m_transferChunkSize == null && rateLimitBytes > 0)
            {

                // reduce chunk size to optimize rate control
                const int chunkSizeMin = 64;
                while (chunkSize > chunkSizeMin)
                {
                    var chunkLenInMs = 1000L * chunkSize / rateLimitBytes;
                    if (chunkLenInMs <= rateControlResolution)
                    {
                        break;
                    }

                    chunkSize = Math.Max(chunkSize >> 1, chunkSizeMin);
                }
            }
            return chunkSize;
        }

#if !NET40
        /// <summary>
        /// Download a file from the server and write the data into the given stream asynchronously.
        /// Reads data in chunks. Retries if server disconnects midway.
        /// </summary>
        private async Task<bool> DownloadFileInternalAsync(string localPath, string remotePath, Stream outStream, long restartPosition,
            IProgress<FtpProgress> progress, CancellationToken token, FtpProgress metaProgress, long knownFileSize, bool isAppend)
        {

            Stream downStream = null;
            var disposeOutStream = false;

            try
            {
                // get file size if progress requested
                long fileLen = 0;

                if (progress != null)
                {
                    fileLen = knownFileSize > 0 ? knownFileSize : await GetFileSizeAsync(remotePath, -1, token);
                }

                // open the file for reading
                downStream = await OpenReadAsync(remotePath, DownloadDataType, restartPosition, fileLen, token);

                // if the server has not provided a length for this file or
                // if the mode is ASCII or
                // if the server is IBM z/OS
                // we read until EOF instead of reading a specific number of bytes
                var readToEnd = (fileLen <= 0) ||
                                (DownloadDataType == FtpDataType.ASCII) ||
                                (ServerType == FtpServer.IBMzOSFTP);

                const int rateControlResolution = 100;
                var rateLimitBytes = DownloadRateLimit != 0 ? (long)DownloadRateLimit * 1024 : 0;
                var chunkSize = CalculateTransferChunkSize(rateLimitBytes, rateControlResolution);

                // loop till entire file downloaded
                var buffer = new byte[chunkSize];
                var offset = restartPosition;

                var transferStarted = DateTime.Now;
                var sw = new Stopwatch();

                var anyNoop = false;

                // Fix #554: ability to download zero-byte files
                if (DownloadZeroByteFiles && outStream == null && localPath != null)
                {
                    outStream = FtpFileStream.GetFileWriteStream(this, localPath, true, QuickTransferLimit, knownFileSize, isAppend, restartPosition);
                    disposeOutStream = true;
                }

                while (offset < fileLen || readToEnd)
                {
                    try
                    {
                        // read a chunk of bytes from the FTP stream
                        var readBytes = 1;
                        long limitCheckBytes = 0;
                        long bytesProcessed = 0;

                        sw.Start();
                        while ((readBytes = await downStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                        {

                            // Fix #552: only create outstream when first bytes downloaded
                            if (outStream == null && localPath != null)
                            {
                                outStream = FtpFileStream.GetFileWriteStream(this, localPath, true, QuickTransferLimit, knownFileSize, isAppend, restartPosition);
                                disposeOutStream = true;
                            }

                            // write chunk to output stream
                            await outStream.WriteAsync(buffer, 0, readBytes, token);
                            offset += readBytes;
                            bytesProcessed += readBytes;
                            limitCheckBytes += readBytes;

                            // send progress reports
                            if (progress != null)
                            {
                                ReportProgress(progress, fileLen, offset, bytesProcessed, DateTime.Now - transferStarted, localPath, remotePath, metaProgress);
                            }

                            // Fix #387: keep alive with NOOP as configured and needed
                            if (!m_threadSafeDataChannels)
                            {
                                anyNoop = await NoopAsync(token) || anyNoop;
                            }

                            // honor the rate limit
                            var swTime = sw.ElapsedMilliseconds;
                            if (rateLimitBytes > 0)
                            {
                                var timeShouldTake = limitCheckBytes * 1000 / rateLimitBytes;
                                if (timeShouldTake > swTime)
                                {
                                    await Task.Delay((int)(timeShouldTake - swTime), token);
                                    token.ThrowIfCancellationRequested();
                                }
                                else if (swTime > timeShouldTake + rateControlResolution)
                                {
                                    limitCheckBytes = 0;
                                    sw.Restart();
                                }
                            }
                        }

                        // if we reach here means EOF encountered
                        // stop if we are in "read until EOF" mode
                        if (readToEnd || offset == fileLen)
                        {
                            break;
                        }

                        // zero return value (with no Exception) indicates EOS; so we should fail here and attempt to resume
                        throw new IOException($"Unexpected EOF for remote file {remotePath} [{offset}/{fileLen} bytes read]");
                    }
                    catch (IOException ex)
                    {

                        // resume if server disconnected midway, or throw if there is an exception doing that as well
                        var resumeResult = await ResumeDownloadAsync(remotePath, downStream, offset, ex);
                        if (resumeResult.Item1)
                        {
                            downStream = resumeResult.Item2;
                        }
                        else
                        {
                            sw.Stop();
                            throw;
                        }
                    }
                    catch (TimeoutException ex)
                    {

                        // fix: attempting to download data after we reached the end of the stream
                        // often throws a timeout exception, so we silently absorb that here
                        if (offset >= fileLen && !readToEnd)
                        {
                            break;
                        }
                        else
                        {
                            sw.Stop();
                            throw ex;
                        }
                    }
                }

                sw.Stop();

                // disconnect FTP stream before exiting
                if (outStream != null)
                {
                    await outStream.FlushAsync(token);
                }
                downStream.Dispose();

                // Fix #552: close the filestream if it was created in this method
                if (disposeOutStream)
                {
                    outStream.Dispose();
                    disposeOutStream = false;
                }

                // send progress reports
                if (progress != null)
                {
                    progress.Report(new FtpProgress(100.0, offset, 0, TimeSpan.Zero, localPath, remotePath, metaProgress));
                }

                // FIX : if this is not added, there appears to be "stale data" on the socket
                // listen for a success/failure reply
                try
                {
                    while (!m_threadSafeDataChannels)
                    {
                        FtpReply status = await GetReplyAsync(token);

                        // Fix #387: exhaust any NOOP responses (not guaranteed during file transfers)
                        if (anyNoop && status.Message != null && status.Message.Contains("NOOP"))
                        {
                            continue;
                        }

                        // Fix #353: if server sends 550 or 5xx the transfer was received but could not be confirmed by the server
                        // Fix #509: if server sends 450 or 4xx the transfer was aborted or failed midway
                        if (status.Code != null && !status.Success)
                        {
                            return false;
                        }

                        // Fix #387: exhaust any NOOP responses also after "226 Transfer complete."
                        if (anyNoop)
                        {
                            await ReadStaleDataAsync(false, true, true, token);
                        }

                        break;
                    }
                }

                // absorb "System.TimeoutException: Timed out trying to read data from the socket stream!" at GetReply()
                catch (Exception) { }

                return true;
            }
            catch (Exception ex1)
            {

                // close stream before throwing error
                try
                {
                    downStream.Dispose();
                }
                catch (Exception)
                {
                }

                // Fix #552: close the filestream if it was created in this method
                if (disposeOutStream)
                {
                    try
                    {
                        outStream.Dispose();
                        disposeOutStream = false;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (ex1 is IOException)
                {
                    LogStatus(FtpTraceLevel.Verbose, "IOException for file " + localPath + " : " + ex1.Message);
                    return false;
                }

                if (ex1 is OperationCanceledException)
                {
                    LogStatus(FtpTraceLevel.Info, "Download cancellation requested");
                    throw;
                }

                // absorb "file does not exist" exceptions and simply return false
                if (ex1.Message.IsKnownError(FtpServerStrings.fileNotFound))
                {
                    LogStatus(FtpTraceLevel.Error, "File does not exist: " + ex1.Message);
                    return false;
                }

                // catch errors during download
                throw new FtpException("Error while downloading the file from the server. See InnerException for more info.", ex1);
            }
        }
#endif

        private bool ResumeDownload(string remotePath, ref Stream downStream, long offset, IOException ex)
        {
            if (ex.IsResumeAllowed())
            {
                downStream.Dispose();
                downStream = OpenRead(remotePath, DownloadDataType, offset);

                return true;
            }

            return false;
        }

#if !NET40
        private async Task<Tuple<bool, Stream>> ResumeDownloadAsync(string remotePath, Stream downStream, long offset, IOException ex)
        {
            if (ex.IsResumeAllowed())
            {
                downStream.Dispose();

                return Tuple.Create(true, await OpenReadAsync(remotePath, DownloadDataType, offset));
            }

            return Tuple.Create(false, (Stream)null);
        }
#endif

        #endregion
        #endregion // FtpClient_FileDownload.cs
        #region // FtpClient_FileManagement.cs

        #region Delete File

        /// <summary>
        /// Deletes a file on the server
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        public void DeleteFile(string path)
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }
            lock (m_lock)
            {
                path = path.GetFtpPath();

                LogFunc(nameof(DeleteFile), new object[] { path });

                if (!(reply = Execute("DELE " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
        }

#if !NET40
        /// <summary>
        /// Deletes a file from the server asynchronously
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task DeleteFileAsync(string path, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(DeleteFileAsync), new object[] { path });

            if (!(reply = await ExecuteAsync("DELE " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }
        }
#endif

        #endregion

        #region File Exists

        /// <summary>
        /// Checks if a file exists on the server.
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        /// <returns>True if the file exists</returns>
        public bool FileExists(string path)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }
            lock (m_lock)
            {
                path = path.GetFtpPath();

                LogFunc(nameof(FileExists), new object[] { path });

                // A check for path.StartsWith("/") tells us, even if it is z/OS, we can use the normal unix logic

                // If z/OS: Do not GetAbsolutePath(), unless we have a leading slash
                if (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/"))
                {
                    // calc the absolute filepath
                    path = GetAbsolutePath(path);
                }

                // since FTP does not include a specific command to check if a file exists
                // here we check if file exists by attempting to get its filesize (SIZE)
                // If z/OS: Do not do SIZE, unless we have a leading slash
                if (HasFeature(FtpCapability.SIZE) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
                {

                    // Fix #328: get filesize in ASCII or Binary mode as required by server
                    var sizeReply = new FtpSizeReply();
                    GetFileSizeInternal(path, sizeReply, -1);

                    // handle known errors to the SIZE command
                    var sizeKnownError = CheckFileExistsBySize(sizeReply);
                    if (sizeKnownError.HasValue)
                    {
                        return sizeKnownError.Value;
                    }
                }

                // check if file exists by attempting to get its date modified (MDTM)
                // If z/OS: Do not do MDTM, unless we have a leading slash
                if (HasFeature(FtpCapability.MDTM) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
                {
                    var reply = Execute("MDTM " + path);
                    var ch = reply.Code[0];
                    if (ch == '2')
                    {
                        return true;
                    }
                    if (ch == '5' && reply.Message.IsKnownError(FtpServerStrings.fileNotFound))
                    {
                        return false;
                    }
                }

                // If z/OS: different handling, unless we have a leading slash
                if (ServerType == FtpServer.IBMzOSFTP && !path.StartsWith("/"))
                {
                    var fileList = GetNameListing(path);
                    return fileList.Count() > 0;
                }
                else
                // check if file exists by getting a name listing (NLST)
                {
                    var fileList = GetNameListing(path.GetFtpDirectoryName());
                    return FileListings.FileExistsInNameListing(fileList, path);
                }
            }
        }

        private bool? CheckFileExistsBySize(FtpSizeReply sizeReply)
        {

            // file surely exists
            if (sizeReply.Reply.Code[0] == '2')
            {
                return true;
            }

            // file surely does not exist
            if (sizeReply.Reply.Code[0] == '5' && sizeReply.Reply.Message.IsKnownError(FtpServerStrings.fileNotFound))
            {
                return false;
            }

            // Fix #518: This check is too broad and must be disabled, need to fallback to MDTM or NLST instead.
            // Fix #179: Add a generic check to since server returns 550 if file not found or no access to file.
            /*if (sizeReply.Reply.Code.Substring(0, 3) == "550") {
				return false;
			}*/

            // fallback to MDTM or NLST
            return null;
        }

#if !NET40
        /// <summary>
        /// Checks if a file exists on the server asynchronously.
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>True if the file exists, false otherwise</returns>
        public async Task<bool> FileExistsAsync(string path, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(FileExistsAsync), new object[] { path });

            // A check for path.StartsWith("/") tells us, even if it is z/OS, we can use the normal unix logic

            // Do not need GetAbsolutePath(path) if z/OS
            if (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/"))
            {
                // calc the absolute filepath
                path = await GetAbsolutePathAsync(path, token);
            }

            // since FTP does not include a specific command to check if a file exists
            // here we check if file exists by attempting to get its filesize (SIZE)
            // If z/OS: Do not do SIZE, unless we have a leading slash
            if (HasFeature(FtpCapability.SIZE) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
            {

                // Fix #328: get filesize in ASCII or Binary mode as required by server
                FtpSizeReply sizeReply = new FtpSizeReply();
                await GetFileSizeInternalAsync(path, -1, token, sizeReply);

                // handle known errors to the SIZE command
                var sizeKnownError = CheckFileExistsBySize(sizeReply);
                if (sizeKnownError.HasValue)
                {
                    return sizeKnownError.Value;
                }
            }

            // check if file exists by attempting to get its date modified (MDTM)
            // If z/OS: Do not do MDTM, unless we have a leading slash
            if (HasFeature(FtpCapability.MDTM) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
            {
                FtpReply reply = await ExecuteAsync("MDTM " + path, token);
                var ch = reply.Code[0];
                if (ch == '2')
                {
                    return true;
                }

                if (ch == '5' && reply.Message.IsKnownError(FtpServerStrings.fileNotFound))
                {
                    return false;
                }
            }

            // If z/OS: different handling, unless we have a leading slash
            if (ServerType == FtpServer.IBMzOSFTP && !path.StartsWith("/"))
            {
                var fileList = await GetNameListingAsync(path, token);
                return fileList.Count() > 0;
            }
            else
            // check if file exists by getting a name listing (NLST)
            {
                var fileList = await GetNameListingAsync(path.GetFtpDirectoryName(), token);
                return FileListings.FileExistsInNameListing(fileList, path);
            }
        }
#endif

        #endregion

        #region Rename File/Directory

        /// <summary>
        /// Renames an object on the remote file system.
        /// Low level method that should NOT be used in most cases. Prefer MoveFile() and MoveDirectory().
        /// Throws exceptions if the file does not exist, or if the destination file already exists.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        public void Rename(string path, string dest)
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }
            lock (m_lock)
            {
                path = path.GetFtpPath();
                dest = dest.GetFtpPath();

                LogFunc(nameof(Rename), new object[] { path, dest });

                // calc the absolute filepaths
                path = GetAbsolutePath(path);
                dest = GetAbsolutePath(dest);

                if (!(reply = Execute("RNFR " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }

                if (!(reply = Execute("RNTO " + dest)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
        }

#if !NET40
        /// <summary>
        /// Renames an object on the remote file system asynchronously.
        /// Low level method that should NOT be used in most cases. Prefer MoveFile() and MoveDirectory().
        /// Throws exceptions if the file does not exist, or if the destination file already exists.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task RenameAsync(string path, string dest, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(RenameAsync), new object[] { path, dest });

            // calc the absolute filepaths
            path = await GetAbsolutePathAsync(path, token);
            dest = await GetAbsolutePathAsync(dest, token);

            if (!(reply = await ExecuteAsync("RNFR " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            if (!(reply = await ExecuteAsync("RNTO " + dest, token)).Success)
            {
                throw new FtpCommandException(reply);
            }
        }
#endif

        #endregion

        #region Move File

        /// <summary>
        /// Moves a file on the remote file system from one directory to another.
        /// Always checks if the source file exists. Checks if the dest file exists based on the `existsMode` parameter.
        /// Only throws exceptions for critical errors.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="existsMode">Should we check if the dest file exists? And if it does should we overwrite/skip the operation?</param>
        /// <returns>Whether the file was moved</returns>
        public bool MoveFile(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(MoveFile), new object[] { path, dest, existsMode });

            if (FileExists(path))
            {
                // check if dest file exists and act accordingly
                if (existsMode != FtpRemoteExists.NoCheck)
                {
                    var destExists = FileExists(dest);
                    if (destExists)
                    {
                        switch (existsMode)
                        {
                            case FtpRemoteExists.Overwrite:
                                DeleteFile(dest);
                                break;

                            case FtpRemoteExists.Skip:
                                return false;
                        }
                    }
                }

                // move the file
                Rename(path, dest);

                return true;
            }

            return false;
        }

#if !NET40
        /// <summary>
        /// Moves a file asynchronously on the remote file system from one directory to another.
        /// Always checks if the source file exists. Checks if the dest file exists based on the `existsMode` parameter.
        /// Only throws exceptions for critical errors.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="existsMode">Should we check if the dest file exists? And if it does should we overwrite/skip the operation?</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>Whether the file was moved</returns>
        public async Task<bool> MoveFileAsync(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(MoveFileAsync), new object[] { path, dest, existsMode });

            if (await FileExistsAsync(path, token))
            {
                // check if dest file exists and act accordingly
                if (existsMode != FtpRemoteExists.NoCheck)
                {
                    bool destExists = await FileExistsAsync(dest, token);
                    if (destExists)
                    {
                        switch (existsMode)
                        {
                            case FtpRemoteExists.Overwrite:
                                await DeleteFileAsync(dest, token);
                                break;

                            case FtpRemoteExists.Skip:
                                return false;
                        }
                    }
                }

                // move the file
                await RenameAsync(path, dest, token);

                return true;
            }

            return false;
        }
#endif

        #endregion
        #endregion // FtpClient_FileManagement.cs
        #region // FtpClient_FilePermissions.cs
        #region File Permissions / Chmod

        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="permissions">The permissions in CHMOD format</param>
        public void SetFilePermissions(string path, int permissions)
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }
            lock (m_lock)
            {
                path = path.GetFtpPath();

                LogFunc(nameof(SetFilePermissions), new object[] { path, permissions });

                if (!(reply = Execute("SITE CHMOD " + permissions.ToString() + " " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
        }

#if !NET40
        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="permissions">The permissions in CHMOD format</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task SetFilePermissionsAsync(string path, int permissions, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(SetFilePermissionsAsync), new object[] { path, permissions });

            if (!(reply = await ExecuteAsync("SITE CHMOD " + permissions.ToString() + " " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }
        }
#endif

        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="permissions">The permissions in CHMOD format</param>
        public void Chmod(string path, int permissions)
        {
            SetFilePermissions(path, permissions);
        }

#if !NET40
        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="permissions">The permissions in CHMOD format</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task ChmodAsync(string path, int permissions, CancellationToken token = default(CancellationToken))
        {
            return SetFilePermissionsAsync(path, permissions, token);
        }
#endif

        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="owner">The owner permissions</param>
        /// <param name="group">The group permissions</param>
        /// <param name="other">The other permissions</param>
        public void SetFilePermissions(string path, FtpPermission owner, FtpPermission group, FtpPermission other)
        {
            SetFilePermissions(path, Permissions.CalcChmod(owner, group, other));
        }

#if !NET40
        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="owner">The owner permissions</param>
        /// <param name="group">The group permissions</param>
        /// <param name="other">The other permissions</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task SetFilePermissionsAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default(CancellationToken))
        {
            return SetFilePermissionsAsync(path, Permissions.CalcChmod(owner, group, other), token);
        }
#endif

        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="owner">The owner permissions</param>
        /// <param name="group">The group permissions</param>
        /// <param name="other">The other permissions</param>
        public void Chmod(string path, FtpPermission owner, FtpPermission group, FtpPermission other)
        {
            SetFilePermissions(path, owner, group, other);
        }

#if !NET40
        /// <summary>
        /// Modify the permissions of the given file/folder.
        /// Only works on *NIX systems, and not on Windows/IIS servers.
        /// Only works if the FTP server supports the SITE CHMOD command
        /// (requires the CHMOD extension to be installed and enabled).
        /// Throws FtpCommandException if there is an issue.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="owner">The owner permissions</param>
        /// <param name="group">The group permissions</param>
        /// <param name="other">The other permissions</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task ChmodAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default(CancellationToken))
        {
            return SetFilePermissionsAsync(path, owner, group, other, token);
        }
#endif

        /// <summary>
        /// Retrieve the permissions of the given file/folder as an FtpListItem object with all "Permission" properties set.
        /// Throws FtpCommandException if there is an issue.
        /// Returns null if the server did not specify a permission value.
        /// Use `GetChmod` if you required the integer value instead.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        public FtpListItem GetFilePermissions(string path)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetFilePermissions), new object[] { path });

            var result = GetObjectInfo(path);

            return result;
        }

#if !NET40
        /// <summary>
        /// Retrieve the permissions of the given file/folder as an FtpListItem object with all "Permission" properties set.
        /// Throws FtpCommandException if there is an issue.
        /// Returns null if the server did not specify a permission value.
        /// Use `GetChmod` if you required the integer value instead.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task<FtpListItem> GetFilePermissionsAsync(string path, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetFilePermissionsAsync), new object[] { path });

            var result = await GetObjectInfoAsync(path, false, token);

            return result;
        }
#endif

        /// <summary>
        /// Retrieve the permissions of the given file/folder as an integer in the CHMOD format.
        /// Throws FtpCommandException if there is an issue.
        /// Returns 0 if the server did not specify a permission value.
        /// Use `GetFilePermissions` if you required the permissions in the FtpPermission format.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        public int GetChmod(string path)
        {
            var item = GetFilePermissions(path);
            return item != null ? item.Chmod : 0;
        }

#if !NET40
        /// <summary>
        /// Retrieve the permissions of the given file/folder as an integer in the CHMOD format.
        /// Throws FtpCommandException if there is an issue.
        /// Returns 0 if the server did not specify a permission value.
        /// Use `GetFilePermissions` if you required the permissions in the FtpPermission format.
        /// </summary>
        /// <param name="path">The full or relative path to the item</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task<int> GetChmodAsync(string path, CancellationToken token = default(CancellationToken))
        {
            FtpListItem item = await GetFilePermissionsAsync(path, token);
            return item != null ? item.Chmod : 0;
        }
#endif

        #endregion
        #endregion // FtpClient_FilePermissions.cs
        #region // FtpClient_FileProperties.cs
        #region Dereference Link

        /// <summary>
        /// Recursively dereferences a symbolic link. See the
        /// MaximumDereferenceCount property for controlling
        /// how deep this method will recurse before giving up.
        /// </summary>
        /// <param name="item">The symbolic link</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        public FtpListItem DereferenceLink(FtpListItem item)
        {
            return DereferenceLink(item, MaximumDereferenceCount);
        }

        /// <summary>
        /// Recursively dereferences a symbolic link
        /// </summary>
        /// <param name="item">The symbolic link</param>
        /// <param name="recMax">The maximum depth of recursion that can be performed before giving up.</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        public FtpListItem DereferenceLink(FtpListItem item, int recMax)
        {
            LogFunc(nameof(DereferenceLink), new object[] { item.FullName, recMax });

            var count = 0;
            return DereferenceLink(item, recMax, ref count);
        }

        /// <summary>
        /// Dereference a FtpListItem object
        /// </summary>
        /// <param name="item">The item to dereference</param>
        /// <param name="recMax">Maximum recursive calls</param>
        /// <param name="count">Counter</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        private FtpListItem DereferenceLink(FtpListItem item, int recMax, ref int count)
        {
            if (item.Type != FtpFileSystemObjectType.Link)
            {
                throw new FtpException("You can only dereference a symbolic link. Please verify the item type is Link.");
            }

            if (item.LinkTarget == null)
            {
                throw new FtpException("The link target was null. Please check this before trying to dereference the link.");
            }

            foreach (var obj in GetListing(item.LinkTarget.GetFtpDirectoryName()))
            {
                if (item.LinkTarget == obj.FullName)
                {
                    if (obj.Type == FtpFileSystemObjectType.Link)
                    {
                        if (++count == recMax)
                        {
                            return null;
                        }

                        return DereferenceLink(obj, recMax, ref count);
                    }

                    if (HasFeature(FtpCapability.MDTM))
                    {
                        var modify = GetModifiedTime(obj.FullName);

                        if (modify != DateTime.MinValue)
                        {
                            obj.Modified = modify;
                        }
                    }

                    if (obj.Type == FtpFileSystemObjectType.File && obj.Size < 0 && HasFeature(FtpCapability.SIZE))
                    {
                        obj.Size = GetFileSize(obj.FullName);
                    }

                    return obj;
                }
            }

            return null;
        }

#if !NET40
        /// <summary>
        /// Dereference a FtpListItem object
        /// </summary>
        /// <param name="item">The item to dereference</param>
        /// <param name="recMax">Maximum recursive calls</param>
        /// <param name="count">Counter</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        private async Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, int recMax, IntRef count, CancellationToken token = default(CancellationToken))
        {
            if (item.Type != FtpFileSystemObjectType.Link)
            {
                throw new FtpException("You can only dereference a symbolic link. Please verify the item type is Link.");
            }

            if (item.LinkTarget == null)
            {
                throw new FtpException("The link target was null. Please check this before trying to dereference the link.");
            }
            var listing = await GetListingAsync(item.LinkTarget.GetFtpDirectoryName(), token);
            foreach (FtpListItem obj in listing)
            {
                if (item.LinkTarget == obj.FullName)
                {
                    if (obj.Type == FtpFileSystemObjectType.Link)
                    {
                        if (++count.Value == recMax)
                        {
                            return null;
                        }

                        return await DereferenceLinkAsync(obj, recMax, count, token);
                    }

                    if (HasFeature(FtpCapability.MDTM))
                    {
                        var modify = GetModifiedTime(obj.FullName);

                        if (modify != DateTime.MinValue)
                        {
                            obj.Modified = modify;
                        }
                    }

                    if (obj.Type == FtpFileSystemObjectType.File && obj.Size < 0 && HasFeature(FtpCapability.SIZE))
                    {
                        obj.Size = GetFileSize(obj.FullName);
                    }

                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Dereference a <see cref="FtpListItem"/> object asynchronously
        /// </summary>
        /// <param name="item">The item to dereference</param>
        /// <param name="recMax">Maximum recursive calls</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        public Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, int recMax, CancellationToken token = default(CancellationToken))
        {
            LogFunc(nameof(DereferenceLinkAsync), new object[] { item.FullName, recMax });

            var count = new IntRef { Value = 0 };
            return DereferenceLinkAsync(item, recMax, count, token);
        }

        /// <summary>
        /// Dereference a <see cref="FtpListItem"/> object asynchronously
        /// </summary>
        /// <param name="item">The item to dereference</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>FtpListItem, null if the link can't be dereferenced</returns>
        public Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, CancellationToken token = default(CancellationToken))
        {
            return DereferenceLinkAsync(item, MaximumDereferenceCount, token);
        }
#endif

        #endregion

        #region Get File Size

        /// <summary>
        /// Gets the size of a remote file, in bytes.
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="defaultValue">Value to return if there was an error obtaining the file size, or if the file does not exist</param>
        /// <returns>The size of the file, or defaultValue if there was a problem.</returns>
        public virtual long GetFileSize(string path, long defaultValue = -1)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetFileSize), new object[] { path });

            // execute server-specific file size fetching logic, if any
            if (ServerHandler != null && ServerHandler.IsCustomFileSize())
            {
                return ServerHandler.GetFileSize(this, path);
            }

            if (!HasFeature(FtpCapability.SIZE))
            {
                return defaultValue;
            }

            var sizeReply = new FtpSizeReply();
            lock (m_lock)
            {
                GetFileSizeInternal(path, sizeReply, defaultValue);
            }
            return sizeReply.FileSize;
        }

        /// <summary>
        /// Gets the file size of an object, without locking
        /// </summary>
        private void GetFileSizeInternal(string path, FtpSizeReply sizeReply, long defaultValue)
        {
            long length = defaultValue;

            path = path.GetFtpPath();

            // Fix #137: Switch to binary mode since some servers don't support SIZE command for ASCII files.
            if (_FileSizeASCIINotSupported)
            {
                SetDataTypeNoLock(FtpDataType.Binary);
            }

            // execute the SIZE command
            var reply = Execute("SIZE " + path);
            sizeReply.Reply = reply;
            if (!reply.Success)
            {
                length = defaultValue;

                // Fix #137: FTP server returns 'SIZE not allowed in ASCII mode'
                if (!_FileSizeASCIINotSupported && reply.Message.IsKnownError(FtpServerStrings.fileSizeNotInASCII))
                {
                    // set the flag so mode switching is done
                    _FileSizeASCIINotSupported = true;

                    // retry getting the file size
                    GetFileSizeInternal(path, sizeReply, defaultValue);
                    return;
                }
            }
            else if (!long.TryParse(reply.Message, out length))
            {
                length = defaultValue;
            }

            sizeReply.FileSize = length;
        }

#if !NET40
        /// <summary>
        /// Asynchronously gets the size of a remote file, in bytes.
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="defaultValue">Value to return if there was an error obtaining the file size, or if the file does not exist</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>The size of the file, or defaultValue if there was a problem.</returns>
        public async Task<long> GetFileSizeAsync(string path, long defaultValue = -1, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetFileSizeAsync), new object[] { path, defaultValue });

            // execute server-specific file size fetching logic, if any
            if (ServerHandler != null && ServerHandler.IsCustomFileSize())
            {
                return await ServerHandler.GetFileSizeAsync(this, path, token);
            }

            if (!HasFeature(FtpCapability.SIZE))
            {
                return defaultValue;
            }

            FtpSizeReply sizeReply = new FtpSizeReply();
            await GetFileSizeInternalAsync(path, defaultValue, token, sizeReply);

            return sizeReply.FileSize;
        }

        /// <summary>
        /// Gets the file size of an object, without locking
        /// </summary>
        private async Task GetFileSizeInternalAsync(string path, long defaultValue, CancellationToken token, FtpSizeReply sizeReply)
        {
            long length = defaultValue;

            path = path.GetFtpPath();

            // Fix #137: Switch to binary mode since some servers don't support SIZE command for ASCII files.
            if (_FileSizeASCIINotSupported)
            {
                await SetDataTypeNoLockAsync(FtpDataType.Binary, token);
            }

            // execute the SIZE command
            var reply = await ExecuteAsync("SIZE " + path, token);
            sizeReply.Reply = reply;
            if (!reply.Success)
            {
                sizeReply.FileSize = defaultValue;

                // Fix #137: FTP server returns 'SIZE not allowed in ASCII mode'
                if (!_FileSizeASCIINotSupported && reply.Message.IsKnownError(FtpServerStrings.fileSizeNotInASCII))
                {
                    // set the flag so mode switching is done
                    _FileSizeASCIINotSupported = true;

                    // retry getting the file size
                    await GetFileSizeInternalAsync(path, defaultValue, token, sizeReply);
                    return;
                }
            }
            else if (!long.TryParse(reply.Message, out length))
            {
                length = defaultValue;
            }

            sizeReply.FileSize = length;

            return;
        }


#endif
        #endregion

        #region Get Modified Time

        /// <summary>
        /// Gets the modified time of a remote file.
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The modified time, or <see cref="DateTime.MinValue"/> if there was a problem</returns>
        public virtual DateTime GetModifiedTime(string path)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetModifiedTime), new object[] { path });

            var date = DateTime.MinValue;
            FtpReply reply;
            lock (m_lock)
            {
                // get modified date of a file
                if ((reply = Execute("MDTM " + path)).Success)
                {
                    date = reply.Message.ParseFtpDate(this);
                    date = ConvertDate(date);
                }
            }
            return date;
        }

#if !NET40
        /// <summary>
        /// Gets the modified time of a remote file asynchronously
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>The modified time, or <see cref="DateTime.MinValue"/> if there was a problem</returns>
        public async Task<DateTime> GetModifiedTimeAsync(string path, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetModifiedTimeAsync), new object[] { path });

            var date = DateTime.MinValue;
            FtpReply reply;

            // get modified date of a file
            if ((reply = await ExecuteAsync("MDTM " + path, token)).Success)
            {
                date = reply.Message.ParseFtpDate(this);
                date = ConvertDate(date);
            }

            return date;
        }
#endif

        #endregion

        #region Set Modified Time

        /// <summary>
        /// Changes the modified time of a remote file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <param name="date">The new modified date/time value</param>
        public virtual void SetModifiedTime(string path, DateTime date)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(SetModifiedTime), new object[] { path, date });

            FtpReply reply;
            lock (m_lock)
            {
                // calculate the final date string with the timezone conversion
                date = ConvertDate(date, true);
                var timeStr = date.GenerateFtpDate();

                // set modified date of a file
                if ((reply = Execute("MFMT " + timeStr + " " + path)).Success)
                {
                }
            }
        }

#if !NET40
        /// <summary>
        /// Gets the modified time of a remote file asynchronously
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <param name="date">The new modified date/time value</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task SetModifiedTimeAsync(string path, DateTime date, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(SetModifiedTimeAsync), new object[] { path, date });

            FtpReply reply;

            // calculate the final date string with the timezone conversion
            date = ConvertDate(date, true);
            var timeStr = date.GenerateFtpDate();

            // set modified date of a file
            if ((reply = await ExecuteAsync("MFMT " + timeStr + " " + path, token)).Success)
            {
            }
        }
#endif

        #endregion
        #endregion // FtpClient_FileProperties
        #region // FtpClient_FileUpload.cs
        #region Upload Multiple Files

        /// <summary>
        /// Uploads the given file paths to a single folder on the server.
        /// All files are placed directly into the given folder regardless of their path on the local filesystem.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// Faster than uploading single files with <see cref="o:UploadFile"/> since it performs a single "file exists" check rather than one check per file.
        /// </summary>
        /// <param name="localPaths">The full or relative paths to the files on the local file system. Files can be from multiple folders.</param>
        /// <param name="remoteDir">The full or relative path to the directory that files will be uploaded on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to <see cref="FtpRemoteExists.NoCheck"/> for fastest performance,
        ///  but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist.</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="errorHandling">Used to determine how errors are handled</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        /// <returns>The count of how many files were uploaded successfully. Affected when files are skipped when they already exist.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted the existsMode will automatically be set to <see cref="FtpRemoteExists.Overwrite"/>.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        public int UploadFiles(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true,
            FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null)
        {

            // verify args
            if (!errorHandling.IsValidCombination())
            {
                throw new ArgumentException("Invalid combination of FtpError flags.  Throw & Stop cannot be combined");
            }

            if (remoteDir.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteDir");
            }

            remoteDir = remoteDir.GetFtpPath();

            LogFunc(nameof(UploadFiles), new object[] { localPaths, remoteDir, existsMode, createRemoteDir, verifyOptions, errorHandling });

            //int count = 0;
            var errorEncountered = false;
            var successfulUploads = new List<string>();

            // ensure ends with slash if remote is not PDS (MVS Dataset)
            bool isPDS = false;
            if (remoteDir.StartsWith("'") && ServerType == FtpServer.IBMzOSFTP)
            {
                isPDS = true;
            }
            else
            {
                remoteDir = !remoteDir.EndsWith("/") ? remoteDir + "/" : remoteDir;
            }

            //flag to determine if existence checks are required
            var checkFileExistence = true;

            // create remote dir if wanted
            if (createRemoteDir)
            {
                if (!DirectoryExists(remoteDir))
                {
                    CreateDirectory(remoteDir);
                    checkFileExistence = false;
                }
            }

            // get all the already existing files
            var existingFiles = checkFileExistence ? GetNameListing(remoteDir) : new string[0];

            // per local file
            var r = -1;
            foreach (var localPath in localPaths)
            {
                r++;

                // calc remote path
                var fileName = Path.GetFileName(localPath);
                var remotePath = "";

                if (isPDS)
                {
                    // STOR cmd is intelligent enough to determine the full path
                    // and if it needs to append the ".filename" or "(filename)"
                    // addition to the remote path internally for Dsorgs.
                    remotePath = fileName;
                }
                else
                {
                    remotePath = remoteDir + fileName;
                }

                // create meta progress to store the file progress
                var metaProgress = new FtpProgress(localPaths.Count(), r);

                // try to upload it
                try
                {
                    var ok = UploadFileFromFile(localPath, remotePath, false, existsMode, FileListings.FileExistsInNameListing(existingFiles, remotePath), true, verifyOptions, progress, metaProgress);
                    if (ok.IsSuccess())
                    {
                        successfulUploads.Add(remotePath);

                        //count++;
                    }
                    else if ((int)errorHandling > 1)
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogStatus(FtpTraceLevel.Error, "Upload Failure for " + localPath + ": " + ex);
                    if (errorHandling.HasFlag(FtpError.Stop))
                    {
                        errorEncountered = true;
                        break;
                    }

                    if (errorHandling.HasFlag(FtpError.Throw))
                    {
                        if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                        {
                            PurgeSuccessfulUploads(successfulUploads);
                        }

                        throw new FtpException("An error occurred uploading file(s).  See inner exception for more info.", ex);
                    }
                }
            }

            if (errorEncountered)
            {
                //Delete any successful uploads if needed
                if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                {
                    PurgeSuccessfulUploads(successfulUploads);
                    successfulUploads.Clear(); //forces return of 0
                }

                //Throw generic error because requested
                if (errorHandling.HasFlag(FtpError.Throw))
                {
                    throw new FtpException("An error occurred uploading one or more files.  Refer to trace output if available.");
                }
            }

            return successfulUploads.Count;
        }

        private void PurgeSuccessfulUploads(IEnumerable<string> remotePaths)
        {
            foreach (var remotePath in remotePaths)
            {
                DeleteFile(remotePath);
            }
        }

        /// <summary>
        /// Uploads the given file paths to a single folder on the server.
        /// All files are placed directly into the given folder regardless of their path on the local filesystem.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// Faster than uploading single files with <see cref="o:UploadFile"/> since it performs a single "file exists" check rather than one check per file.
        /// </summary>
        /// <param name="localFiles">Files to be uploaded</param>
        /// <param name="remoteDir">The full or relative path to the directory that files will be uploaded on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to FtpExists.None for fastest performance but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist.</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="errorHandling">Used to determine how errors are handled</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        /// <returns>The count of how many files were downloaded successfully. When existing files are skipped, they are not counted.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted the existsMode will automatically be set to <see cref="FtpRemoteExists.Overwrite"/>.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        public int UploadFiles(IEnumerable<FileInfo> localFiles, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true,
            FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, Action<FtpProgress> progress = null)
        {
            return UploadFiles(localFiles.Select(f => f.FullName), remoteDir, existsMode, createRemoteDir, verifyOptions, errorHandling, progress);
        }

#if !NET40
        /// <summary>
        /// Uploads the given file paths to a single folder on the server asynchronously.
        /// All files are placed directly into the given folder regardless of their path on the local filesystem.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// Faster than uploading single files with <see cref="o:UploadFile"/> since it performs a single "file exists" check rather than one check per file.
        /// </summary>
        /// <param name="localPaths">The full or relative paths to the files on the local file system. Files can be from multiple folders.</param>
        /// <param name="remoteDir">The full or relative path to the directory that files will be uploaded on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to FtpExists.None for fastest performance but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist.</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="errorHandling">Used to determine how errors are handled</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <returns>The count of how many files were uploaded successfully. Affected when files are skipped when they already exist.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted the existsMode will automatically be set to <see cref="FtpRemoteExists.Overwrite"/>.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        public async Task<int> UploadFilesAsync(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true,
            FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default(CancellationToken), IProgress<FtpProgress> progress = null)
        {

            // verify args
            if (!errorHandling.IsValidCombination())
            {
                throw new ArgumentException("Invalid combination of FtpError flags.  Throw & Stop cannot be combined");
            }

            if (remoteDir.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteDir");
            }

            remoteDir = remoteDir.GetFtpPath();

            LogFunc(nameof(UploadFilesAsync), new object[] { localPaths, remoteDir, existsMode, createRemoteDir, verifyOptions, errorHandling });

            //check if cancellation was requested and throw to set TaskStatus state to Canceled
            token.ThrowIfCancellationRequested();

            //int count = 0;
            var errorEncountered = false;
            var successfulUploads = new List<string>();

            // ensure ends with slash if remote is not PDS (MVS Dataset)
            bool isPDS = false;
            if (remoteDir.StartsWith("'") && ServerType == FtpServer.IBMzOSFTP)
            {
                isPDS = true;
            }
            else
            {
                remoteDir = !remoteDir.EndsWith("/") ? remoteDir + "/" : remoteDir;
            }

            //flag to determine if existence checks are required
            var checkFileExistence = true;

            // create remote dir if wanted
            if (createRemoteDir)
            {
                if (!await DirectoryExistsAsync(remoteDir, token))
                {
                    await CreateDirectoryAsync(remoteDir, token);
                    checkFileExistence = false;
                }
            }

            // get all the already existing files (if directory was created just create an empty array)
            var existingFiles = checkFileExistence ? await GetNameListingAsync(remoteDir, token) : new string[0];

            // per local file
            var r = -1;
            foreach (var localPath in localPaths)
            {
                r++;

                // check if cancellation was requested and throw to set TaskStatus state to Canceled
                token.ThrowIfCancellationRequested();

                // calc remote path
                var fileName = Path.GetFileName(localPath);
                var remotePath = "";

                if (isPDS)
                {
                    // STOR cmd is intelligent enough to determine the full path
                    // and if it needs to append the ".filename" or "(filename)"
                    // addition to the remote path internally for Dsorgs.
                    remotePath = fileName;
                }
                else
                {
                    remotePath = remoteDir + fileName;
                }

                // create meta progress to store the file progress
                var metaProgress = new FtpProgress(localPaths.Count(), r);

                // try to upload it
                try
                {
                    var ok = await UploadFileFromFileAsync(localPath, remotePath, false, existsMode, FileListings.FileExistsInNameListing(existingFiles, remotePath), true, verifyOptions, token, progress, metaProgress);
                    if (ok.IsSuccess())
                    {
                        successfulUploads.Add(remotePath);
                    }
                    else if ((int)errorHandling > 1)
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        //DO NOT SUPPRESS CANCELLATION REQUESTS -- BUBBLE UP!
                        LogStatus(FtpTraceLevel.Info, "Upload cancellation requested");
                        throw;
                    }

                    //suppress all other upload exceptions (errors are still written to FtpTrace)
                    LogStatus(FtpTraceLevel.Error, "Upload Failure for " + localPath + ": " + ex);
                    if (errorHandling.HasFlag(FtpError.Stop))
                    {
                        errorEncountered = true;
                        break;
                    }

                    if (errorHandling.HasFlag(FtpError.Throw))
                    {
                        if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                        {
                            PurgeSuccessfulUploads(successfulUploads);
                        }

                        throw new FtpException("An error occurred uploading file(s).  See inner exception for more info.", ex);
                    }
                }
            }

            if (errorEncountered)
            {
                //Delete any successful uploads if needed
                if (errorHandling.HasFlag(FtpError.DeleteProcessed))
                {
                    await PurgeSuccessfulUploadsAsync(successfulUploads);
                    successfulUploads.Clear(); //forces return of 0
                }

                //Throw generic error because requested
                if (errorHandling.HasFlag(FtpError.Throw))
                {
                    throw new FtpException("An error occurred uploading one or more files.  Refer to trace output if available.");
                }
            }

            return successfulUploads.Count;
        }

        private async Task PurgeSuccessfulUploadsAsync(IEnumerable<string> remotePaths)
        {
            foreach (var remotePath in remotePaths)
            {
                await DeleteFileAsync(remotePath);
            }
        }
#endif

        #endregion

        #region Upload File

        /// <summary>
        /// Uploads the specified file directly onto the server.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to  <see cref="FtpRemoteExists.NoCheck"/> for fastest performance 
        /// but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <returns>FtpStatus flag indicating if the file was uploaded, skipped or failed to transfer.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted the existsMode will automatically be set to <see cref="FtpRemoteExists.Overwrite"/>.
        /// </remarks>
        public FtpStatus UploadFile(string localPath, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false,
            FtpVerify verifyOptions = FtpVerify.None, Action<FtpProgress> progress = null)
        {
            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            return UploadFileFromFile(localPath, remotePath, createRemoteDir, existsMode, false, false, verifyOptions, progress, new FtpProgress(1, 0));
        }

        private FtpStatus UploadFileFromFile(string localPath, string remotePath, bool createRemoteDir, FtpRemoteExists existsMode,
            bool fileExists, bool fileExistsKnown, FtpVerify verifyOptions, Action<FtpProgress> progress, FtpProgress metaProgress)
        {

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(UploadFile), new object[] { localPath, remotePath, existsMode, createRemoteDir, verifyOptions });

            // skip uploading if the local file does not exist
            if (!File.Exists(localPath))
            {
                LogStatus(FtpTraceLevel.Error, "File does not exist.");
                return FtpStatus.Failed;
            }

            // If retries are allowed set the retry counter to the allowed count
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;

            // Default validation to true (if verification isn't needed it'll allow a pass-through)
            var verified = true;
            FtpStatus uploadStatus;
            bool uploadSuccess;
            do
            {
                // write the file onto the server
                using (var fileStream = FtpFileStream.GetFileReadStream(this, localPath, false, QuickTransferLimit))
                {
                    // Upload file
                    uploadStatus = UploadFileInternal(fileStream, localPath, remotePath, createRemoteDir, existsMode, fileExists, fileExistsKnown, progress, metaProgress);
                    uploadSuccess = uploadStatus.IsSuccess();
                    attemptsLeft--;

                    if (!uploadSuccess)
                    {
                        LogStatus(FtpTraceLevel.Info, "Failed to upload file.");

                        if (attemptsLeft > 0)
                            LogStatus(FtpTraceLevel.Info, "Retrying to upload file.");
                    }

                    // If verification is needed, update the validated flag
                    if (uploadSuccess && verifyOptions != FtpVerify.None)
                    {
                        verified = VerifyTransfer(localPath, remotePath);
                        LogStatus(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                        if (!verified && attemptsLeft > 0)
                        {
                            LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode != FtpRemoteExists.Overwrite ? "  Switching to FtpExists.Overwrite mode.  " : "  ") + attemptsLeft + " attempts remaining");
                            // Force overwrite if a retry is required
                            existsMode = FtpRemoteExists.Overwrite;
                        }
                    }
                }
            } while ((!uploadSuccess || !verified) && attemptsLeft > 0); //Loop if attempts are available and the transfer or validation failed

            if (uploadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                DeleteFile(remotePath);
            }

            if (uploadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Uploaded file checksum value does not match local file");
            }

            // if uploaded OK then correctly return Skipped or Success, else return Failed
            return uploadSuccess && verified ? uploadStatus : FtpStatus.Failed;
        }

#if !NET40
        /// <summary>
        /// Uploads the specified file directly onto the server asynchronously.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="localPath">The full or relative path to the file on the local file system</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to  <see cref="FtpRemoteExists.NoCheck"/> for fastest performance
        ///  but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="token">The token that can be used to cancel the entire process.</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <returns>FtpStatus flag indicating if the file was uploaded, skipped or failed to transfer.</returns>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted the existsMode will automatically be set to <see cref="FtpRemoteExists.Overwrite"/>.
        /// </remarks>
        public async Task<FtpStatus> UploadFileAsync(string localPath, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            return await UploadFileFromFileAsync(localPath, remotePath, createRemoteDir, existsMode, false, false, verifyOptions, token, progress, new FtpProgress(1, 0));
        }

        private async Task<FtpStatus> UploadFileFromFileAsync(string localPath, string remotePath, bool createRemoteDir, FtpRemoteExists existsMode,
            bool fileExists, bool fileExistsKnown, FtpVerify verifyOptions, CancellationToken token, IProgress<FtpProgress> progress, FtpProgress metaProgress)
        {


            // skip uploading if the local file does not exist
#if NETFx
            if (!await Task.Run(() => File.Exists(localPath), token))
            {
#else
            if (!File.Exists(localPath))
            {
#endif
                LogStatus(FtpTraceLevel.Error, "File does not exist.");
                return FtpStatus.Failed;
            }

            LogFunc(nameof(UploadFileAsync), new object[] { localPath, remotePath, existsMode, createRemoteDir, verifyOptions });

            // If retries are allowed set the retry counter to the allowed count
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;

            // Default validation to true (if verification isn't needed it'll allow a pass-through)
            var verified = true;
            FtpStatus uploadStatus;
            bool uploadSuccess;
            do
            {
                // write the file onto the server
                using (var fileStream = FtpFileStream.GetFileReadStream(this, localPath, true, QuickTransferLimit))
                {
                    uploadStatus = await UploadFileInternalAsync(fileStream, localPath, remotePath, createRemoteDir, existsMode, fileExists, fileExistsKnown, progress, token, metaProgress);
                    uploadSuccess = uploadStatus.IsSuccess();
                    attemptsLeft--;

                    if (!uploadSuccess)
                    {
                        LogStatus(FtpTraceLevel.Info, "Failed to upload file.");

                        if (attemptsLeft > 0)
                            LogStatus(FtpTraceLevel.Info, "Retrying to upload file.");
                    }

                    // If verification is needed, update the validated flag
                    if (verifyOptions != FtpVerify.None)
                    {
                        verified = await VerifyTransferAsync(localPath, remotePath, token);
                        LogStatus(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                        if (!verified && attemptsLeft > 0)
                        {
                            LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode != FtpRemoteExists.Overwrite ? "  Switching to FtpExists.Overwrite mode.  " : "  ") + attemptsLeft + " attempts remaining");
                            // Force overwrite if a retry is required
                            existsMode = FtpRemoteExists.Overwrite;
                        }
                    }
                }
            } while ((!uploadSuccess || !verified) && attemptsLeft > 0);

            if (uploadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                await DeleteFileAsync(remotePath, token);
            }

            if (uploadSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Uploaded file checksum value does not match local file");
            }

            // if uploaded OK then correctly return Skipped or Success, else return Failed
            return uploadSuccess && verified ? uploadStatus : FtpStatus.Failed;
        }

#endif

        #endregion

        #region	Upload Bytes/Stream

        /// <summary>
        /// Uploads the specified stream as a file onto the server.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="fileStream">The full data of the file, as a stream</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to <see cref="FtpRemoteExists.NoCheck"/> for fastest performance
        /// but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        public FtpStatus Upload(Stream fileStream, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, Action<FtpProgress> progress = null)
        {
            // verify args
            if (fileStream == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "fileStream");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(Upload), new object[] { remotePath, existsMode, createRemoteDir });

            // write the file onto the server
            return UploadFileInternal(fileStream, null, remotePath, createRemoteDir, existsMode, false, false, progress, new FtpProgress(1, 0));
        }

        /// <summary>
        /// Uploads the specified byte array as a file onto the server.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="fileData">The full data of the file, as a byte array</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to <see cref="FtpRemoteExists.NoCheck"/> for fastest performance 
        /// but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        public FtpStatus Upload(byte[] fileData, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, Action<FtpProgress> progress = null)
        {
            // verify args
            if (fileData == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "fileData");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(Upload), new object[] { remotePath, existsMode, createRemoteDir });

            // write the file onto the server
            using (var ms = new MemoryStream(fileData))
            {
                ms.Position = 0;
                return UploadFileInternal(ms, null, remotePath, createRemoteDir, existsMode, false, false, progress, new FtpProgress(1, 0));
            }
        }


#if !NET40
        /// <summary>
        /// Uploads the specified stream as a file onto the server asynchronously.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="fileStream">The full data of the file, as a stream</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to <see cref="FtpRemoteExists.NoCheck"/> for fastest performance,
        ///  but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="token">The token that can be used to cancel the entire process.</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <returns>FtpStatus flag indicating if the file was uploaded, skipped or failed to transfer.</returns>
        public async Task<FtpStatus> UploadAsync(Stream fileStream, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (fileStream == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "fileStream");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(UploadAsync), new object[] { remotePath, existsMode, createRemoteDir });

            // write the file onto the server
            return await UploadFileInternalAsync(fileStream, null, remotePath, createRemoteDir, existsMode, false, false, progress, token, new FtpProgress(1, 0));
        }

        /// <summary>
        /// Uploads the specified byte array as a file onto the server asynchronously.
        /// High-level API that takes care of various edge cases internally.
        /// Supports very large files since it uploads data in chunks.
        /// </summary>
        /// <param name="fileData">The full data of the file, as a byte array</param>
        /// <param name="remotePath">The full or relative path to the file on the server</param>
        /// <param name="existsMode">What to do if the file already exists? Skip, overwrite or append? Set this to <see cref="FtpRemoteExists.NoCheck"/> for fastest performance,
        ///  but only if you are SURE that the files do not exist on the server.</param>
        /// <param name="createRemoteDir">Create the remote directory if it does not exist. Slows down upload due to additional checks required.</param>
        /// <param name="token">The token that can be used to cancel the entire process.</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <returns>FtpStatus flag indicating if the file was uploaded, skipped or failed to transfer.</returns>
        public async Task<FtpStatus> UploadAsync(byte[] fileData, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (fileData == null)
            {
                throw new ArgumentException("Required parameter is null or blank.", "fileData");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(UploadAsync), new object[] { remotePath, existsMode, createRemoteDir });

            // write the file onto the server
            using (var ms = new MemoryStream(fileData))
            {
                ms.Position = 0;
                return await UploadFileInternalAsync(ms, null, remotePath, createRemoteDir, existsMode, false, false, progress, token, new FtpProgress(1, 0));
            }
        }
#endif

        #endregion

        #region Upload File Internal

        /// <summary>
        /// Upload the given stream to the server as a new file. Overwrites the file if it exists.
        /// Writes data in chunks. Retries if server disconnects midway.
        /// </summary>
        private FtpStatus UploadFileInternal(Stream fileData, string localPath, string remotePath, bool createRemoteDir,
            FtpRemoteExists existsMode, bool fileExists, bool fileExistsKnown, Action<FtpProgress> progress, FtpProgress metaProgress)
        {

            Stream upStream = null;

            // throw an error if need to resume uploading and cannot seek the local file stream
            if (!fileData.CanSeek && existsMode == FtpRemoteExists.Resume)
            {
                throw new ArgumentException("You have requested resuming file upload with FtpRemoteExists.Resume, but the local file stream cannot be seeked. Use another type of Stream or another existsMode.", "fileData");
            }

            try
            {
                long localPosition = 0, remotePosition = 0, remoteFileLen = 0;

                // check if the file exists, and skip, overwrite or append
                if (existsMode == FtpRemoteExists.NoCheck)
                {
                }
                else if (existsMode == FtpRemoteExists.ResumeNoCheck || existsMode == FtpRemoteExists.AddToEndNoCheck)
                {

                    // start from the end of the remote file, or if failed to read the length then start from the beginning
                    remoteFileLen = remotePosition = GetFileSize(remotePath, 0);

                    // calculate the local position for appending / resuming
                    localPosition = CalculateAppendLocalPosition(remotePath, existsMode, remotePosition);

                }
                else
                {

                    // check if the remote file exists
                    if (!fileExistsKnown)
                    {
                        fileExists = FileExists(remotePath);
                    }

                    if (existsMode == FtpRemoteExists.Skip)
                    {

                        if (fileExists)
                        {
                            LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists on server (Remote: " + remotePath + ", Local: " + localPath + ")");

                            // Fix #413 - progress callback isn't called if the file has already been uploaded to the server
                            // send progress reports for skipped files
                            if (progress != null)
                            {
                                progress(new FtpProgress(100.0, localPosition, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                            }

                            return FtpStatus.Skipped;
                        }
                    }
                    else if (existsMode == FtpRemoteExists.Overwrite)
                    {

                        // delete the remote file if it exists and we need to overwrite
                        if (fileExists)
                        {
                            DeleteFile(remotePath);
                        }
                    }
                    else if (existsMode == FtpRemoteExists.Resume || existsMode == FtpRemoteExists.AddToEnd)
                    {
                        if (fileExists)
                        {

                            // start from the end of the remote file, or if failed to read the length then start from the beginning
                            remoteFileLen = remotePosition = GetFileSize(remotePath, 0);

                            // calculate the local position for appending / resuming
                            localPosition = CalculateAppendLocalPosition(remotePath, existsMode, remotePosition);
                        }
                    }
                }

                // ensure the remote dir exists .. only if the file does not already exist!
                if (createRemoteDir && !fileExists)
                {
                    var dirname = remotePath.GetFtpDirectoryName();
                    if (!DirectoryExists(dirname))
                    {
                        CreateDirectory(dirname);
                    }
                }

                // FIX #213 : Do not change Stream.Position if not supported
                if (fileData.CanSeek)
                {
                    try
                    {
                        // seek to required offset
                        fileData.Position = localPosition;
                    }
                    catch (Exception) { }
                }

                // calc local file len
                var localFileLen = fileData.Length;

                // skip uploading if the mode is resume and the local and remote file have the same length
                if ((existsMode == FtpRemoteExists.Resume || existsMode == FtpRemoteExists.ResumeNoCheck) &&
                    (localFileLen == remoteFileLen))
                {
                    LogStatus(FtpTraceLevel.Info, "Skipping file because Resume is enabled and file is fully uploaded (Remote: " + remotePath + ", Local: " + localPath + ")");

                    // send progress reports for skipped files
                    if (progress != null)
                    {
                        progress(new FtpProgress(100.0, localPosition, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                    }
                    return FtpStatus.Skipped;
                }

                // open a file connection
                if (remotePosition == 0 && existsMode != FtpRemoteExists.ResumeNoCheck && existsMode != FtpRemoteExists.AddToEndNoCheck)
                {
                    upStream = OpenWrite(remotePath, UploadDataType, remoteFileLen);
                }
                else
                {
                    upStream = OpenAppend(remotePath, UploadDataType, remoteFileLen);
                }

                // calculate chunk size and rate limiting
                const int rateControlResolution = 100;
                var rateLimitBytes = UploadRateLimit != 0 ? (long)UploadRateLimit * 1024 : 0;
                var chunkSize = CalculateTransferChunkSize(rateLimitBytes, rateControlResolution);

                // calc desired length based on the mode (if need to append to the end of remote file, length is sum of local+remote)
                var remoteFileDesiredLen = (existsMode == FtpRemoteExists.AddToEnd || existsMode == FtpRemoteExists.AddToEndNoCheck) ?
                    (upStream.Length + localFileLen)
                    : localFileLen;

                var buffer = new byte[chunkSize];

                var transferStarted = DateTime.Now;
                var sw = new Stopwatch();

                // always set the length of the remote file based on the desired size
                // also fixes #288 - Upload hangs with only a few bytes left
                try
                {
                    upStream.SetLength(remoteFileDesiredLen);
                }
                catch (Exception) { }

                var anyNoop = false;

                // loop till entire file uploaded
                while (localPosition < localFileLen)
                {
                    try
                    {
                        // read a chunk of bytes from the file
                        int readBytes;
                        long limitCheckBytes = 0;
                        long bytesProcessed = 0;

                        sw.Start();
                        while ((readBytes = fileData.Read(buffer, 0, buffer.Length)) > 0)
                        {

                            // write chunk to the FTP stream
                            upStream.Write(buffer, 0, readBytes);
                            upStream.Flush();

                            // move file pointers ahead
                            localPosition += readBytes;
                            remotePosition += readBytes;
                            bytesProcessed += readBytes;
                            limitCheckBytes += readBytes;

                            // send progress reports
                            if (progress != null)
                            {
                                ReportProgress(progress, localFileLen, localPosition, bytesProcessed, DateTime.Now - transferStarted, localPath, remotePath, metaProgress);
                            }

                            // Fix #387: keep alive with NOOP as configured and needed
                            if (!m_threadSafeDataChannels)
                            {
                                anyNoop = Noop() || anyNoop;
                            }

                            // honor the speed limit
                            var swTime = sw.ElapsedMilliseconds;
                            if (rateLimitBytes > 0)
                            {
                                var timeShouldTake = limitCheckBytes * 1000 / rateLimitBytes;
                                if (timeShouldTake > swTime)
                                {
                                    Thread.Sleep((int)(timeShouldTake - swTime)); // Task.Delay((int)(timeShouldTake - swTime)).Wait();
                                }
                                else if (swTime > timeShouldTake + rateControlResolution)
                                {
                                    limitCheckBytes = 0;
                                    sw.Restart();
                                }
                            }
                        }

                        // zero return value (with no Exception) indicates EOS; so we should terminate the outer loop here
                        break;
                    }
                    catch (IOException ex)
                    {

                        // resume if server disconnected midway, or throw if there is an exception doing that as well
                        if (!ResumeUpload(remotePath, ref upStream, remotePosition, ex))
                        {
                            sw.Stop();
                            throw;
                        }

                        // since the remote stream has been seeked, we need to reposition the local stream too
                        if (fileData.CanSeek)
                        {
                            fileData.Seek(localPosition, SeekOrigin.Begin);
                        }
                        else
                        {
                            sw.Stop();
                            throw;
                        }

                    }
                    catch (TimeoutException ex)
                    {
                        // fix: attempting to upload data after we reached the end of the stream
                        // often throws a timeout exception, so we silently absorb that here
                        if (localPosition >= localFileLen)
                        {
                            break;
                        }
                        else
                        {
                            sw.Stop();
                            throw ex;
                        }
                    }
                }

                sw.Stop();


                // wait for transfer to get over
                while (upStream.Position < upStream.Length)
                {
                }

                // send progress reports
                if (progress != null)
                {
                    progress(new FtpProgress(100.0, upStream.Length, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                }

                // disconnect FTP stream before exiting
                upStream.Dispose();

                // FIX : if this is not added, there appears to be "stale data" on the socket
                // listen for a success/failure reply
                try
                {
                    while (!m_threadSafeDataChannels)
                    {
                        var status = GetReply();

                        // Fix #387: exhaust any NOOP responses (not guaranteed during file transfers)
                        if (anyNoop && status.Message != null && status.Message.Contains("NOOP"))
                        {
                            continue;
                        }

                        // Fix #353: if server sends 550 or 5xx the transfer was received but could not be confirmed by the server
                        // Fix #509: if server sends 450 or 4xx the transfer was aborted or failed midway
                        if (status.Code != null && !status.Success)
                        {
                            return FtpStatus.Failed;
                        }

                        // Fix #387: exhaust any NOOP responses also after "226 Transfer complete."
                        if (anyNoop)
                        {
                            ReadStaleData(false, true, true);
                        }

                        break;
                    }
                }

                // absorb "System.TimeoutException: Timed out trying to read data from the socket stream!" at GetReply()
                catch (Exception) { }

                return FtpStatus.Success;
            }
            catch (Exception ex1)
            {
                // close stream before throwing error
                try
                {
                    if (upStream != null)
                    {
                        upStream.Dispose();
                    }
                }
                catch (Exception)
                {
                }

                if (ex1 is IOException)
                {
                    LogStatus(FtpTraceLevel.Verbose, "IOException for file " + localPath + " : " + ex1.Message);
                    return FtpStatus.Failed;
                }

                // catch errors during upload, 
                throw new FtpException("Error while uploading the file to the server. See InnerException for more info.", ex1);
            }
        }

#if !NET40
        /// <summary>
        /// Upload the given stream to the server as a new file asynchronously. Overwrites the file if it exists.
        /// Writes data in chunks. Retries if server disconnects midway.
        /// </summary>
        private async Task<FtpStatus> UploadFileInternalAsync(Stream fileData, string localPath, string remotePath, bool createRemoteDir,
            FtpRemoteExists existsMode, bool fileExists, bool fileExistsKnown, IProgress<FtpProgress> progress, CancellationToken token, FtpProgress metaProgress)
        {

            Stream upStream = null;

            // throw an error if need to resume uploading and cannot seek the local file stream
            if (!fileData.CanSeek && existsMode == FtpRemoteExists.Resume)
            {
                throw new ArgumentException("You have requested resuming file upload with FtpRemoteExists.Resume, but the local file stream cannot be seeked. Use another type of Stream or another existsMode.", "fileData");
            }

            try
            {
                long localPosition = 0, remotePosition = 0, remoteFileLen = 0;

                // check if the file exists, and skip, overwrite or append
                if (existsMode == FtpRemoteExists.NoCheck)
                {
                }
                else if (existsMode == FtpRemoteExists.ResumeNoCheck || existsMode == FtpRemoteExists.AddToEndNoCheck)
                {

                    // start from the end of the remote file, or if failed to read the length then start from the beginning
                    remoteFileLen = remotePosition = await GetFileSizeAsync(remotePath, 0, token);

                    // calculate the local position for appending / resuming
                    localPosition = CalculateAppendLocalPosition(remotePath, existsMode, remotePosition);

                }
                else
                {

                    // check if the remote file exists
                    if (!fileExistsKnown)
                    {
                        fileExists = await FileExistsAsync(remotePath, token);
                    }

                    if (existsMode == FtpRemoteExists.Skip)
                    {

                        if (fileExists)
                        {
                            LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists on server (Remote: " + remotePath + ", Local: " + localPath + ")");

                            // Fix #413 - progress callback isn't called if the file has already been uploaded to the server
                            // send progress reports for skipped files
                            if (progress != null)
                            {
                                progress.Report(new FtpProgress(100.0, localPosition, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                            }

                            return FtpStatus.Skipped;
                        }

                    }
                    else if (existsMode == FtpRemoteExists.Overwrite)
                    {

                        // delete the remote file if it exists and we need to overwrite
                        if (fileExists)
                        {
                            await DeleteFileAsync(remotePath, token);
                        }

                    }
                    else if (existsMode == FtpRemoteExists.Resume || existsMode == FtpRemoteExists.AddToEnd)
                    {
                        if (fileExists)
                        {

                            // start from the end of the remote file, or if failed to read the length then start from the beginning
                            remoteFileLen = remotePosition = await GetFileSizeAsync(remotePath, 0, token);

                            // calculate the local position for appending / resuming
                            localPosition = CalculateAppendLocalPosition(remotePath, existsMode, remotePosition);
                        }

                    }

                }

                // ensure the remote dir exists .. only if the file does not already exist!
                if (createRemoteDir && !fileExists)
                {
                    var dirname = remotePath.GetFtpDirectoryName();
                    if (!await DirectoryExistsAsync(dirname, token))
                    {
                        await CreateDirectoryAsync(dirname, token);
                    }
                }

                // FIX #213 : Do not change Stream.Position if not supported
                if (fileData.CanSeek)
                {
                    try
                    {
                        // seek to required offset
                        fileData.Position = localPosition;
                    }
                    catch (Exception) { }
                }

                // calc local file len
                var localFileLen = fileData.Length;

                // skip uploading if the mode is resume and the local and remote file have the same length
                if ((existsMode == FtpRemoteExists.Resume || existsMode == FtpRemoteExists.ResumeNoCheck) &&
                    (localFileLen == remoteFileLen))
                {
                    LogStatus(FtpTraceLevel.Info, "Skipping file because Resume is enabled and file is fully uploaded (Remote: " + remotePath + ", Local: " + localPath + ")");

                    // send progress reports for skipped files
                    if (progress != null)
                    {
                        progress.Report(new FtpProgress(100.0, localPosition, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                    }
                    return FtpStatus.Skipped;
                }

                // open a file connection
                if (remotePosition == 0 && existsMode != FtpRemoteExists.ResumeNoCheck && existsMode != FtpRemoteExists.AddToEndNoCheck)
                {
                    upStream = await OpenWriteAsync(remotePath, UploadDataType, remoteFileLen, token);
                }
                else
                {
                    upStream = await OpenAppendAsync(remotePath, UploadDataType, remoteFileLen, token);
                }

                // calculate chunk size and rate limiting
                const int rateControlResolution = 100;
                var rateLimitBytes = UploadRateLimit != 0 ? (long)UploadRateLimit * 1024 : 0;
                var chunkSize = CalculateTransferChunkSize(rateLimitBytes, rateControlResolution);

                // calc desired length based on the mode (if need to append to the end of remote file, length is sum of local+remote)
                var remoteFileDesiredLen = (existsMode == FtpRemoteExists.AddToEnd || existsMode == FtpRemoteExists.AddToEndNoCheck) ?
                    (upStream.Length + localFileLen)
                    : localFileLen;

                var buffer = new byte[chunkSize];

                var transferStarted = DateTime.Now;
                var sw = new Stopwatch();

                // always set the length of the remote file based on the desired size
                // also fixes #288 - Upload hangs with only a few bytes left
                try
                {
                    upStream.SetLength(remoteFileDesiredLen);
                }
                catch (Exception) { }

                var anyNoop = false;

                // loop till entire file uploaded
                while (localPosition < localFileLen)
                {
                    try
                    {
                        // read a chunk of bytes from the file
                        int readBytes;
                        long limitCheckBytes = 0;
                        long bytesProcessed = 0;

                        sw.Start();
                        while ((readBytes = await fileData.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                        {
                            // write chunk to the FTP stream
                            await upStream.WriteAsync(buffer, 0, readBytes, token);
                            await upStream.FlushAsync(token);


                            // move file pointers ahead
                            localPosition += readBytes;
                            remotePosition += readBytes;
                            bytesProcessed += readBytes;
                            limitCheckBytes += readBytes;

                            // send progress reports
                            if (progress != null)
                            {
                                ReportProgress(progress, localFileLen, localPosition, bytesProcessed, DateTime.Now - transferStarted, localPath, remotePath, metaProgress);
                            }

                            // Fix #387: keep alive with NOOP as configured and needed
                            if (!m_threadSafeDataChannels)
                            {
                                anyNoop = await NoopAsync(token) || anyNoop;
                            }

                            // honor the rate limit
                            var swTime = sw.ElapsedMilliseconds;
                            if (rateLimitBytes > 0)
                            {
                                var timeShouldTake = limitCheckBytes * 1000 / rateLimitBytes;
                                if (timeShouldTake > swTime)
                                {
                                    await Task.Delay((int)(timeShouldTake - swTime), token);
                                    token.ThrowIfCancellationRequested();
                                }
                                else if (swTime > timeShouldTake + rateControlResolution)
                                {
                                    limitCheckBytes = 0;
                                    sw.Restart();
                                }
                            }
                        }

                        // zero return value (with no Exception) indicates EOS; so we should terminate the outer loop here
                        break;
                    }
                    catch (IOException ex)
                    {

                        // resume if server disconnected midway, or throw if there is an exception doing that as well
                        var resumeResult = await ResumeUploadAsync(remotePath, upStream, remotePosition, ex);
                        if (resumeResult.Item1)
                        {
                            upStream = resumeResult.Item2;

                            // since the remote stream has been seeked, we need to reposition the local stream too
                            if (fileData.CanSeek)
                            {
                                fileData.Seek(localPosition, SeekOrigin.Begin);
                            }
                            else
                            {
                                sw.Stop();
                                throw;
                            }

                        }
                        else
                        {
                            sw.Stop();
                            throw;
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        // fix: attempting to upload data after we reached the end of the stream
                        // often throws a timeout exception, so we silently absorb that here
                        if (localPosition >= localFileLen)
                        {
                            break;
                        }
                        else
                        {
                            sw.Stop();
                            throw ex;
                        }
                    }
                }

                sw.Stop();

                // wait for transfer to get over
                while (upStream.Position < upStream.Length)
                {
                }

                // send progress reports
                if (progress != null)
                {
                    progress.Report(new FtpProgress(100.0, upStream.Length, 0, TimeSpan.FromSeconds(0), localPath, remotePath, metaProgress));
                }

                // disconnect FTP stream before exiting
                upStream.Dispose();

                // FIX : if this is not added, there appears to be "stale data" on the socket
                // listen for a success/failure reply
                try
                {
                    while (!m_threadSafeDataChannels)
                    {
                        FtpReply status = await GetReplyAsync(token);

                        // Fix #387: exhaust any NOOP responses (not guaranteed during file transfers)
                        if (anyNoop && status.Message != null && status.Message.Contains("NOOP"))
                        {
                            continue;
                        }

                        // Fix #353: if server sends 550 or 5xx the transfer was received but could not be confirmed by the server
                        // Fix #509: if server sends 450 or 4xx the transfer was aborted or failed midway
                        if (status.Code != null && !status.Success)
                        {
                            return FtpStatus.Failed;
                        }

                        // Fix #387: exhaust any NOOP responses also after "226 Transfer complete."
                        if (anyNoop)
                        {
                            await ReadStaleDataAsync(false, true, true, token);
                        }

                        break;
                    }
                }

                // absorb "System.TimeoutException: Timed out trying to read data from the socket stream!" at GetReply()
                catch (Exception) { }

                return FtpStatus.Success;
            }
            catch (Exception ex1)
            {
                // close stream before throwing error
                try
                {
                    if (upStream != null)
                    {
                        upStream.Dispose();
                    }
                }
                catch (Exception)
                {
                }

                if (ex1 is IOException)
                {
                    LogStatus(FtpTraceLevel.Verbose, "IOException for file " + localPath + " : " + ex1.Message);
                    return FtpStatus.Failed;
                }

                if (ex1 is OperationCanceledException)
                {
                    LogStatus(FtpTraceLevel.Info, "Upload cancellation requested");
                    throw;
                }

                // catch errors during upload
                throw new FtpException("Error while uploading the file to the server. See InnerException for more info.", ex1);
            }
        }

#endif

        private bool ResumeUpload(string remotePath, ref Stream upStream, long remotePosition, IOException ex)
        {

            // if resume possible
            if (ex.IsResumeAllowed())
            {

                // dispose the old bugged out stream
                upStream.Dispose();

                // create and return a new stream starting at the current remotePosition
                upStream = OpenAppend(remotePath, UploadDataType, 0);
                upStream.Position = remotePosition;
                return true;
            }

            // resume not allowed
            return false;
        }

#if !NET40
        private async Task<Tuple<bool, Stream>> ResumeUploadAsync(string remotePath, Stream upStream, long remotePosition, IOException ex)
        {

            // if resume possible
            if (ex.IsResumeAllowed())
            {

                // dispose the old bugged out stream
                upStream.Dispose();

                // create and return a new stream starting at the current remotePosition
                var returnStream = await OpenAppendAsync(remotePath, UploadDataType, 0);
                returnStream.Position = remotePosition;
                return Tuple.Create(true, returnStream);
            }

            // resume not allowed
            return Tuple.Create(false, (Stream)null);
        }
#endif
        private long CalculateAppendLocalPosition(string remotePath, FtpRemoteExists existsMode, long remotePosition)
        {

            long localPosition = 0;

            // resume - start the local file from the same position as the remote file
            if (existsMode == FtpRemoteExists.Resume || existsMode == FtpRemoteExists.ResumeNoCheck)
            {
                localPosition = remotePosition;
            }

            // append to end - start from the beginning of the local file
            else if (existsMode == FtpRemoteExists.AddToEnd || existsMode == FtpRemoteExists.AddToEndNoCheck)
            {
                localPosition = 0;
            }

            return localPosition;
        }

        #endregion
        #endregion // FtpClient_FileUpload.cs
        #region // FtpClient_FileVerification.cs
        #region Verification

        private bool SupportsChecksum()
        {
            return HasFeature(FtpCapability.HASH) || HasFeature(FtpCapability.MD5) ||
                    HasFeature(FtpCapability.XMD5) || HasFeature(FtpCapability.XCRC) ||
                    HasFeature(FtpCapability.XSHA1) || HasFeature(FtpCapability.XSHA256) ||
                    HasFeature(FtpCapability.XSHA512);
        }

        private bool VerifyTransfer(string localPath, string remotePath)
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }
            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            try
            {
                if (SupportsChecksum())
                {
                    var hash = GetChecksum(remotePath);
                    if (!hash.IsValid)
                    {
                        return false;
                    }

                    return hash.Verify(localPath);
                }

                // not supported, so return true to ignore validation
                return true;
            }
            catch (IOException ex)
            {
                LogStatus(FtpTraceLevel.Warn, "Failed to verify file " + localPath + " : " + ex.Message);
                return false;
            }
        }

#if !NET40
        private async Task<bool> VerifyTransferAsync(string localPath, string remotePath, CancellationToken token = default(CancellationToken))
        {

            // verify args
            if (localPath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localPath");
            }
            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remotePath");
            }

            try
            {
                if (SupportsChecksum())
                {
                    FtpHash hash = await GetChecksumAsync(remotePath, FtpHashAlgorithm.NONE, token);
                    if (!hash.IsValid)
                    {
                        return false;
                    }

                    return hash.Verify(localPath);
                }

                // not supported, so return true to ignore validation
                return true;
            }
            catch (IOException ex)
            {
                LogStatus(FtpTraceLevel.Warn, "Failed to verify file " + localPath + " : " + ex.Message);
                return false;
            }
        }

#endif

        #endregion

        #region Utilities

        /// <summary>
        /// Sends progress to the user, either a value between 0-100 indicating percentage complete, or -1 for indeterminate.
        /// </summary>
        private void ReportProgress(IProgress<FtpProgress> progress, long fileSize, long position, long bytesProcessed, TimeSpan elapsedtime, string localPath, string remotePath, FtpProgress metaProgress)
        {

            //  calculate % done, transfer speed and time remaining
            FtpProgress status = FtpProgress.Generate(fileSize, position, bytesProcessed, elapsedtime, localPath, remotePath, metaProgress);

            // send progress to parent
            progress.Report(status);
        }

        /// <summary>
        /// Sends progress to the user, either a value between 0-100 indicating percentage complete, or -1 for indeterminate.
        /// </summary>
        private void ReportProgress(Action<FtpProgress> progress, long fileSize, long position, long bytesProcessed, TimeSpan elapsedtime, string localPath, string remotePath, FtpProgress metaProgress)
        {

            //  calculate % done, transfer speed and time remaining
            FtpProgress status = FtpProgress.Generate(fileSize, position, bytesProcessed, elapsedtime, localPath, remotePath, metaProgress);

            // send progress to parent
            progress(status);
        }
        #endregion
        #endregion // FtpClient_FileVerification.cs
        #region // FtpClient_FolderDownload.cs
        /// <summary>
        /// Downloads the specified directory onto the local file system.
        /// In Mirror mode, we will download missing files, and delete any extra files from disk that are not present on the server. This is very useful when creating an exact local backup of an FTP directory.
        /// In Update mode, we will only download missing files and preserve any extra files on disk. This is useful when you want to simply download missing files from an FTP directory.
        /// Only downloads the files and folders matching all the rules provided, if any.
        /// All exceptions during downloading are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="localFolder">The full path of the local folder on disk to download into. It is created if it does not exist.</param>
        /// <param name="remoteFolder">The full path of the remote FTP folder that you want to download. If it does not exist, an empty result list is returned.</param>
        /// <param name="mode">Mirror or Update mode, as explained above</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public List<FtpResult> DownloadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpLocalExists existsMode = FtpLocalExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, Action<FtpProgress> progress = null)
        {

            if (localFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // ensure the local path ends with slash
            localFolder = localFolder.EnsurePostfix(Path.DirectorySeparatorChar.ToString());

            // cleanup the remote path
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(DownloadDirectory), new object[] { localFolder, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the dir does not exist, fail fast
            if (!DirectoryExists(remoteFolder))
            {
                return results;
            }

            // ensure the local dir exists
            localFolder.EnsureDirectory();

            // get all the files in the remote directory
            var listing = GetListing(remoteFolder, FtpListOption.Recursive | FtpListOption.Size);

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // loop thru each file and transfer it
            var toDownload = GetFilesToDownload(localFolder, remoteFolder, rules, results, listing, shouldExist);
            DownloadServerFiles(toDownload, existsMode, verifyOptions, progress);

            // delete the extra local files if in mirror mode
            DeleteExtraLocalFiles(localFolder, mode, shouldExist, rules);

            return results;
        }

#if !NET40
        /// <summary>
        /// Downloads the specified directory onto the local file system.
        /// In Mirror mode, we will download missing files, and delete any extra files from disk that are not present on the server. This is very useful when creating an exact local backup of an FTP directory.
        /// In Update mode, we will only download missing files and preserve any extra files on disk. This is useful when you want to simply download missing files from an FTP directory.
        /// Only downloads the files and folders matching all the rules provided, if any.
        /// All exceptions during downloading are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="localFolder">The full path of the local folder on disk to download into. It is created if it does not exist.</param>
        /// <param name="remoteFolder">The full path of the remote FTP folder that you want to download. If it does not exist, an empty result list is returned.</param>
        /// <param name="mode">Mirror or Update mode, as explained above</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public async Task<List<FtpResult>> DownloadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpLocalExists existsMode = FtpLocalExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {

            if (localFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // ensure the local path ends with slash
            localFolder = localFolder.EnsurePostfix(Path.DirectorySeparatorChar.ToString());

            // cleanup the remote path
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(DownloadDirectoryAsync), new object[] { localFolder, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the dir does not exist, fail fast
            if (!await DirectoryExistsAsync(remoteFolder, token))
            {
                return results;
            }

            // ensure the local dir exists
            localFolder.EnsureDirectory();

            // get all the files in the remote directory
            var listing = await GetListingAsync(remoteFolder, FtpListOption.Recursive | FtpListOption.Size, token);

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // loop thru each file and transfer it #1
            var toDownload = GetFilesToDownload(localFolder, remoteFolder, rules, results, listing, shouldExist);

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            /*-------------------------------------------------------------------------------------/
			 *   Cancelling after this point would leave the FTP server in an inconsistent state   *
			 *-------------------------------------------------------------------------------------*/

            // loop thru each file and transfer it #2
            await DownloadServerFilesAsync(toDownload, existsMode, verifyOptions, progress, token);

            // delete the extra local files if in mirror mode
            DeleteExtraLocalFiles(localFolder, mode, shouldExist, rules);

            return results;
        }
#endif

        /// <summary>
        /// Get a list of all the files and folders that need to be downloaded
        /// </summary>
        private List<FtpResult> GetFilesToDownload(string localFolder, string remoteFolder, List<FtpRule> rules, List<FtpResult> results, FtpListItem[] listing, Dictionary<string, bool> shouldExist)
        {

            var toDownload = new List<FtpResult>();

            foreach (var remoteFile in listing)
            {

                // calculate the local path
                var relativePath = remoteFile.FullName.EnsurePrefix("/").RemovePrefix(remoteFolder).Replace('/', Path.DirectorySeparatorChar);
                var localFile = localFolder.CombineLocalPath(relativePath);

                // create the result object
                var result = new FtpResult()
                {
                    Type = remoteFile.Type,
                    Size = remoteFile.Size,
                    Name = remoteFile.Name,
                    RemotePath = remoteFile.FullName,
                    LocalPath = localFile,
                    IsDownload = true,
                };

                // only files and folders are processed
                if (remoteFile.Type == FtpFileSystemObjectType.File ||
                    remoteFile.Type == FtpFileSystemObjectType.Directory)
                {


                    // record the file
                    results.Add(result);

                    // skip downloading the file if it does not pass all the rules
                    if (!FilePassesRules(result, rules, false, remoteFile))
                    {
                        continue;
                    }

                    // record that this file/folder should exist
                    shouldExist.Add(localFile.ToLower(), true);

                    // only files are processed
                    toDownload.Add(result);


                }
            }

            return toDownload;
        }

        /// <summary>
        /// Download all the listed files and folders from the main directory
        /// </summary>
        private void DownloadServerFiles(List<FtpResult> toDownload, FtpLocalExists existsMode, FtpVerify verifyOptions, Action<FtpProgress> progress)
        {

            LogFunc(nameof(DownloadServerFiles), new object[] { toDownload.Count + " files" });

            // per object to download
            var r = -1;
            foreach (var result in toDownload)
            {
                r++;

                if (result.Type == FtpFileSystemObjectType.File)
                {

                    // absorb errors
                    try
                    {

                        // create meta progress to store the file progress
                        var metaProgress = new FtpProgress(toDownload.Count, r);

                        // download the file
                        var transferred = DownloadFileToFile(result.LocalPath, result.RemotePath, existsMode, verifyOptions, progress, metaProgress);
                        result.IsSuccess = transferred.IsSuccess();
                        result.IsSkipped = transferred == FtpStatus.Skipped;
                    }
                    catch (Exception ex)
                    {

                        LogStatus(FtpTraceLevel.Warn, "File failed to download: " + result.RemotePath);

                        // mark that the file failed to download
                        result.IsFailed = true;
                        result.Exception = ex;
                    }

                }
                else if (result.Type == FtpFileSystemObjectType.Directory)
                {

                    // absorb errors
                    try
                    {

                        // create directory on local filesystem
                        // to ensure we download the blank remote dirs as well
                        var created = result.LocalPath.EnsureDirectory();
                        result.IsSuccess = true;
                        result.IsSkipped = !created;

                    }
                    catch (Exception ex)
                    {

                        // mark that the file failed to download
                        result.IsFailed = true;
                        result.Exception = ex;
                    }

                }
            }

        }

#if !NET40
        /// <summary>
        /// Download all the listed files and folders from the main directory
        /// </summary>
        private async Task DownloadServerFilesAsync(List<FtpResult> toDownload, FtpLocalExists existsMode, FtpVerify verifyOptions, IProgress<FtpProgress> progress, CancellationToken token)
        {

            LogFunc(nameof(DownloadServerFilesAsync), new object[] { toDownload.Count + " files" });

            // per object to download
            var r = -1;
            foreach (var result in toDownload)
            {
                r++;

                if (result.Type == FtpFileSystemObjectType.File)
                {

                    // absorb errors
                    try
                    {

                        // create meta progress to store the file progress
                        var metaProgress = new FtpProgress(toDownload.Count, r);

                        // download the file
                        var transferred = await DownloadFileToFileAsync(result.LocalPath, result.RemotePath, existsMode, verifyOptions, progress, token, metaProgress);
                        result.IsSuccess = transferred.IsSuccess();
                        result.IsSkipped = transferred == FtpStatus.Skipped;
                    }
                    catch (Exception ex)
                    {

                        LogStatus(FtpTraceLevel.Warn, "File failed to download: " + result.RemotePath);

                        // mark that the file failed to download
                        result.IsFailed = true;
                        result.Exception = ex;
                    }

                }
                else if (result.Type == FtpFileSystemObjectType.Directory)
                {

                    // absorb errors
                    try
                    {

                        // create directory on local filesystem
                        // to ensure we download the blank remote dirs as well
                        var created = result.LocalPath.EnsureDirectory();
                        result.IsSuccess = true;
                        result.IsSkipped = !created;

                    }
                    catch (Exception ex)
                    {

                        // mark that the file failed to download
                        result.IsFailed = true;
                        result.Exception = ex;
                    }

                }
            }

        }
#endif

        /// <summary>
        /// Delete the extra local files if in mirror mode
        /// </summary>
        private void DeleteExtraLocalFiles(string localFolder, FtpFolderSyncMode mode, Dictionary<string, bool> shouldExist, List<FtpRule> rules)
        {
            if (mode == FtpFolderSyncMode.Mirror)
            {

                LogFunc(nameof(DeleteExtraLocalFiles));

                // get all the local files
                var localListing = Directory.GetFiles(localFolder, "*.*", SearchOption.AllDirectories);

                // delete files that are not in listed in shouldExist
                foreach (var existingLocalFile in localListing)
                {

                    if (!shouldExist.ContainsKey(existingLocalFile.ToLower()))
                    {

                        // only delete the local file if its permitted by the configuration
                        if (CanDeleteLocalFile(rules, existingLocalFile))
                        {
                            LogStatus(FtpTraceLevel.Info, "Delete extra file from disk: " + existingLocalFile);

                            // delete the file from disk
                            try
                            {
                                File.Delete(existingLocalFile);
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if the local file can be deleted, based on the DownloadDirectoryDeleteExcluded property
        /// </summary>
        private bool CanDeleteLocalFile(List<FtpRule> rules, string existingLocalFile)
        {

            // if we should not delete excluded files
            if (!DownloadDirectoryDeleteExcluded && !rules.IsBlank())
            {

                // create the result object to validate rules to ensure that file from excluded
                // directories are not deleted on the local filesystem
                var result = new FtpResult()
                {
                    Type = FtpFileSystemObjectType.File,
                    Size = 0,
                    Name = Path.GetFileName(existingLocalFile),
                    LocalPath = existingLocalFile,
                    IsDownload = false,
                };

                // check if the file passes the rules
                if (FilePassesRules(result, rules, true))
                {
                    // delete the file because it is included
                    return true;
                }
                else
                {
                    // do not delete the file because it is excluded
                    return false;
                }
            }

            // always delete the file whether its included or excluded by the rules
            return true;
        }
        #endregion // FtpClient_FolderDownload.cs
        #region // FtpClient_FolderManagement.cs
        #region Delete Directory

        /// <summary>
        /// Deletes the specified directory and all its contents.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        public void DeleteDirectory(string path)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(DeleteDirectory), new object[] { path });
            DeleteDirInternal(path, true, FtpListOption.Recursive);
        }

        /// <summary>
        /// Deletes the specified directory and all its contents.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        /// <param name="options">Useful to delete hidden files or dot-files.</param>
        public void DeleteDirectory(string path, FtpListOption options)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(DeleteDirectory), new object[] { path, options });
            DeleteDirInternal(path, true, options);
        }

        /// <summary>
        /// Deletes the specified directory and all its contents.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        /// <param name="deleteContents">If the directory is not empty, remove its contents</param>
        /// <param name="options">Useful to delete hidden files or dot-files.</param>
        private void DeleteDirInternal(string path, bool deleteContents, FtpListOption options)
        {
            FtpReply reply;

            path = path.GetFtpPath();
            lock (m_lock)
            {
                // server-specific directory deletion
                if (!path.IsFtpRootDirectory())
                {

                    // ask the server handler to delete a directory
                    if (ServerHandler != null)
                    {
                        if (ServerHandler.DeleteDirectory(this, path, path, deleteContents, options))
                        {
                            return;
                        }
                    }
                }


                // DELETE CONTENTS OF THE DIRECTORY
                if (deleteContents)
                {
                    // when GetListing is called with recursive option, then it does not
                    // make any sense to call another DeleteDirectory with force flag set.
                    // however this requires always delete files first.
                    var recurse = !WasGetListingRecursive(options);

                    // items that are deeper in directory tree are listed first, 
                    // then files will be listed before directories. This matters
                    // only if GetListing was called with recursive option.
                    FtpListItem[] itemList;
                    if (recurse)
                    {
                        itemList = GetListing(path, options);
                    }
                    else
                    {
                        itemList = GetListing(path, options).OrderByDescending(x => x.FullName.Count(c => c.Equals('/'))).ThenBy(x => x.Type).ToArray();
                    }

                    // delete the item based on the type
                    foreach (var item in itemList)
                    {
                        switch (item.Type)
                        {
                            case FtpFileSystemObjectType.File:
                                DeleteFile(item.FullName);
                                break;

                            case FtpFileSystemObjectType.Directory:
                                DeleteDirInternal(item.FullName, recurse, options);
                                break;

                            default:
                                throw new FtpException("Don't know how to delete object type: " + item.Type);
                        }
                    }
                }


                // SKIP DELETING ROOT DIRS

                // can't delete the working directory and
                // can't delete the server root.
                if (path.IsFtpRootDirectory())
                {
                    return;
                }


                // DELETE ACTUAL DIRECTORY

                if (!(reply = Execute("RMD " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
        }

        /// <summary>
        /// Checks whether <see cref="o:GetListing"/> will be called recursively or not.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private bool WasGetListingRecursive(FtpListOption options)
        {
            // FIX: GetListing() now supports recursive listing for all types of lists (name list, file list, machine list)
            //		even if the server does not support recursive listing, because it does its own internal recursion.
            return (options & FtpListOption.Recursive) == FtpListOption.Recursive;
        }

#if !NET40
        /// <summary>
        /// Asynchronously removes a directory and all its contents.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task DeleteDirectoryAsync(string path, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(DeleteDirectoryAsync), new object[] { path });
            return DeleteDirInternalAsync(path, true, FtpListOption.Recursive, token);
        }

        /// <summary>
        /// Asynchronously removes a directory and all its contents.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        /// <param name="options">Useful to delete hidden files or dot-files.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task DeleteDirectoryAsync(string path, FtpListOption options, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(DeleteDirectoryAsync), new object[] { path, options });
            return DeleteDirInternalAsync(path, true, options, token);
        }

        /// <summary>
        /// Asynchronously removes a directory. Used by <see cref="DeleteDirectoryAsync(string, CancellationToken)"/> and
        /// <see cref="DeleteDirectoryAsync(string, FtpListOption,CancellationToken)"/>.
        /// </summary>
        /// <param name="path">The full or relative path of the directory to delete</param>
        /// <param name="deleteContents">Delete the contents before deleting the folder</param>
        /// <param name="options">Useful to delete hidden files or dot-files.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns></returns>
        private async Task DeleteDirInternalAsync(string path, bool deleteContents, FtpListOption options, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;
            path = path.GetFtpPath();

            // server-specific directory deletion
            if (!path.IsFtpRootDirectory())
            {

                // ask the server handler to delete a directory
                if (ServerHandler != null)
                {
                    if (await ServerHandler.DeleteDirectoryAsync(this, path, path, deleteContents, options, token))
                    {
                        return;
                    }
                }
            }

            // DELETE CONTENTS OF THE DIRECTORY
            if (deleteContents)
            {
                // when GetListing is called with recursive option, then it does not
                // make any sense to call another DeleteDirectory with force flag set.
                // however this requires always delete files first.
                var recurse = !WasGetListingRecursive(options);

                // items that are deeper in directory tree are listed first, 
                // then files will be listed before directories. This matters
                // only if GetListing was called with recursive option.
                FtpListItem[] itemList;
                if (recurse)
                {
                    itemList = await GetListingAsync(path, options, token);
                }
                else
                {
                    itemList = (await GetListingAsync(path, options, token)).OrderByDescending(x => x.FullName.Count(c => c.Equals('/'))).ThenBy(x => x.Type).ToArray();
                }

                // delete the item based on the type
                foreach (var item in itemList)
                {
                    switch (item.Type)
                    {
                        case FtpFileSystemObjectType.File:
                            await DeleteFileAsync(item.FullName, token);
                            break;

                        case FtpFileSystemObjectType.Directory:
                            await DeleteDirInternalAsync(item.FullName, recurse, options, token);
                            break;

                        default:
                            throw new FtpException("Don't know how to delete object type: " + item.Type);
                    }
                }
            }

            // SKIP DELETING ROOT DIRS

            // can't delete the working directory and
            // can't delete the server root.
            if (path.IsFtpRootDirectory())
            {
                return;
            }

            // DELETE ACTUAL DIRECTORY

            if (!(reply = await ExecuteAsync("RMD " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }
        }
#endif

        #endregion

        #region Directory Exists

        /// <summary>
        /// Tests if the specified directory exists on the server. This
        /// method works by trying to change the working directory to
        /// the path specified. If it succeeds, the directory is changed
        /// back to the old working directory and true is returned. False
        /// is returned otherwise and since the CWD failed it is assumed
        /// the working directory is still the same.
        /// </summary>
        /// <param name="path">The path of the directory</param>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool DirectoryExists(string path)
        {
            string pwd;

            // don't verify args as blank/null path is OK
            //if (path.IsBlank())
            //	throw new ArgumentException("Required parameter is null or blank.", "path");

            path = path.GetFtpPath();

            LogFunc(nameof(DirectoryExists), new object[] { path });

            // quickly check if root path, then it always exists!
            if (path.IsFtpRootDirectory())
            {
                return true;
            }

            // check if a folder exists by changing the working dir to it
            lock (m_lock)
            {
                pwd = GetWorkingDirectory();

                if (Execute("CWD " + path).Success)
                {
                    var reply = Execute("CWD " + pwd);

                    if (!reply.Success)
                    {
                        throw new FtpException("DirectoryExists(): Failed to restore the working directory.");
                    }

                    return true;
                }
            }

            return false;
        }

#if !NET40
        /// <summary>
        /// Tests if the specified directory exists on the server asynchronously. This
        /// method works by trying to change the working directory to
        /// the path specified. If it succeeds, the directory is changed
        /// back to the old working directory and true is returned. False
        /// is returned otherwise and since the CWD failed it is assumed
        /// the working directory is still the same.
        /// </summary>
        /// <param name='path'>The full or relative path of the directory to check for</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>True if the directory exists. False otherwise.</returns>
        public async Task<bool> DirectoryExistsAsync(string path, CancellationToken token = default(CancellationToken))
        {
            string pwd;

            // don't verify args as blank/null path is OK
            //if (path.IsBlank())
            //	throw new ArgumentException("Required parameter is null or blank.", "path");

            path = path.GetFtpPath();

            LogFunc(nameof(DirectoryExistsAsync), new object[] { path });

            // quickly check if root path, then it always exists!
            if (path.IsFtpRootDirectory())
            {
                return true;
            }

            // check if a folder exists by changing the working dir to it
            pwd = await GetWorkingDirectoryAsync(token);

            if ((await ExecuteAsync("CWD " + path, token)).Success)
            {
                FtpReply reply = await ExecuteAsync("CWD " + pwd, token);

                if (!reply.Success)
                {
                    throw new FtpException("DirectoryExists(): Failed to restore the working directory.");
                }

                return true;
            }

            return false;
        }
#endif

        #endregion

        #region Create Directory

        /// <summary>
        /// Creates a directory on the server. If the preceding
        /// directories do not exist, then they are created.
        /// </summary>
        /// <param name="path">The full or relative path to the new remote directory</param>
        public bool CreateDirectory(string path)
        {
            return CreateDirectory(path, true);
        }

        /// <summary>
        /// Creates a directory on the server
        /// </summary>
        /// <param name="path">The full or relative path to the new remote directory</param>
        /// <param name="force">Try to force all non-existent pieces of the path to be created</param>
        /// <returns>True if directory was created, false if it was skipped</returns>
        public bool CreateDirectory(string path, bool force)
        {
            // don't verify args as blank/null path is OK
            //if (path.IsBlank())
            //	throw new ArgumentException("Required parameter is null or blank.", "path");

            path = path.GetFtpPath();

            LogFunc(nameof(CreateDirectory), new object[] { path, force });

            FtpReply reply;

            // cannot create root or working directory
            if (path.IsFtpRootDirectory())
            {
                return false;
            }
            lock (m_lock)
            {
                // server-specific directory creation
                // ask the server handler to create a directory
                if (ServerHandler != null)
                {
                    if (ServerHandler.CreateDirectory(this, path, path, force))
                    {
                        return true;
                    }
                }

                path = path.TrimEnd('/');

                if (force && !DirectoryExists(path.GetFtpDirectoryName()))
                {
                    LogStatus(FtpTraceLevel.Verbose, "Create non-existent parent directory: " + path.GetFtpDirectoryName());
                    CreateDirectory(path.GetFtpDirectoryName(), true);
                }

                // fix: improve performance by skipping the directory exists check
                /*else if (DirectoryExists(path)) {
					return false;
				}*/

                LogStatus(FtpTraceLevel.Verbose, "CreateDirectory " + path);

                if (!(reply = Execute("MKD " + path)).Success)
                {

                    // if the error indicates the directory already exists, its not an error
                    if (reply.Code == "550")
                    {
                        return false;
                    }
                    if (reply.Code[0] == '5' && reply.Message.IsKnownError(FtpServerStrings.folderExists))
                    {
                        return false;
                    }

                    throw new FtpCommandException(reply);
                }
                return true;
            }
        }

#if !NET40
        /// <summary>
        /// Creates a remote directory asynchronously
        /// </summary>
        /// <param name="path">The full or relative path to the new remote directory</param>
        /// <param name="force">Try to create the whole path if the preceding directories do not exist</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>True if directory was created, false if it was skipped</returns>
        public async Task<bool> CreateDirectoryAsync(string path, bool force, CancellationToken token = default(CancellationToken))
        {
            // don't verify args as blank/null path is OK
            //if (path.IsBlank())
            //	throw new ArgumentException("Required parameter is null or blank.", "path");

            path = path.GetFtpPath();

            LogFunc(nameof(CreateDirectoryAsync), new object[] { path, force });

            FtpReply reply;

            // cannot create root or working directory
            if (path.IsFtpRootDirectory())
            {
                return false;
            }

            // server-specific directory creation
            // ask the server handler to create a directory
            if (ServerHandler != null)
            {
                if (await ServerHandler.CreateDirectoryAsync(this, path, path, force, token))
                {
                    return true;
                }
            }

            path = path.TrimEnd('/');

            if (force && !await DirectoryExistsAsync(path.GetFtpDirectoryName(), token))
            {
                LogStatus(FtpTraceLevel.Verbose, "Create non-existent parent directory: " + path.GetFtpDirectoryName());
                await CreateDirectoryAsync(path.GetFtpDirectoryName(), true, token);
            }

            // fix: improve performance by skipping the directory exists check
            /*else if (await DirectoryExistsAsync(path, token)) {
				return false;
			}*/

            LogStatus(FtpTraceLevel.Verbose, "CreateDirectory " + path);

            if (!(reply = await ExecuteAsync("MKD " + path, token)).Success)
            {

                // if the error indicates the directory already exists, its not an error
                if (reply.Code == "550")
                {
                    return false;
                }
                if (reply.Code[0] == '5' && reply.Message.IsKnownError(FtpServerStrings.folderExists))
                {
                    return false;
                }

                throw new FtpCommandException(reply);
            }
            return true;
        }

        /// <summary>
        /// Creates a remote directory asynchronously. If the preceding
        /// directories do not exist, then they are created.
        /// </summary>
        /// <param name="path">The full or relative path to the new remote directory</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public Task<bool> CreateDirectoryAsync(string path, CancellationToken token = default(CancellationToken))
        {
            return CreateDirectoryAsync(path, true, token);
        }
#endif

        #endregion

        #region Move Directory

        /// <summary>
        /// Moves a directory on the remote file system from one directory to another.
        /// Always checks if the source directory exists. Checks if the dest directory exists based on the `existsMode` parameter.
        /// Only throws exceptions for critical errors.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="existsMode">Should we check if the dest directory exists? And if it does should we overwrite/skip the operation?</param>
        /// <returns>Whether the directory was moved</returns>
        public bool MoveDirectory(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(MoveDirectory), new object[] { path, dest, existsMode });

            if (DirectoryExists(path))
            {
                // check if dest directory exists and act accordingly
                if (existsMode != FtpRemoteExists.NoCheck)
                {
                    var destExists = DirectoryExists(dest);
                    if (destExists)
                    {
                        switch (existsMode)
                        {
                            case FtpRemoteExists.Overwrite:
                                DeleteDirectory(dest);
                                break;

                            case FtpRemoteExists.Skip:
                                return false;
                        }
                    }
                }

                // move the directory
                Rename(path, dest);

                return true;
            }

            return false;
        }

#if !NET40
        /// <summary>
        /// Moves a directory asynchronously on the remote file system from one directory to another.
        /// Always checks if the source directory exists. Checks if the dest directory exists based on the `existsMode` parameter.
        /// Only throws exceptions for critical errors.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="existsMode">Should we check if the dest directory exists? And if it does should we overwrite/skip the operation?</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>Whether the directory was moved</returns>
        public async Task<bool> MoveDirectoryAsync(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(MoveDirectoryAsync), new object[] { path, dest, existsMode });

            if (await DirectoryExistsAsync(path, token))
            {
                // check if dest directory exists and act accordingly
                if (existsMode != FtpRemoteExists.NoCheck)
                {
                    bool destExists = await DirectoryExistsAsync(dest, token);
                    if (destExists)
                    {
                        switch (existsMode)
                        {
                            case FtpRemoteExists.Overwrite:
                                await DeleteDirectoryAsync(dest, token);
                                break;

                            case FtpRemoteExists.Skip:
                                return false;
                        }
                    }
                }

                // move the directory
                await RenameAsync(path, dest, token);

                return true;
            }

            return false;
        }
#endif

        #endregion

        #region Set Working Dir

        /// <summary>
        /// Sets the work directory on the server
        /// </summary>
        /// <param name="path">The path of the directory to change to</param>
        public void SetWorkingDirectory(string path)
        {

            path = path.GetFtpPath();

            LogFunc(nameof(SetWorkingDirectory), new object[] { path });

            FtpReply reply;

            // exit if invalid path
            if (path == "." || path == "./")
            {
                return;
            }
            lock (m_lock)
            {
                // modify working dir
                if (!(reply = Execute("CWD " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }

                // invalidate the cached path
                // This is redundant, Execute(...) will see the CWD and do this
                //_LastWorkingDir = null;
            }
        }


#if !NET40
        /// <summary>
        /// Sets the working directory on the server asynchronously
        /// </summary>
        /// <param name="path">The directory to change to</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task SetWorkingDirectoryAsync(string path, CancellationToken token = default(CancellationToken))
        {

            path = path.GetFtpPath();

            LogFunc(nameof(SetWorkingDirectoryAsync), new object[] { path });

            FtpReply reply;

            // exit if invalid path
            if (path == "." || path == "./")
            {
                return;
            }

            // modify working dir
            if (!(reply = await ExecuteAsync("CWD " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // invalidate the cached path
            // This is redundant, Execute(...) will see the CWD and do this
            //_LastWorkingDir = null;
        }


#endif

        #endregion

        #region Get Working Dir

        /// <summary>
        /// Gets the current working directory
        /// </summary>
        /// <returns>The current working directory, ./ if the response couldn't be parsed.</returns>
        public string GetWorkingDirectory()
        {

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                ReadCurrentWorkingDirectory();
            }

            return _LastWorkingDir;
        }

#if !NET40
        /// <summary>
        /// Gets the current working directory asynchronously
        /// </summary>
        /// <returns>The current working directory, ./ if the response couldn't be parsed.</returns>
        public async Task<string> GetWorkingDirectoryAsync(CancellationToken token = default(CancellationToken))
        {

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                await ReadCurrentWorkingDirectoryAsync(token);
            }

            return _LastWorkingDir;
        }

#endif

        private FtpReply ReadCurrentWorkingDirectory()
        {
            FtpReply reply;
            lock (m_lock)
            {
                // read the absolute path of the current working dir
                if (!(reply = Execute("PWD")).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
            // cache the last working dir
            _LastWorkingDir = ParseWorkingDirectory(reply);
            return reply;
        }

        private string ParseWorkingDirectory(FtpReply reply)
        {
            Match m;

            if ((m = Regex.Match(reply.Message, "\"(?<pwd>.*)\"")).Success)
            {
                return m.Groups["pwd"].Value.GetFtpPath();
            }

            // check for MODCOMP ftp path mentioned in forums: https://netftp.codeplex.com/discussions/444461
            if ((m = Regex.Match(reply.Message, "PWD = (?<pwd>.*)")).Success)
            {
                return m.Groups["pwd"].Value.GetFtpPath();
            }

            LogStatus(FtpTraceLevel.Warn, "Failed to parse working directory from: " + reply.Message);

            return "/";
        }
#if !NET40

        private async Task<FtpReply> ReadCurrentWorkingDirectoryAsync(CancellationToken token)
        {

            FtpReply reply;

            // read the absolute path of the current working dir
            if (!(reply = await ExecuteAsync("PWD", token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // cache the last working dir
            _LastWorkingDir = ParseWorkingDirectory(reply);
            return reply;
        }
#endif

        #endregion

        #region Is Root Dir

        /// <summary>
        /// Is the current working directory the root?
        /// </summary>
        /// <returns>true if root.</returns>
        public bool IsRoot()
        {

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                ReadCurrentWorkingDirectory();
            }

            if (_LastWorkingDir.IsFtpRootDirectory())
            {
                return true;
            }

            // execute server-specific check if the current working dir is a root directory
            if (ServerHandler != null && ServerHandler.IsRoot(this, _LastWorkingDir))
            {
                return true;
            }

            return false;
        }

#if !NET40
        /// <summary>
        /// Is the current working directory the root?
        /// </summary>
        /// <returns>true if root.</returns>
        public async Task<bool> IsRootAsync(CancellationToken token = default(CancellationToken))
        {

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                await ReadCurrentWorkingDirectoryAsync(token);
            }

            if (_LastWorkingDir.IsFtpRootDirectory())
            {
                return true;
            }

            // execute server-specific check if the current working dir is a root directory
            if (ServerHandler != null && ServerHandler.IsRoot(this, _LastWorkingDir))
            {
                return true;
            }

            return false;
        }
#endif
        #endregion
        #endregion // FtpClient_FolderManagement.cs
        #region // FtpClient_FolderUpload.cs
        /// <summary>
        /// Uploads the specified directory onto the server.
        /// In Mirror mode, we will upload missing files, and delete any extra files from the server that are not present on disk. This is very useful when publishing an exact copy of a local folder onto an FTP server.
        /// In Update mode, we will only upload missing files and preserve any extra files on the server. This is useful when you want to simply upload missing files to a server.
        /// Only uploads the files and folders matching all the rules provided, if any.
        /// All exceptions during uploading are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="localFolder">The full path of the local folder on disk that you want to upload. If it does not exist, an empty result list is returned.</param>
        /// <param name="remoteFolder">The full path of the remote FTP folder to upload into. It is created if it does not exist.</param>
        /// <param name="mode">Mirror or Update mode, as explained above</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the upload or restart the upload?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide a callback to track upload progress.</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public List<FtpResult> UploadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, Action<FtpProgress> progress = null)
        {

            if (localFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // ensure the local path ends with slash
            localFolder = localFolder.EnsurePostfix(Path.DirectorySeparatorChar.ToString());

            // cleanup the remote path
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(UploadDirectory), new object[] { localFolder, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the dir does not exist, fail fast
            if (!Directory.Exists(localFolder))
            {
                return results;
            }

            // flag to determine if existence checks are required
            var checkFileExistence = true;

            // ensure the remote dir exists
            if (!DirectoryExists(remoteFolder))
            {
                CreateDirectory(remoteFolder);
                checkFileExistence = false;
            }

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // get all the folders in the local directory
            var dirListing = Directory.GetDirectories(localFolder, "*.*", SearchOption.AllDirectories);

            // get all the already existing files
            var remoteListing = checkFileExistence ? GetListing(remoteFolder, FtpListOption.Recursive) : null;

            // loop thru each folder and ensure it exists
            var dirsToUpload = GetSubDirectoriesToUpload(localFolder, remoteFolder, rules, results, dirListing);
            CreateSubDirectories(this, dirsToUpload);

            // get all the files in the local directory
            var fileListing = Directory.GetFiles(localFolder, "*.*", SearchOption.AllDirectories);

            // loop thru each file and transfer it
            var filesToUpload = GetFilesToUpload(localFolder, remoteFolder, rules, results, shouldExist, fileListing);
            UploadDirectoryFiles(filesToUpload, existsMode, verifyOptions, progress, remoteListing);

            // delete the extra remote files if in mirror mode and the directory was pre-existing
            DeleteExtraServerFiles(mode, remoteFolder, shouldExist, remoteListing, rules);

            return results;
        }

#if !NET40
        /// <summary>
        /// Uploads the specified directory onto the server.
        /// In Mirror mode, we will upload missing files, and delete any extra files from the server that are not present on disk. This is very useful when publishing an exact copy of a local folder onto an FTP server.
        /// In Update mode, we will only upload missing files and preserve any extra files on the server. This is useful when you want to simply upload missing files to a server.
        /// Only uploads the files and folders matching all the rules provided, if any.
        /// All exceptions during uploading are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="localFolder">The full path of the local folder on disk that you want to upload. If it does not exist, an empty result list is returned.</param>
        /// <param name="remoteFolder">The full path of the remote FTP folder to upload into. It is created if it does not exist.</param>
        /// <param name="mode">Mirror or Update mode, as explained above</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the upload or restart the upload?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful upload and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide an implementation of IProgress to track upload progress.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public async Task<List<FtpResult>> UploadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {

            if (localFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "localFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // ensure the local path ends with slash
            localFolder = localFolder.EnsurePostfix(Path.DirectorySeparatorChar.ToString());

            // cleanup the remote path
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(UploadDirectoryAsync), new object[] { localFolder, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the dir does not exist, fail fast
            if (!Directory.Exists(localFolder))
            {
                return results;
            }

            // flag to determine if existence checks are required
            var checkFileExistence = true;

            // ensure the remote dir exists
            if (!await DirectoryExistsAsync(remoteFolder, token))
            {
                await CreateDirectoryAsync(remoteFolder, token);
                checkFileExistence = false;
            }

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // get all the folders in the local directory
            var dirListing = Directory.GetDirectories(localFolder, "*.*", SearchOption.AllDirectories);

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // get all the already existing files
            var remoteListing = checkFileExistence ? GetListing(remoteFolder, FtpListOption.Recursive) : null;

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // loop thru each folder and ensure it exists #1
            var dirsToUpload = GetSubDirectoriesToUpload(localFolder, remoteFolder, rules, results, dirListing);

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            /*-------------------------------------------------------------------------------------/
			 *   Cancelling after this point would leave the FTP server in an inconsistent state   *
			 *-------------------------------------------------------------------------------------*/

            // loop thru each folder and ensure it exists #2
            await CreateSubDirectoriesAsync(this, dirsToUpload, token);

            // get all the files in the local directory
            var fileListing = Directory.GetFiles(localFolder, "*.*", SearchOption.AllDirectories);

            // loop thru each file and transfer it
            var filesToUpload = GetFilesToUpload(localFolder, remoteFolder, rules, results, shouldExist, fileListing);
            await UploadDirectoryFilesAsync(filesToUpload, existsMode, verifyOptions, progress, remoteListing, token);

            // delete the extra remote files if in mirror mode and the directory was pre-existing
            await DeleteExtraServerFilesAsync(mode, remoteFolder, shouldExist, remoteListing, rules, token);

            return results;
        }
#endif

        /// <summary>
        /// Get a list of all the sub directories that need to be created within the main directory
        /// </summary>
        private List<FtpResult> GetSubDirectoriesToUpload(string localFolder, string remoteFolder, List<FtpRule> rules, List<FtpResult> results, string[] dirListing)
        {

            var dirsToUpload = new List<FtpResult>();

            foreach (var localFile in dirListing)
            {

                // calculate the local path
                var relativePath = localFile.RemovePrefix(localFolder).RemovePrefix("\\").RemovePrefix("/").EnsurePostfix(Path.DirectorySeparatorChar.ToString());
                var remoteFile = remoteFolder.EnsurePostfix("/") + relativePath.Replace('\\', '/');

                // create the result object
                var result = new FtpResult()
                {
                    Type = FtpFileSystemObjectType.Directory,
                    Size = 0,
                    Name = Path.GetDirectoryName(localFile),
                    RemotePath = remoteFile,
                    LocalPath = localFile,
                    IsDownload = false,
                };

                // record the folder
                results.Add(result);

                // skip uploading the file if it does not pass all the rules
                if (!FilePassesRules(result, rules, true))
                {
                    continue;
                }

                dirsToUpload.Add(result);
            }

            return dirsToUpload;
        }

        /// <summary>
        /// Create all the sub directories within the main directory
        /// </summary>
        private void CreateSubDirectories(FtpClient client, List<FtpResult> dirsToUpload)
        {
            foreach (var result in dirsToUpload)
            {

                // absorb errors
                try
                {

                    // create directory on the server
                    // to ensure we upload the blank remote dirs as well
                    if (client.CreateDirectory(result.RemotePath))
                    {
                        result.IsSuccess = true;
                        result.IsSkipped = false;
                    }
                    else
                    {
                        result.IsSkipped = true;
                    }

                }
                catch (Exception ex)
                {

                    // mark that the folder failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }
        }

#if !NET40
        /// <summary>
        /// Create all the sub directories within the main directory
        /// </summary>
        private async Task CreateSubDirectoriesAsync(FtpClient client, List<FtpResult> dirsToUpload, CancellationToken token)
        {
            foreach (var result in dirsToUpload)
            {

                // absorb errors
                try
                {

                    // create directory on the server
                    // to ensure we upload the blank remote dirs as well
                    if (await client.CreateDirectoryAsync(result.RemotePath, token))
                    {
                        result.IsSuccess = true;
                        result.IsSkipped = false;
                    }
                    else
                    {
                        result.IsSkipped = true;
                    }

                }
                catch (Exception ex)
                {

                    // mark that the folder failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }
        }
#endif

        /// <summary>
        /// Get a list of all the files that need to be uploaded within the main directory
        /// </summary>
        private List<FtpResult> GetFilesToUpload(string localFolder, string remoteFolder, List<FtpRule> rules, List<FtpResult> results, Dictionary<string, bool> shouldExist, string[] fileListing)
        {

            var filesToUpload = new List<FtpResult>();

            foreach (var localFile in fileListing)
            {

                // calculate the local path
                var relativePath = localFile.Replace(localFolder, "").Replace(Path.DirectorySeparatorChar, '/');
                var remoteFile = remoteFolder + relativePath.Replace('\\', '/');

                // create the result object
                var result = new FtpResult()
                {
                    Type = FtpFileSystemObjectType.File,
                    Size = new FileInfo(localFile).Length,
                    Name = Path.GetFileName(localFile),
                    RemotePath = remoteFile,
                    LocalPath = localFile
                };

                // record the file
                results.Add(result);

                // skip uploading the file if it does not pass all the rules
                if (!FilePassesRules(result, rules, true))
                {
                    continue;
                }

                // record that this file should exist
                shouldExist.Add(remoteFile.ToLower(), true);

                // absorb errors
                filesToUpload.Add(result);
            }

            return filesToUpload;
        }

        /// <summary>
        /// Upload all the files within the main directory
        /// </summary>
        private void UploadDirectoryFiles(List<FtpResult> filesToUpload, FtpRemoteExists existsMode, FtpVerify verifyOptions, Action<FtpProgress> progress, FtpListItem[] remoteListing)
        {

            LogFunc(nameof(UploadDirectoryFiles), new object[] { filesToUpload.Count + " files" });

            int r = -1;
            foreach (var result in filesToUpload)
            {
                r++;

                // absorb errors
                try
                {

                    // skip uploading if the file already exists on the server
                    FtpRemoteExists existsModeToUse;
                    if (!CanUploadFile(result, remoteListing, existsMode, out existsModeToUse))
                    {
                        continue;
                    }

                    // create meta progress to store the file progress
                    var metaProgress = new FtpProgress(filesToUpload.Count, r);

                    // upload the file
                    var transferred = UploadFileFromFile(result.LocalPath, result.RemotePath, false, existsModeToUse, false, false, verifyOptions, progress, metaProgress);
                    result.IsSuccess = transferred.IsSuccess();
                    result.IsSkipped = transferred == FtpStatus.Skipped;

                }
                catch (Exception ex)
                {

                    LogStatus(FtpTraceLevel.Warn, "File failed to upload: " + result.LocalPath);

                    // mark that the file failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }

        }

        /// <summary>
        /// Check if the file is cleared to be uploaded, taking its existence/filesize and existsMode options into account.
        /// </summary>
        private bool CanUploadFile(FtpResult result, FtpListItem[] remoteListing, FtpRemoteExists existsMode, out FtpRemoteExists existsModeToUse)
        {

            // check if the file already exists on the server
            existsModeToUse = existsMode;
            var fileExists = FileListings.FileExistsInListing(remoteListing, result.RemotePath);

            // if we want to skip uploaded files and the file already exists, mark its skipped
            if (existsMode == FtpRemoteExists.Skip && fileExists)
            {

                LogStatus(FtpTraceLevel.Info, "Skipped file that already exists: " + result.LocalPath);

                result.IsSuccess = true;
                result.IsSkipped = true;
                return false;
            }

            // in any mode if the file does not exist, mark that exists check is not required
            if (!fileExists)
            {
                existsModeToUse = existsMode == FtpRemoteExists.Resume ? FtpRemoteExists.ResumeNoCheck : FtpRemoteExists.NoCheck;
            }
            return true;
        }

#if !NET40
        /// <summary>
        /// Upload all the files within the main directory
        /// </summary>
        private async Task UploadDirectoryFilesAsync(List<FtpResult> filesToUpload, FtpRemoteExists existsMode, FtpVerify verifyOptions, IProgress<FtpProgress> progress, FtpListItem[] remoteListing, CancellationToken token)
        {

            LogFunc(nameof(UploadDirectoryFilesAsync), new object[] { filesToUpload.Count + " files" });

            var r = -1;
            foreach (var result in filesToUpload)
            {
                r++;

                // absorb errors
                try
                {

                    // skip uploading if the file already exists on the server
                    FtpRemoteExists existsModeToUse;
                    if (!CanUploadFile(result, remoteListing, existsMode, out existsModeToUse))
                    {
                        continue;
                    }

                    // create meta progress to store the file progress
                    var metaProgress = new FtpProgress(filesToUpload.Count, r);

                    // upload the file
                    var transferred = await UploadFileFromFileAsync(result.LocalPath, result.RemotePath, false, existsModeToUse, false, false, verifyOptions, token, progress, metaProgress);
                    result.IsSuccess = transferred.IsSuccess();
                    result.IsSkipped = transferred == FtpStatus.Skipped;

                }
                catch (Exception ex)
                {

                    LogStatus(FtpTraceLevel.Warn, "File failed to upload: " + result.LocalPath);

                    // mark that the file failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }

        }
#endif

        /// <summary>
        /// Delete the extra remote files if in mirror mode and the directory was pre-existing
        /// </summary>
        private void DeleteExtraServerFiles(FtpFolderSyncMode mode, string remoteFolder, Dictionary<string, bool> shouldExist, FtpListItem[] remoteListing, List<FtpRule> rules)
        {
            if (mode == FtpFolderSyncMode.Mirror && remoteListing != null)
            {

                LogFunc(nameof(DeleteExtraServerFiles));

                // delete files that are not in listed in shouldExist
                foreach (var existingServerFile in remoteListing)
                {

                    if (existingServerFile.Type == FtpFileSystemObjectType.File)
                    {

                        if (!shouldExist.ContainsKey(existingServerFile.FullName.ToLower()))
                        {

                            // only delete the remote file if its permitted by the configuration
                            if (CanDeleteRemoteFile(rules, existingServerFile))
                            {
                                LogStatus(FtpTraceLevel.Info, "Delete extra file from server: " + existingServerFile.FullName);

                                // delete the file from the server
                                try
                                {
                                    DeleteFile(existingServerFile.FullName);
                                }
                                catch (Exception) { }
                            }
                        }

                    }

                }

            }
        }

#if !NET40
        /// <summary>
        /// Delete the extra remote files if in mirror mode and the directory was pre-existing
        /// </summary>
        private async Task DeleteExtraServerFilesAsync(FtpFolderSyncMode mode, string remoteFolder, Dictionary<string, bool> shouldExist, FtpListItem[] remoteListing, List<FtpRule> rules, CancellationToken token)
        {
            if (mode == FtpFolderSyncMode.Mirror && remoteListing != null)
            {

                LogFunc(nameof(DeleteExtraServerFilesAsync));

                // delete files that are not in listed in shouldExist
                foreach (var existingServerFile in remoteListing)
                {

                    if (existingServerFile.Type == FtpFileSystemObjectType.File)
                    {

                        if (!shouldExist.ContainsKey(existingServerFile.FullName.ToLower()))
                        {

                            // only delete the remote file if its permitted by the configuration
                            if (CanDeleteRemoteFile(rules, existingServerFile))
                            {
                                LogStatus(FtpTraceLevel.Info, "Delete extra file from server: " + existingServerFile.FullName);

                                // delete the file from the server
                                try
                                {
                                    await DeleteFileAsync(existingServerFile.FullName, token);
                                }
                                catch (Exception) { }
                            }
                        }

                    }

                }

            }
        }

#endif

        /// <summary>
        /// Check if the remote file can be deleted, based on the UploadDirectoryDeleteExcluded property
        /// </summary>
        private bool CanDeleteRemoteFile(List<FtpRule> rules, FtpListItem existingServerFile)
        {

            // if we should not delete excluded files
            if (!UploadDirectoryDeleteExcluded && !rules.IsBlank())
            {

                // create the result object to validate rules to ensure that file from excluded
                // directories are not deleted on the FTP remote server
                var result = new FtpResult()
                {
                    Type = existingServerFile.Type,
                    Size = existingServerFile.Size,
                    Name = Path.GetFileName(existingServerFile.FullName),
                    RemotePath = existingServerFile.FullName,
                    IsDownload = false,
                };

                // check if the file passes the rules
                if (FilePassesRules(result, rules, false))
                {
                    // delete the file because it is included
                    return true;
                }
                else
                {
                    // do not delete the file because it is excluded
                    return false;
                }
            }

            // always delete the file whether its included or excluded by the rules
            return true;
        }
        #endregion // FtpClient_FolderUpload.cs
        #region // FtpClient_FXPConnection.cs
        /// <summary>
        /// Opens a FXP PASV connection between the source FTP Server and the destination FTP Server
        /// </summary>
        /// <param name="remoteClient">FtpClient instance of the destination FTP Server</param>
        /// <param name="trackProgress"></param>
        /// <returns>A data stream ready to be used</returns>
        private FtpFxpSession OpenPassiveFXPConnection(FtpClient remoteClient, bool trackProgress)
        {
            FtpReply reply, reply2;
            Match m;
            FtpClient sourceClient = null;
            FtpClient destinationClient = null;
            FtpClient progressClient = null;

            // create a new connection to the source FTP server if EnableThreadSafeDataConnections is set
            if (EnableThreadSafeDataConnections)
            {
                sourceClient = CloneConnection();
                sourceClient._AutoDispose = true;
                sourceClient.CopyStateFlags(this);
                sourceClient.Connect();
                sourceClient.SetWorkingDirectory(GetWorkingDirectory());
            }
            else
            {
                sourceClient = this;
            }

            // create a new connection to the target FTP server if EnableThreadSafeDataConnections is set
            if (remoteClient.EnableThreadSafeDataConnections)
            {
                destinationClient = remoteClient.CloneConnection();
                destinationClient._AutoDispose = true;
                destinationClient.CopyStateFlags(remoteClient);
                destinationClient.Connect();
                destinationClient.SetWorkingDirectory(remoteClient.GetWorkingDirectory());
            }
            else
            {
                destinationClient = remoteClient;
            }

            // create a new connection to the target FTP server to track progress
            // if progress tracking is enabled during this FXP transfer
            if (trackProgress)
            {
                progressClient = remoteClient.CloneConnection();
                progressClient._AutoDispose = true;
                progressClient.CopyStateFlags(remoteClient);
                progressClient.Connect();
                progressClient.SetWorkingDirectory(remoteClient.GetWorkingDirectory());
            }

            sourceClient.SetDataType(sourceClient.FXPDataType);
            destinationClient.SetDataType(destinationClient.FXPDataType);

            // send PASV/CPSV commands to destination FTP server to get passive port to be used from source FTP server
            // first try with PASV - commonly supported by all servers
            if (!(reply = destinationClient.Execute("PASV")).Success)
            {

                // then try with CPSV - known to be supported by glFTPd server
                // FIXES #666 - glFTPd server - 435 Failed TLS negotiation on data channel
                if (!(reply2 = destinationClient.Execute("CPSV")).Success)
                {
                    throw new FtpCommandException(reply);
                }
                else
                {

                    // use the CPSV response and extract the port from it
                    reply = reply2;
                }
            }

            // extract port from response
            m = Regex.Match(reply.Message, @"(?<quad1>\d+)," + @"(?<quad2>\d+)," + @"(?<quad3>\d+)," + @"(?<quad4>\d+)," + @"(?<port1>\d+)," + @"(?<port2>\d+)");
            if (!m.Success || m.Groups.Count != 7)
            {
                throw new FtpException("Malformed PASV response: " + reply.Message);
            }

            // Instruct source server to open a connection to the destination Server

            if (!(reply = sourceClient.Execute($"PORT {m.Value}")).Success)
            {
                throw new FtpCommandException(reply);
            }

            // the FXP session stores the active connections used for this FXP transfer
            return new FtpFxpSession
            {
                SourceServer = sourceClient,
                TargetServer = destinationClient,
                ProgressServer = progressClient,
            };
        }

#if !NET40

        /// <summary>
        /// Opens a FXP PASV connection between the source FTP Server and the destination FTP Server
        /// </summary>
        /// <param name="remoteClient">Valid FTP connection to the destination FTP Server</param>
        /// <param name="trackProgress"></param>
        /// <param name="token"></param>
        /// <returns>A data stream ready to be used</returns>
        private async Task<FtpFxpSession> OpenPassiveFXPConnectionAsync(FtpClient remoteClient, bool trackProgress, CancellationToken token)
        {
            FtpReply reply, reply2;
            Match m;
            FtpClient sourceClient = null;
            FtpClient destinationClient = null;
            FtpClient progressClient = null;

            // create a new connection to the source FTP server if EnableThreadSafeDataConnections is set
            if (m_threadSafeDataChannels)
            {
                sourceClient = CloneConnection();
                sourceClient._AutoDispose = true;
                sourceClient.CopyStateFlags(this);
                await sourceClient.ConnectAsync(token);
                await sourceClient.SetWorkingDirectoryAsync(await GetWorkingDirectoryAsync(token), token);
            }
            else
            {
                sourceClient = this;
            }

            // create a new connection to the target FTP server if EnableThreadSafeDataConnections is set
            if (remoteClient.EnableThreadSafeDataConnections)
            {
                destinationClient = remoteClient.CloneConnection();
                destinationClient._AutoDispose = true;
                destinationClient.CopyStateFlags(remoteClient);
                await destinationClient.ConnectAsync(token);
                await destinationClient.SetWorkingDirectoryAsync(await remoteClient.GetWorkingDirectoryAsync(token), token);
            }
            else
            {
                destinationClient = remoteClient;
            }

            // create a new connection to the target FTP server to track progress
            // if progress tracking is enabled during this FXP transfer
            if (trackProgress)
            {
                progressClient = remoteClient.CloneConnection();
                progressClient._AutoDispose = true;
                progressClient.CopyStateFlags(remoteClient);
                await progressClient.ConnectAsync(token);
                await progressClient.SetWorkingDirectoryAsync(await remoteClient.GetWorkingDirectoryAsync(token), token);
            }

            await sourceClient.SetDataTypeAsync(sourceClient.FXPDataType, token);
            await destinationClient.SetDataTypeAsync(destinationClient.FXPDataType, token);

            // send PASV/CPSV commands to destination FTP server to get passive port to be used from source FTP server
            // first try with PASV - commonly supported by all servers
            if (!(reply = await destinationClient.ExecuteAsync("PASV", token)).Success)
            {

                // then try with CPSV - known to be supported by glFTPd server
                // FIXES #666 - glFTPd server - 435 Failed TLS negotiation on data channel
                if (!(reply2 = await destinationClient.ExecuteAsync("CPSV", token)).Success)
                {
                    throw new FtpCommandException(reply);
                }
                else
                {

                    // use the CPSV response and extract the port from it
                    reply = reply2;
                }
            }

            // extract port from response
            m = Regex.Match(reply.Message, @"(?<quad1>\d+)," + @"(?<quad2>\d+)," + @"(?<quad3>\d+)," + @"(?<quad4>\d+)," + @"(?<port1>\d+)," + @"(?<port2>\d+)");

            if (!m.Success || m.Groups.Count != 7)
            {
                throw new FtpException("Malformed PASV response: " + reply.Message);
            }

            // Instruct source server to open a connection to the destination Server

            if (!(reply = await sourceClient.ExecuteAsync($"PORT {m.Value}", token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // the FXP session stores the active connections used for this FXP transfer
            return new FtpFxpSession
            {
                SourceServer = sourceClient,
                TargetServer = destinationClient,
                ProgressServer = progressClient,
            };
        }

#endif

        /// <summary>
        /// Disposes and disconnects this FTP client if it was auto-created for an internal operation.
        /// </summary>
        public void AutoDispose()
        {
            if (_AutoDispose)
            {
                Dispose();
            }
        }
        #endregion // FtpClient_FXPConnection.cs
        #region // FtpClient_FXPFileTransfer.cs
        /// <summary>
        /// Transfer the specified file from the source FTP Server to the destination FTP Server using the FXP protocol.
        /// High-level API that takes care of various edge cases internally.
        /// </summary>
        /// <param name="sourcePath">The full or relative path to the file on the source FTP Server</param>
        /// <param name="remoteClient">Valid FTP connection to the destination FTP Server</param>
        /// <param name="remotePath">The full or relative path to destination file on the remote FTP Server</param>
        /// <param name="createRemoteDir">Indicates if the folder should be created on the remote FTP Server</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <param name="metaProgress"></param>
        /// Returns a FtpStatus indicating if the file was transfered.
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically be set to true for subsequent attempts.
        /// </remarks>
        public FtpStatus TransferFile(string sourcePath, FtpClient remoteClient, string remotePath,
            bool createRemoteDir = false, FtpRemoteExists existsMode = FtpRemoteExists.Resume, FtpVerify verifyOptions = FtpVerify.None, Action<FtpProgress> progress = null, FtpProgress metaProgress = null)
        {

            sourcePath = sourcePath.GetFtpPath();
            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(TransferFile), new object[] { sourcePath, remoteClient, remotePath, FXPDataType, createRemoteDir, existsMode, verifyOptions });

            // verify input params
            VerifyTransferFileParams(sourcePath, remoteClient, remotePath, existsMode);

            // ensure source file exists
            if (!FileExists(sourcePath))
            {
                throw new FtpException("Source File " + sourcePath + " cannot be found or does not exists!");
            }

            bool fxpSuccess;
            var verified = true;
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;
            do
            {

                fxpSuccess = TransferFileFXPInternal(sourcePath, remoteClient, remotePath, createRemoteDir, existsMode, progress, metaProgress is null ? new FtpProgress(1, 0) : metaProgress);
                attemptsLeft--;

                // if verification is needed
                if (fxpSuccess && verifyOptions != FtpVerify.None)
                {
                    verified = VerifyFXPTransfer(sourcePath, remoteClient, remotePath);
                    LogStatus(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                    if (!verified && attemptsLeft > 0)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode == FtpRemoteExists.Resume ? "  Overwrite will occur." : "") + "  " + attemptsLeft + " attempts remaining");
                        // Force overwrite if a retry is required
                        existsMode = FtpRemoteExists.Overwrite;
                    }
                }
            } while (!verified && attemptsLeft > 0);

            if (fxpSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                remoteClient.DeleteFile(remotePath);
            }

            if (fxpSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Destination file checksum value does not match source file");
            }

            return fxpSuccess && verified ? FtpStatus.Success : FtpStatus.Failed;

        }

        private void VerifyTransferFileParams(string sourcePath, FtpClient remoteClient, string remotePath, FtpRemoteExists existsMode)
        {
            if (remoteClient is null)
            {
                throw new ArgumentNullException(nameof(remoteClient), "Destination FXP FtpClient cannot be null!");
            }

            if (sourcePath.IsBlank())
            {
                throw new ArgumentNullException(nameof(sourcePath), "FtpListItem must be specified!");
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", nameof(remotePath));
            }

            if (!remoteClient.IsConnected)
            {
                throw new FtpException("The connection must be open before a transfer between servers can be initiated");
            }

            if (!this.IsConnected)
            {
                throw new FtpException("The source FXP FtpClient must be open and connected before a transfer between servers can be initiated");
            }

            if (existsMode == FtpRemoteExists.AddToEnd || existsMode == FtpRemoteExists.AddToEndNoCheck)
            {
                throw new ArgumentException("FXP file transfer does not currently support AddToEnd or AddToEndNoCheck modes. Use another value for existsMode.", nameof(existsMode));
            }
        }


#if !NET40
        /// <summary>
        /// Transfer the specified file from the source FTP Server to the destination FTP Server asynchronously using the FXP protocol.
        /// High-level API that takes care of various edge cases internally.
        /// </summary>
        /// <param name="sourcePath">The full or relative path to the file on the source FTP Server</param>
        /// <param name="remoteClient">Valid FTP connection to the destination FTP Server</param>
        /// <param name="remotePath">The full or relative path to destination file on the remote FTP Server</param>
        /// <param name="createRemoteDir">Indicates if the folder should be created on the remote FTP Server</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <param name="metaProgress"></param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// Returns a FtpStatus indicating if the file was transfered.
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically be set to true for subsequent attempts.
        /// </remarks>
        public async Task<FtpStatus> TransferFileAsync(string sourcePath, FtpClient remoteClient, string remotePath,
            bool createRemoteDir = false, FtpRemoteExists existsMode = FtpRemoteExists.Resume, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null, FtpProgress metaProgress = null, CancellationToken token = default(CancellationToken))
        {

            sourcePath = sourcePath.GetFtpPath();
            remotePath = remotePath.GetFtpPath();

            LogFunc(nameof(TransferFileAsync), new object[] { sourcePath, remoteClient, remotePath, FXPDataType, createRemoteDir, existsMode, verifyOptions });

            // verify input params
            VerifyTransferFileParams(sourcePath, remoteClient, remotePath, existsMode);

            // ensure source file exists
            if (!await FileExistsAsync(sourcePath, token))
            {
                throw new FtpException("Source File " + sourcePath + " cannot be found or does not exists!");
            }

            bool fxpSuccess;
            var verified = true;
            var attemptsLeft = verifyOptions.HasFlag(FtpVerify.Retry) ? m_retryAttempts : 1;
            do
            {

                fxpSuccess = await TransferFileFXPInternalAsync(sourcePath, remoteClient, remotePath, createRemoteDir, existsMode, progress, token, metaProgress is null ? new FtpProgress(1, 0) : metaProgress);
                attemptsLeft--;

                // if verification is needed
                if (fxpSuccess && verifyOptions != FtpVerify.None)
                {
                    verified = await VerifyFXPTransferAsync(sourcePath, remoteClient, remotePath, token);
                    LogStatus(FtpTraceLevel.Info, "File Verification: " + (verified ? "PASS" : "FAIL"));
                    if (!verified && attemptsLeft > 0)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "Retrying due to failed verification." + (existsMode == FtpRemoteExists.Resume ? "  Overwrite will occur." : "") + "  " + attemptsLeft + " attempts remaining");
                        // Force overwrite if a retry is required
                        existsMode = FtpRemoteExists.Overwrite;
                    }
                }
            } while (!verified && attemptsLeft > 0);

            if (fxpSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Delete))
            {
                await remoteClient.DeleteFileAsync(remotePath, token);
            }

            if (fxpSuccess && !verified && verifyOptions.HasFlag(FtpVerify.Throw))
            {
                throw new FtpException("Destination file checksum value does not match source file");
            }

            return fxpSuccess && verified ? FtpStatus.Success : FtpStatus.Failed;

        }
#endif

        /// <summary>
        /// Transfers a file from the source FTP Server to the destination FTP Server via the FXP protocol
        /// </summary>
        private bool TransferFileFXPInternal(string sourcePath, FtpClient remoteClient, string remotePath, bool createRemoteDir, FtpRemoteExists existsMode,
            Action<FtpProgress> progress, FtpProgress metaProgress)
        {

            FtpReply reply;
            long offset = 0;
            bool fileExists = false;
            long fileSize = 0;

            var ftpFxpSession = OpenPassiveFXPConnection(remoteClient, progress != null);

            if (ftpFxpSession != null)
            {
                try
                {

                    ftpFxpSession.SourceServer.ReadTimeout = (int)TimeSpan.FromMinutes(30.0).TotalMilliseconds;
                    ftpFxpSession.TargetServer.ReadTimeout = (int)TimeSpan.FromMinutes(30.0).TotalMilliseconds;


                    // check if the file exists, and skip, overwrite or append
                    if (existsMode == FtpRemoteExists.ResumeNoCheck)
                    {
                        offset = remoteClient.GetFileSize(remotePath);
                        if (offset == -1)
                        {
                            offset = 0; // start from the beginning
                        }
                    }
                    else
                    {
                        fileExists = remoteClient.FileExists(remotePath);

                        switch (existsMode)
                        {
                            case FtpRemoteExists.Skip:

                                if (fileExists)
                                {
                                    LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists (Source: " + sourcePath + ", Dest: " + remotePath + ")");

                                    //Fix #413 - progress callback isn't called if the file has already been uploaded to the server
                                    //send progress reports
                                    if (progress != null)
                                    {
                                        progress(new FtpProgress(100.0, 0, 0, TimeSpan.FromSeconds(0), sourcePath, remotePath, metaProgress));
                                    }

                                    return true;
                                }

                                break;

                            case FtpRemoteExists.Overwrite:

                                if (fileExists)
                                {
                                    remoteClient.DeleteFile(remotePath);
                                }

                                break;

                            case FtpRemoteExists.Resume:

                                if (fileExists)
                                {
                                    offset = remoteClient.GetFileSize(remotePath);
                                    if (offset == -1)
                                    {
                                        offset = 0; // start from the beginning
                                    }
                                }

                                break;
                        }

                    }

                    fileSize = GetFileSize(sourcePath);

                    // ensure the remote dir exists .. only if the file does not already exist!
                    if (createRemoteDir && !fileExists)
                    {
                        var dirname = remotePath.GetFtpDirectoryName();
                        if (!remoteClient.DirectoryExists(dirname))
                        {
                            remoteClient.CreateDirectory(dirname);
                        }
                    }

                    if (offset == 0 && existsMode != FtpRemoteExists.ResumeNoCheck)
                    {
                        // send command to tell the source server to 'send' the file to the destination server
                        if (!(reply = ftpFxpSession.SourceServer.Execute($"RETR {sourcePath}")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        //Instruct destination server to store the file
                        if (!(reply = ftpFxpSession.TargetServer.Execute($"STOR {remotePath}")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }
                    }
                    else
                    {
                        //tell source server to restart / resume
                        if (!(reply = ftpFxpSession.SourceServer.Execute($"REST {offset}")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        // send command to tell the source server to 'send' the file to the destination server
                        if (!(reply = ftpFxpSession.SourceServer.Execute($"RETR {sourcePath}")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        //Instruct destination server to append the file
                        if (!(reply = ftpFxpSession.TargetServer.Execute($"APPE {remotePath}")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }
                    }

                    var transferStarted = DateTime.Now;
                    long lastSize = 0;

                    var sourceFXPTransferReply = ftpFxpSession.SourceServer.GetReply();
                    var destinationFXPTransferReply = ftpFxpSession.TargetServer.GetReply();

                    // while the transfer is not complete
                    while (!sourceFXPTransferReply.Success || !destinationFXPTransferReply.Success)
                    {

                        // send progress reports every 1 second
                        if (ftpFxpSession.ProgressServer != null)
                        {

                            // send progress reports
                            if (progress != null && fileSize != -1)
                            {
                                offset = ftpFxpSession.ProgressServer.GetFileSize(remotePath);

                                if (offset != -1 && lastSize <= offset)
                                {
                                    long bytesProcessed = offset - lastSize;
                                    lastSize = offset;
                                    ReportProgress(progress, fileSize, offset, bytesProcessed, DateTime.Now - transferStarted, sourcePath, remotePath, metaProgress);
                                }
                            }
                        }
                        Thread.Sleep(FXPProgressInterval); // Task.Delay(FXPProgressInterval);
                    }

                    FtpTrace.WriteLine(FtpTraceLevel.Info, $"FXP transfer of file {sourcePath} has completed");

                    Noop();
                    remoteClient.Noop();

                    ftpFxpSession.Dispose();

                    return true;

                }

                // Fix: catch all exceptions and dispose off the FTP clients if one occurs
                catch (Exception)
                {
                    ftpFxpSession.Dispose();
                    throw;
                }
            }
            else
            {
                FtpTrace.WriteLine(FtpTraceLevel.Error, "Failed to open FXP passive Connection");
                return false;
            }
        }


#if !NET40
        /// <summary>
        /// Transfers a file from the source FTP Server to the destination FTP Server via the FXP protocol asynchronously.
        /// </summary>
        private async Task<bool> TransferFileFXPInternalAsync(string sourcePath, FtpClient remoteClient, string remotePath, bool createRemoteDir, FtpRemoteExists existsMode,
            IProgress<FtpProgress> progress, CancellationToken token, FtpProgress metaProgress)
        {
            FtpReply reply;
            long offset = 0;
            bool fileExists = false;
            long fileSize = 0;

            var ftpFxpSession = await OpenPassiveFXPConnectionAsync(remoteClient, progress != null, token);

            if (ftpFxpSession != null)
            {

                try
                {

                    ftpFxpSession.SourceServer.ReadTimeout = (int)TimeSpan.FromMinutes(30.0).TotalMilliseconds;
                    ftpFxpSession.TargetServer.ReadTimeout = (int)TimeSpan.FromMinutes(30.0).TotalMilliseconds;


                    // check if the file exists, and skip, overwrite or append
                    if (existsMode == FtpRemoteExists.ResumeNoCheck)
                    {
                        offset = await remoteClient.GetFileSizeAsync(remotePath, -1, token);
                        if (offset == -1)
                        {
                            offset = 0; // start from the beginning
                        }
                    }
                    else
                    {
                        fileExists = await remoteClient.FileExistsAsync(remotePath, token);

                        switch (existsMode)
                        {
                            case FtpRemoteExists.Skip:

                                if (fileExists)
                                {
                                    LogStatus(FtpTraceLevel.Info, "Skipping file because Skip is enabled and file already exists (Source: " + sourcePath + ", Dest: " + remotePath + ")");

                                    //Fix #413 - progress callback isn't called if the file has already been uploaded to the server
                                    //send progress reports
                                    if (progress != null)
                                    {
                                        progress.Report(new FtpProgress(100.0, 0, 0, TimeSpan.FromSeconds(0), sourcePath, remotePath, metaProgress));
                                    }

                                    return true;
                                }

                                break;

                            case FtpRemoteExists.Overwrite:

                                if (fileExists)
                                {
                                    await remoteClient.DeleteFileAsync(remotePath, token);
                                }

                                break;

                            case FtpRemoteExists.Resume:

                                if (fileExists)
                                {
                                    offset = await remoteClient.GetFileSizeAsync(remotePath, 0, token);
                                }

                                break;
                        }

                    }

                    fileSize = await GetFileSizeAsync(sourcePath, -1, token);

                    // ensure the remote dir exists .. only if the file does not already exist!
                    if (createRemoteDir && !fileExists)
                    {
                        var dirname = remotePath.GetFtpDirectoryName();
                        if (!await remoteClient.DirectoryExistsAsync(dirname, token))
                        {
                            await remoteClient.CreateDirectoryAsync(dirname, token);
                        }
                    }

                    if (offset == 0 && existsMode != FtpRemoteExists.ResumeNoCheck)
                    {
                        // send command to tell the source server to 'send' the file to the destination server
                        if (!(reply = await ftpFxpSession.SourceServer.ExecuteAsync($"RETR {sourcePath}", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        //Instruct destination server to store the file
                        if (!(reply = await ftpFxpSession.TargetServer.ExecuteAsync($"STOR {remotePath}", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }
                    }
                    else
                    {
                        //tell source server to restart / resume
                        if (!(reply = await ftpFxpSession.SourceServer.ExecuteAsync($"REST {offset}", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        // send command to tell the source server to 'send' the file to the destination server
                        if (!(reply = await ftpFxpSession.SourceServer.ExecuteAsync($"RETR {sourcePath}", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        //Instruct destination server to append the file
                        if (!(reply = await ftpFxpSession.TargetServer.ExecuteAsync($"APPE {remotePath}", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }
                    }

                    var transferStarted = DateTime.Now;
                    long lastSize = 0;


                    var sourceFXPTransferReply = ftpFxpSession.SourceServer.GetReplyAsync(token);
                    var destinationFXPTransferReply = ftpFxpSession.TargetServer.GetReplyAsync(token);

                    // while the transfer is not complete
                    while (!sourceFXPTransferReply.IsCompleted || !destinationFXPTransferReply.IsCompleted)
                    {

                        // send progress reports every 1 second
                        if (ftpFxpSession.ProgressServer != null)
                        {

                            // send progress reports
                            if (progress != null && fileSize != -1)
                            {
                                offset = await ftpFxpSession.ProgressServer.GetFileSizeAsync(remotePath, -1, token);

                                if (offset != -1 && lastSize <= offset)
                                {
                                    long bytesProcessed = offset - lastSize;
                                    lastSize = offset;
                                    ReportProgress(progress, fileSize, offset, bytesProcessed, DateTime.Now - transferStarted, sourcePath, remotePath, metaProgress);
                                }
                            }
                        }

                        await Task.Delay(FXPProgressInterval, token);
                    }

                    FtpTrace.WriteLine(FtpTraceLevel.Info, $"FXP transfer of file {sourcePath} has completed");

                    await NoopAsync(token);
                    await remoteClient.NoopAsync(token);

                    ftpFxpSession.Dispose();

                    return true;

                }

                // Fix: catch all exceptions and dispose off the FTP clients if one occurs
                catch (Exception)
                {
                    ftpFxpSession.Dispose();
                    throw;
                }
            }
            else
            {
                FtpTrace.WriteLine(FtpTraceLevel.Error, "Failed to open FXP passive Connection");
                return false;
            }

        }
#endif

        #endregion // FtpClient_FXPFileTransfer.cs
        #region // FtpClient_FXPFolderTransfer.cs
        /// <summary>
        /// Transfer the specified directory from the source FTP Server onto the remote FTP Server using the FXP protocol.
        /// You will need to create a valid connection to your remote FTP Server before calling this method.
        /// In Update mode, we will only transfer missing files and preserve any extra files on the remote FTP Server. This is useful when you want to simply transfer missing files from an FTP directory.
        /// Currently Mirror mode is not implemented.
        /// Only transfers the files and folders matching all the rules provided, if any.
        /// All exceptions during transfer are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="sourceFolder">The full or relative path to the folder on the source FTP Server. If it does not exist, an empty result list is returned.</param>
        /// <param name="remoteClient">Valid FTP connection to the destination FTP Server</param>
        /// <param name="remoteFolder">The full or relative path to destination folder on the remote FTP Server</param>
        /// <param name="mode">Only Update mode is currently implemented</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public List<FtpResult> TransferDirectory(string sourceFolder, FtpClient remoteClient, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, Action<FtpProgress> progress = null)
        {

            if (sourceFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "sourceFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // cleanup the FTP paths
            sourceFolder = sourceFolder.GetFtpPath().EnsurePostfix("/");
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(TransferDirectory), new object[] { sourceFolder, remoteClient, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the source dir does not exist, fail fast
            if (!DirectoryExists(sourceFolder))
            {
                return results;
            }

            // flag to determine if existence checks are required
            var checkFileExistence = true;

            // ensure the remote dir exists
            if (!remoteClient.DirectoryExists(remoteFolder))
            {
                remoteClient.CreateDirectory(remoteFolder);
                checkFileExistence = false;
            }

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // get all the folders in the local directory
            var dirListing = GetListing(sourceFolder, FtpListOption.Recursive).Where(x => x.Type == FtpFileSystemObjectType.Directory).Select(x => x.FullName).ToArray();

            // get all the already existing files
            var remoteListing = checkFileExistence ? remoteClient.GetListing(remoteFolder, FtpListOption.Recursive) : null;

            // loop thru each folder and ensure it exists #1
            var dirsToUpload = GetSubDirectoriesToTransfer(sourceFolder, remoteFolder, rules, results, dirListing);
            CreateSubDirectories(remoteClient, dirsToUpload);

            // get all the files in the local directory
            var fileListing = GetListing(sourceFolder, FtpListOption.Recursive).Where(x => x.Type == FtpFileSystemObjectType.File).Select(x => x.FullName).ToArray();

            // loop thru each file and transfer it
            var filesToUpload = GetFilesToTransfer(sourceFolder, remoteFolder, rules, results, shouldExist, fileListing);
            TransferServerFiles(filesToUpload, remoteClient, existsMode, verifyOptions, progress, remoteListing);

            // delete the extra remote files if in mirror mode and the directory was pre-existing
            // DeleteExtraServerFiles(mode, shouldExist, remoteListing);

            return results;
        }

#if !NET40

        /// <summary>
        /// Transfer the specified directory from the source FTP Server onto the remote FTP Server asynchronously using the FXP protocol.
        /// You will need to create a valid connection to your remote FTP Server before calling this method.
        /// In Update mode, we will only transfer missing files and preserve any extra files on the remote FTP Server. This is useful when you want to simply transfer missing files from an FTP directory.
        /// Currently Mirror mode is not implemented.
        /// Only transfers the files and folders matching all the rules provided, if any.
        /// All exceptions during transfer are caught, and the exception is stored in the related FtpResult object.
        /// </summary>
        /// <param name="sourceFolder">The full or relative path to the folder on the source FTP Server. If it does not exist, an empty result list is returned.</param>
        /// <param name="remoteClient">Valid FTP connection to the destination FTP Server</param>
        /// <param name="remoteFolder">The full or relative path to destination folder on the remote FTP Server</param>
        /// <param name="mode">Only Update mode is currently implemented</param>
        /// <param name="existsMode">If the file exists on disk, should we skip it, resume the download or restart the download?</param>
        /// <param name="verifyOptions">Sets if checksum verification is required for a successful download and what to do if it fails verification (See Remarks)</param>
        /// <param name="rules">Only files and folders that pass all these rules are downloaded, and the files that don't pass are skipped. In the Mirror mode, the files that fail the rules are also deleted from the local folder.</param>
        /// <param name="progress">Provide a callback to track download progress.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <remarks>
        /// If verification is enabled (All options other than <see cref="FtpVerify.None"/>) the hash will be checked against the server.  If the server does not support
        /// any hash algorithm, then verification is ignored.  If only <see cref="FtpVerify.OnlyChecksum"/> is set then the return of this method depends on both a successful 
        /// upload &amp; verification.  Additionally, if any verify option is set and a retry is attempted then overwrite will automatically switch to true for subsequent attempts.
        /// If <see cref="FtpVerify.Throw"/> is set and <see cref="FtpError.Throw"/> is <i>not set</i>, then individual verification errors will not cause an exception
        /// to propagate from this method.
        /// </remarks>
        /// <returns>
        /// Returns a listing of all the remote files, indicating if they were downloaded, skipped or overwritten.
        /// Returns a blank list if nothing was transfered. Never returns null.
        /// </returns>
        public async Task<List<FtpResult>> TransferDirectoryAsync(string sourceFolder, FtpClient remoteClient, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update,
            FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = default(CancellationToken))
        {

            if (sourceFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "sourceFolder");
            }

            if (remoteFolder.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "remoteFolder");
            }

            // cleanup the FTP paths
            sourceFolder = sourceFolder.GetFtpPath().EnsurePostfix("/");
            remoteFolder = remoteFolder.GetFtpPath().EnsurePostfix("/");

            LogFunc(nameof(TransferDirectoryAsync), new object[] { sourceFolder, remoteClient, remoteFolder, mode, existsMode, verifyOptions, (rules.IsBlank() ? null : rules.Count + " rules") });

            var results = new List<FtpResult>();

            // if the source dir does not exist, fail fast
            if (!await DirectoryExistsAsync(sourceFolder, token))
            {
                return results;
            }

            // flag to determine if existence checks are required
            var checkFileExistence = true;

            // ensure the remote dir exists
            if (!await remoteClient.DirectoryExistsAsync(remoteFolder, token))
            {
                await remoteClient.CreateDirectoryAsync(remoteFolder, token);
                checkFileExistence = false;
            }

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // collect paths of the files that should exist (lowercase for CI checks)
            var shouldExist = new Dictionary<string, bool>();

            // get all the folders in the local directory
            var dirListing = (await GetListingAsync(sourceFolder, FtpListOption.Recursive, token)).Where(x => x.Type == FtpFileSystemObjectType.Directory).Select(x => x.FullName).ToArray();

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // get all the already existing files
            var remoteListing = checkFileExistence ? await remoteClient.GetListingAsync(remoteFolder, FtpListOption.Recursive, token) : null;

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            // loop thru each folder and ensure it exists #1
            var dirsToUpload = GetSubDirectoriesToTransfer(sourceFolder, remoteFolder, rules, results, dirListing);

            // break if task is cancelled
            token.ThrowIfCancellationRequested();

            /*-------------------------------------------------------------------------------------/
			 *   Cancelling after this point would leave the FTP server in an inconsistent state   *
			 *-------------------------------------------------------------------------------------*/

            // loop thru each folder and ensure it exists #2
            await CreateSubDirectoriesAsync(remoteClient, dirsToUpload, token);

            // get all the files in the local directory
            var fileListing = (await GetListingAsync(sourceFolder, FtpListOption.Recursive, token)).Where(x => x.Type == FtpFileSystemObjectType.File).Select(x => x.FullName).ToArray();

            // loop thru each file and transfer it
            var filesToUpload = GetFilesToTransfer(sourceFolder, remoteFolder, rules, results, shouldExist, fileListing);
            await TransferServerFilesAsync(filesToUpload, remoteClient, existsMode, verifyOptions, progress, remoteListing, token);

            // delete the extra remote files if in mirror mode and the directory was pre-existing
            // DeleteExtraServerFiles(mode, shouldExist, remoteListing);

            return results;
        }
#endif

        private List<FtpResult> GetSubDirectoriesToTransfer(string sourceFolder, string remoteFolder, List<FtpRule> rules, List<FtpResult> results, string[] dirListing)
        {

            var dirsToTransfer = new List<FtpResult>();

            foreach (var sourceFile in dirListing)
            {

                // calculate the local path
                var relativePath = sourceFile.Replace(sourceFolder, "").EnsurePostfix("/");
                var remoteFile = remoteFolder + relativePath;

                // create the result object
                var result = new FtpResult
                {
                    Type = FtpFileSystemObjectType.Directory,
                    Size = 0,
                    Name = sourceFile.GetFtpDirectoryName(),
                    RemotePath = remoteFile,
                    LocalPath = sourceFile,
                    IsDownload = false,
                };

                // record the folder
                results.Add(result);

                // skip transferring the file if it does not pass all the rules
                if (!FilePassesRules(result, rules, true))
                {
                    continue;
                }

                dirsToTransfer.Add(result);
            }

            return dirsToTransfer;
        }

        private List<FtpResult> GetFilesToTransfer(string sourceFolder, string remoteFolder, List<FtpRule> rules, List<FtpResult> results, Dictionary<string, bool> shouldExist, string[] fileListing)
        {

            var filesToTransfer = new List<FtpResult>();

            foreach (var sourceFile in fileListing)
            {

                // calculate the local path
                var relativePath = sourceFile.Replace(sourceFolder, "");
                var remoteFile = remoteFolder + relativePath;

                // create the result object
                var result = new FtpResult
                {
                    Type = FtpFileSystemObjectType.File,
                    Size = GetFileSize(sourceFile),
                    Name = sourceFile.GetFtpFileName(),
                    RemotePath = remoteFile,
                    LocalPath = sourceFile
                };

                // record the file
                results.Add(result);

                // skip transferring the file if it does not pass all the rules
                if (!FilePassesRules(result, rules, true))
                {
                    continue;
                }

                // record that this file should exist
                shouldExist.Add(remoteFile.ToLowerInvariant(), true);

                // absorb errors
                filesToTransfer.Add(result);
            }

            return filesToTransfer;
        }

        private void TransferServerFiles(List<FtpResult> filesToTransfer, FtpClient remoteClient, FtpRemoteExists existsMode, FtpVerify verifyOptions, Action<FtpProgress> progress, FtpListItem[] remoteListing)
        {

            LogFunc(nameof(TransferServerFiles), new object[] { filesToTransfer.Count + " files" });

            int r = -1;
            foreach (var result in filesToTransfer)
            {
                r++;

                // absorb errors
                try
                {

                    // skip uploading if the file already exists on the server
                    FtpRemoteExists existsModeToUse;
                    if (!CanUploadFile(result, remoteListing, existsMode, out existsModeToUse))
                    {
                        continue;
                    }

                    // create meta progress to store the file progress
                    var metaProgress = new FtpProgress(filesToTransfer.Count, r);

                    // transfer the file
                    var transferred = TransferFile(result.LocalPath, remoteClient, result.RemotePath, false, existsModeToUse, verifyOptions, progress, metaProgress);
                    result.IsSuccess = transferred.IsSuccess();
                    result.IsSkipped = transferred == FtpStatus.Skipped;

                }
                catch (Exception ex)
                {

                    LogStatus(FtpTraceLevel.Warn, "File failed to transfer: " + result.LocalPath);

                    // mark that the file failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }

        }

#if !NET40

        private async Task TransferServerFilesAsync(List<FtpResult> filesToTransfer, FtpClient remoteClient, FtpRemoteExists existsMode, FtpVerify verifyOptions, IProgress<FtpProgress> progress, FtpListItem[] remoteListing, CancellationToken token)
        {

            LogFunc(nameof(TransferServerFilesAsync), new object[] { filesToTransfer.Count + " files" });

            int r = -1;
            foreach (var result in filesToTransfer)
            {
                r++;

                // absorb errors
                try
                {

                    // skip uploading if the file already exists on the server
                    FtpRemoteExists existsModeToUse;
                    if (!CanUploadFile(result, remoteListing, existsMode, out existsModeToUse))
                    {
                        continue;
                    }

                    // create meta progress to store the file progress
                    var metaProgress = new FtpProgress(filesToTransfer.Count, r);

                    // transfer the file
                    var transferred = await TransferFileAsync(result.LocalPath, remoteClient, result.RemotePath, false, existsModeToUse, verifyOptions, progress, metaProgress, token);
                    result.IsSuccess = transferred.IsSuccess();
                    result.IsSkipped = transferred == FtpStatus.Skipped;

                }
                catch (Exception ex)
                {

                    LogStatus(FtpTraceLevel.Warn, "File failed to transfer: " + result.LocalPath);

                    // mark that the file failed to upload
                    result.IsFailed = true;
                    result.Exception = ex;
                }
            }

        }

#endif
        #endregion // FtpClient_FXPFolderTransfer.cs
        #region // FtpClient_FXPVerification.cs
        private bool VerifyFXPTransfer(string sourcePath, FtpClient fxpDestinationClient, string remotePath)
        {

            // verify args
            if (sourcePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", nameof(sourcePath));
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", nameof(remotePath));
            }

            if (fxpDestinationClient is null)
            {
                throw new ArgumentNullException(nameof(fxpDestinationClient), "Destination FXP FtpClient cannot be null!");
            }

            // check if any algorithm is supported by both servers
            var algorithm = GetFirstMutualChecksum(fxpDestinationClient);
            if (algorithm != FtpHashAlgorithm.NONE)
            {

                // get the hashes of both files using the same mutual algorithm

                FtpHash sourceHash = GetChecksum(sourcePath, algorithm);
                if (!sourceHash.IsValid)
                {
                    return false;
                }

                FtpHash destinationHash = fxpDestinationClient.GetChecksum(remotePath, algorithm);
                if (!destinationHash.IsValid)
                {
                    return false;
                }

                return sourceHash.Value == destinationHash.Value;
            }
            else
            {
                LogLine(FtpTraceLevel.Info, "Source and Destination servers do not support any common hashing algorithm");
            }

            // since not supported return true to ignore validation
            return true;
        }

#if !NET40
        private async Task<bool> VerifyFXPTransferAsync(string sourcePath, FtpClient fxpDestinationClient, string remotePath, CancellationToken token = default(CancellationToken))
        {

            // verify args
            if (sourcePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", nameof(sourcePath));
            }

            if (remotePath.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", nameof(remotePath));
            }

            if (fxpDestinationClient is null)
            {
                throw new ArgumentNullException(nameof(fxpDestinationClient), "Destination FXP FtpClient cannot be null!");
            }

            // check if any algorithm is supported by both servers
            var algorithm = GetFirstMutualChecksum(fxpDestinationClient);
            if (algorithm != FtpHashAlgorithm.NONE)
            {

                // get the hashes of both files using the same mutual algorithm

                FtpHash sourceHash = await GetChecksumAsync(sourcePath, algorithm, token);
                if (!sourceHash.IsValid)
                {
                    return false;
                }

                FtpHash destinationHash = await fxpDestinationClient.GetChecksumAsync(remotePath, algorithm, token);
                if (!destinationHash.IsValid)
                {
                    return false;
                }

                return sourceHash.Value == destinationHash.Value;
            }
            else
            {
                LogLine(FtpTraceLevel.Info, "Source and Destination servers do not support any common hashing algorithm");
            }

            // since not supported return true to ignore validation
            return true;
        }

#endif
        #endregion // FtpClient_FXPVerification.cs
        #region // FtpClient_Hash.cs
        #region Checksum

        /// <summary>
        /// Retrieves a checksum of the given file using the specified checksum algorithm, or using the first available algorithm that the server supports.
        /// </summary>
        /// <remarks>
        /// The algorithm used goes in this order:
        /// 1. HASH command using the first supported algorithm.
        /// 2. MD5 / XMD5 / MMD5 commands
        /// 3. XSHA1 command
        /// 4. XSHA256 command
        /// 5. XSHA512 command
        /// 6. XCRC command
        /// </remarks>
        /// <param name="path">Full or relative path of the file to checksum</param>
        /// <param name="algorithm">Specify an algorithm that you prefer, or NONE to use the first available algorithm. If the preferred algorithm is not supported, a blank hash is returned.</param>
        /// <returns><see cref="FtpHash"/> object containing the value and algorithm. Use the <see cref="FtpHash.IsValid"/> property to
        /// determine if this command was successful. <see cref="FtpCommandException"/>s can be thrown from
        /// the underlying calls.</returns>
        /// <exception cref="FtpCommandException">The command fails</exception>
        public FtpHash GetChecksum(string path, FtpHashAlgorithm algorithm = FtpHashAlgorithm.NONE)
        {

            if (path == null)
            {
                throw new ArgumentException("Required argument is null", "path");
            }

            ValidateHashAlgorithm(algorithm);

            path = path.GetFtpPath();

            LogFunc(nameof(GetChecksum), new object[] { path });

            var useFirst = (algorithm == FtpHashAlgorithm.NONE);

            // if HASH is supported and the caller prefers an algorithm and that algorithm is supported
            if (HasFeature(FtpCapability.HASH) && !useFirst && HashAlgorithms.HasFlag(algorithm))
            {

                // switch to that algorithm
                SetHashAlgorithmInternal(algorithm);

                // get the hash of the file using HASH Command
                return HashCommandInternal(path);

            }

            // if HASH is supported and the caller does not prefer any specific algorithm
            else if (HasFeature(FtpCapability.HASH) && useFirst)
            {

                // switch to the first preferred algorithm
                SetHashAlgorithmInternal(HashAlgos.FirstSupported(HashAlgorithms));

                // get the hash of the file using HASH Command
                return HashCommandInternal(path);
            }
            else
            {
                var result = new FtpHash();

                // execute the first available algorithm, or the preferred algorithm if specified

                if (HasFeature(FtpCapability.MD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = GetHashInternal(path, "MD5");
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.XMD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = GetHashInternal(path, "XMD5");
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.MMD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = GetHashInternal(path, "MMD5");
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.XSHA1) && (useFirst || algorithm == FtpHashAlgorithm.SHA1))
                {
                    result.Value = GetHashInternal(path, "XSHA1");
                    result.Algorithm = FtpHashAlgorithm.SHA1;
                }
                else if (HasFeature(FtpCapability.XSHA256) && (useFirst || algorithm == FtpHashAlgorithm.SHA256))
                {
                    result.Value = GetHashInternal(path, "XSHA256");
                    result.Algorithm = FtpHashAlgorithm.SHA256;
                }
                else if (HasFeature(FtpCapability.XSHA512) && (useFirst || algorithm == FtpHashAlgorithm.SHA512))
                {
                    result.Value = GetHashInternal(path, "XSHA512");
                    result.Algorithm = FtpHashAlgorithm.SHA512;
                }
                else if (HasFeature(FtpCapability.XCRC) && (useFirst || algorithm == FtpHashAlgorithm.CRC))
                {
                    result.Value = GetHashInternal(path, "XCRC");
                    result.Algorithm = FtpHashAlgorithm.CRC;
                }

                return result;
            }
        }

        private void ValidateHashAlgorithm(FtpHashAlgorithm algorithm)
        {

            // if NO hashing algos or commands supported, throw here
            if (!HasFeature(FtpCapability.HASH) &&
                !HasFeature(FtpCapability.MD5) &&
                !HasFeature(FtpCapability.XMD5) &&
                !HasFeature(FtpCapability.MMD5) &&
                !HasFeature(FtpCapability.XSHA1) &&
                !HasFeature(FtpCapability.XSHA256) &&
                !HasFeature(FtpCapability.XSHA512) &&
                !HasFeature(FtpCapability.XCRC))
            {
                throw new FtpHashUnsupportedException();
            }

            // only if the user has specified a certain hash algorithm
            var useFirst = (algorithm == FtpHashAlgorithm.NONE);
            if (!useFirst)
            {

                // first check if the HASH command supports the required algo
                if (HasFeature(FtpCapability.HASH) && HashAlgorithms.HasFlag(algorithm))
                {

                    // we are good

                }
                else
                {

                    // second check if the special FTP command is supported based on the algo
                    if (algorithm == FtpHashAlgorithm.MD5 && !HasFeature(FtpCapability.MD5) &&
                        !HasFeature(FtpCapability.XMD5) && !HasFeature(FtpCapability.MMD5))
                    {
                        throw new FtpHashUnsupportedException(FtpHashAlgorithm.MD5, "MD5, XMD5, MMD5");
                    }
                    if (algorithm == FtpHashAlgorithm.SHA1 && !HasFeature(FtpCapability.XSHA1))
                    {
                        throw new FtpHashUnsupportedException(FtpHashAlgorithm.SHA1, "XSHA1");
                    }
                    if (algorithm == FtpHashAlgorithm.SHA256 && !HasFeature(FtpCapability.XSHA256))
                    {
                        throw new FtpHashUnsupportedException(FtpHashAlgorithm.SHA256, "XSHA256");
                    }
                    if (algorithm == FtpHashAlgorithm.SHA512 && !HasFeature(FtpCapability.XSHA512))
                    {
                        throw new FtpHashUnsupportedException(FtpHashAlgorithm.SHA512, "XSHA512");
                    }
                    if (algorithm == FtpHashAlgorithm.CRC && !HasFeature(FtpCapability.XCRC))
                    {
                        throw new FtpHashUnsupportedException(FtpHashAlgorithm.CRC, "XCRC");
                    }

                    // we are good
                }
            }
        }

#if !NET40
        /// <summary>
        /// Retrieves a checksum of the given file using the specified checksum algorithm, or using the first available algorithm that the server supports.
        /// </summary>
        /// <remarks>
        /// The algorithm used goes in this order:
        /// 1. HASH command using the first supported algorithm.
        /// 2. MD5 / XMD5 / MMD5 commands
        /// 3. XSHA1 command
        /// 4. XSHA256 command
        /// 5. XSHA512 command
        /// 6. XCRC command
        /// </remarks>
        /// <param name="path">Full or relative path of the file to checksum</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="algorithm">Specify an algorithm that you prefer, or NONE to use the first available algorithm. If the preferred algorithm is not supported, a blank hash is returned.</param>
        /// <returns><see cref="FtpHash"/> object containing the value and algorithm. Use the <see cref="FtpHash.IsValid"/> property to
        /// determine if this command was successful. <see cref="FtpCommandException"/>s can be thrown from
        /// the underlying calls.</returns>
        /// <exception cref="FtpCommandException">The command fails</exception>
        public async Task<FtpHash> GetChecksumAsync(string path, FtpHashAlgorithm algorithm = FtpHashAlgorithm.NONE, CancellationToken token = default(CancellationToken))
        {

            if (path == null)
            {
                throw new ArgumentException("Required argument is null", "path");
            }

            ValidateHashAlgorithm(algorithm);

            path = path.GetFtpPath();

            LogFunc(nameof(GetChecksumAsync), new object[] { path });

            var useFirst = (algorithm == FtpHashAlgorithm.NONE);

            // if HASH is supported and the caller prefers an algorithm and that algorithm is supported
            if (HasFeature(FtpCapability.HASH) && !useFirst && HashAlgorithms.HasFlag(algorithm))
            {

                // switch to that algorithm
                await SetHashAlgorithmInternalAsync(algorithm, token);

                // get the hash of the file using HASH Command
                return await HashCommandInternalAsync(path, token);

            }

            // if HASH is supported and the caller does not prefer any specific algorithm
            else if (HasFeature(FtpCapability.HASH) && useFirst)
            {

                // switch to the first preferred algorithm
                await SetHashAlgorithmInternalAsync(HashAlgos.FirstSupported(HashAlgorithms), token);

                // get the hash of the file using HASH Command
                return await HashCommandInternalAsync(path, token);
            }

            else
            {
                var result = new FtpHash();

                // execute the first available algorithm, or the preferred algorithm if specified

                if (HasFeature(FtpCapability.MD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = await GetHashInternalAsync(path, "MD5", token);
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.XMD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = await GetHashInternalAsync(path, "XMD5", token);
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.MMD5) && (useFirst || algorithm == FtpHashAlgorithm.MD5))
                {
                    result.Value = await GetHashInternalAsync(path, "MMD5", token);
                    result.Algorithm = FtpHashAlgorithm.MD5;
                }
                else if (HasFeature(FtpCapability.XSHA1) && (useFirst || algorithm == FtpHashAlgorithm.SHA1))
                {
                    result.Value = await GetHashInternalAsync(path, "XSHA1", token);
                    result.Algorithm = FtpHashAlgorithm.SHA1;
                }
                else if (HasFeature(FtpCapability.XSHA256) && (useFirst || algorithm == FtpHashAlgorithm.SHA256))
                {
                    result.Value = await GetHashInternalAsync(path, "XSHA256", token);
                    result.Algorithm = FtpHashAlgorithm.SHA256;
                }
                else if (HasFeature(FtpCapability.XSHA512) && (useFirst || algorithm == FtpHashAlgorithm.SHA512))
                {
                    result.Value = await GetHashInternalAsync(path, "XSHA512", token);
                    result.Algorithm = FtpHashAlgorithm.SHA512;
                }
                else if (HasFeature(FtpCapability.XCRC) && (useFirst || algorithm == FtpHashAlgorithm.CRC))
                {
                    result.Value = await GetHashInternalAsync(path, "XCRC", token);
                    result.Algorithm = FtpHashAlgorithm.CRC;
                }

                return result;
            }
        }
#endif

        #endregion

        #region MD5, SHA1, SHA256, SHA512 Commands

        /// <summary>
        /// Gets the hash of the specified file using the given command.
        /// </summary>
        internal string GetHashInternal(string path, string command)
        {
            FtpReply reply;
            string response;

            if (!(reply = Execute(command + " " + path)).Success)
            {
                throw new FtpCommandException(reply);
            }

            response = reply.Message;
            response = CleanHashResult(path, response);
            return response;
        }

        private static string CleanHashResult(string path, string response)
        {
            response = response.RemovePrefix(path);
            response = response.RemovePrefix($@"""{path}""");
            return response;
        }

#if !NET40
        /// <summary>
        /// Gets the hash of the specified file using the given command.
        /// </summary>
        internal async Task<string> GetHashInternalAsync(string path, string command, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;
            string response;

            if (!(reply = await ExecuteAsync(command + " " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            response = reply.Message;
            response = CleanHashResult(path, response);
            return response;
        }

#endif

        #endregion

        #region HASH Command

        /// <summary>
        /// Gets the currently selected hash algorithm for the HASH command.
        /// </summary>
        internal FtpHashAlgorithm GetHashAlgorithmUnused()
        {
            FtpReply reply;
            var type = FtpHashAlgorithm.NONE;
            lock (m_lock)
            {
                LogFunc(nameof(GetHashAlgorithmUnused));

                if ((reply = Execute("OPTS HASH")).Success)
                {
                    try
                    {
                        type = HashAlgos.FromString(reply.Message);
                    }
                    catch (InvalidOperationException)
                    {
                        // Do nothing
                    }
                }
            }
            return type;
        }

#if !NET40
        /// <summary>
        /// Gets the currently selected hash algorithm for the HASH command asynchronously.
        /// </summary>
        internal async Task<FtpHashAlgorithm> GetHashAlgorithmUnusedAsync(CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;
            var type = FtpHashAlgorithm.NONE;

            LogFunc(nameof(GetHashAlgorithmUnusedAsync));

            if ((reply = await ExecuteAsync("OPTS HASH", token)).Success)
            {
                try
                {
                    type = HashAlgos.FromString(reply.Message);
                }
                catch (InvalidOperationException)
                {
                    // Do nothing
                }
            }

            return type;
        }
#endif

        /// <summary>
        /// Sets the hash algorithm on the server to use for the HASH command. 
        /// </summary>
        internal void SetHashAlgorithmInternal(FtpHashAlgorithm algorithm)
        {
            FtpReply reply;

            // skip setting the hash algo if the server is already configured to it
            if (_LastHashAlgo == algorithm)
            {
                return;
            }
            lock (m_lock)
            {
                if ((HashAlgorithms & algorithm) != algorithm)
                {
                    throw new NotImplementedException("The hash algorithm " + algorithm.ToString() + " was not advertised by the server.");
                }

                string algoName = HashAlgos.PrintToString(algorithm);

                if (!(reply = Execute("OPTS HASH " + algoName)).Success)
                {
                    throw new FtpCommandException(reply);
                }

                // save the current hash algo so no need to repeat this command
                _LastHashAlgo = algorithm;
            }
        }

#if !NET40
        /// <summary>
        /// Sets the hash algorithm on the server to be used with the HASH command asynchronously.
        /// </summary>
        internal async Task SetHashAlgorithmInternalAsync(FtpHashAlgorithm algorithm, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            // skip setting the hash algo if the server is already configured to it
            if (_LastHashAlgo == algorithm)
            {
                return;
            }

            if ((HashAlgorithms & algorithm) != algorithm)
            {
                throw new NotImplementedException("The hash algorithm " + algorithm.ToString() + " was not advertised by the server.");
            }

            string algoName = HashAlgos.PrintToString(algorithm);

            if (!(reply = await ExecuteAsync("OPTS HASH " + algoName, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // save the current hash algo so no need to repeat this command
            _LastHashAlgo = algorithm;

        }
#endif

        /// <summary>
        /// Gets the hash of an object on the server using the currently selected hash algorithm.
        /// </summary>
        internal FtpHash HashCommandInternal(string path)
        {
            FtpReply reply;
            lock (m_lock)
            {
                if (!(reply = Execute("HASH " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
            // parse hash from the server reply
            return HashParser.Parse(reply.Message);
        }

#if !NET40
        /// <summary>
        /// Gets the hash of an object on the server using the currently selected hash algorithm.
        /// </summary>
        public async Task<FtpHash> HashCommandInternalAsync(string path, CancellationToken token = default(CancellationToken))
        {
            FtpReply reply;

            if (!(reply = await ExecuteAsync("HASH " + path, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // parse hash from the server reply
            return HashParser.Parse(reply.Message);
        }
#endif

        #endregion

        #region FXP Hash Algorithm

        /// <summary>
        /// Get the first checksum algorithm mutually supported by both servers.
        /// </summary>
        private FtpHashAlgorithm GetFirstMutualChecksum(FtpClient destination)
        {

            // special handling for HASH command which is a meta-command supporting all hash types
            if (HasFeature(FtpCapability.HASH) && destination.HasFeature(FtpCapability.HASH))
            {
                if (HashAlgorithms.HasFlag(FtpHashAlgorithm.MD5) && destination.HashAlgorithms.HasFlag(FtpHashAlgorithm.MD5))
                {
                    return FtpHashAlgorithm.MD5;
                }
                if (HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA1) && destination.HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA1))
                {
                    return FtpHashAlgorithm.SHA1;
                }
                if (HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA256) && destination.HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA256))
                {
                    return FtpHashAlgorithm.SHA256;
                }
                if (HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA512) && destination.HashAlgorithms.HasFlag(FtpHashAlgorithm.SHA512))
                {
                    return FtpHashAlgorithm.SHA512;
                }
                if (HashAlgorithms.HasFlag(FtpHashAlgorithm.CRC) && destination.HashAlgorithms.HasFlag(FtpHashAlgorithm.CRC))
                {
                    return FtpHashAlgorithm.CRC;
                }
            }

            // handling for non-standard specific hashing commands
            if (HasFeature(FtpCapability.MD5) && destination.HasFeature(FtpCapability.MD5))
            {
                return FtpHashAlgorithm.MD5;
            }
            if (HasFeature(FtpCapability.XMD5) && destination.HasFeature(FtpCapability.XMD5))
            {
                return FtpHashAlgorithm.MD5;
            }
            if (HasFeature(FtpCapability.MMD5) && destination.HasFeature(FtpCapability.MMD5))
            {
                return FtpHashAlgorithm.MD5;
            }
            if (HasFeature(FtpCapability.XSHA1) && destination.HasFeature(FtpCapability.XSHA1))
            {
                return FtpHashAlgorithm.SHA1;
            }
            if (HasFeature(FtpCapability.XSHA256) && destination.HasFeature(FtpCapability.XSHA256))
            {
                return FtpHashAlgorithm.SHA256;
            }
            if (HasFeature(FtpCapability.XSHA512) && destination.HasFeature(FtpCapability.XSHA512))
            {
                return FtpHashAlgorithm.SHA512;
            }
            if (HasFeature(FtpCapability.XCRC) && destination.HasFeature(FtpCapability.XCRC))
            {
                return FtpHashAlgorithm.CRC;
            }
            return FtpHashAlgorithm.NONE;
        }

        #endregion

        #region Obsolete Commands
        /// <summary>
        /// 获取加密类型
        /// </summary>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public FtpHashAlgorithm GetHashAlgorithm()
        {
            return FtpHashAlgorithm.NONE;
        }
        /// <summary>
        /// 设置加密类型
        /// </summary>
        /// <param name="algorithm"></param>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public void SetHashAlgorithm(FtpHashAlgorithm algorithm) { }
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public FtpHash GetHash(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取MD5值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to MD5. Or use CompareFile.", true)]
        public string GetMD5(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取CRC值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to CRC. Or use CompareFile.", true)]
        public string GetXCRC(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to MD5. Or use CompareFile.", true)]
        public string GetXMD5(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取SHA-1
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA1. Or use CompareFile.", true)]
        public string GetXSHA1(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取SHA256
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA256. Or use CompareFile.", true)]
        public string GetXSHA256(string path)
        {
            return null;
        }
        /// <summary>
        /// 获取SHA512
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA512. Or use CompareFile.", true)]
        public string GetXSHA512(string path)
        {
            return null;
        }


#if !NET40
        /// <summary>
        /// 异步获取加密类型
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public Task<FtpHashAlgorithm> GetHashAlgorithmAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(FtpHashAlgorithm.NONE);
        }
        /// <summary>
        /// 异步设置加密类型
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public Task SetHashAlgorithmAsync(FtpHashAlgorithm algorithm, CancellationToken token = default(CancellationToken))
        {
#if NET45
			return Task.FromResult(true);
#else
            return Task.CompletedTask;
#endif
        }
        /// <summary>
        /// 异步获取哈希值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and pass the algorithm type that you need. Or use CompareFile.", true)]
        public Task<FtpHash> GetHashAsync(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<FtpHash>(null);
        }
        /// <summary>
        /// 异步获取MD5值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to MD5. Or use CompareFile.", true)]
        public Task<string> GetMD5Async(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
        /// <summary>
        /// 异步获取XCRC
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to CRC. Or use CompareFile.", true)]
        public Task<string> GetXCRCAsync(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
        /// <summary>
        /// 异步获取MD5值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to MD5. Or use CompareFile.", true)]
        public Task<string> GetXMD5Async(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
        /// <summary>
        /// 异步获取SHA1
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA1. Or use CompareFile.", true)]
        public Task<string> GetXSHA1Async(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
        /// <summary>
        /// 异步获取SHA256
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA256. Or use CompareFile.", true)]
        public Task<string> GetXSHA256Async(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
        /// <summary>
        /// 异步获取SHA512
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Use GetChecksum instead and set the algorithm to SHA512. Or use CompareFile.", true)]
        public Task<string> GetXSHA512Async(string path, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
#endif

        #endregion
        #endregion // FtpClient_Hash.cs
        #region // FtpClient_IBMzOS.cs
        #region Get z/OS Realm

        /// <summary>
        /// If an FTP Server has "different realms", in which realm is the
        /// current working directory. 
        /// </summary>
        /// <returns>The realm</returns>
        public FtpZOSListRealm GetZOSListRealm()
        {

            LogFunc(nameof(GetZOSListRealm));

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                ReadCurrentWorkingDirectory();
            }

            if (ServerType != FtpServer.IBMzOSFTP)
            {
                return FtpZOSListRealm.Invalid;
            }

            // It is a unix like path (starts with /)
            if (_LastWorkingDir[0] != '\'')
            {
                return FtpZOSListRealm.Unix;
            }

            // Ok, the CWD starts with a single quote. Classic z/OS dataset realm
            FtpReply reply;
            lock (m_lock)
            {
                // Fetch the current working directory. The reply will tell us what it is we are...
                if (!(reply = Execute("CWD " + _LastWorkingDir)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }
            // 250-The working directory may be a load library                          
            // 250 The working directory "GEEK.PRODUCT.LOADLIB" is a partitioned data set

            if (reply.InfoMessages != null &&
                reply.InfoMessages.Contains("may be a load library"))
            {
                return FtpZOSListRealm.MemberU;
            }

            if (reply.Message.Contains("is a partitioned data set"))
            {
                return FtpZOSListRealm.Member;
            }

            return FtpZOSListRealm.Dataset;
        }

#if !NET40
        /// <summary>
        /// If an FTP Server has "different realms", in which realm is the
        /// current working directory. 
        /// </summary>
        /// <returns>The realm</returns>
        public async Task<FtpZOSListRealm> GetZOSListRealmAsync(CancellationToken token = default(CancellationToken))
        {
            LogFunc(nameof(GetZOSListRealmAsync));

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                await ReadCurrentWorkingDirectoryAsync(token);
            }

            if (ServerType != FtpServer.IBMzOSFTP)
            {
                return FtpZOSListRealm.Invalid;
            }

            // It is a unix like path (starts with /)
            if (_LastWorkingDir[0] != '\'')
            {
                return FtpZOSListRealm.Unix;
            }

            // Ok, the CWD starts with a single quote. Classic z/OS dataset realm
            FtpReply reply;

            // Fetch the current working directory. The reply will tell us what it is we are...
            if (!(reply = await ExecuteAsync("CWD " + _LastWorkingDir, token)).Success)
            {
                throw new FtpCommandException(reply);
            }

            // 250-The working directory may be a load library                          
            // 250 The working directory "GEEK.PRODUCTS.LOADLIB" is a partitioned data set

            if (reply.InfoMessages != null &&
                reply.InfoMessages.Contains("may be a load library"))
            {
                return FtpZOSListRealm.MemberU;
            }

            if (reply.Message.Contains("is a partitioned data set"))
            {
                return FtpZOSListRealm.Member;
            }

            return FtpZOSListRealm.Dataset;
        }
#endif
        #endregion
        #endregion // FtpClient_IBMzOS.cs
        #region // FtpClient_Listing.cs
        #region Get File Info

        /// <summary>
        /// Returns information about a file system object. Returns null if the server response can't
        /// be parsed or the server returns a failure completion code. The error for a failure
        /// is logged with FtpTrace. No exception is thrown on error because that would negate
        /// the usefulness of this method for checking for the existence of an object.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="dateModified">Get the accurate modified date using another MDTM command</param>
        /// <returns>A FtpListItem object</returns>
        public FtpListItem GetObjectInfo(string path, bool dateModified = false)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetObjectInfo), new object[] { path, dateModified });

            FtpReply reply;
            string[] res;

            var supportsMachineList = HasFeature(FtpCapability.MLSD);

            FtpListItem result = null;

            if (supportsMachineList)
            {
                // USE MACHINE LISTING TO GET INFO FOR A SINGLE FILE

                if ((reply = Execute("MLST " + path)).Success)
                {
                    res = reply.InfoMessages.Split('\n');
                    if (res.Length > 1)
                    {
                        var info = new StringBuilder();

                        for (var i = 1; i < res.Length; i++)
                        {
                            info.Append(res[i]);
                        }

                        result = m_listParser.ParseSingleLine(null, info.ToString(), m_capabilities, true);
                    }
                }
                else
                {
                    LogStatus(FtpTraceLevel.Warn, "Failed to get object info for path " + path + " with error " + reply.ErrorMessage);
                }
            }
            else
            {
                // USE GETLISTING TO GET ALL FILES IN DIR .. SLOWER BUT AT LEAST IT WORKS

                var dirPath = path.GetFtpDirectoryName();
                var dirItems = GetListing(dirPath);

                foreach (var dirItem in dirItems)
                {
                    if (dirItem.FullName == path)
                    {
                        result = dirItem;
                        break;
                    }
                }

                LogStatus(FtpTraceLevel.Warn, "Failed to get object info for path " + path + " since MLST not supported and GetListing() fails to list file/folder.");
            }

            // Get the accurate date modified using another MDTM command
            if (result != null && dateModified && HasFeature(FtpCapability.MDTM))
            {
                var alternativeModifiedDate = GetModifiedTime(path);
                if (alternativeModifiedDate != default)
                {
                    result.Modified = alternativeModifiedDate;
                }
            }

            return result;
        }

#if !NET40
        /// <summary>
        /// Return information about a remote file system object asynchronously. 
        /// </summary>
        /// <remarks>
        /// You should check the <see cref="Capabilities"/> property for the <see cref="FtpCapability.MLSD"/> 
        /// flag before calling this method. Failing to do so will result in an InvalidOperationException
        /// being thrown when the server does not support machine listings. Returns null if the server response can't
        /// be parsed or the server returns a failure completion code. The error for a failure
        /// is logged with FtpTrace. No exception is thrown on error because that would negate
        /// the usefulness of this method for checking for the existence of an object.</remarks>
        /// <param name="path">Path of the item to retrieve information about</param>
        /// <param name="dateModified">Get the accurate modified date using another MDTM command</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <exception cref="InvalidOperationException">Thrown if the server does not support this Capability</exception>
        /// <returns>A <see cref="FtpListItem"/> if the command succeeded, or null if there was a problem.</returns>
        public async Task<FtpListItem> GetObjectInfoAsync(string path, bool dateModified = false, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(GetObjectInfo), new object[] { path, dateModified });

            FtpReply reply;
            string[] res;

            var supportsMachineList = HasFeature(FtpCapability.MLSD);

            FtpListItem result = null;

            if (supportsMachineList)
            {
                // USE MACHINE LISTING TO GET INFO FOR A SINGLE FILE

                if ((reply = await ExecuteAsync("MLST " + path, token)).Success)
                {
                    res = reply.InfoMessages.Split('\n');
                    if (res.Length > 1)
                    {
                        var info = new StringBuilder();

                        for (var i = 1; i < res.Length; i++)
                        {
                            info.Append(res[i]);
                        }

                        result = m_listParser.ParseSingleLine(null, info.ToString(), m_capabilities, true);
                    }
                }
                else
                {
                    LogStatus(FtpTraceLevel.Warn, "Failed to get object info for path " + path + " with error " + reply.ErrorMessage);
                }
            }
            else
            {
                // USE GETLISTING TO GET ALL FILES IN DIR .. SLOWER BUT AT LEAST IT WORKS

                var dirPath = path.GetFtpDirectoryName();
                var dirItems = await GetListingAsync(dirPath, token);

                foreach (var dirItem in dirItems)
                {
                    if (dirItem.FullName == path)
                    {
                        result = dirItem;
                        break;
                    }
                }

                LogStatus(FtpTraceLevel.Warn, "Failed to get object info for path " + path + " since MLST not supported and GetListing() fails to list file/folder.");
            }

            // Get the accurate date modified using another MDTM command
            if (result != null && dateModified && HasFeature(FtpCapability.MDTM))
            {
                var alternativeModifiedDate = await GetModifiedTimeAsync(path, token);
                if (alternativeModifiedDate != default)
                {
                    result.Modified = alternativeModifiedDate;
                }
            }

            return result;
        }
#endif

        #endregion

        #region Get Listing

        /// <summary>
        /// Gets a file listing from the server from the current working directory. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <returns>An array of FtpListItem objects</returns>
        public FtpListItem[] GetListing()
        {
            return GetListing(null);
        }

        /// <summary>
        /// Gets a file listing from the server. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path of the directory to list</param>
        /// <returns>An array of FtpListItem objects</returns>
        public FtpListItem[] GetListing(string path)
        {
            return GetListing(path, 0);
        }

        /// <summary>
        /// Gets a file listing from the server. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="options">Options that dictate how a list is performed and what information is gathered.</param>
        /// <returns>An array of FtpListItem objects</returns>
        public FtpListItem[] GetListing(string path, FtpListOption options)
        {

            // start recursive process if needed and unsupported by the server
            if (options.HasFlag(FtpListOption.Recursive) && !IsServerSideRecursionSupported(options))
            {
                return GetListingRecursive(GetAbsolutePath(path), options);
            }

            // FIX : #768 NullOrEmpty is valid, means "use working directory".
            if (!string.IsNullOrEmpty(path))
            {
                path = path.GetFtpPath();
            }

            LogFunc(nameof(GetListing), new object[] { path, options });

            FtpListItem item = null;
            var lst = new List<FtpListItem>();
            List<string> rawlisting = null;
            string listcmd = null;
            string buf = null;

            // read flags
            var isIncludeSelf = options.HasFlag(FtpListOption.IncludeSelfAndParent);
            var isNameList = options.HasFlag(FtpListOption.NameList);
            var isRecursive = options.HasFlag(FtpListOption.Recursive) && RecursiveList;
            var isDerefLinks = options.HasFlag(FtpListOption.DerefLinks);
            var isGetModified = options.HasFlag(FtpListOption.Modify);
            var isGetSize = options.HasFlag(FtpListOption.Size);

            // Only disable the GetAbsolutePath(path) if z/OS
            // Note: "TEST.TST" is a "path" that does not start with a slash
            // This could be a unix file on z/OS OR a classic CWD relative dataset
            // Both of these work with the z/OS FTP server LIST command
            if (ServerType != FtpServer.IBMzOSFTP || path == null || path.StartsWith("/"))
            {
                // calc the absolute filepath
                path = GetAbsolutePath(path);
            }

            // MLSD provides a machine readable format with 100% accurate information
            // so always prefer MLSD over LIST unless the caller of this method overrides it with the ForceList option
            bool machineList;
            CalculateGetListingCommand(path, options, out listcmd, out machineList);

            lock (m_lock)
            {
                rawlisting = GetListingInternal(listcmd, options, true);
            }
            for (var i = 0; i < rawlisting.Count; i++)
            {
                buf = rawlisting[i];

                if (isNameList)
                {
                    // if NLST was used we only have a file name so
                    // there is nothing to parse.
                    item = new FtpListItem()
                    {
                        FullName = buf
                    };

                    if (DirectoryExists(item.FullName))
                    {
                        item.Type = FtpFileSystemObjectType.Directory;
                    }
                    else
                    {
                        item.Type = FtpFileSystemObjectType.File;
                    }

                    lst.Add(item);
                }
                else
                {

                    // load basic information available within the file listing
                    if (!LoadBasicListingInfo(ref path, ref item, lst, rawlisting, ref i, listcmd, buf, isRecursive, isIncludeSelf, machineList))
                    {

                        // skip unwanted listings
                        continue;
                    }
                }

                // load extended information that wasn't available if the list options flags say to do so.
                if (item != null)
                {
                    // try to dereference symbolic links if the appropriate list
                    // option was passed
                    if (item.Type == FtpFileSystemObjectType.Link && isDerefLinks)
                    {
                        item.LinkObject = DereferenceLink(item);
                    }

                    // if need to get file modified date
                    if (isGetModified && HasFeature(FtpCapability.MDTM))
                    {
                        // if the modified date was not loaded or the modified date is more than a day in the future 
                        // and the server supports the MDTM command, load the modified date.
                        // most servers do not support retrieving the modified date
                        // of a directory but we try any way.
                        if (item.Modified == DateTime.MinValue || listcmd.StartsWith("LIST"))
                        {
                            DateTime modify;

                            if (item.Type == FtpFileSystemObjectType.Directory)
                            {
                                LogStatus(FtpTraceLevel.Verbose, "Trying to retrieve modification time of a directory, some servers don't like this...");
                            }

                            if ((modify = GetModifiedTime(item.FullName)) != DateTime.MinValue)
                            {
                                item.Modified = modify;
                            }
                        }
                    }

                    // if need to get file size
                    if (isGetSize && HasFeature(FtpCapability.SIZE))
                    {
                        // if no size was parsed, the object is a file and the server
                        // supports the SIZE command, then load the file size
                        if (item.Size == -1)
                        {
                            if (item.Type != FtpFileSystemObjectType.Directory)
                            {
                                item.Size = GetFileSize(item.FullName);
                            }
                            else
                            {
                                item.Size = 0;
                            }
                        }
                    }
                }
            }

            return lst.ToArray();
        }

        private bool LoadBasicListingInfo(ref string path, ref FtpListItem item, List<FtpListItem> lst, List<string> rawlisting, ref int i, string listcmd, string buf, bool isRecursive, bool isIncludeSelf, bool machineList)
        {

            // if this is a result of LIST -R then the path will be spit out
            // before each block of objects
            if (listcmd.StartsWith("LIST") && isRecursive)
            {
                if (buf.StartsWith("/") && buf.EndsWith(":"))
                {
                    path = buf.TrimEnd(':');
                    return false;
                }
            }

            // if the next line in the listing starts with spaces
            // it is assumed to be a continuation of the current line
            if (i + 1 < rawlisting.Count && (rawlisting[i + 1].StartsWith("\t") || rawlisting[i + 1].StartsWith(" ")))
            {
                buf += rawlisting[++i];
            }

            try
            {
                item = m_listParser.ParseSingleLine(path, buf, m_capabilities, machineList);
            }
            catch (FtpListParseException)
            {
                LogStatus(FtpTraceLevel.Verbose, "Restarting parsing from first entry in list");
                i = -1;
                lst.Clear();
                return false;
            }

            // FtpListItem.Parse() returns null if the line
            // could not be parsed
            if (item != null)
            {
                if (isIncludeSelf || !IsItemSelf(path, item))
                {
                    lst.Add(item);
                }
                else
                {
                    //this.LogStatus(FtpTraceLevel.Verbose, "Skipped self or parent item: " + item.Name);
                }
            }
            // for z/OS, return of null actually means, just skip with no warning
            else if (ServerType != FtpServer.IBMzOSFTP)
            {
                LogStatus(FtpTraceLevel.Warn, "Failed to parse file listing: " + buf);
            }
            return true;
        }

        private bool IsItemSelf(string path, FtpListItem item)
        {
            return item.Name == "." ||
                item.Name == ".." ||
                item.SubType == FtpFileSystemObjectSubType.ParentDirectory ||
                item.SubType == FtpFileSystemObjectSubType.SelfDirectory ||
                item.FullName.EnsurePostfix("/") == path;
        }

        private void CalculateGetListingCommand(string path, FtpListOption options, out string listcmd, out bool machineList)
        {

            // read flags
            var isForceList = options.HasFlag(FtpListOption.ForceList);
            var isUseStat = options.HasFlag(FtpListOption.UseStat);
            var isNoPath = options.HasFlag(FtpListOption.NoPath);
            var isNameList = options.HasFlag(FtpListOption.NameList);
            var isUseLS = options.HasFlag(FtpListOption.UseLS);
            var isAllFiles = options.HasFlag(FtpListOption.AllFiles);
            var isRecursive = options.HasFlag(FtpListOption.Recursive) && RecursiveList;

            machineList = false;

            // use stat listing if forced
            if (isUseStat)
            {
                listcmd = "STAT -l";
            }
            else
            {
                // use machine listing if supported by the server
                if (!isForceList && ListingParser == FtpParser.Machine && HasFeature(FtpCapability.MLSD))
                {
                    listcmd = "MLSD";
                    machineList = true;
                }
                else
                {
                    // otherwise use one of the legacy name listing commands
                    if (isUseLS)
                    {
                        listcmd = "LS";
                    }
                    else if (isNameList)
                    {
                        listcmd = "NLST";
                    }
                    else
                    {
                        var listopts = "";

                        listcmd = "LIST";

                        // add option flags
                        if (isAllFiles)
                        {
                            listopts += "a";
                        }

                        if (isRecursive)
                        {
                            listopts += "R";
                        }

                        if (listopts.Length > 0)
                        {
                            listcmd += " -" + listopts;
                        }
                    }
                }
            }

            if (!isNoPath)
            {
                listcmd = listcmd + " " + path.GetFtpPath();
            }
        }

        private bool IsServerSideRecursionSupported(FtpListOption options)
        {

            // Fix #539: Correctly calculate if server-side recursion is supported else fallback to manual recursion

            // check if the connected FTP server supports recursion in the first place
            if (RecursiveList)
            {

                // read flags
                var isForceList = options.HasFlag(FtpListOption.ForceList);
                var isUseStat = options.HasFlag(FtpListOption.UseStat);
                var isNameList = options.HasFlag(FtpListOption.NameList);
                var isUseLS = options.HasFlag(FtpListOption.UseLS);

                // if not using STAT listing
                if (!isUseStat)
                {

                    // if not using machine listing (MSLD)
                    if ((!isForceList || ListingParser == FtpParser.Machine) && HasFeature(FtpCapability.MLSD))
                    {
                    }
                    else
                    {

                        // if not using legacy list (LS) and name listing (NSLT)
                        if (!isUseLS && !isNameList)
                        {

                            // only supported if using LIST
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the records of a file listing and retry if temporary failure.
        /// </summary>
        private List<string> GetListingInternal(string listcmd, FtpListOption options, bool retry)
        {
            var rawlisting = new List<string>();
            var isUseStat = options.HasFlag(FtpListOption.UseStat);

            // always get the file listing in binary to avoid character translation issues with ASCII.
            SetDataTypeNoLock(ListingDataType);

            try
            {
                // read in raw file listing from control stream
                if (isUseStat)
                {
                    var reply = Execute(listcmd);
                    if (reply.Success)
                    {
                        LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");

                        foreach (var line in reply.InfoMessages.Split('\n'))
                        {
                            if (!Strings.IsNullOrWhiteSpace(line))
                            {
                                rawlisting.Add(line);
                                LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                            }
                        }

                        LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
                    }
                }
                else
                {
                    // read in raw file listing from data stream
                    using (var stream = OpenDataStream(listcmd, 0))
                    {
                        try
                        {
                            LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");

                            if (BulkListing)
                            {
                                // increases performance of GetListing by reading multiple lines of the file listing at once
                                foreach (var line in stream.ReadAllLines(Encoding, BulkListingLength))
                                {
                                    if (!Strings.IsNullOrWhiteSpace(line))
                                    {
                                        rawlisting.Add(line);
                                        LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                                    }
                                }
                            }
                            else
                            {
                                // GetListing will read file listings line-by-line (actually byte-by-byte)
                                string buf;
                                while ((buf = stream.ReadLine(Encoding)) != null)
                                {
                                    if (buf.Length > 0)
                                    {
                                        rawlisting.Add(buf);
                                        LogLine(FtpTraceLevel.Verbose, "Listing:  " + buf);
                                    }
                                }
                            }

                            LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
                        }
                        finally
                        {
                            stream.Close();
                        }
                    }
                }
            }
            catch (FtpMissingSocketException)
            {
                // Some FTP server does not send any response when listing an empty directory
                // and the connection fails because no communication socket is provided by the server
            }
            catch (FtpCommandException ftpEx)
            {
                // Fix for #589 - CompletionCode is null
                if (ftpEx.CompletionCode == null)
                {
                    throw new FtpException(ftpEx.Message + " - Try using FtpListOption.UseStat which might fix this.", ftpEx);
                }
                // Some FTP servers throw 550 for empty folders. Absorb these.
                if (!ftpEx.CompletionCode.StartsWith("550"))
                {
                    throw ftpEx;
                }
            }
            catch (IOException ioEx)
            {
                // Some FTP servers forcibly close the connection, we absorb these errors

                // Fix #410: Retry if its a temporary failure ("Received an unexpected EOF or 0 bytes from the transport stream")
                if (retry && ioEx.Message.IsKnownError(FtpServerStrings.unexpectedEOF))
                {
                    // retry once more, but do not go into a infinite recursion loop here
                    LogLine(FtpTraceLevel.Verbose, "Warning:  Retry GetListing once more due to unexpected EOF");
                    return GetListingInternal(listcmd, options, false);
                }
                else
                {
                    // suppress all other types of exceptions
                }
            }

            return rawlisting;
        }

#if NETFx
        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path to list</param>
        /// <param name="options">Options that dictate how the list operation is performed</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="enumToken">The token that can be used to cancel the enumerator</param>
        /// <returns>An array of items retrieved in the listing</returns>
        public async IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(string path, FtpListOption options, CancellationToken token = default(CancellationToken), [EnumeratorCancellation] CancellationToken enumToken = default(CancellationToken))
        {

            // start recursive process if needed and unsupported by the server
            if (options.HasFlag(FtpListOption.Recursive) && !IsServerSideRecursionSupported(options))
            {
                await foreach (FtpListItem i in GetListingRecursiveAsyncEnumerable(GetAbsolutePath(path), options, token, enumToken))
                {
                    yield return i;
                }

                yield break;
            }

            LogFunc(nameof(GetListingAsync), new object[] { path, options });

            var lst = new List<FtpListItem>();
            var rawlisting = new List<string>();
            string listcmd = null;

            // read flags
            var isIncludeSelf = options.HasFlag(FtpListOption.IncludeSelfAndParent);
            var isNameList = options.HasFlag(FtpListOption.NameList);
            var isRecursive = options.HasFlag(FtpListOption.Recursive) && RecursiveList;
            var isDerefLinks = options.HasFlag(FtpListOption.DerefLinks);
            var isGetModified = options.HasFlag(FtpListOption.Modify);
            var isGetSize = options.HasFlag(FtpListOption.Size);

            // Only disable the GetAbsolutePath(path) if z/OS
            // Note: "TEST.TST" is a "path" that does not start with a slash
            // This could be a unix file on z/OS OR a classic CWD relative dataset
            // Both of these work with the z/OS FTP server LIST command
            if (ServerType != FtpServer.IBMzOSFTP || path == null || path.StartsWith("/"))
            {
                // calc the absolute filepath
                path = await GetAbsolutePathAsync(path, token);
            }

            // MLSD provides a machine readable format with 100% accurate information
            // so always prefer MLSD over LIST unless the caller of this method overrides it with the ForceList option
            bool machineList;
            CalculateGetListingCommand(path, options, out listcmd, out machineList);

            // read in raw file listing
            rawlisting = await GetListingInternalAsync(listcmd, options, true, token);

            FtpListItem item = null;

            for (var i = 0; i < rawlisting.Count; i++)
            {
                string rawEntry = rawlisting[i];

                // break if task is cancelled
                token.ThrowIfCancellationRequested();

                if (!isNameList)
                {

                    // load basic information available within the file listing
                    if (!LoadBasicListingInfo(ref path, ref item, lst, rawlisting, ref i, listcmd, rawEntry, isRecursive, isIncludeSelf, machineList))
                    {

                        // skip unwanted listings
                        continue;
                    }

                }

                item = await GetListingProcessItemAsync(item, lst, rawEntry, listcmd, token,
                    isIncludeSelf, isNameList, isRecursive, isDerefLinks, isGetModified, isGetSize
                );
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path to list</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <param name="enumToken">The token that can be used to cancel the enumerator</param>
        /// <returns>An array of items retrieved in the listing</returns>
        public IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(string path, CancellationToken token = default(CancellationToken), CancellationToken enumToken = default(CancellationToken))
        {
            return GetListingAsyncEnumerable(path, 0, token, enumToken);
        }

        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <returns>An array of items retrieved in the listing</returns>
        public IAsyncEnumerable<FtpListItem> GetListingAsyncEnumerable(CancellationToken token = default(CancellationToken), CancellationToken enumToken = default(CancellationToken))
        {
            return GetListingAsyncEnumerable(null, token, enumToken);
        }

#endif


#if !NET40
        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path to list</param>
        /// <param name="options">Options that dictate how the list operation is performed</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>An array of items retrieved in the listing</returns>
        public async Task<FtpListItem[]> GetListingAsync(string path, FtpListOption options, CancellationToken token = default(CancellationToken))
        {

            // start recursive process if needed and unsupported by the server
            if (options.HasFlag(FtpListOption.Recursive) && !IsServerSideRecursionSupported(options))
            {
                return await GetListingRecursiveAsync(GetAbsolutePath(path), options, token);
            }

            // FIX : #768 NullOrEmpty is valid, means "use working directory".
            if (!string.IsNullOrEmpty(path))
            {
                path = path.GetFtpPath();
            }

            LogFunc(nameof(GetListingAsync), new object[] { path, options });

            var lst = new List<FtpListItem>();
            var rawlisting = new List<string>();
            string listcmd = null;

            // read flags
            var isIncludeSelf = options.HasFlag(FtpListOption.IncludeSelfAndParent);
            var isNameList = options.HasFlag(FtpListOption.NameList);
            var isRecursive = options.HasFlag(FtpListOption.Recursive) && RecursiveList;
            var isDerefLinks = options.HasFlag(FtpListOption.DerefLinks);
            var isGetModified = options.HasFlag(FtpListOption.Modify);
            var isGetSize = options.HasFlag(FtpListOption.Size);

            // calc path to request
            path = await GetAbsolutePathAsync(path, token);

            // MLSD provides a machine readable format with 100% accurate information
            // so always prefer MLSD over LIST unless the caller of this method overrides it with the ForceList option
            bool machineList;
            CalculateGetListingCommand(path, options, out listcmd, out machineList);

            // read in raw file listing
            rawlisting = await GetListingInternalAsync(listcmd, options, true, token);

            FtpListItem item = null;

            for (var i = 0; i < rawlisting.Count; i++)
            {
                string rawEntry = rawlisting[i];

                // break if task is cancelled
                token.ThrowIfCancellationRequested();

                if (!isNameList)
                {

                    // load basic information available within the file listing
                    if (!LoadBasicListingInfo(ref path, ref item, lst, rawlisting, ref i, listcmd, rawEntry, isRecursive, isIncludeSelf, machineList))
                    {

                        // skip unwanted listings
                        continue;
                    }

                }

                item = await GetListingProcessItemAsync(item, lst, rawEntry, listcmd, token,
                    isIncludeSelf, isNameList, isRecursive, isDerefLinks, isGetModified, isGetSize
                );

            }
            return lst.ToArray();
        }

        private async Task<FtpListItem> GetListingProcessItemAsync(FtpListItem item, List<FtpListItem> lst, string rawEntry, string listcmd, CancellationToken token, bool isIncludeSelf, bool isNameList, bool isRecursive, bool isDerefLinks, bool isGetModified, bool isGetSize)
        {

            if (isNameList)
            {
                // if NLST was used we only have a file name so
                // there is nothing to parse.
                item = new FtpListItem()
                {
                    FullName = rawEntry
                };

                if (await DirectoryExistsAsync(item.FullName, token))
                {
                    item.Type = FtpFileSystemObjectType.Directory;
                }
                else
                {
                    item.Type = FtpFileSystemObjectType.File;
                }
                lst.Add(item);
            }

            // load extended information that wasn't available if the list options flags say to do so.
            if (item != null)
            {
                // try to dereference symbolic links if the appropriate list
                // option was passed
                if (item.Type == FtpFileSystemObjectType.Link && isDerefLinks)
                {
                    item.LinkObject = await DereferenceLinkAsync(item, token);
                }

                // if need to get file modified date
                if (isGetModified && HasFeature(FtpCapability.MDTM))
                {
                    // if the modified date was not loaded or the modified date is more than a day in the future 
                    // and the server supports the MDTM command, load the modified date.
                    // most servers do not support retrieving the modified date
                    // of a directory but we try any way.
                    if (item.Modified == DateTime.MinValue || listcmd.StartsWith("LIST"))
                    {
                        DateTime modify;

                        if (item.Type == FtpFileSystemObjectType.Directory)
                        {
                            LogStatus(FtpTraceLevel.Verbose, "Trying to retrieve modification time of a directory, some servers don't like this...");
                        }

                        if ((modify = await GetModifiedTimeAsync(item.FullName, token: token)) != DateTime.MinValue)
                        {
                            item.Modified = modify;
                        }
                    }
                }

                // if need to get file size
                if (isGetSize && HasFeature(FtpCapability.SIZE))
                {
                    // if no size was parsed, the object is a file and the server
                    // supports the SIZE command, then load the file size
                    if (item.Size == -1)
                    {
                        if (item.Type != FtpFileSystemObjectType.Directory)
                        {
                            item.Size = await GetFileSizeAsync(item.FullName, -1, token);
                        }
                        else
                        {
                            item.Size = 0;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Get the records of a file listing and retry if temporary failure.
        /// </summary>
        private async Task<List<string>> GetListingInternalAsync(string listcmd, FtpListOption options, bool retry, CancellationToken token)
        {
            var rawlisting = new List<string>();
            var isUseStat = options.HasFlag(FtpListOption.UseStat);

            // always get the file listing in binary to avoid character translation issues with ASCII.
            await SetDataTypeNoLockAsync(ListingDataType, token);

            try
            {

                // read in raw file listing from control stream
                if (isUseStat)
                {
                    var reply = await ExecuteAsync(listcmd, token);
                    if (reply.Success)
                    {

                        LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");

                        foreach (var line in reply.InfoMessages.Split('\n'))
                        {
                            if (!Strings.IsNullOrWhiteSpace(line))
                            {
                                rawlisting.Add(line);
                                LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                            }
                        }

                        LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
                    }
                }
                else
                {

                    // read in raw file listing from data stream
                    using (FtpDataStream stream = await OpenDataStreamAsync(listcmd, 0, token))
                    {
                        try
                        {
                            LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");

                            if (BulkListing)
                            {
                                // increases performance of GetListing by reading multiple lines of the file listing at once
                                foreach (var line in await stream.ReadAllLinesAsync(Encoding, BulkListingLength, token))
                                {
                                    if (!Strings.IsNullOrWhiteSpace(line))
                                    {
                                        rawlisting.Add(line);
                                        LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                                    }
                                }
                            }
                            else
                            {
                                // GetListing will read file listings line-by-line (actually byte-by-byte)
                                string buf;
                                while ((buf = await stream.ReadLineAsync(Encoding, token)) != null)
                                {
                                    if (buf.Length > 0)
                                    {
                                        rawlisting.Add(buf);
                                        LogLine(FtpTraceLevel.Verbose, "Listing:  " + buf);
                                    }
                                }
                            }

                            LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
                        }
                        finally
                        {
                            stream.Close();
                        }
                    }
                }
            }
            catch (FtpMissingSocketException)
            {
                // Some FTP server does not send any response when listing an empty directory
                // and the connection fails because no communication socket is provided by the server
            }
            catch (FtpCommandException ftpEx)
            {
                // Fix for #589 - CompletionCode is null
                if (ftpEx.CompletionCode == null)
                {
                    throw new FtpException(ftpEx.Message + " - Try using FtpListOption.UseStat which might fix this.", ftpEx);
                }
                // Some FTP servers throw 550 for empty folders. Absorb these.
                if (!ftpEx.CompletionCode.StartsWith("550"))
                {
                    throw ftpEx;
                }
            }
            catch (IOException ioEx)
            {
                // Some FTP servers forcibly close the connection, we absorb these errors

                // Fix #410: Retry if its a temporary failure ("Received an unexpected EOF or 0 bytes from the transport stream")
                if (retry && ioEx.Message.IsKnownError(FtpServerStrings.unexpectedEOF))
                {
                    // retry once more, but do not go into a infinite recursion loop here
                    LogLine(FtpTraceLevel.Verbose, "Warning:  Retry GetListing once more due to unexpected EOF");
                    return await GetListingInternalAsync(listcmd, options, false, token);
                }
                else
                {
                    // suppress all other types of exceptions
                }
            }

            return rawlisting;
        }

        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <param name="path">The path to list</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>An array of items retrieved in the listing</returns>
        public Task<FtpListItem[]> GetListingAsync(string path, CancellationToken token = default(CancellationToken))
        {
            return GetListingAsync(path, 0, token);
        }


        /// <summary>
        /// Gets a file listing from the server asynchronously. Each <see cref="FtpListItem"/> object returned
        /// contains information about the file that was able to be retrieved. 
        /// </summary>
        /// <remarks>
        /// If a <see cref="DateTime"/> property is equal to <see cref="DateTime.MinValue"/> then it means the 
        /// date in question was not able to be retrieved. If the <see cref="FtpListItem.Size"/> property
        /// is equal to 0, then it means the size of the object could also not
        /// be retrieved.
        /// </remarks>
        /// <returns>An array of items retrieved in the listing</returns>
        public Task<FtpListItem[]> GetListingAsync(CancellationToken token = default(CancellationToken))
        {
            return GetListingAsync(null, token);
        }

#endif

        #endregion

        #region Get Listing Recursive

        /// <summary>
        /// Recursive method of GetListing, to recurse through directories on servers that do not natively support recursion.
        /// Automatically called by GetListing where required.
        /// Uses flat recursion instead of head recursion.
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="options">Options that dictate how a list is performed and what information is gathered.</param>
        /// <returns>An array of FtpListItem objects</returns>
        protected FtpListItem[] GetListingRecursive(string path, FtpListOption options)
        {
            // remove the recursive flag
            options &= ~FtpListOption.Recursive;

            // add initial path to list of folders to explore
            var stack = new Stack<string>();
            stack.Push(path);
            var allFiles = new List<FtpListItem>();

            // explore folders
            while (stack.Count > 0)
            {
                // get path of folder to list
                var currentPath = stack.Pop();
                if (!currentPath.EndsWith("/"))
                {
                    currentPath += "/";
                }

                // list it
                var items = GetListing(currentPath, options);

                // add it to the final listing
                allFiles.AddRange(items);

                // extract the directories
                foreach (var item in items)
                {
                    if (item.Type == FtpFileSystemObjectType.Directory && item.Name != "." && item.Name != "..")
                    {
                        stack.Push(item.FullName);
                    }
                }

                items = null;

                // recurse
            }

            // final list of all files and dirs
            return allFiles.ToArray();
        }

#if NETFx
        /// <summary>
        /// Recursive method of GetListingAsync, to recurse through directories on servers that do not natively support recursion.
        /// Automatically called by GetListingAsync where required.
        /// Uses flat recursion instead of head recursion.
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="options">Options that dictate how a list is performed and what information is gathered.</param>
        /// <param name="token"></param>
        /// <param name="enumToken"></param>
        /// <returns>An array of FtpListItem objects</returns>

        protected async IAsyncEnumerable<FtpListItem> GetListingRecursiveAsyncEnumerable(string path, FtpListOption options, CancellationToken token, [EnumeratorCancellation] CancellationToken enumToken = default)
        {
            // remove the recursive flag
            options &= ~FtpListOption.Recursive;

            // add initial path to list of folders to explore
            var stack = new Stack<string>();
            stack.Push(path);
            var allFiles = new List<FtpListItem>();

            // explore folders
            while (stack.Count > 0)
            {
                // get path of folder to list
                var currentPath = stack.Pop();
                if (!currentPath.EndsWith("/"))
                {
                    currentPath += "/";
                }

                // extract the directories
                await foreach (var item in GetListingAsyncEnumerable(currentPath, options, token))
                {
                    // break if task is cancelled
                    token.ThrowIfCancellationRequested();

                    if (item.Type == FtpFileSystemObjectType.Directory && item.Name != "." && item.Name != "..")
                    {
                        stack.Push(item.FullName);
                    }

                    yield return item;
                }

                // recurse
            }
        }
#endif


#if !NET40
        /// <summary>
        /// Recursive method of GetListingAsync, to recurse through directories on servers that do not natively support recursion.
        /// Automatically called by GetListingAsync where required.
        /// Uses flat recursion instead of head recursion.
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="options">Options that dictate how a list is performed and what information is gathered.</param>
        /// <param name="token"></param>
        /// <returns>An array of FtpListItem objects</returns>
        protected async Task<FtpListItem[]> GetListingRecursiveAsync(string path, FtpListOption options, CancellationToken token)
        {

            // remove the recursive flag
            options &= ~FtpListOption.Recursive;

            // add initial path to list of folders to explore
            var stack = new Stack<string>();
            stack.Push(path);
            var allFiles = new List<FtpListItem>();

            // explore folders
            while (stack.Count > 0)
            {
                // get path of folder to list
                var currentPath = stack.Pop();
                if (!currentPath.EndsWith("/"))
                {
                    currentPath += "/";
                }

                // list it
                FtpListItem[] items = await GetListingAsync(currentPath, options, token);

                // break if task is cancelled
                token.ThrowIfCancellationRequested();

                // add it to the final listing
                allFiles.AddRange(items);

                // extract the directories
                foreach (var item in items)
                {
                    if (item.Type == FtpFileSystemObjectType.Directory && item.Name != "." && item.Name != "..")
                    {
                        stack.Push(item.FullName);
                    }
                }

                items = null;

                // recurse
            }

            // final list of all files and dirs
            return allFiles.ToArray();
        }
#endif

        #endregion

        #region Get Name Listing

        /// <summary>
        /// Returns a file/directory listing using the NLST command.
        /// </summary>
        /// <returns>A string array of file and directory names if any were returned.</returns>
        public string[] GetNameListing()
        {
            return GetNameListing(null);
        }

        /// <summary>
        /// Returns a file/directory listing using the NLST command.
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <returns>A string array of file and directory names if any were returned.</returns>
        public string[] GetNameListing(string path)
        {

            // FIX : #768 NullOrEmpty is valid, means "use working directory".
            if (!string.IsNullOrEmpty(path))
            {
                path = path.GetFtpPath();
            }

            LogFunc(nameof(GetNameListing), new object[] { path });

            var listing = new List<string>();

            if (ServerType != FtpServer.IBMzOSFTP || path == null || path.StartsWith("/"))
            {
                // calc path to request
                path = GetAbsolutePath(path);
            }
            lock (m_lock)
            {
                // always get the file listing in binary to avoid character translation issues with ASCII.
                SetDataTypeNoLock(ListingDataType);

                // read in raw listing
                try
                {
                    using (var stream = OpenDataStream("NLST " + path, 0))
                    {
                        LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                        string line;

                        try
                        {
                            while ((line = stream.ReadLine(Encoding)) != null)
                            {
                                listing.Add(line);
                                LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                            }
                        }
                        finally
                        {
                            stream.Close();
                        }
                        LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                    }
                }
                catch (FtpMissingSocketException)
                {
                    // Some FTP server does not send any response when listing an empty directory
                    // and the connection fails because no communication socket is provided by the server
                }
                catch (FtpCommandException ftpEx)
                {
                    // Some FTP servers throw 550 for empty folders. Absorb these.
                    if (ftpEx.CompletionCode == null || !ftpEx.CompletionCode.StartsWith("550"))
                    {
                        throw ftpEx;
                    }
                }
                catch (IOException)
                {
                    // Some FTP servers forcibly close the connection, we absorb these errors
                }
            }
            return listing.ToArray();
        }

#if NET40
        private delegate string[] AsyncGetNameListing(string path);

        /// <summary>
        /// Begin an asynchronous operation to return a file/directory listing using the NLST command.
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="callback">Async Callback</param>
        /// <param name="state">State object</param>
        /// <returns>IAsyncResult</returns>
        public IAsyncResult BeginGetNameListing(string path, AsyncCallback callback, object state)
        {
            IAsyncResult ar;
            AsyncGetNameListing func;

            lock (m_asyncmethods)
            {
                ar = (func = new AsyncGetNameListing(GetNameListing)).BeginInvoke(path, callback, state);
                m_asyncmethods.Add(ar, func);
            }

            return ar;
        }

        /// <summary>
        /// Begin an asynchronous operation to return a file/directory listing using the NLST command.
        /// </summary>
        /// <param name="callback">Async Callback</param>
        /// <param name="state">State object</param>
        /// <returns>IAsyncResult</returns>
        public IAsyncResult BeginGetNameListing(AsyncCallback callback, object state)
        {
            return BeginGetNameListing(null, callback, state);
        }

        /// <summary>
        /// Ends a call to <see cref="o:BeginGetNameListing"/>
        /// </summary>
        /// <param name="ar">IAsyncResult object returned from <see cref="o:BeginGetNameListing"/></param>
        /// <returns>An array of file and directory names if any were returned.</returns>
        public string[] EndGetNameListing(IAsyncResult ar)
        {
            return GetAsyncDelegate<AsyncGetNameListing>(ar).EndInvoke(ar);
        }

#endif
#if !NET40
        /// <summary>
        /// Returns a file/directory listing using the NLST command asynchronously
        /// </summary>
        /// <param name="path">The path of the directory to list</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>An array of file and directory names if any were returned.</returns>
        public async Task<string[]> GetNameListingAsync(string path, CancellationToken token = default(CancellationToken))
        {

            // FIX : #768 NullOrEmpty is valid, means "use working directory".
            if (!string.IsNullOrEmpty(path))
            {
                path = path.GetFtpPath();
            }

            LogFunc(nameof(GetNameListingAsync), new object[] { path });

            var listing = new List<string>();

            if (ServerType != FtpServer.IBMzOSFTP || path == null || path.StartsWith("/"))
            {
                // calc path to request
                path = await GetAbsolutePathAsync(path, token);
            }

            // always get the file listing in binary to avoid character translation issues with ASCII.
            await SetDataTypeNoLockAsync(ListingDataType, token);

            // read in raw listing
            try
            {
                using (FtpDataStream stream = await OpenDataStreamAsync("NLST " + path, 0, token))
                {
                    LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                    string line;

                    try
                    {
                        while ((line = await stream.ReadLineAsync(Encoding, token)) != null)
                        {
                            listing.Add(line);
                            LogLine(FtpTraceLevel.Verbose, "Listing:  " + line);
                        }
                    }
                    finally
                    {
                        stream.Close();
                    }
                    LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                }
            }
            catch (FtpMissingSocketException)
            {
                // Some FTP server does not send any response when listing an empty directory
                // and the connection fails because no communication socket is provided by the server
            }
            catch (FtpCommandException ftpEx)
            {
                // Some FTP servers throw 550 for empty folders. Absorb these.
                if (ftpEx.CompletionCode == null || !ftpEx.CompletionCode.StartsWith("550"))
                {
                    throw ftpEx;
                }
            }
            catch (IOException)
            {
                // Some FTP servers forcibly close the connection, we absorb these errors
            }

            return listing.ToArray();
        }

        /// <summary>
        /// Returns a file/directory listing using the NLST command asynchronously
        /// </summary>
        /// <returns>An array of file and directory names if any were returned.</returns>
        public Task<string[]> GetNameListingAsync(CancellationToken token = default(CancellationToken))
        {
            return GetNameListingAsync(null, token);
        }
#endif

        #endregion
        #endregion // FtpClient_Listing.cs
        #region // FtpClient_Logging.cs 
        /// <summary>
        /// Add a custom listener here to get events every time a message is logged.
        /// </summary>
        public Action<FtpTraceLevel, string> OnLogEvent;

        /// <summary>
        /// Log a function call with relevant arguments
        /// </summary>
        /// <param name="function">The name of the API function</param>
        /// <param name="args">The args passed to the function</param>
        public void LogFunc(string function, object[] args = null)
        {
            // log to attached logger if given
            if (OnLogEvent != null)
            {
                OnLogEvent(FtpTraceLevel.Verbose, ">         " + function + "(" + args.ItemsToString().Join(", ") + ")");
            }

            // log to system
            FtpTrace.WriteFunc(function, args);
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">The message to write</param>
        public void LogLine(FtpTraceLevel eventType, string message)
        {
            // log to attached logger if given
            if (OnLogEvent != null)
            {
                OnLogEvent(eventType, message);
            }

            // log to system
            FtpTrace.WriteLine(eventType, message);
        }

        /// <summary>
        /// Log a message, adding an automatic prefix to the message based on the `eventType`
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">The message to write</param>
        public void LogStatus(FtpTraceLevel eventType, string message)
        {
            // add prefix
            message = TraceLevelPrefix(eventType) + message;

            // log to attached logger if given
            if (OnLogEvent != null)
            {
                OnLogEvent(eventType, message);
            }

            // log to system
            FtpTrace.WriteLine(eventType, message);
        }

        private static string TraceLevelPrefix(FtpTraceLevel level)
        {
            switch (level)
            {
                case FtpTraceLevel.Verbose:
                    return "Status:   ";

                case FtpTraceLevel.Info:
                    return "Status:   ";

                case FtpTraceLevel.Warn:
                    return "Warning:  ";

                case FtpTraceLevel.Error:
                    return "Error:    ";
            }

            return "Status:   ";
        }
        #endregion // FtpClient_Logging.cs
        #region // FtpClient_Properties.cs
        #region Internal State Flags

        /// <summary>
        /// Used to improve performance of OpenPassiveDataStream.
        /// Enhanced-passive mode is tried once, and if not supported, is not tried again.
        /// </summary>
        private bool _EPSVNotSupported = false;

        /// <summary>
        /// Used to improve performance of GetFileSize.
        /// SIZE command is tried, and if the server cannot send it in ASCII mode, we switch to binary each time you call GetFileSize.
        /// However most servers will support ASCII, so we can get the file size without switching to binary, improving performance.
        /// </summary>
        private bool _FileSizeASCIINotSupported = false;

        /// <summary>
        /// Used to improve performance of GetListing.
        /// You can set this to true by setting the RecursiveList property.
        /// </summary>
        private bool _RecursiveListSupported = false;

        /// <summary>
        /// Used to automatically dispose cloned connections after FXP transfer has ended.
        /// </summary>
        private bool _AutoDispose = false;

        /// <summary>
        /// Cached value of the last read working directory (absolute path).
        /// </summary>
        private string _LastWorkingDir = null;

        /// <summary>
        /// Cached value of the last set hash algorithm.
        /// </summary>
        private FtpHashAlgorithm _LastHashAlgo = FtpHashAlgorithm.NONE;

        /// <summary>
        /// Did the FTPS connection fail during the last Connect/ConnectAsync attempt?
        /// </summary>
        private bool _ConnectionFTPSFailure = false;

        /// <summary>
        /// Did the UTF8 encoding setting work during the last Connect/ConnectAsync attempt?
        /// </summary>
        private bool _ConnectionUTF8Success = false;

        /// <summary>
        /// These flags must be reset every time we connect, to allow for users to connect to
        /// different FTP servers with the same client object.
        /// </summary>
        private void ResetStateFlags()
        {
            _EPSVNotSupported = false;
            _FileSizeASCIINotSupported = false;
            _RecursiveListSupported = false;
            _LastWorkingDir = null;
            _LastHashAlgo = FtpHashAlgorithm.NONE;
            _ConnectionFTPSFailure = false;
            _ConnectionUTF8Success = false;
        }

        /// <summary>
        /// These flags must be copied when we quickly clone the connection.
        /// </summary>
        private void CopyStateFlags(FtpClient original)
        {
            _EPSVNotSupported = original._EPSVNotSupported;
            _FileSizeASCIINotSupported = original._FileSizeASCIINotSupported;
            _RecursiveListSupported = original._RecursiveListSupported;
        }

        #endregion
        /// <summary>
        /// Used for internally synchronizing access to this
        /// object from multiple threads
        /// </summary>
        private readonly object m_lock = new object();

        /// <summary>
        /// For usage by FTP proxies only
        /// </summary>
        protected object Lock => m_lock;
        /// <summary>
        /// A list of asynchronous methods that are in progress
        /// </summary>
        private readonly Dictionary<IAsyncResult, object> m_asyncmethods = new Dictionary<IAsyncResult, object>();

        /// <summary>
        /// Control connection socket stream
        /// </summary>
        private FtpSocketStream m_stream = null;

        private bool m_isDisposed = false;

        /// <summary>
        /// Gets a value indicating if this object has already been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get => m_isDisposed;
            private set => m_isDisposed = value;
        }

        /// <summary>
        /// Gets the base stream for talking to the server via
        /// the control connection.
        /// </summary>
        protected Stream BaseStream => m_stream;

        private FtpIpVersion m_ipVersions = FtpIpVersion.ANY;

        /// <summary>
        /// Flags specifying which versions of the internet protocol to
        /// support when making a connection. All addresses returned during
        /// name resolution are tried until a successful connection is made.
        /// You can fine tune which versions of the internet protocol to use
        /// by adding or removing flags here. I.e., setting this property
        /// to FtpIpVersion.IPv4 will cause the connection process to
        /// ignore IPv6 addresses. The default value is ANY version.
        /// </summary>
        public FtpIpVersion InternetProtocolVersions
        {
            get => m_ipVersions;
            set => m_ipVersions = value;
        }

        private int m_socketPollInterval = 15000;

        /// <summary>
        /// Gets or sets the length of time in milliseconds
        /// that must pass since the last socket activity
        /// before calling <see cref="System.Net.Sockets.Socket.Poll"/> 
        /// on the socket to test for connectivity. 
        /// Setting this interval too low will
        /// have a negative impact on performance. Setting this
        /// interval to 0 disables Polling all together.
        /// The default value is 15 seconds.
        /// </summary>
        public int SocketPollInterval
        {
            get => m_socketPollInterval;
            set
            {
                m_socketPollInterval = value;
                if (m_stream != null)
                {
                    m_stream.SocketPollInterval = value;
                }
            }
        }

        private bool m_staleDataTest = true;

        /// <summary>
        /// Gets or sets a value indicating whether a test should be performed to
        /// see if there is stale (unrequested data) sitting on the socket. In some
        /// cases the control connection may time out but before the server closes
        /// the connection it might send a 4xx response that was unexpected and
        /// can cause synchronization errors with transactions. To avoid this
        /// problem the <see cref="o:Execute"/> method checks to see if there is any data
        /// available on the socket before executing a command. On Azure hosting
        /// platforms this check can cause an exception to be thrown. In order
        /// to work around the exception you can set this property to false
        /// which will skip the test entirely however doing so eliminates the
        /// best effort attempt of detecting such scenarios. See this thread
        /// for more details about the Azure problem:
        /// https://netftp.codeplex.com/discussions/535879
        /// </summary>
        public bool StaleDataCheck
        {
            get => m_staleDataTest;
            set => m_staleDataTest = value;
        }

        /// <summary>
        /// Returns true if the connection to the FTP server is open.
        /// WARNING: Returns true even if our credentials are incorrect but connection to the server is open.
        /// See the IsAuthenticated property if you want to check if we are correctly logged in.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (m_stream != null)
                {
                    return m_stream.IsConnected;
                }

                return false;
            }
        }

        private bool m_IsAuthenticated = false;

        /// <summary>
        /// Returns true if the connection to the FTP server is open and if the FTP server accepted our credentials.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                if (m_stream != null)
                {
                    return m_stream.IsConnected && m_IsAuthenticated;
                }

                return false;
            }
        }

        private bool m_threadSafeDataChannels = false;

        /// <summary>
        /// When this value is set to true (default) the control connection
        /// is cloned and a new connection the server is established for the
        /// data channel operation. This is a thread safe approach to make
        /// asynchronous operations on a single control connection transparent
        /// to the developer.
        /// </summary>
        public bool EnableThreadSafeDataConnections
        {
            get => m_threadSafeDataChannels;
            set => m_threadSafeDataChannels = value;
        }

        private int m_noopInterval = 0;

        /// <summary>
        /// Gets or sets the length of time in milliseconds after last command
        /// (NOOP or other) that a NOOP command is sent by <see cref="Noop"/>.
        /// This is called during downloading/uploading if
        /// <see cref="EnableThreadSafeDataConnections"/> is false. Setting this
        /// interval to 0 disables <see cref="Noop"/> all together.
        /// The default value is 0 (disabled).
        /// </summary>
        public int NoopInterval
        {
            get => m_noopInterval;
            set => m_noopInterval = value;
        }

        private bool m_checkCapabilities = true;

        /// <summary>
        /// When this value is set to true (default) the control connection
        /// will set which features are available by executing the FEAT command
        /// when the connect method is called.
        /// </summary>
        public bool CheckCapabilities
        {
            get => m_checkCapabilities;
            set => m_checkCapabilities = value;
        }

        private bool m_isClone = false;

        /// <summary>
        /// Gets a value indicating if this control connection is a clone. This property
        /// is used with data streams to determine if the connection should be closed
        /// when the stream is closed. Servers typically only allow 1 data connection
        /// per control connection. If you try to open multiple data connections this
        /// object will be cloned for 2 or more resulting in N new connections to the
        /// server.
        /// </summary>
        internal bool IsClone
        {
            get => m_isClone;
            private set => m_isClone = value;
        }

        private Encoding m_textEncoding = Encoding.ASCII;
        private bool m_textEncodingAutoUTF = true;

        /// <summary>
        /// Gets or sets the text encoding being used when talking with the server. The default
        /// value is <see cref="System.Text.Encoding.ASCII"/> however upon connection, the client checks
        /// for UTF8 support and if it's there this property is switched over to
        /// <see cref="System.Text.Encoding.UTF8"/>. Manually setting this value overrides automatic detection
        /// based on the FEAT list; if you change this value it's always used
        /// regardless of what the server advertises, if anything.
        /// </summary>
        public Encoding Encoding
        {
            get => m_textEncoding;
            set
            {
                lock (m_lock)
                {
                    m_textEncoding = value;
                    m_textEncodingAutoUTF = false;
                }
            }
        }

        private string m_host = null;

        /// <summary>
        /// The server to connect to
        /// </summary>
        public string Host
        {
            get => m_host;
            set
            {
                // remove unwanted prefix/postfix
                if (value.StartsWith("ftp://"))
                {
                    value = value.Substring(value.IndexOf("ftp://") + "ftp://".Length);
                }

                if (value.EndsWith("/"))
                {
                    value = value.Replace("/", "");
                }

                m_host = value;
            }
        }

        private int m_port = 0;

        /// <summary>
        /// The port to connect to. If this value is set to 0 (Default) the port used
        /// will be determined by the type of SSL used or if no SSL is to be used it 
        /// will automatically connect to port 21.
        /// </summary>
        public int Port
        {
            get
            {
                // automatically determine port
                // when m_port is 0.
                if (m_port == 0)
                {
                    if (EncryptionMode == FtpEncryptionMode.Implicit)
                    {
                        return 990;
                    }
                    else
                    {
                        return 21;
                    }
                }

                return m_port;
            }
            set => m_port = value;
        }

        private NetworkCredential m_credentials = new NetworkCredential("anonymous", "anonymous");

        /// <summary>
        /// Credentials used for authentication
        /// </summary>
        public NetworkCredential Credentials
        {
            get => m_credentials;
            set => m_credentials = value;
        }

        private int m_maxDerefCount = 20;

        /// <summary>
        /// Gets or sets a value that controls the maximum depth
        /// of recursion that <see cref="o:DereferenceLink"/> will follow symbolic
        /// links before giving up. You can also specify the value
        /// to be used as one of the overloaded parameters to the
        /// <see cref="o:DereferenceLink"/> method. The default value is 20. Specifying
        /// -1 here means indefinitely try to resolve a link. This is
        /// not recommended for obvious reasons (stack overflow).
        /// </summary>
        public int MaximumDereferenceCount
        {
            get => m_maxDerefCount;
            set => m_maxDerefCount = value;
        }

        private X509CertificateCollection m_clientCerts = new X509CertificateCollection();

        /// <summary>
        /// Client certificates to be used in SSL authentication process
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get => m_clientCerts;
            protected set => m_clientCerts = value;
        }

        // Holds the cached resolved address
        private string m_Address;

        private Func<string> m_AddressResolver;

        /// <summary>
        /// Delegate used for resolving local address, used for active data connections
        /// This can be used in case you're behind a router, but port forwarding is configured to forward the
        /// ports from your router to your internal IP. In that case, we need to send the router's IP instead of our internal IP.
        /// See example: FtpClient.GetPublicIP -> This uses Ipify api to find external IP
        /// </summary>
        public Func<string> AddressResolver
        {
            get => m_AddressResolver;
            set => m_AddressResolver = value;
        }

        private IEnumerable<int> m_ActivePorts;

        /// <summary>
        /// Ports used for Active Data Connection.
        /// Useful when your FTP server has certain ports that are blocked or used for other purposes.
        /// </summary>
        public IEnumerable<int> ActivePorts
        {
            get => m_ActivePorts;
            set => m_ActivePorts = value;
        }

        private IEnumerable<int> m_PassiveBlockedPorts;

        /// <summary>
        /// Ports blocked for Passive Data Connection (PASV and EPSV).
        /// Useful when your FTP server has certain ports that are blocked or used for other purposes.
        /// </summary>
        public IEnumerable<int> PassiveBlockedPorts
        {
            get => m_PassiveBlockedPorts;
            set => m_PassiveBlockedPorts = value;
        }

        private int m_PassiveMaxAttempts = 100;

        /// <summary>
        /// Maximum number of passive connections made in order to find a working port for Passive Data Connection (PASV and EPSV).
        /// Only used if PassiveBlockedPorts is non-null.
        /// </summary>
        public int PassiveMaxAttempts
        {
            get => m_PassiveMaxAttempts;
            set => m_PassiveMaxAttempts = value;
        }

        private FtpDataConnectionType m_dataConnectionType = FtpDataConnectionType.AutoPassive;

        /// <summary>
        /// Data connection type, default is AutoPassive which tries
        /// a connection with EPSV first and if it fails then tries
        /// PASV before giving up. If you know exactly which kind of
        /// connection you need you can slightly increase performance
        /// by defining a specific type of passive or active data
        /// connection here.
        /// </summary>
        public FtpDataConnectionType DataConnectionType
        {
            get => m_dataConnectionType;
            set => m_dataConnectionType = value;
        }

        private bool m_ungracefullDisconnect = false;

        /// <summary>
        /// Disconnect from the server without sending QUIT. This helps
        /// work around IOExceptions caused by buggy connection resets
        /// when closing the control connection.
        /// </summary>
        public bool UngracefullDisconnection
        {
            get => m_ungracefullDisconnect;
            set => m_ungracefullDisconnect = value;
        }

        private int m_connectTimeout = 15000;

        /// <summary>
        /// Gets or sets the length of time in milliseconds to wait for a connection 
        /// attempt to succeed before giving up. Default is 15000 (15 seconds).
        /// </summary>
        public int ConnectTimeout
        {
            get => m_connectTimeout;
            set => m_connectTimeout = value;
        }

        private int m_readTimeout = 15000;

        /// <summary>
        /// Gets or sets the length of time wait in milliseconds for data to be
        /// read from the underlying stream. The default value is 15000 (15 seconds).
        /// </summary>
        public int ReadTimeout
        {
            get => m_readTimeout;
            set => m_readTimeout = value;
        }

        private int m_dataConnectionConnectTimeout = 15000;

        /// <summary>
        /// Gets or sets the length of time in milliseconds for a data connection
        /// to be established before giving up. Default is 15000 (15 seconds).
        /// </summary>
        public int DataConnectionConnectTimeout
        {
            get => m_dataConnectionConnectTimeout;
            set => m_dataConnectionConnectTimeout = value;
        }

        private int m_dataConnectionReadTimeout = 15000;

        /// <summary>
        /// Gets or sets the length of time in milliseconds the data channel
        /// should wait for the server to send data. Default value is 
        /// 15000 (15 seconds).
        /// </summary>
        public int DataConnectionReadTimeout
        {
            get => m_dataConnectionReadTimeout;
            set => m_dataConnectionReadTimeout = value;
        }

        private bool m_keepAlive = false;

        /// <summary>
        /// Gets or sets a value indicating if <see cref="System.Net.Sockets.SocketOptionName.KeepAlive"/> should be set on 
        /// the underlying stream's socket. If the connection is alive, the option is
        /// adjusted in real-time. The value is stored and the KeepAlive option is set
        /// accordingly upon any new connections. The value set here is also applied to
        /// all future data streams. It has no affect on cloned control connections or
        /// data connections already in progress. The default value is false.
        /// </summary>
        public bool SocketKeepAlive
        {
            get => m_keepAlive;
            set
            {
                m_keepAlive = value;
                if (m_stream != null)
                {
                    m_stream.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, value);
                }
            }
        }

        private List<FtpCapability> m_capabilities = null;

        /// <summary>
        /// Gets the server capabilities represented by an array of capability flags
        /// </summary>
        public List<FtpCapability> Capabilities
        {
            get
            {

                // FIX #683: if capabilities are already loaded, don't check if connected and return straightaway
                if (m_capabilities != null && m_capabilities.Count > 0)
                {
                    return m_capabilities;
                }

                // FIX #683: while using async operations, it is possible that the stream is not
                // connected, so don't connect using synchronous connection logic
                if (m_stream == null)
                {
                    throw new FtpException("Please call Connect() before trying to read the Capabilities!");
                }

                return m_capabilities;
            }
            protected set => m_capabilities = value;
        }

        private FtpHashAlgorithm m_hashAlgorithms = FtpHashAlgorithm.NONE;

        /// <summary>
        /// Get the hash types supported by the server for use with the HASH Command.
        /// This is a recent extension to the protocol that is not fully
        /// standardized and is not guaranteed to work. See here for
        /// more details:
        /// http://tools.ietf.org/html/draft-bryan-ftpext-hash-02
        /// </summary>
        public FtpHashAlgorithm HashAlgorithms
        {
            get
            {

                // FIX #683: if hash types are already loaded, don't check if connected and return straightaway
                if (m_hashAlgorithms != FtpHashAlgorithm.NONE)
                {
                    return m_hashAlgorithms;
                }

                // FIX #683: while using async operations, it is possible that the stream is not
                // connected, so don't connect using synchronous connection logic
                if (m_stream == null || !m_stream.IsConnected)
                {
                    throw new FtpException("Please call Connect() before trying to read the HashAlgorithms!");
                }

                return m_hashAlgorithms;
            }
            private set => m_hashAlgorithms = value;
        }

        private FtpEncryptionMode m_encryptionmode = FtpEncryptionMode.None;

        /// <summary>
        /// Type of SSL to use, or none. Default is none. Explicit is TLS, Implicit is SSL.
        /// </summary>
        public FtpEncryptionMode EncryptionMode
        {
            get => m_encryptionmode;
            set => m_encryptionmode = value;
        }

        private bool m_dataConnectionEncryption = true;

        /// <summary>
        /// Indicates if data channel transfers should be encrypted. Only valid if <see cref="EncryptionMode"/>
        /// property is not equal to <see cref="FtpEncryptionMode.None"/>.
        /// </summary>
        public bool DataConnectionEncryption
        {
            get => m_dataConnectionEncryption;
            set => m_dataConnectionEncryption = value;
        }

#if !NETFx
        private bool m_plainTextEncryption = false;

        /// <summary>
        /// Indicates if the encryption should be disabled immediately after connecting using a CCC command.
        /// This is useful when you have a FTP firewall that requires plaintext FTP, but your server mandates FTPS connections.
        /// </summary>
        public bool PlainTextEncryption
        {
            get => m_plainTextEncryption;
            set => m_plainTextEncryption = value;
        }
#endif

#if NETFx || NET45
        private SslProtocols m_SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
#else
        private SslProtocols m_SslProtocols = SslProtocols.Default;
#endif
        /// <summary>
        /// Encryption protocols to use. Only valid if EncryptionMode property is not equal to <see cref="FtpEncryptionMode.None"/>.
        /// Default value is .NET Framework defaults from the <see cref="System.Net.Security.SslStream"/> class.
        /// </summary>
        public SslProtocols SslProtocols
        {
            get => m_SslProtocols;
            set => m_SslProtocols = value;
        }

        private FtpsBuffering m_SslBuffering = FtpsBuffering.Auto;

        /// <summary>
        /// Whether to use SSL Buffering to speed up data transfer during FTP operations.
        /// SSL Buffering is always disabled on .NET 5.0 due to platform issues (see issue 682 in FluentFTP issue tracker).
        /// </summary>
        public FtpsBuffering SslBuffering
        {
            get => m_SslBuffering;
            set => m_SslBuffering = value;
        }

        /// <summary>
        /// Checks if FTPS/SSL encryption is currently active.
        /// Useful to see if your server supports FTPS, when using FtpEncryptionMode.Auto. 
        /// </summary>
        public bool IsEncrypted
        {
            get => m_stream != null && m_stream.IsEncrypted;
        }

        private FtpSslValidation m_ValidateCertificate = null;

        /// <summary>
        /// Event is fired to validate SSL certificates. If this event is
        /// not handled and there are errors validating the certificate
        /// the connection will be aborted.
        /// Not fired if ValidateAnyCertificate is set to true.
        /// </summary>
        public event FtpSslValidation ValidateCertificate
        {
            add => m_ValidateCertificate += value;
            remove => m_ValidateCertificate -= value;
        }

        private bool m_ValidateAnyCertificate = false;

        /// <summary>
        /// Accept any SSL certificate received from the server and skip performing
        /// the validation using the ValidateCertificate callback.
        /// Useful for Powershell users.
        /// </summary>
        public bool ValidateAnyCertificate
        {
            get => m_ValidateAnyCertificate;
            set => m_ValidateAnyCertificate = value;
        }

        private bool m_ValidateCertificateRevocation = false;

        /// <summary>
        /// Indicates if the certificate revocation list is checked during authentication.
        /// Useful when you need to maintain the certificate chain validation,
        /// but skip the certificate revocation check.
        /// WARNING: Enabling this can cause memory leaks in some conditions (see issue #710 for details).
        /// </summary>
        public bool ValidateCertificateRevocation
        {
            get => m_ValidateCertificateRevocation;
            set => m_ValidateCertificateRevocation = value;
        }

        private string m_systemType = "UNKNOWN";

        /// <summary>
        /// Gets the type of system/server that we're connected to. Typically begins with "WINDOWS" or "UNIX".
        /// </summary>
        public string SystemType => m_systemType;

        private FtpServer m_serverType = FtpServer.Unknown;

        /// <summary>
        /// Gets the type of the FTP server software that we're connected to.
        /// </summary>
        public FtpServer ServerType => m_serverType;

        private FtpBaseServer m_serverHandler;

        /// <summary>
        /// Gets the type of the FTP server handler.
        /// This is automatically set based on the detected FTP server, if it is detected. 
        /// You can manually set this property to implement handling for a custom FTP server.
        /// </summary>
        public FtpBaseServer ServerHandler
        {
            get => m_serverHandler;
            set => m_serverHandler = value;
        }

        private FtpOperatingSystem m_serverOS = FtpOperatingSystem.Unknown;

        /// <summary>
        /// Gets the operating system of the FTP server that we're connected to.
        /// </summary>
        public FtpOperatingSystem ServerOS => m_serverOS;

        private string m_connectionType = "Default";

        /// <summary> Gets the connection type </summary>
        public string ConnectionType
        {
            get => m_connectionType;
            protected set => m_connectionType = value;
        }

        private FtpReply m_lastReply;

        /// <summary> Gets the last reply received from the server</summary>
        public FtpReply LastReply
        {
            get => m_lastReply;
            protected set => m_lastReply = value;
        }


        private FtpDataType m_ListingDataType = FtpDataType.Binary;

        /// <summary>
        /// Controls if the file listings are downloaded in Binary or ASCII mode.
        /// </summary>
        public FtpDataType ListingDataType
        {
            get => m_ListingDataType;
            set => m_ListingDataType = value;
        }

        private FtpParser m_parser = FtpParser.Auto;

        /// <summary>
        /// File listing parser to be used. 
        /// Automatically calculated based on the type of the server at the time of connection.
        /// If you want to override this property, make sure to do it after calling Connect.
        /// </summary>
        public FtpParser ListingParser
        {
            get => m_parser;
            set
            {
                m_parser = value;

                // configure parser
                m_listParser.CurrentParser = value;
                m_listParser.ParserConfirmed = false;
            }
        }

        private CultureInfo m_parserCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Culture used to parse file listings
        /// </summary>
        public CultureInfo ListingCulture
        {
            get => m_parserCulture;
            set => m_parserCulture = value;
        }

        private CustomParser m_customParser = null;

        /// <summary>
        /// Custom file listing parser to be used.
        /// </summary>
        public CustomParser ListingCustomParser
        {
            get => m_customParser;
            set
            {
                m_customParser = value;

                // modify the ListingParser to note that a custom parser is set
                if (value != null)
                {
                    ListingParser = FtpParser.Custom;
                }
                else
                {
                    ListingParser = FtpParser.Auto;
                }
            }
        }

        /// <summary>
        /// Callback format to implement your custom FTP listing line parser.
        /// </summary>
        /// <param name="line">The line from the listing</param>
        /// <param name="capabilities">The server capabilities</param>
        /// <param name="client">The FTP client</param>
        /// <returns>Return an FtpListItem object if the line can be parsed, else return null</returns>
        public delegate FtpListItem CustomParser(string line, List<FtpCapability> capabilities, FtpClient client);

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public bool RecursiveList
        {
            get
            {

                // If the user has confirmed support on his server, return true
                if (_RecursiveListSupported)
                {
                    return true;
                }

                // ask the server handler if it supports recursive listing
                if (ServerHandler != null && ServerHandler.RecursiveList())
                {
                    return true;
                }
                return false;

            }
            set
            {
                // You can always set this property if you are sure about
                // your server's support for recursive listing
                _RecursiveListSupported = value;
            }
        }


        private double m_serverTimeZone = 0;
        private TimeSpan m_serverTimeOffset = new TimeSpan();

        /// <summary>
        /// The timezone of the FTP server. If the server is in Tokyo with UTC+9 then set this to 9.
        /// If the server returns timestamps in UTC then keep this 0.
        /// </summary>
        public double TimeZone
        {
            get => m_serverTimeZone;
            set
            {
                if (value < -14 || value > 14)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "TimeZone must be within -14 to +14 to represent UTC-14 to UTC+14");
                }
                m_serverTimeZone = value;

                // configure parser
                if (value == 0)
                {
                    m_serverTimeOffset = TimeSpan.Zero;
                }
                else
                {
                    var hours = (int)Math.Floor(m_serverTimeZone);
                    var mins = (int)Math.Floor((m_serverTimeZone - Math.Floor(m_serverTimeZone)) * 60);
                    m_serverTimeOffset = new TimeSpan(hours, mins, 0);
                }
            }
        }


#if NETFx
        private double m_localTimeZone = 0;
        private TimeSpan m_localTimeOffset = new TimeSpan();

        /// <summary>
        /// The timezone of your machine. If your machine is in Tokyo with UTC+9 then set this to 9.
        /// If your machine is synchronized with UTC then keep this 0.
        /// </summary>
        public double LocalTimeZone
        {
            get => m_localTimeZone;
            set
            {
                if (value < -14 || value > 14)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "LocalTimeZone must be within -14 to +14 to represent UTC-14 to UTC+14");
                }
                m_localTimeZone = value;

                // configure parser
                if (value == 0)
                {
                    m_localTimeOffset = TimeSpan.Zero;
                }
                else
                {
                    var hours = (int)Math.Floor(m_localTimeZone);
                    var mins = (int)Math.Floor((m_localTimeZone - Math.Floor(m_localTimeZone)) * 60);
                    m_localTimeOffset = new TimeSpan(hours, mins, 0);
                }
            }
        }
#endif

        private FtpDate m_timeConversion = FtpDate.ServerTime;

        /// <summary>
        /// Server timestamps are converted into the given timezone.
        /// ServerTime will return the original timestamp.
        /// LocalTime will convert the timestamp into your local machine's timezone.
        /// UTC will convert the timestamp into UTC format (GMT+0).
        /// You need to set TimeZone and LocalTimeZone (.NET core only) for these to work.
        /// </summary>
        public FtpDate TimeConversion
        {
            get => m_timeConversion;
            set
            {
                m_timeConversion = value;
            }
        }

        private bool m_bulkListing = true;

        /// <summary>
        /// If true, increases performance of GetListing by reading multiple lines
        /// of the file listing at once. If false then GetListing will read file
        /// listings line-by-line. If GetListing is having issues with your server,
        /// set it to false.
        /// 
        /// The number of bytes read is based upon <see cref="BulkListingLength"/>.
        /// </summary>
        public bool BulkListing
        {
            get => m_bulkListing;
            set => m_bulkListing = value;
        }

        private int m_bulkListingLength = 128;

        /// <summary>
        /// Bytes to read during GetListing. Only honored if <see cref="BulkListing"/> is true.
        /// </summary>
        public int BulkListingLength
        {
            get => m_bulkListingLength;
            set => m_bulkListingLength = value;
        }

        private int? m_transferChunkSize;

        /// <summary>
        /// Gets or sets the number of bytes transferred in a single chunk (a single FTP command).
        /// Used by <see cref="o:UploadFile"/>/<see cref="o:UploadFileAsync"/> and <see cref="o:DownloadFile"/>/<see cref="o:DownloadFileAsync"/>
        /// to transfer large files in multiple chunks.
        /// </summary>
        public int TransferChunkSize
        {
            get => m_transferChunkSize ?? 65536;
            set => m_transferChunkSize = value;
        }

        private int? m_localFileBufferSize;

        /// <summary>
        /// Gets or sets the size of the file buffer when reading and writing files on the local file system.
        /// Used by <see cref="o:UploadFile"/>/<see cref="o:UploadFileAsync"/> and <see cref="o:DownloadFile"/>/<see cref="o:DownloadFileAsync"/>
        /// and all the other file and directory transfer methods.
        /// </summary>
        public int LocalFileBufferSize
        {
            get => m_localFileBufferSize ?? 4096;
            set => m_localFileBufferSize = value;
        }

        private int m_quickTransferSize = (10 * 1024 * 1024);

        /// <summary>
        /// Files within this size are read and written in a single call to the disk, thereby greatly increasing transfer performance. Measured in bytes.
        /// Reduce this if you notice large memory consumption by FluentFTP. Set this to 0 to disable quick transfer.
        /// </summary>
        internal int QuickTransferLimit
        {
            get => m_quickTransferSize;
            set => m_quickTransferSize = value;
        }

        private FtpDataType CurrentDataType;

        private int m_retryAttempts = 3;

        /// <summary>
        /// Gets or sets the retry attempts allowed when a verification failure occurs during download or upload.
        /// This value must be set to 1 or more.
        /// </summary>
        public int RetryAttempts
        {
            get => m_retryAttempts;
            set => m_retryAttempts = value > 0 ? value : 1;
        }

        private uint m_uploadRateLimit = 0;

        /// <summary>
        /// Rate limit for uploads in kbyte/s. Set this to 0 for unlimited speed.
        /// Honored by high-level API such as Upload(), Download(), UploadFile(), DownloadFile()..
        /// </summary>
        public uint UploadRateLimit
        {
            get => m_uploadRateLimit;
            set => m_uploadRateLimit = value;
        }

        private uint m_downloadRateLimit = 0;

        /// <summary>
        /// Rate limit for downloads in kbytes/s. Set this to 0 for unlimited speed.
        /// Honored by high-level API such as Upload(), Download(), UploadFile(), DownloadFile()..
        /// </summary>
        public uint DownloadRateLimit
        {
            get => m_downloadRateLimit;
            set => m_downloadRateLimit = value;
        }

        private bool m_DownloadZeroByteFiles = true;

        /// <summary>
        /// Controls if zero-byte files should be downloaded or skipped.
        /// If false, then no file is created/overwritten into the filesystem.
        /// </summary>
        public bool DownloadZeroByteFiles
        {
            get => m_DownloadZeroByteFiles;
            set => m_DownloadZeroByteFiles = value;
        }

        private FtpDataType m_UploadDataType = FtpDataType.Binary;

        /// <summary>
        /// Controls if the high-level API uploads files in Binary or ASCII mode.
        /// </summary>
        public FtpDataType UploadDataType
        {
            get => m_UploadDataType;
            set => m_UploadDataType = value;
        }

        private FtpDataType m_DownloadDataType = FtpDataType.Binary;

        /// <summary>
        /// Controls if the high-level API downloads files in Binary or ASCII mode.
        /// </summary>
        public FtpDataType DownloadDataType
        {
            get => m_DownloadDataType;
            set => m_DownloadDataType = value;
        }

        private bool m_UploadDirectoryDeleteExcluded = true;

        /// <summary>
        /// Controls if the UploadDirectory API deletes the excluded files when uploading in Mirror mode.
        /// If true, then any files that are excluded will be deleted from the FTP server if they are
        /// excluded from the local system. This is done to keep the server in sync with the local system.
        /// But if it is false, the excluded files are not touched on the server, and simply ignored.
        /// </summary>
        public bool UploadDirectoryDeleteExcluded
        {
            get => m_UploadDirectoryDeleteExcluded;
            set => m_UploadDirectoryDeleteExcluded = value;
        }

        private bool m_DownloadDirectoryDeleteExcluded = true;

        /// <summary>
        /// Controls if the DownloadDirectory API deletes the excluded files when downloading in Mirror mode.
        /// If true, then any files that are excluded will be deleted from the local filesystem if they are
        /// excluded from the FTP server. This is done to keep the local filesystem in sync with the FTP server.
        /// But if it is false, the excluded files are not touched on the local filesystem, and simply ignored.
        /// </summary>
        public bool DownloadDirectoryDeleteExcluded
        {
            get => m_DownloadDirectoryDeleteExcluded;
            set => m_DownloadDirectoryDeleteExcluded = value;
        }

        private FtpDataType m_FXPDataType = FtpDataType.Binary;

        /// <summary>
        /// Controls if the FXP server-to-server file transfer API uses Binary or ASCII mode.
        /// </summary>
        public FtpDataType FXPDataType
        {
            get => m_FXPDataType;
            set => m_FXPDataType = value;
        }

        private int m_FXPProgressInterval = 1000;

        /// <summary>
        /// Controls how often the progress reports are sent during an FXP file transfer.
        /// The default value is 1000 (1 second).
        /// </summary>
        public int FXPProgressInterval
        {
            get => m_FXPProgressInterval;
            set => m_FXPProgressInterval = value;
        }

        private bool m_SendHost;
        /// <summary>
        /// Controls if the HOST command is sent immediately after the handshake.
        /// Useful when you are using shared hosting and you need to inform the
        /// FTP server which domain you want to connect to.
        /// </summary>
        public bool SendHost
        {
            get => m_SendHost;
            set => m_SendHost = value;
        }

        private string m_SendHostDomain = null;
        /// <summary>
        /// Controls which domain is sent with the HOST command.
        /// If this is null, then the Host parameter of the FTP client is sent.
        /// </summary>
        public string SendHostDomain
        {
            get => m_SendHostDomain;
            set => m_SendHostDomain = value;
        }

#if !NET40
        private IPAddress m_SocketLocalIp;
        /// <summary>
        /// The local socket will be bound to the given local IP/interface.
        /// This is useful if you have several usable public IP addresses and want to use a particular one.
        /// </summary>
        public IPAddress SocketLocalIp
        {
            get => m_SocketLocalIp;
            set => m_SocketLocalIp = value;
        }
#endif

        /// <summary>
        /// Returns the local end point of the FTP socket, if it is available.
        /// </summary>
        public IPEndPoint SocketLocalEndPoint
        {
            get => m_stream?.LocalEndPoint;
        }

        /// <summary>
        /// Returns the remote end point of the FTP socket, if it is available.
        /// </summary>
        public IPEndPoint SocketRemoteEndPoint
        {
            get => m_stream?.RemoteEndPoint;
        }

        private FtpZOSListRealm m_zOSListingRealm;

        /// <summary>
        /// During and after a z/OS GetListing(), this value shows the
        /// z/OS filesystem realm that was encountered.
        /// </summary>
        public FtpZOSListRealm zOSListingRealm
        {
            get => m_zOSListingRealm;
            set => m_zOSListingRealm = value;
        }

        private ushort m_zOSListingLRECL;

        /// <summary>
        /// During and after a z/OS GetListing(), this value shows the
        /// the LRECL that was encountered (for a realm = Member only).
        /// The value is used internally to calculate member sizes
        /// </summary>
        public ushort zOSListingLRECL
        {
            get => m_zOSListingLRECL;
            set => m_zOSListingLRECL = value;
        }

        // ADD PROPERTIES THAT NEED TO BE CLONED INTO
        // FtpClient.CloneConnection()
        #endregion // FtpClient_Properties.cs
        #region // FtpClient_Static.cs
        /// <summary>
        /// Connects to the specified URI. If the path specified by the URI ends with a
        /// / then the working directory is changed to the path specified.
        /// </summary>
        /// <param name="uri">The URI to parse</param>
        /// <param name="checkcertificate">Indicates if a ssl certificate should be validated when using FTPS schemes</param>
        /// <returns>FtpClient object</returns>
        public static FtpClient Connect(Uri uri, bool checkcertificate)
        {
            var cl = new FtpClient();

            if (uri == null)
            {
                throw new ArgumentException("Invalid URI object");
            }

            switch (uri.Scheme.ToLower())
            {
                case "ftp":
                case "ftps":
                    break;

                default:
                    throw new UriFormatException("The specified URI scheme is not supported. Please use ftp:// or ftps://");
            }

            cl.Host = uri.Host;
            cl.Port = uri.Port;

            if (uri.UserInfo != null && uri.UserInfo.Length > 0)
            {
                if (uri.UserInfo.Contains(":"))
                {
                    var parts = uri.UserInfo.Split(':');

                    if (parts.Length != 2)
                    {
                        throw new UriFormatException("The user info portion of the URI contains more than 1 colon. The username and password portion of the URI should be URL encoded.");
                    }

                    cl.Credentials = new NetworkCredential(DecodeUrl(parts[0]), DecodeUrl(parts[1]));
                }
                else
                {
                    cl.Credentials = new NetworkCredential(DecodeUrl(uri.UserInfo), "");
                }
            }
            else
            {
                // if no credentials were supplied just make up
                // some for anonymous authentication.
                cl.Credentials = new NetworkCredential("ftp", "ftp");
            }

            cl.ValidateCertificate += new FtpSslValidation(delegate (FtpClient control, FtpSslValidationEventArgs e)
            {
                if (e.PolicyErrors != System.Net.Security.SslPolicyErrors.None && checkcertificate)
                {
                    e.Accept = false;
                }
                else
                {
                    e.Accept = true;
                }
            });

            cl.Connect();

            if (uri.PathAndQuery != null && uri.PathAndQuery.EndsWith("/"))
            {
                cl.SetWorkingDirectory(uri.PathAndQuery);
            }

            return cl;
        }

        /// <summary>
        /// Connects to the specified URI. If the path specified by the URI ends with a
        /// / then the working directory is changed to the path specified.
        /// </summary>
        /// <param name="uri">The URI to parse</param>
        /// <returns>FtpClient object</returns>
        public static FtpClient Connect(Uri uri)
        {
            return Connect(uri, true);
        }

        /// <summary>
        /// Calculate you public internet IP using the ipify service. Returns null if cannot be calculated.
        /// </summary>
        /// <returns>Public IP Address</returns>
        public static string GetPublicIP()
        {
            try
            {
#if NETFx
                return new System.Net.Http.HttpClient().GetStringAsync("https://api.ipify.org").Result;
#else
                var request = WebRequest.Create("https://api.ipify.org");
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        return stream.ReadToEnd();
                    }
                }
#endif
            }
            catch { }
            return null;
        }
        #endregion // FtpClient_Static.cs
        #region // FtpClient_Stream.cs
        #region Execute Command

        /// <summary>
        /// When last command was sent (NOOP or other), for having <see cref="Noop"/>
        /// respect the <see cref="NoopInterval"/>.
        /// </summary>
        private DateTime m_lastCommandUtc;

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>The servers reply to the command</returns>
        public FtpReply Execute(string command)
        {
            FtpReply reply;
            lock (m_lock)
            {
                if (StaleDataCheck)
                {
                    ReadStaleData(true, false, true);
                }

                if (!IsConnected)
                {
                    if (command == "QUIT")
                    {
                        LogStatus(FtpTraceLevel.Info, "Not sending QUIT because the connection has already been closed.");
                        return new FtpReply()
                        {
                            Code = "200",
                            Message = "Connection already closed."
                        };
                    }

                    Connect();
                }

                // hide sensitive data from logs
                var commandTxt = command;
                if (!FtpTrace.LogUserName && command.StartsWith("USER", StringComparison.Ordinal))
                {
                    commandTxt = "USER ***";
                }

                if (!FtpTrace.LogPassword && command.StartsWith("PASS", StringComparison.Ordinal))
                {
                    commandTxt = "PASS ***";
                }

                // A CWD will invalidate the cached value.
                if (command.StartsWith("CWD ", StringComparison.Ordinal))
                {
                    _LastWorkingDir = null;
                }

                LogLine(FtpTraceLevel.Info, "Command:  " + commandTxt);

                // send command to FTP server
                m_stream.WriteLine(m_textEncoding, command);
                m_lastCommandUtc = DateTime.UtcNow;
                reply = GetReply();
            }
            return reply;
        }

#if NET40
        private delegate FtpReply AsyncExecute(string command);

        /// <summary>
        /// Performs execution of the specified command asynchronously
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> method</param>
        /// <param name="state">State object</param>
        /// <returns>IAsyncResult</returns>
        public IAsyncResult BeginExecute(string command, AsyncCallback callback, object state)
        {
            AsyncExecute func;
            IAsyncResult ar;

            lock (m_asyncmethods)
            {
                ar = (func = new AsyncExecute(Execute)).BeginInvoke(command, callback, state);
                m_asyncmethods.Add(ar, func);
            }

            return ar;
        }

        /// <summary>
        /// Ends an asynchronous command
        /// </summary>
        /// <param name="ar">IAsyncResult returned from BeginExecute</param>
        /// <returns>FtpReply object (never null).</returns>
        public FtpReply EndExecute(IAsyncResult ar)
        {
            return GetAsyncDelegate<AsyncExecute>(ar).EndInvoke(ar);
        }
#endif

#if !NET40
        /// <summary>
        /// Performs an asynchronous execution of the specified command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>The servers reply to the command</returns>
        public async Task<FtpReply> ExecuteAsync(string command, CancellationToken token)
        {
            FtpReply reply;

            if (StaleDataCheck)
            {
#if NETFx
                await ReadStaleDataAsync(true, false, true, token);
#else
				ReadStaleData(true, false, true);
#endif
            }

            if (!IsConnected)
            {
                if (command == "QUIT")
                {
                    LogStatus(FtpTraceLevel.Info, "Not sending QUIT because the connection has already been closed.");
                    return new FtpReply()
                    {
                        Code = "200",
                        Message = "Connection already closed."
                    };
                }

                await ConnectAsync(token);
            }

            // hide sensitive data from logs
            var commandTxt = command;
            if (!FtpTrace.LogUserName && command.StartsWith("USER", StringComparison.Ordinal))
            {
                commandTxt = "USER ***";
            }

            if (!FtpTrace.LogPassword && command.StartsWith("PASS", StringComparison.Ordinal))
            {
                commandTxt = "PASS ***";
            }

            // A CWD will invalidate the cached value.
            if (command.StartsWith("CWD ", StringComparison.Ordinal))
            {
                _LastWorkingDir = null;
            }

            LogLine(FtpTraceLevel.Info, "Command:  " + commandTxt);

            // send command to FTP server
            await m_stream.WriteLineAsync(m_textEncoding, command, token);
            m_lastCommandUtc = DateTime.UtcNow;
            reply = await GetReplyAsync(token);

            return reply;
        }
#endif

        /// <summary>
        /// Sends the NOOP command according to <see cref="NoopInterval"/> (effectively a no-op if 0).
        /// Please call <see cref="GetReply"/> as needed to read the "OK" command sent by the server and prevent stale data on the socket.
        /// Note that response is not guaranteed by all FTP servers when sent during file transfers.
        /// </summary>
        /// <returns>true if NOOP command was sent</returns>
        public bool Noop()
        {
            if (m_noopInterval > 0 && DateTime.UtcNow.Subtract(m_lastCommandUtc).TotalMilliseconds > m_noopInterval)
            {
                LogLine(FtpTraceLevel.Verbose, "Command:  NOOP");

                m_stream.WriteLine(m_textEncoding, "NOOP");
                m_lastCommandUtc = DateTime.UtcNow;

                return true;
            }

            return false;
        }

#if !NET40
        /// <summary>
        /// Sends the NOOP command according to <see cref="NoopInterval"/> (effectively a no-op if 0).
        /// Please call <see cref="GetReplyAsync"/> as needed to read the "OK" command sent by the server and prevent stale data on the socket.
        /// Note that response is not guaranteed by all FTP servers when sent during file transfers.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>true if NOOP command was sent</returns>
        private async Task<bool> NoopAsync(CancellationToken token)
        {
            if (m_noopInterval > 0 && DateTime.UtcNow.Subtract(m_lastCommandUtc).TotalMilliseconds > m_noopInterval)
            {
                LogLine(FtpTraceLevel.Verbose, "Command:  NOOP");

                await m_stream.WriteLineAsync(m_textEncoding, "NOOP", token);
                m_lastCommandUtc = DateTime.UtcNow;

                return true;
            }

            return false;
        }
#endif

        #endregion

        #region Get Reply

        /// <summary>
        /// Retrieves a reply from the server. Do not execute this method
        /// unless you are sure that a reply has been sent, i.e., you
        /// executed a command. Doing so will cause the code to hang
        /// indefinitely waiting for a server reply that is never coming.
        /// </summary>
        /// <returns>FtpReply representing the response from the server</returns>
        public FtpReply GetReply()
        {
            var reply = new FtpReply();
            string buf;
            lock (m_lock)
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("No connection to the server has been established.");
                }

                m_stream.ReadTimeout = m_readTimeout;
                while ((buf = m_stream.ReadLine(Encoding)) != null)
                {
                    if (DecodeStringToReply(buf, ref reply))
                    {
                        break;
                    }
                    reply.InfoMessages += buf + "\n";
                }

                // log multiline response messages
                if (reply.InfoMessages != null)
                {
                    reply.InfoMessages = reply.InfoMessages.Trim();
                }

                if (!string.IsNullOrEmpty(reply.InfoMessages))
                {
                    //this.LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                    LogLine(FtpTraceLevel.Verbose, reply.InfoMessages.Split('\n').AddPrefix("Response: ", true).Join("\n"));

                    //this.LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
                }

                // if reply received
                if (reply.Code != null)
                {

                    // hide sensitive data from logs
                    var logMsg = reply.Message;
                    if (!FtpTrace.LogUserName && reply.Code == "331" && logMsg.StartsWith("User ", StringComparison.Ordinal) && logMsg.Contains(" OK"))
                    {
                        logMsg = logMsg.Replace(Credentials.UserName, "***");
                    }

                    // log response code + message
                    LogLine(FtpTraceLevel.Info, "Response: " + reply.Code + " " + logMsg);
                }
            }
            LastReply = reply;

            return reply;
        }

        /// <summary>
        /// Decodes the given FTP response string into a FtpReply, separating the FTP return code and message.
        /// Returns true if the string was decoded correctly or false if it is not a standard format FTP response.
        /// </summary>
        private bool DecodeStringToReply(string text, ref FtpReply reply)
        {
            Match m = Regex.Match(text, "^(?<code>[0-9]{3}) (?<message>.*)$");
            if (m.Success)
            {
                reply.Code = m.Groups["code"].Value;
                reply.Message = m.Groups["message"].Value;
            }
            return m.Success;
        }

#if !NET40
        // TODO: add example
        /// <summary>
        /// Retrieves a reply from the server. Do not execute this method
        /// unless you are sure that a reply has been sent, i.e., you
        /// executed a command. Doing so will cause the code to hang
        /// indefinitely waiting for a server reply that is never coming.
        /// </summary>
        /// <returns>FtpReply representing the response from the server</returns>
        public async Task<FtpReply> GetReplyAsync(CancellationToken token)
        {
            var reply = new FtpReply();
            string buf;

            if (!IsConnected)
            {
                throw new InvalidOperationException("No connection to the server has been established.");
            }

            m_stream.ReadTimeout = m_readTimeout;
            while ((buf = await m_stream.ReadLineAsync(Encoding, token)) != null)
            {
                if (DecodeStringToReply(buf, ref reply))
                {
                    break;
                }
                reply.InfoMessages += buf + "\n";
            }

            // log multiline response messages
            if (reply.InfoMessages != null)
            {
                reply.InfoMessages = reply.InfoMessages.Trim();
            }

            if (!string.IsNullOrEmpty(reply.InfoMessages))
            {
                //this.LogLine(FtpTraceLevel.Verbose, "+---------------------------------------+");
                LogLine(FtpTraceLevel.Verbose, reply.InfoMessages.Split('\n').AddPrefix("Response: ", true).Join("\n"));

                //this.LogLine(FtpTraceLevel.Verbose, "-----------------------------------------");
            }

            // if reply received
            if (reply.Code != null)
            {
                // hide sensitive data from logs
                var logMsg = reply.Message;
                if (!FtpTrace.LogUserName && reply.Code == "331" && logMsg.StartsWith("User ", StringComparison.Ordinal) && logMsg.Contains(" OK"))
                {
                    logMsg = logMsg.Replace(Credentials.UserName, "***");
                }

                // log response code + message
                LogLine(FtpTraceLevel.Info, "Response: " + reply.Code + " " + logMsg);
            }

            LastReply = reply;

            return reply;
        }
#endif

        #endregion

        #region Active/Passive Streams

        /// <summary>
        /// Opens the specified type of passive data stream
        /// </summary>
        /// <param name="type">Type of passive data stream to open</param>
        /// <param name="command">The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <returns>A data stream ready to be used</returns>
        private FtpDataStream OpenPassiveDataStream(FtpDataConnectionType type, string command, long restart)
        {
            LogFunc(nameof(OpenPassiveDataStream), new object[] { type, command, restart });

            FtpDataStream stream = null;
            FtpReply reply;
            string host = null;
            var port = 0;

            if (m_stream == null)
            {
                throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open a passive data stream.");
            }

            for (int a = 0; a <= m_PassiveMaxAttempts;)
            {

                if ((type == FtpDataConnectionType.EPSV || type == FtpDataConnectionType.AutoPassive) && !_EPSVNotSupported)
                {

                    // execute EPSV to try enhanced-passive mode
                    if (!(reply = Execute("EPSV")).Success)
                    {

                        // if we're connected with IPv4 and data channel type is AutoPassive then fallback to IPv4
                        if ((reply.Type == FtpResponseType.TransientNegativeCompletion || reply.Type == FtpResponseType.PermanentNegativeCompletion)
                            && type == FtpDataConnectionType.AutoPassive
                            && m_stream != null
                            && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // mark EPSV not supported so we do not try EPSV again during this connection
                            _EPSVNotSupported = true;
                            return OpenPassiveDataStream(FtpDataConnectionType.PASV, command, restart);
                        }

                        // throw this unknown error
                        throw new FtpCommandException(reply);
                    }

                    // read the connection port from the EPSV response
                    GetEnhancedPassivePort(reply, out host, out port);

                }
                else
                {
                    if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
                    {
                        throw new FtpException("Only IPv4 is supported by the PASV command. Use EPSV instead.");
                    }

                    // execute PRET before passive if server requires it
                    if (HasFeature(FtpCapability.PRET))
                    {
                        reply = Execute("PRET " + command);
                    }

                    // execute PASV to try passive mode
                    if (!(reply = Execute("PASV")).Success)
                    {
                        throw new FtpCommandException(reply);
                    }

                    // get the passive port taking proxy config into account (if any)
                    GetPassivePort(type, reply, out host, out port);

                }



                // break if too many tries
                a++;
                if (a >= m_PassiveMaxAttempts)
                {
                    throw new FtpException("Could not find a suitable port for PASV/EPSV Data Connection after trying " + m_PassiveMaxAttempts + " times.");
                }

                // accept first port if not configured
                if (m_PassiveBlockedPorts.IsBlank())
                {
                    break;
                }
                else
                {

                    // check port against blacklist if configured
                    if (!m_PassiveBlockedPorts.Contains(port))
                    {

                        // blacklist does not port, accept it
                        break;
                    }
                    else
                    {

                        // blacklist contains port, try again
                        continue;
                    }
                }

            }

            stream = new FtpDataStream(this);
            stream.ConnectTimeout = DataConnectionConnectTimeout;
            stream.ReadTimeout = DataConnectionReadTimeout;
            Connect(stream, host, port, InternetProtocolVersions);
            stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);

            if (restart > 0)
            {
                if (!(reply = Execute("REST " + restart)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }

            if (!(reply = Execute(command)).Success)
            {
                stream.Close();
                if (command.StartsWith("NLST ") && reply.Code == "550" && reply.Message == "No files found.")
                {
                    //workaround for ftpd which responses "550 No files found." when folder exists but is empty
                }
                else
                {
                    throw new FtpCommandException(reply);
                }
            }

            // the command status is used to determine
            // if a reply needs to be read from the server
            // when the stream is closed so always set it
            // otherwise things can get out of sync.
            stream.CommandStatus = reply;
            // this needs to take place after the command is executed
            if (m_dataConnectionEncryption && m_encryptionmode != FtpEncryptionMode.None)
            {
                stream.ActivateEncryption(m_host,
                    ClientCertificates.Count > 0 ? ClientCertificates : null,
                    m_SslProtocols);
            }
            return stream;
        }

#if !NET40
        /// <summary>
        /// Opens the specified type of passive data stream
        /// </summary>
        /// <param name="type">Type of passive data stream to open</param>
        /// <param name="command">The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <param name="token"></param>
        /// <returns>A data stream ready to be used</returns>
        private async Task<FtpDataStream> OpenPassiveDataStreamAsync(FtpDataConnectionType type, string command, long restart, CancellationToken token = default(CancellationToken))
        {
            LogFunc(nameof(OpenPassiveDataStreamAsync), new object[] { type, command, restart });

            FtpDataStream stream = null;
            FtpReply reply;
            string host = null;
            var port = 0;

            if (m_stream == null)
            {
                throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open a passive data stream.");
            }


            for (int a = 0; a <= m_PassiveMaxAttempts;)
            {

                if ((type == FtpDataConnectionType.EPSV || type == FtpDataConnectionType.AutoPassive) && !_EPSVNotSupported)
                {
                    // execute EPSV to try enhanced-passive mode
                    if (!(reply = await ExecuteAsync("EPSV", token)).Success)
                    {
                        // if we're connected with IPv4 and data channel type is AutoPassive then fallback to IPv4
                        if ((reply.Type == FtpResponseType.TransientNegativeCompletion || reply.Type == FtpResponseType.PermanentNegativeCompletion)
                            && type == FtpDataConnectionType.AutoPassive
                            && m_stream != null
                            && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // mark EPSV not supported so we do not try EPSV again during this connection
                            _EPSVNotSupported = true;
                            return await OpenPassiveDataStreamAsync(FtpDataConnectionType.PASV, command, restart, token);
                        }

                        // throw this unknown error
                        throw new FtpCommandException(reply);
                    }

                    // read the connection port from the EPSV response
                    GetEnhancedPassivePort(reply, out host, out port);

                }
                else
                {
                    if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
                    {
                        throw new FtpException("Only IPv4 is supported by the PASV command. Use EPSV instead.");
                    }

                    // execute PRET before passive if server requires it
                    if (HasFeature(FtpCapability.PRET))
                    {
                        reply = await ExecuteAsync("PRET " + command, token);
                    }

                    // execute PASV to try passive mode
                    if (!(reply = await ExecuteAsync("PASV", token)).Success)
                    {
                        throw new FtpCommandException(reply);
                    }

                    // get the passive port taking proxy config into account (if any)
                    GetPassivePort(type, reply, out host, out port);

                }


                // break if too many tries
                a++;
                if (a >= m_PassiveMaxAttempts)
                {
                    throw new FtpException("Could not find a suitable port for PASV/EPSV Data Connection after trying " + m_PassiveMaxAttempts + " times.");
                }

                // accept first port if not configured
                if (m_PassiveBlockedPorts.IsBlank())
                {
                    break;
                }
                else
                {

                    // check port against blacklist if configured
                    if (!m_PassiveBlockedPorts.Contains(port))
                    {

                        // blacklist does not port, accept it
                        break;
                    }
                    else
                    {

                        // blacklist contains port, try again
                        continue;
                    }
                }
            }

            stream = new FtpDataStream(this);
            stream.ConnectTimeout = DataConnectionConnectTimeout;
            stream.ReadTimeout = DataConnectionReadTimeout;
            await ConnectAsync(stream, host, port, InternetProtocolVersions, token);
            stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);

            if (restart > 0)
            {
                if (!(reply = await ExecuteAsync("REST " + restart, token)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }

            if (!(reply = await ExecuteAsync(command, token)).Success)
            {
                stream.Close();
                throw new FtpCommandException(reply);
            }

            // the command status is used to determine
            // if a reply needs to be read from the server
            // when the stream is closed so always set it
            // otherwise things can get out of sync.
            stream.CommandStatus = reply;
            // this needs to take place after the command is executed
            if (m_dataConnectionEncryption && m_encryptionmode != FtpEncryptionMode.None && !_ConnectionFTPSFailure)
            {
                await stream.ActivateEncryptionAsync(m_host,
                    ClientCertificates.Count > 0 ? ClientCertificates : null,
                    m_SslProtocols);
            }
            return stream;
        }
#endif

        /// <summary>
        /// Parse the host and port number from an EPSV response
        /// </summary>
        private void GetEnhancedPassivePort(FtpReply reply, out string host, out int port)
        {
            var m = Regex.Match(reply.Message, @"\(\|\|\|(?<port>\d+)\|\)");
            if (!m.Success)
            {
                // In the case that ESPV is responded with a regular "Entering Passive Mode" instead, we'll try that parsing before we raise the exception
                /* Example:
				Command: EPSV
				Response: 227 Entering Passive Mode(XX, XX, XX, XX, 143, 225).
				*/

                try
                {
                    GetPassivePort(FtpDataConnectionType.AutoPassive, reply, out host, out port);
                    return;
                }
                catch
                {
                    throw new FtpException("Failed to get the EPSV port from: " + reply.Message);
                }
            }
            // If ESPV is responded with Entering Extended Passive. The IP must remain the same.
            /* Example:
			Command: EPSV
			Response: 229 Entering Extended Passive Mode(|||10016|)

			If we set the host to ftp.host.com and ftp.host.com has multiple ip's we may end up with the wrong ip.
			Making sure that we use the same IP.
			host = m_host; 
			*/
            host = SocketRemoteEndPoint.Address.ToString();
            port = int.Parse(m.Groups["port"].Value);
        }

        /// <summary>
        /// Parse the host and port number from an PASV or PASVEX response
        /// </summary>
        private void GetPassivePort(FtpDataConnectionType type, FtpReply reply, out string host, out int port)
        {
            var m = Regex.Match(reply.Message, @"(?<quad1>\d+)," + @"(?<quad2>\d+)," + @"(?<quad3>\d+)," + @"(?<quad4>\d+)," + @"(?<port1>\d+)," + @"(?<port2>\d+)");

            if (!m.Success || m.Groups.Count != 7)
            {
                throw new FtpException("Malformed PASV response: " + reply.Message);
            }

            // PASVEX mode ignores the host supplied in the PASV response
            if (type == FtpDataConnectionType.PASVEX)
            {
                host = m_host;
            }
            else
            {
                host = m.Groups["quad1"].Value + "." + m.Groups["quad2"].Value + "." + m.Groups["quad3"].Value + "." + m.Groups["quad4"].Value;
            }

            port = (int.Parse(m.Groups["port1"].Value) << 8) + int.Parse(m.Groups["port2"].Value);

            // Fix #409 for BlueCoat proxy connections. This code replaces the name of the proxy with the name of the FTP server and then nothing works.
            if (!IsProxy())
            {
                //use host ip if server advertises a non-routable IP
                m = Regex.Match(host, @"(^10\.)|(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^192\.168\.)|(^127\.0\.0\.1)|(^0\.0\.0\.0)");

                if (m.Success)
                {
                    host = m_host;
                }
            }
        }

        /// <summary>
        /// Returns the ip address to be sent to the server for the active connection
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string GetLocalAddress(IPAddress ip)
        {
            // Use resolver
            if (m_AddressResolver != null)
            {
                return m_Address ?? (m_Address = m_AddressResolver());
            }

            // Use supplied ip
            return ip.ToString();
        }

        /// <summary>
        /// Opens the specified type of active data stream
        /// </summary>
        /// <param name="type">Type of passive data stream to open</param>
        /// <param name="command">The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <returns>A data stream ready to be used</returns>
        private FtpDataStream OpenActiveDataStream(FtpDataConnectionType type, string command, long restart)
        {
            LogFunc(nameof(OpenActiveDataStream), new object[] { type, command, restart });

            var stream = new FtpDataStream(this);
            FtpReply reply;
#if !NETFx
            IAsyncResult ar;
#endif

            if (m_stream == null)
            {
                throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open an active data stream.");
            }

            StartListeningOnPort(stream);
#if NETFx
            var args = stream.BeginAccept();
#else
            ar = stream.BeginAccept(null, null);
#endif

            if (type == FtpDataConnectionType.EPRT || type == FtpDataConnectionType.AutoActive)
            {
                var ipver = 0;

                switch (stream.LocalEndPoint.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        ipver = 1; // IPv4
                        break;

                    case AddressFamily.InterNetworkV6:
                        ipver = 2; // IPv6
                        break;

                    default:
                        throw new InvalidOperationException("The IP protocol being used is not supported.");
                }

                if (!(reply = Execute("EPRT |" + ipver + "|" + GetLocalAddress(stream.LocalEndPoint.Address) + "|" + stream.LocalEndPoint.Port + "|")).Success)
                {
                    // if we're connected with IPv4 and the data channel type is AutoActive then try to fall back to the PORT command
                    if (reply.Type == FtpResponseType.PermanentNegativeCompletion && type == FtpDataConnectionType.AutoActive && m_stream != null && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
                    {
                        stream.ControlConnection = null; // we don't want this failed EPRT attempt to close our control connection when the stream is closed so clear out the reference.
                        stream.Close();
                        return OpenActiveDataStream(FtpDataConnectionType.PORT, command, restart);
                    }
                    else
                    {
                        stream.Close();
                        throw new FtpCommandException(reply);
                    }
                }
            }
            else
            {
                if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
                {
                    throw new FtpException("Only IPv4 is supported by the PORT command. Use EPRT instead.");
                }

                if (!(reply = Execute("PORT " +
                                      GetLocalAddress(stream.LocalEndPoint.Address).Replace('.', ',') + "," +
                                      stream.LocalEndPoint.Port / 256 + "," +
                                      stream.LocalEndPoint.Port % 256)).Success)
                {
                    stream.Close();
                    throw new FtpCommandException(reply);
                }
            }

            if (restart > 0)
            {
                if (!(reply = Execute("REST " + restart)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }

            if (!(reply = Execute(command)).Success)
            {
                stream.Close();
                throw new FtpCommandException(reply);
            }

            // the command status is used to determine
            // if a reply needs to be read from the server
            // when the stream is closed so always set it
            // otherwise things can get out of sync.
            stream.CommandStatus = reply;

#if NETFx
            stream.EndAccept(args, m_dataConnectionConnectTimeout);
#else
            ar.AsyncWaitHandle.WaitOne(m_dataConnectionConnectTimeout);
            ar.AsyncWaitHandle.Close();
            if (!ar.IsCompleted)
            {
                stream.Close();
                throw new TimeoutException("Timed out waiting for the server to connect to the active data socket.");
            }

            stream.EndAccept(ar);
#endif
            if (m_dataConnectionEncryption && m_encryptionmode != FtpEncryptionMode.None)
            {
                stream.ActivateEncryption(m_host,
                    ClientCertificates.Count > 0 ? ClientCertificates : null,
                    m_SslProtocols);
            }
            stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
            stream.ReadTimeout = m_dataConnectionReadTimeout;

            return stream;
        }

#if !NET40
        /// <summary>
        /// Opens the specified type of active data stream
        /// </summary>
        /// <param name="type">Type of passive data stream to open</param>
        /// <param name="command">The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A data stream ready to be used</returns>
        private async Task<FtpDataStream> OpenActiveDataStreamAsync(FtpDataConnectionType type, string command, long restart, CancellationToken token = default(CancellationToken))
        {
            LogFunc(nameof(OpenActiveDataStreamAsync), new object[] { type, command, restart });

            var stream = new FtpDataStream(this);
            FtpReply reply;

#if !NETFx
			IAsyncResult ar;
#endif

            if (m_stream == null)
            {
                throw new InvalidOperationException("The control connection stream is null! Generally this means there is no connection to the server. Cannot open an active data stream.");
            }

            StartListeningOnPort(stream);

#if NETFx
            var args = stream.BeginAccept();
#else
			ar = stream.BeginAccept(null, null);
#endif

            if (type == FtpDataConnectionType.EPRT || type == FtpDataConnectionType.AutoActive)
            {
                var ipver = 0;

                switch (stream.LocalEndPoint.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        ipver = 1; // IPv4
                        break;

                    case AddressFamily.InterNetworkV6:
                        ipver = 2; // IPv6
                        break;

                    default:
                        throw new InvalidOperationException("The IP protocol being used is not supported.");
                }

                if (!(reply = await ExecuteAsync("EPRT |" + ipver + "|" + GetLocalAddress(stream.LocalEndPoint.Address) + "|" + stream.LocalEndPoint.Port + "|", token)).Success)
                {
                    // if we're connected with IPv4 and the data channel type is AutoActive then try to fall back to the PORT command
                    if (reply.Type == FtpResponseType.PermanentNegativeCompletion && type == FtpDataConnectionType.AutoActive && m_stream != null && m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetwork)
                    {
                        stream.ControlConnection = null; // we don't want this failed EPRT attempt to close our control connection when the stream is closed so clear out the reference.
                        stream.Close();
                        return await OpenActiveDataStreamAsync(FtpDataConnectionType.PORT, command, restart, token);
                    }
                    else
                    {
                        stream.Close();
                        throw new FtpCommandException(reply);
                    }
                }
            }
            else
            {
                if (m_stream.LocalEndPoint.AddressFamily != AddressFamily.InterNetwork)
                {
                    throw new FtpException("Only IPv4 is supported by the PORT command. Use EPRT instead.");
                }

                if (!(reply = await ExecuteAsync("PORT " +
                                                 GetLocalAddress(stream.LocalEndPoint.Address).Replace('.', ',') + "," +
                                                 stream.LocalEndPoint.Port / 256 + "," +
                                                 stream.LocalEndPoint.Port % 256, token)).Success)
                {
                    stream.Close();
                    throw new FtpCommandException(reply);
                }
            }

            if (restart > 0)
            {
                if (!(reply = await ExecuteAsync("REST " + restart, token)).Success)
                {
                    throw new FtpCommandException(reply);
                }
            }

            if (!(reply = await ExecuteAsync(command, token)).Success)
            {
                stream.Close();
                throw new FtpCommandException(reply);
            }

            // the command status is used to determine
            // if a reply needs to be read from the server
            // when the stream is closed so always set it
            // otherwise things can get out of sync.
            stream.CommandStatus = reply;

#if NETFx
            stream.EndAccept(args, m_dataConnectionConnectTimeout);
#else
			ar.AsyncWaitHandle.WaitOne(m_dataConnectionConnectTimeout);
			ar.AsyncWaitHandle.Close();
			if (!ar.IsCompleted) {
				stream.Close();
				throw new TimeoutException("Timed out waiting for the server to connect to the active data socket.");
			}

			stream.EndAccept(ar);
#endif
            if (m_dataConnectionEncryption && m_encryptionmode != FtpEncryptionMode.None && !_ConnectionFTPSFailure)
            {
                await stream.ActivateEncryptionAsync(m_host,
                    ClientCertificates.Count > 0 ? ClientCertificates : null,
                    m_SslProtocols);
            }
            stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_keepAlive);
            stream.ReadTimeout = m_dataConnectionReadTimeout;

            return stream;
        }
#endif
        /// <summary>
        /// Opens a data stream.
        /// </summary>
        /// <param name='command'>The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <returns>The data stream.</returns>
        private FtpDataStream OpenDataStream(string command, long restart)
        {
            var type = m_dataConnectionType;
            FtpDataStream stream = null;
            lock (m_lock)
            {
                if (!IsConnected)
                {
                    Connect();
                }

                // The PORT and PASV commands do not work with IPv6 so
                // if either one of those types are set change them
                // to EPSV or EPRT appropriately.
                if (m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    switch (type)
                    {
                        case FtpDataConnectionType.PORT:
                            type = FtpDataConnectionType.EPRT;
                            LogLine(FtpTraceLevel.Info, "Changed data connection type to EPRT because we are connected with IPv6.");
                            break;

                        case FtpDataConnectionType.PASV:
                        case FtpDataConnectionType.PASVEX:
                            type = FtpDataConnectionType.EPSV;
                            LogLine(FtpTraceLevel.Info, "Changed data connection type to EPSV because we are connected with IPv6.");
                            break;
                    }
                }

                switch (type)
                {
                    case FtpDataConnectionType.AutoPassive:
                    case FtpDataConnectionType.EPSV:
                    case FtpDataConnectionType.PASV:
                    case FtpDataConnectionType.PASVEX:
                        stream = OpenPassiveDataStream(type, command, restart);
                        break;

                    case FtpDataConnectionType.AutoActive:
                    case FtpDataConnectionType.EPRT:
                    case FtpDataConnectionType.PORT:
                        stream = OpenActiveDataStream(type, command, restart);
                        break;
                }

                if (stream == null)
                {
                    throw new InvalidOperationException("The specified data channel type is not implemented.");
                }
            }
            return stream;
        }

#if !NET40
        /// <summary>
        /// Opens a data stream.
        /// </summary>
        /// <param name='command'>The command to execute that requires a data stream</param>
        /// <param name="restart">Restart location in bytes for file transfer</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>The data stream.</returns>
        private async Task<FtpDataStream> OpenDataStreamAsync(string command, long restart, CancellationToken token = default(CancellationToken))
        {
            var type = m_dataConnectionType;
            FtpDataStream stream = null;

            if (!IsConnected)
            {
                await ConnectAsync(token);
            }

            // The PORT and PASV commands do not work with IPv6 so
            // if either one of those types are set change them
            // to EPSV or EPRT appropriately.
            if (m_stream.LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                switch (type)
                {
                    case FtpDataConnectionType.PORT:
                        type = FtpDataConnectionType.EPRT;
                        LogLine(FtpTraceLevel.Info, "Changed data connection type to EPRT because we are connected with IPv6.");
                        break;

                    case FtpDataConnectionType.PASV:
                    case FtpDataConnectionType.PASVEX:
                        type = FtpDataConnectionType.EPSV;
                        LogLine(FtpTraceLevel.Info, "Changed data connection type to EPSV because we are connected with IPv6.");
                        break;
                }
            }

            switch (type)
            {
                case FtpDataConnectionType.AutoPassive:
                case FtpDataConnectionType.EPSV:
                case FtpDataConnectionType.PASV:
                case FtpDataConnectionType.PASVEX:
                    stream = await OpenPassiveDataStreamAsync(type, command, restart, token);
                    break;

                case FtpDataConnectionType.AutoActive:
                case FtpDataConnectionType.EPRT:
                case FtpDataConnectionType.PORT:
                    stream = await OpenActiveDataStreamAsync(type, command, restart, token);
                    break;
            }

            if (stream == null)
            {
                throw new InvalidOperationException("The specified data channel type is not implemented.");
            }

            return stream;
        }
#endif

        /// <summary>
        /// Disconnects a data stream
        /// </summary>
        /// <param name="stream">The data stream to close</param>
        internal FtpReply CloseDataStream(FtpDataStream stream)
        {
            LogFunc(nameof(CloseDataStream));

            var reply = new FtpReply();

            if (stream == null)
            {
                throw new ArgumentException("The data stream parameter was null");
            }
            lock (m_lock)
            {
                try
                {
                    if (IsConnected)
                    {
                        // if the command that required the data connection was
                        // not successful then there will be no reply from
                        // the server, however if the command was successful
                        // the server will send a reply when the data connection
                        // is closed.
                        if (stream.CommandStatus.Type == FtpResponseType.PositivePreliminary)
                        {
                            if (!(reply = GetReply()).Success)
                            {
                                throw new FtpCommandException(reply);
                            }
                        }
                    }
                }
                finally
                {
                    // if this is a clone of the original control
                    // connection we should Dispose()
                    if (IsClone)
                    {
                        Disconnect();
                        Dispose();
                    }
                }
            }
            return reply;
        }

        /// <summary>
        /// Open a local port on the given ActivePort or a random port.
        /// </summary>
        /// <param name="stream"></param>
        private void StartListeningOnPort(FtpDataStream stream)
        {
            if (m_ActivePorts.IsBlank())
            {
                // Use random port
                stream.Listen(m_stream.LocalEndPoint.Address, 0);
            }
            else
            {
                var success = false;

                // Use one of the specified ports
                foreach (var port in m_ActivePorts)
                {
                    try
                    {
                        stream.Listen(m_stream.LocalEndPoint.Address, port);
                        success = true;
                        break;
                    }
                    catch (SocketException se)
                    {
#if NETFrame
						// Already in use
						if (se.ErrorCode != 10048) {
							throw;
						}
#else
                        if (se.SocketErrorCode != SocketError.AddressAlreadyInUse)
                        {
                            throw;
                        }
#endif
                    }
                }

                // No usable port found
                if (!success)
                {
                    throw new Exception("No valid active data port available!");
                }
            }
        }

        #endregion

        #region Open Read

        /// <summary>
        /// Opens the specified file for reading
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="restart">Resume location</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <returns>A stream for reading the file on the server</returns>
        [Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
        public virtual Stream OpenRead(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true)
        {
            return OpenRead(path, type, restart, checkIfFileExists ? 0 : -1);
        }

        /// <summary>
        /// Opens the specified file for reading
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="restart">Resume location</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <returns>A stream for reading the file on the server</returns>
        [Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
        public virtual Stream OpenRead(string path, FtpDataType type, long restart, long fileLen)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenRead), new object[] { path, type, restart, fileLen });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;
            lock (m_lock)
            {
                if (m_threadSafeDataChannels)
                {
                    client = CloneConnection();
                    client.CopyStateFlags(this);
                    client.Connect();
                    client.SetWorkingDirectory(GetWorkingDirectory());
                }
                else
                {
                    client = this;
                }

                length = fileLen == 0 ? client.GetFileSize(path) : fileLen;

                client.SetDataType(type);
                stream = client.OpenDataStream("RETR " + path, restart);
            }
            if (stream != null)
            {
                if (length > 0)
                {
                    stream.SetLength(length);
                }

                if (restart > 0)
                {
                    stream.SetPosition(restart);
                }
            }

            return stream;
        }

#if !NET40

        /// <summary>
        /// Opens the specified file for reading asynchronously
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="restart">Resume location</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for reading the file on the server</returns>
        [Obsolete("OpenReadAsync() is obsolete, please use DownloadAsync() or DownloadFileAsync() instead", false)]
        public virtual Task<Stream> OpenReadAsync(string path, FtpDataType type = FtpDataType.Binary, long restart = 0,
            bool checkIfFileExists = true, CancellationToken token = default(CancellationToken))
        {
            return OpenReadAsync(path, type, restart, checkIfFileExists ? 0 : -1, token);
        }

        /// <summary>
        /// Opens the specified file for reading asynchronously
        /// </summary>
        /// <param name="path">The full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="restart">Resume location</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for reading the file on the server</returns>
        [Obsolete("OpenReadAsync() is obsolete, please use DownloadAsync() or DownloadFileAsync() instead", false)]
        public virtual async Task<Stream> OpenReadAsync(string path, FtpDataType type, long restart, long fileLen, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenReadAsync), new object[] { path, type, restart, fileLen });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;

            if (m_threadSafeDataChannels)
            {
                client = CloneConnection();
                client.CopyStateFlags(this);
                await client.ConnectAsync(token);
                await client.SetWorkingDirectoryAsync(await GetWorkingDirectoryAsync(token), token);
            }
            else
            {
                client = this;
            }

            length = fileLen == 0 ? await client.GetFileSizeAsync(path, -1, token) : fileLen;

            await client.SetDataTypeAsync(type, token);
            stream = await client.OpenDataStreamAsync("RETR " + path, restart, token);

            if (stream != null)
            {
                if (length > 0)
                {
                    stream.SetLength(length);
                }

                if (restart > 0)
                {
                    stream.SetPosition(restart);
                }
            }

            return stream;
        }

#endif

        #endregion

        #region Open Write

        /// <summary>
        /// Opens the specified file for writing. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenWrite() is obsolete, please use Upload() or UploadFile() instead", false)]
        public virtual Stream OpenWrite(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true)
        {
            return OpenWrite(path, type, checkIfFileExists ? 0 : -1);
        }

        /// <summary>
        /// Opens the specified file for writing. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenWrite() is obsolete, please use Upload() or UploadFile() instead", false)]
        public virtual Stream OpenWrite(string path, FtpDataType type, long fileLen)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenWrite), new object[] { path, type });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;
            lock (m_lock)
            {
                if (m_threadSafeDataChannels)
                {
                    client = CloneConnection();
                    client.CopyStateFlags(this);
                    client.Connect();
                    client.SetWorkingDirectory(GetWorkingDirectory());
                }
                else
                {
                    client = this;
                }

                length = fileLen == 0 ? client.GetFileSize(path) : fileLen;

                client.SetDataType(type);
                stream = client.OpenDataStream("STOR " + path, 0);

                if (length > 0 && stream != null)
                {
                    stream.SetLength(length);
                }
            }
            return stream;
        }
#if !NET40
        /// <summary>
        /// Opens the specified file for writing. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenWriteAsync() is obsolete, please use UploadAsync() or UploadFileAsync() instead", false)]
        public virtual Task<Stream> OpenWriteAsync(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default(CancellationToken))
        {
            return OpenWriteAsync(path, type, checkIfFileExists ? 0 : -1, token);
        }

        /// <summary>
        /// Opens the specified file for writing. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenWriteAsync() is obsolete, please use UploadAsync() or UploadFileAsync() instead", false)]
        public virtual async Task<Stream> OpenWriteAsync(string path, FtpDataType type, long fileLen, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenWriteAsync), new object[] { path, type });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;

            if (m_threadSafeDataChannels)
            {
                client = CloneConnection();
                client.CopyStateFlags(this);
                await client.ConnectAsync(token);
                await client.SetWorkingDirectoryAsync(await GetWorkingDirectoryAsync(token), token);
            }
            else
            {
                client = this;
            }

            length = fileLen == 0 ? await client.GetFileSizeAsync(path, -1, token) : fileLen;

            await client.SetDataTypeAsync(type, token);
            stream = await client.OpenDataStreamAsync("STOR " + path, 0, token);

            if (length > 0 && stream != null)
            {
                stream.SetLength(length);
            }

            return stream;
        }

#endif

        #endregion

        #region Open Append

        /// <summary>
        /// Opens the specified file for appending. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">The full or relative path to the file to be opened</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenAppend() is obsolete, please use UploadFile() with FtpRemoteExists.Resume or FtpRemoteExists.AddToEnd instead", false)]
        public virtual Stream OpenAppend(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true)
        {
            return OpenAppend(path, type, checkIfFileExists ? 0 : -1);
        }

        /// <summary>
        /// Opens the specified file for appending. Please call GetReply() after you have successfully transfered the file to read the "OK" command sent by the server and prevent stale data on the socket.
        /// </summary>
        /// <param name="path">The full or relative path to the file to be opened</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenAppend() is obsolete, please use UploadFile() with FtpRemoteExists.Resume or FtpRemoteExists.AddToEnd instead", false)]
        public virtual Stream OpenAppend(string path, FtpDataType type, long fileLen)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenAppend), new object[] { path, type });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;
            lock (m_lock)
            {
                if (m_threadSafeDataChannels)
                {
                    client = CloneConnection();
                    client.CopyStateFlags(this);
                    client.Connect();
                    client.SetWorkingDirectory(GetWorkingDirectory());
                }
                else
                {
                    client = this;
                }

                length = fileLen == 0 ? client.GetFileSize(path) : fileLen;

                client.SetDataType(type);
                stream = client.OpenDataStream("APPE " + path, 0);

                if (length > 0 && stream != null)
                {
                    stream.SetLength(length);
                    stream.SetPosition(length);
                }
            }
            return stream;
        }

#if !NET40
        /// <summary>
        /// Opens the specified file to be appended asynchronously
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenAppendAsync() is obsolete, please use UploadFileAsync() with FtpRemoteExists.Resume or FtpRemoteExists.AddToEnd instead", false)]
        public virtual Task<Stream> OpenAppendAsync(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default(CancellationToken))
        {
            return OpenAppendAsync(path, type, checkIfFileExists ? 0 : -1, token);
        }

        /// <summary>
        /// Opens the specified file to be appended asynchronously
        /// </summary>
        /// <param name="path">Full or relative path of the file</param>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="fileLen">
        /// <para>Pass in a file length if known</para>
        /// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
        /// <br> 0  => File length is unknown, try to determine it</br>
        /// <br> >0 => File length is KNOWN. No need to determine it</br>
        /// </param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        /// <returns>A stream for writing to the file on the server</returns>
        [Obsolete("OpenAppendAsync() is obsolete, please use UploadFileAsync() with FtpRemoteExists.Resume or FtpRemoteExists.AddToEnd instead", false)]
        public virtual async Task<Stream> OpenAppendAsync(string path, FtpDataType type, long fileLen, CancellationToken token = default(CancellationToken))
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            path = path.GetFtpPath();

            LogFunc(nameof(OpenAppendAsync), new object[] { path, type });

            FtpClient client = null;
            FtpDataStream stream = null;
            long length = 0;


            if (m_threadSafeDataChannels)
            {
                client = CloneConnection();
                client.CopyStateFlags(this);
                await client.ConnectAsync(token);
                await client.SetWorkingDirectoryAsync(await GetWorkingDirectoryAsync(token), token);
            }
            else
            {
                client = this;
            }

            length = fileLen == 0 ? await client.GetFileSizeAsync(path, -1, token) : fileLen;

            await client.SetDataTypeAsync(type, token);
            stream = await client.OpenDataStreamAsync("APPE " + path, 0, token);

            if (length > 0 && stream != null)
            {
                stream.SetLength(length);
                stream.SetPosition(length);
            }

            return stream;
        }

#endif

        #endregion

        #region Set Data Type
        /// <summary>
        /// 强制设置数据类型
        /// </summary>
        protected bool ForceSetDataType = false;

        /// <summary>
        /// Sets the data type of information sent over the data stream
        /// </summary>
        /// <param name="type">ASCII/Binary</param>
        protected void SetDataType(FtpDataType type)
        {
            lock (m_lock)
            {
                SetDataTypeNoLock(type);
            }
        }

        /// <summary>Internal method that handles actually setting the data type.</summary>
        /// <exception cref="FtpCommandException">Thrown when a FTP Command error condition occurs.</exception>
        /// <exception cref="FtpException">Thrown when a FTP error condition occurs.</exception>
        /// <param name="type">ASCII/Binary.</param>
        /// <remarks>This method doesn't do any locking to prevent recursive lock scenarios.  Callers must do their own locking.</remarks>
        private void SetDataTypeNoLock(FtpDataType type)
        {
            // FIX : #291 only change the data type if different
            if (CurrentDataType != type || ForceSetDataType)
            {
                // FIX : #318 always set the type when we create a new connection
                ForceSetDataType = false;

                FtpReply reply;
                switch (type)
                {
                    case FtpDataType.ASCII:
                        if (!(reply = Execute("TYPE A")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        break;

                    case FtpDataType.Binary:
                        if (!(reply = Execute("TYPE I")).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        break;

                    default:
                        throw new FtpException("Unsupported data type: " + type.ToString());
                }

                CurrentDataType = type;
            }
        }

#if !NET40
        /// <summary>
        /// Sets the data type of information sent over the data stream asynchronously
        /// </summary>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        protected async Task SetDataTypeAsync(FtpDataType type, CancellationToken token = default(CancellationToken))
        {

            await SetDataTypeNoLockAsync(type, token);
        }

        /// <summary>
        /// Sets the data type of information sent over the data stream asynchronously
        /// </summary>
        /// <param name="type">ASCII/Binary</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        protected async Task SetDataTypeNoLockAsync(FtpDataType type, CancellationToken token = default(CancellationToken))
        {
            // FIX : #291 only change the data type if different
            if (CurrentDataType != type || ForceSetDataType)
            {
                // FIX : #318 always set the type when we create a new connection
                ForceSetDataType = false;

                FtpReply reply;
                switch (type)
                {
                    case FtpDataType.ASCII:
                        if (!(reply = await ExecuteAsync("TYPE A", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        break;

                    case FtpDataType.Binary:
                        if (!(reply = await ExecuteAsync("TYPE I", token)).Success)
                        {
                            throw new FtpCommandException(reply);
                        }

                        break;

                    default:
                        throw new FtpException("Unsupported data type: " + type.ToString());
                }

                CurrentDataType = type;
            }
        }
#endif

        #endregion
        #endregion // FtpClient_Stream.cs
        #region // FtpClient_Timezone.cs
        /// <summary>
        /// If reverse is false, converts the date provided by the FTP server into the timezone required locally.
        /// If reverse is true, converts the local timezone date into the date required by the FTP server.
        /// 
        /// Affected by properties: TimeConversion, TimeZone, LocalTimeZone.
        /// </summary>
        public DateTime ConvertDate(DateTime date, bool reverse = false)
        {

            // if server time is wanted, don't perform any conversion
            if (m_timeConversion != FtpDate.ServerTime)
            {

                // convert server time to local time
                if (!reverse)
                {

                    // convert server timezone to UTC based on the TimeZone property
                    if (m_serverTimeZone != 0)
                    {
                        date = date - m_serverTimeOffset;
                    }

                    // convert UTC to local time if wanted (on .NET Core this is based on the LocalTimeZone property)
                    if (m_timeConversion == FtpDate.LocalTime)
                    {
#if NETFx
                        date = date + m_localTimeOffset;
#else
                        date = System.TimeZone.CurrentTimeZone.ToLocalTime(date);
#endif
                    }

                }

                // convert local time to server time
                else
                {

                    // convert local to UTC if wanted (on .NET Core this is based on the LocalTimeZone property)
                    if (m_timeConversion == FtpDate.LocalTime)
                    {
#if NETFx
                        date = date - m_localTimeOffset;
#else
                        date = System.TimeZone.CurrentTimeZone.ToUniversalTime(date);
#endif
                    }

                    // convert UTC to server timezone, based on the TimeZone property
                    if (m_serverTimeZone != 0)
                    {
                        date = date + m_serverTimeOffset;
                    }
                }
            }

            // return the final date value
            return date;
        }
        #endregion // FtpClient_Timezone.cs
        #region // FtpClient_Utils.cs
        /// <summary>
        /// Performs a bitwise and to check if the specified
        /// flag is set on the <see cref="Capabilities"/>  property.
        /// </summary>
        /// <param name="cap">The <see cref="FtpCapability"/> to check for</param>
        /// <returns>True if the feature was found, false otherwise</returns>
        public bool HasFeature(FtpCapability cap)
        {
            if (cap == FtpCapability.NONE && Capabilities.Count == 0)
            {
                return true;
            }

            return Capabilities.Contains(cap);
        }

        /// <summary>
        /// Retrieves the delegate for the specified IAsyncResult and removes
        /// it from the m_asyncmethods collection if the operation is successful
        /// </summary>
        /// <typeparam name="T">Type of delegate to retrieve</typeparam>
        /// <param name="ar">The IAsyncResult to retrieve the delegate for</param>
        /// <returns>The delegate that generated the specified IAsyncResult</returns>
        protected T GetAsyncDelegate<T>(IAsyncResult ar)
        {
            T func;

            lock (m_asyncmethods)
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException("This connection object has already been disposed.");
                }

                if (!m_asyncmethods.ContainsKey(ar))
                {
                    throw new InvalidOperationException("The specified IAsyncResult could not be located.");
                }

                if (!(m_asyncmethods[ar] is T))
                {
#if NETFx
                    throw new InvalidCastException("The AsyncResult cannot be matched to the specified delegate. ");
#else
                    var st = new StackTrace(1);

                    throw new InvalidCastException("The AsyncResult cannot be matched to the specified delegate. " + "Are you sure you meant to call " + st.GetFrame(0).GetMethod().Name + " and not another method?"
                    );
#endif
                }

                func = (T)m_asyncmethods[ar];
                m_asyncmethods.Remove(ar);
            }

            return func;
        }

        /// <summary>
        /// Ensure a relative path is absolute by appending the working dir
        /// </summary>
        private string GetAbsolutePath(string path)
        {

            if (path == null || path.Trim().Length == 0)
            {
                // if path not given, then use working dir
                var pwd = GetWorkingDirectory();
                if (pwd != null && pwd.Trim().Length > 0)
                {
                    path = pwd;
                }
                else
                {
                    path = "/";
                }

            }

            // FIX : #153 ensure this check works with unix & windows
            // FIX : #454 OpenVMS paths can be a single character
            else if (!path.StartsWith("/") && !(path.Length > 1 && path[1] == ':'))
            {

                // if its a server-specific absolute path then don't add base dir
                if (ServerHandler != null && ServerHandler.IsAbsolutePath(path))
                {
                    return path;
                }

                // if relative path given then add working dir to calc full path
                var pwd = GetWorkingDirectory();
                if (pwd != null && pwd.Trim().Length > 0 && path != pwd)
                {
                    // Check if PDS (MVS Dataset) file system
                    if (pwd.StartsWith("'") && ServerType == FtpServer.IBMzOSFTP)
                    {
                        // PDS that has single quotes is already fully qualified
                        return pwd;
                    }

                    if (path.StartsWith("./"))
                    {
                        path = path.Remove(0, 2);
                    }

                    path = (pwd + "/" + path).GetFtpPath();
                }
            }

            return path;
        }

#if !NET40
        /// <summary>
        /// Ensure a relative path is absolute by appending the working dir
        /// </summary>
        private async Task<string> GetAbsolutePathAsync(string path, CancellationToken token)
        {

            if (path == null || path.Trim().Length == 0)
            {
                // if path not given, then use working dir
                string pwd = await GetWorkingDirectoryAsync(token);
                if (pwd != null && pwd.Trim().Length > 0)
                {
                    path = pwd;
                }
                else
                {
                    path = "/";
                }
            }

            // FIX : #153 ensure this check works with unix & windows
            // FIX : #454 OpenVMS paths can be a single character
            else if (!path.StartsWith("/") && !(path.Length > 1 && path[1] == ':'))
            {

                // if its a server-specific absolute path then don't add base dir
                if (ServerHandler != null && ServerHandler.IsAbsolutePath(path))
                {
                    return path;
                }

                // if relative path given then add working dir to calc full path
                string pwd = await GetWorkingDirectoryAsync(token);
                if (pwd != null && pwd.Trim().Length > 0)
                {
                    if (path.StartsWith("./"))
                    {
                        path = path.Remove(0, 2);
                    }

                    path = (pwd + "/" + path).GetFtpPath();
                }
            }

            return path;
        }
#endif

        private static string DecodeUrl(string url)
        {
#if NETFx
            return WebUtility.UrlDecode(url);
#else
            return HttpUtility.UrlDecode(url);
#endif
        }

        /// <summary>
        /// Disables UTF8 support and changes the Encoding property
        /// back to ASCII. If the server returns an error when trying
        /// to turn UTF8 off a FtpCommandException will be thrown.
        /// </summary>
        public void DisableUTF8()
        {
            FtpReply reply;
            lock (m_lock)
            {
                if (!(reply = Execute("OPTS UTF8 OFF")).Success)
                {
                    throw new FtpCommandException(reply);
                }

                m_textEncoding = Encoding.ASCII;
                m_textEncodingAutoUTF = false;
            }
        }

        /// <summary>
        /// Data shouldn't be on the socket, if it is it probably means we've been disconnected.
        /// Read and discard whatever is there and optionally close the connection.
        /// Returns the stale data as text, if any, or null if none was found.
        /// </summary>
        /// <param name="closeStream">close the connection?</param>
        /// <param name="evenEncrypted">even read encrypted data?</param>
        /// <param name="traceData">trace data to logs?</param>
        private string ReadStaleData(bool closeStream, bool evenEncrypted, bool traceData)
        {
            string staleData = null;
            if (m_stream != null && m_stream.SocketDataAvailable > 0)
            {
                if (traceData)
                {
                    LogStatus(FtpTraceLevel.Info, "There is stale data on the socket, maybe our connection timed out or you did not call GetReply(). Re-connecting...");
                }

                if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted))
                {
                    var buf = new byte[m_stream.SocketDataAvailable];
                    m_stream.RawSocketRead(buf);
                    staleData = Encoding.GetString(buf).TrimEnd('\r', '\n');
                    if (traceData)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "The stale data was: " + staleData);
                    }
                    if (string.IsNullOrEmpty(staleData))
                    {
                        closeStream = false;
                    }
                }

                if (closeStream)
                {
                    m_stream.Close();
                }
            }
            return staleData;
        }

#if !NET40
        /// <summary>
        /// Data shouldn't be on the socket, if it is it probably means we've been disconnected.
        /// Read and discard whatever is there and optionally close the connection.
        /// Returns the stale data as text, if any, or null if none was found.
        /// </summary>
        /// <param name="closeStream">close the connection?</param>
        /// <param name="evenEncrypted">even read encrypted data?</param>
        /// <param name="traceData">trace data to logs?</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        private async Task<string> ReadStaleDataAsync(bool closeStream, bool evenEncrypted, bool traceData, CancellationToken token)
        {
            string staleData = null;
            if (m_stream != null && m_stream.SocketDataAvailable > 0)
            {
                if (traceData)
                {
                    LogStatus(FtpTraceLevel.Info, "There is stale data on the socket, maybe our connection timed out or you did not call GetReply(). Re-connecting...");
                }

                if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted))
                {
                    var buf = new byte[m_stream.SocketDataAvailable];
                    await m_stream.RawSocketReadAsync(buf, token);
                    staleData = Encoding.GetString(buf).TrimEnd('\r', '\n');
                    if (traceData)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "The stale data was: " + staleData);
                    }
                }

                if (closeStream)
                {
                    m_stream.Close();
                }
            }
            return staleData;
        }
#endif

        /// <summary>
        /// Checks if this FTP/FTPS connection is made through a proxy.
        /// </summary>
        public bool IsProxy()
        {
            return this is FtpClientProxy;
        }

        /// <summary>
        /// Returns true if the file passes all the rules
        /// </summary>
        private bool FilePassesRules(FtpResult result, List<FtpRule> rules, bool useLocalPath, FtpListItem item = null)
        {
            if (rules != null && rules.Count > 0)
            {
                var passes = FtpRule.IsAllAllowed(rules, item ?? result.ToListItem(useLocalPath));
                if (!passes)
                {

                    LogStatus(FtpTraceLevel.Info, "Skipped file due to rule: " + (useLocalPath ? result.LocalPath : result.RemotePath));

                    // mark that the file was skipped due to a rule
                    result.IsSkipped = true;
                    result.IsSkippedByRule = true;

                    // skip uploading the file
                    return false;
                }
            }
            return true;
        }
        #endregion // FtpClient_Utils.cs
    }
    #region // Proxy
    /// <summary> 
    /// A FTP client with a user@host proxy identification, that works with Blue Coat FTP Service servers.
    /// 
    /// The 'blue coat variant' forces the client to wait for a 220 FTP response code in 
    /// the handshake phase.
    /// </summary>
    public class FtpClientBlueCoatProxy : FtpClientProxy
    {
        /// <summary> A FTP client with a user@host proxy identification. </summary>
        /// <param name="proxy">Proxy information</param>
        public FtpClientBlueCoatProxy(ProxyInfo proxy)
            : base(proxy)
        {
            ConnectionType = "User@Host";
        }

        /// <summary>
        /// Creates a new instance of this class. Useful in FTP proxy classes.
        /// </summary>
        protected override FtpClient Create()
        {
            return new FtpClientBlueCoatProxy(Proxy);
        }

        /// <summary> Redefine the first dialog: auth with proxy information </summary>
        protected override void Handshake()
        {
            // Proxy authentication eventually needed.
            if (Proxy.Credentials != null)
            {
                Authenticate(Proxy.Credentials.UserName, Proxy.Credentials.Password, Proxy.Credentials.Domain);
            }

            // Connection USER@Host means to change user name to add host.
            Credentials.UserName = Credentials.UserName + "@" + Host + ":" + Port;

            var reply = GetReply();
            if (reply.Code == "220")
            {
                LogLine(FtpTraceLevel.Info, "Status: Server is ready for the new client");
            }

            // TO TEST: if we are able to detect the actual FTP server software from this reply
            HandshakeReply = reply;
        }
    }
    /// <summary> A FTP client with a HTTP 1.1 proxy implementation. </summary>
    public class FtpClientHttp11Proxy : FtpClientProxy
    {
        /// <summary> A FTP client with a HTTP 1.1 proxy implementation </summary>
        /// <param name="proxy">Proxy information</param>
        public FtpClientHttp11Proxy(ProxyInfo proxy)
            : base(proxy)
        {
            ConnectionType = "HTTP 1.1 Proxy";
        }

        /// <summary> Redefine the first dialog: HTTP Frame for the HTTP 1.1 Proxy </summary>
        protected override void Handshake()
        {
            var proxyConnectionReply = GetReply();
            if (!proxyConnectionReply.Success)
            {
                throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " +
                                       proxyConnectionReply.ErrorMessage);
            }

            // TO TEST: if we are able to detect the actual FTP server software from this reply
            HandshakeReply = proxyConnectionReply;
        }

        /// <summary>
        /// Creates a new instance of this class. Useful in FTP proxy classes.
        /// </summary>
        protected override FtpClient Create()
        {
            return new FtpClientHttp11Proxy(Proxy);
        }

        /// <summary>
        /// Connects to the server using an existing <see cref="FtpSocketStream"/>
        /// </summary>
        /// <param name="stream">The existing socket stream</param>
        protected override void Connect(FtpSocketStream stream)
        {
            Connect(stream, Host, Port, FtpIpVersion.ANY);
        }

#if !NET40
        /// <summary>
        /// Connects to the server using an existing <see cref="FtpSocketStream"/>
        /// </summary>
        /// <param name="stream">The existing socket stream</param>
        /// <param name="token"></param>
        protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token)
        {
            return ConnectAsync(stream, Host, Port, FtpIpVersion.ANY, token);
        }
#endif

        /// <summary>
        /// Connects to the server using an existing <see cref="FtpSocketStream"/>
        /// </summary>
        /// <param name="stream">The existing socket stream</param>
        /// <param name="host">Host name</param>
        /// <param name="port">Port number</param>
        /// <param name="ipVersions">IP version to use</param>
        protected override void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
        {
            base.Connect(stream);

            var writer = new StreamWriter(stream);
            writer.WriteLine("CONNECT {0}:{1} HTTP/1.1", host, port);
            writer.WriteLine("Host: {0}:{1}", host, port);
            if (Proxy.Credentials != null)
            {
                var credentialsHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" + Proxy.Credentials.Password));
                writer.WriteLine("Proxy-Authorization: Basic " + credentialsHash);
            }

            writer.WriteLine("User-Agent: custom-ftp-client");
            writer.WriteLine();
            writer.Flush();

            ProxyHandshake(stream);
        }

#if !NET40
        /// <summary>
        /// Connects to the server using an existing <see cref="FtpSocketStream"/>
        /// </summary>
        /// <param name="stream">The existing socket stream</param>
        /// <param name="host">Host name</param>
        /// <param name="port">Port number</param>
        /// <param name="ipVersions">IP version to use</param>
        /// <param name="token">IP version to use</param>
        protected override async Task ConnectAsync(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions, CancellationToken token)
        {
            await base.ConnectAsync(stream, token);

            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(string.Format("CONNECT {0}:{1} HTTP/1.1", host, port));
            await writer.WriteLineAsync(string.Format("Host: {0}:{1}", host, port));
            if (Proxy.Credentials != null)
            {
                var credentialsHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" + Proxy.Credentials.Password));
                await writer.WriteLineAsync("Proxy-Authorization: Basic " + credentialsHash);
            }

            await writer.WriteLineAsync("User-Agent: custom-ftp-client");
            await writer.WriteLineAsync();
            await writer.FlushAsync();

            await ProxyHandshakeAsync(stream, token);
        }
#endif

        private void ProxyHandshake(FtpSocketStream stream)
        {
            var proxyConnectionReply = GetProxyReply(stream);
            if (!proxyConnectionReply.Success)
            {
                throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " + proxyConnectionReply.ErrorMessage);
            }
        }

#if !NET40
        private async Task ProxyHandshakeAsync(FtpSocketStream stream, CancellationToken token = default(CancellationToken))
        {
            var proxyConnectionReply = await GetProxyReplyAsync(stream, token);
            if (!proxyConnectionReply.Success)
            {
                throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " + proxyConnectionReply.ErrorMessage);
            }
        }
#endif

        private FtpReply GetProxyReply(FtpSocketStream stream)
        {
            var reply = new FtpReply();
            string buf;
            lock (Lock)
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("No connection to the server has been established.");
                }

                stream.ReadTimeout = ReadTimeout;
                while ((buf = stream.ReadLine(Encoding)) != null)
                {
                    Match m;

                    LogLine(FtpTraceLevel.Info, buf);

                    if ((m = Regex.Match(buf, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                    {
                        reply.Code = m.Groups["code"].Value;
                        reply.Message = m.Groups["message"].Value;
                        break;
                    }

                    reply.InfoMessages += buf + "\n";
                }

                // fixes #84 (missing bytes when downloading/uploading files through proxy)
                while ((buf = stream.ReadLine(Encoding)) != null)
                {
                    LogLine(FtpTraceLevel.Info, buf);

                    if (Strings.IsNullOrWhiteSpace(buf))
                    {
                        break;
                    }

                    reply.InfoMessages += buf + "\n";
                }
            }
            return reply;
        }

#if !NET40
        private async Task<FtpReply> GetProxyReplyAsync(FtpSocketStream stream, CancellationToken token = default(CancellationToken))
        {
            var reply = new FtpReply();
            string buf;

            if (!IsConnected)
            {
                throw new InvalidOperationException("No connection to the server has been established.");
            }

            stream.ReadTimeout = ReadTimeout;
            while ((buf = await stream.ReadLineAsync(Encoding, token)) != null)
            {
                Match m;

                LogLine(FtpTraceLevel.Info, buf);

                if ((m = Regex.Match(buf, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                {
                    reply.Code = m.Groups["code"].Value;
                    reply.Message = m.Groups["message"].Value;
                    break;
                }

                reply.InfoMessages += buf + "\n";
            }

            // fixes #84 (missing bytes when downloading/uploading files through proxy)
            while ((buf = await stream.ReadLineAsync(Encoding, token)) != null)
            {
                LogLine(FtpTraceLevel.Info, buf);

                if (Strings.IsNullOrWhiteSpace(buf))
                {
                    break;
                }

                reply.InfoMessages += buf + "\n";
            }

            return reply;
        }

#endif
    }
    /// <summary>
    /// Abstraction of an FtpClient with a proxy
    /// </summary>
    public abstract class FtpClientProxy : FtpClient
    {
        private ProxyInfo _proxy;

        /// <summary> The proxy connection info. </summary>
        protected ProxyInfo Proxy => _proxy;

        /// <summary> A FTP client with a HTTP 1.1 proxy implementation </summary>
        /// <param name="proxy">Proxy information</param>
        protected FtpClientProxy(ProxyInfo proxy)
        {
            _proxy = proxy;
        }

        /// <summary> Redefine connect for FtpClient : authentication on the Proxy  </summary>
        /// <param name="stream">The socket stream.</param>
        protected override void Connect(FtpSocketStream stream)
        {
            stream.Connect(Proxy.Host, Proxy.Port, InternetProtocolVersions);
        }

#if !NET40
        /// <summary> Redefine connect for FtpClient : authentication on the Proxy  </summary>
        /// <param name="stream">The socket stream.</param>
        /// <param name="token">Cancellation token.</param>
        protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token)
        {
            return stream.ConnectAsync(Proxy.Host, Proxy.Port, InternetProtocolVersions, token);
        }

#endif
    }
    /// <summary> A FTP client with a SOCKS5 proxy implementation. </summary>
    public class FtpClientSocks5Proxy : FtpClientProxy
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="proxy"></param>
        public FtpClientSocks5Proxy(ProxyInfo proxy) : base(proxy)
        {
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="stream"></param>
        protected override void Connect(FtpSocketStream stream)
        {
            base.Connect(stream);
            var proxy = new SocksProxy(Host, Port, stream);
            proxy.Negotiate();
            proxy.Authenticate();
            proxy.Connect();
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="ipVersions"></param>
        protected override void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
        {
            base.Connect(stream);
            var proxy = new SocksProxy(Host, port, stream);
            proxy.Negotiate();
            proxy.Authenticate();
            proxy.Connect();
        }

#if !NET40
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ConnectAsync(FtpSocketStream stream, CancellationToken cancellationToken)
        {
            await base.ConnectAsync(stream, cancellationToken);
            var proxy = new SocksProxy(Host, Port, stream);
            await proxy.NegotiateAsync();
            await proxy.AuthenticateAsync();
            await proxy.ConnectAsync();
        }
#endif
    }
    /// <summary> A FTP client with a user@host proxy identification. </summary>
    public class FtpClientUserAtHostProxy : FtpClientProxy
    {
        /// <summary> A FTP client with a user@host proxy identification. </summary>
        /// <param name="proxy">Proxy information</param>
        public FtpClientUserAtHostProxy(ProxyInfo proxy)
            : base(proxy)
        {
            ConnectionType = "User@Host";
        }

        /// <summary>
        /// Creates a new instance of this class. Useful in FTP proxy classes.
        /// </summary>
        protected override FtpClient Create()
        {
            return new FtpClientUserAtHostProxy(Proxy);
        }

        /// <summary> Redefine the first dialog: auth with proxy information </summary>
        protected override void Handshake()
        {
            // Proxy authentication eventually needed.
            if (Proxy.Credentials != null)
            {
                Authenticate(Proxy.Credentials.UserName, Proxy.Credentials.Password, Proxy.Credentials.Domain);
            }

            // Connection USER@Host means to change user name to add host.
            Credentials.UserName = Credentials.UserName + "@" + Host + ":" + Port;
        }
    }
    /// <summary> POCO holding proxy information</summary>
    public class ProxyInfo
    {
        /// <summary> Proxy host name </summary>
        public string Host { get; set; }

        /// <summary> Proxy port </summary>
        public int Port { get; set; }

        /// <summary> Proxy login credentials </summary>
        public NetworkCredential Credentials { get; set; }
    }
    /// <summary>
    ///     This class is not reusable.
    ///     You have to create a new instance for each connection / attempt.
    /// </summary>
    public class SocksProxy
    {
        private readonly byte[] _buffer;
        private readonly string _destinationHost;
        private readonly int _destinationPort;
        private readonly FtpSocketStream _socketStream;
        private SocksAuthType? _authType;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="destinationHost"></param>
        /// <param name="destinationPort"></param>
        /// <param name="socketStream"></param>
        public SocksProxy(string destinationHost, int destinationPort, FtpSocketStream socketStream)
        {
            _buffer = new byte[512];
            _destinationHost = destinationHost;
            _destinationPort = destinationPort;
            _socketStream = socketStream;
        }
        /// <summary>
        /// 越过
        /// </summary>
        /// <exception cref="SocksProxyException"></exception>
        public void Negotiate()
        {
            // The client connects to the server,
            // and sends a version identifier / method selection message.
            var methodsBuffer = new byte[]
            {
                (byte)SocksVersion.Five, // VER
				0x01, // NMETHODS
				(byte)SocksAuthType.NoAuthRequired // Methods
			};

            _socketStream.Write(methodsBuffer, 0, methodsBuffer.Length);

            // The server selects from one of the methods given in METHODS,
            // and sends a METHOD selection message:
            var receivedBytes = _socketStream.Read(_buffer, 0, 2);
            if (receivedBytes != 2)
            {
                _socketStream.Close();
                throw new SocksProxyException($"Negotiation Response had an invalid length of {receivedBytes}");
            }

            _authType = (SocksAuthType)_buffer[1];
        }
        /// <summary>
        /// 认证
        /// </summary>
        public void Authenticate()
        {
            AuthenticateInternal();
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <exception cref="SocksProxyException"></exception>
        public void Connect()
        {
            var requestBuffer = GetConnectRequest();
            _socketStream.Write(requestBuffer, 0, requestBuffer.Length);

            SocksReply reply;

            // The server evaluates the request, and returns a reply.
            // - First we read VER, REP, RSV & ATYP
            var received = _socketStream.Read(_buffer, 0, 4);
            if (received != 4)
            {
                if (received >= 2)
                {
                    reply = (SocksReply)_buffer[1];
                    HandleProxyCommandError(reply);
                }

                _socketStream.Close();
                throw new SocksProxyException($"Connect Reply has Invalid Length {received}. Expecting 4.");
            }

            // - Now we check if the reply was positive.
            reply = (SocksReply)_buffer[1];

            if (reply != SocksReply.Succeeded)
            {
                HandleProxyCommandError(reply);
            }

            // - Consume rest of the SOCKS5 protocol so the next read will give application data.
            var atyp = (SocksRequestAddressType)_buffer[3];
            int atypSize;
            int read;

            switch (atyp)
            {
                case SocksRequestAddressType.IPv4:
                    atypSize = 6;
                    read = _socketStream.Read(_buffer, 0, atypSize);
                    break;
                case SocksRequestAddressType.IPv6:
                    atypSize = 18;
                    read = _socketStream.Read(_buffer, 0, atypSize);
                    break;
                case SocksRequestAddressType.FQDN:
                    atypSize = 1;
                    _socketStream.Read(_buffer, 0, atypSize);
                    atypSize = _buffer[0] + 2;
                    read = _socketStream.Read(_buffer, 0, atypSize);
                    break;
                default:
                    _socketStream.Close();
                    throw new SocksProxyException("Unknown Socks Request Address Type", new ArgumentOutOfRangeException());
            }

            if (read != atypSize)
            {
                _socketStream.Close();
                throw new SocksProxyException($"Unexpected Response size from Request Type Data. Expected {atypSize} received {read}");
            }
        }


        private void AuthenticateInternal()
        {
            if (!_authType.HasValue)
            {
                _socketStream.Close();
                throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.", new ArgumentException("No SOCKS5 auth method has been set."));
            }

            // The client and server then enter a method-specific sub-negotiation.
            switch (_authType.Value)
            {
                case SocksAuthType.NoAuthRequired:
                    break;

                case SocksAuthType.GSSAPI:
                    _socketStream.Close();
                    throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.", new NotSupportedException("GSSAPI is not implemented."));

                case SocksAuthType.UsernamePassword:
                    _socketStream.Close();
                    throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
                        new NotSupportedException("UsernamePassword is not implemented."));

                // If the selected METHOD is X'FF', none of the methods listed by the
                // client are acceptable, and the client MUST close the connection
                case SocksAuthType.NoAcceptableMethods:
                    _socketStream.Close();
                    throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
                        new MissingMethodException("METHOD is X'FF' No Client requested methods are acceptable. Closing the connection."));

                default:
                    _socketStream.Close();
                    throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
                        new ArgumentOutOfRangeException());
            }
        }

        private byte[] GetConnectRequest()
        {
            // Once the method-dependent sub negotiation has completed,
            // the client sends the request details.
            bool issHostname = !IPAddress.TryParse(_destinationHost, out var ip);

            var dstAddress = issHostname
                ? Encoding.ASCII.GetBytes(_destinationHost)
                : ip.GetAddressBytes();

            var requestBuffer = issHostname
                ? new byte[7 + dstAddress.Length]
                : new byte[6 + dstAddress.Length];

            requestBuffer[0] = (byte)SocksVersion.Five;
            requestBuffer[1] = (byte)SocksRequestCommand.Connect;

            if (issHostname)
            {
                requestBuffer[3] = (byte)SocksRequestAddressType.FQDN;
                requestBuffer[4] = (byte)dstAddress.Length;

                for (var i = 0; i < dstAddress.Length; i++)
                {
                    requestBuffer[5 + i] = dstAddress[i];
                }

                requestBuffer[5 + dstAddress.Length] = (byte)(_destinationPort >> 8);
                requestBuffer[6 + dstAddress.Length] = (byte)_destinationPort;
            }
            else
            {
                requestBuffer[3] = dstAddress.Length == 4
                    ? (byte)SocksRequestAddressType.IPv4
                    : (byte)SocksRequestAddressType.IPv6;

                for (var i = 0; i < dstAddress.Length; i++)
                {
                    requestBuffer[4 + i] = dstAddress[i];
                }

                requestBuffer[4 + dstAddress.Length] = (byte)(_destinationPort >> 8);
                requestBuffer[5 + dstAddress.Length] = (byte)_destinationPort;
            }

            return requestBuffer;
        }

#if !NET40
        /// <summary>
        /// 异步越过
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocksProxyException"></exception>
        public async Task NegotiateAsync()
        {
            // The client connects to the server,
            // and sends a version identifier / method selection message.
            var methodsBuffer = new byte[]
            {
                (byte)SocksVersion.Five, // VER
				0x01, // NMETHODS
				(byte)SocksAuthType.NoAuthRequired // Methods
			};

            await _socketStream.WriteAsync(methodsBuffer, 0, methodsBuffer.Length);

            // The server selects from one of the methods given in METHODS,
            // and sends a METHOD selection message:
            var receivedBytes = await _socketStream.ReadAsync(_buffer, 0, 2);
            if (receivedBytes != 2)
            {
                _socketStream.Close();
                throw new SocksProxyException($"Negotiation Response had an invalid length of {receivedBytes}");
            }

            _authType = (SocksAuthType)_buffer[1];
        }
        /// <summary>
        /// 异步认证
        /// </summary>
        /// <returns></returns>
        public Task AuthenticateAsync()
        {
            AuthenticateInternal();
            return Task.FromResult(0);
        }
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocksProxyException"></exception>
        public async Task ConnectAsync()
        {
            var requestBuffer = GetConnectRequest();
            await _socketStream.WriteAsync(requestBuffer, 0, requestBuffer.Length);

            SocksReply reply;

            // The server evaluates the request, and returns a reply.
            // - First we read VER, REP, RSV & ATYP
            var received = await _socketStream.ReadAsync(_buffer, 0, 4);
            if (received != 4)
            {
                if (received >= 2)
                {
                    reply = (SocksReply)_buffer[1];
                    HandleProxyCommandError(reply);
                }

                _socketStream.Close();
                throw new SocksProxyException($"Connect Reply has Invalid Length {received}. Expecting 4.");
            }

            // - Now we check if the reply was positive.
            reply = (SocksReply)_buffer[1];

            if (reply != SocksReply.Succeeded)
            {
                HandleProxyCommandError(reply);
            }

            // - Consume rest of the SOCKS5 protocol so the next read will give application data.
            var atyp = (SocksRequestAddressType)_buffer[3];
            int atypSize;
            int read;

            switch (atyp)
            {
                case SocksRequestAddressType.IPv4:
                    atypSize = 6;
                    read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
                    break;
                case SocksRequestAddressType.IPv6:
                    atypSize = 18;
                    read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
                    break;
                case SocksRequestAddressType.FQDN:
                    atypSize = 1;
                    await _socketStream.ReadAsync(_buffer, 0, atypSize);

                    atypSize = _buffer[0] + 2;
                    read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
                    break;
                default:
                    _socketStream.Close();
                    throw new SocksProxyException("Unknown Socks Request Address Type", new ArgumentOutOfRangeException());
            }

            if (read != atypSize)
            {
                _socketStream.Close();
                throw new SocksProxyException($"Unexpected Response size from Request Type Data. Expected {atypSize} received {read}");
            }
        }
#endif
        private void HandleProxyCommandError(SocksReply replyCode)
        {
            string proxyErrorText;
            switch (replyCode)
            {
                case SocksReply.GeneralSOCKSServerFailure:
                    proxyErrorText = "a general socks destination failure occurred";
                    break;
                case SocksReply.NotAllowedByRuleset:
                    proxyErrorText = "the connection is not allowed by proxy destination rule set";
                    break;
                case SocksReply.NetworkUnreachable:
                    proxyErrorText = "the network was unreachable";
                    break;
                case SocksReply.HostUnreachable:
                    proxyErrorText = "the host was unreachable";
                    break;
                case SocksReply.ConnectionRefused:
                    proxyErrorText = "the connection was refused by the remote network";
                    break;
                case SocksReply.TTLExpired:
                    proxyErrorText = "the time to live (TTL) has expired";
                    break;
                case SocksReply.CommandNotSupported:
                    proxyErrorText = "the command issued by the proxy client is not supported by the proxy destination";
                    break;
                case SocksReply.AddressTypeNotSupported:
                    proxyErrorText = "the address type specified is not supported";
                    break;
                default:
                    proxyErrorText = $"an unknown SOCKS reply with the code value '{replyCode}' was received";
                    break;
            }

            _socketStream.Close();
            throw new SocksProxyException($"Proxy error: {proxyErrorText} for destination host {_destinationHost} port number {_destinationPort}.");
        }
    }
    #endregion // Socks
}
