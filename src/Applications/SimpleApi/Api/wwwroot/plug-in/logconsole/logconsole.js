window.onload = () => {
    //vue实例
    const Main = {
        data: getData,
        created() {
            init();
        },
        methods: {
            logTypeTabsChanged
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
            overall_state: 'overall_state_success',
            overall_state_title: '一切正常',
            overall_explain: '一切正常...',
            logTypeTabsValue: 'Console',
            logs: [
                //{
                //    content: '支持使用图标',
                //    timestamp: '2018-04-12 20:46',
                //    size: 'large',
                //    icon: 'el-icon-success'
                //},
                //{
                //    content: '支持自定义颜色',
                //    timestamp: '2018-04-03 20:46',
                //    icon: 'el-icon-warning'
                //},
                //{
                //    content: '支持自定义尺寸',
                //    timestamp: '2018-04-03 20:46',
                //    size: 'large',
                //    icon: 'el-icon-error'
                //},
                //{
                //    content: '默认样式的节点',
                //    timestamp: '2018-04-03 20:46'
                //}
            ]
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
    function getLogDefaultType(e, done) {
        axios.get('/log/default-type')
            .then(function (response) {
                console.log(response);
                if (response.Success)
                    done && done(response.Data);
                else
                    ElementPlus.ElMessage(response.Msg);
            })
            .catch(function (error) {
                console.log(error);
                e.loading = false;
                ElementPlus.ElMessage('获取默认的日志组件类型时发生异常.');
            });
    }

    /**
     * 切换至指定的标签
     *
     * @param {string} logType 日志组件类型
     * */
    function switchToPane(logType) {
        this.logTypeTabsValue = logType;
        if (logType == "Console") {
            const connection = generateSignalrConnection();
            connection.on("ReceiveLog", (time, level, type, message) => {
                console.info(message);
                this.logs.push({
                    content: message,
                    timestamp: time,
                    size: 'large',
                    icon: getIconForlogLevel(level)
                });
            });
            connection.start();
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
     * 切换标签时的点击事件
     * @param {any} tab
     */
    function logTypeTabsChanged(tab) {
        switchToPane(tab.props.name);
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
}