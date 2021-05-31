using System;

/* 
 * JWT相关业务模型
 */
namespace Model.Utils.JWT.JWTDTO
{
    /// <summary>
    /// 令牌信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires { get; set; }
    }
}
