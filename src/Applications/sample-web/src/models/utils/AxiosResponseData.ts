/**
 * axios 服务器响应数据
 */
interface AxiosResponseData {
    /**
     * 服务器响应的数据
     */
    data?: any;

    /**
     * 服务器响应的HTTP状态码
     * 详情见 HttpStatusCodes
     */
    status: number;

    /**
     * 服务器响应的HTTP状态信息
     */
    statusText: string;

    /**
     * 服务器响应的头
     */
    headers: any;

    /**
     * 为请求提供的配置信息
     */
    config: any;

    /**
     * `request`是生成此响应的请求
     * 它是node.js中最后一个ClientRequest实例（在重定向中）
     * 以及浏览器的XMLHttpRequest实例
     */
    request: any;

    /**
     * 其他属性
     */
    [propName: string]: any;
}