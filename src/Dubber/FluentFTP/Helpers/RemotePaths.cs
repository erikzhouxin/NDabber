﻿using System;
using System.Text.RegularExpressions;

namespace System.Data.Dubber.Helpers {
	/// <summary>
	/// Extension methods related to FTP tasks
	/// </summary>
	public static class RemotePaths {

		/// <summary>
		/// Checks if this FTP path is a top level path
		/// </summary>
		public static bool IsAbsolutePath(this string path) {
			return path.StartsWith("/") || path.StartsWith("./") || path.StartsWith("../");
		}

		/// <summary>
		/// Checks if the given path is a root directory or working directory path
		/// </summary>
		/// <param name="ftppath"></param>
		/// <returns></returns>
		public static bool IsFtpRootDirectory(this string ftppath) {
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
		public static string GetFtpPath(this string path) {
			if (string.IsNullOrEmpty(path)) {
				return "/";
			}

			path = path.Replace('\\', '/');
			path = Regex.Replace(path, "[/]+", "/");
			path = path.TrimEnd('/');

			if (path.Length == 0) {
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
		public static string GetFtpPath(this string path, params string[] segments) {
			if (string.IsNullOrEmpty(path)) {
				path = "/";
			}

			foreach (var part in segments) {
				if (part != null) {
					if (path.Length > 0 && !path.EndsWith("/")) {
						path += "/";
					}

					path += Regex.Replace(part.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
				}
			}

			path = Regex.Replace(path.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
			if (path.Length == 0) {
				path = "/";
			}

			return path;
		}

		/// <summary>
		/// Gets the parent directory path (formatted for a FTP server)
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>The parent directory path</returns>
		public static string GetFtpDirectoryName(this string path) {
			var tpath = path == null ? "" : path.GetFtpPath();

			if (tpath.Length == 0 || tpath == "/") {
				return "/";
			}

			var lastslash = tpath.LastIndexOf('/');
			if (lastslash < 0) {
				return ".";
			}

			if (lastslash == 0) {
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
		public static string GetFtpFileName(this string path) {
			var tpath = path == null ? null : path;
			var lastslash = -1;

			// no change in path
			if (tpath == null) {
				return null;
			}

			// find the index of the right-most slash character
			lastslash = tpath.LastIndexOf('/');
			if (lastslash < 0) {
				lastslash = tpath.LastIndexOf('\\');
				if (lastslash < 0) {

					// no change in path
					return tpath;
				}
			}
			lastslash += 1;
			if (lastslash >= tpath.Length) {

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
		public static string[] GetPathSegments(this string path) {
			if (path.Contains("/")) {
				return path.Split('/');
			}
			else if (path.Contains("\\")) {
				return path.Split('\\');
			}
			else {
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
}