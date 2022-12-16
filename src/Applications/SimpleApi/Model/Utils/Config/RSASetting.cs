using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.Config
{
    /// <summary>
    /// RSA配置
    /// </summary>
    public class RSASetting
    {
        /// <summary>
        /// 使用证书
        /// </summary>
        public bool UseCertFile { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// p12证书地址
        /// </summary>
        public string CertFilePath { get; set; }

        /// <summary>
        /// 证书密码
        /// <para>一般默认为商户号</para>
        /// </summary>
        public string CertPassword { get; set; }

        /// <summary>
        /// pem公钥文件地址
        /// </summary>
        public string PemFilePath { get; set; }
    }
}
