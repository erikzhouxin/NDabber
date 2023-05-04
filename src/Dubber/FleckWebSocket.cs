using System;
using System.Collections.Generic;
using System.Data.Logger;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Dubber
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketConnection : IWebSocketConnection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="initialize"></param>
        /// <param name="parseRequest"></param>
        /// <param name="handlerFactory"></param>
        /// <param name="negotiateSubProtocol"></param>
        public WebSocketConnection(ISocket socket, Action<IWebSocketConnection> initialize, Func<byte[], WebSocketHttpRequest> parseRequest, Func<WebSocketHttpRequest, IHandler> handlerFactory, Func<IEnumerable<string>, string> negotiateSubProtocol)
        {
            Socket = socket;
            OnOpen = () => { };
            OnClose = () => { };
            OnMessage = x => { };
            OnBinary = x => { };
            OnPing = x => SendPong(x);
            OnPong = x => { };
            OnError = x => { };
            _initialize = initialize;
            _handlerFactory = handlerFactory;
            _parseRequest = parseRequest;
            _negotiateSubProtocol = negotiateSubProtocol;
        }
        /// <summary>
        /// 
        /// </summary>
        public ISocket Socket { get; set; }

        private readonly Action<IWebSocketConnection> _initialize;
        private readonly Func<WebSocketHttpRequest, IHandler> _handlerFactory;
        private readonly Func<IEnumerable<string>, string> _negotiateSubProtocol;
        readonly Func<byte[], WebSocketHttpRequest> _parseRequest;
        /// <summary>
        /// 
        /// </summary>
        public IHandler Handler { get; set; }

        private bool _closing;
        private bool _closed;
        private const int ReadSize = 1024 * 4;
        /// <summary>
        /// 
        /// </summary>
        public Action OnOpen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action OnClose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<string> OnMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<byte[]> OnBinary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<byte[]> OnPing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<byte[]> OnPong { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<Exception> OnError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IWebSocketConnectionInfo ConnectionInfo { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAvailable
        {
            get { return !_closing && !_closed && Socket.Connected; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(string message)
        {
            return Send(message, Handler.FrameText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(byte[] message)
        {
            return Send(message, Handler.FrameBinary);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendPing(byte[] message)
        {
            return Send(message, Handler.FramePing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendPong(byte[] message)
        {
            return Send(message, Handler.FramePong);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="createFrame"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Task Send<T>(T message, Func<T, byte[]> createFrame)
        {
            if (Handler == null)
                throw new InvalidOperationException("Cannot send before handshake");

            if (!IsAvailable)
            {
                const string errorMessage = "Data sent while closing or after close. Ignoring.";
                LoggerConsole.Warn(errorMessage);

                var taskForException = new TaskCompletionSource<object>();
                taskForException.SetException(new ConnectionNotAvailableException(errorMessage));
                return taskForException.Task;
            }

            var bytes = createFrame(message);
            return SendBytes(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        public void StartReceiving()
        {
            var data = new List<byte>(ReadSize);
            var buffer = new byte[ReadSize];
            Read(data, buffer);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            Close(WebSocketStatusCodes.NormalClosure);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void Close(int code)
        {
            if (!IsAvailable)
                return;

            _closing = true;

            if (Handler == null)
            {
                CloseSocket();
                return;
            }

            var bytes = Handler.FrameClose(code);
            if (bytes.Length == 0)
                CloseSocket();
            else
                SendBytes(bytes, CloseSocket);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void CreateHandler(IEnumerable<byte> data)
        {
            var request = _parseRequest(data.ToArray());
            if (request == null)
                return;
            Handler = _handlerFactory(request);
            if (Handler == null)
                return;
            var subProtocol = _negotiateSubProtocol(request.SubProtocols);
            ConnectionInfo = WebSocketConnectionInfo.Create(request, Socket.RemoteIpAddress, Socket.RemotePort, subProtocol);

            _initialize(this);

            var handshake = Handler.CreateHandshake(subProtocol);
            SendBytes(handshake, OnOpen);
        }

        private void Read(List<byte> data, byte[] buffer)
        {
            if (!IsAvailable)
                return;

            Socket.Receive(buffer, r =>
            {
                if (r <= 0)
                {
                    LoggerConsole.Debug("0 bytes read. Closing.");
                    CloseSocket();
                    return;
                }
                LoggerConsole.Debug(r + " bytes read");
                var readBytes = buffer.Take(r);
                if (Handler != null)
                {
                    Handler.Receive(readBytes);
                }
                else
                {
                    data.AddRange(readBytes);
                    CreateHandler(data);
                }

                Read(data, buffer);
            },
            HandleReadError);
        }

        private void HandleReadError(Exception e)
        {
            if (e is AggregateException)
            {
                var agg = e as AggregateException;
                HandleReadError(agg.InnerException);
                return;
            }

            if (e is ObjectDisposedException)
            {
                LoggerConsole.Debug("Swallowing ObjectDisposedException", e);
                return;
            }

            OnError(e);

            if (e is HandshakeException)
            {
                LoggerConsole.Debug("Error while reading", e);
            }
            else if (e is WebSocketException)
            {
                LoggerConsole.Debug("Error while reading", e);
                Close(((WebSocketException)e).StatusCode);
            }
            else if (e is SubProtocolNegotiationFailureException)
            {
                LoggerConsole.Debug(e.Message);
                Close(WebSocketStatusCodes.ProtocolError);
            }
            else if (e is IOException)
            {
                LoggerConsole.Debug("Error while reading", e);
                Close(WebSocketStatusCodes.AbnormalClosure);
            }
            else
            {
                LoggerConsole.Error("Application Error", e);
                Close(WebSocketStatusCodes.InternalServerError);
            }
        }

        private Task SendBytes(byte[] bytes, Action callback = null)
        {
            return Socket.Send(bytes, () =>
            {
                LoggerConsole.Debug("Sent " + bytes.Length + " bytes");
                if (callback != null)
                    callback();
            },
                              e =>
                              {
                                  if (e is IOException)
                                      LoggerConsole.Debug("Failed to send. Disconnecting.", e);
                                  else
                                      LoggerConsole.Info("Failed to send. Disconnecting.", e);
                                  CloseSocket();
                              });
        }

        private void CloseSocket()
        {
            _closing = true;
            OnClose();
            _closed = true;
            Socket.Close();
            Socket.Dispose();
            _closing = false;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketConnectionInfo : IWebSocketConnectionInfo
    {
        const string CookiePattern = @"((;)*(\s)*(?<cookie_name>[^=]+)=(?<cookie_value>[^\;]+))+";
        private static readonly Regex CookieRegex = new Regex(CookiePattern, RegexOptions.Compiled);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientIp"></param>
        /// <param name="clientPort"></param>
        /// <param name="negotiatedSubprotocol"></param>
        /// <returns></returns>
        public static WebSocketConnectionInfo Create(WebSocketHttpRequest request, string clientIp, int clientPort, string negotiatedSubprotocol)
        {
            var info = new WebSocketConnectionInfo
            {
                Origin = request["Origin"] ?? request["Sec-WebSocket-Origin"],
                Host = request["Host"],
                SubProtocol = request["Sec-WebSocket-Protocol"],
                Path = request.Path,
                ClientIpAddress = clientIp,
                ClientPort = clientPort,
                NegotiatedSubProtocol = negotiatedSubprotocol,
                Headers = new Dictionary<string, string>(request.Headers, System.StringComparer.InvariantCultureIgnoreCase)
            };
            var cookieHeader = request["Cookie"];

            if (cookieHeader != null)
            {
                var match = CookieRegex.Match(cookieHeader);
                var fields = match.Groups["cookie_name"].Captures;
                var values = match.Groups["cookie_value"].Captures;
                for (var i = 0; i < fields.Count; i++)
                {
                    var name = fields[i].ToString();
                    var value = values[i].ToString();
                    info.Cookies[name] = value;
                }
            }

            return info;
        }


        WebSocketConnectionInfo()
        {
            Cookies = new Dictionary<string, string>();
            Id = Guid.NewGuid();
        }
        /// <summary>
        /// 
        /// </summary>
        public string NegotiatedSubProtocol { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string SubProtocol { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Origin { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ClientPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Cookies { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketServer : IWebSocketServer
    {
        private readonly string _scheme;
        private readonly IPAddress _locationIP;
        private Action<IWebSocketConnection> _config;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        public WebSocketServer(string location)
            : this(8181, location)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="location"></param>
        public WebSocketServer(int port, string location)
        {
            var uri = new Uri(location);
            Port = uri.Port > 0 ? uri.Port : port;
            Location = location;
            _locationIP = ParseIPAddress(uri);
            _scheme = uri.Scheme;
            var socket = new Socket(_locationIP.AddressFamily, SocketType.Stream, ProtocolType.IP);
            if (!MonoHelper.IsRunningOnMono())
            {
#if __MonoCS__
#else
                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
#endif
            }
            ListenerSocket = new SocketWrapper(socket);
            SupportedSubProtocols = new string[0];
        }
        /// <summary>
        /// 
        /// </summary>
        public ISocket ListenerSocket { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SslProtocols EnabledSslProtocols { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> SupportedSubProtocols { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RestartAfterListenError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSecure
        {
            get { return _scheme == "wss" && Certificate != null; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ListenerSocket.Dispose();
        }

        private IPAddress ParseIPAddress(Uri uri)
        {
            string ipStr = uri.Host;

            if (ipStr == "0.0.0.0")
            {
                return IPAddress.Any;
            }
            else if (ipStr == "[0000:0000:0000:0000:0000:0000:0000:0000]")
            {
                return IPAddress.IPv6Any;
            }
            else
            {
                try
                {
                    return IPAddress.Parse(ipStr);
                }
                catch (Exception ex)
                {
                    throw new FormatException("Failed to parse the IP address part of the location. Please make sure you specify a valid IP address. Use 0.0.0.0 or [::] to listen on all interfaces.", ex);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Start(Action<IWebSocketConnection> config)
        {
            var ipLocal = new IPEndPoint(_locationIP, Port);
            ListenerSocket.Bind(ipLocal);
            ListenerSocket.Listen(100);
            Port = ((IPEndPoint)ListenerSocket.LocalEndPoint).Port;
            LoggerConsole.Info(string.Format("Server started at {0} (actual port {1})", Location, Port));
            if (_scheme == "wss")
            {
                if (Certificate == null)
                {
                    LoggerConsole.Error("Scheme cannot be 'wss' without a Certificate");
                    return;
                }

                if (EnabledSslProtocols == SslProtocols.None)
                {
                    EnabledSslProtocols = SslProtocols.Tls;
                    LoggerConsole.Debug("Using default TLS 1.0 security protocol.");
                }
            }
            ListenForClients();
            _config = config;
        }

        private void ListenForClients()
        {
            ListenerSocket.Accept(OnClientConnect, e => {
                LoggerConsole.Error("Listener socket is closed", e);
                if (RestartAfterListenError)
                {
                    LoggerConsole.Info("Listener socket restarting");
                    try
                    {
                        ListenerSocket.Dispose();
                        var socket = new Socket(_locationIP.AddressFamily, SocketType.Stream, ProtocolType.IP);
                        ListenerSocket = new SocketWrapper(socket);
                        Start(_config);
                        LoggerConsole.Info("Listener socket restarted");
                    }
                    catch (Exception ex)
                    {
                        LoggerConsole.Error("Listener could not be restarted", ex);
                    }
                }
            });
        }

        private void OnClientConnect(ISocket clientSocket)
        {
            if (clientSocket == null) return; // socket closed

            LoggerConsole.Debug(String.Format("Client connected from {0}:{1}", clientSocket.RemoteIpAddress, clientSocket.RemotePort.ToString()));
            ListenForClients();

            WebSocketConnection connection = null;

            connection = new WebSocketConnection(
                clientSocket,
                _config,
                bytes => RequestParser.Parse(bytes, _scheme),
                r => HandlerFactory.BuildHandler(r,
                                                 s => connection.OnMessage(s),
                                                 connection.Close,
                                                 b => connection.OnBinary(b),
                                                 b => connection.OnPing(b),
                                                 b => connection.OnPong(b)),
                s => SubProtocolNegotiator.Negotiate(SupportedSubProtocols, s));

            if (IsSecure)
            {
                LoggerConsole.Debug("Authenticating Secure Connection");
                clientSocket
                    .Authenticate(Certificate,
                                  EnabledSslProtocols,
                                  connection.StartReceiving,
                                  e => LoggerConsole.Warn("Failed to Authenticate", e));
            }
            else
            {
                connection.StartReceiving();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketHttpRequest
    {
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>(System.StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// 
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Bytes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                string value;
                return _headers.TryGetValue(name, out value) ? value : default(string);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get
            {
                return _headers;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string[] SubProtocols
        {
            get
            {
                string value;
                return _headers.TryGetValue("Sec-WebSocket-Protocol", out value)
                    ? value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    : new string[0];
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class WebSocketStatusCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public const ushort NormalClosure = 1000;
        /// <summary>
        /// 
        /// </summary>
        public const ushort GoingAway = 1001;
        /// <summary>
        /// 
        /// </summary>
        public const ushort ProtocolError = 1002;
        /// <summary>
        /// 
        /// </summary>
        public const ushort UnsupportedDataType = 1003;
        /// <summary>
        /// 
        /// </summary>
        public const ushort NoStatusReceived = 1005;
        /// <summary>
        /// 
        /// </summary>
        public const ushort AbnormalClosure = 1006;
        /// <summary>
        /// 
        /// </summary>
        public const ushort InvalidFramePayloadData = 1007;
        /// <summary>
        /// 
        /// </summary>
        public const ushort PolicyViolation = 1008;
        /// <summary>
        /// 
        /// </summary>
        public const ushort MessageTooBig = 1009;
        /// <summary>
        /// 
        /// </summary>
        public const ushort MandatoryExt = 1010;
        /// <summary>
        /// 
        /// </summary>
        public const ushort InternalServerError = 1011;
        /// <summary>
        /// 
        /// </summary>
        public const ushort TLSHandshake = 1015;
        /// <summary>
        /// 
        /// </summary>
        public const ushort ApplicationError = 3000;
        /// <summary>
        /// 
        /// </summary>
        public static ushort[] ValidCloseCodes = new[]{
            NormalClosure, GoingAway, ProtocolError, UnsupportedDataType,
            InvalidFramePayloadData, PolicyViolation, MessageTooBig,
            MandatoryExt, InternalServerError
        };
    }
    /// <summary>
    /// 
    /// </summary>
    public class ComposableHandler : IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public Func<string, byte[]> Handshake = s => new byte[0];
        /// <summary>
        /// 
        /// </summary>
        public Func<string, byte[]> TextFrame = x => new byte[0];
        /// <summary>
        /// 
        /// </summary>
        public Func<byte[], byte[]> BinaryFrame = x => new byte[0];
        /// <summary>
        /// 
        /// </summary>
        public Action<List<byte>> ReceiveData = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public Func<byte[], byte[]> PingFrame = i => new byte[0];
        /// <summary>
        /// 
        /// </summary>
        public Func<byte[], byte[]> PongFrame = i => new byte[0];
        /// <summary>
        /// 
        /// </summary>
        public Func<int, byte[]> CloseFrame = i => new byte[0];

        private readonly List<byte> _data = new List<byte>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subProtocol"></param>
        /// <returns></returns>
        public byte[] CreateHandshake(string subProtocol = null)
        {
            return Handshake(subProtocol);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Receive(IEnumerable<byte> data)
        {
            _data.AddRange(data);

            ReceiveData(_data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] FrameText(string text)
        {
            return TextFrame(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] FrameBinary(byte[] bytes)
        {
            return BinaryFrame(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] FramePing(byte[] bytes)
        {
            return PingFrame(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] FramePong(byte[] bytes)
        {
            return PongFrame(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public byte[] FrameClose(int code)
        {
            return CloseFrame(code);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class Draft76Handler
    {
        private const byte End = 255;
        private const byte Start = 0;
        private const int MaxSize = 1024 * 1024 * 5;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        public static IHandler Create(WebSocketHttpRequest request, Action<string> onMessage)
        {
            return new ComposableHandler
            {
                TextFrame = Draft76Handler.FrameText,
                Handshake = sub => Draft76Handler.Handshake(request, sub),
                ReceiveData = data => ReceiveData(onMessage, data)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="data"></param>
        /// <exception cref="WebSocketException"></exception>
        public static void ReceiveData(Action<string> onMessage, List<byte> data)
        {
            while (data.Count > 0)
            {
                if (data[0] != Start)
                    throw new WebSocketException(WebSocketStatusCodes.InvalidFramePayloadData);

                var endIndex = data.IndexOf(End);
                if (endIndex < 0)
                    return;

                if (endIndex > MaxSize)
                    throw new WebSocketException(WebSocketStatusCodes.MessageTooBig);

                var bytes = data.Skip(1).Take(endIndex - 1).ToArray();

                data.RemoveRange(0, endIndex + 1);

                var message = Encoding.UTF8.GetString(bytes);

                onMessage(message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] FrameText(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            // wrap the array with the wrapper bytes
            var wrappedBytes = new byte[bytes.Length + 2];
            wrappedBytes[0] = Start;
            wrappedBytes[wrappedBytes.Length - 1] = End;
            Array.Copy(bytes, 0, wrappedBytes, 1, bytes.Length);
            return wrappedBytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="subProtocol"></param>
        /// <returns></returns>
        public static byte[] Handshake(WebSocketHttpRequest request, string subProtocol)
        {
            LoggerConsole.Debug("Building Draft76 Response");

            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 101 WebSocket Protocol Handshake\r\n");
            builder.Append("Upgrade: WebSocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            builder.AppendFormat("Sec-WebSocket-Origin: {0}\r\n", request["Origin"]);
            builder.AppendFormat("Sec-WebSocket-Location: {0}://{1}{2}\r\n", request.Scheme, request["Host"], request.Path);

            if (subProtocol != null)
                builder.AppendFormat("Sec-WebSocket-Protocol: {0}\r\n", subProtocol);

            builder.Append("\r\n");

            var key1 = request["Sec-WebSocket-Key1"];
            var key2 = request["Sec-WebSocket-Key2"];
            var challenge = new ArraySegment<byte>(request.Bytes, request.Bytes.Length - 8, 8);

            var answerBytes = CalculateAnswerBytes(key1, key2, challenge);

            byte[] byteResponse = Encoding.ASCII.GetBytes(builder.ToString());
            int byteResponseLength = byteResponse.Length;
            Array.Resize(ref byteResponse, byteResponseLength + answerBytes.Length);
            Array.Copy(answerBytes, 0, byteResponse, byteResponseLength, answerBytes.Length);

            return byteResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="challenge"></param>
        /// <returns></returns>
        public static byte[] CalculateAnswerBytes(string key1, string key2, ArraySegment<byte> challenge)
        {
            byte[] result1Bytes = ParseKey(key1);
            byte[] result2Bytes = ParseKey(key2);

            var rawAnswer = new byte[16];
            Array.Copy(result1Bytes, 0, rawAnswer, 0, 4);
            Array.Copy(result2Bytes, 0, rawAnswer, 4, 4);
            Array.Copy(challenge.Array, challenge.Offset, rawAnswer, 8, 8);

            return MD5.Create().ComputeHash(rawAnswer);
        }

        private static byte[] ParseKey(string key)
        {
            int spaces = key.Count(x => x == ' ');
            var digits = new String(key.Where(Char.IsDigit).ToArray());

            var value = (Int32)(Int64.Parse(digits) / spaces);

            byte[] result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);
            return result;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FlashSocketPolicyRequestHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public static string PolicyResponse =
"<?xml version=\"1.0\"?>\n" +
"<cross-domain-policy>\n" +
"   <allow-access-from domain=\"*\" to-ports=\"*\"/>\n" +
"   <site-control permitted-cross-domain-policies=\"all\"/>\n" +
"</cross-domain-policy>\n" +
"\0";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IHandler Create(WebSocketHttpRequest request)
        {
            return new ComposableHandler
            {
                Handshake = sub => FlashSocketPolicyRequestHandler.Handshake(request, sub),
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="subProtocol"></param>
        /// <returns></returns>
        public static byte[] Handshake(WebSocketHttpRequest request, string subProtocol)
        {
            LoggerConsole.Debug("Building Flash Socket Policy Response");
            return Encoding.UTF8.GetBytes(PolicyResponse);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class Hybi13Handler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onMessage"></param>
        /// <param name="onClose"></param>
        /// <param name="onBinary"></param>
        /// <param name="onPing"></param>
        /// <param name="onPong"></param>
        /// <returns></returns>
        public static IHandler Create(WebSocketHttpRequest request, Action<string> onMessage, Action onClose, Action<byte[]> onBinary, Action<byte[]> onPing, Action<byte[]> onPong)
        {
            var readState = new ReadState();
            return new ComposableHandler
            {
                Handshake = sub => Hybi13Handler.BuildHandshake(request, sub),
                TextFrame = s => Hybi13Handler.FrameData(Encoding.UTF8.GetBytes(s), FrameType.Text),
                BinaryFrame = s => Hybi13Handler.FrameData(s, FrameType.Binary),
                PingFrame = s => Hybi13Handler.FrameData(s, FrameType.Ping),
                PongFrame = s => Hybi13Handler.FrameData(s, FrameType.Pong),
                CloseFrame = i => Hybi13Handler.FrameData(i.ToBigEndianBytes<ushort>(), FrameType.Close),
                ReceiveData = d => Hybi13Handler.ReceiveData(d, readState, (op, data) => Hybi13Handler.ProcessFrame(op, data, onMessage, onClose, onBinary, onPing, onPong))
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="frameType"></param>
        /// <returns></returns>
        public static byte[] FrameData(byte[] payload, FrameType frameType)
        {
            var memoryStream = new MemoryStream();
            byte op = (byte)((byte)frameType + 128);

            memoryStream.WriteByte(op);

            if (payload.Length > UInt16.MaxValue)
            {
                memoryStream.WriteByte(127);
                var lengthBytes = payload.Length.ToBigEndianBytes<ulong>();
                memoryStream.Write(lengthBytes, 0, lengthBytes.Length);
            }
            else if (payload.Length > 125)
            {
                memoryStream.WriteByte(126);
                var lengthBytes = payload.Length.ToBigEndianBytes<ushort>();
                memoryStream.Write(lengthBytes, 0, lengthBytes.Length);
            }
            else
            {
                memoryStream.WriteByte((byte)payload.Length);
            }

            memoryStream.Write(payload, 0, payload.Length);

            return memoryStream.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="readState"></param>
        /// <param name="processFrame"></param>
        /// <exception cref="WebSocketException"></exception>
        public static void ReceiveData(List<byte> data, ReadState readState, Action<FrameType, byte[]> processFrame)
        {

            while (data.Count >= 2)
            {
                var isFinal = (data[0] & 128) != 0;
                var reservedBits = (data[0] & 112);
                var frameType = (FrameType)(data[0] & 15);
                var isMasked = (data[1] & 128) != 0;
                var length = (data[1] & 127);


                if (!isMasked
                    || !Enum.IsDefined(typeof(FrameType), frameType)
                    || reservedBits != 0 //Must be zero per spec 5.2
                    || (frameType == FrameType.Continuation && !readState.FrameType.HasValue))
                    throw new WebSocketException(WebSocketStatusCodes.ProtocolError);

                var index = 2;
                int payloadLength;

                if (length == 127)
                {
                    if (data.Count < index + 8)
                        return; //Not complete
                    payloadLength = data.Skip(index).Take(8).ToArray().ToLittleEndianInt();
                    index += 8;
                }
                else if (length == 126)
                {
                    if (data.Count < index + 2)
                        return; //Not complete
                    payloadLength = data.Skip(index).Take(2).ToArray().ToLittleEndianInt();
                    index += 2;
                }
                else
                {
                    payloadLength = length;
                }

                if (data.Count < index + 4)
                    return; //Not complete

                var maskBytes = data.Skip(index).Take(4).ToArray();

                index += 4;


                if (data.Count < index + payloadLength)
                    return; //Not complete

                var payload = data
                                .Skip(index)
                                .Take(payloadLength)
                                .Select((x, i) => (byte)(x ^ maskBytes[i % 4]));


                readState.Data.AddRange(payload);
                data.RemoveRange(0, index + payloadLength);

                if (frameType != FrameType.Continuation)
                    readState.FrameType = frameType;

                if (isFinal && readState.FrameType.HasValue)
                {
                    var stateData = readState.Data.ToArray();
                    var stateFrameType = readState.FrameType;
                    readState.Clear();

                    processFrame(stateFrameType.Value, stateData);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameType"></param>
        /// <param name="data"></param>
        /// <param name="onMessage"></param>
        /// <param name="onClose"></param>
        /// <param name="onBinary"></param>
        /// <param name="onPing"></param>
        /// <param name="onPong"></param>
        /// <exception cref="WebSocketException"></exception>
        public static void ProcessFrame(FrameType frameType, byte[] data, Action<string> onMessage, Action onClose, Action<byte[]> onBinary, Action<byte[]> onPing, Action<byte[]> onPong)
        {
            switch (frameType)
            {
                case FrameType.Close:
                    if (data.Length == 1 || data.Length > 125)
                        throw new WebSocketException(WebSocketStatusCodes.ProtocolError);

                    if (data.Length >= 2)
                    {
                        var closeCode = (ushort)data.Take(2).ToArray().ToLittleEndianInt();
                        if (!WebSocketStatusCodes.ValidCloseCodes.Contains(closeCode) && (closeCode < 3000 || closeCode > 4999))
                            throw new WebSocketException(WebSocketStatusCodes.ProtocolError);
                    }

                    if (data.Length > 2)
                        ReadUTF8PayloadData(data.Skip(2).ToArray());

                    onClose();
                    break;
                case FrameType.Binary:
                    onBinary(data);
                    break;
                case FrameType.Ping:
                    onPing(data);
                    break;
                case FrameType.Pong:
                    onPong(data);
                    break;
                case FrameType.Text:
                    onMessage(ReadUTF8PayloadData(data));
                    break;
                default:
                    LoggerConsole.Debug("Received unhandled " + frameType);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="subProtocol"></param>
        /// <returns></returns>
        public static byte[] BuildHandshake(WebSocketHttpRequest request, string subProtocol)
        {
            LoggerConsole.Debug("Building Hybi-14 Response");

            var builder = new StringBuilder();

            builder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            if (subProtocol != null)
                builder.AppendFormat("Sec-WebSocket-Protocol: {0}\r\n", subProtocol);

            var responseKey = CreateResponseKey(request["Sec-WebSocket-Key"]);
            builder.AppendFormat("Sec-WebSocket-Accept: {0}\r\n", responseKey);
            builder.Append("\r\n");

            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        private const string WebSocketResponseGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestKey"></param>
        /// <returns></returns>
        public static string CreateResponseKey(string requestKey)
        {
            var combined = requestKey + WebSocketResponseGuid;

            var bytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(combined));

            return Convert.ToBase64String(bytes);
        }

        private static string ReadUTF8PayloadData(byte[] bytes)
        {
            var encoding = new UTF8Encoding(false, true);
            try
            {
                return encoding.GetString(bytes);
            }
            catch (ArgumentException)
            {
                throw new WebSocketException(WebSocketStatusCodes.InvalidFramePayloadData);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class MonoHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subProtocol"></param>
        /// <returns></returns>
        byte[] CreateHandshake(string subProtocol = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void Receive(IEnumerable<byte> data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        byte[] FrameText(string text);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] FrameBinary(byte[] bytes);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] FramePing(byte[] bytes);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] FramePong(byte[] bytes);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        byte[] FrameClose(int code);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// 
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 
        /// </summary>
        string RemoteIpAddress { get; }
        /// <summary>
        /// 
        /// </summary>
        int RemotePort { get; }
        /// <summary>
        /// 
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// 
        /// </summary>
        bool NoDelay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        EndPoint LocalEndPoint { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Task<ISocket> Accept(Action<ISocket> callback, Action<Exception> error);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Task Send(byte[] buffer, Action callback, Action<Exception> error);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<int> Receive(byte[] buffer, Action<int> callback, Action<Exception> error, int offset = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="enabledSslProtocols"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Task Authenticate(X509Certificate2 certificate, SslProtocols enabledSslProtocols, Action callback, Action<Exception> error);
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        /// <summary>
        /// 
        /// </summary>
        void Close();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipLocal"></param>
        void Bind(EndPoint ipLocal);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backlog"></param>
        void Listen(int backlog);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSocketConnection
    {
        /// <summary>
        /// 
        /// </summary>
        Action OnOpen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action OnClose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action<string> OnMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action<byte[]> OnBinary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action<byte[]> OnPing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action<byte[]> OnPong { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Action<Exception> OnError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send(byte[] message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendPing(byte[] message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendPong(byte[] message);
        /// <summary>
        /// 
        /// </summary>
        void Close();
        /// <summary>
        /// 
        /// </summary>
        IWebSocketConnectionInfo ConnectionInfo { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsAvailable { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSocketConnectionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        string SubProtocol { get; }
        /// <summary>
        /// 
        /// </summary>
        string Origin { get; }
        /// <summary>
        /// 
        /// </summary>
        string Host { get; }
        /// <summary>
        /// 
        /// </summary>
        string Path { get; }
        /// <summary>
        /// 
        /// </summary>
        string ClientIpAddress { get; }
        /// <summary>
        /// 
        /// </summary>
        int ClientPort { get; }
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, string> Cookies { get; }
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, string> Headers { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        string NegotiatedSubProtocol { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSocketServer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        void Start(Action<IWebSocketConnection> config);
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionNotAvailableException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectionNotAvailableException() : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConnectionNotAvailableException(string message) : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConnectionNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum FrameType : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Continuation,
        /// <summary>
        /// 
        /// </summary>
        Text,
        /// <summary>
        /// 
        /// </summary>
        Binary,
        /// <summary>
        /// 
        /// </summary>
        Close = 8,
        /// <summary>
        /// 
        /// </summary>
        Ping = 9,
        /// <summary>
        /// 
        /// </summary>
        Pong = 10,
    }
    /// <summary>
    /// 
    /// </summary>
    public class HandlerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onMessage"></param>
        /// <param name="onClose"></param>
        /// <param name="onBinary"></param>
        /// <param name="onPing"></param>
        /// <param name="onPong"></param>
        /// <returns></returns>
        /// <exception cref="WebSocketException"></exception>
        public static IHandler BuildHandler(WebSocketHttpRequest request, Action<string> onMessage, Action onClose, Action<byte[]> onBinary, Action<byte[]> onPing, Action<byte[]> onPong)
        {
            var version = GetVersion(request);

            switch (version)
            {
                case "76":
                    return Draft76Handler.Create(request, onMessage);
                case "7":
                case "8":
                case "13":
                    return Hybi13Handler.Create(request, onMessage, onClose, onBinary, onPing, onPong);
                case "policy-file-request":
                    return FlashSocketPolicyRequestHandler.Create(request);
            }

            throw new WebSocketException(WebSocketStatusCodes.UnsupportedDataType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetVersion(WebSocketHttpRequest request)
        {
            string version;
            if (request.Headers.TryGetValue("Sec-WebSocket-Version", out version))
                return version;

            if (request.Headers.TryGetValue("Sec-WebSocket-Draft", out version))
                return version;

            if (request.Headers.ContainsKey("Sec-WebSocket-Key1"))
                return "76";

            if ((request.Body != null) && request.Body.ToLower().Contains("policy-file-request"))
                return "policy-file-request";

            return "75";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HandshakeException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public HandshakeException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HandshakeException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandshakeException(string message, Exception innerException) : base(message, innerException) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static byte[] ToBigEndianBytes<T>(this int source)
        {
            byte[] bytes;

            var type = typeof(T);
            if (type == typeof(ushort))
                bytes = BitConverter.GetBytes((ushort)source);
            else if (type == typeof(ulong))
                bytes = BitConverter.GetBytes((ulong)source);
            else if (type == typeof(int))
                bytes = BitConverter.GetBytes(source);
            else
                throw new InvalidCastException("Cannot be cast to T");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ToLittleEndianInt(this byte[] source)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(source);

            if (source.Length == 2)
                return BitConverter.ToUInt16(source, 0);

            if (source.Length == 8)
                return (int)BitConverter.ToUInt64(source, 0);

            throw new ArgumentException("Unsupported Size");
        }
    }
    /// <summary>
    /// Wraps a stream and queues multiple write operations.
    /// Useful for wrapping SslStream as it does not support multiple simultaneous write operations.
    /// </summary>
    public class QueuedStream : Stream
    {
        readonly Stream _stream;
        readonly Queue<WriteData> _queue = new Queue<WriteData>();
        int _pendingWrite;
        bool _disposed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public QueuedStream(Stream stream)
        {
            _stream = stream;
        }
        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get { return _stream.Length; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="NotSupportedException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("QueuedStream does not support synchronous write operations yet.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            lock (_queue)
            {
                var data = new WriteData(buffer, offset, count, callback, state);
                if (_pendingWrite > 0)
                {
                    _queue.Enqueue(data);
                    return data.AsyncResult;
                }
                return BeginWriteInternal(buffer, offset, count, callback, state, data);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return _stream.EndRead(asyncResult);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult is QueuedWriteResult)
            {
                var queuedResult = asyncResult as QueuedWriteResult;
                if (queuedResult.Exception != null) throw queuedResult.Exception;
                var ar = queuedResult.ActualResult;
                if (ar == null)
                {
                    throw new NotSupportedException(
                        "QueuedStream does not support synchronous write operations. Please wait for callback to be invoked before calling EndWrite.");
                }
                // EndWrite on actual stream should already be invoked.
            }
            else
            {
                throw new ArgumentException();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            _stream.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        IAsyncResult BeginWriteInternal(byte[] buffer, int offset, int count, AsyncCallback callback, object state, WriteData queued)
        {
            _pendingWrite++;
            var result = _stream.BeginWrite(buffer, offset, count, ar =>
            {
                // callback can be executed even before return value of BeginWriteInternal is set to this property
                queued.AsyncResult.ActualResult = ar;
                try
                {
                    // so that we can call BeginWrite again
                    _stream.EndWrite(ar);
                }
                catch (Exception exc)
                {
                    queued.AsyncResult.Exception = exc;
                }

                // one down, another is good to go
                lock (_queue)
                {
                    _pendingWrite--;
                    while (_queue.Count > 0)
                    {
                        var data = _queue.Dequeue();
                        try
                        {
                            data.AsyncResult.ActualResult = BeginWriteInternal(data.Buffer, data.Offset, data.Count, data.Callback, data.State, data);
                            break;
                        }
                        catch (Exception exc)
                        {
                            _pendingWrite--;
                            data.AsyncResult.Exception = exc;
                            data.Callback(data.AsyncResult);
                        }
                    }
                    callback(queued.AsyncResult);
                }
            }, state);

            // always return the wrapped async result.
            // this is especially important if the underlying stream completed the operation synchronously (hence "result.CompletedSynchronously" is true!)
            queued.AsyncResult.ActualResult = result;
            return queued.AsyncResult;
        }

        #region Nested type: WriteData

        class WriteData
        {
            public readonly byte[] Buffer;
            public readonly int Offset;
            public readonly int Count;
            public readonly AsyncCallback Callback;
            public readonly object State;
            public readonly QueuedWriteResult AsyncResult;

            public WriteData(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                Buffer = buffer;
                Offset = offset;
                Count = count;
                Callback = callback;
                State = state;
                AsyncResult = new QueuedWriteResult(state);
            }
        }

        #endregion

        #region Nested type: QueuedWriteResult

        class QueuedWriteResult : IAsyncResult
        {
            readonly object _state;

            public QueuedWriteResult(object state)
            {
                _state = state;
            }

            public Exception Exception { get; set; }

            public IAsyncResult ActualResult { get; set; }

            public object AsyncState
            {
                get { return _state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { throw new NotSupportedException("Queued write operations do not support wait handle."); }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return ActualResult != null && ActualResult.IsCompleted; }
            }
        }

        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class ReadState
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadState()
        {
            Data = new List<byte>();
        }
        /// <summary>
        /// 
        /// </summary>
        public List<byte> Data { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public FrameType? FrameType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Data.Clear();
            FrameType = null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestParser
    {
        const string pattern = @"^(?<method>[^\s]+)\s(?<path>[^\s]+)\sHTTP\/1\.1\r\n" + // request line
                               @"((?<field_name>[^:\r\n]+):(?([^\r\n])\s)*(?<field_value>[^\r\n]*)\r\n)+" + //headers
                               @"\r\n" + //newline
                               @"(?<body>.+)?";
        const string FlashSocketPolicyRequestPattern = @"^[<]policy-file-request\s*[/][>]";

        private static readonly Regex _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _FlashSocketPolicyRequestRegex = new Regex(FlashSocketPolicyRequestPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static WebSocketHttpRequest Parse(byte[] bytes)
        {
            return Parse(bytes, "ws");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static WebSocketHttpRequest Parse(byte[] bytes, string scheme)
        {
            // Check for websocket request header
            var body = Encoding.UTF8.GetString(bytes);
            Match match = _regex.Match(body);

            if (!match.Success)
            {
                // No websocket request header found, check for a flash socket policy request
                match = _FlashSocketPolicyRequestRegex.Match(body);
                if (match.Success)
                {
                    // It's a flash socket policy request, so return
                    return new WebSocketHttpRequest
                    {
                        Body = body,
                        Bytes = bytes
                    };
                }
                else
                {
                    return null;
                }
            }

            var request = new WebSocketHttpRequest
            {
                Method = match.Groups["method"].Value,
                Path = match.Groups["path"].Value,
                Body = match.Groups["body"].Value,
                Bytes = bytes,
                Scheme = scheme
            };

            var fields = match.Groups["field_name"].Captures;
            var values = match.Groups["field_value"].Captures;
            for (var i = 0; i < fields.Count; i++)
            {
                var name = fields[i].ToString();
                var value = values[i].ToString();
                request.Headers[name] = value;
            }

            return request;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SocketWrapper : ISocket
    {
        private readonly Socket _socket;
        private Stream _stream;
        private CancellationTokenSource _tokenSource;
        private TaskFactory _taskFactory;

        /// <summary>
        /// 
        /// </summary>
        public string RemoteIpAddress
        {
            get
            {
                var endpoint = _socket.RemoteEndPoint as IPEndPoint;
                return endpoint != null ? endpoint.Address.ToString() : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RemotePort
        {
            get
            {
                var endpoint = _socket.RemoteEndPoint as IPEndPoint;
                return endpoint != null ? endpoint.Port : -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        public SocketWrapper(Socket socket)
        {
            _tokenSource = new CancellationTokenSource();
            _taskFactory = new TaskFactory(_tokenSource.Token);
            _socket = socket;
            if (_socket.Connected)
                _stream = new NetworkStream(_socket);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="enabledSslProtocols"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Task Authenticate(X509Certificate2 certificate, SslProtocols enabledSslProtocols, Action callback, Action<Exception> error)
        {
            var ssl = new SslStream(_stream, false);
            _stream = new QueuedStream(ssl);
            Func<AsyncCallback, object, IAsyncResult> begin =
                (cb, s) => ssl.BeginAuthenticateAsServer(certificate, false, enabledSslProtocols, false, cb, s);

            Task task = Task.Factory.FromAsync(begin, ssl.EndAuthenticateAsServer, null);
            task.ContinueWith(t => callback(), TaskContinuationOptions.NotOnFaulted)
                .ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backlog"></param>
        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        public void Bind(EndPoint endPoint)
        {
            _socket.Bind(endPoint);
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Connected
        {
            get { return _socket.Connected; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Stream Stream
        {
            get { return _stream; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool NoDelay
        {
            get { return _socket.NoDelay; }
            set { _socket.NoDelay = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get { return _socket.LocalEndPoint; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<int> Receive(byte[] buffer, Action<int> callback, Action<Exception> error, int offset)
        {
            try
            {
                Func<AsyncCallback, object, IAsyncResult> begin =
               (cb, s) => _stream.BeginRead(buffer, offset, buffer.Length, cb, s);

                Task<int> task = Task.Factory.FromAsync<int>(begin, _stream.EndRead, null);
                task.ContinueWith(t => callback(t.Result), TaskContinuationOptions.NotOnFaulted)
                    .ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                return task;
            }
            catch (Exception e)
            {
                error(e);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Task<ISocket> Accept(Action<ISocket> callback, Action<Exception> error)
        {
            Func<IAsyncResult, ISocket> end = r => _tokenSource.Token.IsCancellationRequested ? null : new SocketWrapper(_socket.EndAccept(r));
            var task = _taskFactory.FromAsync(_socket.BeginAccept, end, null);
            task.ContinueWith(t => callback(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _tokenSource.Cancel();
            if (_stream != null) _stream.Dispose();
            if (_socket != null) _socket.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _tokenSource.Cancel();
            if (_stream != null) _stream.Close();
            if (_socket != null) _socket.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public int EndSend(IAsyncResult asyncResult)
        {
            _stream.EndWrite(asyncResult);
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Task Send(byte[] buffer, Action callback, Action<Exception> error)
        {
            if (_tokenSource.IsCancellationRequested)
                return null;

            try
            {
                Func<AsyncCallback, object, IAsyncResult> begin =
                    (cb, s) => _stream.BeginWrite(buffer, 0, buffer.Length, cb, s);

                Task task = Task.Factory.FromAsync(begin, _stream.EndWrite, null);
                task.ContinueWith(t => callback(), TaskContinuationOptions.NotOnFaulted)
                    .ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith(t => error(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

                return task;
            }
            catch (Exception e)
            {
                error(e);
                return null;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SubProtocolNegotiationFailureException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public SubProtocolNegotiationFailureException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SubProtocolNegotiationFailureException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SubProtocolNegotiationFailureException(string message, Exception innerException) : base(message, innerException) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SubProtocolNegotiator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="SubProtocolNegotiationFailureException"></exception>
        public static string Negotiate(IEnumerable<string> server, IEnumerable<string> client)
        {
            if (!server.Any() || !client.Any())
            {
                return null;
            }

            var matches = client.Intersect(server);
            if (!matches.Any())
            {
                throw new SubProtocolNegotiationFailureException("Unable to negotiate a subprotocol");
            }
            return matches.First();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        public WebSocketException(ushort statusCode) : base()
        {
            StatusCode = statusCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        public WebSocketException(ushort statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WebSocketException(ushort statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
        /// <summary>
        /// 
        /// </summary>
        public ushort StatusCode { get; private set; }
    }
}
