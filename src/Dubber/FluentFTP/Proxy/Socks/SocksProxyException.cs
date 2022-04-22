using System;

namespace System.Data.Dubber.Proxy.Socks
{
	public class SocksProxyException : Exception
	{
		public SocksProxyException()
		{
		}

		public SocksProxyException(string message)
			: base(message)
		{
		}

		public SocksProxyException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}