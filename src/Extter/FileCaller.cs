using System;
using System.Collections.Generic;
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
    public static class FileCaller
    {
        #region // 文件空间及大小
        /// <summary>
        /// 用于获取盘信息的api
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <param name="sectorsPerCluster"></param>
        /// <param name="bytesPerSector"></param>
        /// <param name="numberOfFreeClusters"></param>
        /// <param name="totalNumbeOfClusters"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName, ref uint sectorsPerCluster, ref uint bytesPerSector, ref uint numberOfFreeClusters, ref uint totalNumbeOfClusters);
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
            GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
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
