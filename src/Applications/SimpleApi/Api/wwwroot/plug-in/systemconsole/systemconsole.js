window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        saLogin: '/sa/login',
        state: '/system-console/state',
        shutdown: '/system-console/shutdown',
        reboot: '/system-console/reboot'
    };

    //vue实例
    const Main = {
        data: getData,
        created() {
            main = this;
            addLoginFilter();
            getState();
        },
        methods: {
            saLogin,
            getState,
            shutdown,
            start,
            reboot
        }
    };

    const app = Vue.createApp(Main);
    ElementPlus.locale(ElementPlus.lang.zhCn);
    app.use(ElementPlus);
    app.mount("#app");

    /**
     * 获取渲染数据
     */
    function getData() {
        return {
            loading: true,
            init: false,
            error: '',
            sa: {
                show: false,
                loading: true,
                username: '',
                password: ''
            },
            models: {
                data: {},
                x: 1,
                y: 0
            }
        };
    }

    /**
     * 添加登录拦截器
     * */
    function addLoginFilter() {
        axios.interceptors.response.use((response) => {
            return response;
        }, (error) => {
            if (error && error.response && error.response.status == 401)
                main.sa.show ? 1 : main.sa.show = true;
            else if (error && error.response && error.response.status == 403) {
                error.message = "无权限!";
                return Promise.reject(error);
            }
            else
                return Promise.reject(error);
        });
    }

    /**
     * SA身份验证
     * */
    function saLogin() {
        main.sa.loading = true;
        axios.post(apiUrls.saLogin, {
            Account: main.sa.username,
            Password: main.sa.password
        }).then(function (response) {
            main.sa.loading = false;
            if (response.data.Success) {
                main.sa.username = '';
                main.sa.Password = '';
                main.sa.show = false;
                getLogConfig();
            }
            else
                ElementPlus.ElMessage(response.data.Message);
        }).catch(function (error) {
            main.sa.loading = false;
            ElementPlus.ElMessage('SA身份验证时发生异常.');
        });
    }

    /**
     * 加载中
     *
     * @method loading
     *
     * @param {string} name 名称
     *
     * */
    function loading(name = null) {
        if (!name)
            main.loading = true;
        else
            main.models.data[name].loading = true;
    }

    /**
     * 加载结束
     *
     * @method endLoading
     *
     * @param {string} name 名称
     * @param {string} error 异常信息
     *
     * */
    function endLoading(name = null, error = null) {
        if (error)
            ElementPlus.ElMessage(error);

        if (!name) {
            main.loading = false;
            if (error)
                main.error = error;
        }
        else {
            main.models.data[name].loading = false;
            if (error)
                main.models.data[name].error = error;
        }
    }

    /**
     * 设置状态信息
     *
     * @method setStateInfo
     * 
     * @param {any} model
     * 
     */
    function setStateInfo(model) {
        switch (model.state) {
            case 'none':
                model.icon = 'el-icon-turn-off';
                model.explain = '未启用';
                model.disable = true;
                return;
            case 'free':
                model.icon = 'el-icon-remove-outline';
                model.explain = '空闲';
                break;
            case 'run':
                model.icon = 'el-icon-loading';
                model.explain = '运行中';
                break;
            case 'stop':
                model.icon = 'el-icon-switch-button';
                model.explain = '已停止';
                break;
            default:
                model.icon = 'el-icon-question';
                model.explain = '未知';
                model.disable = true;
                break;
        }
    }

    /**
     * 获取状态
     * 
     * @method getState
     * 
     * @param {string} name 名称
     * 
     * */
    function getState(name = null) {
        loading(name);

        axios.post(apiUrls.state + (!name ? '' : `?name=${name}`))
            .then((response) => {
                if (response.data.Success) {
                    var count = Object.keys(response.data.Data).length;
                    main.models.x = Math.ceil(Math.sqrt(count));
                    main.models.y = Math.ceil(count / main.models.x);

                    if (count > 0)
                        for (let key in response.data.Data) {
                            let model = {
                                state: response.data.Data[key],
                                icon: '',
                                explain: '',
                                loading: false,
                                disable: false,
                                error: ''
                            };

                            setStateInfo(model);

                            main.models.data[name] = model;
                        }

                    endLoading(name);
                }
                else {
                    endLoading(name, response.data.Message);
                }
            })
            .catch((error) => {
                endLoading(name, '获取状态时发生异常.');
            });
    }

    /**
     * 关停
     *
     * @method shutdown
     *
     * @param {string} name 名称
     * 
     * */
    function shutdown(name = null) {
        loading(name);

        axios.post(apiUrls.shutdown + (!name ? '' : `?name=${name}`))
            .then((response) => {
                if (!response.data.Success) {
                    getState(name);
                }
                else {
                    endLoading(name, response.data.Message);
                }
            })
            .catch((error) => {
                endLoading(name, '关停时发生异常.');
            });
    }

    /**
     * 启动
     *
     * @method start
     *
     * @param {string} name 名称
     * 
     * */
    function start(name = null) {
        loading(name);

        axios.post(apiUrls.reboot + (!name ? '' : `?name=${name}`))
            .then((response) => {
                if (!response.data.Success) {
                    getState(name);
                }
                else {
                    endLoading(name, response.data.Message);
                }
            })
            .catch((error) => {
                endLoading(name, '启动时发生异常.');
            });
    }

    /**
     * 重启
     *
     * @method reboot
     *
     * @param {string} name 名称
     * 
     * */
    function reboot(name = null) {
        loading(name);

        axios.post(apiUrls.reboot + (!name ? '' : `?name=${name}`))
            .then((response) => {
                if (!response.data.Success) {
                    getState(name);
                }
                else {
                    endLoading(name, response.data.Message);
                }
            })
            .catch((error) => {
                endLoading(name, '重启时发生异常.');
            });
    }
}