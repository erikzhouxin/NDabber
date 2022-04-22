using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Dubber
{
	/// <summary> 
	/// A FTP client with a user@host proxy identification, that works with Blue Coat FTP Service servers.
	/// 
	/// The 'blue coat variant' forces the client to wait for a 220 FTP response code in 
	/// the handshake phase.
	/// </summary>
	public class FtpClientBlueCoatProxy : FtpClientProxy
	{
		/// <summary> A FTP client with a user@host proxy identification. </summary>
		/// <param name="proxy">Proxy information</param>
		public FtpClientBlueCoatProxy(ProxyInfo proxy)
			: base(proxy)
		{
			ConnectionType = "User@Host";
		}

		/// <summary>
		/// Creates a new instance of this class. Useful in FTP proxy classes.
		/// </summary>
		protected override FtpClient Create()
		{
			return new FtpClientBlueCoatProxy(Proxy);
		}

		/// <summary> Redefine the first dialog: auth with proxy information </summary>
		protected override void Handshake()
		{
			// Proxy authentication eventually needed.
			if (Proxy.Credentials != null)
			{
				Authenticate(Proxy.Credentials.UserName, Proxy.Credentials.Password, Proxy.Credentials.Domain);
			}

			// Connection USER@Host means to change user name to add host.
			Credentials.UserName = Credentials.UserName + "@" + Host + ":" + Port;

			var reply = GetReply();
			if (reply.Code == "220")
			{
				LogLine(FtpTraceLevel.Info, "Status: Server is ready for the new client");
			}

			// TO TEST: if we are able to detect the actual FTP server software from this reply
			HandshakeReply = reply;
		}
	}
	/// <summary> A FTP client with a HTTP 1.1 proxy implementation. </summary>
	public class FtpClientHttp11Proxy : FtpClientProxy
	{
		/// <summary> A FTP client with a HTTP 1.1 proxy implementation </summary>
		/// <param name="proxy">Proxy information</param>
		public FtpClientHttp11Proxy(ProxyInfo proxy)
			: base(proxy)
		{
			ConnectionType = "HTTP 1.1 Proxy";
		}

		/// <summary> Redefine the first dialog: HTTP Frame for the HTTP 1.1 Proxy </summary>
		protected override void Handshake()
		{
			var proxyConnectionReply = GetReply();
			if (!proxyConnectionReply.Success)
			{
				throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " +
									   proxyConnectionReply.ErrorMessage);
			}

			// TO TEST: if we are able to detect the actual FTP server software from this reply
			HandshakeReply = proxyConnectionReply;
		}

		/// <summary>
		/// Creates a new instance of this class. Useful in FTP proxy classes.
		/// </summary>
		protected override FtpClient Create()
		{
			return new FtpClientHttp11Proxy(Proxy);
		}

		/// <summary>
		/// Connects to the server using an existing <see cref="FtpSocketStream"/>
		/// </summary>
		/// <param name="stream">The existing socket stream</param>
		protected override void Connect(FtpSocketStream stream)
		{
			Connect(stream, Host, Port, FtpIpVersion.ANY);
		}

#if ASYNC
		/// <summary>
		/// Connects to the server using an existing <see cref="FtpSocketStream"/>
		/// </summary>
		/// <param name="stream">The existing socket stream</param>
		protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token) {
			return ConnectAsync(stream, Host, Port, FtpIpVersion.ANY, token);
		}
#endif

		/// <summary>
		/// Connects to the server using an existing <see cref="FtpSocketStream"/>
		/// </summary>
		/// <param name="stream">The existing socket stream</param>
		/// <param name="host">Host name</param>
		/// <param name="port">Port number</param>
		/// <param name="ipVersions">IP version to use</param>
		protected override void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
		{
			base.Connect(stream);

			var writer = new StreamWriter(stream);
			writer.WriteLine("CONNECT {0}:{1} HTTP/1.1", host, port);
			writer.WriteLine("Host: {0}:{1}", host, port);
			if (Proxy.Credentials != null)
			{
				var credentialsHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" + Proxy.Credentials.Password));
				writer.WriteLine("Proxy-Authorization: Basic " + credentialsHash);
			}

			writer.WriteLine("User-Agent: custom-ftp-client");
			writer.WriteLine();
			writer.Flush();

			ProxyHandshake(stream);
		}

