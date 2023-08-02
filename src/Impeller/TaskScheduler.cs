using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Impeller
{
#pragma warning disable CS0108 // 成员隐藏继承的成员；缺少关键字 new
#pragma warning disable CS0618 // 类型或成员已过时
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct _SYSTEMTIME
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort wYear;
        /// <summary>
        /// 
        /// </summary>
        public ushort wMonth;
        /// <summary>
        /// 
        /// </summary>
        public ushort wDayOfWeek;
        /// <summary>
        /// 
        /// </summary>
        public ushort wDay;
        /// <summary>
        /// 
        /// </summary>
        public ushort wHour;
        /// <summary>
        /// 
        /// </summary>
        public ushort wMinute;
        /// <summary>
        /// 
        /// </summary>
        public ushort wSecond;
        /// <summary>
        /// 
        /// </summary>
        public ushort wMilliseconds;
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_ACTION_TYPE
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_ACTION_EXEC,
        /// <summary>
        /// 
        /// </summary>
        TASK_ACTION_COM_HANDLER = 5,
        /// <summary>
        /// 
        /// </summary>
        TASK_ACTION_SEND_EMAIL,
        /// <summary>
        /// 
        /// </summary>
        TASK_ACTION_SHOW_MESSAGE
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_COMPATIBILITY
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_AT,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V1,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V2,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V2_1,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V2_2,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V2_3,
        /// <summary>
        /// 
        /// </summary>
        TASK_COMPATIBILITY_V2_4
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_CREATION
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_VALIDATE_ONLY = 1,
        /// <summary>
        /// 
        /// </summary>
        TASK_CREATE,
        /// <summary>
        /// 
        /// </summary>
        TASK_UPDATE = 4,
        /// <summary>
        /// 
        /// </summary>
        TASK_CREATE_OR_UPDATE = 6,
        /// <summary>
        /// 
        /// </summary>
        TASK_DISABLE = 8,
        /// <summary>
        /// 
        /// </summary>
        TASK_DONT_ADD_PRINCIPAL_ACE = 16,
        /// <summary>
        /// 
        /// </summary>
        TASK_IGNORE_REGISTRATION_TRIGGERS = 32
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_ENUM_FLAGS
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_ENUM_HIDDEN = 1
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_INSTANCES_POLICY
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_INSTANCES_PARALLEL,
        /// <summary>
        /// 
        /// </summary>
        TASK_INSTANCES_QUEUE,
        /// <summary>
        /// 
        /// </summary>
        TASK_INSTANCES_IGNORE_NEW,
        /// <summary>
        /// 
        /// </summary>
        TASK_INSTANCES_STOP_EXISTING
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_LOGON_TYPE
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_NONE,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_PASSWORD,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_S4U,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_INTERACTIVE_TOKEN,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_GROUP,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_SERVICE_ACCOUNT,
        /// <summary>
        /// 
        /// </summary>
        TASK_LOGON_INTERACTIVE_TOKEN_OR_PASSWORD
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_PROCESSTOKENSID
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_PROCESSTOKENSID_NONE,
        /// <summary>
        /// 
        /// </summary>
        TASK_PROCESSTOKENSID_UNRESTRICTED,
        /// <summary>
        /// 
        /// </summary>
        TASK_PROCESSTOKENSID_DEFAULT
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_RUN_FLAGS
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_RUN_NO_FLAGS,
        /// <summary>
        /// 
        /// </summary>
        TASK_RUN_AS_SELF,
        /// <summary>
        /// 
        /// </summary>
        TASK_RUN_IGNORE_CONSTRAINTS,
        /// <summary>
        /// 
        /// </summary>
        TASK_RUN_USE_SESSION_ID = 4,
        /// <summary>
        /// 
        /// </summary>
        TASK_RUN_USER_SID = 8
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_RUNLEVEL
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_RUNLEVEL_LUA,
        /// <summary>
        /// 
        /// </summary>
        TASK_RUNLEVEL_HIGHEST
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_SESSION_STATE_CHANGE_TYPE
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_CONSOLE_CONNECT = 1,
        /// <summary>
        /// 
        /// </summary>
        TASK_CONSOLE_DISCONNECT,
        /// <summary>
        /// 
        /// </summary>
        TASK_REMOTE_CONNECT,
        /// <summary>
        /// 
        /// </summary>
        TASK_REMOTE_DISCONNECT,
        /// <summary>
        /// 
        /// </summary>
        TASK_SESSION_LOCK = 7,
        /// <summary>
        /// 
        /// </summary>
        TASK_SESSION_UNLOCK
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_STATE
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_STATE_UNKNOWN,
        /// <summary>
        /// 
        /// </summary>
        TASK_STATE_DISABLED,
        /// <summary>
        /// 
        /// </summary>
        TASK_STATE_QUEUED,
        /// <summary>
        /// 
        /// </summary>
        TASK_STATE_READY,
        /// <summary>
        /// 
        /// </summary>
        TASK_STATE_RUNNING
    }
    /// <summary>
    /// 
    /// </summary>
    public enum _TASK_TRIGGER_TYPE2
    {
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_EVENT,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_TIME,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_DAILY,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_WEEKLY,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_MONTHLY,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_MONTHLYDOW,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_IDLE,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_REGISTRATION,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_BOOT,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_LOGON,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_SESSION_STATE_CHANGE = 11,
        /// <summary>
        /// 
        /// </summary>
        TASK_TRIGGER_CUSTOM_TRIGGER_01
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("BAE54997-48B1-4CBE-9965-D6BE263EBEA4")]
    [ComImport]
    public interface IAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("02820E19-7B98-4ED2-B2E8-FDCCCEFF619B")]
    [ComImport]
    public interface IActionCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        int Count { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        IAction this[[In] int index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string XmlText { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IAction Create([In] _TASK_ACTION_TYPE Type);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([MarshalAs(UnmanagedType.Struct)][In] object index);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string Context { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("2A9C35DA-D357-41F4-BBC1-207AC1B1F3CB")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IBootTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string Delay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("6D2FD252-75C5-4F66-90BA-2A7D8CC3039F")]
    [ComImport]
    public interface IComHandlerAction : IAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string ClassId { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string Data { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("126C5CD8-B288-41D5-8DBF-E491446ADC5C")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IDailyTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(25)]
        short DaysInterval { [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string RandomDelay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("10F62C64-7E16-4314-A0C2-0C3683F99D40")]
    [ComImport]
    public interface IEmailAction : IAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string Server { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string Subject { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        string To { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        string Cc { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(14)]
        string Bcc { [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(15)]
        string ReplyTo { [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(16)]
        string From { [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(17)]
        ITaskNamedValueCollection HeaderFields { [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(18)]
        string Body { [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(19)]
        Array Attachments { [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] get; [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("D45B0167-9653-4EEF-B94F-0732CA7AF251")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IEventTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string Subscription { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(21)]
        string Delay { [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(22)]
        ITaskNamedValueCollection ValueQueries { [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("4C3D624D-FD6B-49A3-B9B7-09CB3CD3F047")]
    [ComImport]
    public interface IExecAction : IAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string Path { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string Arguments { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        string WorkingDirectory { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("F2A82542-BDA5-4E6B-9143-E2BF4F8987B6")]
    [ComImport]
    public interface IExecAction2 : IExecAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string Path { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string Arguments { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        string WorkingDirectory { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        bool HideAppWindow { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("84594461-0053-4342-A8FD-088FABF11F32")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IIdleSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string IdleDuration { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string WaitTimeout { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        bool StopOnIdleEnd { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        bool RestartOnIdle { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("D537D2B0-9FB3-4D34-9739-1FF5CE7B1EF3")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IIdleTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("72DADE38-FAE4-4B3E-BAF4-5D009AF02B1C")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ILogonTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string Delay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(21)]
        string UserId { [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("A6024FA8-9652-4ADB-A6BF-5CFCD877A7BA")]
    [ComImport]
    public interface IMaintenanceSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(34)]
        string Period { [DispId(34)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(34)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(35)]
        string Deadline { [DispId(35)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(35)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(36)]
        bool Exclusive { [DispId(36)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(36)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("77D025A3-90FA-43AA-B52E-CDA5499B946A")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IMonthlyDOWTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(25)]
        short DaysOfWeek { [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(26)]
        short WeeksOfMonth { [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(27)]
        short MonthsOfYear { [DispId(27)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(27)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(28)]
        bool RunOnLastWeekOfMonth { [DispId(28)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(28)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string RandomDelay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("97C45EF1-6B02-4A1A-9C0E-1EBFBA1500AC")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IMonthlyTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(25)]
        int DaysOfMonth { [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(26)]
        short MonthsOfYear { [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(27)]
        bool RunOnLastDayOfMonth { [DispId(27)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(27)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string RandomDelay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("9F7DEA84-C30B-4245-80B6-00E9F646F1B4")]
    [ComImport]
    public interface INetworkSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Name { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("D98D51E5-C9B4-496A-A9C1-18980261CF0F")]
    [ComImport]
    public interface IPrincipal
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string DisplayName { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        string UserId { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        _TASK_LOGON_TYPE LogonType { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string GroupId { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        _TASK_RUNLEVEL RunLevel { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("248919AE-E345-4A6D-8AEB-E0D3165C904E")]
    [ComImport]
    public interface IPrincipal2
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        _TASK_PROCESSTOKENSID ProcessTokenSidType { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        int RequiredPrivilegeCount { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        string RequiredPrivilege { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void AddRequiredPrivilege([MarshalAs(UnmanagedType.BStr)][In] string privilege);
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("9C86F320-DEE3-4DD1-B972-A303F26B061E")]
    [ComConversionLoss]
    [TypeLibType(4288)]
    [DefaultMember("Path")]
    [ComImport]
    public interface IRegisteredTask
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Name { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("Path")]
        string Path { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_STATE State { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        bool Enabled { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRunningTask Run([MarshalAs(UnmanagedType.Struct)][In] object @params);

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRunningTask RunEx([MarshalAs(UnmanagedType.Struct)][In] object @params, [In] int flags, [In] int sessionID, [MarshalAs(UnmanagedType.BStr)][In] string user);

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRunningTaskCollection GetInstances([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        DateTime LastRunTime { [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(9)]
        int LastTaskResult { [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        int NumberOfMissedRuns { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        DateTime NextRunTime { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        ITaskDefinition Definition { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(14)]
        string Xml { [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(15)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetSecurityDescriptor([In] int securityInformation);

        /// <summary>
        /// 
        /// </summary>
        [DispId(16)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void SetSecurityDescriptor([MarshalAs(UnmanagedType.BStr)][In] string sddl, [In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(17)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Stop([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(1610743825)]
        [TypeLibFunc(65)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void GetRunTimes([In] ref _SYSTEMTIME pstStart, [In] ref _SYSTEMTIME pstEnd, [In][Out] ref uint pCount, [Out] IntPtr pRunTimes);
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("86627EB4-42A7-41E4-A4D9-AC33A72F2D52")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IRegisteredTaskCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1610743808)]
        int Count { [DispId(1610743808)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        IRegisteredTask this[[MarshalAs(UnmanagedType.Struct)][In] object index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("416D8B73-CB41-4EA1-805C-9BE9A5AC4A74")]
    [ComImport]
    public interface IRegistrationInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Description { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Author { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string Version { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string Date { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string Documentation { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(9)]
        string XmlText { [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string URI { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        object SecurityDescriptor { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Struct)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Struct)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        string Source { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("4C8FEC3A-C218-4E0C-B23D-629024DB91A2")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IRegistrationTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string Delay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("7FB9ACF1-26BE-400E-85B5-294B9C75DFD6")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IRepetitionPattern
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Interval { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Duration { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        bool StopAtDurationEnd { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [DefaultMember("InstanceGuid")]
    [Guid("653758FB-7B9A-4F1E-A471-BEEB8E9B834E")]
    [ComImport]
    public interface IRunningTask
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Name { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("InstanceGuid")]
        string InstanceGuid { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Path { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        _TASK_STATE State { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string CurrentAction { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Stop();

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Refresh();

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        uint EnginePID { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("6A67614B-6828-4FEC-AA54-6D52E8F1F2DB")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IRunningTaskCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        int Count { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        IRunningTask this[[MarshalAs(UnmanagedType.Struct)][In] object index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("754DA71B-4385-4475-9DD9-598294FA3641")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ISessionStateChangeTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string Delay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(21)]
        string UserId { [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(22)]
        _TASK_SESSION_STATE_CHANGE_TYPE StateChange { [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("505E9E68-AF89-46B8-A30F-56162A83D537")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IShowMessageAction : IAction
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Id { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        _TASK_ACTION_TYPE Type { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        string Title { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string MessageBody { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("F5BC8FC5-536D-4F77-B852-FBC1356FDEB6")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ITaskDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        IRegistrationInfo RegistrationInfo { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        ITriggerCollection Triggers { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        ITaskSettings Settings { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string Data { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        IPrincipal Principal { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        IActionCollection Actions { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(14)]
        string XmlText { [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DefaultMember("Path")]
    [TypeLibType(4288)]
    [Guid("8CFAC062-A080-4C15-9A88-AA7C2AF80DFC")]
    [ComImport]
    public interface ITaskFolder
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Name { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("Path")]
        string Path { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskFolder GetFolder([MarshalAs(UnmanagedType.BStr)] string Path);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskFolderCollection GetFolders([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskFolder CreateFolder([MarshalAs(UnmanagedType.BStr)][In] string subFolderName, [MarshalAs(UnmanagedType.Struct)][In][Optional] object sddl);

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void DeleteFolder([MarshalAs(UnmanagedType.BStr)] string subFolderName, [In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRegisteredTask GetTask([MarshalAs(UnmanagedType.BStr)][In] string Path);

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRegisteredTaskCollection GetTasks([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void DeleteTask([MarshalAs(UnmanagedType.BStr)][In] string Name, [In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRegisteredTask RegisterTask([MarshalAs(UnmanagedType.BStr)][In] string Path, [MarshalAs(UnmanagedType.BStr)][In] string XmlText, [In] int flags, [MarshalAs(UnmanagedType.Struct)][In] object UserId, [MarshalAs(UnmanagedType.Struct)][In] object password, [In] _TASK_LOGON_TYPE LogonType, [MarshalAs(UnmanagedType.Struct)][In][Optional] object sddl);

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRegisteredTask RegisterTaskDefinition([MarshalAs(UnmanagedType.BStr)][In] string Path, [MarshalAs(UnmanagedType.Interface)][In] ITaskDefinition pDefinition, [In] int flags, [MarshalAs(UnmanagedType.Struct)][In] object UserId, [MarshalAs(UnmanagedType.Struct)][In] object password, [In] _TASK_LOGON_TYPE LogonType, [MarshalAs(UnmanagedType.Struct)][In][Optional] object sddl);

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetSecurityDescriptor(int securityInformation);

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void SetSecurityDescriptor([MarshalAs(UnmanagedType.BStr)][In] string sddl, [In] int flags);
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("79184A66-8664-423F-97F1-637356A5D812")]
    [ComImport]
    public interface ITaskFolderCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1610743808)]
        int Count { [DispId(1610743808)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        ITaskFolder this[[MarshalAs(UnmanagedType.Struct)][In] object index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("839D7762-5121-4009-9234-4F0D19394F04")]
    [InterfaceType(1)]
    [ComImport]
    public interface ITaskHandler
    {
        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Start([MarshalAs(UnmanagedType.IUnknown)][In] object pHandlerServices, [MarshalAs(UnmanagedType.BStr)][In] string Data);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Stop([MarshalAs(UnmanagedType.Error)] out int pRetCode);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Pause();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Resume();
    }
    /// <summary>
    /// 
    /// </summary>
    [InterfaceType(1)]
    [Guid("EAEC7A8F-27A0-4DDC-8675-14726A01A38A")]
    [ComImport]
    public interface ITaskHandlerStatus
    {
        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void UpdateStatus([In] short percentComplete, [MarshalAs(UnmanagedType.BStr)][In] string statusMessage);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void TaskCompleted([MarshalAs(UnmanagedType.Error)][In] int taskErrCode);
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("B4EF826B-63C3-46E4-A504-EF69E4F7EA4D")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ITaskNamedValueCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        int Count { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        ITaskNamedValuePair this[[In] int index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskNamedValuePair Create([MarshalAs(UnmanagedType.BStr)][In] string Name, [MarshalAs(UnmanagedType.BStr)][In] string Value);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([In] int index);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Clear();
    }
    /// <summary>
    /// 
    /// </summary>
    [DefaultMember("Name")]
    [Guid("39038068-2B46-4AFD-8662-7BB6F868D221")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ITaskNamedValuePair
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("Name")]
        string Name { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        string Value { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [DefaultMember("TargetServer")]
    [Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85")]
    [ComImport]
    public interface ITaskService
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskFolder GetFolder([MarshalAs(UnmanagedType.BStr)][In] string Path);

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRunningTaskCollection GetRunningTasks([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITaskDefinition NewTask([In] uint flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Connect([MarshalAs(UnmanagedType.Struct)][In][Optional] object serverName, [MarshalAs(UnmanagedType.Struct)][In][Optional] object user, [MarshalAs(UnmanagedType.Struct)][In][Optional] object domain, [MarshalAs(UnmanagedType.Struct)][In][Optional] object password);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        bool Connected { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("TargetServer")]
        string TargetServer { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string ConnectedUser { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        string ConnectedDomain { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        uint HighestVersion { [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("8FD4711D-2D02-4C8C-87E3-EFF699DE127E")]
    [ComImport]
    public interface ITaskSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        bool AllowDemandStart { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string RestartInterval { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        int RestartCount { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        _TASK_INSTANCES_POLICY MultipleInstances { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool StopIfGoingOnBatteries { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        bool DisallowStartIfOnBatteries { [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(9)]
        bool AllowHardTerminate { [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        bool StartWhenAvailable { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string XmlText { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        bool RunOnlyIfNetworkAvailable { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        string ExecutionTimeLimit { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(14)]
        bool Enabled { [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(15)]
        string DeleteExpiredTaskAfter { [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(16)]
        int Priority { [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(17)]
        _TASK_COMPATIBILITY Compatibility { [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(18)]
        bool Hidden { [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(19)]
        IIdleSettings IdleSettings { [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        bool RunOnlyIfIdle { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(21)]
        bool WakeToRun { [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(22)]
        INetworkSettings NetworkSettings { [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("2C05C3F0-6EED-4C05-A15F-ED7D7A98A369")]
    [TypeLibType(4288)]
    [ComImport]
    public interface ITaskSettings2
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(30)]
        bool DisallowStartOnRemoteAppSession { [DispId(30)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(30)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(31)]
        bool UseUnifiedSchedulingEngine { [DispId(31)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(31)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("0AD9D0D7-0C7F-4EBB-9A5F-D1C648DCA528")]
    [ComImport]
    public interface ITaskSettings3 : ITaskSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        bool AllowDemandStart { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string RestartInterval { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        int RestartCount { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        _TASK_INSTANCES_POLICY MultipleInstances { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool StopIfGoingOnBatteries { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        bool DisallowStartIfOnBatteries { [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(9)]
        bool AllowHardTerminate { [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(9)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(10)]
        bool StartWhenAvailable { [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(10)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(11)]
        string XmlText { [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(11)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(12)]
        bool RunOnlyIfNetworkAvailable { [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(12)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(13)]
        string ExecutionTimeLimit { [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(13)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(14)]
        bool Enabled { [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(14)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(15)]
        string DeleteExpiredTaskAfter { [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(15)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(16)]
        int Priority { [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(16)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(17)]
        _TASK_COMPATIBILITY Compatibility { [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(17)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(18)]
        bool Hidden { [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(18)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(19)]
        IIdleSettings IdleSettings { [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(19)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        bool RunOnlyIfIdle { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(21)]
        bool WakeToRun { [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(21)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(22)]
        INetworkSettings NetworkSettings { [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(22)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(30)]
        bool DisallowStartOnRemoteAppSession { [DispId(30)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(30)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(31)]
        bool UseUnifiedSchedulingEngine { [DispId(31)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(31)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(40)]
        IMaintenanceSettings MaintenanceSettings { [DispId(40)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(40)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(41)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IMaintenanceSettings CreateMaintenanceSettings();

        /// <summary>
        /// 
        /// </summary>
        [DispId(42)]
        bool Volatile { [DispId(42)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(42)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("3E4C9351-D966-4B8B-BB87-CEBA68BB0107")]
    [InterfaceType(1)]
    [ComImport]
    public interface ITaskVariables
    {
        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInput();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        void SetOutput([MarshalAs(UnmanagedType.BStr)][In] string input);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetContext();
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("B45747E0-EBA7-4276-9F29-85C5BB300006")]
    [ComImport]
    public interface ITimeTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string RandomDelay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("09941815-EA89-4B5B-89E0-2A773801FAC3")]
    [ComImport]
    public interface ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [TypeLibType(4288)]
    [Guid("85DF5081-1B24-4F32-878A-D9D14DF4CB77")]
    [ComImport]
    public interface ITriggerCollection : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        int Count { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        ITrigger this[[In] int index]
        {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        IEnumerator GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITrigger Create([In] _TASK_TRIGGER_TYPE2 Type);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([MarshalAs(UnmanagedType.Struct)][In] object index);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Clear();
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("5038FC98-82FF-436D-8728-A512A57C9DC1")]
    [TypeLibType(4288)]
    [ComImport]
    public interface IWeeklyTrigger : ITrigger
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        _TASK_TRIGGER_TYPE2 Type { [DispId(1)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        string Id { [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        IRepetitionPattern Repetition { [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.Interface)] get; [DispId(3)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.Interface)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        string ExecutionTimeLimit { [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        string StartBoundary { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        string EndBoundary { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        bool Enabled { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(25)]
        short DaysOfWeek { [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(25)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(26)]
        short WeeksInterval { [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)] get; [DispId(26)][MethodImpl(MethodImplOptions.InternalCall)][param: In] set; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(20)]
        string RandomDelay { [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; [DispId(20)][MethodImpl(MethodImplOptions.InternalCall)][param: MarshalAs(UnmanagedType.BStr)][param: In] set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("839D7762-5121-4009-9234-4F0D19394F04")]
    [CoClass(typeof(TaskHandlerPSClass))]
    [ComImport]
    public interface TaskHandlerPS : ITaskHandler
    {
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("F2A69DB7-DA2C-4352-9066-86FEE6DACAC9")]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(2)]
    [ComImport]
    public class TaskHandlerPSClass : ITaskHandler, TaskHandlerPS
    {
        ////// <summary>
        ////// 
        ////// </summary>
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern TaskHandlerPSClass();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Start([MarshalAs(UnmanagedType.IUnknown)][In] object pHandlerServices, [MarshalAs(UnmanagedType.BStr)][In] string Data);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Stop([MarshalAs(UnmanagedType.Error)] out int pRetCode);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Pause();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Resume();
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("EAEC7A8F-27A0-4DDC-8675-14726A01A38A")]
    [CoClass(typeof(TaskHandlerStatusPSClass))]
    [ComImport]
    public interface TaskHandlerStatusPS : ITaskHandlerStatus
    {
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("9F15266D-D7BA-48F0-93C1-E6895F6FE5AC")]
    [TypeLibType(2)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    public class TaskHandlerStatusPSClass : ITaskHandlerStatus, TaskHandlerStatusPS, ITaskVariables
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern TaskHandlerStatusPSClass();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void UpdateStatus([In] short percentComplete, [MarshalAs(UnmanagedType.BStr)][In] string statusMessage);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void TaskCompleted([MarshalAs(UnmanagedType.Error)][In] int taskErrCode);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern string GetInput();

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void SetOutput([MarshalAs(UnmanagedType.BStr)][In] string input);

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern string GetContext();
    }
    /// <summary>
    /// 
    /// </summary>
    [Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85")]
    [CoClass(typeof(TaskSchedulerClass))]
    [ComImport]
    public interface TaskScheduler : ITaskService
    {
    }
    /// <summary>
    /// 
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [DefaultMember("TargetServer")]
    [Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")]
    [TypeLibType(2)]
    [ComImport]
    public class TaskSchedulerClass : ITaskService, TaskScheduler
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern TaskSchedulerClass();

        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ITaskFolder GetFolder([MarshalAs(UnmanagedType.BStr)][In] string Path);

        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern IRunningTaskCollection GetRunningTasks([In] int flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ITaskDefinition NewTask([In] uint flags);

        /// <summary>
        /// 
        /// </summary>
        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Connect([MarshalAs(UnmanagedType.Struct)][In][Optional] object serverName, [MarshalAs(UnmanagedType.Struct)][In][Optional] object user, [MarshalAs(UnmanagedType.Struct)][In][Optional] object domain, [MarshalAs(UnmanagedType.Struct)][In][Optional] object password);

        /// <summary>
        /// 
        /// </summary>
        [DispId(5)]
        public virtual extern bool Connected { [DispId(5)][MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        //[IndexerName("TargetServer")]
        public virtual extern string TargetServer { [DispId(0)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(6)]
        public virtual extern string ConnectedUser { [DispId(6)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(7)]
        public virtual extern string ConnectedDomain { [DispId(7)][MethodImpl(MethodImplOptions.InternalCall)][return: MarshalAs(UnmanagedType.BStr)] get; }

        /// <summary>
        /// 
        /// </summary>
        [DispId(8)]
        public virtual extern uint HighestVersion { [DispId(8)][MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
#pragma warning restore CS0618 // 类型或成员已过时
#pragma warning restore CS0108 // 成员隐藏继承的成员；缺少关键字 new
}
