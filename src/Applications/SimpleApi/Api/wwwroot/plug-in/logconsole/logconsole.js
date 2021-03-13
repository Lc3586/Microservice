window.onload = () => {
    var main;

    //vue实例
    const Main = {
        data: getData,
        created() {
            main = this;
            init();
        },
        methods: {
            logTypeTabsChanged,
            mouseenterConsole,
            mouseleaveConsole,
            openLogFile,
            downloadLogFile,
            esAllLevel,
            esLevelChange,
            dbAllLevel,
            dbLevelChange
        },
        watch: {
            realLogs: {
                handler(val) {
                    if (this.scroll)
                        this.$nextTick(() => {
                            var container = this.$el.querySelector('#pane-Console');
                            container.scrollTop = container.scrollHeight
                        });
                },
                deep: true
            },
            'logFile.date'(newValue, oldValue) {
                if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                    return;
                getLogFileList();
            }
        }
    };

    const app = Vue.createApp(Main);
    app.use(ElementPlus);
    app.mount("#app");

    /**
     * 获取渲染数据
     */
    function getData() {
        return {
            loading: true,
            scroll: true,
            overall: {
                state: 'el-icon-loading',
                title: '一切正常',
                explain: '...',
                warn: 0,
                error: 0
            },
            logTypeTabsValue: 'Console',
            realLogs: [],
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
            logFile: {
                loading: false,
                list: [],
                search: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()]
            },
            log: {
                levels: [],
                types: []
            },
            logES: {
                list: [],
                levels: [],
                checkAllLevels: true,
                isLevelIndeterminate: true,
                types: [],
                checkAllTypes: true,
                isTypeIndeterminate: true,
                content: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()]
            },
            logDB: {
                list: [],
                levels: [],
                types: [],
                content: '',
                date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()]
            }
        };
    }

    /**
     * 初始化
     * 
     */
    function init() {
        getLogDefaultType(switchToPane);
    }

    /**
     * 获取默认的日志组件类型
     * 
     * @param {*} e 当前vue对象的实例
     * @param {Function} done 回调
     * */
    function getLogDefaultType(done) {
        axios.get('/log/default-type')
            .then(function (response) {
                main.loading = false;
                if (response.data.Success)
                    done && done(response.data.Data);
                else
                    ElementPlus.ElMessage(response.data.Msg);
            })
            .catch(function (error) {
                main.loading = false;
                ElementPlus.ElMessage('获取默认的日志组件类型时发生异常.');
            });
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
        main.logTypeTabsValue = logType;
        switch (logType) {
            case 'Console':
            default:
                const connection = generateSignalrConnection();
                connection.on("ReceiveLog", (time, level, type, message) => {
                    main.realLogs.push({
                        content: message,
                        timestamp: time,
                        size: 'large',
                        icon: getIconForlogLevel(level)
                    });

                    changeStateFromlogLevel(level);
                });
                connection.start();
                break;
            case 'File':
                getLogFileList();
                break;
            case 'ElasticSearch':

                break;
            case 'RDBMS':

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
                if (main.overall.state == 'el-icon-loading')
                    break;
                main.overall.state = 'el-icon-loading';
                main.overall.title = '一切正常';
                main.overall.explain = '...';
                break;
            case 'Warn':
                main.overall.warn++;
                if (main.overall.state != 'el-icon-warning') {
                    main.overall.state = 'el-icon-warning';
                    main.overall.title = '出现警告';
                }
                main.overall.explain = '共' + main.overall.warn + '个警告.';
                break;
            case 'Error':
            case 'Fatal':
                main.overall.error++;
                if (main.overall.state != 'el-icon-error') {
                    main.overall.state = 'el-icon-error';
                    main.overall.title = '发生异常';
                }
                main.overall.explain = '共' + main.overall.error + '个异常.';
                break;
        }
    }

    /**
     * 鼠标移入实时日志控制台
     * */
    function mouseenterConsole() {
        main.scroll = false;
    }

    /**
     * 鼠标移出实时日志控制台
     * */
    function mouseleaveConsole() {
        main.scroll = true;
    }

    /**
     * 创建SignalR连接对象
     * */
    function generateSignalrConnection() {
        return new signalR.HubConnectionBuilder()
            .withUrl("/loghub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    /**
     * 获取日志文件列表
     * */
    function getLogFileList() {
        main.logFile.loading = true;
        axios.get('/log/file-list/' + dayjs(main.logFile.date[0]).format('YYYY-MM-DD') + '/' + dayjs(main.logFile.date[1]).format('YYYY-MM-DD'))
            .then(function (response) {
                main.logFile.loading = false;
                if (response.data.Success)
                    main.logFile.list = response.data.Data;
                else
                    ElementPlus.ElMessage(response.data.Msg);
            })
            .catch(function (error) {
                main.logFile.loading = false;
                ElementPlus.ElMessage('获取日志文件列表时发生异常.');
            });
    }

    /**
     * 打开日志文件
     * @param {any} index
     * @param {any} data
     */
    function openLogFile(index, data) {
        window.open('/log/file-content/' + data.Name);
    }

    /**
     * 下载日志文件
     * @param {any} index
     * @param {any} data
     */
    function downloadLogFile(index, data) {
        window.open('/log/file-download/' + data.Name);
    }

    /**
     * 获取所有日志级别
     * */
    function getLogLevels() {
        axios.get('/log/log-levels/')
            .then(function (response) {
                if (response.data.Success)
                    main.log.levels = response.data.Data;
                else
                    ElementPlus.ElMessage(response.data.Msg);
            })
            .catch(function (error) {
                ElementPlus.ElMessage('获取日志文件列表时发生异常.');
            });
    }

    /**
     * 获取所有日志类型
     * */
    function getLogTypes() {
        axios.get('/log/log-types/')
            .then(function (response) {
                if (response.data.Success)
                    main.log.types = response.data.Data;
                else
                    ElementPlus.ElMessage(response.data.Msg);
            })
            .catch(function (error) {
                ElementPlus.ElMessage('获取日志文件列表时发生异常.');
            });
    }

    /**
     * ES所有日志级别
     * */
    function esAllLevel() {
        main.logES.level = val ? main.log.levels : [];
        main.logES.isIndeterminate = false;
    }

    /**
     * ES更改日志级别
     * */
    function esLevelChange() {

    }

    /**
     * DB所有日志级别
     * */
    function dbAllLevel() {

    }

    /**
     * DB更改日志级别
     * */
    function dbLevelChange() {

    }

    /**
     * 获取ES数据列表
     * */
    function getLogESList() {

    }

    /**
     * 获取ES数据详情
     * */
    function getLogESDetail() {

    }

    /**
     * 获取DB数据列表
     * */
    function getLogDBList() {

    }

    /**
     * 获取DB数据详情
     * */
    function getLogDBDetail() {

    }
}