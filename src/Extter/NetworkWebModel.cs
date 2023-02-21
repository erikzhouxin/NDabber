using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// FTP请求模型
    /// </summary>
    public class FtpRequestModel
    {
        /// <summary>
        /// FTP的IP地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 上传FTP目录
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 获取请求连接
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            if (!Host.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                return "ftp://" + Host.TrimEnd('/') + "/" + Path.TrimStart('/');
            }
            return Host.TrimEnd('/') + "/" + Path.TrimStart('/');
        }
    }
    /// <summary>
    /// FTP请求模型
    /// </summary>
    public class FtpRequestTypeModel : FtpRequestModel
    {
        /// <summary>
        /// 是否上传
        /// </summary>
        public ReqType Type { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public String FileName { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public enum ReqType
        {
            /// <summary>
            /// 下载
            /// </summary>
            Download = 0,
            /// <summary>
            /// 上传
            /// </summary>
            Upload = 1,
            /// <summary>
            /// 删除
            /// </summary>
            Delete = 2,
        }
        /// <summary>
        /// 获取一个请求
        /// </summary>
        /// <returns></returns>
        public IAlertMsg GetRequest()
        {
            switch (Type)
            {
                case FtpRequestTypeModel.ReqType.Upload: return UploadFile(FileName);
                case FtpRequestTypeModel.ReqType.Delete: return DeleteFile();
                case FtpRequestTypeModel.ReqType.Download:
                default: return DownloadFile(FileName);
            }
        }
        /// <summary>
        /// 上传
        /// </summary>
        public AlertMsg UploadFile(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            string uri = GetUrl().TrimEnd('/') + fileInfo.Name;
#pragma warning disable SYSLIB0014 // 类型或成员已过时
            var reqFTP = ExtterCaller.CreateWebRequest<FtpWebRequest>(uri);
#pragma warning restore SYSLIB0014 // 类型或成员已过时
            reqFTP.Credentials = new NetworkCredential(Account, Password);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInfo.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            using (FileStream fs = fileInfo.OpenRead())
            {
                using (Stream strm = reqFTP.GetRequestStream())
                {
                    try
                    {
                        contentLen = fs.Read(buff, 0, buffLength);
                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                        strm.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return new AlertMsg(false, ex.Message);
                    }
                    return AlertMsg.OperSuccess;
                }
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        public IAlertMsg DownloadFile(string saveFileName)
        {
            try
            {
                FileStream outputStream = new FileStream(saveFileName, FileMode.Create);
#pragma warning disable SYSLIB0014 // 类型或成员已过时
                var reqFTP = ExtterCaller.CreateWebRequest<FtpWebRequest>(GetUrl());
#pragma warning restore SYSLIB0014 // 类型或成员已过时
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Account, Password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, ex.Message);
            }
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public IAlertMsg DeleteFile()
        {
            try
            {
#pragma warning disable SYSLIB0014 // 类型或成员已过时
                var reqFTP = ExtterCaller.CreateWebRequest<FtpWebRequest>(GetUrl());
#pragma warning restore SYSLIB0014 // 类型或成员已过时
                reqFTP.Credentials = new NetworkCredential(Account, Password);
                reqFTP.KeepAlive = false;
                reqFTP.Method = nameof(WebRequestType.DELE);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                var result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AlertMsg(false, ex.Message);
            }
            return AlertMsg.OperSuccess;
        }
    }
    /// <summary>
    /// FTP请求类型
    /// </summary>
    public enum WebRequestType
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <see cref="WebRequestMethods.File.DownloadFile"/>
        /// <see cref="WebRequestMethods.Http.Get"/>
        GET = 0,
        /// <summary>
        /// 提交
        /// </summary>
        /// <see cref="WebRequestMethods.File.UploadFile"/>
        /// <see cref="WebRequestMethods.Http.Put"/>
        PUT = 1,
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.DownloadFile"/>
        RETR = 2,
        /// <summary>
        /// 列举目录
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.ListDirectory"/>
        NLST = 3,
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.UploadFile"/>
        STOR = 4,
        /// <summary>
        /// 删除
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.DeleteFile"/>
        DELE = 5,
        /// <summary>
        /// 附加文件
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.AppendFile"/>
        APPE = 6,
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.GetFileSize"/>
        SIZE = 7,
        /// <summary>
        /// 上传文件带着一个唯一名称
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.UploadFileWithUniqueName"/>
        STOU = 8,
        /// <summary>
        /// 创建文件目录
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.MakeDirectory"/>
        MKD = 9,
        /// <summary>
        /// 移除文件目录
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.RemoveDirectory"/>
        RMD = 10,
        /// <summary>
        /// 列举目录详情
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.ListDirectoryDetails"/>
        LIST = 11,
        /// <summary>
        /// 获取日期时间戳
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.GetDateTimestamp"/>
        MDTM = 12,
        /// <summary>
        /// 打印工作目录
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.PrintWorkingDirectory"/>
        PWD = 13,
        /// <summary>
        /// 重命名
        /// </summary>
        /// <see cref="WebRequestMethods.Ftp.Rename"/>
        RENAME = 14,
        /// <summary>
        /// 连接
        /// </summary>
        /// <see cref="WebRequestMethods.Http.Connect"/>
        CONNECT = 15,
        /// <summary>
        /// 头请求
        /// </summary>
        /// <see cref="WebRequestMethods.Http.Head"/>
        HEAD = 16,
        /// <summary>
        /// 推送
        /// </summary>
        /// <see cref="WebRequestMethods.Http.Post"/>
        POST = 17,
        /// <summary>
        /// 接下来，添加 mkcol() 方法，该方法在 Slide 储存库中创建一个集合 (collection ) —— 一个文件夹或目录。别忘了，WebDAV 规范是 HTTP 的一个扩展，因此 HTTP 响应代码将是类似的。mkcol() HTTP 请求返回一个响应代码，并附有一些文本作为解释。WebDAV 规范 (RFC 2518) 是这样描述这些代码的：
        /// 201 (Created)：集合或结构化资源是完整地创建的。
        /// 403 (Forbidden)：这个错误表明至少出现以下两种情况中的一种：1) 服务器不允许在其名称空间中的给定位置上创建集合，或者 2) Uniform Resource Indicator (URI) 请求的父集合存在，但是不接受成员。
        /// 405 (Method Not Allowed)： mkcol() 方法只能在被删除或不存在的资源上执行。
        /// 409 (Conflict)：只有在创建了一个或多个中间集合之后才能在被请求的 URI 上建立集合。
        /// 415 (Unsupported Media Type)：服务器不支持主体的请求类型。
        /// 507 (Insufficient Storage)：在执行该方法后资源没有足够的空间来记录资源的状态。
        /// </summary>
        /// <see cref="WebRequestMethods.Http.MkCol"/>
        MKCOL = 18,
    }
}
