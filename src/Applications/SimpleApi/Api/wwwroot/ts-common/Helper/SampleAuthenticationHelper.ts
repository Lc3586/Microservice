/**
 * 简易身份验证帮助类
 * */
class SampleAuthenticationHelper {
    /**
     * 渲染数据
     */
    RenderData: AuthenticationDialoRenderData;

    /**
     * 请求头设置
     */
    Headers: Record<string, string>;

    /**
     * Http请求工具
     */
    Axios: any;

    /**
     * http请求工具实例
     */
    AxiosInstance: any;

    /**
     * 初始化
     * @param renderData 数据
     * @param axios http请求工具
     * @param headers 请求头设置
     * 
     * @returns Http请求工具实例
     */
    async init(renderData: AuthenticationDialoRenderData, axios: any = null, headers: Record<string, string> = null) {
        const promise = new Promise<any>((resolve, reject) => {
            this.RenderData = renderData;
            this.Headers = headers;

            const setup = async () => {
                this.AxiosInstance = this.createAxiosInstance();

                resolve(this.AxiosInstance);
            }

            if (axios != null) {
                this.Axios = axios;
                setup();
            } else {
                ImportHelper.ImportFile(
                    [
                        {
                            Tag: ImportFileTag.JS,
                            Attributes: {
                                type: 'text/javascript',
                                src: ApiUri.BaseUrl + '/utils/axios.min.js'
                            }
                        }],
                    () => {
                        this.Axios = new (<any>window).axios;
                        setup();
                    });
            }
        });

        return promise;
    }

    /**
     * 创建Http请求实例
     * 
     * @returns Http请求工具实例
     */
    private createAxiosInstance(): any {
        const axiosInstance = this.Axios.create({});
        if (this.Headers != null) {
            for (const key in this.Headers) {
                axiosInstance.defaults.headers.common[key] = this.Headers[key];
            }
        }
        return axiosInstance;
    }

    /**
     * 添加登录拦截器
     * @param axiosInstance http请求工具实例
     * @param validationFailed 验证不通过时触发的事件
     * */
    addLoginFilter(axiosInstance: any, validationFailed: () => void) {
        axiosInstance.interceptors.response.use((response) => {
            return response;
        }, (error) => {
            if (error && error.response && error.response.status == 401) {
                validationFailed && validationFailed();
                return Promise.reject(new Error(error.response.data));
            }
            else if (error && error.response && error.response.status == 403) {
                error.message = "无权限!";
                return Promise.reject(new Error(error.response.data));
            }
            else
                return Promise.reject(error);
        });
    }

    /**
     * 身份验证
     * 
     * @returns 身份验证信息
     * */
    async authorized() {
        const promise = new Promise<AuthenticationInfo>(async (resolve, reject) => {
            this.RenderData.Loading = true;
            this.AxiosInstance.post(ApiUri.SAAuthorized).then((response: { data: ResponseData_T<AuthenticationInfo> }) => {
                this.RenderData.Loading = false;
                if (response && response.data && response.data.Success)
                    resolve(response.data.Data);
                else
                    reject(new Error(response.data.Message));
            }).catch(error => {
                console.error(error);
                this.RenderData.Loading = false;
                reject(error);
            });
        });

        return promise;
    }

    /**
     * 登录
     * 
     * */
    async login() {
        const promise = new Promise<AuthenticationInfo>(async (resolve, reject) => {
            console.debug('login!');
            console.debug(this.RenderData);
            this.RenderData.Loading = true;
            let data = {
                Account: this.RenderData.Username,
                Password: this.RenderData.Password
            };
            this.AxiosInstance.post(ApiUri.SALogin, data).then((response: { data: ResponseData_T<AuthenticationInfo> }) => {
                if (response.data.Success) {
                    this.RenderData.Username = '';
                    this.RenderData.Password = '';

                    resolve(response.data.Data);
                }
                else
                    reject(new Error(response.data.Message));
            }).catch(error => {
                console.error(error);
                this.RenderData.Loading = false;
                reject(new Error('登录时发生异常'));
            });
        });

        return promise;
    }

    /**
     * 登出
     * 
     * */
    logout(returnUrl: string) {
        window.location.href = ApiUri.SALogout(returnUrl);
    }
}