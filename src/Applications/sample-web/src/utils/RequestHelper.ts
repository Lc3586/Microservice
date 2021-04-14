import axios from 'axios';
import { MessageBox, Message } from 'element-ui';
// import store from '@/store';

/**
 * 接口请求帮助类
 */
class RequestHelper {
    /**
     * axios实例
     */
    private service: any;

    constructor() {
        // 创建axios实例
        this.service = axios.create({
            baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
            withCredentials: true, // send cookies when cross-domain requests
            timeout: 10000 // 超时时间
        });

        // 请求拦截器
        this.service.interceptors.request.use(
            (config: any) => {
                // 在发送请求之前做些什么
                return config;
            },
            (error: any) => {
                // 对请求错误做些什么
                console.debug(error);
                return Promise.reject(error);
            }
        )

        // 输出拦截器
        this.service.interceptors.response.use(
            (response: AxiosResponseData) => {
                //对响应数据做点什么
                if (response.status === HttpStatusCodes.异常) {
                    Message({
                        message: response.data || '系统繁忙',
                        type: 'error',
                        duration: 5 * 1000
                    });
                } else if (response.status === HttpStatusCodes.维护中) {
                    MessageBox.confirm('站点维护中，请稍后再试。', '站点维护', {
                        confirmButtonText: '确认',
                        type: 'warning'
                    });
                } else if (response.status === HttpStatusCodes.未登录) {
                    MessageBox.confirm('您的登录信息已失效，请点击重新登录按钮去登录, 或者您也可以点击取消按钮停留在此页面。', '登录确认', {
                        confirmButtonText: '重新登录',
                        cancelButtonText: '取消',
                        type: 'warning'
                    });
                } else if (response.status === HttpStatusCodes.无权限) {
                    MessageBox.confirm(response.data, '无权限', {
                        confirmButtonText: '确认',
                        type: 'warning'
                    });
                } else {
                    return response;
                }

                return Promise.reject(new Error(response.data));
            },
            (error: any) => {
                console.debug('异常：' + error);
                Message({
                    message: error.message,
                    type: 'error',
                    duration: 5 * 1000
                });
                return Promise.reject(error);
            }
        )
    };

    create(): any {
        return this.service;
    }
}
