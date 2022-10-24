using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace System.Data.Dibber
{

    #region // 公开类

    public class ColorTable : IList<long>, ICommiter
    {
        private ConsoleProperties owner;

        internal ColorTable(ConsoleProperties owner)
        {
            this.owner = owner;
        }

        #region IList<long> Members

        public int IndexOf(long item)
        {
            UInt32 value;
            checked
            {
                value = (UInt32)item;
            }
            for (int i = 0; i < owner.nt_console_props.ColorTable.Length; i++)
            {
                if (owner.nt_console_props.ColorTable[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, long item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public long this[int index]
        {
            get { return owner.nt_console_props.ColorTable[index]; }
            set
            {
                checked
                {
                    owner.nt_console_props.ColorTable[index] = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        #endregion

        #region ICollection<long> Members

        public void Add(long item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            for (int i = 0; i < owner.nt_console_props.ColorTable.Length; i++)
            {
                owner.nt_console_props.ColorTable[i] = 0;
            }
            this.Commit();
        }

        public bool Contains(long item)
        {
            return this.IndexOf(item) >= 0;
        }

        public void CopyTo(long[] array, int arrayIndex)
        {
            for (int i = 0; i < owner.nt_console_props.ColorTable.Length; i++)
            {
                array[i + arrayIndex] = owner.nt_console_props.ColorTable[i];
            }
        }

        public int Count
        {
            get { return owner.nt_console_props.ColorTable.Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(long item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<long> Members

        public IEnumerator<long> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICommiter Members

        public void Commit()
        {
            if (owner != null)
            {
                owner.Commit();
            }
        }

        #endregion
    }

    public class ConsoleProperties : ICommiter, ICloneable
    {
        private const int LF_FACESIZE = 32;
        internal NT_CONSOLE_PROPS nt_console_props;
        private ColorTable colorTable;
        private ShellLink owner;

        public ConsoleProperties()
        {
            nt_console_props = NT_CONSOLE_PROPS.AnEmptyOne();
            colorTable = new ColorTable(this);
        }

        ///<summary>
        ///  Makes a copy of another ConsoleProperty
        ///</summary>
        ///<remarks>
        ///  Note that the 'owner' field is not copied here.
        ///</remarks>
        public ConsoleProperties(ConsoleProperties another)
        {
            nt_console_props = another.nt_console_props;
            colorTable = new ColorTable(this);
        }

        /// <summary>
        ///   This should be only called by a ShellLink constructor
        /// </summary>
        /// <param name = "owner"></param>
        internal ConsoleProperties(ShellLink owner)
            : this()
        {
            this.owner = owner;
        }

        /// <summary>
        ///   Gets or sets the Fill attribute for the console.
        /// </summary>
        public int FillAttribute
        {
            get { return nt_console_props.wFillAttribute; }
            set
            {
                checked
                {
                    nt_console_props.wFillAttribute = (UInt16)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets Fill attribute for console popups.
        /// </summary>
        public int PopupFillAttribute
        {
            get { return nt_console_props.wPopupFillAttribute; }
            set
            {
                checked
                {
                    nt_console_props.wPopupFillAttribute = (UInt16)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   gets or sets the console's screen buffer size.  X is width, Y is height.
        /// </summary>
        public Coordinate ScreenBufferSize
        {
            get { return new Coordinate(nt_console_props.dwScreenBufferSize); }
            set
            {
                checked
                {
                    nt_console_props.dwScreenBufferSize = value.AsCOORD();
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   gets or sets the console's window size.  X is width, Y is height.
        /// </summary>
        public Coordinate WindowSize
        {
            get { return new Coordinate(nt_console_props.dwWindowSize); }
            set
            {
                checked
                {
                    nt_console_props.dwWindowSize = value.AsCOORD();
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   gets or sets the console's window origin.  X is left, Y is top.
        /// </summary>
        public Coordinate WindowOrigin
        {
            get { return new Coordinate(nt_console_props.dwWindowOrigin); }
            set
            {
                checked
                {
                    nt_console_props.dwWindowOrigin = value.AsCOORD();
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the font.
        /// </summary>
        public long Font
        {
            get
            {
                checked
                {
                    return (int)nt_console_props.nFont;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.nFont = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's input buffer size.
        /// </summary>
        public long InputBufferSize
        {
            get
            {
                checked
                {
                    return nt_console_props.nInputBufferSize;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.nInputBufferSize = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   gets or sets the console's font size.
        /// </summary>
        public Coordinate FontSize
        {
            get { return new Coordinate(nt_console_props.dwFontSize); }
            set
            {
                checked
                {
                    nt_console_props.dwFontSize = value.AsCOORD();
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's font family.
        /// </summary>
        public long FontFamily
        {
            get
            {
                checked
                {
                    return nt_console_props.uFontFamily;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.uFontFamily = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's font weight.
        /// </summary>
        public long FontWeight
        {
            get
            {
                checked
                {
                    return nt_console_props.uFontWeight;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.uFontWeight = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's font face name.
        /// </summary>
        public string FaceName
        {
            get
            {
                if (nt_console_props.FaceName[0] == '\0')
                {
                    return string.Empty;
                }
                int lastChar;
                for (lastChar = 1; lastChar < LF_FACESIZE; ++lastChar)
                {
                    if (nt_console_props.FaceName[lastChar] == '\0')
                    {
                        break;
                    }
                }

                var facename = new string(nt_console_props.FaceName, 0, lastChar);
                return facename;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    for (int i = 0; i < LF_FACESIZE; ++i)
                    {
                        nt_console_props.FaceName[i] = '\0';
                    }
                    this.Commit();
                    return;
                }
                if (value.Length > LF_FACESIZE)
                {
                    string msg = string.Format("The value is too long for the FaceName.  It must be {0} or less in length.", LF_FACESIZE);
                    throw new ArgumentException(msg);
                }

                {
                    int i;
                    for (i = 0; i < value.Length; ++i)
                    {
                        nt_console_props.FaceName[i] = value[i];
                    }
                    if (i < LF_FACESIZE)
                    {
                        nt_console_props.FaceName[i] = '\0';
                    }
                    this.Commit();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the console's cursor size.
        /// </summary>
        public long CursorSize
        {
            get
            {
                checked
                {
                    return nt_console_props.uCursorSize;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.uCursorSize = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's full screen flag.
        /// </summary>
        public bool FullScreen
        {
            get { return nt_console_props.bFullScreen; }
            set
            {
                nt_console_props.bFullScreen = value;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's quick edit flag.
        /// </summary>
        public bool QuickEdit
        {
            get { return nt_console_props.bQuickEdit; }
            set
            {
                nt_console_props.bQuickEdit = value;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's insert mode flag. True for insert mode, false for overrite
        /// </summary>
        public bool InsertMode
        {
            get { return nt_console_props.bInsertMode; }
            set
            {
                nt_console_props.bInsertMode = value;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's auto position flag. True to auto position the window.
        /// </summary>
        public bool AutoPosition
        {
            get { return nt_console_props.bAutoPosition; }
            set
            {
                nt_console_props.bAutoPosition = value;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the size of each console history buffer.
        /// </summary>
        public long HistoryBufferSize
        {
            get
            {
                checked
                {
                    return nt_console_props.uHistoryBufferSize;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.uHistoryBufferSize = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the number of history buffers for the console.
        /// </summary>
        public long NumberOfHistoryBuffers
        {
            get
            {
                checked
                {
                    return nt_console_props.uNumberOfHistoryBuffers;
                }
            }
            set
            {
                checked
                {
                    nt_console_props.uNumberOfHistoryBuffers = (UInt32)value;
                }
                ;
                this.Commit();
            }
        }

        /// <summary>
        ///   Gets or sets the console's histry no dupe flag. True if old duplicate history lists should be discarded, or false otherwise.
        public bool HistoryNoDup
        {
            get { return nt_console_props.bHistoryNoDup; }
            set
            {
                nt_console_props.bHistoryNoDup = value;
                this.Commit();
            }
        }

        /// <summary>
        ///   An array of color reference values for the console. Colors are specified as an index into this array.
        /// </summary>
        public ColorTable ColorTable
        {
            get { return this.colorTable; }
        }

        #region ICommiter Members

        public void Commit()
        {
            if (owner != null)
            {
                owner.WriteConsoleProperties();
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var clone = new ConsoleProperties(this);
            return clone;
        }

        #endregion
    }

    /// <summary>
    ///   A coordinate.  Values are limited to UInt16.MinValue and UInt16.MaxValue
    /// </summary>
    /// <remarks>
    ///   This structure maps to the native COORD data strcture.  The values used here are ints so
    ///   this class is CLS compliant.
    /// </remarks>
    public struct Coordinate : IEquatable<Coordinate>
    {
        private int x;
        private int y;

        /// <summary>
        ///   Create a coordinate with specified X and Y values.
        /// </summary>
        public Coordinate(int x, int y)
        {
            if (x > UInt16.MaxValue)
            {
                string msg = string.Format("must be <= {0}", UInt16.MaxValue);
                throw new ArgumentException(msg, "x");
            }
            if (x < UInt16.MinValue)
            {
                string msg = string.Format("must be >= {0}", UInt16.MinValue);
                throw new ArgumentException(msg, "x");
            }
            if (y > UInt16.MaxValue)
            {
                string msg = string.Format("must be <= {0}", UInt16.MaxValue);
                throw new ArgumentException(msg, "y");
            }
            if (y < UInt16.MinValue)
            {
                string msg = string.Format("must be >= {0}", UInt16.MinValue);
                throw new ArgumentException(msg, "y");
            }

            this.x = x;
            this.y = y;
        }

        /// <summary>
        ///   Make a copy of another Coordiante
        /// </summary>
        public Coordinate(Coordinate another)
        {
            x = another.X;
            y = another.Y;
        }

        internal Coordinate(COORD coord)
        {
            x = coord.X;
            y = coord.Y;
        }

        internal COORD AsCOORD()
        {
            unchecked
            {
                COORD coord;
                coord.X = (Int16)this.X;
                coord.Y = (Int16)this.Y;
                return coord;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate)
            {
                var other = (Coordinate)obj;
                return this.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.X.GetHashCode();
        }

        public override string ToString()
        {
            string xs = x.ToString();
            string ys = y.ToString();
            var sb = new StringBuilder(xs.Length + ys.Length + 1);
            sb.Append(xs);
            sb.Append(',');
            sb.Append(ys);
            return sb.ToString();
        }

        /// <summary>
        ///   Gets and sets the coordinates X value
        /// </summary>
        public int X
        {
            get { return x; }
            set
            {
                if (value > UInt16.MaxValue)
                {
                    string msg = string.Format("value for X must be <= {0}", UInt16.MaxValue);
                    throw new ArgumentException(msg);
                }
                if (value < UInt16.MinValue)
                {
                    string msg = string.Format("value for X must be >= {0}", UInt16.MinValue);
                    throw new ArgumentException(msg);
                }
                x = value;
            }
        }

        /// <summary>
        ///   Gets and sets the coordinates Y value
        /// </summary>
        public int Y
        {
            get { return y; }
            set
            {
                if (value > UInt16.MaxValue)
                {
                    string msg = string.Format("value for Y must be <= {0}", UInt16.MaxValue);
                    throw new ArgumentException(msg);
                }
                if (value < UInt16.MinValue)
                {
                    string msg = string.Format("value for Y must be >= {0}", UInt16.MinValue);
                    throw new ArgumentException(msg);
                }
                y = value;
            }
        }

        #region IEquatable<Coordinate> Members

        public bool Equals(Coordinate other)
        {
            if (this.X == other.X && this.Y == other.Y)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
    /// <summary>
    ///   Holds a reference to an icon in a file.  An icon locatoin object is 'empty' if its path is zero length.
    /// </summary>
    public class IconLocation
    {
        private string path = string.Empty;
        private int index;

        /// <summary>
        ///   Creates an empty icon location
        /// </summary>
        public IconLocation()
        {
            this.path = string.Empty;
            this.index = 0;
        }

        /// <summary>
        ///   Create a icon locatoin object
        /// </summary>
        /// <param name = "path">The path to the file containing the icon</param>
        /// <param name = "index">The index of the icon.</param>
        public IconLocation(string path, int index)
        {
            this.path = path;
            this.index = index;
        }

        /// <summary>
        ///   Gets the path to the file contaning the icon.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        ///   Gets the index of the icon in the file.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        ///   True of the icon location is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(path); }
        }
    }
    /// <summary>
    ///   These values map directly to the native SLR_* values for the IShellLink::Resolve() method
    /// </summary>
    public enum ResolveFlags
    {
        None = 0,
        /// <summary>
        ///   Do not display a dialog box if the link cannot be resolved.
        /// </summary>
        NoUi = 0x1,
        /// <summary>
        ///   Not used - ignored
        /// </summary>
        AnyMatch = 0x2,
        /// <summary>
        ///   If the link object has changed, update its path and list of identifiers. If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine whether or not the link object has changed.
        /// </summary>
        Update = 0x4,
        /// <summary>
        ///   Do not update the link information.
        /// </summary>
        NoUpdate = 0x8,
        /// <summary>
        ///   Do not execute the search heuristics.
        /// </summary>
        NoSearch = 0x10,
        /// <summary>
        ///   Do not use distributed link tracking.
        /// </summary>
        NoTrack = 0x20,
        /// <summary>
        ///   Disable distributed link tracking. By default, distributed link tracking tracks removable media across multiple devices based on the volume name. It also uses the UNC path to track remote file systems whose drive letter has changed. Setting NoLinkInfo disables both types of tracking
        /// </summary>
        NoLinkInfo = 0x40,
        /// <summary>
        ///   Call the Windows Installer
        /// </summary>
        InvokeMSI = 0x80,
        /// <summary>
        ///   Windows XP and later.
        /// </summary>
        NoUIWithMessagePump = 0x101,
        /// <summary>
        ///   Windows 7 and later. Offer the option to delete the shortcut when this method is unable to resolve it, even if the shortcut is not a shortcut to a file.
        /// </summary>
        OfferDeleteWithoutFile = 0x201,
        /// <summary>
        ///   Windows 7 and later. Report as dirty if the target is a known folder and the known folder was redirected. This only works if the original target path was a file system path or ID list and not an aliased known folder ID list.
        /// </summary>
        KnownFolder = 0x400,
        /// <summary>
        ///   Windows 7 and later. Resolve the computer name in UNC targets that point to a local computer. This value is used with SLDF_KEEP_LOCAL_IDLIST_FOR_UNC_TARGET.
        /// </summary>
        MachineInLocaltarget = 0x800,
        /// <summary>
        ///   Windows 7 and later. Update the computer GUID and user SID if necessary.
        /// </summary>
        UpdateMachineAndSid = 0x1000
    }
    /// <summary>
    /// 快捷方式
    /// </summary>
    public class ShellLink : IDisposable
    {
        #region Fields

        private ShellLinkCoClass theShellLinkObject;
        private IShellLink shellLink;
        private IShellLinkDataList dataList;
        private ConsoleProperties consoleProperties;

        private const int MAX_PATH = 260;

        #endregion
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="actualFilePath"></param>
        /// <param name="description"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static ShellLink CreateShortcut(string shortcutPath, string actualFilePath, string description = null, string workingDirectory = null, string arguments = null)
        {
            shortcutPath = shortcutPath.GetFullPath();
            actualFilePath = actualFilePath.GetFullPath();
            if (!System.IO.Path.HasExtension(shortcutPath))
                shortcutPath += ".LNK";

            var link = new ShellLink(shortcutPath);
            link.Path = actualFilePath;

            link.WorkingDirectory = workingDirectory ?? System.IO.Path.GetDirectoryName(actualFilePath);

            if (description != null)
                link.Description = description;

            if (arguments != null)
                link.Arguments = arguments;

            link.Save(shortcutPath);
            return link;
        }
        /// <summary>
        /// 是快捷方式路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        public static bool IsShellLink(string shortcutPath)
        {
            shortcutPath = shortcutPath.GetFullPath();

            if (File.Exists(shortcutPath))
            {
                try
                {
                    var shortcut = Load(shortcutPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// 指向路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static bool PointsTo(string shortcutPath, string targetPath)
        {
            shortcutPath = shortcutPath.GetFullPath();
            targetPath = targetPath.GetFullPath();

            if (File.Exists(shortcutPath))
            {
                try
                {
                    return Load(shortcutPath).Path.GetFullPath().Equals(targetPath, StringComparison.CurrentCultureIgnoreCase);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        #region Construction and Disposal

        /// <summary>
        ///   Create an empty shell link object
        /// </summary>
        public ShellLink()
        {
            theShellLinkObject = new ShellLinkCoClass();
            shellLink = (IShellLink)theShellLinkObject;
            dataList = (IShellLinkDataList)theShellLinkObject;
            consoleProperties = new ConsoleProperties(this);
        }

        /// <summary>
        ///   Load a shell link from a file.
        /// </summary>
        /// <param name = "linkFilePath">the path to the file</param>
        public ShellLink(string linkFilePath) : this()
        {
            if (File.Exists(linkFilePath))
            {
                ((IPersistFile)shellLink).Load(linkFilePath, (int)STGM_FLAGS.STGM_READ);
                ReadConsoleProperties();
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ShellLink()
        {
            Dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (dataList != null)
            {
                Marshal.ReleaseComObject(dataList);
                dataList = null;
            }

            if (shellLink != null)
            {
                Marshal.ReleaseComObject(shellLink);
                shellLink = null;
            }

            if (theShellLinkObject != null)
            {
                Marshal.ReleaseComObject(theShellLinkObject);
                theShellLinkObject = null;
            }
        }

        #endregion

        /// <summary>
        ///   Save a shortcut to a file.  The shell requires a '.lnk' file extension.
        /// </summary>
        /// <remarks>
        ///   If the file exists it is silently overwritten.
        /// </remarks>
        /// <param name = "lnkPath">The path to the saved file. </param>
        public void Save(string lnkPath)
        {
            ((IPersistFile)shellLink).Save(lnkPath, true);
        }

        /// <summary>
        ///   Load a shortcut from a file.
        /// </summary>
        /// <param name = "linkPath">A path to the file.</param>
        public static ShellLink Load(string linkPath)
        {
            var result = new ShellLink();
            ((IPersistFile)result.shellLink).Load(linkPath, (int)STGM_FLAGS.STGM_READ);
            result.ReadConsoleProperties();
            return result;
        }

        /// <summary>
        ///   Get or sets the target of the shell link. The getter for this property uses the SLGP_RAWPATH flags.
        /// </summary>
        public String Path
        {
            get
            {
                var sb = new StringBuilder(WIN32_FIND_DATAW.MAX_PATH);
                WIN32_FIND_DATAW findData;
                shellLink.GetPath(sb, sb.Capacity, out findData, SLGP_FLAGS.SLGP_RAWPATH);
                return sb.ToString();
            }

            set { shellLink.SetPath(value); }
        }

        /// <summary>
        ///   Gets the the path to the shortcut (.lnk) file using the SLGP_SHORTPATH flag
        /// </summary>
        public string ShortPath
        {
            get
            {
                var sb = new StringBuilder(WIN32_FIND_DATAW.MAX_PATH);
                WIN32_FIND_DATAW findData;
                shellLink.GetPath(sb, sb.Capacity, out findData, SLGP_FLAGS.SLGP_SHORTPATH);
                return sb.ToString();
            }
        }

        /// <summary>
        ///   Gets the the path to the shortcut (.lnk) file using the SLGP_UNCPRIORITY flag
        /// </summary>
        public string UncPriorityPath
        {
            get
            {
                var sb = new StringBuilder(WIN32_FIND_DATAW.MAX_PATH);
                WIN32_FIND_DATAW findData;
                shellLink.GetPath(sb, sb.Capacity, out findData, SLGP_FLAGS.SLGP_UNCPRIORITY);
                return sb.ToString();
            }
        }

        /// <summary>
        ///   The command line arguments to the shortcut.
        /// </summary>
        public String Arguments
        {
            get
            {
                var sb = new StringBuilder(260);
                shellLink.GetArguments(sb, sb.Capacity);
                return sb.ToString();
            }

            set { shellLink.SetArguments(value); }
        }

        /// <summary>
        ///   Attempts to find the target of a Shell link, even if it has been moved or renamed.
        /// </summary>
        /// <param name = "flags">Flags that control the resolution process</param>
        public void Resolve(ResolveFlags flags)
        {
            shellLink.Resolve(IntPtr.Zero, (SLR_FLAGS)flags);
        }

        /// <summary>
        ///   Attempts to find the target of a Shell link, even if it has been moved or renamed.
        /// </summary>
        /// <param name = "hwnd">A handle to the window that the Shell will use as the parent for a dialog box. The Shell displays the dialog box if it needs to prompt the user for more information while resolving a Shell link.</param>
        /// <param name = "flags">Flags that control the resolution process</param>
        public void Resolve(IntPtr hwnd, ResolveFlags flags)
        {
            shellLink.Resolve(hwnd, (SLR_FLAGS)flags);
        }

        /// <summary>
        ///   Attempts to find the target of a Shell link, even if it has been moved or renamed.
        /// </summary>
        /// <param name = "flags">Flags that control the resolution process</param>
        /// <param name = "noUxTimeoutMs">The timeout, in ms, to wait for resolution when there is no UX</param>
        public void Resolve(ResolveFlags flags, int noUxTimeoutMs)
        {
            if ((flags & ResolveFlags.NoUi) == 0)
            {
                throw new ArgumentException("This methiod requires that the ResolveFlags.NoUi flag is set in the flags parameter.");
            }

            if (noUxTimeoutMs > short.MaxValue)
            {
                throw new ArgumentException(string.Format("the nouxTimeoutMs value must be <= {0}", short.MaxValue));
            }

            unchecked
            {
                flags = flags & (ResolveFlags)0x0000FFFF;
                flags |= (ResolveFlags)(noUxTimeoutMs << 16);
            }

            shellLink.Resolve(IntPtr.Zero, (SLR_FLAGS)flags);
        }

        /// <summary>
        ///   Gets or sets the shortcut's working directory.
        /// </summary>
        public String WorkingDirectory
        {
            get
            {
                var sb = new StringBuilder(260);
                shellLink.GetWorkingDirectory(sb, sb.Capacity);
                return sb.ToString();
            }

            set { shellLink.SetWorkingDirectory(value); }
        }

        /// <summary>
        ///   Gets or sets the shortcut's description
        /// </summary>
        public String Description
        {
            get
            {
                var sb = new StringBuilder(260);
                shellLink.GetDescription(sb, sb.Capacity);
                return sb.ToString();
            }
            set { shellLink.SetDescription(value); }
        }

        /// <summary>
        ///   Gets and sets the location of the shortcut's ICON.  This may return an empty IconLocatoin object, one where the path property is empty.
        /// </summary>
        public IconLocation IconLocation
        {
            get
            {
                var sb = new StringBuilder(MAX_PATH);
                int iIcon;
                if (shellLink.GetIconLocation(sb, sb.Capacity, out iIcon) < 0)
                {
                    return new IconLocation();
                }
                return new IconLocation(sb.ToString(), iIcon);
            }

            set { shellLink.SetIconLocation(value.Path, value.Index); }
        }

        /// <summary>
        ///   Gets and sets the show command for shell link's object.
        /// </summary>
        public ShowWindowCommand ShowCommand
        {
            get
            {
                int showCmd;
                if (shellLink.GetShowCmd(out showCmd) < 0)
                {
                    return ShowWindowCommand.Hide;
                }
                return (ShowWindowCommand)showCmd;
            }
            set { shellLink.SetShowCmd((int)value); }
        }

        /// <summary>
        ///   Gets or sets the Shell Link Data Flags for a shell link
        /// </summary>
        public ShellLinkFlags Flags
        {
            get
            {
                UInt32 flags;
                dataList.GetFlags(out flags);
                return (ShellLinkFlags)flags;
            }
            set { dataList.SetFlags((UInt32)value); }
        }

        /// <summary>
        ///   True if the Shell Link has an NT_CONSOLE_PROPS data block.
        /// </summary>
        public bool HasConsoleProperties
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(NT_CONSOLE_PROPS.NT_CONSOLE_PROPS_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return false;
                }
                else
                {
                    Marshal.FreeHGlobal(ppDataBlock);
                    return true;
                }
            }
        }

        /// <summary>
        ///   Gets the console properties for a shell link.  If HasConsoleProperties is false, then this 
        ///   property returns a ConsoleProperties that contains sensible default values.
        /// </summary>
        public ConsoleProperties ConsoleProperties
        {
            get { return this.consoleProperties; }
        }

        /// <summary>
        ///   Is true if the shell link as an NT_FE_CONSOLE_PROPS data block.
        /// </summary>
        public bool HasCodePage
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(NT_FE_CONSOLE_PROPS.NT_FE_CONSOLE_PROPS_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return false;
                }
                else
                {
                    Marshal.FreeHGlobal(ppDataBlock);
                    return true;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the code page for the console.  if there is no code page then the value for this property is zero.
        ///   Setting this propety to zero removes the assocated NT_FE_CONSOLE_PROPS data block from the shell link.  
        ///   When in doubt, use the Windows 1252 code page.
        /// </summary>
        /// <exception cref = "OverflowExeption">Thrown if the set value cannot be converted to a UInt32 wihtout overflow.</exception>
        public long CodePage
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(NT_FE_CONSOLE_PROPS.NT_FE_CONSOLE_PROPS_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return 0;
                }

                var nt_fe_console_props = (NT_FE_CONSOLE_PROPS)Marshal.PtrToStructure(ppDataBlock, typeof(NT_FE_CONSOLE_PROPS));
                Marshal.FreeHGlobal(ppDataBlock);
                return (nt_fe_console_props.uCodePage);
            }

            set
            {
                dataList.RemoveDataBlock(NT_FE_CONSOLE_PROPS.NT_FE_CONSOLE_PROPS_SIG);

                if (value == 0)
                {
                    return;
                }

                UInt32 uCodePage;
                checked
                {
                    uCodePage = (UInt32)value;
                }

                NT_FE_CONSOLE_PROPS nt_fe_console_props = NT_FE_CONSOLE_PROPS.AnEmptyOne();
                nt_fe_console_props.uCodePage = uCodePage;

                // pin the structure, add it to the shell link, then un-pin it.
                GCHandle handle = GCHandle.Alloc(nt_fe_console_props, GCHandleType.Pinned); // pin the value
                dataList.AddDataBlock(GCHandle.ToIntPtr(handle));
                handle.Free(); // un-pin the value
            }
        }

        /// <summary>
        ///   Is true if the shell link has an EXP_SZ_LINK datablock with the EXP_SZ_LINK_SIG signature.
        /// </summary>
        public bool HasExpSzLink
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_SZ_LINK.EXP_SZ_LINK_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return false;
                }
                else
                {
                    Marshal.FreeHGlobal(ppDataBlock);
                    return true;
                }
            }
        }

        /// <summary>
        ///   Get and sets the EXP_SZ_LINK property for a shell link. If there is no link then the property 
        ///   value is an empty string. Setting this to null, an empty string, or a string that is all white space 
        ///   removes the EXP_SZ_LINK data block with the EXP_SZ_LINK_SIG signature from the assocated shell link.
        /// </summary>
        public string ExpSzLink
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_SZ_LINK.EXP_SZ_LINK_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return string.Empty;
                }

                var exp_sz_link = (EXP_SZ_LINK)Marshal.PtrToStructure(ppDataBlock, typeof(EXP_SZ_LINK));
                Marshal.FreeHGlobal(ppDataBlock);
                var value = new string(exp_sz_link.swzTarget);
                return value;
            }
            set
            {
                dataList.RemoveDataBlock(EXP_SZ_LINK.EXP_SZ_LINK_SIG);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (value.Length >= EXP_SZ_LINK.MAX_PATH)
                {
                    throw new ArgumentException(string.Format("The value must be less than {0} characters in lenght.", EXP_SZ_LINK.MAX_PATH));
                }

                EXP_SZ_LINK exp_sz_link = EXP_SZ_LINK.AnEmptyOne();

                value.CopyTo(0, exp_sz_link.swzTarget, 0, exp_sz_link.swzTarget.Length - 1);
                exp_sz_link.swzTarget[value.Length] = '\0';

                exp_sz_link.szTarget.Initialize(); // make this all zeros.

                GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned); // pin the value

                dataList.AddDataBlock(GCHandle.ToIntPtr(handle));

                handle.Free(); // un-pin the value
            }
        }

        /// <summary>
        ///   Is true if the shell link has an EXP_SZ_LINK datablock with the EXP_SZ_ICON_SIG signature.
        /// </summary>
        public bool HasExpSzIcon
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_SZ_ICON.EXP_SZ_ICON_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return false;
                }
                else
                {
                    Marshal.FreeHGlobal(ppDataBlock);
                    return true;
                }
            }
        }

        /// <summary>
        ///   Get and sets the EXP_SZ_ICON property for a shell link. If there is no link then the property 
        ///   value is an empty string. Setting this to null, an empty string, or a string that is all white space 
        ///   removes the EXP_SZ_LINK data block with the EXP_SZ_ICON_SIG signature from the assocated shell link.
        /// </summary>
        public string ExpSzIcon
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_SZ_ICON.EXP_SZ_ICON_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return string.Empty;
                }

                var exp_sz_icon = (EXP_SZ_ICON)Marshal.PtrToStructure(ppDataBlock, typeof(EXP_SZ_ICON));
                Marshal.FreeHGlobal(ppDataBlock);
                var value = new string(exp_sz_icon.swzTarget);
                return value;
            }
            set
            {
                dataList.RemoveDataBlock(EXP_SZ_ICON.EXP_SZ_ICON_SIG);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (value.Length >= EXP_SZ_ICON.MAX_PATH)
                {
                    throw new ArgumentException(string.Format("The value must be less than {0} characters in length.", EXP_SZ_ICON.MAX_PATH));
                }

                EXP_SZ_ICON exp_sz_link = EXP_SZ_ICON.AnEmptyOne();

                value.CopyTo(0, exp_sz_link.swzTarget, 0, exp_sz_link.swzTarget.Length - 1);
                exp_sz_link.swzTarget[value.Length] = '\0';

                exp_sz_link.szTarget.Initialize(); // make this all zeros.

                GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned); // pin the value

                dataList.AddDataBlock(GCHandle.ToIntPtr(handle));

                handle.Free(); // un-pin the value
            }
        }

        /// <summary>
        ///   True if the shell link ha a EXP_DARWIN_LINK data block.
        /// </summary>
        public bool HasDarwinLink
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_DARWIN_LINK.EXP_DARWIN_ID_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return false;
                }
                else
                {
                    Marshal.FreeHGlobal(ppDataBlock);
                    return true;
                }
            }
        }

        /// <summary>
        ///   Get and sets the EXP_DARWIN_LINK property for a shell link. If there is no link then the property 
        ///   value is an empty string. Setting this to null, an empty string, or a string that is all white space 
        ///   removes the EXP_DARWIN_LINK data block from the assocated shell link.
        /// </summary>
        public string DarwinLink
        {
            get
            {
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_DARWIN_LINK.EXP_DARWIN_ID_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return string.Empty;
                }

                var exp_darwin_link = (EXP_DARWIN_LINK)Marshal.PtrToStructure(ppDataBlock, typeof(EXP_DARWIN_LINK));
                Marshal.FreeHGlobal(ppDataBlock);
                var value = new string(exp_darwin_link.szwDarwinID);
                return value;
            }
            set
            {
                dataList.RemoveDataBlock(EXP_DARWIN_LINK.EXP_DARWIN_ID_SIG);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (value.Length >= EXP_DARWIN_LINK.MAX_PATH)
                {
                    throw new ArgumentException(string.Format("The value must be less than {0} characters in lenght.", EXP_SZ_ICON.MAX_PATH));
                }

                EXP_DARWIN_LINK exp_darwin_link = EXP_DARWIN_LINK.AnEmptyOne();

                value.CopyTo(0, exp_darwin_link.szwDarwinID, 0, exp_darwin_link.szwDarwinID.Length - 1);
                exp_darwin_link.szwDarwinID[value.Length] = '\0';

                exp_darwin_link.szDarwinID.Initialize(); // make this all zeros.

                GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned); // pin the value

                dataList.AddDataBlock(GCHandle.ToIntPtr(handle));

                handle.Free(); // un-pin the value
            }
        }

        #region Internal Shell Support

        /// <summary>
        ///   Removes a data block
        /// </summary>
        /// <param name = "signature">The signature of the data block</param>
        /// <exception cref = "ArgumentException">Thrown if the signature is not supported.</exception>
        internal void RemoveData(UInt32 signature)
        {
            switch (signature)
            {
                case NT_CONSOLE_PROPS.NT_CONSOLE_PROPS_SIG:
                case NT_FE_CONSOLE_PROPS.NT_FE_CONSOLE_PROPS_SIG:
                case EXP_SZ_LINK.EXP_SZ_LINK_SIG:
                case EXP_SZ_ICON.EXP_SZ_ICON_SIG:
                case EXP_SPECIAL_FOLDER.EXP_SPECIAL_FOLDER_SIG:
                case EXP_DARWIN_LINK.EXP_DARWIN_ID_SIG:
                    dataList.RemoveDataBlock(signature);
                    return;

                default:
                    throw new ArgumentException("signature is invalid.");
            }
        }

        /// <summary>
        ///   Read the console properties from the shell link
        /// </summary>
        /// <returns>True if they exists and were read.  False if they did not exist.</returns>
        internal bool ReadConsoleProperties()
        {
            IntPtr ppDataBlock;
            Int32 hr = dataList.CopyDataBlock(NT_CONSOLE_PROPS.NT_CONSOLE_PROPS_SIG, out ppDataBlock);

            if (hr < 0)
            {
                return false;
            }

            var nt_console_props = (NT_CONSOLE_PROPS)Marshal.PtrToStructure(ppDataBlock, typeof(NT_CONSOLE_PROPS));
            Marshal.FreeHGlobal(ppDataBlock);

            this.consoleProperties.nt_console_props = nt_console_props;

            return true;
        }

        /// <summary>
        ///   Write the current NT_CONSOLE_PROPS properties to the link.
        /// </summary>
        internal void WriteConsoleProperties()
        {
            RemoveData(NT_CONSOLE_PROPS.NT_CONSOLE_PROPS_SIG);

            IntPtr dataBlock = Marshal.AllocCoTaskMem(Marshal.SizeOf(this.consoleProperties.nt_console_props));

            Marshal.StructureToPtr(this.consoleProperties.nt_console_props, dataBlock, false);

            dataList.AddDataBlock(dataBlock);

            Marshal.FreeCoTaskMem(dataBlock);
        }

        /// <summary>
        ///   Get and sets the Special Folder property for a shell link.
        /// </summary>
        internal EXP_SPECIAL_FOLDER ExpSpecialFolder
        {
            get
            {
                EXP_SPECIAL_FOLDER value;
                IntPtr ppDataBlock;
                Int32 hr = dataList.CopyDataBlock(EXP_SPECIAL_FOLDER.EXP_SPECIAL_FOLDER_SIG, out ppDataBlock);

                if (hr < 0)
                {
                    return new EXP_SPECIAL_FOLDER();
                }

                value = (EXP_SPECIAL_FOLDER)Marshal.PtrToStructure(ppDataBlock, typeof(EXP_SPECIAL_FOLDER));
                Marshal.FreeHGlobal(ppDataBlock);
                return value;
            }

            set
            {
                dataList.RemoveDataBlock(EXP_SPECIAL_FOLDER.EXP_SPECIAL_FOLDER_SIG);

                value.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(EXP_SPECIAL_FOLDER)));
                value.dbh.dwSignature = EXP_SPECIAL_FOLDER.EXP_SPECIAL_FOLDER_SIG;

                GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned); // pin the value

                dataList.AddDataBlock(GCHandle.ToIntPtr(handle));

                handle.Free(); // un-pin the value
            }
        }

        #endregion
    }

    /// <summary>
    ///   These flag values mapp to the native SHELL_LINK_DATA_FLAGS Enumeration.  See MSDN
    /// </summary>
    [Flags]
    public enum ShellLinkFlags
    {
        /// <summary>
        ///   Default value used when no other flag is explicitly set.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        ///   Default value used when no other flag is explicitly set.
        /// </summary>
        Default = 0x00000000,
        /// <summary>
        ///   The Shell link was saved with an ID list
        /// </summary>
        HasIdList = 0x00000001,
        /// <summary>
        ///   The Shell link was saved with link information to enable distributed tracking. This information is used by .lnk files to locate the target if the targets's path has changed. It includes information such as volume label and serial number, although the specific stored information can change from release to release.
        /// </summary>
        HasLinkInfo = 0x00000002,
        /// <summary>
        ///   The link has a name.
        /// </summary>
        HasName = 0x00000004,
        /// <summary>
        ///   The link has a relative path.
        /// </summary>
        HasRelativePath = 0x00000008,
        /// <summary>
        ///   The link has a working directory.
        /// </summary>
        HasWorkingDirectory = 0x00000010,
        /// <summary>
        ///   The link has arguments.
        /// </summary>
        HasArguments = 0x00000020,
        /// <summary>
        ///   The link has an icon location.
        /// </summary>
        HasIconLocation = 0x00000040,
        /// <summary>
        ///   Stored strings are Unicode.
        /// </summary>
        Unicode = 0x00000080,
        /// <summary>
        ///   Prevents the storage of link tracking information. If this flag is set, it is less likely, though not impossible, that a target can be found by the link if that target is moved.
        /// </summary>
        ForceNoLinkInfo = 0x00000100,
        /// <summary>
        ///   The link contains expandable environment strings such as %windir%.
        /// </summary>
        HasExpSz = 0x00000200,
        /// <summary>
        ///   Causes a 16-bit target application to run in a separate Virtual DOS Machine (VDM)/Windows on Windows (WOW).
        /// </summary>
        RunInSeparate = 0x00000400,
        /// <summary>
        ///   Not supported. Note that as of Windows Vista, this value is no longer defined.
        /// </summary>
        HasLogo3Id = 0x00000800,
        /// <summary>
        ///   The link is a special Windows Installer link.
        /// </summary>
        HasDarwinId = 0x00001000,
        /// <summary>
        ///   Causes the target application to run as a different user.
        /// </summary>
        RunAsUser = 0x00002000,
        /// <summary>
        ///   The icon path in the link contains an expandable environment string such as such as %windir%.
        /// </summary>
        HasExpIconSz = 0x00004000,
        /// <summary>
        ///   Prevents the use of ID list alias mapping when parsing the ID list from the path.
        /// </summary>
        NoPidlAlias = 0x00008000,
        /// <summary>
        ///   Forces the use of the UNC name (a full network resource name), rather than the local name
        /// </summary>
        ForceUncName = 0x00010000,
        /// <summary>
        ///   Causes the target of this link to launch with a shim layer active. A shim is an intermediate DLL that facilitates compatibility between otherwise incompatible software services. Shims are typically used to provide version compatibility.
        /// </summary>
        RunWithShimLayer = 0x00020000,
        /// <summary>
        ///   Windows Vista and later. Disable object ID distributed tracking information.
        /// </summary>
        ForceNoLinkTrack = 0x00040000,
        /// <summary>
        ///   Windows Vista and later. Enable the caching of target metadata into the link file.
        /// </summary>
        EnableTargetMetadata = 0x000800000,
        /// <summary>
        ///   Windows 7 and later. Disable shell link tracking.
        /// </summary>
        DisableLinkpathTracking = 0x00100000,
        /// <summary>
        ///   Windows Vista and later. Disable known folder tracking information.
        /// </summary>
        DisableKnownFolderRelativeTracking = 0x00200000,
        /// <summary>
        ///   Windows 7 and later. Disable known folder alias mapping when loading the IDList during deserialization.
        /// </summary>
        NoKfAlias = 0x00400000,
        /// <summary>
        ///   Windows 7 and later. Allow link to point to another shell link as long as this does not create cycles.
        /// </summary>
        AllowLinkToLInk = 0x00800000,
        /// <summary>
        ///   Windows 7 and later. Remove alias when saving the IDList
        /// </summary>
        UnaliasOnSave = 0x01000000,
        /// <summary>
        ///   Windows 7 and later. Recalculate the IDList from the path with the environmental variables at load time, rather than persisting the IDList.
        /// </summary>
        PreferEnvironmentPath = 0x02000000,
        /// <summary>
        ///   Windows 7 and later. If the target is a UNC location on a local machine, keep the local IDList target in addition to the remote target.
        /// </summary>
        KeepLocalIDListForUncTarget = 0x04000000,
        /// <summary>
        ///   Valid values for W7
        /// </summary>
        W7Valid = 0x07FFF7FF,
        /// <summary>
        ///   Valid Values For W8
        /// </summary>
        VistaValid = 0x003FF7FF,
        /// <summary>
        ///   Reserved, do not use
        /// </summary>
        Reserved = -2147483648
    }
    /// <summary>
    ///   Window 'show' commands - seee the Win32 ShowWindow() API for more info.
    /// </summary>
    public enum ShowWindowCommand
    {
        /// <summary>
        ///   SW_FORCEMINIMIZE - minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11,

        /// <summary>
        ///   SW_HIDE - Hides the window and activates another window.
        /// </summary>
        Hide = 0,

        /// <summary>
        ///   SW_MAXIMIZE - Maximizes the specified window.
        /// </summary>
        Maximize = 3,

        /// <summary>
        ///   SW_MINIMIZE - Minimizes the specified window and activates the next top-level window in the Z order.
        /// </summary>
        Minimize = 6,

        /// <summary>
        ///   SW_RESTORE - Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,

        /// <summary>
        ///   SW_SHOW - Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,

        /// <summary>
        ///   SW_SHOWDEFAULT - Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
        /// </summary>
        ShowDeafult = 10,

        /// <summary>
        ///   SW_SHOWMAXIMIZED - Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,

        /// <summary>
        ///   SW_SHOWMINIMIZED - Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,

        /// <summary>
        ///   SW_SHOWMINNOACTIVE - Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        /// </summary>
        ShowMinNoActive = 7,

        /// <summary>
        ///   SW_SHOWNORMAL - Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        /// </summary>
        ShowNormal = 1
    }
    #endregion
    #region // 内部类    
    internal interface ICommiter
    {
        void Commit();
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct COORD //C#ify
    {
        public COORD(Int16 X, Int16 Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Int16 X;
        public Int16 Y;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DATABLOCK_HEADER
    {
        public UInt32 cbSize;
        public UInt32 dwSignature;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EXP_DARWIN_LINK
    {
        public const UInt32 EXP_DARWIN_ID_SIG = 0xA0000006;
        public const int MAX_PATH = 260;

        /// <summary>
        ///   Gets an empty structure with a valid data block header and sensible defaults.
        /// </summary>
        public static EXP_DARWIN_LINK AnEmptyOne()
        {
            var value = new EXP_DARWIN_LINK();

            value.SetDataBlockHeader();

            value.szDarwinID = new sbyte[MAX_PATH];
            value.szwDarwinID = new char[MAX_PATH];

            return value;
        }

        /// <summary>
        ///   Sets the datablock header values for this sturcture.
        /// </summary>
        public void SetDataBlockHeader()
        {
            this.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(EXP_DARWIN_LINK)));
            this.dbh.dwSignature = EXP_DARWIN_ID_SIG;
        }

        public DATABLOCK_HEADER dbh;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public sbyte[] szDarwinID; // ANSI darwin ID associated with link

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public char[] szwDarwinID; // UNICODE darwin ID associated with link
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EXP_SPECIAL_FOLDER
    {
        public const UInt32 EXP_SPECIAL_FOLDER_SIG = 0xA0000005;

        public DATABLOCK_HEADER dbh;

        private UInt32 cbSize; // Size of this extra data block
        private UInt32 dwSignature; // signature of this extra data block
        private UInt32 idSpecialFolder; // special folder id this link points into
        private UInt32 cbOffset; // ofset into pidl from SLDF_HAS_ID_LIST for child
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EXP_SZ_ICON
    {
        public const UInt32 EXP_SZ_ICON_SIG = 0xA0000007; // LPEXP_SZ_LINK (icon)
        public const int MAX_PATH = 260;

        /// <summary>
        ///   Gets an empty structure with a valid data block header and sensible defaults.
        /// </summary>
        public static EXP_SZ_ICON AnEmptyOne()
        {
            var value = new EXP_SZ_ICON();

            value.SetDataBlockHeader();

            value.szTarget = new sbyte[MAX_PATH];
            value.swzTarget = new char[MAX_PATH];

            return value;
        }

        /// <summary>
        ///   Sets the datablock header values for this sturcture.
        /// </summary>
        public void SetDataBlockHeader()
        {
            this.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(EXP_SZ_ICON)));
            this.dbh.dwSignature = EXP_SZ_ICON_SIG;
        }

        public DATABLOCK_HEADER dbh;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public sbyte[] szTarget; // ANSI target name w/EXP_SZ in it

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public char[] swzTarget; // UNICODE target name w/EXP_SZ in it
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EXP_SZ_LINK
    {
        public const UInt32 EXP_SZ_LINK_SIG = 0xA0000001; // LPEXP_SZ_LINK (target)
        public const int MAX_PATH = 260;

        /// <summary>
        ///   Gets an empty structure with a valid data block header and sensible defaults.
        /// </summary>
        public static EXP_SZ_LINK AnEmptyOne()
        {
            var value = new EXP_SZ_LINK();

            value.SetDataBlockHeader();

            value.szTarget = new sbyte[MAX_PATH];
            value.swzTarget = new char[MAX_PATH];

            return value;
        }

        /// <summary>
        ///   Sets the datablock header values for this sturcture.
        /// </summary>
        public void SetDataBlockHeader()
        {
            this.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(EXP_SZ_LINK)));
            this.dbh.dwSignature = EXP_SZ_LINK_SIG;
        }

        public DATABLOCK_HEADER dbh;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public sbyte[] szTarget; // ANSI target name w/EXP_SZ in it

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public char[] swzTarget; // UNICODE target name w/EXP_SZ in it
    }
    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("0000010B-0000-0000-C000-000000000046")
    ]
    internal interface IPersistFile
    {
        #region Methods inherited from IPersist

        void GetClassID(out Guid pClassID);

        #endregion

        [PreserveSig]
        int IsDirty();

        void Load(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            int dwMode);

        void Save(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [MarshalAs(UnmanagedType.Bool)] bool fRemember);

        void SaveCompleted(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        void GetCurFile(
            out IntPtr ppszFileName);
    }
    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("000214EE-0000-0000-C000-000000000046")
    ]
    internal interface IShellLink
    {
        void GetPath(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile,
            int cchMaxPath,
            out WIN32_FIND_DATAW pfd,
            SLGP_FLAGS fFlags);

        void GetIDList(
            out IntPtr ppidl);

        void SetIDList(
            IntPtr pidl);

        void GetDescription(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName,
            int cchMaxName);

        void SetDescription(
            [MarshalAs(UnmanagedType.LPStr)] string pszName);

        void GetWorkingDirectory(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir,
            int cchMaxPath);

        void SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPStr)] string pszDir);

        void GetArguments(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs,
            int cchMaxPath);

        void SetArguments(
            [MarshalAs(UnmanagedType.LPStr)] string pszArgs);

        void GetHotkey(
            out short pwHotkey);

        void SetHotkey(
            short wHotkey);

        [PreserveSig]
        Int32 GetShowCmd(
            out int piShowCmd);

        void SetShowCmd(
            int iShowCmd);

        [PreserveSig]
        Int32 GetIconLocation(
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath,
            int cchIconPath,
            out int piIcon);

        void SetIconLocation(
            [MarshalAs(UnmanagedType.LPStr)] string pszIconPath,
            int iIcon);

        void SetRelativePath(
            [MarshalAs(UnmanagedType.LPStr)] string pszPathRel,
            int dwReserved);

        void Resolve(
            IntPtr hwnd,
            SLR_FLAGS fFlags);

        void SetPath(
            [MarshalAs(UnmanagedType.LPStr)] string pszFile);
    }
    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("45e2b4ae-b1c3-11d0-b92f-00a0c90312e1")
    ]
    internal interface IShellLinkDataList
    {
        [PreserveSig]
        Int32 AddDataBlock(IntPtr pDataBlock);

        [PreserveSig]
        Int32 CopyDataBlock(UInt32 dwSig, out IntPtr ppDataBlock);

        [PreserveSig]
        Int32 RemoveDataBlock(UInt32 dwSig);

        // The flags paramter values are defined in shlobj.h - see the SHELL_LINK_DATA_FLAGS enumeration
        void GetFlags(out UInt32 pdwFlags);
        void SetFlags(UInt32 dwFlags);
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct NT_CONSOLE_PROPS
    {
        public const UInt32 NT_CONSOLE_PROPS_SIG = 0xA0000002;

        /// <summary>
        ///   Gets an empty structure with a valid data block header and sensible defaults.
        /// </summary>
        public static NT_CONSOLE_PROPS AnEmptyOne()
        {
            var value = new NT_CONSOLE_PROPS();

            value.SetDataBlockHeader();

            value.wFillAttribute = 15;
            value.wPopupFillAttribute = 245;
            value.dwScreenBufferSize.X = 80;
            value.dwScreenBufferSize.Y = 300;
            value.dwWindowSize.X = 80;
            value.dwWindowSize.Y = 25;
            value.dwWindowOrigin.X = 0;
            value.dwWindowOrigin.Y = 0;
            value.nFont = 12;
            value.nInputBufferSize = 0;
            value.dwFontSize = new COORD(0, 12);
            value.uFontFamily = 54;
            value.uFontWeight = 400;
            value.FaceName = new char[32] {
                'C', 'o', 'n', 's', 'o', 'l', 'a', 's', '\0', '\0',
                '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
                '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
                '\0', '\0'
            };
            value.uCursorSize = 25;
            value.bFullScreen = false;
            value.bQuickEdit = false;
            value.bInsertMode = true;
            value.bAutoPosition = true;
            value.uHistoryBufferSize = 50;
            value.uNumberOfHistoryBuffers = 4;
            value.bHistoryNoDup = false;

            value.ColorTable = new UInt32[16] {
                0x0, // Black - 0
                0x800000, // Dark Blue - 8388608
                0x8000, // Dark Green - 32768
                0x808000, // Teal - 8421376
                0x80, // Dark Red - 128
                0x800080, // Dark Purple - 8388736
                0x8080, // Olive- 32896
                0xC0C0C0, // Light Grey - 12632256
                0x808080, // Dark Grey - 8421504
                0xFF0000, // Blue - 16711680
                0xFF00, // Light Green - 65280
                0xFFFF00, // Cyan -16776960
                0xFF, // Red - 255
                0xFF00FF, // Chartruse - 16711935
                0xFFFF, // Yellow - 65535
                0xFFFFFF // White - 16777215
            };

            return value;
        }

        /// <summary>
        ///   Sets the datablock header values for this sturcture.
        /// </summary>
        public void SetDataBlockHeader()
        {
            this.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(NT_CONSOLE_PROPS)));
            this.dbh.dwSignature = NT_CONSOLE_PROPS_SIG;
        }

        public DATABLOCK_HEADER dbh;

        public UInt16 wFillAttribute;
        public UInt16 wPopupFillAttribute;
        public COORD dwScreenBufferSize;
        public COORD dwWindowSize;
        public COORD dwWindowOrigin;
        public UInt32 nFont;
        public UInt32 nInputBufferSize;
        public COORD dwFontSize;
        public UInt32 uFontFamily;
        public UInt32 uFontWeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public char[] FaceName;
        public UInt32 uCursorSize;
        [MarshalAs(UnmanagedType.Bool)] public bool bFullScreen;
        [MarshalAs(UnmanagedType.Bool)] public bool bQuickEdit;
        [MarshalAs(UnmanagedType.Bool)] public bool bInsertMode;
        [MarshalAs(UnmanagedType.Bool)] public bool bAutoPosition;
        public UInt32 uHistoryBufferSize;
        public UInt32 uNumberOfHistoryBuffers;
        [MarshalAs(UnmanagedType.Bool)] public bool bHistoryNoDup;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public UInt32[] ColorTable;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct NT_FE_CONSOLE_PROPS
    {
        public const UInt32 NT_FE_CONSOLE_PROPS_SIG = 0xA0000004;

        /// <summary>
        ///   Gets an empty structure with a valid data block header and sensible defaults.
        /// </summary>
        /// <returns></returns>
        public static NT_FE_CONSOLE_PROPS AnEmptyOne()
        {
            var value = new NT_FE_CONSOLE_PROPS();

            value.SetDataBlockHeader();

            value.uCodePage = 0;

            return value;
        }

        /// <summary>
        ///   Sets the datablock header values for this sturcture.
        /// </summary>
        public void SetDataBlockHeader()
        {
            this.dbh.cbSize = unchecked((UInt32)Marshal.SizeOf(typeof(NT_FE_CONSOLE_PROPS)));
            this.dbh.dwSignature = NT_FE_CONSOLE_PROPS_SIG;
        }

        public DATABLOCK_HEADER dbh;

        public UInt32 uCodePage;
    }
    /// <summary>
    ///   This is the CoClass that impliments the shell link interfaces.
    /// </summary>
    [
        ComImport,
        Guid("00021401-0000-0000-C000-000000000046")
    ]
    internal class ShellLinkCoClass // : IPersistFile, IShellLink, IShellLinkDataList
    {
    }
    [Flags]
    internal enum SLGP_FLAGS
    {
        SLGP_SHORTPATH = 0x1,
        SLGP_UNCPRIORITY = 0x2,
        SLGP_RAWPATH = 0x4
    }
    [Flags]
    internal enum SLR_FLAGS
    {
        /// <summary>
        ///   Do not display a dialog box if the link cannot be resolved.
        /// </summary>
        SLR_NO_UI = 0x1,
        /// <summary>
        ///   Not used - ignored
        /// </summary>
        SLR_ANY_MATCH = 0x2,
        /// <summary>
        ///   If the link object has changed, update its path and list of identifiers. If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine whether or not the link object has changed.
        /// </summary>
        SLR_UPDATE = 0x4,
        /// <summary>
        ///   Do not update the link information.
        /// </summary>
        SLR_NOUPDATE = 0x8,
        /// <summary>
        ///   Do not execute the search heuristics.
        /// </summary>
        SLR_NOSEARCH = 0x10,
        /// <summary>
        ///   Do not use distributed link tracking.
        /// </summary>
        SLR_NOTRACK = 0x20,
        /// <summary>
        ///   Disable distributed link tracking. By default, distributed link tracking tracks removable media across multiple devices based on the volume name. It also uses the UNC path to track remote file systems whose drive letter has changed. Setting NoLinkInfo disables both types of tracking
        /// </summary>
        SLR_NOLINKINFO = 0x40,
        /// <summary>
        ///   Call the Windows Installer
        /// </summary>
        SLR_INVOKE_MSI = 0x80,
        /// <summary>
        ///   Windows XP and later.
        /// </summary>
        SLR_NO_UI_WITH_MSG_PUMP = 0x101,
        /// <summary>
        ///   Windows 7 and later. Offer the option to delete the shortcut when this method is unable to resolve it, even if the shortcut is not a shortcut to a file.
        /// </summary>
        SLR_OFFER_DELETE_WITHOUT_FILE = 0x201,
        /// <summary>
        ///   Windows 7 and later. Report as dirty if the target is a known folder and the known folder was redirected. This only works if the original target path was a file system path or ID list and not an aliased known folder ID list.
        /// </summary>
        SLR_KNOWNFOLDER = 0x400,
        /// <summary>
        ///   Windows 7 and later. Resolve the computer name in UNC targets that point to a local computer. This value is used with SLDF_KEEP_LOCAL_IDLIST_FOR_UNC_TARGET.
        /// </summary>
        SLR_MACHINE_IN_LOCAL_TARGET = 0x800,
        /// <summary>
        ///   Windows 7 and later. Update the computer GUID and user SID if necessary.
        /// </summary>
        SLR_UPDATE_MACHINE_AND_SID = 0x1000
    }
    [Flags]
    internal enum STGM_FLAGS
    {
        STGM_READ = 0x00000000,
        STGM_WRITE = 0x00000001,
        STGM_READWRITE = 0x00000002,
        STGM_SHARE_DENY_NONE = 0x00000040,
        STGM_SHARE_DENY_READ = 0x00000030,
        STGM_SHARE_DENY_WRITE = 0x00000020,
        STGM_SHARE_EXCLUSIVE = 0x00000010,
        STGM_PRIORITY = 0x00040000,
        STGM_CREATE = 0x00001000,
        STGM_CONVERT = 0x00020000,
        STGM_FAILIFTHERE = 0x00000000,
        STGM_DIRECT = 0x00000000,
        STGM_TRANSACTED = 0x00010000,
        STGM_NOSCRATCH = 0x00100000,
        STGM_NOSNAPSHOT = 0x00200000,
        STGM_SIMPLE = 0x08000000,
        STGM_DIRECT_SWMR = 0x00400000,
        STGM_DELETEONRELEASE = 0x04000000
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_DATAW
    {
        public int dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string cAlternateFileName;
        public const int MAX_PATH = 260;
    }
    #endregion
}
