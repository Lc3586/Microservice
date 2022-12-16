using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// SignalR 集线器身份验证策略
    /// </summary>
    public class SignalRHubRequirement :
        IAuthorizationRequirement
    {

    }
}
