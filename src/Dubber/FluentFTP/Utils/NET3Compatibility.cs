﻿using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Sockets;
using System.Data.Dubber.Servers;

#if NET20 || NET35
namespace System.Data.Dubber {
	/// <summary>
	/// Extension methods related to FTP tasks
	/// </summary>
	public static class NET3Compatibility {

		public static bool HasFlag(this FtpHashAlgorithm flags, FtpHashAlgorithm flag) {
			return (flags & flag) == flag;
		}

		public static bool HasFlag(this FtpListOption flags, FtpListOption flag) {
			return (flags & flag) == flag;
		}

		public static bool HasFlag(this FtpCompareOption flags, FtpCompareOption flag) {
			return (flags & flag) == flag;
		}

		public static bool HasFlag(this FtpVerify flags, FtpVerify flag) {
			return (flags & flag) == flag;
		}

		public static bool HasFlag(this FtpError flags, FtpError flag) {
			return (flags & flag) == flag;
		}

		public static void Restart(this Stopwatch watch) {
			watch.Stop();
			watch.Start();
		}

	}
}
#endif