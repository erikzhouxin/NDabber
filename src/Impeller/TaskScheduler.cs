using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TaskScheduler;

namespace System.Data.Impeller
{
    public class SysTaskScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        [ComImport]
        //[ClassInterface(0)]
        [DefaultMember("TargetServer")]
        [Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD")]
        [TypeLibType(2)]
        public class TaskSchedulerClass : ITaskService, TaskScheduler
        {
            [DispId(5)]
            public virtual extern bool Connected
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(5)]
                get;
            }

            [DispId(0)]
            public virtual extern string TargetServer
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(6)]
            public virtual extern string ConnectedUser
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(6)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(7)]
            public virtual extern string ConnectedDomain
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(7)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(8)]
            public virtual extern uint HighestVersion
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(8)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern TaskSchedulerClass();

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [return: MarshalAs(UnmanagedType.Interface)]
            public virtual extern ITaskFolder GetFolder([In][MarshalAs(UnmanagedType.BStr)] string Path);

            ITaskFolder ITaskService.GetFolder([In][MarshalAs(UnmanagedType.BStr)] string Path)
            {
                //ILSpy generated this explicit interface implementation from .override directive in GetFolder
                return this.GetFolder(Path);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [return: MarshalAs(UnmanagedType.Interface)]
            public virtual extern IRunningTaskCollection GetRunningTasks([In] int flags);

            IRunningTaskCollection ITaskService.GetRunningTasks([In] int flags)
            {
                //ILSpy generated this explicit interface implementation from .override directive in GetRunningTasks
                return this.GetRunningTasks(flags);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.Interface)]
            public virtual extern ITaskDefinition NewTask([In] uint flags);

            ITaskDefinition ITaskService.NewTask([In] uint flags)
            {
                //ILSpy generated this explicit interface implementation from .override directive in NewTask
                return this.NewTask(flags);
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            public virtual extern void Connect([Optional][In][MarshalAs(UnmanagedType.Struct)] object serverName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object user, [Optional][In][MarshalAs(UnmanagedType.Struct)] object domain, [Optional][In][MarshalAs(UnmanagedType.Struct)] object password);

            void ITaskService.Connect([Optional][In][MarshalAs(UnmanagedType.Struct)] object serverName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object user, [Optional][In][MarshalAs(UnmanagedType.Struct)] object domain, [Optional][In][MarshalAs(UnmanagedType.Struct)] object password)
            {
                //ILSpy generated this explicit interface implementation from .override directive in Connect
                this.Connect(serverName, user, domain, password);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [ComImport]
        [TypeLibType(4288)]
        [DefaultMember("TargetServer")]
        [Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85")]
        public interface ITaskService
        {
            [DispId(5)]
            bool Connected
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(5)]
                get;
            }

            [DispId(0)]
            string TargetServer
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(6)]
            string ConnectedUser
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(6)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(7)]
            string ConnectedDomain
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(7)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(8)]
            uint HighestVersion
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(8)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [return: MarshalAs(UnmanagedType.Interface)]
            ITaskFolder GetFolder([In][MarshalAs(UnmanagedType.BStr)] string Path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IRunningTaskCollection GetRunningTasks([In] int flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.Interface)]
            ITaskDefinition NewTask([In] uint flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            void Connect([Optional][In][MarshalAs(UnmanagedType.Struct)] object serverName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object user, [Optional][In][MarshalAs(UnmanagedType.Struct)] object domain, [Optional][In][MarshalAs(UnmanagedType.Struct)] object password);
        }
        /// <summary>
        /// 
        /// </summary>
        [ComImport]
        [Guid("2FABA4C7-4DA9-4013-9697-20CC3FD40F85")]
        [CoClass(typeof(TaskSchedulerClass))]
        public interface TaskScheduler : ITaskService
        {
        }
        [ComImport]
        [Guid("6A67614B-6828-4FEC-AA54-6D52E8F1F2DB")]
        [TypeLibType(4288)]
        public interface IRunningTaskCollection : IEnumerable
        {
            [DispId(1)]
            int Count
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                get;
            }

            [DispId(0)]
            IRunningTask this[[In][MarshalAs(UnmanagedType.Struct)] object index]
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(-4)]
            [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
            new IEnumerator GetEnumerator();
        }
        [ComImport]
        [DefaultMember("Path")]
        [TypeLibType(4288)]
        [Guid("8CFAC062-A080-4C15-9A88-AA7C2AF80DFC")]
        public interface ITaskFolder
        {
            [DispId(1)]
            string Name
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(0)]
            string Path
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.Interface)]
            ITaskFolder GetFolder([MarshalAs(UnmanagedType.BStr)] string Path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [return: MarshalAs(UnmanagedType.Interface)]
            ITaskFolderCollection GetFolders([In] int flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            [return: MarshalAs(UnmanagedType.Interface)]
            ITaskFolder CreateFolder([In][MarshalAs(UnmanagedType.BStr)] string subFolderName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object sddl);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            void DeleteFolder([MarshalAs(UnmanagedType.BStr)] string subFolderName, [In] int flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IRegisteredTask GetTask([In][MarshalAs(UnmanagedType.BStr)] string Path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(8)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IRegisteredTaskCollection GetTasks([In] int flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(9)]
            void DeleteTask([In][MarshalAs(UnmanagedType.BStr)] string Name, [In] int flags);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IRegisteredTask RegisterTask([In][MarshalAs(UnmanagedType.BStr)] string Path, [In][MarshalAs(UnmanagedType.BStr)] string XmlText, [In] int flags, [In][MarshalAs(UnmanagedType.Struct)] object UserId, [In][MarshalAs(UnmanagedType.Struct)] object password, [In] _TASK_LOGON_TYPE LogonType, [Optional][In][MarshalAs(UnmanagedType.Struct)] object sddl);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(11)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IRegisteredTask RegisterTaskDefinition([In][MarshalAs(UnmanagedType.BStr)] string Path, [In][MarshalAs(UnmanagedType.Interface)] ITaskDefinition pDefinition, [In] int flags, [In][MarshalAs(UnmanagedType.Struct)] object UserId, [In][MarshalAs(UnmanagedType.Struct)] object password, [In] _TASK_LOGON_TYPE LogonType, [Optional][In][MarshalAs(UnmanagedType.Struct)] object sddl);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(12)]
            [return: MarshalAs(UnmanagedType.BStr)]
            string GetSecurityDescriptor(int securityInformation);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(13)]
            void SetSecurityDescriptor([In][MarshalAs(UnmanagedType.BStr)] string sddl, [In] int flags);
        }
        [ComImport]
        [TypeLibType(4288)]
        [DefaultMember("InstanceGuid")]
        [Guid("653758FB-7B9A-4F1E-A471-BEEB8E9B834E")]
        public interface IRunningTask
        {
            [DispId(1)]
            string Name
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(0)]
            string InstanceGuid
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(2)]
            string Path
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(3)]
            _TASK_STATE State
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(3)]
                get;
            }

            [DispId(4)]
            string CurrentAction
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(4)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            [DispId(7)]
            uint EnginePID
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(7)]
                get;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            void Stop();

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            void Refresh();
        }
        [ComImport]
        [Guid("F5BC8FC5-536D-4F77-B852-FBC1356FDEB6")]
        [TypeLibType(4288)]
        public interface ITaskDefinition
        {
            [DispId(1)]
            IRegistrationInfo RegistrationInfo
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [param: In]
                [param: MarshalAs(UnmanagedType.Interface)]
                set;
            }

            [DispId(2)]
            ITriggerCollection Triggers
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                [param: In]
                [param: MarshalAs(UnmanagedType.Interface)]
                set;
            }

            [DispId(7)]
            ITaskSettings Settings
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(7)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(7)]
                [param: In]
                [param: MarshalAs(UnmanagedType.Interface)]
                set;
            }

            [DispId(11)]
            string Data
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(11)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(11)]
                [param: In]
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            [DispId(12)]
            IPrincipal Principal
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(12)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(12)]
                [param: In]
                [param: MarshalAs(UnmanagedType.Interface)]
                set;
            }

            [DispId(13)]
            IActionCollection Actions
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(13)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(13)]
                [param: In]
                [param: MarshalAs(UnmanagedType.Interface)]
                set;
            }

            [DispId(14)]
            string XmlText
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(14)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(14)]
                [param: In]
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }
        }
        [ComImport]
        [TypeLibType(4288)]
        [Guid("02820E19-7B98-4ED2-B2E8-FDCCCEFF619B")]
        public interface IActionCollection : IEnumerable
        {
            [DispId(1)]
            int Count
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                get;
            }

            [DispId(0)]
            IAction this[[In] int index]
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(0)]
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
            }

            [DispId(2)]
            string XmlText
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                [param: In]
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            [DispId(6)]
            string Context
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(6)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(6)]
                [param: In]
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(-4)]
            [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
            new IEnumerator GetEnumerator();

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.Interface)]
            IAction Create([In] _TASK_ACTION_TYPE Type);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            void Clear();
        }
        [ComImport]
        [TypeLibType(4288)]
        [Guid("BAE54997-48B1-4CBE-9965-D6BE263EBEA4")]
        public interface IAction
        {
            [DispId(1)]
            string Id
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(1)]
                [param: In]
                [param: MarshalAs(UnmanagedType.BStr)]
                set;
            }

            [DispId(2)]
            _TASK_ACTION_TYPE Type
            {
                [MethodImpl(MethodImplOptions.InternalCall)]
                [DispId(2)]
                get;
            }
        }
        public enum _TASK_ACTION_TYPE
        {
            TASK_ACTION_EXEC = 0,
            TASK_ACTION_COM_HANDLER = 5,
            TASK_ACTION_SEND_EMAIL = 6,
            TASK_ACTION_SHOW_MESSAGE = 7
        }
    }
}
