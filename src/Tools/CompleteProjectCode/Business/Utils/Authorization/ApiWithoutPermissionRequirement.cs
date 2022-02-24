using Microsoft.AspNetCore.Authorization;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 接口无权限身份验证策略
    /// </summary>
    public class ApiWithoutPermissionRequirement :
        IAuthorizationRequirement
    {

    }
}