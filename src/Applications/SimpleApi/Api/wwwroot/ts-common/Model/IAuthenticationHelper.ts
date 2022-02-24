/**
 * 身份验证帮助类接口
 * */
interface IAuthenticationHelper {
    /**
     * 添加登录拦截器
     * @param validationFailed 验证不通过时触发的事件
     * */
    addLoginFilter(validationFailed: () => void);

    /**
     * 身份验证
     * 
     * @returns 成功时返回身份验证信息，失败则可视为未登录
     * */
    authorized(): Promise<AuthenticationInfo>;

    /**
     * 登录
     * 
     * @returns 身份验证信息
     * */
    login(): Promise<AuthenticationInfo>;

    /**
     * 登出
     * 
     * */
    logout(): Promise<void>;
}