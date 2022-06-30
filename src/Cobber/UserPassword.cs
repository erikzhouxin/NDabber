using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 用户密码
    /// </summary>
    public partial class UserPassword
    {
        /// <summary>
        /// 获取加密密码委托
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <param name="isLower"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public delegate String ComputeHash(string pass, string salt, bool isLower, Encoding encoding);
        /// <summary>
        /// 默认加密口令8
        /// </summary>
        public static string DefaultPassword8 { get; } = "EZhouXin";
        /// <summary>
        /// 默认安全防护密码10
        /// </summary>
        public static string DefaultPasswordA { get; } = "Ezx2020!@#";
        /// <summary>
        /// 默认通用密码11
        /// </summary>
        public static string DefaultPasswordB { get; } = "72B83BAE4DD79440B0EF1D3A39A7D1570792F5E0";
        /// <summary>
        /// 默认通用密码11
        /// </summary>
        public static string DefaultPasswordC { get; } = "430e105a7782b1bc15c5e80dcc8264072d7efe90";
        /// <summary>
        /// 默认密码10
        /// </summary>
        public static string DefaultPassword { get; set; } = DefaultPasswordA;
        private static EType _defaultType = EType.SHA1;
        private static bool _defaultCase = false;
        private static ComputeHash _defaultCompute;
        /// <summary>
        /// 默认类型(优先于计算方式)
        /// </summary>
        public static EType DefaultType
        {
            get => _defaultType;
            set
            {
                _defaultCase = GetIsLower(_defaultType = value);
                if (_defaultType != EType.Custom && _defaultType != EType.CustomL)
                {
                    _defaultCompute = null;
                }
            }
        }
        /// <summary>
        /// 默认大小写
        /// </summary>
        public static bool DefaultCase
        {
            get => _defaultCase;
            set => _defaultType = ChangeType(_defaultType, _defaultCase = value);
        }
        /// <summary>
        /// 默认类型
        /// </summary>
        public static ComputeHash DefaultCompute
        {
            get => _defaultCompute;
            set
            {
                _defaultType = value == null ? (_defaultCase ? EType.SHA1L : EType.SHA1) : (_defaultCase ? EType.CustomL : EType.Custom);
                _defaultCompute = value;
            }
        }
        /// <summary>
        /// 默认编码
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        private string _origin;
        private string _salt;
        private LazyBone<string> _hash;
        private EType _type = DefaultType;
        private Encoding _encoding = DefaultEncoding;
        private ComputeHash _getHash;
        private bool _isLower;
        /// <summary>
        /// 默认密码字符初始化
        /// </summary>
        public UserPassword() : this(DefaultPassword, CobberCaller.GuidEmpty, DefaultEncoding, DefaultType, DefaultCompute) { }
        /// <summary>
        /// 设定密码字符初始化,保存密码为设定密码
        /// </summary>
        /// <param name="origin">密码字符串</param>
        public UserPassword(string origin) : this(origin, CobberCaller.GuidString, DefaultEncoding, DefaultType, DefaultCompute) { }
        /// <summary>
        /// 完整密码初始化,保存密码为设定密码,盐值为设定值
        /// </summary>
        /// <param name="origin">新密码</param>
        /// <param name="salt">盐值</param>
        public UserPassword(string origin, Guid salt) : this(origin, salt.GetString(), DefaultEncoding, DefaultType, DefaultCompute) { }
        /// <summary>
        /// 完整密码初始化,保存密码为设定密码,盐值为设定值
        /// </summary>
        /// <param name="origin">新密码</param>
        /// <param name="salt">盐值</param>
        public UserPassword(string origin, string salt) : this(origin, salt, DefaultEncoding, DefaultType, DefaultCompute) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="type"></param>
        public UserPassword(string origin, EType type) : this(origin, string.Empty, DefaultEncoding, type, null) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, ComputeHash GetHash) : this(origin, string.Empty, DefaultEncoding, EType.Custom, GetHash) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="encoding"></param>
        public UserPassword(string origin, Encoding encoding) : this(origin, string.Empty, encoding, DefaultType, DefaultCompute) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="type"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, EType type, ComputeHash GetHash) : this(origin, string.Empty, DefaultEncoding, type, GetHash) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="type"></param>
        public UserPassword(string origin, string salt, EType type) : this(origin, salt, DefaultEncoding, type, null) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, string salt, ComputeHash GetHash) : this(origin, salt, DefaultEncoding, EType.Custom, GetHash) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="type"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, string salt, EType type, ComputeHash GetHash) : this(origin, salt, DefaultEncoding, type, GetHash) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="encoding"></param>
        /// <param name="type"></param>
        public UserPassword(string origin, string salt, Encoding encoding, EType type) : this(origin, salt, encoding, type, null) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="encoding"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, string salt, Encoding encoding, ComputeHash GetHash) : this(origin, salt, encoding, EType.Custom, GetHash) { }
        /// <summary>
        /// 构造密码初始化
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="salt"></param>
        /// <param name="encoding"></param>
        /// <param name="type"></param>
        /// <param name="GetHash"></param>
        public UserPassword(string origin, string salt, Encoding encoding, EType type, ComputeHash GetHash)
        {
            _origin = origin;
            _salt = salt;
            _encoding = encoding;
            _type = type;
            _getHash = GetHash;
            _isLower = GetIsLower(type);
            _hash = new LazyBone<string>(Compute, true);
        }
        /// <summary>
        /// 实时计算密码值
        /// </summary>
        /// <returns></returns>
        public string Compute()
        {
            switch (_type)
            {
                case EType.SHA1:
                case EType.SHA1L: _getHash = GetSha1Hash; break;
                case EType.SHA256:
                case EType.SHA256L: _getHash = GetSha256Hash; break;
                case EType.SHA384:
                case EType.SHA384L: _getHash = GetSha384Hash; break;
                case EType.SHA512:
                case EType.SHA512L: _getHash = GetSha512Hash; break;
                case EType.MD5:
                case EType.MD5L: _getHash = GetMd5Hash; break;
                case EType.Custom:
                default: _getHash ??= GetSha1Hash; break;
            }
            return _getHash.Invoke(_origin, _salt, _isLower, _encoding);
        }
        /// <summary>
        /// 原密码
        /// </summary>
        public String Origin
        {
            get => _origin;
            set
            {
                _origin = value;
                _hash.Reload();
            }
        }
        /// <summary>
        /// 盐值字符
        /// </summary>
        public string Salt
        {
            get => _salt;
            set
            {
                _salt = value;
                _hash.Reload();
            }
        }
        /// <summary>
        /// 生成方式
        /// </summary>
        public EType Type
        {
            get => _type;
            set
            {
                _type = value;
                _hash.Reload();
            }
        }
        /// <summary>
        /// 是小写
        /// </summary>
        public bool IsLower
        {
            get => _isLower;
            set
            {
                _type = ChangeType(_type, _isLower = value);
                _hash.Reload();
            }
        }
        /// <summary>
        /// 加密值
        /// </summary>
        public string Hash { get => _hash.Value; }
        /// <summary>
        /// 字符编码
        /// </summary>
        public Encoding Encoding
        {
            get => _encoding;
            set
            {
                _encoding = value ?? Encoding.UTF8;
                _hash.Reload();
            }
        }
        /// <summary>
        /// 设置
        /// </summary>
        public ComputeHash GetHash
        {
            get => GetHash;
            set
            {
                _type = value == null ? (_isLower ? EType.SHA1L : EType.SHA1) : (_isLower ? EType.CustomL : EType.Custom);
                _getHash = value;
                _hash.Reload();
            }
        }
        #region // SHA384值
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass) => UserCrypto.GetSha384HexString(pass, DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, bool isLower) => UserCrypto.GetSha384HexString(pass, isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, Encoding encoding) => UserCrypto.GetSha384HexString(pass, DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, string salt) => UserCrypto.GetSha384HexString($"{pass}{salt}", DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, string salt, bool isLower) => UserCrypto.GetSha384HexString($"{pass}{salt}", isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, string salt, Encoding encoding) => UserCrypto.GetSha384HexString($"{pass}{salt}", DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, string salt, bool isLower, Encoding encoding) => UserCrypto.GetSha384HexString($"{pass}{salt}", isLower, encoding);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha384Hash(string pass, bool isLower, Encoding encoding) => UserCrypto.GetSha384HexString(encoding.GetBytes(pass), isLower);
        #endregion
        #region // SHA512值
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass) => UserCrypto.GetSha512HexString(pass, DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, bool isLower) => UserCrypto.GetSha512HexString(pass, isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, Encoding encoding) => UserCrypto.GetSha512HexString(pass, DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, string salt) => UserCrypto.GetSha512HexString($"{pass}{salt}", DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, string salt, bool isLower) => UserCrypto.GetSha512HexString($"{pass}{salt}", isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, string salt, Encoding encoding) => UserCrypto.GetSha512HexString($"{pass}{salt}", DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, string salt, bool isLower, Encoding encoding) => UserCrypto.GetSha512HexString($"{pass}{salt}", isLower, encoding);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, bool isLower, Encoding encoding) => UserCrypto.GetSha512HexString(pass, isLower, encoding);
        #endregion
        #region // SHA256值
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass) => UserCrypto.GetSha256HexString(pass, DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, bool isLower) => UserCrypto.GetSha256HexString(pass, isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, Encoding encoding) => UserCrypto.GetSha256HexString(pass, DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, string salt) => UserCrypto.GetSha256HexString($"{pass}{salt}", DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, string salt, bool isLower) => UserCrypto.GetSha256HexString($"{pass}{salt}", isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, string salt, Encoding encoding) => UserCrypto.GetSha256HexString($"{pass}{salt}", DefaultCase, encoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, string salt, bool isLower, Encoding encoding) => UserCrypto.GetSha256HexString($"{pass}{salt}", isLower, encoding);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, bool isLower, Encoding encoding) => UserCrypto.GetSha256HexString(pass, isLower, encoding);
        #endregion
        #region // SHA1值
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass) => UserCrypto.GetSha1HexString(pass, false, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, bool isLower) => UserCrypto.GetSha1HexString(pass, isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, Encoding encoding) => UserCrypto.GetSha1HexString(pass, false, encoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, string salt) => UserCrypto.GetSha1HexString($"{pass}{salt}", false, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, string salt, bool isLower) => UserCrypto.GetSha1HexString($"{pass}{salt}", isLower, DefaultEncoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, string salt, Encoding encoding) => UserCrypto.GetSha1HexString($"{pass}{salt}", false, encoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, string salt, bool isLower, Encoding encoding) => UserCrypto.GetSha1HexString($"{pass}{salt}", isLower, encoding);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, bool isLower, Encoding encoding) => UserCrypto.GetSha1HexString(pass, isLower, encoding);
        #endregion
        #region // MD5值
        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass) => UserCrypto.GetMd5HexString(pass, DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, bool isLower) => UserCrypto.GetMd5HexString(pass, isLower, DefaultEncoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, Encoding encoding) => UserCrypto.GetMd5HexString(pass, DefaultCase, encoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt) => UserCrypto.GetMd5HexString($"{pass}{salt}", DefaultCase, DefaultEncoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt, bool isLower) => UserCrypto.GetMd5HexString($"{pass}{salt}", isLower, DefaultEncoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt, Encoding encoding) => UserCrypto.GetMd5HexString($"{pass}{salt}", DefaultCase, encoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt, bool isLower, Encoding encoding) => UserCrypto.GetMd5HexString($"{pass}{salt}", isLower, encoding);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, bool isLower, Encoding encoding) => UserCrypto.GetMd5HexString(pass, isLower, encoding);
        #endregion
        /// <summary>
        /// 加密类型
        /// </summary>
        public enum EType
        {
            /// <summary>
            /// SHA-1加密,返回大写串
            /// </summary>
            SHA1 = 0,
            /// <summary>
            /// SHA-1加密,返回小写串
            /// </summary>
            SHA1L = 10,
            /// <summary>
            /// SHA-256加密,返回大写串
            /// </summary>
            SHA256 = 20,
            /// <summary>
            /// SHA-256加密,返回小写串
            /// </summary>
            SHA256L = 30,
            /// <summary>
            /// SHA-384加密,返回大写串
            /// </summary>
            SHA384 = 40,
            /// <summary>
            /// SHA-384加密,返回小写串
            /// </summary>
            SHA384L = 50,
            /// <summary>
            /// SHA-512加密,返回大写串
            /// </summary>
            SHA512 = 60,
            /// <summary>
            /// SHA-512加密,返回小写串
            /// </summary>
            SHA512L = 70,
            /// <summary>
            /// MD5加密,返回大写串
            /// </summary>
            MD5 = 80,
            /// <summary>
            /// MD5加密,返回小写串
            /// </summary>
            MD5L = 90,
            /// <summary>
            /// 自定义加密
            /// </summary>
            Custom = 900,
            /// <summary>
            /// 自定义加密
            /// </summary>
            CustomL = 990,
        }
        #region // 私有方法
        private static bool GetIsLower(EType type)
        {
            switch (type)
            {
                case EType.SHA1L:
                case EType.SHA256L:
                case EType.SHA384L:
                case EType.SHA512L:
                case EType.MD5L:
                case EType.CustomL:
                    return true;
                case EType.SHA1:
                case EType.SHA256:
                case EType.SHA384:
                case EType.SHA512:
                case EType.MD5:
                case EType.Custom:
                default: return false;
            }
        }
        private static EType ChangeType(EType type, bool isLower)
        {
            if (isLower)
            {
                switch (type)
                {
                    case EType.SHA1: return EType.SHA1L;
                    case EType.SHA256: return EType.SHA256L;
                    case EType.SHA384: return EType.SHA384L;
                    case EType.SHA512: return EType.SHA512L;
                    case EType.MD5: return EType.MD5L;
                    case EType.Custom: return EType.CustomL;
                    case EType.SHA1L:
                    case EType.SHA256L:
                    case EType.SHA384L:
                    case EType.SHA512L:
                    case EType.MD5L:
                    case EType.CustomL:
                    default: return type;
                }
            }
            switch (type)
            {
                case EType.SHA1L: return EType.SHA1;
                case EType.SHA256L: return EType.SHA256;
                case EType.SHA384L: return EType.SHA384;
                case EType.SHA512L: return EType.SHA512;
                case EType.MD5L: return EType.MD5;
                case EType.CustomL: return EType.Custom;
                case EType.SHA1:
                case EType.SHA256:
                case EType.SHA384:
                case EType.SHA512:
                case EType.MD5:
                case EType.Custom:
                default: return type;
            }
        }
        #endregion
    }
}
