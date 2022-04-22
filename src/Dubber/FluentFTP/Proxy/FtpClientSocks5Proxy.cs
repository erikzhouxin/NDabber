﻿using System.Net;
using System.Data.Dubber.Proxy.Socks;
#if ASYNC

using System.Threading;
using System.Threading.Tasks;

#endif


namespace System.Data.Dubber.Proxy
{
    /// <summary> A FTP client with a SOCKS5 proxy implementation. </summary>
    public class FtpClientSocks5Proxy : FtpClientProxy
    {
        public FtpClientSocks5Proxy(ProxyInfo proxy) : base(proxy)
        {
        }

        protected override void Connect(FtpSocketStream stream)
        {
            base.Connect(stream);
            var proxy = new SocksProxy(Host, Port, stream);
            proxy.Negotiate();
            proxy.Authenticate();
            proxy.Connect();
        }

        protected override void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
        {
            base.Connect(stream);
            var proxy = new SocksProxy(Host, port, stream);
            proxy.Negotiate();
            proxy.Authenticate();
            proxy.Connect();
        }

#if ASYNC
		protected override async Task ConnectAsync(FtpSocketStream stream, CancellationToken cancellationToken) {
			await base.ConnectAsync(stream, cancellationToken);
			var proxy = new SocksProxy(Host, Port, stream);
			await proxy.NegotiateAsync();
			await proxy.AuthenticateAsync();
			await proxy.ConnectAsync();
		}
#endif
    }
}