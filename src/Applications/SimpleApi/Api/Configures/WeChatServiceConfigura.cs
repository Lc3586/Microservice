using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.RegisterServices;

namespace Api.Configures
{
    /// <summary>
    /// 微信配置类
    /// </summary>
    public static class WeChatServiceConfigura
    {
        /// <summary>
        /// 注册WeChat服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterWeChat(
            this IServiceCollection services,
            IConfiguration configuration,
            SystemConfig config)
        {
            services.AddWeChatService(configuration, option =>
            {
                option.WeChatDevOptions.TokenVerificationUrl = new PathString(config.WeChatService.TokenVerificationUrl);
                option.WeChatDevOptions.Token = config.WeChatService.Token;

                option.WeChatOAuthOptions.WebRootUrl = config.WebRootUrl;
                option.WeChatOAuthOptions.OAuthBaseUrl = new PathString(config.WeChatService.OAuthBaseUrl);
                option.WeChatOAuthOptions.OAuthUserInfoUrl = new PathString(config.WeChatService.OAuthUserInfoUrl);
                option.WeChatOAuthOptions.AuthorizeUrl = config.WeChatService.AuthorizeUrl;
                option.WeChatOAuthOptions.AccessTokenUrl = config.WeChatService.AccessTokenUrl;
                option.WeChatOAuthOptions.UserInfoUrl = config.WeChatService.UserInfoUrl;

                option.WeChatBaseOptions.AppId = config.WeChatService.AppId;
                option.WeChatBaseOptions.Appsecret = config.WeChatService.Appsecret;
                option.WeChatBaseOptions.MchId = config.WeChatService.MchId;
                option.WeChatBaseOptions.Key = config.WeChatService.Key;
                option.WeChatBaseOptions.PayNotifyUrl = config.WeChatService.PayNotifyUrl;
                option.WeChatBaseOptions.RefundNotifyUrl = config.WeChatService.RefundNotifyUrl;
                option.WeChatBaseOptions.UserHostAddress = config.WeChatService.UserHostAddress;
                option.WeChatBaseOptions.CertFilePath = config.WeChatService.CertFilePath;
                option.WeChatBaseOptions.CertPassword = config.WeChatService.CertPassword;
                option.WeChatBaseOptions.PemFilePath = config.WeChatService.PemFilePath;
            });

            return services;
        }

        /// <summary>
        /// 配置WeChat
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraWeChat(this IApplicationBuilder app, SystemConfig config)
        {
            app.UseWeChatTokenVerification();
            app.UseWeChatOAuthV2();

            return app;
        }
    }
}
