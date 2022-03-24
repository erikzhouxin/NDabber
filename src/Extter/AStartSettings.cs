using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 启动设置内容
    /// </summary>
    public abstract class AStartSettings
    {
        /// <summary>
        /// 开始执行
        /// </summary>
        public abstract IAlertMsg Start(FileInfo file, string secret);
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public abstract string GetSecret();
    }
    /// <summary>
    /// 启动设置
    /// </summary>
    public abstract class AStartSettings<T> : AStartSettings
        where T : AStartSettings<T>
    {
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IAlertMsg SetData(T data);
        private string _secret;
        /// <summary>
        /// 获取密钥内容
        /// </summary>
        /// <returns></returns>
        public sealed override string GetSecret()
        {
            return _secret;
        }
        /// <summary>
        /// 开始进行
        /// </summary>
        public sealed override IAlertMsg Start(FileInfo file, string secret)
        {
            if (file == null || !file.Exists) { return AlertMsg.NotFound; }
            var text = File.ReadAllText(file.FullName, Encoding.UTF8);
            if (text.Length <= 32) { return AlertMsg.NotFound; }
            _secret = secret + UserPassword.DefaultPasswordB;
            var content = UserCrypto.GetAesDecrypt(text, _secret);
            var model = content.GetJsonObject<T>();
            var res = SetData(model);
            file.Delete();
            return res;
        }
    }
}