#if ASYNC
		/// <summary>
		/// Connects to the server using an existing <see cref="FtpSocketStream"/>
		/// </summary>
		/// <param name="stream">The existing socket stream</param>
		/// <param name="host">Host name</param>
		/// <param name="port">Port number</param>
		/// <param name="ipVersions">IP version to use</param>
		/// <param name="token">IP version to use</param>
		protected override async Task ConnectAsync(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions, CancellationToken token) {
			await base.ConnectAsync(stream, token);

			var writer = new StreamWriter(stream);
			await writer.WriteLineAsync(string.Format("CONNECT {0}:{1} HTTP/1.1", host, port));
			await writer.WriteLineAsync(string.Format("Host: {0}:{1}", host, port));
			if (Proxy.Credentials != null) {
				var credentialsHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Proxy.Credentials.UserName + ":" + Proxy.Credentials.Password));
				await writer.WriteLineAsync("Proxy-Authorization: Basic " + credentialsHash);
			}

			await writer.WriteLineAsync("User-Agent: custom-ftp-client");
			await writer.WriteLineAsync();
			await writer.FlushAsync();

			await ProxyHandshakeAsync(stream, token);
		}
#endif

		private void ProxyHandshake(FtpSocketStream stream)
		{
			var proxyConnectionReply = GetProxyReply(stream);
			if (!proxyConnectionReply.Success)
			{
				throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " + proxyConnectionReply.ErrorMessage);
			}
		}

#if ASYNC
		private async Task ProxyHandshakeAsync(FtpSocketStream stream, CancellationToken token = default(CancellationToken)) {
			var proxyConnectionReply = await GetProxyReplyAsync(stream, token);
			if (!proxyConnectionReply.Success) {
				throw new FtpException("Can't connect " + Host + " via proxy " + Proxy.Host + ".\nMessage : " + proxyConnectionReply.ErrorMessage);
			}
		}
