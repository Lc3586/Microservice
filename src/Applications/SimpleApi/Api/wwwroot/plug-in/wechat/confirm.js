window.onload = () => {
    var main;

    //接口地址
    const apiUrls = {
        explain: window.BaseUrl + '/wechat-oath/explain',
        canfirm: window.BaseUrl + '/wechat-oath/confirm',
        cancel: window.BaseUrl + '/wechat-oath/cancel',
    };

    //vue实例
    const app = new Vue({
        el: '#app',
        data: getData,
        created() {
            main = this;
            getParam();
        },
        beforeDestroy() {
            if (!main.finish) {
                cancel();
                main.finish = true;
            }
        },
        destroyed() {
            if (!main.finish)
                cancel();
        },
        methods: {
            confirm: confirm,
            cancel: cancel
        }
    });

    /**
     * 获取渲染数据
     */
    function getData() {
        return {
            loading: true,
            explain: '',
            inform: '请确认这是您本人的操作',
            close: false,
            finish: false,
            canfirmDisabled: true,
            cancelDisabled: true,
            canfirmLoading: false,
            cancelLoading: false,
            message: ''
        };
    }

    /**
     * 获取参数
     * */
    function getParam() {
        main.state = getState();

        axios.get(apiUrls.explain + '/' + main.state).then(response => {
            main.loading = false;

            if (response.data.Success) {
                main.canfirmDisabled = false;
                main.cancelDisabled = false;
                main.explain = response.data.Data;
            }
            else {
                main.inform = '请重新扫码';
                main.close = true;
                vant.Dialog.alert({
                    title: '错误',
                    message: response.data.Message,
                }).then(() => {
                    main.explain = response.data.Message;
                });
            }
        }).catch(error => {
            main.loading = false;
            main.inform = '请重新扫码';
            main.close = true;
            vant.Dialog.alert({
                title: '错误',
                message: '获取数据失败.',
            }).then(() => {
                main.explain = '获取数据失败.';
            });
        });
    }

    /**
     * 获取state参数
     * */
    function getState() {
        var query = window.location.href; var iLen = 'state'.length; var iStart = query.indexOf('state'); if (iStart == -1) return ""; iStart += iLen + 1; var iEnd = query.indexOf("&", iStart); if (iEnd == -1) return query.substring(iStart); return query.substring(iStart, iEnd);
    }

    /**
     * 确认
     * */
    function confirm() {
        main.canfirmLoading = true;
        axios.get(apiUrls.canfirm + '/' + main.state).then(response => {
            if (response.data.Success) {
                main.finish = true;
                main.message = "已确认";
            }
            else {
                main.canfirmLoading = false;

                vant.Dialog.alert({
                    title: '错误',
                    message: response.data.Message,
                });
            }
        }).catch(error => {
            main.canfirmLoading = false;
            vant.Dialog.alert({
                title: '错误',
                message: '确认失败.',
            });
        });
    }

    /**
     * 取消
     * */
    function cancel() {
        main.cancelLoading = true;
        axios.get(apiUrls.cancel + '/' + main.state).then(response => {
            if (response.data.Success) {
                main.finish = true;
                main.message = "已取消";
            }
            else {
                main.cancelLoading = false;

                vant.Dialog.alert({
                    title: '错误',
                    message: response.data.Message,
                });
            }
        }).catch(error => {
            main.cancelLoading = false;
            vant.Dialog.alert({
                title: '错误',
                message: '取消失败.',
            });
        });
    }
};