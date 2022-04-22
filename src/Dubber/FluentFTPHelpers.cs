using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Net;
using System.Net.NetworkInformation;

namespace System.Data.Dubber
{
    #region // Hashing
    internal class CRC32 : HashAlgorithm
    {
        public const uint DefaultPolynomial = 0xedb88320;

        public const uint DefaultSeed = 0xffffffff;

        private uint hash;
        private readonly uint seed;
        private readonly uint[] table;
        private static uint[] defaultTable;

        public CRC32()
        {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
        }

        public CRC32(uint polynomial, uint seed)
        {
            table = InitializeTable(polynomial);
            this.seed = seed;
            Initialize();
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = CalculateHash(table, hash, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
#if !NETSTANDARD || NETSTANDARD2_0
            this.HashValue = hashBuffer;
#endif
            return hashBuffer;
        }

        public override int HashSize
        {
            get { return 32; }
        }

        public static uint Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        public static uint Compute(uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        public static uint Compute(uint polynomial, uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static uint[] InitializeTable(uint polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
            {
                return defaultTable;
            }

            uint[] createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint entry = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry >>= 1;
                    }
                }

                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
            {
                defaultTable = createTable;
            }

            return createTable;
        }

        private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint crc = seed;
            for (int i = start; i < size; i++)
            {
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ (crc & 0xff)];
                }
            }