#endif

		private FtpReply GetProxyReply(FtpSocketStream stream)
		{
			var reply = new FtpReply();
			string buf;

#if !CORE14
			lock (Lock)
			{
#endif
				if (!IsConnected)
				{
					throw new InvalidOperationException("No connection to the server has been established.");
				}

				stream.ReadTimeout = ReadTimeout;
				while ((buf = stream.ReadLine(Encoding)) != null)
				{
					Match m;

					LogLine(FtpTraceLevel.Info, buf);

					if ((m = Regex.Match(buf, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success)
					{
						reply.Code = m.Groups["code"].Value;
						reply.Message = m.Groups["message"].Value;
						break;
					}

					reply.InfoMessages += buf + "\n";
				}

				// fixes #84 (missing bytes when downloading/uploading files through proxy)
				while ((buf = stream.ReadLine(Encoding)) != null)
				{
					LogLine(FtpTraceLevel.Info, buf);

					if (Strings.IsNullOrWhiteSpace(buf))
					{
						break;
					}

					reply.InfoMessages += buf + "\n";
				}

#if !CORE14
			}
#endif

			return reply;
		}

#if ASYNC
		private async Task<FtpReply> GetProxyReplyAsync(FtpSocketStream stream, CancellationToken token = default(CancellationToken)) {
			var reply = new FtpReply();
			string buf;

			if (!IsConnected) {
				throw new InvalidOperationException("No connection to the server has been established.");
			}

			stream.ReadTimeout = ReadTimeout;
			while ((buf = await stream.ReadLineAsync(Encoding, token)) != null) {
				Match m;

				LogLine(FtpTraceLevel.Info, buf);

				if ((m = Regex.Match(buf, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success) {
					reply.Code = m.Groups["code"].Value;
					reply.Message = m.Groups["message"].Value;
					break;
				}

				reply.InfoMessages += buf + "\n";
			}

			// fixes #84 (missing bytes when downloading/uploading files through proxy)
			while ((buf = await stream.ReadLineAsync(Encoding, token)) != null) {
				LogLine(FtpTraceLevel.Info, buf);

				if (Strings.IsNullOrWhiteSpace(buf)) {
					break;
				}

				reply.InfoMessages += buf + "\n";
			}

			return reply;
		}

#endif
	}
	/// <summary>
	/// Abstraction of an FtpClient with a proxy
	/// </summary>
	public abstract class FtpClientProxy : FtpClient
	{
		private ProxyInfo _proxy;

		/// <summary> The proxy connection info. </summary>
		protected ProxyInfo Proxy => _proxy;

		/// <summary> A FTP client with a HTTP 1.1 proxy implementation </summary>
		/// <param name="proxy">Proxy information</param>
		protected FtpClientProxy(ProxyInfo proxy)
		{
			_proxy = proxy;
		}

		/// <summary> Redefine connect for FtpClient : authentication on the Proxy  </summary>
		/// <param name="stream">The socket stream.</param>
		protected override void Connect(FtpSocketStream stream)
		{
			stream.Connect(Proxy.Host, Proxy.Port, InternetProtocolVersions);
		}

#if ASYNC
		/// <summary> Redefine connect for FtpClient : authentication on the Proxy  </summary>
		/// <param name="stream">The socket stream.</param>
		/// <param name="token">Cancellation token.</param>
		protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token) {
			return stream.ConnectAsync(Proxy.Host, Proxy.Port, InternetProtocolVersions, token);
		}

#endif
	}
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
	/// <summary> A FTP client with a user@host proxy identification. </summary>
	public class FtpClientUserAtHostProxy : FtpClientProxy
	{
		/// <summary> A FTP client with a user@host proxy identification. </summary>
		/// <param name="proxy">Proxy information</param>
		public FtpClientUserAtHostProxy(ProxyInfo proxy)
			: base(proxy)
		{
			ConnectionType = "User@Host";
		}

		/// <summary>
		/// Creates a new instance of this class. Useful in FTP proxy classes.
		/// </summary>
		protected override FtpClient Create()
		{
			return new FtpClientUserAtHostProxy(Proxy);
		}

		/// <summary> Redefine the first dialog: auth with proxy information </summary>
		protected override void Handshake()
		{
			// Proxy authentication eventually needed.
			if (Proxy.Credentials != null)
			{
				Authenticate(Proxy.Credentials.UserName, Proxy.Credentials.Password, Proxy.Credentials.Domain);
			}

			// Connection USER@Host means to change user name to add host.
			Credentials.UserName = Credentials.UserName + "@" + Host + ":" + Port;
		}
	}
	/// <summary> POCO holding proxy information</summary>
	public class ProxyInfo
	{
		/// <summary> Proxy host name </summary>
		public string Host { get; set; }

		/// <summary> Proxy port </summary>
		public int Port { get; set; }

		/// <summary> Proxy login credentials </summary>
		public NetworkCredential Credentials { get; set; }
	}
	#region // Socks
	public enum SocksAuthType
	{
		NoAuthRequired = 0x00,
		GSSAPI = 0x01,
		UsernamePassword = 0x02,
		NoAcceptableMethods = 0xFF
	}

	public enum SocksReply
	{
		Succeeded = 0x00,
		GeneralSOCKSServerFailure = 0x01,
		NotAllowedByRuleset = 0x02,
		NetworkUnreachable = 0x03,
		HostUnreachable = 0x04,
		ConnectionRefused = 0x05,
		TTLExpired = 0x06,
		CommandNotSupported = 0x07,
		AddressTypeNotSupported = 0x08
	}

	internal enum SocksRequestAddressType
	{
		Unknown = 0x00,
		IPv4 = 0x01,
		FQDN = 0x03,
		IPv6 = 0x04
	}

	internal enum SocksRequestCommand : byte
	{
		Connect = 0x01,
		Bind = 0x02,
		UdpAssociate = 0x03
	}

	internal enum SocksVersion
	{
		Four = 0x04,
		Five = 0x05
	}
	/// <summary>
	///     This class is not reusable.
	///     You have to create a new instance for each connection / attempt.
	/// </summary>
	public class SocksProxy
	{
		private readonly byte[] _buffer;
		private readonly string _destinationHost;
		private readonly int _destinationPort;
		private readonly FtpSocketStream _socketStream;
		private SocksAuthType? _authType;

		public SocksProxy(string destinationHost, int destinationPort, FtpSocketStream socketStream)
		{
			_buffer = new byte[512];
			_destinationHost = destinationHost;
			_destinationPort = destinationPort;
			_socketStream = socketStream;
		}

		public void Negotiate()
		{
			// The client connects to the server,
			// and sends a version identifier / method selection message.
			var methodsBuffer = new byte[]
			{
				(byte)SocksVersion.Five, // VER
				0x01, // NMETHODS
				(byte)SocksAuthType.NoAuthRequired // Methods
			};

			_socketStream.Write(methodsBuffer, 0, methodsBuffer.Length);

			// The server selects from one of the methods given in METHODS,
			// and sends a METHOD selection message:
			var receivedBytes = _socketStream.Read(_buffer, 0, 2);
			if (receivedBytes != 2)
			{
				_socketStream.Close();
				throw new SocksProxyException($"Negotiation Response had an invalid length of {receivedBytes}");
			}

			_authType = (SocksAuthType)_buffer[1];
		}

		public void Authenticate()
		{
			AuthenticateInternal();
		}

		public void Connect()
		{
			var requestBuffer = GetConnectRequest();
			_socketStream.Write(requestBuffer, 0, requestBuffer.Length);

			SocksReply reply;

			// The server evaluates the request, and returns a reply.
			// - First we read VER, REP, RSV & ATYP
			var received = _socketStream.Read(_buffer, 0, 4);
			if (received != 4)
			{
				if (received >= 2)
				{
					reply = (SocksReply)_buffer[1];
					HandleProxyCommandError(reply);
				}

				_socketStream.Close();
				throw new SocksProxyException($"Connect Reply has Invalid Length {received}. Expecting 4.");
			}

			// - Now we check if the reply was positive.
			reply = (SocksReply)_buffer[1];

			if (reply != SocksReply.Succeeded)
			{
				HandleProxyCommandError(reply);
			}

			// - Consume rest of the SOCKS5 protocol so the next read will give application data.
			var atyp = (SocksRequestAddressType)_buffer[3];
			int atypSize;
			int read;

			switch (atyp)
			{
				case SocksRequestAddressType.IPv4:
					atypSize = 6;
					read = _socketStream.Read(_buffer, 0, atypSize);
					break;
				case SocksRequestAddressType.IPv6:
					atypSize = 18;
					read = _socketStream.Read(_buffer, 0, atypSize);
					break;
				case SocksRequestAddressType.FQDN:
					atypSize = 1;
					_socketStream.Read(_buffer, 0, atypSize);
					atypSize = _buffer[0] + 2;
					read = _socketStream.Read(_buffer, 0, atypSize);
					break;
				default:
					_socketStream.Close();
					throw new SocksProxyException("Unknown Socks Request Address Type", new ArgumentOutOfRangeException());
			}

			if (read != atypSize)
			{
				_socketStream.Close();
				throw new SocksProxyException($"Unexpected Response size from Request Type Data. Expected {atypSize} received {read}");
			}
		}


		private void AuthenticateInternal()
		{
			if (!_authType.HasValue)
			{
				_socketStream.Close();
				throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.", new ArgumentException("No SOCKS5 auth method has been set."));
			}

			// The client and server then enter a method-specific sub-negotiation.
			switch (_authType.Value)
			{
				case SocksAuthType.NoAuthRequired:
					break;

				case SocksAuthType.GSSAPI:
					_socketStream.Close();
					throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.", new NotSupportedException("GSSAPI is not implemented."));

				case SocksAuthType.UsernamePassword:
					_socketStream.Close();
					throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
						new NotSupportedException("UsernamePassword is not implemented."));

				// If the selected METHOD is X'FF', none of the methods listed by the
				// client are acceptable, and the client MUST close the connection
				case SocksAuthType.NoAcceptableMethods:
					_socketStream.Close();
					throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
						new MissingMethodException("METHOD is X'FF' No Client requested methods are acceptable. Closing the connection."));

				default:
					_socketStream.Close();
					throw new SocksProxyException("Invalid Auth Type Declared, see inner exception for details.",
						new ArgumentOutOfRangeException());
			}
		}

		private byte[] GetConnectRequest()
		{
			// Once the method-dependent sub negotiation has completed,
			// the client sends the request details.
			bool issHostname = !IPAddress.TryParse(_destinationHost, out var ip);

			var dstAddress = issHostname
				? Encoding.ASCII.GetBytes(_destinationHost)
				: ip.GetAddressBytes();

			var requestBuffer = issHostname
				? new byte[7 + dstAddress.Length]
				: new byte[6 + dstAddress.Length];

			requestBuffer[0] = (byte)SocksVersion.Five;
			requestBuffer[1] = (byte)SocksRequestCommand.Connect;

			if (issHostname)
			{
				requestBuffer[3] = (byte)SocksRequestAddressType.FQDN;
				requestBuffer[4] = (byte)dstAddress.Length;

				for (var i = 0; i < dstAddress.Length; i++)
				{
					requestBuffer[5 + i] = dstAddress[i];
				}

				requestBuffer[5 + dstAddress.Length] = (byte)(_destinationPort >> 8);
				requestBuffer[6 + dstAddress.Length] = (byte)_destinationPort;
			}
			else
			{
				requestBuffer[3] = dstAddress.Length == 4
					? (byte)SocksRequestAddressType.IPv4
					: (byte)SocksRequestAddressType.IPv6;

				for (var i = 0; i < dstAddress.Length; i++)
				{
					requestBuffer[4 + i] = dstAddress[i];
				}

				requestBuffer[4 + dstAddress.Length] = (byte)(_destinationPort >> 8);
				requestBuffer[5 + dstAddress.Length] = (byte)_destinationPort;
			}

			return requestBuffer;
		}

#if ASYNC
		public async Task NegotiateAsync()
		{
			// The client connects to the server,
			// and sends a version identifier / method selection message.
			var methodsBuffer = new byte[]
			{
				(byte)SocksVersion.Five, // VER
				0x01, // NMETHODS
				(byte)SocksAuthType.NoAuthRequired // Methods
			};

			await _socketStream.WriteAsync(methodsBuffer, 0, methodsBuffer.Length);

			// The server selects from one of the methods given in METHODS,
			// and sends a METHOD selection message:
			var receivedBytes = await _socketStream.ReadAsync(_buffer, 0, 2);
			if (receivedBytes != 2)
			{
				_socketStream.Close();
				throw new SocksProxyException($"Negotiation Response had an invalid length of {receivedBytes}");
			}

			_authType = (SocksAuthType)_buffer[1];
		}

		public Task AuthenticateAsync()
		{
			AuthenticateInternal();
			return Task.FromResult(0);
		}

		public async Task ConnectAsync()
		{
			var requestBuffer = GetConnectRequest();
			await _socketStream.WriteAsync(requestBuffer, 0, requestBuffer.Length);

			SocksReply reply;

			// The server evaluates the request, and returns a reply.
			// - First we read VER, REP, RSV & ATYP
			var received = await _socketStream.ReadAsync(_buffer, 0, 4);
			if (received != 4)
			{
				if (received >= 2)
				{
					reply = (SocksReply)_buffer[1];
					HandleProxyCommandError(reply);
				}

				_socketStream.Close();
				throw new SocksProxyException($"Connect Reply has Invalid Length {received}. Expecting 4.");
			}

			// - Now we check if the reply was positive.
			reply = (SocksReply)_buffer[1];

			if (reply != SocksReply.Succeeded)
			{
				HandleProxyCommandError(reply);
			}

			// - Consume rest of the SOCKS5 protocol so the next read will give application data.
			var atyp = (SocksRequestAddressType)_buffer[3];
			int atypSize;
			int read;

			switch (atyp)
			{
				case SocksRequestAddressType.IPv4:
					atypSize = 6;
					read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
					break;
				case SocksRequestAddressType.IPv6:
					atypSize = 18;
					read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
					break;
				case SocksRequestAddressType.FQDN:
					atypSize = 1;
					await _socketStream.ReadAsync(_buffer, 0, atypSize);

					atypSize = _buffer[0] + 2;
					read = await _socketStream.ReadAsync(_buffer, 0, atypSize);
					break;
				default:
					_socketStream.Close();
					throw new SocksProxyException("Unknown Socks Request Address Type", new ArgumentOutOfRangeException());
			}

			if (read != atypSize)
			{
				_socketStream.Close();
				throw new SocksProxyException($"Unexpected Response size from Request Type Data. Expected {atypSize} received {read}");
			}
		}
#endif
		private void HandleProxyCommandError(SocksReply replyCode)
		{
			string proxyErrorText;
			switch (replyCode)
			{
				case SocksReply.GeneralSOCKSServerFailure:
					proxyErrorText = "a general socks destination failure occurred";
					break;
				case SocksReply.NotAllowedByRuleset:
					proxyErrorText = "the connection is not allowed by proxy destination rule set";
					break;
				case SocksReply.NetworkUnreachable:
					proxyErrorText = "the network was unreachable";
					break;
				case SocksReply.HostUnreachable:
					proxyErrorText = "the host was unreachable";
					break;
				case SocksReply.ConnectionRefused:
					proxyErrorText = "the connection was refused by the remote network";
					break;
				case SocksReply.TTLExpired:
					proxyErrorText = "the time to live (TTL) has expired";
					break;
				case SocksReply.CommandNotSupported:
					proxyErrorText = "the command issued by the proxy client is not supported by the proxy destination";
					break;
				case SocksReply.AddressTypeNotSupported:
					proxyErrorText = "the address type specified is not supported";
					break;
				default:
					proxyErrorText = $"an unknown SOCKS reply with the code value '{replyCode}' was received";
					break;
			}

			_socketStream.Close();
			throw new SocksProxyException($"Proxy error: {proxyErrorText} for destination host {_destinationHost} port number {_destinationPort}.");
		}
	}
	#endregion // Socks
}
