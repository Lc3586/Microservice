/* 
 * SampleAuthentication相关业务模型
 */
using System.Collections.Generic;

namespace Model.Utils.SampleAuthentication.SampleAuthenticationDTO
{
    /// <summary>
    /// 登录参数
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 验证信息
    /// </summary>
    public class AuthenticationInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public List<string> RoleTypes { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public List<string> RoleNames { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Face { get; set; }

        /// <summary>
        /// 授权时所用的方法
        /// </summary>
        public string AuthenticationMethod { get; set; }
    }

    /// <summary>
    /// 登出参数
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// 登出后跳转地址
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
