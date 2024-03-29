﻿window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        saLogin: window.BaseUrl + '/sa/login',
        defaultLoggerType: window.BaseUrl + '/log/default-type',
        logLevels: window.BaseUrl + '/log/log-levels/',
        logTypes: window.BaseUrl + '/log/log-types/',
        logHub: window.BaseUrl + '/loghub',
        logFileList: window.BaseUrl + '/log/file-list/',
        logFileContent: window.BaseUrl + '/log/file-content/',
        logFileDownload: window.BaseUrl + '/log/file-download/',
        logESList: window.BaseUrl + '/log/es-list',
        logESDetail: window.BaseUrl + '/log/es-detail-data/',
        logDBList: window.BaseUrl + '/log/db-list',
        logDBDetail: window.BaseUrl + '/log/db-detail-data/'
    };

    //vue实例
    const Main = {
        data: getData,
        created() {
            main = this;
            addLoginFilter();
            getLogConfig();

            this.logES.pageSize = getPageSize();
            this.logDB.pageSize = getPageSize();

            //延迟处理用户的输入内容
            this.logESContentGetAnswer = _.debounce(getLogESList, 500);
            this.logDBContentGetAnswer = _.debounce(getLogDBList, 500);
        },
        methods: {
            saLogin,
            logTypeTabsChanged,
            mouseenterConsole,
            mouseleaveConsole,
            openLogFile,
            closeLogFile,
            downloadFileContent,
            downloadLogFile,
            realReceiveChange,
            realAllLevel,
            realLevelChange,
            realAllType,
            realTypeChange,
            handleFilterClose,
            handleFilterInputConfirm,
            showFilterInput,
            handleKeywordClose,
            handleKeywordInputConfirm,
            showKeywordInput,
            esAllLevel,
            esLevelChange,
            esAllType,
            esTypeChange,
            logESSort,
            logESListSizeChange,
            logESListCurrentChange,
            logESListRowClassName,
            LogESDetail: getLogESDetail,
            dbAllLevel,
            dbLevelChange,
            dbAllType,
            dbTypeChange,
            logDBSort,
            logDBListSizeChange,
            logDBListCurrentChange,
            logDBListRowClassName,
            LogDBDetail: getLogDBDetail,
            closeLog
        },
        watch: {
            'logReal.list': {
                handler(val) {
                    if (this.logReal.scroll)
                        this.$nextTick(() => {
                            var container = this.$refs['realLog'];
                            container.scrollTop = container.scrollHeight;
                        });
                },
                deep: true
            },
            'logFile.date'(newValue, oldValue) {
                if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                    return;
                getLogFileList();
            },
            'logES.date'(newValue, oldValue) {
                if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                    return;
                getLogESList();
            },
            'logDB.date'(newValue, oldValue) {
                if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                    return;
                getLogDBList();
            },
            'logES.content'(newValue, oldValue) {
                this.logESContentGetAnswer();
            },
            'logDB.content'(newValue, oldValue) {
                this.logDBContentGetAnswer();
            }
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
            dataRangShortcuts: [
                {
                    text: '最近一周',
                    value: (() => {
                        const end = new Date()
                        const start = new Date()
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
                        return [start, end]
                    })(),
                },
                {
                    text: '最近一个月',
                    value: (() => {
                        const end = new Date()
                        const start = new Date()
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30)
                        return [start, end]
                    })(),
                },
                {
                    text: '最近三个月',
                    value: (() => {
                        const end = new Date()
                        const start = new Date()
                        start.setTime(start.getTime() - 3600 * 1000 * 24 * 90)
                        return [start, end]
                    })(),
                }
            ],
            logConfig: {
                LoggerType: '',
                levels: [],
                types: []
            },
            logReal: {
                init: false,
                loading: true,
                scroll: true,
                scrollLocked: false,
                receive: true,
                max: 1000,
                connection: null,
                list: [],
                levels: [],
                levels_history: [],
                checkAllLevels: true,
                isLevelIndeterminate: false,
                types: [],
                types_history: [],
                checkAllTypes: true,
                isTypeIndeterminate: false,
                filters: [],
                filters_history: [],
                filterInputVisible: false,
                filterInputValue: '',
                keywords: [],
                keywords_history: [],
                keywordInputVisible: false,
                keywordInputValue: '',
                search: ''
            },
            logFile: {
                init: false,
                loading: true,
                error: '',
                list: [],
                search: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()],
                fileContent: {
                    show: false,
                    loading: false,
                    currentFile: '',
                    percentage: 0,
                    bytes: 0,
                    size: '',
                    content: ''
                }
            },
            log: {
                show: false,
                loading: true,
                tag: ''
            },
            pageConfig: {
                sizes: [5, 10, 15, 20, 50, 100, 150, 200, 300, 400, 500]
            },
            logES: {
                init: false,
                loading: true,
                error: '',
                list: [],
                sorts: [],
                currentPage: 1,
                pageSize: 15,
                pageTotal: 0,
                hidePagination: true,
                total: 0,
                levels: [],
                checkAllLevels: true,
                isLevelIndeterminate: false,
                types: [],
                checkAllTypes: true,
                isTypeIndeterminate: false,
                content: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()]
            },
            logDB: {
                init: false,
                loading: true,
                error: '',
                list: [],
                sorts: [],
                currentPage: 1,
                pageSize: 15,
                pageTotal: 0,
                hidePagination: true,
                total: 0,
                levels: [],
                checkAllLevels: true,
                isLevelIndeterminate: false,
                types: [],
                checkAllTypes: true,
                isTypeIndeterminate: false,
                content: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()]
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
     * 获取日志配置
     * */
    function getLogConfig() {
        axios.all([getDefaultLoggerType(), getLogLevels(), getLogTypes()])
            .then(axios.spread((type, levels, types) => {
                main.loading = false;

                if (type.data.Success)
                    switchToPane(type.data.Data);
                else
                    ElementPlus.ElMessage(type.data.Message);

                if (levels.data.Success) {
                    main.logConfig.levels = levels.data.Data;

                    main.logReal.levels = levels.data.Data;
                    main.logReal.checkAllLevels = true;
                    main.logReal.isLevelIndeterminate = false;

                    main.logES.levels = levels.data.Data;
                    main.logES.checkAllLevels = true;
                    main.logES.isLevelIndeterminate = false;

                    main.logDB.levels = levels.data.Data;
                    main.logDB.checkAllLevels = true;
                    main.logDB.isLevelIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(levels.data.Message);

                if (types.data.Success) {
                    main.logConfig.types = types.data.Data;

                    main.logReal.types = types.data.Data;
                    main.logReal.checkAllTypes = true;
                    main.logReal.isTypeIndeterminate = false;

                    main.logES.types = types.data.Data;
                    main.logES.checkAllTypes = true;
                    main.logES.isTypeIndeterminate = false;

                    main.logDB.types = types.data.Data;
                    main.logDB.checkAllTypes = true;
                    main.logDB.isTypeIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(types.data.Message);

            }))
            .catch((error) => {
                main.loading = false;
                if (!main.sa.show)
                    ElementPlus.ElMessage('获取日志配置时发生异常.');
            });
    }

    /**
     * 获取默认的日志组件类型
     * 
     * */
    function getDefaultLoggerType() {
        return axios.get(apiUrls.defaultLoggerType);
    }

    /**
     * 获取所有日志级别
     * */
    function getLogLevels() {
        return axios.get(apiUrls.logLevels);
    }

    /**
     * 获取所有日志类型
     * */
    function getLogTypes() {
        return axios.get(apiUrls.logTypes);
    }

    /**
     * 切换标签时的点击事件
     * @param {any} tab
     */
    function logTypeTabsChanged(tab) {
        switchToPane(tab.props.name);
    }

    /**
     * 切换至指定的标签
     *
     * @param {string} logType 日志组件类型
     * */
    function switchToPane(logType) {
        main.logConfig.LoggerType = logType;
        switch (logType) {
            case 'Console':
            default:
                if (main.logReal.init) {
                    main.$nextTick(() => {
                        var container = main.$refs['realLog'].parentElement;
                        container.scrollTop = container.scrollHeight;
                    });
                    return;
                }
                connectToLogHub();
                break;
            case 'File':
                if (main.logFile.init)
                    return;
                getLogFileList();
                break;
            case 'ElasticSearch':
                if (main.logES.init)
                    return;
                getLogESList();
                break;
            case 'RDBMS':
                if (main.logDB.init)
                    return;
                getLogDBList();
                break;
        }
    }

    /**
     * 获取不同的日志级别对应的图标
     * @param {any} level
     */
    function getIconForlogLevel(level) {
        switch (level) {
            case 'Trace':
            case 'Debug':
            case 'Info':
            default:
                return 'el-icon-info';
            case 'Warn':
                return 'el-icon-warning';
            case 'Error':
            case 'Fatal':
                return 'el-icon-error';
        }
    }

    /**
     * 根据日志级别更改当前状态信息
     * @param {any} level
     */
    function changeStateFromlogLevel(level) {
        switch (level) {
            case 'Trace':
            case 'Debug':
            case 'Info':
            default:
                if (main.overall.state != 'el-icon-loading')
                    break;
                main.overall.state = 'el-icon-loading';
                main.overall.title = '一切正常';
                main.overall.explain = '...';
                return;
            case 'Warn':
                main.overall.warn++;
                if (main.overall.state != 'el-icon-warning' && main.overall.state != 'el-icon-error') {
                    main.overall.state = 'el-icon-warning';
                    main.overall.title = '出现警告';
                }
                break;
            case 'Error':
            case 'Fatal':
                main.overall.error++;
                if (main.overall.state != 'el-icon-error') {
                    main.overall.state = 'el-icon-error';
                    main.overall.title = '发生异常';
                }
                break;
        }

        main.overall.explain = (main.overall.warn > 0 ? (main.overall.warn + '个警告.') : '') + (main.overall.error > 0 ? (main.overall.error + '个异常.') : '');
    }

    /**
     * 鼠标移入实时日志控制台
     * */
    function mouseenterConsole() {
        if (!main.logReal.scrollLocked)
            main.logReal.scroll = false;
    }

    /**
     * 鼠标移出实时日志控制台
     * */
    function mouseleaveConsole() {
        if (!main.logReal.scrollLocked)
            main.logReal.scroll = true;
    }

    /**
     * 连接至日志中心
     * */
    function connectToLogHub() {
        main.logReal.loading = true;
        main.logReal.connection = new signalR.HubConnectionBuilder()
            .withUrl(apiUrls.logHub)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        main.logReal.connection.onreconnecting(error => {
            main.overall.title = '连接已断开';
            main.overall.explain = '正在尝试重新连接...';
        });
        main.logReal.connection.onreconnected(connectionId => {
            console.info(connectionId);
            main.overall.title = '连接已恢复';
            main.overall.explain = '已重新连接至服务器';

            initHubMethod();
        });
        main.logReal.connection.onclose(() => {
            main.overall.title = '连接已关闭';
            main.overall.explain = '...';
        });

        main.logReal.connection.on("ReceiveLog", data => {
            if (!main.logReal.receive)
                return;

            if (main.logReal.list.length == main.logReal.max)
                main.logReal.list.shift();
            else if (main.logReal.list.length > main.logReal.max)
                main.logReal.list.splice(0, main.logReal.list.length - main.logReal.max - 1);

            main.logReal.list.push({
                content: data.Data,
                timestamp: data.CreateTime,
                size: 'large',
                icon: getIconForlogLevel(data.Level)
            });

            changeStateFromlogLevel(data.Level);
        });
        main.logReal.connection
            .start()
            .then(() => {
                main.overall.title = '已连接';
                main.overall.explain = '...';

                main.logReal.loading = false;
                main.logReal.init = true;

                initHubMethod();
            });
    }

    /**
     * 初始化Hub方法
     * */
    function initHubMethod() {
        startRealLog(() => {
            setRealLogSetting('AddLevels', main.logReal.levels, () => {
                main.logReal.levels_history = main.logReal.levels;
            }, () => {
                main.logReal.levels = [];
            });
            setRealLogSetting('AddTypes', main.logReal.types, () => {
                main.logReal.types_history = main.logReal.types;
            }, () => {
                main.logReal.types = [];
            });
        });
    }

    /**
     * Real更改接收状态
     * @param {any} val
     */
    function realReceiveChange(val) {
        val ? startRealLog() : pauseRealLog();
    }

    /**
     * 启动实时日志
     * @param {any} done 回调
     */
    function startRealLog(done) {
        return main.logReal.connection
            .invoke('Start')
            .then(() => {
                main.logReal.receive = true;
                done && done();
            })
            .catch(error => {
                ElementPlus.ElMessage('实时日志启动失败');
            });
    }

    /**
     * 暂停实时日志
     * @param {any} done 回调
     */
    function pauseRealLog(done) {
        return main.logReal.connection
            .invoke('Pause')
            .then(() => {
                main.logReal.receive = false;
                done && done();
            })
            .catch(error => {
                ElementPlus.ElMessage('实时日志暂停失败');
            });
    }

    /**
     * 设置实时日志的配置
     * */
    function setRealLogSetting(type, value, done, error) {
        main.logReal.connection
            .invoke(type, value)
            .then(done)
            .catch(data => {
                error && error(data);
                ElementPlus.ElMessage('实时日志设置失败');
            });
    }

    /**
     * Real所有日志级别
     * */
    function realAllLevel(val) {
        main.logReal.levels = val ? main.logConfig.levels : [];
        main.logReal.isLevelIndeterminate = false;

        if (main.logReal.levels.length == 0)
            setRealLogSetting('RemoveLevels', main.logConfig.levels, () => {
                main.logReal.levels_history = main.logConfig.levels;
            }, () => {
                main.logReal.levels = main.logConfig.levels;
            });
        else
            setRealLogSetting('AddLevels', main.logReal.levels, () => {
                main.logReal.levels_history = [];
            }, () => {
                main.logReal.levels = [];
            });
    }

    /**
     * Real更改日志级别
     * */
    function realLevelChange(val) {
        var levels_new = main.logReal.levels.filter((item, index) => {
            return main.logReal.levels_history.indexOf(item) < 0;
        }),
            levels_remove = main.logReal.levels_history.filter((item, index) => {
                return main.logReal.levels.indexOf(item) < 0;
            });

        var remove = () => {
            setRealLogSetting('RemoveLevels', levels_remove, () => {
                main.logReal.levels_history = val;
            }, () => {
                main.logReal.levels = main.logReal.levels.filter((item, index) => {
                    return levels_remove.indexOf(item) >= 0;
                });
            });
        };
        var add = () => {
            setRealLogSetting('AddLevels', levels_new, () => {
                if (levels_remove.length > 0)
                    remove();
            }, () => {
                main.logReal.levels = main.logReal.levels.filter((item, index) => {
                    return levels_new.indexOf(item) < 0;
                });
            });
        };

        if (levels_new.length > 0)
            add();
        else if (levels_remove.length > 0)
            remove();
    }

    /**
     * Real所有日志类型
     * */
    function realAllType(val) {
        main.logReal.types = val ? main.logConfig.types : [];
        main.logReal.isTypeIndeterminate = false;

        if (main.logReal.types.length == 0)
            setRealLogSetting('RemoveTypes', main.logConfig.types, () => {
                main.logReal.types_history = main.logConfig.types;
            }, () => {
                main.logReal.types = main.logConfig.types;
            });
        else
            setRealLogSetting('AddTypes', main.logReal.types, () => {
                main.logReal.types_history = [];
            }, () => {
                main.logReal.types = [];
            });
    }

    /**
     * Real更改日志类型
     * */
    function realTypeChange(val) {
        var types_new = main.logReal.types.filter((item, index) => {
            return main.logReal.types_history.indexOf(item) < 0;
        }),
            types_remove = main.logReal.types_history.filter((item, index) => {
                return main.logReal.types.indexOf(item) < 0;
            });

        var remove = () => {
            setRealLogSetting('RemoveTypes', types_remove, () => {
                main.logReal.types_history = val;
            }, () => {
                main.logReal.types = main.logReal.types.filter((item, index) => {
                    return types_remove.indexOf(item) >= 0;
                });
            });
        };

        var add = () => {
            setRealLogSetting('AddTypes', types_new, () => {
                if (types_remove.length > 0)
                    remove();
            }, () => {
                main.logReal.types = main.logReal.types.filter((item, index) => {
                    return types_new.indexOf(item) < 0;
                });
            });
        };

        if (types_new.length > 0)
            add();
        else if (types_remove.length > 0)
            remove();
    }

    /**
     * Real移除过滤条件
     * @param {any} val
     */
    function handleFilterClose(val) {
        main.logReal.filters.splice(main.logReal.filters.indexOf(val), 1);

        setRealLogSetting('RemoveFilters', [val], () => {
            main.logReal.filters_history.splice(main.logReal.filters_history.indexOf(val), 1);
        }, () => {
            main.logReal.filters.push(val);
        });
    }

    /**
     * Real新增过滤条件
     * @param {any} val
     */
    function handleFilterInputConfirm() {
        if (main.logReal.filterInputValue) {
            var val = main.logReal.filterInputValue;

            main.logReal.filters.push(val);

            setRealLogSetting('AddFilters', [val], () => {
                main.logReal.filters_history.push(val);
            }, () => {
                main.logReal.filters.splice(main.logReal.filters.indexOf(val), 1);
            });
        }

        main.logReal.filterInputVisible = false;
        main.logReal.filterInputValue = '';
    }

    /**
     * Real显示过滤条件输入框
     * */
    function showFilterInput() {
        main.logReal.filterInputVisible = true;
        this.$nextTick(_ => {
            this.$refs.saveFilterInput.$refs.input.focus();
        });
    }

    /**
     * Real移除筛选条件
     * @param {any} val
     */
    function handleKeywordClose(val) {
        main.logReal.keywords.splice(main.logReal.keywords.indexOf(val), 1);

        setRealLogSetting('RemoveKeywords', [val], () => {
            main.logReal.keywords_history.splice(main.logReal.keywords_history.indexOf(val), 1);
        }, () => {
            main.logReal.keywords.push(val);
        });
    }

    /**
     * Real新增筛选条件
     * @param {any} val
     */
    function handleKeywordInputConfirm() {
        if (main.logReal.keywordInputValue) {
            var val = main.logReal.keywordInputValue;

            main.logReal.keywords.push(val);

            setRealLogSetting('AddKeywords', [val], () => {
                main.logReal.keywords_history.push(val);
            }, () => {
                main.logReal.keywords.splice(main.logReal.keywords.indexOf(val), 1);
            });
        }

        main.logReal.keywordInputVisible = false;
        main.logReal.keywordInputValue = '';
    }

    /**
     * Real显示筛选条件输入框
     * */
    function showKeywordInput() {
        main.logReal.keywordInputVisible = true;
        this.$nextTick(_ => {
            this.$refs.saveKeywordInput.$refs.input.focus();
        });
    }

    /**
     * 获取日志文件列表
     * */
    function getLogFileList() {
        main.logFile.loading = true;
        axios.get(apiUrls.logFileList + dayjs(main.logFile.date[0]).format('YYYY-MM-DD') + '/' + dayjs(main.logFile.date[1]).format('YYYY-MM-DD'))
            .then((response) => {
                if (response.data.Success)
                    main.logFile.list = response.data.Data;
                else {
                    main.logFile.error = response.data.Message;
                    ElementPlus.ElMessage(response.data.Message);
                }
                main.logFile.loading = false;
            })
            .catch((error) => {
                main.logFile.loading = false;
                main.logFile.error = error.message;
                ElementPlus.ElMessage('获取日志文件列表时发生异常.');
            });
        main.logFile.init = true;
    }

    /**
     * 打开日志文件
     * @param {any} index
     * @param {any} data
     */
    function openLogFile(index, data) {
        main.logFile.fileContent.loading = true;
        main.logFile.fileContent.currentFile = data.Name + data.Suffix;
        main.logFile.fileContent.bytes = data.Bytes;
        main.logFile.fileContent.size = data.Size;
        main.logFile.fileContent.show = true;
        axios.get(apiUrls.logFileContent + data.Name, {
            onDownloadProgress: (progressEvent) => {
                console.info(progressEvent);
                main.logFile.fileContent.percentage = parseInt((progressEvent.loaded / main.logFile.fileContent.bytes * 100).toFixed(0));
            }
        }).then((response) => {
            main.logFile.fileContent.content = response.data;
            main.logFile.fileContent.loading = false;
        }).catch((error) => {
            main.logFile.fileContent.loading = false;
            ElementPlus.ElMessage('打开日志文件时发生异常.');
        });
    }

    /**
     * 关闭日志文件
     * */
    function closeLogFile() {
        main.logFile.fileContent.show = false;
        main.logFile.fileContent.currentFile = '';
        main.logFile.fileContent.content = '';
    }

    /**
     * 下载日志文件内容
     * */
    function downloadFileContent() {
        const logFileBlob = new Blob([main.logFile.fileContent.content], { type: 'text/plain;charset=utf-8' });
        //window.open(URL.createObjectURL(logFileBlob));
        const link = document.createElement('a');
        link.download = main.logFile.fileContent.currentFile;
        link.style.display = 'none';
        link.href = URL.createObjectURL(logFileBlob);
        document.body.appendChild(link);
        link.click();
        URL.revokeObjectURL(link.href);
    }

    /**
     * 下载日志文件
     * @param {any} index
     * @param {any} data
     */
    function downloadLogFile(index, data) {
        window.open(apiUrls.logFileDownload + data.Name);
    }

    /**
     * ES所有日志级别
     * */
    function esAllLevel(val) {
        main.logES.levels = val ? main.logConfig.levels : [];
        main.logES.isLevelIndeterminate = false;
        getLogESList();
    }

    /**
     * ES更改日志级别
     * */
    function esLevelChange(val) {
        getLogESList();
    }

    /**
     * ES所有日志类型
     * */
    function esAllType(val) {
        main.logES.types = val ? main.logConfig.types : [];
        main.logES.isTypeIndeterminate = false;
        getLogESList();
    }

    /**
     * ES更改日志类型
     * */
    function esTypeChange(val) {
        getLogESList();
    }

    /**
     * DB所有日志级别
     * */
    function dbAllLevel(val) {
        main.logDB.levels = val ? main.logConfig.levels : [];
        main.logDB.isLevelIndeterminate = false;
        getLogDBList();
    }

    /**
     * DB更改日志级别
     * */
    function dbLevelChange(val) {
        getLogDBList();
    }

    /**
     * DB所有日志类型
     * */
    function dbAllType(val) {
        main.logDB.types = val ? main.logConfig.types : [];
        main.logDB.isTypeIndeterminate = false;
        getLogDBList();
    }

    /**
     * DB更改日志类型
     * */
    function dbTypeChange(val) {
        getLogDBList();
    }

    /**
     * 根据页面大小调整页面数据量
     * */
    function getPageSize() {
        var current = window.innerHeight / 100,
            min,
            result;
        for (var i in main.pageConfig.sizes) {
            var size = main.pageConfig.sizes[i];
            var abs = Math.abs(current - size);
            if (!min)
                min = abs;
            else if (abs <= min)
                min = abs;
            else
                continue;

            result = size;
        }

        return result;
    }

    /**
     * 获取列表接口参数
     * @param {any} target
     */
    function getLogListParams(target) {
        var params = {
            PageIndex: target.currentPage,
            PageRows: target.pageSize,
            AdvancedSort: [
                {
                    Field: "CreateTime",
                    Type: "desc"
                }
            ],
            DynamicFilterInfo: []
        };

        if (target.sorts.length > 0)
            params.AdvancedSort = target.sorts;

        if (target.levels.length != 0 && target.levels.length != main.logConfig.levels.length)
            params.DynamicFilterInfo.push({
                Field: 'Level',
                Value: target.levels,
                Compare: 'inSet'
            });

        if (target.types.length != 0 && target.types.length != main.logConfig.types.length)
            params.DynamicFilterInfo.push({
                Field: 'LogType',
                Value: target.types,
                Compare: 'inSet'
            });

        if (target.content && main.logDB.content.length > 0)
            params.DynamicFilterInfo.push({
                Field: 'LogContent',
                Value: target.content,
                Compare: 'in'
            });

        if (target.date && target.date.length == 2)
            params.DynamicFilterInfo.push({
                Relation: 'and',
                DynamicFilterInfo: [
                    {
                        Field: 'CreateTime',
                        Value: dayjs(target.date[0]).format('YYYY-MM-DD HH:mm:ss'),
                        Compare: 'ge'
                    },
                    {
                        Field: 'CreateTime',
                        Value: dayjs(target.date[1]).format('YYYY-MM-DD HH:mm:ss'),
                        Compare: 'le'
                    }
                ]
            });

        return params;
    }

    /**
     * 获取ES数据列表
     * */
    function getLogESList() {
        main.logES.loading = true;
        axios.post(apiUrls.logESList, {

        }).then(function (response) {
            if (response.data.Success) {
                main.logES.currentPage = response.data.Data.PageIndex;
                main.logES.pageSize = response.data.Data.PageSize;
                main.logES.pageTotal = response.data.Data.PageTotal;
                main.logES.total = response.data.Data.Total;
                main.logES.list = response.data.Data.List;
            }
            else {
                main.logES.error = response.data.Message;
                ElementPlus.ElMessage(response.data.Message);
            }
            main.logES.loading = false;
        }).catch(function (error) {
            main.logES.loading = false;
            main.logES.error = error.message;
            ElementPlus.ElMessage('获取ES数据列表时发生异常.');
        });
        main.logES.init = true;
    }

    /**
     * ES数据列表排序
     * @param {any} val 值
     */
    function logESSort(val) {
        if (val.prop == null)
            main.logES.sorts = [];
        else if (!val.order)
            main.logES.sorts = main.logES.sorts.filter(data => data.field != val.prop);
        else {
            for (var item in main.logES.sorts) {
                if (main.logES.sorts[item].field == val.prop) {
                    main.logES.sorts[item].type = val.order == 'descending' ? 'desc' : 'asc';
                    getLogESList();
                    return;
                }
            }
            main.logES.sorts.push({ field: val.prop, type: val.order == 'descending' ? 'desc' : 'asc' });
        }
        getLogESList();
    }

    /**
     * 更改ES数据列表每页数据量
     * @param {number} val
     */
    function logESListSizeChange(val) {
        main.logES.pageSize = val;
        getLogESList();
    }

    /**
     * 跳转至ES数据列表指定页
     * @param {number} val
     */
    function logESListCurrentChange(val) {
        main.logES.currentPage = val;
        getLogESList();
    }

    /**
     * 获取ES数据指定的类名
     * @param {any} param0
     */
    function logESListRowClassName({ row, rowIndex }) {
        switch (row.Level) {
            case 'Trace':
            case 'Debug':
            case 'Info':
            default:
                return '';
            case 'Warn':
                return 'warning-row';
            case 'Error':
            case 'Fatal':
                return 'error-row';
        }
    }

    /**
     * 获取ES数据详情
     * @param {any} index
     * @param {any} row
     */
    function getLogESDetail(index, row) {
        main.log.show = true;
        main.log.loading = true;

        axios.get(apiUrls.logESDetail + row.Id)
            .then((response) => {
                if (response.data.Success) {
                    main.log.tag = getLogTagType(response.data.Data.Level);
                    for (var item in response.data.Data) {
                        main.log[item] = response.data.Data[item];
                    }
                }
                else {
                    ElementPlus.ElMessage(response.data.Message);
                }
                main.log.loading = false;
            })
            .catch((error) => {
                main.log.loading = false;
                ElementPlus.ElMessage('获取日志数据详情时发生异常.');
            });
    }

    /**
     * 获取DB数据列表
     * */
    function getLogDBList() {
        main.logDB.loading = true;
        axios.post(apiUrls.logDBList, getLogListParams(main.logDB)).then(function (response) {
            if (response.data.Success) {
                main.logDB.currentPage = response.data.Data.PageIndex;
                main.logDB.pageSize = response.data.Data.PageSize;
                main.logDB.pageTotal = response.data.Data.PageTotal;
                main.logDB.total = response.data.Data.Total;
                main.logDB.list = response.data.Data.List;
            }
            else {
                main.logDB.error = response.data.Message;
                ElementPlus.ElMessage(response.data.Message);
            }
            main.logDB.loading = false;
        }).catch(function (error) {
            main.logDB.loading = false;
            main.logDB.error = error.message;
            ElementPlus.ElMessage('获取DB数据列表时发生异常.');
        });
        main.logDB.init = true;
    }

    /**
     * DB数据列表排序
     * @param {any} val 值
     */
    function logDBSort(val) {
        if (val.prop == null)
            main.logDB.sorts = [];
        else if (!val.order)
            main.logDB.sorts = main.logDB.sorts.filter(data => data.field != val.prop);
        else {
            for (var item in main.logDB.sorts) {
                if (main.logDB.sorts[item].field == val.prop) {
                    main.logDB.sorts[item].type = val.order == 'descending' ? 'desc' : 'asc';
                    getLogDBList();
                    return;
                }
            }
            main.logDB.sorts.push({ field: val.prop, type: val.order == 'descending' ? 'desc' : 'asc' });
        }
        getLogDBList();
    }

    /**
     * 更改DB数据列表每页数据量
     * @param {number} val
     */
    function logDBListSizeChange(val) {
        main.logDB.pageSize = val;
        getLogDBList();
    }

    /**
     * 跳转至DB数据列表指定页
     * @param {number} val
     */
    function logDBListCurrentChange(val) {
        main.logDB.currentPage = val;
        getLogDBList();
    }

    /**
     * 获取DB数据指定的类名
     * @param {any} param0
     */
    function logDBListRowClassName({ row, rowIndex }) {
        switch (row.Level) {
            case 'Trace':
            case 'Debug':
            case 'Info':
            default:
                return '';
            case 'Warn':
                return 'warning-row';
            case 'Error':
            case 'Fatal':
                return 'error-row';
        }
    }

    /**
     * 获取DB数据详情
     * @param {any} index
     * @param {any} row
     */
    function getLogDBDetail(index, row) {
        main.log.show = true;
        main.log.loading = true;

        axios.get(apiUrls.logDBDetail + row.Id)
            .then((response) => {
                if (response.data.Success) {
                    main.log.tag = getLogTagType(response.data.Data.Level);
                    for (var item in response.data.Data) {
                        main.log[item] = response.data.Data[item];
                    }
                }
                else {
                    ElementPlus.ElMessage(response.data.Message);
                }
                main.log.loading = false;
            })
            .catch((error) => {
                main.log.loading = false;
                ElementPlus.ElMessage('获取日志数据详情时发生异常.');
            });
    }

    /**
     * 获取数据指定的标签类型
     * @param {any} level
     */
    function getLogTagType(level) {
        switch (level) {
            case 'Trace':
            case 'Debug':
            case 'Info':
            default:
                return 'info';
            case 'Warn':
                return 'warning';
            case 'Error':
            case 'Fatal':
                return 'danger';
        }
    }

    /**
     * 关闭日志详情
     * */
    function closeLog() {
        main.log.show = false;
        for (var item in main.log) {
            if (item != 'show' && item != 'loding')
                main.log[item] = '';
        }
    }
}