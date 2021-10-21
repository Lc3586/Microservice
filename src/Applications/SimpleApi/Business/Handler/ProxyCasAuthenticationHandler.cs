using GSS.Authentication.CAS.AspNetCore;
using GSS.Authentication.CAS.Security;
using Microservice.Library.Container;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Utils.Config;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Business.Handler
{
    /// <summary>
    /// 代理服务器CAS验证处理类
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class ProxyCasAuthenticationHandler<TOptions> : CasAuthenticationHandler<TOptions>
         where TOptions : CasAuthenticationOptions, new()
    {
        public ProxyCasAuthenticationHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            Config = AutofacHelper.GetService<SystemConfig>();
        }

        private readonly SystemConfig Config;

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = CurrentUri;
            }

            // CSRF
            GenerateCorrelationId(properties);

            var state = Options.StateDataFormat?.Protect(properties);
            var service = $"{Config.WebRootUrlMatchScheme(Context.Request.Scheme)}/cas/pre-signin";
            if (!string.IsNullOrWhiteSpace(state))
                service += $"?state={Uri.EscapeDataString(state)}";
            var authorizationEndpoint = $"{Options.CasServerUrlBase}/login?service={Uri.EscapeDataString(service)}";

            var redirectContext = new RedirectContext<CasAuthenticationOptions>(
                Context, Scheme, Options,
                properties, authorizationEndpoint);

            await Options.Events.RedirectToAuthorizationEndpoint(redirectContext).ConfigureAwait(false);
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var query = Request.Query;
            var state = query["state"];
            var properties = Options.StateDataFormat?.Unprotect(state);

            if (properties == null)
            {
                return HandleRequestResult.Fail("The state was missing or invalid.");
            }

            // CSRF
            if (!ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.");
            }

            var serviceTicket = query["ticket"];

            if (string.IsNullOrEmpty(serviceTicket))
            {
                return HandleRequestResult.Fail("Missing CAS ticket.");
            }

            var service = $"{Config.WebRootUrlMatchScheme(Context.Request.Scheme)}/cas/pre-signin?state={Uri.EscapeDataString(state)}";
            ICasPrincipal? principal = null;
            if (Options.ServiceTicketValidator != null)
            {
                principal = await Options.ServiceTicketValidator
                    .ValidateAsync(serviceTicket, service, Context.RequestAborted).ConfigureAwait(false);
            }

            if (principal == null)
            {
                return HandleRequestResult.Fail("Missing Validate Principal.");
            }

            if (Options.SaveTokens)
            {
                properties.StoreTokens(new List<AuthenticationToken>
                {
                    new AuthenticationToken {Name = "access_token", Value = serviceTicket}
                });
            }

            var ticket = await CreateTicketAsync(principal as ClaimsPrincipal ?? new ClaimsPrincipal(principal),
                properties, principal.Assertion).ConfigureAwait(false);

            return ticket != null
                ? HandleRequestResult.Success(ticket)
                : HandleRequestResult.Fail("Failed to retrieve user information from remote server.");
        }
    }
}
