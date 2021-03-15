window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        saLogin: '/sa/login',
        defaultLoggerType: '/log/default-type',
        logLevels: '/log/log-levels/',
        logTypes: '/log/log-types/',
        logHub: '/loghub',
        logFileList: '/log/file-list/',
        logFileContent: '/log/file-content/',
        logFileDownload: '/log/file-download/',
        logESList: '/log/es-list',
        logESDetail: '/log/es-detail-data/',
        logDBList: '/log/db-list',
        logDBDetail: '/log/db-detail-data/'
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
                title: '一切正常',
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
                list: [],
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
                ElementPlus.ElMessage(response.data.Msg);
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
                    ElementPlus.ElMessage(type.data.Msg);

                if (levels.data.Success) {
                    main.logConfig.levels = levels.data.Data;
                    main.logES.levels = levels.data.Data;
                    main.logES.checkAllLevels = true;
                    main.logES.isLevelIndeterminate = false;
                    main.logDB.levels = levels.data.Data;
                    main.logDB.checkAllLevels = true;
                    main.logDB.isLevelIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(levels.data.Msg);

                if (types.data.Success) {
                    main.logConfig.types = types.data.Data;
                    main.logES.types = types.data.Data;
                    main.logES.checkAllTypes = true;
                    main.logES.isTypeIndeterminate = false;
                    main.logDB.types = types.data.Data;
                    main.logDB.checkAllTypes = true;
                    main.logDB.isTypeIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(types.data.Msg);

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
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiUrls.logHub)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.on("ReceiveLog", (time, level, type, message) => {
            if (!main.logReal.receive)
                return;

            if (main.logReal.list.length == main.logReal.max)
                main.logReal.list.shift();
            else if (main.logReal.list.length > main.logReal.max)
                main.logReal.list.splice(0, main.logReal.list.length - main.logReal.max - 1);

            main.logReal.list.push({
                content: message,
                timestamp: time,
                size: 'large',
                icon: getIconForlogLevel(level)
            });

            changeStateFromlogLevel(level);
        });
        connection.start();
        main.logReal.loading = false;
        main.logReal.init = true;
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
                    main.logFile.error = response.data.Msg;
                    ElementPlus.ElMessage(response.data.Msg);
                }
                main.logFile.loading = false;
            })
            .catch((error) => {
                main.logFile.loading = false;
                main.logFile.error = error;
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
        main.logFile.fileContent.size = data.Size;
        main.logFile.fileContent.show = true;
        axios.get(apiUrls.logFileContent + data.Name).then((response) => {
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
                main.logES.error = response.data.Msg;
                ElementPlus.ElMessage(response.data.Msg);
            }
            main.logES.loading = false;
        }).catch(function (error) {
            main.logES.loading = false;
            main.logES.error = error;
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
                    ElementPlus.ElMessage(response.data.Msg);
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
                main.logDB.error = response.data.Msg;
                ElementPlus.ElMessage(response.data.Msg);
            }
            main.logDB.loading = false;
        }).catch(function (error) {
            main.logDB.loading = false;
            main.logDB.error = error;
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
                    ElementPlus.ElMessage(response.data.Msg);
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