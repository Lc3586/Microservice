/**
 * 请求状态码
 */
const enum HttpStatusCodes {
    正常 = 200,
    未登录 = 401,
    无权限 = 403,
    异常 = 500,
    维护中 = 503
}