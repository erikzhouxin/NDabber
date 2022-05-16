using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace System.Data.Piper
{
    /// <summary>
    /// 管道调用方法
    /// </summary>
    public static partial class PiperCaller
    {
        /// <summary>
        /// 创建一个命名管道实例
        /// 所有默认参数来源于构造
        /// <see cref="NamedPipeServerStream"/>
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="direction"></param>
        /// <param name="maxNumberOfServerInstances"></param>
        /// <param name="transmissionMode"></param>
        /// <param name="options"></param>
        /// <param name="inBufferSize"></param>
        /// <param name="outBufferSize"></param>
        /// <param name="pipeSecurity"></param>
        /// <param name="inheritability"></param>
        /// <param name="additionalAccessRights"></param>
        /// <returns></returns>
        [SecurityCritical]
        public static NamedPipeServerStream CreateNamedPipe(string pipeName,
            PipeDirection direction = PipeDirection.InOut,
            int maxNumberOfServerInstances = 1,
            PipeTransmissionMode transmissionMode = PipeTransmissionMode.Byte,
            PipeOptions options = PipeOptions.None,
            int inBufferSize = 0,
            int outBufferSize = 0,
            PipeSecurity pipeSecurity = null,
            HandleInheritability inheritability = HandleInheritability.None,
            PipeAccessRights additionalAccessRights = 0)
        {
            if (string.IsNullOrWhiteSpace(pipeName)) { throw new ArgumentNullException(nameof(pipeName)); }
            if ((options & ~(PipeOptions.WriteThrough | PipeOptions.Asynchronous)) != PipeOptions.None)
            { throw new ArgumentOutOfRangeException(nameof(options), "ArgumentOutOfRange_OptionsInvalid"); }
            if (inBufferSize < 0)
            { throw new ArgumentOutOfRangeException(nameof(inBufferSize), "ArgumentOutOfRange_NeedNonNegNum"); }
            if ((maxNumberOfServerInstances < 1 || maxNumberOfServerInstances > 254) && maxNumberOfServerInstances != -1)
            { throw new ArgumentOutOfRangeException(nameof(maxNumberOfServerInstances), "ArgumentOutOfRange_MaxNumServerInstances"); }
            if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
            { throw new ArgumentOutOfRangeException(nameof(inheritability), "ArgumentOutOfRange_HandleInheritabilityNoneOrInheritable"); }
            if ((additionalAccessRights & ~(PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.AccessSystemSecurity)) != (PipeAccessRights)0)
            { throw new ArgumentOutOfRangeException(nameof(additionalAccessRights), "ArgumentOutOfRange_AdditionalAccessLimited"); }
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
            { throw new PlatformNotSupportedException("PlatformNotSupported_NamedPipeServers"); }
            string fullPath = Path.GetFullPath("\\\\.\\pipe\\" + pipeName);
            if (string.Compare(fullPath, "\\\\.\\pipe\\anonymous", StringComparison.OrdinalIgnoreCase) == 0)
            { throw new ArgumentOutOfRangeException(nameof(pipeName), "ArgumentOutOfRange_AnonymousReserved"); }
            object pinningHandle = (object)null;
            SECURITY_ATTRIBUTES secAttrs = GetSecAttrs(inheritability, pipeSecurity, out pinningHandle);
            try
            {
                int openMode = (int)((PipeOptions)(direction | (maxNumberOfServerInstances == 1 ? (PipeDirection)524288 : (PipeDirection)0)) | options | (PipeOptions)additionalAccessRights);
                int pipeMode = (int)transmissionMode << 2 | (int)transmissionMode << 1;
                if (maxNumberOfServerInstances == -1) { maxNumberOfServerInstances = (int)byte.MaxValue; }
#pragma warning disable CA2000 // Dispose objects before losing scope
                SafePipeHandle namedPipe = CreateNamedPipe(fullPath, openMode, pipeMode, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, secAttrs);
#pragma warning restore CA2000 // Dispose objects before losing scope
                if (namedPipe.IsInvalid)
                { WinIOError(Marshal.GetLastWin32Error(), string.Empty); }
                return new NamedPipeServerStream(direction, (uint)(options & PipeOptions.Asynchronous) > 0U, false, namedPipe);
            }
            finally
            {
                if (pinningHandle != null)
                { ((GCHandle)pinningHandle).Free(); }
            }
        }
        /// <summary>
        /// 接收名称
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="Callback"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static NamedPipeServerStream ReceiveNamedWithSecurity(string pipeName, Action<PipeCommunication.ModelString> Callback, int size = 65535)
        {
            PipeSecurity pipeSecurity = new PipeSecurity();

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                // Allow the Administrators group full access to the pipe.
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)),
                    PipeAccessRights.FullControl, AccessControlType.Allow));
            }
            else
            {
                // Allow AuthenticatedUser read and write access to the pipe.
                pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }
            var accessRights = PipeAccessRights.FullControl | PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance;
            pipeSecurity.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            NamedPipeServerStream pipeServer = CreateNamedPipe(pipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 1024, pipeSecurity);
            //PipeSecurity pse = namedPipeServerStream.GetAccessControl();
            //pse.SetAccessRule(new PipeAccessRule("Administrators", accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule("Users", accessRights, AccessControlType.Allow));//设置访问规则Pipe
            //pse.SetAccessRule(new PipeAccessRule("CREATOR OWNER", accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule("SYSTEM", accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), accessRights, AccessControlType.Allow));
            //pse.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), accessRights, AccessControlType.Allow));
            //namedPipeServerStream.SetAccessControl(pse);

            pipeServer.BeginWaitForConnection(delegate (IAsyncResult ar)
            {
                NamedPipeServerStream namedPipeServerStream2 = (NamedPipeServerStream)ar.AsyncState;
                namedPipeServerStream2.EndWaitForConnection(ar);
                byte[] array = new byte[size];
                int num = namedPipeServerStream2.Read(array, 0, size);
                if (num > 0)
                {
                    Callback(Encoding.UTF8.GetString(array, 0, num).GetJsonObject<PipeCommunication.ModelString>());
                }
                namedPipeServerStream2.Dispose();
                ReceiveNamedWithSecurity(pipeName, Callback, size);
            }, pipeServer);
            return pipeServer;
        }
        static Dictionary<string, int> _piperServerDic = new Dictionary<string, int>();
        static object _piperServerLocker = new object();
        static List<Thread> _piperServerThreads = new List<Thread>();
        /// <summary>
        /// 接收消息内容
        /// 消息内容最长65535
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static void ReceiveNamedWidthDefaultSecurity(string pipeName, Func<AlertPipeString, AlertPipeResult> Callback)
        {
            PipeSecurity pipeSecurity = new PipeSecurity();
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                // Allow the Administrators group full access to the pipe.
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)),
                    PipeAccessRights.FullControl, AccessControlType.Allow));
            }
            else
            {
                // Allow AuthenticatedUser read and write access to the pipe.
                pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }
            var accessRights = PipeAccessRights.FullControl | PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance;
            pipeSecurity.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)), accessRights, AccessControlType.Allow));
            if (Monitor.TryEnter(_piperServerLocker, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    var thread = new Thread(() =>
                    {
                        var pipeServer = CreateNamedPipe(pipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 65535, 65535, pipeSecurity);
                        byte[] array = new byte[65536];
                        while (true)
                        {
                            pipeServer.WaitForConnection();
                            int num = pipeServer.Read(array, 0, array.Length);
                            if (num > 0)
                            {
                                try
                                {
                                    var receiveJson = Encoding.UTF8.GetString(array, 0, num);
                                    var obj = receiveJson.GetJsonObject<AlertPipeString>();
                                    if (obj != null)
                                    {
                                        var res = Callback(obj);
                                        var bytes = res.GetJsonString().GetBytes(Encoding.UTF8);
                                        pipeServer.Write(bytes, 0, bytes.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exKey = UserPassword.GetMd5Hash(ex.Message, ex.StackTrace);
                                    CacheModel.Dictionary.GetOrAdd($"EZhouXin:Exception:{exKey}", () => true, null, new DateTimeOffset(DateTime.Now.AddHours(1)));
                                }
                            }
                            pipeServer.Disconnect();
                        }
                    });
                    thread.IsBackground = true;
                    thread.Start();
                    _piperServerThreads.Add(thread);
                }
                finally
                {
                    Monitor.Exit(_piperServerLocker);
                }
            }
        }
        /// <summary>
        /// 接收消息内容
        /// 消息内容最长65535
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static void ReceiveNamedWidthDefaultSecurity(string pipeName, Func<string, string, AlertPipeResult> Callback)
        {
            ReceiveNamedWidthDefaultSecurity(pipeName, (model) => PipeCommunication.Analysis(model, Callback));
        }
        #region // 内部方法或定义
        [SecurityCritical]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern SafePipeHandle CreateNamedPipe(string pipeName, int openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, SECURITY_ATTRIBUTES securityAttributes);
        [SecurityCritical]
        internal static unsafe SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability, PipeSecurity pipeSecurity, out object pinningHandle)
        {
            pinningHandle = (object)null;
            SECURITY_ATTRIBUTES securityAttributes = (SECURITY_ATTRIBUTES)null;
            if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None || pipeSecurity != null)
            {
                securityAttributes = new SECURITY_ATTRIBUTES();
                securityAttributes.nLength = Marshal.SizeOf((object)securityAttributes);
                if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None)
                    securityAttributes.bInheritHandle = 1;
                if (pipeSecurity != null)
                {
                    byte[] descriptorBinaryForm = pipeSecurity.GetSecurityDescriptorBinaryForm();
                    pinningHandle = (object)GCHandle.Alloc((object)descriptorBinaryForm, GCHandleType.Pinned);
                    fixed (byte* numPtr = descriptorBinaryForm)
                        securityAttributes.pSecurityDescriptor = numPtr;
                }
            }
            return securityAttributes;
        }
        [SecurityCritical]
        internal static void WinIOError(int errorCode, string maybeFullPath)
        {
            bool isInvalidPath = errorCode == 123 || errorCode == 161;
            string displayablePath = GetDisplayablePath(maybeFullPath, isInvalidPath);
            switch (errorCode)
            {
                case 2:
                    if (displayablePath.Length == 0) { throw new FileNotFoundException("IO_FileNotFound"); }
                    throw new FileNotFoundException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "IO_FileNotFound_FileName", displayablePath), displayablePath);
                case 3:
                    if (displayablePath.Length == 0) { throw new DirectoryNotFoundException("IO_PathNotFound_NoPathName"); }
                    throw new DirectoryNotFoundException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "IO_PathNotFound_Path", displayablePath));
                case 5:
                    if (displayablePath.Length == 0) { throw new UnauthorizedAccessException("UnauthorizedAccess_IODenied_NoPathName"); }
                    throw new UnauthorizedAccessException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "UnauthorizedAccess_IODenied_Path", displayablePath));
                case 15:
                    throw new DriveNotFoundException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "IO_DriveNotFound_Drive", displayablePath));
                case 32:
                    if (displayablePath.Length == 0) { throw new IOException("IO_IO_SharingViolation_NoFileName", MakeHRFromErrorCode(errorCode)); }
                    throw new IOException(String.Format("IO_IO_SharingViolation_File", displayablePath), MakeHRFromErrorCode(errorCode));
                case 80:
                    if (displayablePath.Length != 0)
                        throw new IOException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "IO_IO_FileExists_Name", displayablePath), MakeHRFromErrorCode(errorCode));
                    break;
                case 87:
                    throw new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));
                case 183:
                    if (displayablePath.Length != 0)
                        throw new IOException(String.Format("IO_IO_AlreadyExists_Name", (object)displayablePath), MakeHRFromErrorCode(errorCode));
                    break;
                case 206:
                    throw new PathTooLongException("IO_PathTooLong");
                case 995:
                    throw new OperationCanceledException();
            }
            throw new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));
        }
        [SecuritySafeCritical]
        internal static string GetDisplayablePath(string path, bool isInvalidPath)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            bool flag1 = false;
            if (path.Length < 2) { return path; }
            if ((int)path[0] == (int)Path.DirectorySeparatorChar && (int)path[1] == (int)Path.DirectorySeparatorChar)
            { flag1 = true; }
            else if ((int)path[1] == (int)Path.VolumeSeparatorChar)
            { flag1 = true; }
            if (!flag1 && !isInvalidPath)
            { return path; }
            bool flag2 = false;
            try
            {
                if (!isInvalidPath)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[1]
                    {
                        path
                    }).Demand();
                    flag2 = true;
                }
            }
            catch (SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (NotSupportedException)
            {
            }
            if (!flag2)
            { path = (int)path[path.Length - 1] != (int)Path.DirectorySeparatorChar ? Path.GetFileName(path) : "IO_IO_NoPermissionToDirectoryName"; }
            return path;
        }
        internal static int MakeHRFromErrorCode(int errorCode)
        {
            return -2147024896 | errorCode;
        }
        [SecurityCritical]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr va_list_arguments);
        internal static readonly IntPtr NULL = IntPtr.Zero;
        [SecurityCritical]
        internal static string GetMessage(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(512);
            return FormatMessage(12800, NULL, errorCode, 0, lpBuffer, lpBuffer.Capacity, NULL) != 0 ? lpBuffer.ToString() : "UnknownError_Num " + (object)errorCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            internal int nLength;
            [SecurityCritical]
            internal unsafe byte* pSecurityDescriptor;
            internal int bInheritHandle;
        }
        #endregion
    }
    /// <summary>
    /// 管道通信
    /// </summary>
    public class PipeCommunication
    {
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed<T>(string pipeName, ModelString model) => SendClientNamed(".", pipeName, model);
        /// <summary>
        /// 命名管道指定机子
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <param name="model"></param>
        public static void SendClientNamed(string serverName, string pipeName, ModelString model)
        {
            try
            {
                using (var pipe = new NamedPipeClientStream(serverName, pipeName, PipeDirection.Out, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
                {
                    pipe.Connect();
                    byte[] data = Encoding.UTF8.GetBytes(model?.GetJsonString() ?? string.Empty);
                    pipe.Write(data, 0, data.Length);
                }
            }
            catch { }
        }
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed<T>(string pipeName, string cmd, T model = default) => SendClientNamed(".", pipeName, new ModelString(cmd, model?.GetJsonString()));
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static void SendNamed(string pipeName, string cmd, string content = "") => SendClientNamed(".", pipeName, new ModelString(cmd, content));
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static NamedPipeServerStream ReceiveNamed<T>(string pipeName, Action<ModelString> Callback, int size = 65535)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
#pragma warning restore CA1416 // 验证平台兼容性
            serverPipe.BeginWaitForConnection((ar) =>
            {
                var ps = (NamedPipeServerStream)ar.AsyncState;
                ps.EndWaitForConnection(ar);
                var data = new byte[size];
                var count = ps.Read(data, 0, size);
                if (count > 0)
                {
                    Callback(Encoding.UTF8.GetString(data, 0, count).GetJsonObject<ModelString>());
                }
                ps.Dispose();
                ReceiveNamed<T>(pipeName, Callback, size);
            }, serverPipe);
            return serverPipe;
        }
        /// <summary>
        /// 命名管道发送
        /// </summary>
        public static NamedPipeServerStream ReceiveNamed(string pipeName, Action<ModelString> Callback, int size = 65535)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
#pragma warning restore CA1416 // 验证平台兼容性
            serverPipe.BeginWaitForConnection((ar) =>
            {
                var ps = (NamedPipeServerStream)ar.AsyncState;
                ps.EndWaitForConnection(ar);
                var data = new byte[size];
                var count = ps.Read(data, 0, size);
                if (count > 0)
                {
                    Callback(Encoding.UTF8.GetString(data, 0, count).GetJsonObject<ModelString>());
                }
                ps.Dispose();
                ReceiveNamed(pipeName, Callback, size);
            }, serverPipe);
            return serverPipe;
        }
        /// <summary>
        /// 字符串数据
        /// </summary>
        public class ModelString
        {
            /// <summary>
            /// 构造
            /// </summary>
            public ModelString() { }
            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="cmd"></param>
            /// <param name="msg"></param>
            public ModelString(string cmd, string msg = "")
            {
                C = cmd;
                M = msg;
            }
            /// <summary>
            /// 命令
            /// </summary>
            public string C { get; set; }
            /// <summary>
            /// 模型内容
            /// </summary>
            public String M { get; set; }
        }
        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Regist"></param>
        /// <returns></returns>
        public static AlertPipeResult Analysis(AlertPipeString model, Func<string, string, AlertPipeResult> Regist)
        {
            if (string.IsNullOrWhiteSpace(model.C)) { return new AlertPipeResult(true, "发送命令成功"); }
            var args = model.C.Split(':');
            if (args.Length == 1) // 本地命令
            {
                var cmd = args[0];
                return Regist(cmd, model.M);
            }
            if (args.Length == 2)
            {
                using (var client = new NamedPipeClientStream(".", args[0], PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous))
                {
                    client.Connect();
                    byte[] bytes = Encoding.UTF8.GetBytes(new { C = args[1], M = model.M }.GetJsonString());
                    client.Write(bytes, 0, bytes.Length);
                }
                return new AlertPipeResult(true, "发送命令成功");
            }
            if (args.Length == 3)
            {
                using (var client = new NamedPipeClientStream(args[0], args[1], PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous))
                {
                    client.Connect();
                    byte[] bytes = Encoding.UTF8.GetBytes(new { C = args[2], M = model.M }.GetJsonString());
                    client.Write(bytes, 0, bytes.Length);
                }
                return new AlertPipeResult(true, "发送命令成功");
            }
            return AlertPipeResult.GetUnknown(model);
        }
    }
    /// <summary>
    /// 字符串数据
    /// </summary>
    public class AlertPipeString
    {
        /// <summary>
        /// 命令
        /// </summary>
        public virtual string C { get; set; }
        /// <summary>
        /// 模型内容
        /// </summary>
        public virtual String M { get; set; }
    }
    /// <summary>
    /// 管道提示结果
    /// </summary>
    public class AlertPipeResult : AlertPipeString
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int K { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool I { get; set; }
        /// <summary>
        /// 文本提示
        /// </summary>
        public dynamic D { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="alert"></param>
        public AlertPipeResult(IAlertMsg alert)
        {
            C = alert.Code.ToString();
            I = alert.IsSuccess;
            M = alert.Message;
            D = alert.Data;
            K = alert.Code;
        }
        /// <summary>
        /// 构造
        /// </summary>
        public AlertPipeResult() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertPipeResult(bool isSuccess, string message)
        {
            this.I = isSuccess;
            this.M = message;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public AlertPipeResult(bool isSuccess, string message, int code, object data)
        {
            this.I = isSuccess;
            this.M = message;
            this.C = code.ToString();
            this.D = data;
            this.K = code;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="alert"></param>
        public static implicit operator AlertPipeResult(AlertMsg alert)
        {
            return new AlertPipeResult(alert);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="alert"></param>
        public static implicit operator AlertMsg(AlertPipeResult alert)
        {
            return new AlertMsg(alert.I, alert.M) { Code = alert.K, Data = alert.D };
        }
        /// <summary>
        /// 获取未知命令结果
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static AlertPipeResult GetUnknown(AlertPipeString arg)
        {
            return new AlertPipeResult(false, "未知命令", 404, arg);
        }
        /// <summary>
        /// 获取未知命令结果
        /// </summary>
        /// <returns></returns>
        public static AlertPipeResult GetUnknown(string cmd, string msg)
        {
            return new AlertPipeResult(false, "未知命令", 404, new AlertPipeString() { C = cmd, M = msg });
        }
    }
}
