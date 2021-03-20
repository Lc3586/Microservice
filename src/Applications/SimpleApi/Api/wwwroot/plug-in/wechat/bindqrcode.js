window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        qrCodeUrl: '/wechat-oath/user-bind-url',
        wechatHub: '/wechathub',
    };

    //vue实例
    const Main = {
        data: getData,
        created() {
            main = this;
            getQRCodeUrl();
        },
        methods: {

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
            explain: '使用微信扫一扫绑定账号',
            qr: {
                show: true,
                url: '',
                loading: true
            },
            signalr: {
                s: ''
            },
            info: {
                show: false,
                nickname: '',
                sexicon: '',
                headimgUrl: ''
            }
        };
    }

    /**
     * 获取Url参数
     * */
    function getParam(name) {
        var query = window.location.href; var iLen = name.length; var iStart = query.indexOf(name); if (iStart == -1) return ""; iStart += iLen + 1; var iEnd = query.indexOf("&", iStart); if (iEnd == -1) return query.substring(iStart); return query.substring(iStart, iEnd);
    }

    /**
     * 获取二维码地址
     * */
    function getQRCodeUrl() {
        main.qr.loading = true;
        axios.get(apiUrls.qrCodeUrl + '/' + getParam('userId') + '?returnUrl=' + encodeURIComponent(window.location.origin + '/plug-in/wechat/confirm.html') + '&asyncUserInfo=' + getParam('asyncUserInfo')).then(response => {
            if (response.data.Success) {
                var size = parseInt(getParam('size')) || 500;
                new AwesomeQR.AwesomeQR({
                    text: response.data.Data.Url,
                    size: size,
                    colorLight: '#ffffff'
                }).draw().then((dataURL) => {
                    main.qr.url = dataURL;
                    main.qr.loading = false;
                });

                main.signalr.s = response.data.Data.S;
                connectToLogHub();
            }
            else
                ElementPlus.ElMessage(response.data.Msg);
        }).catch(error => {
            main.qr.loading = false;
            ElementPlus.ElMessage('获取二维码地址时发生异常.');
        });
    }

    /**
     * 连接至微信中心
     * */
    function connectToLogHub() {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiUrls.wechatHub)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.on("Scanned", data => {
            main.explain = '请在您的手机上进行确认';
            main.qr.show = false;
            main.info.show = true;
            main.info.nickname = data.nickname;
            main.info.sex = data.sex == 1 ? 'male' : (data.sex == 2 ? 'female' : '');
            main.info.headimgUrl = data.headimgUrl;
        });

        connection.on("Confirmed", data => {
            window.location.href = '/plug-in/wechat/loginqrcode.html';
        });

        connection.on("Canceled", () => {
            main.qr.show = true;
            main.info.show = false;
            getQRCodeUrl();
        });

        connection.start().then(() => {
            connection.invoke('SetState', main.signalr.s).then(() => {

            }).catch(error => {
                console.info('无法接收消息推送.');
            });
        });
    }
};