            return crc;
        }

        private byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new[] {
                (byte)((x >> 24) & 0xff),
                (byte)((x >> 16) & 0xff),
                (byte)((x >> 8) & 0xff),
                (byte)(x & 0xff)
            };
        }
    }
    /// <summary>
    /// Helper class to convert FtpHashAlgorithm
    /// </summary>
    internal static class HashAlgorithms
    {

        private static readonly Dictionary<string, FtpHashAlgorithm> NameToEnum = new Dictionary<string, FtpHashAlgorithm> {
            { "SHA-1", FtpHashAlgorithm.SHA1 },
            { "SHA-256", FtpHashAlgorithm.SHA256 },
            { "SHA-512", FtpHashAlgorithm.SHA512 },
            { "MD5", FtpHashAlgorithm.MD5 },
            { "CRC", FtpHashAlgorithm.CRC },
        };

        private static readonly Dictionary<FtpHashAlgorithm, string> EnumToName = new Dictionary<FtpHashAlgorithm, string> {
            { FtpHashAlgorithm.SHA1, "SHA-1" },
            { FtpHashAlgorithm.SHA256, "SHA-256" },
            { FtpHashAlgorithm.SHA512, "SHA-512" },
            { FtpHashAlgorithm.MD5, "MD5" },
            { FtpHashAlgorithm.CRC, "CRC" },
        };

        /// <summary>
        /// Get FtpHashAlgorithm from its string representation
        /// </summary>
        /// <param name="name">Name of the hash algorithm</param>
        /// <returns>The FtpHashAlgorithm</returns>
        public static FtpHashAlgorithm FromString(string name)
        {
            if (!NameToEnum.ContainsKey(name.ToUpper()))
            {
                throw new NotImplementedException("Unknown hash algorithm: " + name);
            }

            return NameToEnum[name];
        }

        /// <summary>
        /// Get string representation of FtpHashAlgorithm
        /// </summary>
        /// <param name="name">FtpHashAlgorithm to be converted into string</param>
        /// <returns>Name of the hash algorithm</returns>
        public static string PrintToString(this FtpHashAlgorithm name)
        {
            if (!EnumToName.ContainsKey(name))
            {
                return name.ToString();
            }

            return EnumToName[name];
        }

        private static readonly List<FtpHashAlgorithm> AlgoPreference = new List<FtpHashAlgorithm> {
            FtpHashAlgorithm.MD5, FtpHashAlgorithm.SHA1, FtpHashAlgorithm.SHA256, FtpHashAlgorithm.SHA512, FtpHashAlgorithm.CRC
        };

        /// <summary>
        /// Get the first supported algorithm, in the standard order of preference. If no hashing algos found, returns NONE.
        /// </summary>
        public static FtpHashAlgorithm FirstSupported(FtpHashAlgorithm supportedFlags)
        {
            foreach (var algo in AlgoPreference)
            {
                if (supportedFlags.HasFlag(algo))
                {
                    return algo;
                }
            }
            return FtpHashAlgorithm.NONE;
        }
    }
    internal static class HashParser
    {

        /// <summary>
        /// Parses the received FTP hash response into a new FtpHash object.
        /// </summary>
        public static FtpHash Parse(string reply)
        {

            // Current draft says the server should return this:
            //		SHA-256 0-49 169cd22282da7f147cb491e559e9dd filename.ext

            // Current version of FileZilla returns this:
            //		SHA-1 21c2ca15cf570582949eb59fb78038b9c27ffcaf 

            // Real reply that was failing:
            //		213 MD5 0-170500096 3197bf4ec5fa2d441c0f50264ca52f11


            var hash = new FtpHash();

            // FIX #722 - remove the FTP status code causing a wrong hash to be returned
            if (reply.StartsWith("2") && reply.Length > 10)
            {
                reply = reply.Substring(4);
            }

            Match m;
            if (!(m = Regex.Match(reply,
                @"(?<algorithm>.+)\s" +
                @"(?<bytestart>\d+)-(?<byteend>\d+)\s" +
                @"(?<hash>.+)\s" +
                @"(?<filename>.+)")).Success)
            {
                m = Regex.Match(reply, @"(?<algorithm>.+)\s(?<hash>.+)\s");
            }

            if (m != null && m.Success)
            {
                hash.Algorithm = HashAlgorithms.FromString(m.Groups["algorithm"].Value);
                hash.Value = m.Groups["hash"].Value;
            }
            else
            {
                // failed to parse
            }

            return hash;
        }


    }
    #endregion // Hashing
    #region // Parsers
    internal static class FtpIBMOS400Parser
    {
        private static int formatIndex = 0;

        /// <summary>
        /// Checks if the given listing is a valid IBM OS/400 file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] listing)
        {
            var count = Math.Min(listing.Length, 10);

            for (var i = 0; i < count; i++)
            {
                if (listing[i].ContainsAny(ValidListFormats, 0))
                {
                    return true;
                }
            }

            client.LogStatus(FtpTraceLevel.Verbose, "Not in OS/400 format");
            return false;
        }

        /// <summary>
        /// Parses IBM OS/400 format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record)
        {
            var values = record.SplitString();

            // skip blank lines
            if (values.Length <= 0)
            {
                return null;
            }

            // return what we can for MEM
            if (values.Length >= 2 && values[1].Equals(MemoryMarker))
            {
                var lastModifiedm = DateTime.MinValue;
                var ownerm = values[0];
                var namem = values[2];
                var filem = new FtpListItem(record, namem, 0, false, ref lastModifiedm);
                filem.RawOwner = ownerm;
                return filem;
            }

            if (values.Length < MinFieldCount)
            {
                return null;
            }

            // first field is owner
            var owner = values[0];

            // next is size
            var size = long.Parse(values[1]);

            var lastModifiedStr = values[2] + " " + values[3];
            var lastModified = ParseDateTime(client, lastModifiedStr);

            // test is dir
            var isDir = false;
            if (values[4] == DirectoryMarker || values[4] == DDirectoryMarker || values.Length == 5 && values[4] == FileMarker)
            {
                isDir = true;
            }

            // If there's no name, it's because we're inside a file.  Fake out a "current directory" name instead.
            var name = values.Length >= 6 ? values[5] : ".";
            if (name.EndsWith("/"))
            {
                isDir = true;
                name = name.Substring(0, name.Length - 1);
            }

            // create a new list item object with the parsed metadata
            var file = new FtpListItem(record, name, size, isDir, ref lastModified);
            file.RawOwner = owner;
            return file;
        }

        /// <summary>
        /// Parses the last modified date from IBM OS/400 format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            var lastModified = DateTime.MinValue;
            if (formatIndex >= DateTimeFormats.Length)
            {
                client.LogStatus(FtpTraceLevel.Warn, "Exhausted formats - failed to parse date");
                return DateTime.MinValue;
            }

            var prevIndex = formatIndex;
            for (var i = formatIndex; i < DateTimeFormats.Length; i++, formatIndex++)
            {
                try
                {
                    lastModified = DateTime.ParseExact(lastModifiedStr, DateTimeFormats[formatIndex], client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
                    if (lastModified > DateTime.Now.AddDays(2))
                    {
                        client.LogStatus(FtpTraceLevel.Verbose, "Swapping to alternate format (found date in future)");
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException)
                {
                    continue;
                }
            }

            if (formatIndex >= DateTimeFormats.Length)
            {
                client.LogStatus(FtpTraceLevel.Warn, "Exhausted formats - failed to parse date");
                return DateTime.MinValue;
            }

            if (formatIndex > prevIndex) // we've changed FTP formatters so redo
            {
                throw new FtpListParseException();
            }

            return lastModified;
        }

        #region Constants

        private static string DirectoryMarker = "*DIR";
        private static string DDirectoryMarker = "*DDIR";
        private static string MemoryMarker = "*MEM";
        private static string FileMarker = "*FILE";
        private static int MinFieldCount = 5;
        private static string[][] DateTimeFormats = { new[] { "dd'/'MM'/'yy' 'HH':'mm':'ss", "dd'/'MM'/'yyyy' 'HH':'mm':'ss", "dd'.'MM'.'yy' 'HH':'mm':'ss" }, new[] { "yy'/'MM'/'dd' 'HH':'mm':'ss", "yyyy'/'MM'/'dd' 'HH':'mm':'ss", "yy'.'MM'.'dd' 'HH':'mm':'ss" }, new[] { "MM'/'dd'/'yy' 'HH':'mm':'ss", "MM'/'dd'/'yyyy' 'HH':'mm':'ss", "MM'.'dd'.'yy' 'HH':'mm':'ss" } };
        private static string[] ValidListFormats = new[] { "*DIR", "*FILE", "*FLR", "*DDIR", "*STMF", "*LIB" };

        #endregion
    }
    internal static class FtpIBMzOSParser
    {
        /// <summary>
        /// Checks if the given listing is a valid IBM z/OS file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] listing)
        {
            // Check validity by using the title line
            // USS Realm     : "total nnnn"
            // Dataset       : "Volume Unit    Referred Ext Used Recfm Lrecl BlkSz Dsorg Dsname"
            // Member        : " Name     VV.MM   Created       Changed      Size  Init   Mod   Id"
            // Member Loadlib: " Name      Size     TTR   Alias-of AC--------- Attributes--------- Amode Rmode"

            return listing[0].Contains("total") ||
                   listing[0].Contains("Volume Unit") ||
                   listing[0].Contains("Name     VV.MM") ||
                   listing[0].Contains("Name      Size     TTR");
        }

        /// <summary>
        /// Parses IBM z/OS format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record, string path)
        {
            // Skip title line - all modes have one. 
            // Also set zOSListingRealm to remember the mode we are in

            // "total nnnn"
            if (record.Contains("total"))
            {
                client.zOSListingRealm = FtpZOSListRealm.Unix;
                return null;
            }

            // "Volume Unit    Referred Ext Used Recfm Lrecl BlkSz Dsorg Dsname"
            if (record.Contains("Volume Unit"))
            {
                client.zOSListingRealm = FtpZOSListRealm.Dataset;
                return null;
            }

            // " Name     VV.MM   Created       Changed      Size  Init   Mod   Id"
            if (record.Contains("Name     VV.MM"))
            {
                // This is an opportunity to issue XDSS and get the LRECL, but how?
                FtpReply reply;
                string cwd;
                // Is caller using FtpListOption.NoPath and CWD to the right place?
                if (path.Length == 0)
                {
                    cwd = client.GetWorkingDirectory();
                }
                // Caller is not using FtpListOption.NoPath, so the path can be used
                // but needs modification depending on its ending. Remove the "(...)"
                else if (path.EndsWith(")'"))
                {
                    cwd = path.Substring(0, path.IndexOf('(')) + "\'";
                }
                else if (path.EndsWith(")"))
                {
                    cwd = path.Substring(0, path.IndexOf('('));
                }
                else
                {
                    cwd = path;
                }
                if (!(reply = client.Execute("XDSS " + cwd)).Success)
                {
                    throw new FtpCommandException(reply);
                }
                // SITE PDSTYPE=PDSE RECFM=FB BLKSIZE=16000 DIRECTORY=1 LRECL=80 PRIMARY=3 SECONDARY=110 TRACKS EATTR=SYSTEM
                string[] words = reply.Message.Split(' ');
                string[] val = words[5].Split('=');
                client.zOSListingLRECL = UInt16.Parse(val[1]);
                client.zOSListingRealm = FtpZOSListRealm.Member;
                return null;
            }

            // "Name      Size     TTR   Alias-of AC--------- Attributes--------- Amode Rmode"
            if (record.Contains("Name      Size     TTR"))
            {
                client.zOSListingRealm = FtpZOSListRealm.MemberU;
                return null;
            }

            if (client.zOSListingRealm == FtpZOSListRealm.Unix)
            {
                // unix mode
                //
                //total 320
                //
                return FtpUnixParser.Parse(client, record);
            }

            if (client.zOSListingRealm == FtpZOSListRealm.Dataset)
            {
                // PS/PO mode
                //
                //Volume Unit    Referred Ext Used Recfm Lrecl BlkSz Dsorg Dsname    
                //ANSYBG 3390   2020/01/03  1   15  VB   32756 32760  PS  $.ADATA.XAA
                //ANSYBH 3390   2022/02/18  1+++++  VBS  32767 27966  PS  $.BDATA.XBB
                //

                // Ignore title line AND also ignore "VSAM", "Not Mounted" and "Error determining attributes"

                if (record.Substring(51, 4).Trim() == "PO" || record.Substring(51, 4).Trim() == "PS")
                {
                    string volume = record.Substring(0, 6);
                    string unit = record.Substring(7, 4);
                    string referred = record.Substring(14, 10).Trim();
                    string ext = record.Substring(25, 2).Trim();
                    string used = record.Substring(27, 5).Trim();
                    string recfm = record.Substring(34, 4).Trim();
                    string lrecl = record.Substring(39, 5).Trim();
                    string blksz = record.Substring(45, 5).Trim();
                    string dsorg = record.Substring(51, 4).Trim();
                    string dsname = record.Remove(0, 56).Trim().Split(' ')[0];
                    bool isDir = dsorg == "PO";
                    var lastModifiedStr = referred;
                    if (lastModifiedStr != "**NONE**")
                    {
                        lastModifiedStr += " 00:00";
                    }
                    var lastModified = ParseDateTime(client, lastModifiedStr);
                    // If "+++++" we could assume maximum "normal" size of 65535 tracks. (3.46GB)
                    // or preferably "large format sequential" of 16777215 tracks (885.38GB)
                    // This is a huge over-estimation in all probability but it cannot be helped.
                    var size = 16777216L * 56664L;
                    if (used != "+++++")
                    {
                        size = long.Parse(used) * 56664L; // 3390 dev bytes per track
                    }
                    var file = new FtpListItem(record, dsname, size, isDir, ref lastModified);
                    return file;
                }
                return null;
            }

            if (client.zOSListingRealm == FtpZOSListRealm.Member)
            {
                // Member mode
                //
                // Name     VV.MM   Created       Changed      Size  Init   Mod   Id   
                //$2CPF1    01.01 2001/10/18 2001/10/18 11:58    29    29     0 QFX3076
                //

                string name = record.Substring(0, 8).Trim();
                string changed = string.Empty;
                string records = "0";
                // Member stats may be empty
                if (record.TrimEnd().Length > 8)
                {
                    string vvmm = record.Substring(10, 5).Trim();
                    string created = record.Substring(17, 10).Trim();
                    changed = record.Substring(27, 16).Trim();
                    records = record.Substring(44, 5).Trim();
                    string init = record.Substring(50, 5).Trim();
                    string mod = record.Substring(56, 5).Trim();
                    string id = record.Substring(62, 6).Trim();
                }
                bool isDir = false;
                var lastModifiedStr = changed;
                var lastModified = ParseDateTime(client, lastModifiedStr);
                var size = ushort.Parse(records) * client.zOSListingLRECL;
                var file = new FtpListItem(record, name, size, isDir, ref lastModified);
                return file;
            }

            if (client.zOSListingRealm == FtpZOSListRealm.MemberU)
            {
                // Member Loadlib mode
                //
                // Name      Size     TTR   Alias-of AC --------- Attributes --------- Amode Rmode
                //EAGKCPT   000058   000009          00 FO             RN RU            31    ANY
                //EAGRTPRC  005F48   000011 EAGRTALT 00 FO             RN RU            31    ANY
                //

                string name = record.Substring(0, 8).Trim();
                string changed = string.Empty;
                string memsize = record.Substring(10, 6);
                string TTR = record.Substring(19, 6);
                string Alias = record.Substring(26, 8).Trim();
                string Attributes = record.Substring(38, 30);
                string Amode = record.Substring(70, 2);
                string Rmode = record.Substring(76, 3);
                bool isDir = false;
                var lastModifiedStr = changed;
                var lastModified = ParseDateTime(client, lastModifiedStr);
                var size = int.Parse(memsize, System.Globalization.NumberStyles.HexNumber);
                var file = new FtpListItem(record, name, size, isDir, ref lastModified);
                return file;
            }

            return null;
        }

        /// <summary>
        /// Parses the last modified date from IBM z/OS format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            var lastModified = DateTime.MinValue;
            if (lastModifiedStr == string.Empty || lastModifiedStr == "**NONE**")
            {
                return lastModified;
            }
            lastModified = DateTime.ParseExact(lastModifiedStr, @"yyyy'/'MM'/'dd HH':'mm", client.ListingCulture.DateTimeFormat, DateTimeStyles.None);

            return lastModified;
        }
    }
    internal static class FtpMachineListParser
    {
        /// <summary>
        /// Parses MLSD/MLST format listings
        /// </summary>
        /// <param name="record">A line from the listing</param>
        /// <param name="capabilities">Server capabilities</param>
        /// <param name="client">The FTP client</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(string record, List<FtpCapability> capabilities, FtpClient client)
        {
            var item = new FtpListItem();
            Match m;

            if (!(m = Regex.Match(record, "type=(?<type>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                return null;
            }

            switch (m.Groups["type"].Value.ToLower())
            {

                // Parent and self-directories are parsed but not always returned
                case "pdir":
                    item.Type = FtpFileSystemObjectType.Directory;
                    item.SubType = FtpFileSystemObjectSubType.ParentDirectory;
                    break;
                case "cdir":
                    item.Type = FtpFileSystemObjectType.Directory;
                    item.SubType = FtpFileSystemObjectSubType.SelfDirectory;
                    break;

                // Always list sub directories and files
                case "dir":
                    item.Type = FtpFileSystemObjectType.Directory;
                    item.SubType = FtpFileSystemObjectSubType.SubDirectory;
                    break;
                case "file":
                    item.Type = FtpFileSystemObjectType.File;
                    break;

                // These are not supported
                case "link":
                case "device":
                default:
                    return null;

            }

            if ((m = Regex.Match(record, "; (?<name>.*)$", RegexOptions.IgnoreCase)).Success)
            {
                item.Name = m.Groups["name"].Value;
            }
            else
            {
                // if we can't parse the file name there is a problem.
                return null;
            }

            ParseDateTime(record, item, client);

            ParseFileSize(record, item);

            ParsePermissions(record, item);

            return item;
        }

        /// <summary>
        /// Parses the date modified field from MLSD/MLST format listings
        /// </summary>
        private static void ParseDateTime(string record, FtpListItem item, FtpClient client)
        {
            Match m;
            if ((m = Regex.Match(record, "modify=(?<modify>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                item.Modified = m.Groups["modify"].Value.ParseFtpDate(client);
            }

            if ((m = Regex.Match(record, "created?=(?<create>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                item.Created = m.Groups["create"].Value.ParseFtpDate(client);
            }
        }

        /// <summary>
        /// Parses the file size field from MLSD/MLST format listings
        /// </summary>
        private static void ParseFileSize(string record, FtpListItem item)
        {
            Match m;
            if ((m = Regex.Match(record, @"size=(?<size>\d+);", RegexOptions.IgnoreCase)).Success)
            {
                long size;

                if (long.TryParse(m.Groups["size"].Value, out size))
                {
                    item.Size = size;
                }
            }
        }

        /// <summary>
        /// Parses the permissions from MLSD/MLST format listings
        /// </summary>
        private static void ParsePermissions(string record, FtpListItem item)
        {
            Match m;
            if ((m = Regex.Match(record, @"unix.mode=(?<mode>\d+);", RegexOptions.IgnoreCase)).Success)
            {
                if (m.Groups["mode"].Value.Length == 4)
                {
                    item.SpecialPermissions = (FtpSpecialPermissions)int.Parse(m.Groups["mode"].Value[0].ToString());
                    item.OwnerPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[1].ToString());
                    item.GroupPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[2].ToString());
                    item.OthersPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[3].ToString());
                    item.CalculateChmod();
                }
                else if (m.Groups["mode"].Value.Length == 3)
                {
                    item.OwnerPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[0].ToString());
                    item.GroupPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[1].ToString());
                    item.OthersPermissions = (FtpPermission)int.Parse(m.Groups["mode"].Value[2].ToString());
                    item.CalculateChmod();
                }
            }
        }
    }
    internal static class FtpNonStopParser
    {
        /// <summary>
        /// Checks if the given listing is a valid NonStop file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] records)
        {
            return IsHeader(records[0]);
        }

        private static bool IsHeader(string line)
        {
            if (line.Contains("Code") && line.Contains("EOF") && line.Contains("RWEP"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses NonStop format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record)
        {
            if (IsHeader(record))
            {
                return null;
            }

            var values = record.SplitString();

            if (values.Length < MinFieldCount)
            {
                return null;
            }

            // parse name
            var name = values[0];

            // parse date modified
            var lastModified = ParseDateTime(client, values[3] + " " + values[4]);

            // check if is a dir & parse file size
            bool isDir;
            long size;
            ParseDirAndFileSize(client, values, out isDir, out size);

            // parse owner and permissions
            var owner = values[5] + values[6];
            var permissions = values[7].Trim(TrimValues);

            // create a new list item object with the parsed metadata
            var file = new FtpListItem(record, name, size, isDir, ref lastModified);
            file.RawOwner = owner;
            file.RawPermissions = permissions;
            return file;
        }

        /// <summary>
        /// Parses the directory type and file size from NonStop format listings
        /// </summary>
        private static void ParseDirAndFileSize(FtpClient client, string[] values, out bool isDir, out long size)
        {
            isDir = false;
            size = 0L;
            try
            {
                size = long.Parse(values[2]);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + values[2]);
            }
        }

        /// <summary>
        /// Parses the last modified date from NonStop format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            try
            {
                var lastModified = DateTime.ParseExact(lastModifiedStr, DateTimeFormats, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
                return lastModified;
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + lastModifiedStr + "'");
            }
            return DateTime.MinValue;
        }

        #region Constants

        private static char[] TrimValues = { '"' };
        private static int MinFieldCount = 7;
        private static string[] DateTimeFormats = { "d'-'MMM'-'yy HH':'mm':'ss" };

        #endregion
    }
    internal static class FtpUnixParser
    {

        /// <summary>
        /// Checks if the given listing is a valid Unix file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] records)
        {
            var count = Math.Min(records.Length, 10);

            var perms1 = false;
            var perms2 = false;

            for (var i = 0; i < count; i++)
            {
                var record = records[i];
                if (record.Trim().Length == 0)
                {
                    continue;
                }

                var values = record.SplitString();
                if (values.Length < MinFieldCount)
                {
                    continue;
                }

                // check perms
                var ch00 = char.ToLower(values[0][0]);
                if (ch00 == '-' || ch00 == 'l' || ch00 == 'd')
                {
                    perms1 = true;
                }

                if (values[0].Length > 1)
                {
                    var ch01 = char.ToLower(values[0][1]);
                    if (ch01 == 'r' || ch01 == '-')
                    {
                        perms2 = true;
                    }
                }

                // last chance - Connect:Enterprise has -ART------TCP
                if (!perms2 && values[0].Length > 2 && values[0].IndexOf('-', 2) > 0)
                {
                    perms2 = true;
                }
            }

            if (perms1 && perms2)
            {
                return true;
            }

            client.LogStatus(FtpTraceLevel.Verbose, "Not in UNIX format");
            return false;
        }

        /// <summary>
        /// Parses Unix format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record)
        {
            // test it is a valid line, e.g. "total 342522" is invalid
            var ch = record[0];
            if (ch != FileMarker && ch != DirectoryMarker && ch != SymbolicLinkMarker)
            {
                return null;
            }

            var values = record.SplitString();

            if (values.Length < MinFieldCount)
            {
                var msg = new StringBuilder("Unexpected number of fields in listing '");
                msg
                    .Append(record)
                    .Append("' - expected minimum ").Append(MinFieldCount)
                    .Append(" fields but found ").Append(values.Length).Append(" fields");
                client.LogStatus(FtpTraceLevel.Verbose, msg.ToString());
                return null;
            }

            // field pos
            var index = 0;

            // first field is perms
            string permissions;
            bool isDir, isLink;
            ParsePermissions(values, ref index, out permissions, out isDir, out isLink);

            // some servers don't supply the link count
            var linkCount = ParseLinkCount(client, values, ref index);

            // parse owner & group permissions
            string owner, group;
            ParseOwnerGroup(values, ref index, out owner, out group);

            // parse size
            var size = ParseFileSize(client, values, ref index);

            // parse the date/time fields
            var dayOfMonth = ParseDayOfMonth(values, ref index);

            var dateTimePos = index;
            var lastModified = DateTime.MinValue;
            ParseDateTime(client, values, ref index, dayOfMonth, ref lastModified);

            // parse name of file or dir. Extract symlink if possible
            string name = null;
            string linkedname = null;
            ParseName(client, record, values, isLink, dayOfMonth, dateTimePos, ref name, ref linkedname);

            // create a new list item object with the parsed metadata
            var file = new FtpListItem(record, name, size, isDir, ref lastModified);
            if (isLink)
            {
                file.Type = FtpFileSystemObjectType.Link;
                file.LinkCount = linkCount;
                file.LinkTarget = linkedname.Trim();
            }

            file.RawGroup = group;
            file.RawOwner = owner;
            file.RawPermissions = permissions;
            file.CalculateUnixPermissions(permissions);
            return file;
        }

        /// <summary>
        /// Parses the permissions from Unix format listings
        /// </summary>
        private static int ParsePermissions(string[] values, ref int index, out string permissions, out bool isDir, out bool isLink)
        {
            permissions = values[index++];
            var ch = permissions[0];
            isDir = false;
            isLink = false;
            if (ch == DirectoryMarker)
            {
                isDir = true;
            }
            else if (ch == SymbolicLinkMarker)
            {
                isLink = true;
            }

            return index;
        }

        /// <summary>
        /// Parses the link count from Unix format listings
        /// </summary>
        private static int ParseLinkCount(FtpClient client, string[] values, ref int index)
        {
            var linkCount = 0;
            if (char.IsDigit(values[index][0]))
            {
                // assume it is if a digit
                var linkCountStr = values[index++];
                try
                {
                    linkCount = int.Parse(linkCountStr);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse link count: " + linkCountStr);
                }
            }
            else if (values[index][0] == '-')
            {
                // IPXOS Treck FTP server
                index++;
            }

            return linkCount;
        }

        /// <summary>
        /// Parses the owner and group permissions from Unix format listings
        /// </summary>
        private static void ParseOwnerGroup(string[] values, ref int index, out string owner, out string group)
        {
            // owner and group
            owner = "";
            group = "";

            // if 2 fields ahead is numeric and there's enough fields beyond (4) for
            // the date, then the next two fields should be the owner & group
            if (values[index + 2].IsNumeric() && values.Length - (index + 2) > 4)
            {
                owner = values[index++];
                group = values[index++];
            }

            // no owner
            else if (values[index + 1].IsNumeric() && values.Length - (index + 1) > 4)
            {
                group = values[index++];
            }
        }

        /// <summary>
        /// Parses the file size from Unix format listings
        /// </summary>
        private static long ParseFileSize(FtpClient client, string[] values, ref int index)
        {
            var size = 0L;
            var sizeStr = values[index++].Replace(".", ""); // get rid of .'s in size           
            try
            {
                size = long.Parse(sizeStr);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + sizeStr);
            }

            return size;
        }

        /// <summary>
        /// Parses day-of-month from Unix format listings
        /// </summary>
        private static int ParseDayOfMonth(string[] values, ref int index)
        {
            var dayOfMonth = -1;

            // we expect the month first on Unix. 
            // Connect:Enterprise UNIX has a weird extra numeric field here - we test if the 
            // next field is numeric and if so, we skip it (except we check for a BSD variant
            // that means it is the day of the month)
            if (values[index].IsNumeric())
            {
                // this just might be the day of month - BSD variant
                // we check it is <= 31 AND that the next field starts
                // with a letter AND the next has a ':' within it
                try
                {
                    char[] chars = { '0' };
                    var str = values[index].TrimStart(chars);
                    dayOfMonth = int.Parse(values[index]);
                    if (dayOfMonth > 31) // can't be day of month
                    {
                        dayOfMonth = -1;
                    }

                    if (!char.IsLetter(values[index + 1][0]))
                    {
                        dayOfMonth = -1;
                    }

                    if (values[index + 2].IndexOf(':') <= 0)
                    {
                        dayOfMonth = -1;
                    }
                }
                catch (FormatException)
                {
                }

                index++;
            }

            return dayOfMonth;
        }

        /// <summary>
        /// Parses the file or folder name from Unix format listings
        /// </summary>
        private static void ParseName(FtpClient client, string record, string[] values, bool isLink, int dayOfMonth, int dateTimePos, ref string name, ref string linkedname)
        {
            // find the starting point of the name by finding the pos of all the date/time fields
            var pos = 0;
            var ok = true;
            var dateFieldCount = dayOfMonth > 0 ? 2 : 3; // only 2 fields left if we had a leading day of month
            for (var i = dateTimePos; i < dateTimePos + dateFieldCount; i++)
            {
                pos = record.IndexOf(values[i], pos);
                if (pos < 0)
                {
                    ok = false;
                    break;
                }
                else
                {
                    pos += values[i].Length;
                }
            }

            if (ok)
            {
                var remainder = record.Substring(pos).Trim();
                if (!isLink)
                {
                    name = remainder;
                }
                else
                {
                    // symlink, try to extract it
                    pos = remainder.IndexOf(SymbolicLinkArrowMarker);
                    if (pos <= 0)
                    {
                        // couldn't find symlink, give up & just assign as name
                        name = remainder;
                    }
                    else
                    {
                        var len = SymbolicLinkArrowMarker.Length;
                        name = remainder.Substring(0, pos - 0).Trim();
                        if (pos + len < remainder.Length)
                        {
                            linkedname = remainder.Substring(pos + len);
                        }
                    }
                }
            }
            else
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            }
        }

        /// <summary>
        /// Parses the last modified date from Unix format listings
        /// </summary>
        private static void ParseDateTime(FtpClient client, string[] values, ref int index, int dayOfMonth, ref DateTime lastModified)
        {
            var stamp = new StringBuilder(values[index++]);
            stamp.Append('-');
            if (dayOfMonth > 0)
            {
                stamp.Append(dayOfMonth);
            }
            else
            {
                stamp.Append(values[index++]);
            }

            stamp.Append('-');

            var field = values[index++];
            if (field.IndexOf((char)':') < 0 && field.IndexOf((char)'.') < 0)
            {
                stamp.Append(field); // year
                lastModified = ParseYear(client, stamp, DateTimeFormats1);
            }
            else
            {
                // add the year ourselves as not present
                var year = client.ListingCulture.Calendar.GetYear(DateTime.Now);
                stamp.Append(year).Append('-').Append(field);
                lastModified = ParseDateTime(client, stamp, DateTimeFormats2);
            }
        }

        /// <summary>
        /// Parses the last modified year from Unix format listings
        /// </summary>
        private static DateTime ParseYear(FtpClient client, StringBuilder stamp, string[] format)
        {
            var lastModified = DateTime.MinValue;
            try
            {
                lastModified = DateTime.ParseExact(stamp.ToString(), format, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + stamp.ToString() + "'");
            }

            return lastModified;
        }

        /// <summary>
        /// Parses the last modified date from Unix format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, StringBuilder stamp, string[] format)
        {
            var lastModified = DateTime.MinValue;
            try
            {
                lastModified = DateTime.ParseExact(stamp.ToString(), format, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + stamp.ToString() + "'");
            }

            // can't be in the future - must be the previous year
            // add 2 days for time zones
            if (lastModified > DateTime.Now.AddDays(2))
            {
                lastModified = lastModified.AddYears(-1);
            }

            return lastModified;
        }

        /// <summary>
        /// Parses Unix format listings with alternate parser
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem ParseUnixAlt(FtpClient client, string record)
        {
            // test it is a valid line, e.g. "total 342522" is invalid
            var ch = record[0];
            if (ch != FileMarker && ch != DirectoryMarker && ch != SymbolicLinkMarker)
            {
                return null;
            }

            var values = record.SplitString();

            if (values.Length < MinFieldCountAlt)
            {
                var listing = new StringBuilder("Unexpected number of fields in listing '");
                listing.Append(record)
                    .Append("' - expected minimum ").Append(MinFieldCountAlt)
                    .Append(" fields but found ").Append(values.Length).Append(" fields");
                throw new FormatException(listing.ToString());
            }

            // field pos
            var index = 0;

            // first field is perms
            var permissions = values[index++];
            ch = permissions[0];
            var isDir = false;
            var isLink = false;
            if (ch == DirectoryMarker)
            {
                isDir = true;
            }
            else if (ch == SymbolicLinkMarker)
            {
                isLink = true;
            }

            var group = values[index++];

            // some servers don't supply the link count
            var linkCount = 0;
            if (char.IsDigit(values[index][0]))
            {
                // assume it is if a digit
                var linkCountStr = values[index++];
                try
                {
                    linkCount = int.Parse(linkCountStr);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse link count: " + linkCountStr);
                }
            }

            var owner = values[index++];


            // size
            var size = 0L;
            var sizeStr = values[index++];
            try
            {
                size = long.Parse(sizeStr);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + sizeStr);
            }

            // next 3 fields are the date time

            // we expect the month first on Unix. 
            var dateTimePos = index;
            var lastModified = DateTime.MinValue;
            var stamp = new StringBuilder(values[index++]);
            stamp.Append('-').Append(values[index++]).Append('-');

            var field = values[index++];
            if (field.IndexOf((char)':') < 0)
            {
                stamp.Append(field); // year
                lastModified = ParseYear(client, stamp, DateTimeAltFormats1);
            }
            else
            {
                // add the year ourselves as not present
                var year = client.ListingCulture.Calendar.GetYear(DateTime.Now);
                stamp.Append(year).Append('-').Append(field);
                lastModified = ParseDateTime(client, stamp, DateTimeAltFormats2);
            }

            // name of file or dir. Extract symlink if possible
            string name = null;

            // find the starting point of the name by finding the pos of all the date/time fields
            var pos = 0;
            var ok = true;
            for (var i = dateTimePos; i < dateTimePos + 3; i++)
            {
                pos = record.IndexOf(values[i], pos);
                if (pos < 0)
                {
                    ok = false;
                    break;
                }
                else
                {
                    pos += values[i].Length;
                }
            }

            if (ok)
            {
                name = record.Substring(pos).Trim();
            }
            else
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            }

            // create a new list item object with the parsed metadata
            var file = new FtpListItem(record, name, size, isDir, ref lastModified);
            if (isLink)
            {
                file.Type = FtpFileSystemObjectType.Link;
                file.LinkCount = linkCount;
            }

            file.RawGroup = group;
            file.RawOwner = owner;
            file.RawPermissions = permissions;
            file.CalculateUnixPermissions(permissions);
            return file;
        }

        #region Constants

        private static string SymbolicLinkArrowMarker = "->";
        private static char SymbolicLinkMarker = 'l';
        private static char FileMarker = '-';
        private static char DirectoryMarker = 'd';
        private static int MinFieldCount = 7;
        private static int MinFieldCountAlt = 8;
        private static string[] DateTimeFormats1 = { "MMM'-'d'-'yyyy", "MMM'-'dd'-'yyyy" };
        private static string[] DateTimeFormats2 = { "MMM'-'d'-'yyyy'-'HH':'mm", "MMM'-'dd'-'yyyy'-'HH':'mm", "MMM'-'d'-'yyyy'-'H':'mm", "MMM'-'dd'-'yyyy'-'H':'mm", "MMM'-'dd'-'yyyy'-'H'.'mm" };
        private static string[] DateTimeAltFormats1 = { "MMM'-'d'-'yyyy", "MMM'-'dd'-'yyyy" };
        private static string[] DateTimeAltFormats2 = { "MMM'-'d'-'yyyy'-'HH':'mm:ss", "MMM'-'dd'-'yyyy'-'HH':'mm:ss", "MMM'-'d'-'yyyy'-'H':'mm:ss", "MMM'-'dd'-'yyyy'-'H':'mm:ss" };

        #endregion
    }
    internal static class FtpVMSParser
    {

        /// <summary>
        /// Checks if the given listing is a valid VMS file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] records)
        {
            var count = Math.Min(records.Length, 10);

            var semiColonName = false;
            bool squareBracketStart = false, squareBracketEnd = false;

            for (var i = 0; i < count; i++)
            {
                var record = records[i];
                if (record.Trim().Length == 0)
                {
                    continue;
                }

                var pos = 0;
                if ((pos = record.IndexOf(';')) > 0 && ++pos < record.Length &&
                    char.IsDigit(record[pos]))
                {
                    semiColonName = true;
                }

                if (record.Contains('['))
                {
                    squareBracketStart = true;
                }

                if (record.Contains(']'))
                {
                    squareBracketEnd = true;
                }
            }

            if (semiColonName && squareBracketStart && squareBracketEnd)
            {
                return true;
            }

            client.LogStatus(FtpTraceLevel.Verbose, "Not in VMS format");
            return false;
        }

        /// <summary>
        /// Parses Vax/VMS format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record)
        {
            var values = record.SplitString();

            // skip blank lines
            if (values.Length <= 0)
            {
                return null;
            }

            // skip line which lists Directory
            if (values.Length >= 2 && values[0].Equals(HDirectoryMarker))
            {
                return null;
            }

            // skip line which lists Total
            if (values.Length > 0 && values[0].Equals(TotalMarker))
            {
                return null;
            }

            if (values.Length < MinFieldCount)
            {
                return null;
            }

            // first field is name
            var name = values[0];

            // make sure it is the name (ends with ';<INT>')
            var semiPos = name.LastIndexOf(';');

            // check for ;
            if (semiPos <= 0)
            {
                client.LogStatus(FtpTraceLevel.Verbose, "File version number not found in name '" + name + "'");
                return null;
            }

            var nameNoVersion = name.Substring(0, semiPos);

            // check for version after ;
            var afterSemi = values[0].Substring(semiPos + 1);
            try
            {
                long.Parse(afterSemi);

                // didn't throw exception yet, must be number
                // we don't use it currently but we might in future
            }
            catch (FormatException)
            {
                // don't worry about version number
            }

            // test is dir
            var isDir = false;
            if (nameNoVersion.EndsWith(DirectoryMarker))
            {
                isDir = true;
                name = nameNoVersion.Substring(0, nameNoVersion.Length - DirectoryMarker.Length);
            }

            if (!FtpListParser.VMSNameHasVersion && !isDir)
            {
                name = nameNoVersion;
            }

            // 2nd field is size USED/ALLOCATED format, or perhaps just USED
            var size = ParseFileSize(values[1]);

            // 3 & 4 fields are date time
            var lastModified = ParseDateTime(client, values[2], values[3]);

            // 5th field is [group,owner]
            string group = null;
            string owner = null;
            ParseGroupOwner(values, out group, out owner);

            // 6th field is permissions e.g. (RWED,RWED,RE,)
            var permissions = ParsePermissions(values);

            // create a new list item object with the parsed metadata
            var file = new FtpListItem(record, name, size, isDir, ref lastModified);
            file.RawGroup = group;
            file.RawOwner = owner;
            file.RawPermissions = permissions;
            return file;
        }

        /// <summary>
        /// Parses the file size from Vax/VMS format listings
        /// </summary>
        private static long ParseFileSize(string sizeStr)
        {
            long size;
            var slashPos = sizeStr.IndexOf('/');
            var sizeUsed = sizeStr;
            if (slashPos == -1)
            {
                // only filesize in bytes
                size = long.Parse(sizeStr);
            }
            else
            {
                if (slashPos > 0)
                {
                    sizeUsed = sizeStr.Substring(0, slashPos);
                }

                size = long.Parse(sizeUsed) * FileBlockSize;
            }

            return size;
        }

        /// <summary>
        /// Parses the owner and group permissions from Vax/VMS format listings
        /// </summary>
        private static void ParseGroupOwner(string[] values, out string group, out string owner)
        {
            group = null;
            owner = null;
            if (values.Length >= 5)
            {
                if (values[4][0] == '[' && values[4][values[4].Length - 1] == ']')
                {
                    var commaPos = values[4].IndexOf(',');
                    if (commaPos < 0)
                    {
                        owner = values[4].Substring(1, values[4].Length - 2);
                        group = "";
                    }
                    else
                    {
                        group = values[4].Substring(1, commaPos - 1);
                        owner = values[4].Substring(commaPos + 1, values[4].Length - commaPos - 2);
                    }
                }
            }
        }

        /// <summary>
        /// Parses the permissions from Vax/VMS format listings
        /// </summary>
        private static string ParsePermissions(string[] values)
        {
            if (values.Length >= 6)
            {
                if (values[5][0] == '(' && values[5][values[5].Length - 1] == ')')
                {
                    return values[5].Substring(1, values[5].Length - 2);
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the last modified date from Vax/VMS format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, string date, string time)
        {
            var sb = new StringBuilder();
            var monthFound = false;

            // add date
            for (var i = 0; i < date.Length; i++)
            {
                if (!char.IsLetter(date[i]))
                {
                    sb.Append(date[i]);
                }
                else
                {
                    if (!monthFound)
                    {
                        sb.Append(date[i]);
                        monthFound = true;
                    }
                    else
                    {
                        // convert the last 2 chars of month to lower case
                        sb.Append(char.ToLower(date[i]));
                    }
                }
            }

            // add time
            sb.Append(" ").Append(time);
            var lastModifiedStr = sb.ToString();

            // parse it into a date/time object
            try
            {
                var lastModified = DateTime.Parse(lastModifiedStr, client.ListingCulture.DateTimeFormat);
                return lastModified;
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + lastModifiedStr + "'");
            }

            return DateTime.MinValue;
        }

        #region Constants

        private static string DirectoryMarker = ".DIR";
        private static string HDirectoryMarker = "Directory";
        private static string TotalMarker = "Total";
        private static int MinFieldCount = 4;
        private static int FileBlockSize = 512 * 1024;

        #endregion
    }
    internal static class FtpWindowsParser
    {

        /// <summary>
        /// Checks if the given listing is a valid IIS/DOS file listing
        /// </summary>
        public static bool IsValid(FtpClient client, string[] records)
        {
            var count = Math.Min(records.Length, 10);

            var dateStart = false;
            var timeColon = false;
            var dirOrFile = false;

            for (var i = 0; i < count; i++)
            {
                var record = records[i];
                if (record.Trim().Length == 0)
                {
                    continue;
                }

                var values = record.SplitString();
                if (values.Length < MinFieldCount)
                {
                    continue;
                }

                // first & last chars are digits of first field
                if (char.IsDigit(values[0][0]) && char.IsDigit(values[0][values[0].Length - 1]))
                {
                    dateStart = true;
                }

                if (values[1].IndexOf(':') > 0)
                {
                    timeColon = true;
                }

                if (values[2].ToUpper() == DirectoryMarker || char.IsDigit(values[2][0]))
                {
                    dirOrFile = true;
                }
            }

            if (dateStart && timeColon && dirOrFile)
            {
                return true;
            }

            client.LogStatus(FtpTraceLevel.Verbose, "Not in Windows format");
            return false;
        }

        /// <summary>
        /// Parses IIS/DOS format listings
        /// </summary>
        /// <param name="client">The FTP client</param>
        /// <param name="record">A line from the listing</param>
        /// <returns>FtpListItem if the item is able to be parsed</returns>
        public static FtpListItem Parse(FtpClient client, string record)
        {
            var values = record.SplitString();

            if (values.Length < MinFieldCount)
            {
                return null;
            }

            // parse date & time
            var lastModified = ParseDateTime(client, values[0] + " " + values[1]);

            // parse dir flag & file size
            bool isDir;
            long size;
            ParseTypeAndFileSize(client, values[2], out isDir, out size);

            // parse name of file or folder
            var name = ParseName(client, record, values);

            return new FtpListItem(record, name, size, isDir, ref lastModified);
        }

        /// <summary>
        /// Parses the file or folder name from IIS/DOS format listings
        /// </summary>
        private static string ParseName(FtpClient client, string record, string[] values)
        {
            // Find starting point of the name by finding the pos of all the date/time fields.
            var pos = 0;
            var ok = true;
            for (var i = 0; i < 3; i++)
            {
                pos = record.IndexOf(values[i], pos);
                if (pos < 0)
                {
                    ok = false;
                    break;
                }
                else
                {
                    pos += values[i].Length;
                }
            }

            string name = null;
            if (ok)
            {
                name = record.Substring(pos).Trim();
            }
            else
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            }

            return name;
        }

        /// <summary>
        /// Parses the file size and checks if the item is a directory from IIS/DOS format listings
        /// </summary>
        private static void ParseTypeAndFileSize(FtpClient client, string type, out bool isDir, out long size)
        {
            isDir = false;
            size = 0L;
            if (type.ToUpper().Equals(DirectoryMarker.ToUpper()))
            {
                isDir = true;
            }
            else
            {
                try
                {
                    size = long.Parse(type);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + type);
                }
            }
        }

        /// <summary>
        /// Parses the last modified date from IIS/DOS format listings
        /// </summary>
        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            try
            {
                var lastModified = DateTime.ParseExact(lastModifiedStr, DateTimeFormats, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
                return lastModified;
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + lastModifiedStr + "'");
            }

            return DateTime.MinValue;
        }

        #region Constants

        private static string DirectoryMarker = "<DIR>";
        private static int MinFieldCount = 4;
        private static string[] DateTimeFormats = { "MM'-'dd'-'yy hh':'mmtt", "MM'-'dd'-'yy HH':'mm", "MM'-'dd'-'yyyy hh':'mmtt" };

        #endregion
    }
    #endregion // Parsers
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    internal static class Collections
    {

        /// <summary>
        /// Checks if the array is null or 0 length.
        /// </summary>
        public static bool IsBlank(this IList value)
        {
            return value == null || value.Count == 0;
        }

        /// <summary>
        /// Checks if the array is null or 0 length.
        /// </summary>
        public static bool IsBlank(this IEnumerable value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is IList)
            {
                return ((IList)value).Count == 0;
            }

            if (value is byte[])
            {
                return ((byte[])value).Length == 0;
            }

            return false;
        }

        /// <summary>
        /// Adds a prefix to the given strings, returns a new array.
        /// </summary>
        public static List<string> ItemsToString(this object[] args)
        {
            var results = new List<string>();
            if (args == null)
            {
                return results;
            }

            foreach (var v in args)
            {
                string txt;
                if (v == null)
                {
                    txt = "null";
                }
                else if (v is string)
                {
                    txt = "\"" + v as string + "\"";
                }
                else
                {
                    txt = v.ToString();
                }

                results.Add(txt);
            }

            return results;
        }

        /// <summary>
        /// Ensures the given item is only added once. If it was not present true is returned, else false is returned.
        /// </summary>
        public static bool AddOnce<T>(this List<T> items, T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                return true;
            }
            return false;
        }


    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class DateTimes
    {

        /// <summary>
        /// Converts the FTP date string into a DateTime object, without performing any timezone conversion.
        /// </summary>
        /// <param name="dateString">The date string</param>
        /// <param name="formats">Date formats to try parsing the value from (eg "yyyyMMddHHmmss")</param>
        /// <returns>A <see cref="DateTime"/> object representing the date, or <see cref="DateTime.MinValue"/> if there was a problem</returns>
        public static DateTime ParseFtpDate(this string dateString, FtpClient client, string[] formats = null)
        {
            if (formats == null)
            {
                formats = FtpDateFormats;
            }

            // parse the raw timestamp without performing any timezone conversions
            try
            {
                DateTime date = DateTime.ParseExact(dateString, FtpDateFormats, client.ListingCulture.DateTimeFormat, DateTimeStyles.None); // or client.ListingCulture.DateTimeFormat

                return date;
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + dateString + "'");
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Generates an FTP date-string from the DateTime object, without performing any timezone conversion.
        /// </summary>
        /// <param name="date">The date value</param>
        /// <returns>A string representing the date</returns>
        public static string GenerateFtpDate(this DateTime date)
        {

            // generate final pretty printed date
            var timeStr = date.ToString("yyyyMMddHHmmss");
            return timeStr;
        }

        private static string[] FtpDateFormats = { "yyyyMMddHHmmss", "yyyyMMddHHmmss'.'f", "yyyyMMddHHmmss'.'ff", "yyyyMMddHHmmss'.'fff", "MMM dd  yyyy", "MMM  d  yyyy", "MMM dd HH:mm", "MMM  d HH:mm" };

    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class Enums
    {

        /// <summary>
        /// Validates that the FtpError flags set are not in an invalid combination.
        /// </summary>
        /// <param name="options">The error handling options set</param>
        /// <returns>True if a valid combination, otherwise false</returns>
        public static bool IsValidCombination(this FtpError options)
        {
            return options != (FtpError.Stop | FtpError.Throw) &&
                   options != (FtpError.Throw | FtpError.Stop | FtpError.DeleteProcessed);
        }

        /// <summary>
        /// Checks if the operation was successful or skipped (indicating success).
        /// </summary>
        public static bool IsSuccess(this FtpStatus status)
        {
            return status == FtpStatus.Success || status == FtpStatus.Skipped;
        }

        /// <summary>
        /// Checks if the operation has failed.
        /// </summary>
        public static bool IsFailure(this FtpStatus status)
        {
            return status == FtpStatus.Failed;
        }

    }

    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    internal static class FileListings
    {


        /// <summary>
        /// Checks if the given file exists in the given file listing.
        /// Supports servers that return:  1) full paths,  2) only filenames,  3) full paths without slash prefixed,  4) full paths with invalid slashes
        /// </summary>
        /// <param name="fileList">The listing returned by GetNameListing</param>
        /// <param name="path">The full file path you want to check</param>
        /// <returns></returns>
        public static bool FileExistsInNameListing(string[] fileList, string path)
        {
            // exit quickly if no paths
            if (fileList.Length == 0)
            {
                return false;
            }

            // cleanup file path, get file name
            var pathName = path.GetFtpFileName();
            var pathPrefixed = path.EnsurePrefix("/");


            // FAST MODE

            // per entry in the name list
            foreach (var fileListEntry in fileList)
            {
                // support servers that return:  1) full paths,  2) only filenames,  3) full paths without slash prefixed
                if (fileListEntry == pathName || fileListEntry == path || fileListEntry.EnsurePrefix("/") == pathPrefixed)
                {
                    return true;
                }
            }


            // SLOW MODE 1

            // per entry in the name list
            foreach (var fileListEntry in fileList)
            {
                // support servers that return:  2) only filenames
                if (fileListEntry.GetFtpFileName() == pathName)
                {
                    return true;
                }
            }


            // SLOW MODE 2
            // Fix #745: FileExists returns false when file exists [Windows NT Server]

            // per entry in the name list
            foreach (var fileListEntry in fileList)
            {
                // support servers that return:  4) full paths with invalid slashes
                if (fileListEntry.GetFtpPath() == path)
                {
                    return true;
                }
            }


            return false;
        }

        /// <summary>
        /// Checks if the given file exists in the given file listing.
        /// </summary>
        /// <param name="fileList">The listing returned by GetListing</param>
        /// <param name="path">The full file path you want to check</param>
        /// <returns></returns>
        public static bool FileExistsInListing(FtpListItem[] fileList, string path)
        {
            // exit quickly if no paths
            if (fileList == null || fileList.Length == 0)
            {
                return false;
            }

            // cleanup file path, get file name
            var trimSlash = new char[] { '/' };
            var pathClean = path.Trim(trimSlash);

            // per entry in the list
            foreach (var fileListEntry in fileList)
            {
                if (fileListEntry.FullName.Trim(trimSlash) == pathClean)
                {
                    return true;
                }
            }

            return false;
        }


    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class FileSizes
    {

        /// <summary>
        /// Converts a file size in bytes to a string representation (eg. 12345 becomes 12.3 KB)
        /// </summary>
        public static string FileSizeToString(this int bytes)
        {
            return ((long)bytes).FileSizeToString();
        }

        /// <summary>
        /// Converts a file size in bytes to a string representation (eg. 12345 becomes 12.3 KB)
        /// </summary>
        public static string FileSizeToString(this uint bytes)
        {
            return ((long)bytes).FileSizeToString();
        }

        /// <summary>
        /// Converts a file size in bytes to a string representation (eg. 12345 becomes 12.3 KB)
        /// </summary>
        public static string FileSizeToString(this ulong bytes)
        {
            return ((long)bytes).FileSizeToString();
        }

        /// <summary>
        /// Converts a file size in bytes to a string representation (eg. 12345 becomes 12.3 KB)
        /// </summary>
        public static string FileSizeToString(this long bytes)
        {
            var order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizePostfix.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.#} {1}", len, sizePostfix[order]);
        }
        private static string[] sizePostfix = { "bytes", "KB", "MB", "GB", "TB" };


    }
    /// <summary>
    /// Parses a line from a file listing using the first successful parser, or the specified parser.
    /// Returns an FtpListItem object representing the parsed line, or null if the line was unable to be parsed.
    /// </summary>
    public class FtpListParser
    {
        #region Internal API

        /// <summary>
        /// the FTP connection that owns this parser
        /// </summary>
        public FtpClient client;

        private static List<FtpParser> parsers = new List<FtpParser> {
            FtpParser.Unix, FtpParser.Windows, FtpParser.VMS, FtpParser.IBMzOS, FtpParser.IBMOS400, FtpParser.NonStop
        };

        /// <summary>
        /// current parser, or parser set by user
        /// </summary>
        public FtpParser CurrentParser = FtpParser.Auto;

        /// <summary>
        /// parser calculated based on system type (SYST command)
        /// </summary>
        public FtpParser DetectedParser = FtpParser.Auto;

        /// <summary>
        /// if we have detected that the current parser is valid
        /// </summary>
        public bool ParserConfirmed = false;

        /// <summary>
        /// Is the version number returned as part of the filename?
        /// 
        /// Some VMS FTP servers do not permit a file to be deleted unless
        /// the filename includes the version number. Note that directories are
        /// never returned with the version number.
        /// </summary>
        public static bool VMSNameHasVersion = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpListParser"/> class.
        /// </summary>
        /// <param name="client">An existing <see cref="FtpClient"/> object</param>
        public FtpListParser(FtpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Try to auto-detect which parser is suitable given a system string.
        /// </summary>
        public void Init(FtpOperatingSystem system, FtpParser forcedParser = FtpParser.Auto)
        {
            ParserConfirmed = false;

            if (forcedParser != FtpParser.Auto)
            {
                // use the parser that the server handler specified
                CurrentParser = forcedParser;
            }
            else
            {

                if (system == FtpOperatingSystem.Windows)
                {
                    CurrentParser = FtpParser.Windows;
                }
                else if (system == FtpOperatingSystem.Unix || system == FtpOperatingSystem.SunOS)
                {
                    CurrentParser = FtpParser.Unix;
                }
                else if (system == FtpOperatingSystem.VMS)
                {
                    CurrentParser = FtpParser.VMS;
                }
                else if (system == FtpOperatingSystem.IBMzOS)
                {
                    CurrentParser = FtpParser.IBMzOS;
                }
                else if (system == FtpOperatingSystem.IBMOS400)
                {
                    CurrentParser = FtpParser.IBMOS400;
                }
                else
                {
                    CurrentParser = FtpParser.Unix;
                    client.LogStatus(FtpTraceLevel.Warn, "Cannot auto-detect listing parser for system '" + system + "', using Unix parser");
                }
            }

            DetectedParser = CurrentParser;

            client.LogStatus(FtpTraceLevel.Verbose, "Listing parser set to: " + DetectedParser.ToString());
        }

        /// <summary>
        /// Parse raw file from server into a file object, using the currently active parser.
        /// </summary>
        public FtpListItem ParseSingleLine(string path, string file, List<FtpCapability> caps, bool isMachineList)
        {
            FtpListItem result = null;

            // force machine listing if it is
            if (isMachineList)
            {
                result = FtpMachineListParser.Parse(file, caps, client);
            }
            else
            {
                // use custom parser if given
                if (client.ListingParser == FtpParser.Custom && client.ListingCustomParser != null)
                {
                    result = client.ListingCustomParser(file, caps, client);
                }
                else
                {
                    if (IsWrongParser())
                    {
                        ValidateParser(new[] { file });
                    }

                    // use one of the in-built parsers
                    switch (CurrentParser)
                    {
                        case FtpParser.Machine:
                            result = FtpMachineListParser.Parse(file, caps, client);
                            break;

                        case FtpParser.Windows:
                            result = FtpWindowsParser.Parse(client, file);
                            break;

                        case FtpParser.Unix:
                            result = FtpUnixParser.Parse(client, file);
                            break;

                        case FtpParser.UnixAlt:
                            result = FtpUnixParser.ParseUnixAlt(client, file);
                            break;

                        case FtpParser.VMS:
                            result = FtpVMSParser.Parse(client, file);
                            break;

                        case FtpParser.IBMzOS:
                            result = FtpIBMzOSParser.Parse(client, file, path);
                            break;

                        case FtpParser.IBMOS400:
                            result = FtpIBMOS400Parser.Parse(client, file);
                            break;

                        case FtpParser.NonStop:
                            result = FtpNonStopParser.Parse(client, file);
                            break;
                    }
                }
            }

            // if parsed file successfully
            if (result != null)
            {

                // process created date into the timezone required
                result.RawCreated = result.Created;
                if (result.Created != DateTime.MinValue)
                {
                    result.Created = client.ConvertDate(result.Created);
                }

                // process modified date into the timezone required
                result.RawModified = result.Modified;
                if (result.Modified != DateTime.MinValue)
                {
                    result.Modified = client.ConvertDate(result.Modified);
                }

                // calc absolute file paths
                result.CalculateFullFtpPath(client, path);
            }

            return result;
        }

        /// <summary>
        /// Validate if the current parser is correct, or if another parser seems more appropriate.
        /// </summary>
        private void ValidateParser(string[] files)
        {
            if (IsWrongParser())
            {
                // by default use the UNIX parser, if none detected
                if (DetectedParser == FtpParser.Auto)
                {
                    DetectedParser = FtpParser.Unix;
                }

                if (CurrentParser == FtpParser.Auto)
                {
                    CurrentParser = DetectedParser;
                }

                // if machine listings not supported, switch to UNIX parser
                if (IsWrongMachineListing())
                {
                    CurrentParser = DetectedParser;
                }

                // use the initially set parser (from SYST)
                if (IsParserValid(CurrentParser, files))
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Confirmed format " + CurrentParser.ToString());
                    ParserConfirmed = true;
                    return;
                }

                foreach (var p in parsers)
                {
                    if (IsParserValid(p, files))
                    {
                        CurrentParser = p;
                        client.LogStatus(FtpTraceLevel.Verbose, "Detected format " + CurrentParser.ToString());
                        ParserConfirmed = true;
                        return;
                    }
                }

                CurrentParser = FtpParser.Unix;
                client.LogStatus(FtpTraceLevel.Verbose, "Could not detect format. Using default " + CurrentParser.ToString());
            }
        }

        private bool IsWrongParser()
        {
            return CurrentParser == FtpParser.Auto || !ParserConfirmed || IsWrongMachineListing();
        }

        private bool IsWrongMachineListing()
        {
            return CurrentParser == FtpParser.Machine && client != null && !client.HasFeature(FtpCapability.MLSD);
        }

        /// <summary>
        /// Validate if the current parser is correct
        /// </summary>
        private bool IsParserValid(FtpParser p, string[] files)
        {
            switch (p)
            {
                case FtpParser.Windows:
                    return FtpWindowsParser.IsValid(client, files);

                case FtpParser.Unix:
                    return FtpUnixParser.IsValid(client, files);

                case FtpParser.VMS:
                    return FtpVMSParser.IsValid(client, files);

                case FtpParser.IBMzOS:
                    return FtpIBMzOSParser.IsValid(client, files);

                case FtpParser.IBMOS400:
                    return FtpIBMOS400Parser.IsValid(client, files);

                case FtpParser.NonStop:
                    return FtpNonStopParser.IsValid(client, files);
            }

            return false;
        }

        #endregion

    }
    /// <summary>
    /// Used for transaction logging and debug information.
    /// </summary>
    public static class FtpTrace
    {
#if !NETFx
        private static volatile TraceSource m_traceSource = new TraceSource("FluentFTP")
        {
            Switch = new SourceSwitch("sourceSwitch", "Verbose") { Level = SourceLevels.All }
        };

        private static bool m_flushOnWrite = true;


        /// <summary>
        /// Should the trace listeners be flushed immediately after writing to them?
        /// </summary>
        public static bool FlushOnWrite
        {
            get => m_flushOnWrite;
            set => m_flushOnWrite = value;
        }

        private static bool m_prefix = false;

        /// <summary>
        /// Should the log entries be written with a prefix of "FluentFTP"?
        /// Useful if you have a single TraceListener shared across multiple libraries.
        /// </summary>
        public static bool LogPrefix
        {
            get => m_prefix;
            set => m_prefix = value;
        }


        /// <summary>
        /// Add a TraceListner to the collection. You can use one of the predefined
        /// TraceListeners in the System.Diagnostics namespace, such as ConsoleTraceListener
        /// for logging to the console, or you can write your own deriving from 
        /// System.Diagnostics.TraceListener.
        /// </summary>
        /// <param name="listener">The TraceListener to add to the collection</param>
        public static void AddListener(TraceListener listener)
        {
            lock (m_traceSource)
            {
                m_traceSource.Listeners.Add(listener);
            }
        }

        /// <summary>
        /// Remove the specified TraceListener from the collection
        /// </summary>
        /// <param name="listener">The TraceListener to remove from the collection.</param>
        public static void RemoveListener(TraceListener listener)
        {
            lock (m_traceSource)
            {
                m_traceSource.Listeners.Remove(listener);
            }
        }

#endif

        private static bool m_LogToConsole = false;

        /// <summary>
        /// Should FTP communication be logged to console?
        /// </summary>
        public static bool LogToConsole
        {
            get => m_LogToConsole;
            set => m_LogToConsole = value;
        }

        private static string m_LogToFile = null;

        /// <summary>
        /// Set this to a file path to append all FTP communication to it.
        /// </summary>
        public static string LogToFile
        {
            get => m_LogToFile;
            set => m_LogToFile = value;
        }

        private static bool m_functions = true;

        /// <summary>
        /// Should the function calls be logged in Verbose mode?
        /// </summary>
        public static bool LogFunctions
        {
            get => m_functions;
            set => m_functions = value;
        }

        private static bool m_IP = true;

        /// <summary>
        /// Should the FTP server IP addresses be included in the logs?
        /// </summary>
        public static bool LogIP
        {
            get => m_IP;
            set => m_IP = value;
        }

        private static bool m_username = true;

        /// <summary>
        /// Should the FTP usernames be included in the logs?
        /// </summary>
        public static bool LogUserName
        {
            get => m_username;
            set => m_username = value;
        }

        private static bool m_password = false;

        /// <summary>
        /// Should the FTP passwords be included in the logs?
        /// </summary>
        public static bool LogPassword
        {
            get => m_password;
            set => m_password = value;
        }

        private static bool m_tracing = true;

        /// <summary>
        /// Should we trace at all?
        /// </summary>
        public static bool EnableTracing
        {
            get => m_tracing;
            set => m_tracing = value;
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="message">The message to write</param>

        //[Obsolete("Use overloads with FtpTraceLevel")]
        public static void Write(string message)
        {
            Write(FtpTraceLevel.Verbose, message);
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="message">The message to write</param>

        //[Obsolete("Use overloads with FtpTraceLevel")]
        public static void WriteLine(object message)
        {
            Write(FtpTraceLevel.Verbose, message.ToString());
        }

        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">The message to write</param>
        public static void WriteLine(FtpTraceLevel eventType, object message)
        {
            Write(eventType, message.ToString());
        }

        /// <summary>
        /// Write to the TraceListeners, for the purpose of logging a API function call
        /// </summary>
        /// <param name="function">The name of the API function</param>
        /// <param name="args">The args passed to the function</param>
        public static void WriteFunc(string function, object[] args = null)
        {
            if (m_functions)
            {
                Write(FtpTraceLevel.Verbose, "");
                Write(FtpTraceLevel.Verbose, "# " + function + "(" + args.ItemsToString().Join(", ") + ")");
            }
        }


        /// <summary>
        /// Write to the TraceListeners
        /// </summary>
        /// <param name="eventType">The type of tracing event</param>
        /// <param name="message">A formattable string to write</param>
        public static void Write(FtpTraceLevel eventType, string message)
        {
            if (!EnableTracing)
            {
                return;
            }

#if DEBUG
            Debug.WriteLine(message);
#else
			if (m_LogToConsole) {
				Console.WriteLine(message);
			}

			if (m_LogToFile != null) {
				File.AppendAllText(m_LogToFile, message + "\n");
			}
#endif

#if !NETFx

            if (m_prefix)
            {
                // if prefix is wanted then use TraceEvent()
                m_traceSource.TraceEvent(TraceLevelTranslation(eventType), 0, message);
            }
            else
            {
                // if prefix is NOT wanted then write manually
                EmitEvent(m_traceSource, TraceLevelTranslation(eventType), message);
            }

            if (m_flushOnWrite)
            {
                m_traceSource.Flush();
            }

#endif
        }


#if !NETFx

        private static TraceEventType TraceLevelTranslation(FtpTraceLevel level)
        {
            switch (level)
            {
                case FtpTraceLevel.Verbose:
                    return TraceEventType.Verbose;

                case FtpTraceLevel.Info:
                    return TraceEventType.Information;

                case FtpTraceLevel.Warn:
                    return TraceEventType.Warning;

                case FtpTraceLevel.Error:
                    return TraceEventType.Error;

                default:
                    return TraceEventType.Verbose;
            }
        }

        private static object traceSync = new object();

        private static void EmitEvent(TraceSource traceSource, TraceEventType eventType, string message)
        {
            try
            {
                lock (traceSync)
                {
                    if (traceSource.Switch.ShouldTrace(eventType))
                    {
                        foreach (TraceListener listener in traceSource.Listeners)
                        {
                            try
                            {
                                listener.WriteLine(message);
                                listener.Flush();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

#endif
    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class LocalPaths
    {

        /// <summary>
        /// Returns true if the given path is a directory path.
        /// </summary>
        public static bool IsLocalFolderPath(string localPath)
        {
            return localPath.EndsWith("/") || localPath.EndsWith("\\") || Directory.Exists(localPath);
        }

        /// <summary>
        /// Ensures the given directory exists.
        /// </summary>
        public static bool EnsureDirectory(this string localPath)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Combine the given base path with the relative path
        /// </summary>
        public static string CombineLocalPath(this string path, string fileOrFolder)
        {

            string directorySeperator = Path.DirectorySeparatorChar.ToString();

            // fast mode if there is exactly one slash between path & file
            var pathHasSep = path.EndsWith(directorySeperator);
            var fileHasSep = fileOrFolder.StartsWith(directorySeperator);
            if ((pathHasSep && !fileHasSep) || (!pathHasSep && fileHasSep))
            {
                return path + fileOrFolder;
            }

            // slow mode if slashes need to be fixed
            if (pathHasSep && fileHasSep)
            {
                return path + fileOrFolder.Substring(1);
            }
            if (!pathHasSep && !fileHasSep)
            {
                return path + directorySeperator + fileOrFolder;
            }

            // nothing
            return null;
        }

    }
    /// <summary>
    /// The local ports.
    /// </summary>
    internal static class LocalPorts
    {
#if !NET40
        internal static readonly Random randomGen = new Random();

		/// <summary>
		/// Get random local port for the given local IP address
		/// </summary>
		public static int GetRandomAvailable(IPAddress localIpAddress) {
			lock (randomGen) {
				var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
				var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
				var inUsePorts = new HashSet<int>(
					tcpConnInfoArray.Where(ipEndPoint => localIpAddress.Equals(ipEndPoint.Address))
						.Select(ipEndPoint => ipEndPoint.Port));
				int localPort;
				do {
					localPort = 1025 + randomGen.Next(32000);
				}
				while (inUsePorts.Contains(localPort));

				return localPort;
			}
		}
#endif
    }
    internal static class Operators
    {


        public static bool Validate(FtpOperator op, int value, int x, int y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return value == x;
                case FtpOperator.NotEquals:
                    return value != x;
                case FtpOperator.LessThan:
                    return value < x;
                case FtpOperator.LessThanOrEquals:
                    return value <= x;
                case FtpOperator.MoreThan:
                    return value > x;
                case FtpOperator.MoreThanOrEquals:
                    return value >= x;
                case FtpOperator.BetweenRange:
                    return value >= x && value <= y;
                case FtpOperator.OutsideRange:
                    return value < x || value > y;
            }
            return false;
        }

        public static bool Validate(FtpOperator op, double value, double x, double y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return value == x;
                case FtpOperator.NotEquals:
                    return value != x;
                case FtpOperator.LessThan:
                    return value < x;
                case FtpOperator.LessThanOrEquals:
                    return value <= x;
                case FtpOperator.MoreThan:
                    return value > x;
                case FtpOperator.MoreThanOrEquals:
                    return value >= x;
                case FtpOperator.BetweenRange:
                    return value >= x && value <= y;
                case FtpOperator.OutsideRange:
                    return value < x || value > y;
            }
            return false;
        }

        public static bool Validate(FtpOperator op, long value, long x, long y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return value == x;
                case FtpOperator.NotEquals:
                    return value != x;
                case FtpOperator.LessThan:
                    return value < x;
                case FtpOperator.LessThanOrEquals:
                    return value <= x;
                case FtpOperator.MoreThan:
                    return value > x;
                case FtpOperator.MoreThanOrEquals:
                    return value >= x;
                case FtpOperator.BetweenRange:
                    return value >= x && value <= y;
                case FtpOperator.OutsideRange:
                    return value < x || value > y;
            }
            return false;
        }
    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    internal static class Permissions
    {

        /// <summary>
        /// Calculates the CHMOD value from the permissions flags
        /// </summary>
        public static void CalculateChmod(this FtpListItem item)
        {
            item.Chmod = CalcChmod(item.OwnerPermissions, item.GroupPermissions, item.OthersPermissions);
        }

        /// <summary>
        /// Calculates the permissions flags from the CHMOD value
        /// </summary>
        public static void CalculateUnixPermissions(this FtpListItem item, string permissions)
        {
            var perms = Regex.Match(permissions,
                @"[\w-]{1}(?<owner>[\w-]{3})(?<group>[\w-]{3})(?<others>[\w-]{3})",
                RegexOptions.IgnoreCase);

            if (perms.Success)
            {
                if (perms.Groups["owner"].Value.Length == 3)
                {
                    if (perms.Groups["owner"].Value[0] == 'r')
                    {
                        item.OwnerPermissions |= FtpPermission.Read;
                    }

                    if (perms.Groups["owner"].Value[1] == 'w')
                    {
                        item.OwnerPermissions |= FtpPermission.Write;
                    }

                    if (perms.Groups["owner"].Value[2] == 'x' || perms.Groups["owner"].Value[2] == 's')
                    {
                        item.OwnerPermissions |= FtpPermission.Execute;
                    }

                    if (perms.Groups["owner"].Value[2] == 's' || perms.Groups["owner"].Value[2] == 'S')
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.SetUserID;
                    }
                }

                if (perms.Groups["group"].Value.Length == 3)
                {
                    if (perms.Groups["group"].Value[0] == 'r')
                    {
                        item.GroupPermissions |= FtpPermission.Read;
                    }

                    if (perms.Groups["group"].Value[1] == 'w')
                    {
                        item.GroupPermissions |= FtpPermission.Write;
                    }

                    if (perms.Groups["group"].Value[2] == 'x' || perms.Groups["group"].Value[2] == 's')
                    {
                        item.GroupPermissions |= FtpPermission.Execute;
                    }

                    if (perms.Groups["group"].Value[2] == 's' || perms.Groups["group"].Value[2] == 'S')
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.SetGroupID;
                    }
                }

                if (perms.Groups["others"].Value.Length == 3)
                {
                    if (perms.Groups["others"].Value[0] == 'r')
                    {
                        item.OthersPermissions |= FtpPermission.Read;
                    }

                    if (perms.Groups["others"].Value[1] == 'w')
                    {
                        item.OthersPermissions |= FtpPermission.Write;
                    }

                    if (perms.Groups["others"].Value[2] == 'x' || perms.Groups["others"].Value[2] == 't')
                    {
                        item.OthersPermissions |= FtpPermission.Execute;
                    }

                    if (perms.Groups["others"].Value[2] == 't' || perms.Groups["others"].Value[2] == 'T')
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.Sticky;
                    }
                }

                CalculateChmod(item);
            }
        }

        /// <summary>
        /// Calculate the CHMOD integer value given a set of permissions.
        /// </summary>
        public static int CalcChmod(FtpPermission owner, FtpPermission group, FtpPermission other)
        {
            var chmod = 0;

            if (HasPermission(owner, FtpPermission.Read))
            {
                chmod += 400;
            }

            if (HasPermission(owner, FtpPermission.Write))
            {
                chmod += 200;
            }

            if (HasPermission(owner, FtpPermission.Execute))
            {
                chmod += 100;
            }

            if (HasPermission(group, FtpPermission.Read))
            {
                chmod += 40;
            }

            if (HasPermission(group, FtpPermission.Write))
            {
                chmod += 20;
            }

            if (HasPermission(group, FtpPermission.Execute))
            {
                chmod += 10;
            }

            if (HasPermission(other, FtpPermission.Read))
            {
                chmod += 4;
            }

            if (HasPermission(other, FtpPermission.Write))
            {
                chmod += 2;
            }

            if (HasPermission(other, FtpPermission.Execute))
            {
                chmod += 1;
            }

            return chmod;
        }

        /// <summary>
        /// Checks if the permission value has the given flag
        /// </summary>
        private static bool HasPermission(FtpPermission owner, FtpPermission flag)
        {
            return (owner & flag) == flag;
        }

    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class RemotePaths
    {

        /// <summary>
        /// Checks if this FTP path is a top level path
        /// </summary>
        public static bool IsAbsolutePath(this string path)
        {
            return path.StartsWith("/") || path.StartsWith("./") || path.StartsWith("../");
        }

        /// <summary>
        /// Checks if the given path is a root directory or working directory path
        /// </summary>
        /// <param name="ftppath"></param>
        /// <returns></returns>
        public static bool IsFtpRootDirectory(this string ftppath)
        {
            return ftppath == "." || ftppath == "./" || ftppath == "/";
        }

        /// <summary>
        /// Converts the specified path into a valid FTP file system path.
        /// Replaces invalid back-slashes with valid forward-slashes.
        /// Replaces multiple slashes with single slashes.
        /// Removes the ending postfix slash if any.
        /// </summary>
        /// <param name="path">The file system path</param>
        /// <returns>A path formatted for FTP</returns>
        public static string GetFtpPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "/";
            }

            path = path.Replace('\\', '/');
            path = Regex.Replace(path, "[/]+", "/");
            path = path.TrimEnd('/');

            if (path.Length == 0)
            {
                path = "/";
            }

            return path;
        }

        /// <summary>
        /// Creates a valid FTP path by appending the specified segments to this string
        /// </summary>
        /// <param name="path">This string</param>
        /// <param name="segments">The path segments to append</param>
        /// <returns>A valid FTP path</returns>
        public static string GetFtpPath(this string path, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            foreach (var part in segments)
            {
                if (part != null)
                {
                    if (path.Length > 0 && !path.EndsWith("/"))
                    {
                        path += "/";
                    }

                    path += Regex.Replace(part.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
                }
            }

            path = Regex.Replace(path.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
            if (path.Length == 0)
            {
                path = "/";
            }

            return path;
        }

        /// <summary>
        /// Gets the parent directory path (formatted for a FTP server)
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The parent directory path</returns>
        public static string GetFtpDirectoryName(this string path)
        {
            var tpath = path == null ? "" : path.GetFtpPath();

            if (tpath.Length == 0 || tpath == "/")
            {
                return "/";
            }

            var lastslash = tpath.LastIndexOf('/');
            if (lastslash < 0)
            {
                return ".";
            }

            if (lastslash == 0)
            {
                return "/";
            }

            return tpath.Substring(0, lastslash);
        }

        /// <summary>
        /// Gets the file name and extension from the path.
        /// Supports paths with backslashes and forwardslashes.
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The file name</returns>
        public static string GetFtpFileName(this string path)
        {
            var tpath = path == null ? null : path;
            var lastslash = -1;

            // no change in path
            if (tpath == null)
            {
                return null;
            }

            // find the index of the right-most slash character
            lastslash = tpath.LastIndexOf('/');
            if (lastslash < 0)
            {
                lastslash = tpath.LastIndexOf('\\');
                if (lastslash < 0)
                {

                    // no change in path
                    return tpath;
                }
            }
            lastslash += 1;
            if (lastslash >= tpath.Length)
            {

                // no change in path
                return tpath;
            }

            // only return the filename and extension portion
            // skipping all the path folders
            return tpath.Substring(lastslash, tpath.Length - lastslash);
        }

        /// <summary>
        /// Converts a Windows or Unix-style path into its segments for segment-wise processing
        /// </summary>
        /// <returns></returns>
        public static string[] GetPathSegments(this string path)
        {
            if (path.Contains("/"))
            {
                return path.Split('/');
            }
            else if (path.Contains("\\"))
            {
                return path.Split('\\');
            }
            else
            {
                return new string[] { path };
            }
        }

        /// <summary>
        /// Get the full path of a given FTP Listing entry
        /// </summary>
        public static void CalculateFullFtpPath(this FtpListItem item, FtpClient client, string path)
        {
            // EXIT IF NO DIR PATH PROVIDED
            if (path == null)
            {
                // check if the path is absolute
                if (IsAbsolutePath(item.Name))
                {
                    item.FullName = item.Name;
                    item.Name = item.Name.GetFtpFileName();
                }

                return;
            }

            // ONLY IF DIR PATH PROVIDED
            if (client.ServerType == FtpServer.IBMzOSFTP &&
                client.zOSListingRealm != FtpZOSListRealm.Unix)
            {
                // The user might be using GetListing("", FtpListOption.NoPath)
                // or he might be using    GetListing("not_fully_qualified_zOS path")
                // or he might be using    GetListing("'fully_qualified_zOS path'") (note the single quotes)

                // The following examples in the comments assume a current working
                // directory of 'GEEK.'.

                // If it is not a FtpZOSListRealm.Dataset, it must be FtpZOSListRealm.Member*

                // Is caller using FtpListOption.NoPath and CWD to the right place?
                if (path.Length == 0)
                {
                    if (client.zOSListingRealm == FtpZOSListRealm.Dataset)
                    {
                        // Path: ""
                        // Fullname: 'GEEK.PROJECTS.LOADLIB'
                        item.FullName = client.GetWorkingDirectory().TrimEnd('\'') + item.Name + "\'";
                    }
                    else
                    {
                        // Path: ""
                        // Fullname: 'GEEK.PROJECTS.LOADLIB(MYPROG)'
                        item.FullName = client.GetWorkingDirectory().TrimEnd('\'') + "(" + item.Name + ")\'";
                    }
                }
                // Caller is not using FtpListOption.NoPath, so the fullname can be built
                // depending on the listing realm
                else if (path[0] == '\'')
                {
                    if (client.zOSListingRealm == FtpZOSListRealm.Dataset)
                    {
                        // Path: "'GEEK.PROJECTS.LOADLIB'"
                        // Fullname: 'GEEK.PROJECTS.LOADLIB'
                        item.FullName = item.Name;
                    }
                    else
                    {
                        // Path: "'GEEK.PROJECTS.LOADLIB(*)'"
                        // Fullname: 'GEEK.PROJECTS.LOADLIB(MYPROG)'
                        item.FullName = path.Substring(0, path.Length - 4) + "(" + item.Name + ")\'";
                    }
                }
                else
                {
                    if (client.zOSListingRealm == FtpZOSListRealm.Dataset)
                    {
                        // Path: "PROJECTS.LOADLIB"
                        // Fullname: 'GEEK.PROJECTS.LOADLIB'
                        item.FullName = client.GetWorkingDirectory().TrimEnd('\'') + item.Name + '\'';
                    }
                    else
                    {
                        // Path: "PROJECTS.LOADLIB(*)"
                        // Fullname: 'GEEK.PROJECTS.LOADLIB(MYPROG)'
                        item.FullName = client.GetWorkingDirectory().TrimEnd('\'') + path.Substring(0, path.Length - 3) + "(" + item.Name + ")\'";
                    }
                }
                return;
            }
            else if (client.ServerType == FtpServer.OpenVMS &&
                     client.ServerOS == FtpOperatingSystem.VMS)
            {
                // if this is a vax/openvms file listing
                // there are no slashes in the path name
                item.FullName = path + item.Name;
            }
            else
            {
                //this.client.LogStatus(item.Name);

                // remove globbing/wildcard from path
                if (path.GetFtpFileName().Contains("*"))
                {
                    path = path.GetFtpDirectoryName();
                }

                if (path.Length == 0)
                {
                    path = client.GetWorkingDirectory();
                }

                if (item.Name != null)
                {
                    // absolute path? then ignore the path input to this method.
                    if (IsAbsolutePath(item.Name))
                    {
                        item.FullName = item.Name;
                        item.Name = item.Name.GetFtpFileName();
                    }
                    else if (path != null)
                    {
                        item.FullName = path.GetFtpPath(item.Name); //.GetFtpPathWithoutGlob();
                    }
                    else
                    {
                        client.LogStatus(FtpTraceLevel.Warn, "Couldn't determine the full path of this object: " +
                                                             Environment.NewLine + item.ToString());
                    }
                }

                // if a link target is set and it doesn't include an absolute path
                // then try to resolve it.
                if (item.LinkTarget != null && !item.LinkTarget.StartsWith("/"))
                {
                    if (item.LinkTarget.StartsWith("./"))
                    {
                        item.LinkTarget = path.GetFtpPath(item.LinkTarget.Remove(0, 2)).Trim();
                    }
                    else
                    {
                        item.LinkTarget = path.GetFtpPath(item.LinkTarget).Trim();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class Strings
    {


        /// <summary>
        /// Checks if every character in the string is whitespace, or the string is null.
        /// </summary>
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null)
            {
                return true;
            }

            for (var i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the string is null or 0 length.
        /// </summary>
        public static bool IsBlank(this string value)
        {
            return value == null || value.Length == 0;
        }

        /// <summary>
        /// Join the given strings by a delimiter.
        /// </summary>
        public static string Join(this string[] values, string delimiter)
        {
            return string.Join(delimiter, values);
        }

        /// <summary>
        /// Join the given strings by a delimiter.
        /// </summary>
        public static string Join(this List<string> values, string delimiter)
        {
#if NET20 || NET35
			return string.Join(delimiter, values.ToArray());
#else
            return string.Join(delimiter, values);
#endif
        }

        /// <summary>
        /// Adds a prefix to the given strings, returns a new array.
        /// </summary>
        public static string[] AddPrefix(this string[] values, string prefix, bool trim = false)
        {
            var results = new List<string>();
            foreach (var v in values)
            {
                var txt = prefix + (trim ? v.Trim() : v);
                results.Add(txt);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Adds a prefix to the given strings, returns a new array.
        /// </summary>
        public static List<string> AddPrefix(this List<string> values, string prefix, bool trim = false)
        {
            var results = new List<string>();
            foreach (var v in values)
            {
                var txt = prefix + (trim ? v.Trim() : v);
                results.Add(txt);
            }

            return results;
        }

        /// <summary>
        /// Ensure a string has the given prefix
        /// </summary>
        public static string EnsurePrefix(this string text, string prefix)
        {
            if (!text.StartsWith(prefix))
            {
                return prefix + text;
            }

            return text;
        }

        /// <summary>
        /// Ensure a string has the given postfix
        /// </summary>
        public static string EnsurePostfix(this string text, string postfix)
        {
            if (!text.EndsWith(postfix))
            {
                return text + postfix;
            }

            return text;
        }

        /// <summary>
        /// Remove a prefix from a string, only if it has the given prefix
        /// </summary>
        public static string RemovePrefix(this string text, string prefix)
        {
            if (text.StartsWith(prefix))
            {
                return text.Substring(prefix.Length).Trim();
            }
            return text;
        }

        /// <summary>
        /// Remove a postfix from a string, only if it has the given postfix
        /// </summary>
        public static string RemovePostfix(this string text, string postfix)
        {
            if (text.EndsWith(postfix))
            {
                return text.Substring(0, text.Length - postfix.Length);
            }
            return text;
        }


        /// <summary>
        /// Escape a string into a valid C# string literal.
        /// Implementation from StackOverflow - https://stackoverflow.com/a/14087738
        /// </summary>
        public static string EscapeStringLiteral(this string input)
        {
            var literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'':
                        literal.Append(@"\'");
                        break;

                    case '\"':
                        literal.Append("\\\"");
                        break;

                    case '\\':
                        literal.Append(@"\\");
                        break;

                    case '\0':
                        literal.Append(@"\0");
                        break;

                    case '\a':
                        literal.Append(@"\a");
                        break;

                    case '\b':
                        literal.Append(@"\b");
                        break;

                    case '\f':
                        literal.Append(@"\f");
                        break;

                    case '\n':
                        literal.Append(@"\n");
                        break;

                    case '\r':
                        literal.Append(@"\r");
                        break;

                    case '\t':
                        literal.Append(@"\t");
                        break;

                    case '\v':
                        literal.Append(@"\v");
                        break;

                    default:

                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                        }
                        else
                        {
                            // As UTF16 escaped character
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }

                        break;
                }
            }

            literal.Append("\"");
            return literal.ToString();
        }


        /// <summary>
        /// Split into fields by splitting on tokens
        /// </summary>
        public static string[] SplitString(this string str)
        {
            var allTokens = new List<string>(str.Split(null));
            for (var i = allTokens.Count - 1; i >= 0; i--)
            {
                if (((string)allTokens[i]).Trim().Length == 0)
                {
                    allTokens.RemoveAt(i);
                }
            }

            return (string[])allTokens.ToArray();
        }

        /// <summary>
        /// Checks if all the characters in this string are digits or dots
        /// </summary>
        public static bool IsNumeric(this string field)
        {
            field = field.Replace(".", ""); // strip dots
            for (var i = 0; i < field.Length; i++)
            {
                if (!char.IsDigit(field[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the string contains any of the given values
        /// </summary>
        public static bool ContainsAny(this string field, string[] values, int afterChar = -1)
        {
            foreach (var value in values)
            {
                if (field.IndexOf(value, StringComparison.Ordinal) > afterChar)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if RexEx Pattern is valid
        /// </summary>
        public static bool IsValidRegEx(this string pattern)
        {
            bool isValid = true;

            if ((pattern != null) && (pattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match("", pattern);
                }
                catch (ArgumentException)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                }
            }
            else
            {
                //BAD PATTERN: Pattern is null or blank
                isValid = false;
            }

            return (isValid);
        }

        /// <summary>
        /// Checks if the reply contains any of the known error strings
        /// </summary>
        public static bool IsKnownError(this string reply, string[] strings)
        {

            // FIX: absorb cases where the reply is null (see issue #631)
            if (reply == null)
            {
                return false;
            }

            reply = reply.ToLower();
            foreach (var msg in strings)
            {
                if (reply.Contains(msg))
                {
                    return true;
                }
            }

            return false;
        }


    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    internal static class Uris
    {
        /// <summary>
        /// Ensures that the URI points to a server, and not a directory or invalid path.
        /// </summary>
        /// <param name="uri"></param>
        public static void ValidateFtpServer(this Uri uri)
        {
            if (string.IsNullOrEmpty(uri.PathAndQuery))
            {
                throw new UriFormatException("The supplied URI does not contain a valid path.");
            }

            if (uri.PathAndQuery.EndsWith("/"))
            {
                throw new UriFormatException("The supplied URI points at a directory.");
            }
        }


    }

    internal static class FtpReflection
    {
#if !NETFx
        public static object GetField(this object obj, string fieldName)
        {
            var tp = obj.GetType();
            var info = GetAllFields(tp).Where(f => f.Name == fieldName).Single();
            return info.GetValue(obj);
        }

        public static void SetField(this object obj, string fieldName, object value)
        {
            var tp = obj.GetType();
            var info = GetAllFields(tp).Where(f => f.Name == fieldName).Single();
            info.SetValue(obj, value);
        }

        public static object GetStaticField(this Assembly assembly, string typeName, string fieldName)
        {
            var tp = assembly.GetType(typeName);
            var info = GetAllFields(tp).Where(f => f.IsStatic).Where(f => f.Name == fieldName).Single();
            return info.GetValue(null);
        }

        public static object GetProperty(this object obj, string propertyName)
        {
            var tp = obj.GetType();
            var info = GetAllProperties(tp).Where(f => f.Name == propertyName).Single();
            return info.GetValue(obj, null);
        }

        public static object CallMethod(this object obj, string methodName, params object[] prm)
        {
            var tp = obj.GetType();
            var info = GetAllMethods(tp).Where(f => f.Name == methodName && f.GetParameters().Length == prm.Length).Single();
            var rez = info.Invoke(obj, prm);
            return rez;
        }

        public static object NewInstance(this Assembly assembly, string typeName, params object[] prm)
        {
            var tp = assembly.GetType(typeName);
            var info = tp.GetConstructors().Where(f => f.GetParameters().Length == prm.Length).Single();
            var rez = info.Invoke(prm);
            return rez;
        }

        public static object InvokeStaticMethod(this Assembly assembly, string typeName, string methodName, params object[] prm)
        {
            var tp = assembly.GetType(typeName);
            var info = GetAllMethods(tp).Where(f => f.IsStatic).Where(f => f.Name == methodName && f.GetParameters().Length == prm.Length).Single();
            var rez = info.Invoke(null, prm);
            return rez;
        }

        public static object GetEnumValue(this Assembly assembly, string typeName, int value)
        {
            var tp = assembly.GetType(typeName);
            var rez = Enum.ToObject(tp, value);
            return rez;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetProperties(flags).Concat(GetAllProperties(t.BaseType));
        }

        private static IEnumerable<MethodInfo> GetAllMethods(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetMethods(flags).Concat(GetAllMethods(t.BaseType));
        }

#endif
    }
}
