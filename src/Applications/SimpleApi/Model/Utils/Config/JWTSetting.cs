using System;

namespace Model.Utils.Config
{
    /// <summary>
    /// JWT配置
    /// </summary>
    public class JWTSetting
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// 算法
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 有效期时长
        /// <para>默认20分钟</para>
        /// </summary>
        public TimeSpan Validity { get; set; } = TimeSpan.FromMinutes(20);
    }
}
