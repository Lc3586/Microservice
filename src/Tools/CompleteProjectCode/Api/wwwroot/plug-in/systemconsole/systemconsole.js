window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        saLogin: window.BaseUrl + '/sa/login',
        state: window.BaseUrl + '/system-console/state',
        shutdown: window.BaseUrl + '/system-console/shutdown',
        reboot: window.BaseUrl + '/system-console/reboot'
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
    app.use(ElementPlus, {
        locale: ElementPlus.lang.zhCn
    });
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
            modulars: {
                x: 1,
                y: 0,
                data: {},
                coordinate: {}
            },
            delayedEvents: {}
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
                getState();
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
        else {
            const coordinate = main.modulars.coordinate[name];
            main.modulars.data[coordinate.y][coordinate.x].loading = true;
        }
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
            const coordinate = main.modulars.coordinate[name];
            main.modulars.data[coordinate.y][coordinate.x].loading = false;
            if (error)
                main.modulars.data[coordinate.y][coordinate.x].error = error;
        }
    }

    /**
     * 设置状态信息
     *
     * @method setStateInfo
     * 
     * @param {any} modular
     * 
     */
    function setStateInfo(modular) {
        switch (modular.State) {
            case 'none':
                modular.icon = 'el-icon-turn-off';
                modular.explain = '未启用';
                modular.disable = true;
                return;
            case 'free':
                modular.icon = 'el-icon-remove-outline';
                modular.explain = '空闲';
                modular.disable = false;
                break;
            case 'run':
                modular.icon = 'el-icon-loading';
                modular.explain = '运行中';
                modular.disable = false;
                break;
            case 'stop':
                modular.icon = 'el-icon-switch-button';
                modular.explain = '已停止';
                modular.disable = false;
                break;
            default:
                modular.icon = 'el-icon-question';
                modular.explain = '未知';
                modular.disable = true;
                break;
        }
    }

    /**
     *
     * 延时事件 
     *
     * @method delayedEvent
     *
     * @param {Function} handler 处理函数
     * @param {any} params 参数
     * @param {number} event 延时(毫秒)(默认800)
     * @param {string} event 事件名称
     * @param {boolean} repeat 禁止重复(默认禁止)
     *
    */
    function delayedEvent(handler, params, timeout, event, repeat = false) {
        if (!repeat) {
            event ? 1 : event = Date.now();
            if (main.delayedEvents[event])
                window.clearTimeout(main.delayedEvents[event]);
        }

        main.delayedEvents[event] = window.setTimeout(() => { main.delayedEvents[event] = 0; handler(params); }, timeout || 800);
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
                    if (!name) {
                        const count = response.data.Data.length;
                        main.modulars.x = Math.min(Math.ceil(Math.sqrt(count)), 3);
                        main.modulars.y = Math.ceil(count / main.modulars.x);
                        main.modulars.data = new Array(main.modulars.y);
                        main.modulars.coordinate = {};

                        let x = 0,
                            y = 0;

                        response.data.Data.forEach(item => {
                            let modular = {
                                ...item,
                                icon: '',
                                explain: '',
                                loading: false,
                                disable: false,
                                error: ''
                            };

                            setStateInfo(modular);

                            if (!main.modulars.data[y])
                                main.modulars.data[y] = new Array(main.modulars.x);

                            main.modulars.data[y][x] = modular;
                            main.modulars.coordinate[modular.Name] = { x: x, y: y };

                            if (x < main.modulars.x)
                                x++;
                            else {
                                x = 0;
                                y++;
                            }

                            delayedEvent(_name => { getState(_name); }, modular.Name, 1000, `getState-${modular.Name}`);
                        });
                    } else {
                        const coordinate = main.modulars.coordinate[name];
                        let modular = main.modulars.data[coordinate.y][coordinate.x];
                        modular.State = response.data.Data.State;
                        modular.Data = response.data.Data.Data;

                        setStateInfo(modular);

                        delayedEvent(_name => { getState(_name); }, name, 1000, `getState-${name}`);
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
                if (response.data.Success)
                    getState(name);
                else
                    endLoading(name, response.data.Message);
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
                if (response.data.Success)
                    getState(name);
                else
                    endLoading(name, response.data.Message);
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
                if (response.data.Success)
                    getState(name);
                else
                    endLoading(name, response.data.Message);
            })
            .catch((error) => {
                endLoading(name, '重启时发生异常.');
            });
    }
}