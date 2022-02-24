/**
 * 验证信息
 * */
class AuthenticationInfo {
    constructor() {
        this.RoleTypes = [];
        this.RoleNames = [];
    }

    /**
     * Id
     */
    Id: string;

    /**
     * 用户类型
     */
    UserType: string;

    /**
     * 角色类型
     */
    RoleTypes: string[];

    /**
     * 角色名称
     */
    RoleNames: string[];

    /**
     * 账号
     */
    Account: string;

    /**
     * 昵称
     */
    Nickname: string;

    /**
     * 性别
     */
    Sex: string;

    /**
     * 头像
     */
    Face: string;

    /**
     * 授权时所用的方法
     */
    AuthenticationMethod: string;
}

/**
 * 身份验证模式
 * */
const enum AuthenticationSchema {
    SampleAuthentication = 'SA',
    JWT = 'JWT',
    CAS = 'CAS'
}