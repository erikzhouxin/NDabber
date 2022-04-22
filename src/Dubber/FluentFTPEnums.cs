using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Dubber
{
    /// <summary>
    /// Server features
    /// </summary>
    [Flags]
    public enum FtpCapability : int
    {
        /// <summary>
        /// This server said it doesn't support anything!
        /// </summary>
        NONE = 1,

        /// <summary>
        /// Supports the MLST command
        /// </summary>
        MLSD = 2,

        /// <summary>
        /// Supports the SIZE command
        /// </summary>
        SIZE = 3,

        /// <summary>
        /// Supports the MDTM command
        /// </summary>
        MDTM = 4,

        /// <summary>
        /// Supports download/upload stream resumes
        /// </summary>
        REST = 5,

        /// <summary>
        /// Supports UTF8
        /// </summary>
        UTF8 = 6,

        /// <summary>
        /// PRET Command used by DrFTPD
        /// </summary>
        PRET = 7,

        /// <summary>
        /// Server supports the MFMT command for setting the
        /// modified date of an object on the server
        /// </summary>
        MFMT = 8,

        /// <summary>
        /// Server supports the MFCT command for setting the
        /// created date of an object on the server
        /// </summary>
        MFCT = 9,

        /// <summary>
        /// Server supports the MFF command for setting certain facts
        /// about file system objects. It typically allows you to modify
        /// the last modification time, creation time, UNIX group/owner/mode of a file.
        /// </summary>
        MFF = 10,

        /// <summary>
        /// Server supports the STAT command
        /// </summary>
        STAT = 11,

        /// <summary>
        /// Support for the HASH command
        /// </summary>
        HASH = 12,

        /// <summary>
        /// Support for the MD5 command
        /// </summary>
        MD5 = 13,

        /// <summary>
        /// Support for the XMD5 command
        /// </summary>
        XMD5 = 14,

        /// <summary>
        /// Support for the XCRC command
        /// </summary>
        XCRC = 15,

        /// <summary>
        /// Support for the XSHA1 command
        /// </summary>
        XSHA1 = 16,

        /// <summary>
        /// Support for the XSHA256 command
        /// </summary>
        XSHA256 = 17,

        /// <summary>
        /// Support for the XSHA512 command
        /// </summary>
        XSHA512 = 18,

        /// <summary>
        /// Support for the EPSV file-transfer command
        /// </summary>
        EPSV = 19,

        /// <summary>
        /// Support for the CPSV command
        /// </summary>
        CPSV = 20,

        /// <summary>
        /// Support for the NOOP command
        /// </summary>
        NOOP = 21,

        /// <summary>
        /// Support for the CLNT command
        /// </summary>
        CLNT = 22,

        /// <summary>
        /// Support for the SSCN command
        /// </summary>
        SSCN = 23,

        /// <summary>
        /// Support for the SITE MKDIR (make directory) server-specific command for ProFTPd
        /// </summary>
        SITE_MKDIR = 24,

        /// <summary>
        /// Support for the SITE RMDIR (remove directory) server-specific command for ProFTPd
        /// </summary>
        SITE_RMDIR = 25,

        /// <summary>
        /// Support for the SITE UTIME server-specific command for ProFTPd
        /// </summary>
        SITE_UTIME = 26,

        /// <summary>
        /// Support for the SITE SYMLINK server-specific command for ProFTPd
        /// </summary>
        SITE_SYMLINK = 27,

        /// <summary>
        /// Support for the AVBL (get available space) server-specific command for Serv-U
        /// </summary>
        AVBL = 28,

        /// <summary>
        /// Support for the THMB (get image thumbnail) server-specific command for Serv-U
        /// </summary>
        THMB = 29,

        /// <summary>
        /// Support for the RMDA (remove directory) server-specific command for Serv-U
        /// </summary>
        RMDA = 30,

        /// <summary>
        /// Support for the DSIZ (get directory size) server-specific command for Serv-U
        /// </summary>
        DSIZ = 31,

        /// <summary>
        /// Support for the HOST (get host) server-specific command for Serv-U
        /// </summary>
        HOST = 32,

        /// <summary>
        /// Support for the CCC (Clear Command Channel) command, which makes a secure FTP channel revert back to plain text.
        /// </summary>
        CCC = 33,

        /// <summary>
        /// Support for the MODE Z (compression enabled) command, which says that the server supports ZLIB compression for all transfers
        /// </summary>
        MODE_Z = 34,

        /// <summary>
        /// Support for the LANG (language negotiation) command.
        /// </summary>
        LANG = 35,

        /// <summary>
        /// Support for the MMD5 (multiple MD5 hash) command.
        /// </summary>
        MMD5 = 36,

    }
    /// <summary>
    /// Flags that control how file comparison is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpCompareOption
    {

        /// <summary>
        /// Compares the file size and the checksum of the file (using the first supported hash algorithm).
        /// The local and remote file sizes and checksums should exactly match for the file to be considered equal.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Compares the file size.
        /// Both file sizes should exactly match for the file to be considered equal.
        /// </summary>
        Size = 1,

        /// <summary>
        /// Compares the date modified of the file.
        /// Both dates should exactly match for the file to be considered equal.
        /// </summary>
        DateModified = 2,

        /// <summary>
        /// Compares the checksum or hash of the file using the first supported hash algorithm.
        /// Both checksums should exactly match for the file to be considered equal.
        /// </summary>
        Checksum = 4,

    }
    /// <summary>
    /// The result of a file comparison operation.
    /// </summary>
    public enum FtpCompareResult
    {

        /// <summary>
        /// Success. Local and remote files are exactly equal.
        /// </summary>
        Equal = 1,

        /// <summary>
        /// Failure. Local and remote files do not match.
        /// </summary>
        NotEqual = 2,

        /// <summary>
        /// Failure. Either the local or remote file does not exist.
        /// </summary>
        FileNotExisting = 3,

        /// <summary>
        /// Failure. Checksum verification is enabled and your server does not support any hash algorithm.
        /// </summary>
        ChecksumNotSupported = 4,

    }
    /// <summary>
    /// Data connection type
    /// </summary>
    public enum FtpDataConnectionType
    {
        /// <summary>
        /// This type of data connection attempts to use the EPSV command
        /// and if the server does not support EPSV it falls back to the
        /// PASV command before giving up unless you are connected via IPv6
        /// in which case the PASV command is not supported.
        /// </summary>
        AutoPassive,

        /// <summary>
        /// Passive data connection. EPSV is a better
        /// option if it's supported. Passive connections
        /// connect to the IP address dictated by the server
        /// which may or may not be accessible by the client
        /// for example a server behind a NAT device may
        /// give an IP address on its local network that
        /// is inaccessible to the client. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASV,

        /// <summary>
        /// Same as PASV except the host supplied by the server is ignored
        /// and the data connection is made to the same address that the control
        /// connection is connected to. This is useful in scenarios where the
        /// server supplies a private/non-routable network address in the
        /// PASV response. It's functionally identical to EPSV except some
        /// servers may not implement the EPSV command. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASVEX,

        /// <summary>
        /// Extended passive data connection, recommended. Works
        /// the same as a PASV connection except the server
        /// does not dictate an IP address to connect to, instead
        /// the passive connection goes to the same address used
        /// in the control connection. This type of data connection
        /// supports IPv4 and IPv6.
        /// </summary>
        EPSV,

        /// <summary>
        /// This type of data connection attempts to use the EPRT command
        /// and if the server does not support EPRT it falls back to the
        /// PORT command before giving up unless you are connected via IPv6
        /// in which case the PORT command is not supported.
        /// </summary>
        AutoActive,

        /// <summary>
        /// Active data connection, not recommended unless
        /// you have a specific reason for using this type.
        /// Creates a listening socket on the client which
        /// requires firewall exceptions on the client system
        /// as well as client network when connecting to a
        /// server outside of the client's network. In addition
        /// the IP address of the interface used to connect to the
        /// server is the address the server is told to connect to
        /// which, if behind a NAT device, may be inaccessible to
        /// the server. This type of data connection is not supported
        /// by IPv6. If you specify PORT and are connected via IPv6
        /// EPRT will automatically be used instead.
        /// </summary>
        PORT,

        /// <summary>
        /// Extended active data connection, not recommended
        /// unless you have a specific reason for using this
        /// type. Creates a listening socket on the client
        /// which requires firewall exceptions on the client
        /// as well as client network when connecting to a 
        /// server outside of the client's network. The server
        /// connects to the IP address it sees the client coming
        /// from. This type of data connection supports IPv4 and IPv6.
        /// </summary>
        EPRT
    }
    /// <summary>
    /// Type of data transfer to do
    /// </summary>
    public enum FtpDataType
    {
        /// <summary>
        /// ASCII transfer
        /// </summary>
        ASCII,

        /// <summary>
        /// Binary transfer
        /// </summary>
        Binary
    }
    /// <summary>
    /// Controls how timestamps returned by the server are converted.
    /// </summary>
    public enum FtpDate
    {

        /// <summary>
        /// Returns the server timestamps in Server Time. No timezone conversion is performed.
        /// </summary>
        ServerTime = 0,

        /// <summary>
        /// Returns the server timestamps in Local Time.
        /// Ensure that the TimeZone property is correctly set to the server's timezone.
        /// If you are on .NET Core/.NET Standard, you need to set the LocalTimeZone property for this to work.
        /// </summary>
        LocalTime = 1,

        /// <summary>
        /// Returns the server timestamps in UTC (Coordinated Universal Time).
        /// Ensure that the TimeZone property is correctly set to the server's timezone.
        /// </summary>
        UTC = 2,

    }
    /// <summary>
    /// Defines the type of encryption to use
    /// </summary>
    public enum FtpEncryptionMode
    {
        /// <summary>
        /// Plain text.
        /// </summary>
        None,

        /// <summary>
        /// FTPS encryption is used from the start of the connection, port 990.
        /// </summary>
        Implicit,

        /// <summary>
        /// Connection starts in plain text and FTPS encryption is enabled
        /// with the AUTH command immediately after the server greeting.
        /// </summary>
        Explicit,

        /// <summary>
        /// FTPS encryption is used if supported by the server, otherwise it falls back to plaintext FTP communication.
        /// </summary>
        Auto
    }
    /// <summary>
    /// Defines how multi-file processes should handle a processing error.
    /// </summary>
    /// <remarks><see cref="FtpError.Stop"/> &amp; <see cref="FtpError.Throw"/> Cannot Be Combined</remarks>
    [Flags]
    public enum FtpError
    {
        /// <summary>
        /// No action is taken upon errors.  The method absorbs the error and continues.
        /// </summary>
        None = 0,

        /// <summary>
        /// If any files have completed successfully (or failed after a partial download/upload) then should be deleted.  
        /// This will simulate an all-or-nothing transaction downloading or uploading multiple files.  If this option is not
        /// combined with <see cref="FtpError.Stop"/> or <see cref="FtpError.Throw"/> then the method will
        /// continue to process all items whether if they are successful or not and then delete everything if a failure was
        /// encountered at any point.
        /// </summary>
        DeleteProcessed = 1,

        /// <summary>
        /// The method should stop processing any additional files and immediately return upon encountering an error.
        /// Cannot be combined with <see cref="FtpError.Throw"/>
        /// </summary>
        Stop = 2,

        /// <summary>
        /// The method should stop processing any additional files and immediately throw the current error.
        /// Cannot be combined with <see cref="FtpError.Stop"/>
        /// </summary>
        Throw = 4,
    }
    /// <summary>
    /// Type of file system of object
    /// </summary>
    public enum FtpFileSystemObjectSubType
    {

        /// <summary>
        /// The default subtype.
        /// </summary>
        Unknown,

        /// <summary>
        /// A sub directory within the listed directory.
        /// (Only set when machine listing is available and type is 'dir')
        /// </summary>
        SubDirectory,

        /// <summary>
        /// The self directory.
        /// (Only set when machine listing is available and type is 'cdir')
        /// </summary>
        SelfDirectory,

        /// <summary>
        /// The parent directory.
        /// (Only set when machine listing is available and type is 'pdir')
        /// </summary>
        ParentDirectory,

    }
    /// <summary>
    /// Type of file system of object
    /// </summary>
    public enum FtpFileSystemObjectType
    {
        /// <summary>
        /// A file
        /// </summary>
        File,

        /// <summary>
        /// A directory
        /// </summary>
        Directory,

        /// <summary>
        /// A symbolic link
        /// </summary>
        Link
    }
    /// <summary>
    /// Determines how we handle downloading and uploading folders
    /// </summary>
    public enum FtpFolderSyncMode
    {

        /// <summary>
        /// Dangerous but useful method!
        /// Uploads/downloads all the missing files to update the server/local filesystem.
        /// Deletes the extra files to ensure that the target is an exact mirror of the source.
        /// </summary>
        Mirror,

        /// <summary>
        /// Safe method!
        /// Uploads/downloads all the missing files to update the server/local filesystem.
        /// </summary>
        Update,

    }
    /// <summary>
    /// Different types of hashing algorithms for computing checksums.
    /// </summary>
    [Flags]
    public enum FtpHashAlgorithm : int
    {

        /// <summary>
        /// Automatic algorithm, or hashing not supported.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// SHA-1 algorithm
        /// </summary>
        SHA1 = 1,

        /// <summary>
        /// SHA-256 algorithm
        /// </summary>
        SHA256 = 2,

        /// <summary>
        /// SHA-512 algorithm
        /// </summary>
        SHA512 = 4,

        /// <summary>
        /// MD5 algorithm
        /// </summary>
        MD5 = 8,

        /// <summary>
        /// CRC algorithm
        /// </summary>
        CRC = 16
    }
    /// <summary>
    /// IP Versions to allow when connecting
    /// to a server.
    /// </summary>
    [Flags]
    public enum FtpIpVersion : int
    {
        /// <summary>
        /// Internet Protocol Version 4
        /// </summary>
        IPv4 = 1,

        /// <summary>
        /// Internet Protocol Version 6
        /// </summary>
        IPv6 = 2,

        /// <summary>
        /// Allow any supported version
        /// </summary>
        ANY = IPv4 | IPv6
    }
    /// <summary>
    /// Flags that can control how a file listing is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpListOption
    {
        /// <summary>
        /// Tries machine listings (MDTM command) if supported,
        /// and if not then falls back to OS-specific listings (LIST command)
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Load the modify date using MDTM when it could not
        /// be parsed from the server listing. This only pertains
        /// to servers that do not implement the MLSD command.
        /// </summary>
        Modify = 1,

        /// <summary>
        /// Load the file size using the SIZE command when it
        /// could not be parsed from the server listing. This
        /// only pertains to servers that do not support the
        /// MLSD command.
        /// </summary>
        Size = 2,

        /// <summary>
        /// Combines the Modify and Size flags
        /// </summary>
        SizeModify = Modify | Size,

        /// <summary>
        /// Show hidden/dot files. This only pertains to servers
        /// that do not support the MLSD command. This option
        /// makes use the non standard -a parameter to LIST to
        /// tell the server to show hidden files. Since it's a
        /// non-standard option it may not always work. MLSD listings
        /// have no such option and whether or not a hidden file is
        /// shown is at the discretion of the server.
        /// </summary>
        AllFiles = 4,

        /// <summary>
        /// Force the use of OS-specific listings (LIST command) even if
        /// machine listings (MLSD command) are supported by the server
        /// </summary>
        ForceList = 8,

        /// <summary>
        /// Use the NLST command instead of LIST for a reliable file listing
        /// </summary>
        NameList = 16,

        /// <summary>
        /// Force the use of the NLST command (the slowest mode) even if machine listings
        /// and OS-specific listings are supported by the server
        /// </summary>
        ForceNameList = ForceList | NameList,

        /// <summary>
        /// Try to dereference symbolic links, and stored the linked file/directory in FtpListItem.LinkObject
        /// </summary>
        DerefLinks = 32,

        /// <summary>
        /// Sets the ForceList flag and uses `LS' instead of `LIST' as the
        /// command for getting a directory listing. This option overrides
        /// ForceNameList and ignores the AllFiles flag.
        /// </summary>
        UseLS = 64 | ForceList,

        /// <summary>
        /// Gets files within subdirectories as well. Adds the -r option to the LIST command.
        /// Some servers may not support this feature.
        /// </summary>
        Recursive = 128,

        /// <summary>
        /// Do not retrieve path when no path is supplied to GetListing(),
        /// instead just execute LIST with no path argument.
        /// </summary>
        NoPath = 256,

        /// <summary>
        /// Include two extra items into the listing, for the current directory (".")
        /// and the parent directory (".."). Meaningless unless you want these two
        /// items for some reason.
        /// </summary>
        IncludeSelfAndParent = 512,

        /// <summary>
        /// Force the use of STAT command for getting file listings
        /// </summary>
        UseStat = 1024
    }
    /// <summary>
    /// Determines how we handle partially downloaded files
    /// </summary>
    public enum FtpLocalExists
    {
        /// <summary>
        /// Restart the download of a file if it is partially downloaded.
        /// Overwrites the file if it exists on disk.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Resume the download of a file if it is partially downloaded.
        /// Appends to the file if it exists, by checking the length and adding the missing data.
        /// If the file doesn't exist on disk, a new file is created.
        /// </summary>
        Resume,

        /// <summary>
        /// Blindly skip downloading the file if it exists on disk, without any more checks.
        /// This is only included to be compatible with legacy behaviour.
        /// </summary>
        Skip,

        /// <summary>
        /// Append is now renamed to Resume.
        /// </summary>
        [ObsoleteAttribute("Append is now renamed to Resume to better reflect its behaviour.", true)]
        Append,
    }
    /// <summary>
    /// Defines the operating system of the FTP server.
    /// </summary>
    public enum FtpOperatingSystem
    {
        /// <summary>
        /// Unknown operating system
        /// </summary>
        Unknown,

        /// <summary>
        /// Definitely Windows or Windows Server
        /// </summary>
        Windows,

        /// <summary>
        /// Definitely Unix or AIX-based server
        /// </summary>
        Unix,

        /// <summary>
        /// Definitely VMS or OpenVMS server
        /// </summary>
        VMS,

        /// <summary>
        /// Definitely IBM OS/400 server
        /// </summary>
        IBMOS400,

        /// <summary>
        /// Definitely IBM z/OS server
        /// </summary>
        IBMzOS,

        /// <summary>
        /// Definitely SUN OS/Solaris server
        /// </summary>
        SunOS,
    }
    public enum FtpOperator
    {

        /// <summary>
        /// If the value is exactly equal to X
        /// </summary>
        Equals,
        /// <summary>
        /// If the value is anything except for X
        /// </summary>
        NotEquals,
        /// <summary>
        /// If the value is less than X
        /// </summary>
        LessThan,
        /// <summary>
        /// If the value is less than or equal to X
        /// </summary>
        LessThanOrEquals,
        /// <summary>
        /// If the value is more than X
        /// </summary>
        MoreThan,
        /// <summary>
        /// If the value is more than or equal to X
        /// </summary>
        MoreThanOrEquals,
        /// <summary>
        /// If the value is between the range of X and Y
        /// </summary>
        BetweenRange,
        /// <summary>
        /// If the value is outside the range of X and Y
        /// </summary>
        OutsideRange,

    }
    /// <summary>
    /// The type of response the server responded with
    /// </summary>
    public enum FtpParser : int
    {
        /// <summary>
        /// Use the custom parser that you have set on the FtpClient object (ListingCustomParser property)
        /// </summary>
        Custom = -1,

        /// <summary>
        /// Automatically detect the file listing parser to use based on the FTP server (SYST command).
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Machine listing parser, works on any FTP server supporting the MLST/MLSD commands.
        /// </summary>
        Machine = 1,

        /// <summary>
        /// File listing parser for Windows/IIS.
        /// </summary>
        Windows = 2,

        /// <summary>
        /// File listing parser for Unix.
        /// </summary>
        Unix = 3,

        /// <summary>
        /// Alternate parser for Unix. Use this if the default one does not work.
        /// </summary>
        UnixAlt = 4,

        /// <summary>
        /// File listing parser for Vax/VMS/OpenVMS.
        /// </summary>
        VMS = 5,

        /// <summary>
        /// File listing parser for IBM z/OS
        /// </summary>
        IBMzOS = 6,

        /// <summary>
        /// File listing parser for IBM OS/400.
        /// </summary>
        IBMOS400 = 7,

        /// <summary>
        /// File listing parser for Tandem/Nonstop Guardian OS.
        /// </summary>
        NonStop = 8
    }
    /// <summary>
    /// Types of file permissions
    /// </summary>
    [Flags]
    public enum FtpPermission : uint
    {
        /// <summary>
        /// No access
        /// </summary>
        None = 0,

        /// <summary>
        /// Executable
        /// </summary>
        Execute = 1,

        /// <summary>
        /// Writable
        /// </summary>
        Write = 2,

        /// <summary>
        /// Readable
        /// </summary>
        Read = 4
    }
    /// <summary>
    /// Defines the behavior for uploading/downloading files that already exist
    /// </summary>
    public enum FtpRemoteExists
    {

        /// <summary>
        /// Do not check if the file exists. A bit faster than the other options.
        /// Only use this if you are SURE that the file does not exist on the server.
        /// Otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        NoCheck,

        /// <summary>
        /// Resume uploading by appending to the remote file, but don't check if it exists and add missing data.
        /// This might be required if you don't have permissions on the server to list files in the folder.
        /// Only use this if you are SURE that the file does not exist on the server otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        ResumeNoCheck,

        /// <summary>
        /// Append the local file to the end of the remote file, but don't check if it exists and add missing data.
        /// This might be required if you don't have permissions on the server to list files in the folder.
        /// Only use this if you are SURE that the file does not exist on the server otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        AddToEndNoCheck,

        /// <summary>
        /// Skip the file if it exists, without any more checks.
        /// </summary>
        Skip,

        /// <summary>
        /// Overwrite the file if it exists.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Resume uploading by appending to the remote file if it exists.
        /// It works by checking the remote file length and adding the missing data.
        /// </summary>
        Resume,

        /// <summary>
        /// Append the local file to the end of the remote file.
        /// </summary>
        AddToEnd,

        /// <summary>
        /// Append is now renamed to Resume. Alternatively you can use AddToEnd.
        /// </summary>
        [ObsoleteAttribute("Append is now renamed to Resume to better reflect its behaviour. If you instead want to append the local file to the END of the remote file without resuming, then use AddToEnd.", true)]
        Append,
    }
    /// <summary>
    /// The type of response the server responded with
    /// </summary>
    public enum FtpResponseType : int
    {
        /// <summary>
        /// No response
        /// </summary>
        None = 0,

        /// <summary>
        /// Success
        /// </summary>
        PositivePreliminary = 1,

        /// <summary>
        /// Success
        /// </summary>
        PositiveCompletion = 2,

        /// <summary>
        /// Success
        /// </summary>
        PositiveIntermediate = 3,

        /// <summary>
        /// Temporary failure
        /// </summary>
        TransientNegativeCompletion = 4,

        /// <summary>
        /// Permanent failure
        /// </summary>
        PermanentNegativeCompletion = 5
    }
    /// <summary>
    /// Determines how SSL Buffering is handled
    /// </summary>
    public enum FtpsBuffering
    {
        /// <summary>
        /// Enables buffering in all cases except when using FTP proxies.
        /// </summary>
        Auto,

        /// <summary>
        /// Always disables SSL Buffering to reduce FTPS connectivity issues.
        /// </summary>
        Off,

        /// <summary>
        /// Always enables SSL Buffering to massively speed up FTPS operations.
        /// </summary>
        On
    }
    /// <summary>
    /// Defines the type of the FTP server software.
    /// Add constants here as you add detection scripts for individual server types.
    /// </summary>
    public enum FtpServer
    {
        /// <summary>
        /// Unknown FTP server software
        /// </summary>
        Unknown,

        /// <summary>
        /// Definitely PureFTPd server
        /// </summary>
        PureFTPd,

        /// <summary>
        /// Definitely VsFTPd server
        /// </summary>
        VsFTPd,

        /// <summary>
        /// Definitely ProFTPD server
        /// </summary>
        ProFTPD,

        /// <summary>
        /// Definitely FileZilla server
        /// </summary>
        FileZilla,

        /// <summary>
        /// Definitely OpenVMS server
        /// </summary>
        OpenVMS,

        /// <summary>
        /// Definitely Windows CE FTP server
        /// </summary>
        WindowsCE,

        /// <summary>
        /// Definitely WuFTPd server
        /// </summary>
        WuFTPd,

        /// <summary>
        /// Definitely GlobalScape EFT server
        /// </summary>
        GlobalScapeEFT,

        /// <summary>
        /// Definitely HP NonStop/Tandem server
        /// </summary>
        NonStopTandem,

        /// <summary>
        /// Definitely Serv-U server
        /// </summary>
        ServU,

        /// <summary>
        /// Definitely Cerberus FTP server
        /// </summary>
        Cerberus,

        /// <summary>
        /// Definitely Windows Server/IIS FTP server
        /// </summary>
        WindowsServerIIS,

        /// <summary>
        /// Definitely CrushFTP server
        /// </summary>
        CrushFTP,

        /// <summary>
        /// Definitely glFTPd server
        /// </summary>
        glFTPd,

        /// <summary>
        /// Definitely Homegate FTP server
        /// </summary>
        HomegateFTP,

        /// <summary>
        /// Definitely BFTPd server
        /// </summary>
        BFTPd,

        /// <summary>
        /// Definitely FTP2S3 gateway server
        /// </summary>
        FTP2S3Gateway,

        /// <summary>
        /// Definitely XLight FTP server
        /// </summary>
        XLight,

        /// <summary>
        /// Definitely Sun OS Solaris FTP server
        /// </summary>
        SolarisFTP,

        /// <summary>
        /// Definitely IBM z/OS FTP server
        /// </summary>
        IBMzOSFTP,

        /// <summary>
        /// Definitely IBM OS/400 FTP server
        /// </summary>
        IBMOS400FTP,

        /// <summary>
        /// Definitely FritzBox FTP server
        /// </summary>
        FritzBox,

        /// <summary>
        /// Definitely WS_FTP server
        /// </summary>
        WSFTPServer,

        /// <summary>
        /// Definitely PyFtpdLib server
        /// </summary>
        PyFtpdLib,

        /// <summary>
        /// Definitely Titan FTP server
        /// </summary>
        TitanFTP,
    }
    /// <summary>
    /// Types of special UNIX permissions
    /// </summary>
    [Flags]
    public enum FtpSpecialPermissions : int
    {
        /// <summary>
        /// No special permissions are set
        /// </summary>
        None = 0,

        /// <summary>
        /// Sticky bit is set
        /// </summary>
        Sticky = 1,

        /// <summary>
        /// SGID bit is set
        /// </summary>
        SetGroupID = 2,

        /// <summary>
        /// SUID bit is set
        /// </summary>
        SetUserID = 4
    }
    /// <summary>
    /// The result of an upload or download operation
    /// </summary>
    public enum FtpStatus
    {

        /// <summary>
        /// The upload or download failed with an error transferring, or the source file did not exist
        /// </summary>
        Failed = 0,

        /// <summary>
        /// The upload or download completed successfully
        /// </summary>
        Success = 1,

        /// <summary>
        /// The upload or download was skipped because the file already existed on the target
        /// </summary>
        Skipped = 2

    }
    /// <summary>
    /// Defines the level of the tracing message.  Depending on the framework version this is translated
    /// to an equivalent logging level in System.Diagnostices (if available)
    /// </summary>
    public enum FtpTraceLevel
    {
        /// <summary>
        /// Used for logging Debug or Verbose level messages
        /// </summary>
        Verbose,

        /// <summary>
        /// Used for logging Informational messages
        /// </summary>
        Info,

        /// <summary>
        /// Used for logging non-fatal or ignorable error messages
        /// </summary>
        Warn,

        /// <summary>
        /// Used for logging Error messages that may need investigation 
        /// </summary>
        Error
    }
    /// <summary>
    /// Defines if additional verification and actions upon failure that 
    /// should be performed when uploading/downloading files using the high-level APIs.  Ignored if the 
    /// FTP server does not support any hashing algorithms.
    /// </summary>
    [Flags]
    public enum FtpVerify
    {
        /// <summary>
        /// No verification of the file is performed
        /// </summary>
        None = 0,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then we retry the download/upload
        /// a specified amount of times before giving up. (See <see cref="FtpClient.RetryAttempts"/>)
        /// </summary>
        Retry = 1,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then the failed file will be deleted.
        /// If combined with <see cref="FtpVerify.Retry"/>, then
        /// the deletion will occur if it fails upon the final retry.
        /// </summary>
        Delete = 2,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then an exception will be thrown.
        /// If combined with <see cref="FtpVerify.Retry"/>, then the throw will
        /// occur upon the failure of the final retry, and/or if combined with <see cref="FtpVerify.Delete"/>
        /// the method will throw after the deletion is processed.
        /// </summary>
        Throw = 4,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then the method returns false and no other action is taken.
        /// </summary>
        OnlyChecksum = 8,
    }
    /// <summary>
    /// Flags that can control how a file listing is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpZOSListRealm
    {
        /// <summary>
        /// Not z/OS Server
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// HFS / USS 
        /// </summary>
        Unix = 0,

        /// <summary>
        /// z/OS classic dataset
        /// </summary>
        Dataset = 1,

        /// <summary>
        /// Partitioned dataset member, RECFM != U
        /// </summary>
        Member = 2,

        /// <summary>
        /// Partitioned dataset member, RECFM = U
        /// </summary>
        MemberU = 3
    }
}
