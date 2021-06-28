window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        saLogin: '/sa/login',
        genTypes: '/cagc/gen-types',
        generateByCSV: '/cagc/generate-by-csv',
        download: '/cagc/download',
        cagcHub: '/cagchub',
        getTempInfo: '/cagc/temp-info',
        clearTemp: '/cagc/clear-temp'
    };

    //vue实例
    const Main = {
        data: getData,
        created() {
            main = this;
            addLoginFilter();
            getConfig();
        },
        methods: {
            saLogin,
            tabsChanged,
            mouseenterOutput,
            mouseleaveOutput,
            genTypeChange,
            nextStep,
            generateByCSVBefore,
            generateByCSVProgress,
            generateByCSVSuccess,
            generateByCSVError,
            download,
            refreshTemp,
            clearTemp
        },
        watch: {
            'output.list': {
                handler(val) {
                    if (this.output.scroll)
                        this.$nextTick(() => {
                            var container = this.$refs['output'];
                            container.scrollTop = container.scrollHeight;
                        });
                },
                deep: true
            }
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
            sa: {
                show: false,
                loading: true,
                username: '',
                password: ''
            },
            overall: {
                state: 'el-icon-loading',
                title: '...',
                explain: '...',
                warn: 0,
                error: 0
            },
            config: {
                apiUrls: apiUrls,
                configType: 'Default',
                types: [],
                activeNames: ['csv'],
                step: 1,
                genType: "EnrichmentProject",
                uploading: false,
                uploadPercentage: 0,
                download: false,
                downloadKey: null
            },
            output: {
                init: false,
                loading: true,
                scroll: true,
                scrollLocked: false,
                receive: true,
                connection: null,
                list: [],
                search: ''
            },
            temp: {
                loading: false,
                explain: '',
                Size: '',
                disable: false
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
                getConfig();
            }
            else
                ElementPlus.ElMessage(response.data.Message);
        }).catch(function (error) {
            main.sa.loading = false;
            ElementPlus.ElMessage('SA身份验证时发生异常.');
        });
    }

    /**
     * 获取配置
     * */
    function getConfig() {
        axios.all([genTypes()])
            .then(axios.spread((types) => {
                main.loading = false;

                if (types.data.Success) {
                    main.config.types = types.data.Data;
                    switchToPane(main.config.configType);
                }
                else
                    ElementPlus.ElMessage(types.data.Message);

            }))
            .catch((error) => {
                main.loading = false;
                if (!main.sa.show)
                    ElementPlus.ElMessage('获取配置时发生异常.');
            });
    }

    /**
     * 获取所有生成类型
     * */
    function genTypes() {
        return axios.get(apiUrls.genTypes);
    }

    /**
     * 切换标签时的点击事件
     * @param {any} tab
     */
    function tabsChanged(tab) {
        switchToPane(tab.props.name);
    }

    /**
     * 切换至指定的标签
     *
     * @param {string} configType 配置类型
     * */
    function switchToPane(configType) {
        main.config.configType = configType;
        switch (configType) {
            case 'Default':
            default:
                if (main.output.init) {
                    main.$nextTick(() => {
                        var container = main.$refs['output'].parentElement;
                        container.scrollTop = container.scrollHeight;
                    });
                    return;
                }
                connectToCAGCHub();
                break;
            case 'Custom':
                ElementPlus.ElMessage('暂不支持.');
                break;
            case "Temp":
                getTempInfo();
                break;
        }
    }

    /**
     * 鼠标移入输出容器
     * */
    function mouseenterOutput() {
        if (!main.output.scrollLocked)
            main.output.scroll = false;
    }

    /**
     * 鼠标移出输出容器
     * */
    function mouseleaveOutput() {
        if (!main.output.scrollLocked)
            main.output.scroll = true;
    }

    /**
     * 连接至日志中心
     * */
    function connectToCAGCHub() {
        main.output.loading = true;
        main.output.connection = new signalR.HubConnectionBuilder()
            .withUrl(apiUrls.cagcHub)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        main.output.connection.onreconnecting(error => {
            main.overall.title = '连接已断开';
            main.overall.explain = '正在尝试重新连接...';
        });
        main.output.connection.onreconnected(connectionId => {
            console.info(connectionId);
            main.overall.title = '连接已恢复';
            main.overall.explain = '已重新连接至服务器';
        });
        main.output.connection.onclose(() => {
            main.overall.title = '连接已关闭';
            main.overall.explain = '...';
        });

        main.output.connection.on("ReceiveInfo", data => {
            if (!main.output.receive)
                return;

            main.output.list.push({
                content: data,
                icon: 'el-icon-info',
                size: 'large',
                timestamp: dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss.SSS')
            });
        });
        main.output.connection.on("ReceiveError", data => {
            if (!main.output.receive)
                return;

            main.output.list.push({
                content: data,
                icon: 'el-icon-error',
                size: 'large',
                timestamp: dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss.SSS')
            });
        });

        main.output.connection
            .start()
            .then(() => {
                main.overall.title = '已连接';
                main.overall.explain = '...';

                main.output.loading = false;
                main.output.init = true;

                openOutput();
            });
    }

    /**
     * 开启输出
     * @param {any} done 回调
     */
    function openOutput(done) {
        return main.output.connection
            .invoke('Open')
            .then(() => {
                main.output.receive = true;
                done && done();
            })
            .catch(error => {
                ElementPlus.ElMessage('开启输出失败');
            });
    }

    /**
     * 关闭输出
     * @param {any} done 回调
     */
    function closeOutput(done) {
        return main.output.connection
            .invoke('Close')
            .then(() => {
                main.output.receive = false;
                done && done();
            })
            .catch(error => {
                ElementPlus.ElMessage('关闭输出失败');
            });
    }

    /**
     * 切换生成类型
     * */
    function genTypeChange() {
        main.config.step = 2;
    }

    /**
     * 下一步
     * */
    function nextStep() {
        main.config.step = main.config.step == 1 ? 2 : 1;
    }

    /**
     * 准备上传CSV文件
     * @param {any} file 当前文件
     * */
    function generateByCSVBefore(file) {
        main.config.uploadPercentage = 0;
        main.config.uploading = true;
    }

    /**
     * CSV文件上传进度
     * @param {any} event 事件
     * @param {any} file 当前文件
     * @param {any} fileList 文件列表
     * */
    function generateByCSVProgress(event, file, fileList) {
        main.config.uploadPercentage = event.percent;
    }

    /**
     * CSV文件上传成功
     * @param {any} response 输出
     * @param {any} file 当前文件
     * @param {any} fileList 文件列表
     * */
    function generateByCSVSuccess(response, file, fileList) {
        main.$refs['upload-file'].clearFiles();
        main.config.uploading = false;

        ElementPlus.ElMessage(response.Message);

        if (response.Success) {
            main.config.download = true;
            main.config.downloadKey = response.Data;
        }
    }

    /**
     * CSV文件上传失败
     * @param {any} err 错误
     * @param {any} file 当前文件
     * @param {any} fileList 文件列表
     * */
    function generateByCSVError(err, file, fileList) {
        main.$refs['upload-file'].clearFiles();
        main.config.uploading = false;
        console.info(err);
        console.info(fileList);
        console.info(file);
    }

    /**
     * 下载
     * */
    function download() {
        window.open(`${apiUrls.download}/${main.config.downloadKey}`);
    }

    /**
     * 获取缓存信息
     * */
    function getTempInfo() {
        main.temp.disable = true;
        main.temp.loading = true;

        axios.get(apiUrls.getTempInfo)
            .then((response) => {
                if (response.data.Success) {
                    main.temp.Size = response.data.Data.OccupiedSpace;
                    main.temp.explain = `占用空间${main.temp.Size}, 包含${response.data.Data.FileCount}个文件.`;

                    if (response.data.Data.FileCount > 0)
                        main.temp.disable = false
                }
                else {
                    ElementPlus.ElMessage(response.data.Message);
                }
                main.temp.loading = false;
            })
            .catch((error) => {
                main.temp.loading = false;
                ElementPlus.ElMessage('获取缓存信息时发生异常.');
            });
    }

    /**
     * 刷新缓存信息
     * */
    function refreshTemp() {
        getTempInfo();
    }

    /**
     * 清理缓存
     * */
    function clearTemp() {
        main.temp.disable = true;

        axios.get(apiUrls.clearTemp)
            .then((response) => {
                ElementPlus.ElMessage(response.data.Message);
                if (response.data.Success) {
                    getTempInfo();
                }
                else
                    main.temp.loading = false;
            })
            .catch((error) => {
                main.temp.disable = false;
                main.temp.loading = false;
                ElementPlus.ElMessage('清理缓存时发生异常.');
            });
    }
}