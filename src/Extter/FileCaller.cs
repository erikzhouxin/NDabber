using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 文件调用者
    /// </summary>
    public static partial class ExtterCaller
    {
        #region // 静态定义
        private static readonly object _pathLocker = new object();
        private static readonly Dictionary<string, string> _pathDic = new Dictionary<string, string>();
        #endregion 静态定义
        #region // 文件空间及大小
        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static FolderSizeInfo GetFolderSize(this DirectoryInfo directory)
        {
            FolderSizeInfo result = new FolderSizeInfo();
            if (!directory.Exists) { return result; }
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            long fileSize = 0;
            long spaceSize = 0;
            var diskInfo = directory.GetDiskInfo();
            foreach (var item in files)
            {
                fileSize += item.Length;
                spaceSize += CalcSpaceSize(item.Length, diskInfo.ClusterSize);
            }
            var dirs = directory.GetDirectories("*", SearchOption.AllDirectories);
            result.FolderCount = dirs.Length;
            result.FolderSize = spaceSize;
            result.FileCount = files.Length;
            result.FileSize = fileSize;
            return result;
        }
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(this DirectoryInfo dir) => GetDiskInfo(dir.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(this FileInfo file) => GetDiskInfo(file.Directory.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(string rootPathName)
        {
            uint sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            Impeller.KERNEL32.GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            return new DiskSizeInfo()
            {
                SectorsPerCluster = sectorsPerCluster,
                BytesPerSector = bytesPerSector
            };
        }
        private static long CalcSpaceSize(long fileSize, long clusterSize)
        {
            if (fileSize % clusterSize == 0) { return fileSize; }
            decimal res = fileSize / clusterSize;
            int clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
            return clusterSize * clu;
        }
        #endregion
        #region // 判断文件夹
        /// <summary>
        /// 尝试打开文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool TryOpenDirectory(string dir)
        {
            try
            {
                if (File.Exists(dir))
                {
                    Process.Start("explorer.exe", $"/e,/select,{dir}");
                }
                else
                {
                    Process.Start("explorer.exe", $"/e,{dir}");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 最后一个盘符
        /// </summary>
        public static String LastLocalDisk { get; } = GetLastLocalDisk();
        /// <summary>
        /// 最后一个盘符
        /// 如:F:\
        /// </summary>
        /// <returns></returns>
        public static string GetLastLocalDisk()
        {
            return DriveInfo.GetDrives().LastOrDefault(s => s.DriveType == DriveType.Fixed)?.Name;
        }
        /// <summary>
        /// 获取已存在的保存目录
        /// 反人类方法规则:
        /// 1.如果saveDir不为空,返回saveDir
        /// 2.如果saveDir不为空,将拼接parent和subDir为saveDir返回
        /// 3.如果出现异常将返回Directory.GetCurrentDirectory()下的Temp文件夹
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public static string GetExistSaveDir(string saveDir, string parent, string subDir, bool isRecursive)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(saveDir))
                {
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    return saveDir;
                }
                saveDir = Path.GetFullPath(Path.Combine(parent, subDir));
                CreateDir(new DirectoryInfo(saveDir), isRecursive);
                return saveDir;
            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }
        }
        /// <summary>
        /// 获取已存在的保存目录
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <returns></returns>
        public static string GetExistSaveDir(string parent, string subDir)
        {
            try
            {
                var saveDir = Path.GetFullPath(Path.Combine(parent, subDir));
                CreateDir(new DirectoryInfo(saveDir), true);
                return saveDir;
            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isRecursive">循环递归创建</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDir(this DirectoryInfo dir, bool isRecursive = false)
        {
            if (isRecursive) { return CreateRecursiveDir(dir); }
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateRecursiveDir(this DirectoryInfo dir)
        {
            if (dir.Exists) { return dir; }
            CreateRecursiveDir(dir.Parent);
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联检查目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        public static void CheckRecursiveDir(this DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                CheckRecursiveDir(dir.Parent);
                dir.Create();
            }
        }
        /// <summary>
        /// 存在内容
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool HasContent(this DirectoryInfo dir)
        {
            if (dir == null) { return false; }
            return dir.Exists && (dir.GetFiles().Length > 0 || dir.GetDirectories().Length > 0);
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void RegistPath(string key, string value)
        {
            lock (_pathLocker)
            {
                _pathDic[key] = value;
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetOrRegistPath(string key, string value)
        {
            if (_pathDic.TryGetValue(key, out var path))
            {
                return path;
            }
            lock (_pathLocker)
            {
                if (_pathDic.TryGetValue(key, out path))
                {
                    return path;
                }
                return _pathDic[key] = value;
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="GetPath"></param>
        /// <returns></returns>
        public static string GetOrRegistPath(string key, Func<string> GetPath)
        {
            if (_pathDic.TryGetValue(key, out var path)) { return path; }
            lock (_pathLocker)
            {
                if (_pathDic.TryGetValue(key, out path)) { return path; }
                return _pathDic[key] = GetPath();
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPath(string key)
        {
            if (_pathDic.TryGetValue(key, out var path)) { return path; }
            return Path.GetFullPath(string.Empty);
        }
        /// <summary>
        /// 获取一个目录信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="defSubDir"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectory(string parent, string subDir, string defSubDir)
        {
            if (string.IsNullOrWhiteSpace(subDir)) { subDir = string.IsNullOrWhiteSpace(defSubDir) ? "Temp" : defSubDir; }
            var path = new DirectoryInfo(Path.GetFullPath(Path.Combine(parent, subDir)));
            if (!path.Exists) { path.Create(); }
            return path;
        }
        /// <summary>
        /// 获取一个目录信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="defSubDir"></param>
        /// <returns></returns>
        public static LazyBone<DirectoryInfo> GetLazyDirectory(string parent, string subDir, string defSubDir = null)
        {
            return new LazyBone<DirectoryInfo>(() => GetDirectory(parent, subDir, defSubDir), true);
        }
    }
    ///<summary>
    /// 结构。硬盘信息
    /// </summary>
    public struct DiskSizeInfo
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPathName;
        /// <summary>
        /// 每簇的扇区数
        /// </summary>
        public uint SectorsPerCluster;
        /// <summary>
        /// 每扇区字节
        /// </summary>
        public uint BytesPerSector;
        /// <summary>
        /// 可用簇
        /// </summary>
        public uint NumberOfFreeClusters;
        /// <summary>
        /// 总簇数
        /// </summary>
        public uint TotalNumberOfClusters;
        /// <summary>
        /// 簇字节数=每簇扇区数*每扇区字节数
        /// </summary>
        public long ClusterSize { get => BytesPerSector * SectorsPerCluster; }
    }
    /// <summary>
    /// 文件夹大小信息
    /// </summary>
    public class FolderSizeInfo
    {
        /// <summary>
        /// 文件计数
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// 文件夹计数
        /// </summary>
        public int FolderCount { get; set; }
        /// <summary>
        /// 文件长度
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件夹长度
        /// </summary>
        public long FolderSize { get; set; }
    }
}
