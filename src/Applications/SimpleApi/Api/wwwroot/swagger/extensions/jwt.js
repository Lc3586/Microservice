/*
*   swagger 新增 JWT功能
*
*   LCTR 2021-06-02
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => JWT');

    window.importFile(
        [
            {
                tag: 'script',
                type: 'text/javascript',
                src: '../../jquery/jquery-2.2.4.min.js'
            },
            {
                tag: 'script',
                type: 'text/javascript',
                src: '../../jquery/jquery-ui.min.js'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: '../../jquery/jquery-ui.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: '../../jquery/jquery-ui.theme.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: '../../jquery/jquery-ui.structure.min.css'
            }
        ],
        () => {
            var tokenInfo = JSON.parse(window.localStorage.getItem('jwt-token'));

            /**
            *
            * 检查登录状态
            *
            * @method check
            * 
            * @param {Function} done 回调事件
            *
            */
            var check = (done) => {
                $.ajax({
                    type: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + tokenInfo.AccessToken
                    },
                    url: "/jwt/authorized",
                    dataType: 'json',
                    success: function (data) {
                        typeof (appOffline) != "undefined" ? appOffline = false : 1;
                        if (data.Success)
                            done(data.Data);
                        else
                            done(false);
                    },
                    error: function (response) {
                        if (response.status == 401)
                            done(false);
                        else if (response.status == 503) {
                            console.info('站点维护中.');
                            typeof (appOffline) != "undefined" && appOffline ? 1 : (appOffline = true, window.showDialog('站点维护中'));
                        }
                        else {
                            done(false);
                            window.showDialog(
                                '接口异常',
                                [
                                    ['H5', '状态码', response.status],
                                    ['label', '输出内容', response.responseText]
                                ]);
                        }
                    }
                });
            };

            /**
            *
            * 添加操作按钮
            *
            * @method addBtn
            *
            * @param {string} data 登录信息
            * 
            */
            var addBtn = (data) => {
                var open = (name, e) => {
                    if (data) {
                        var content = [];
                        for (var item in data) {
                            content.push(['input-readonly', item, data[item]]);
                        }
                        window.showDialog(
                            'JWT登录信息',
                            content,
                            {
                                '刷新令牌': {
                                    'click': () => {
                                        refreshToken();
                                    }
                                },
                                '查看对接说明': {
                                    'click': () => {
                                        window.showDialog(
                                            '刷新令牌',
                                            [
                                                ['label', '将之前的token配置在Header中', '"Authorization":"Bearer xxxxxxxxxxxxxx"'],
                                                ['label', '刷新令牌', 'POST请求/jwt/refresh-token接口'],
                                                ['label', '状态码401', '令牌无效或已过期, 重新请求/jwt/get-token接口，并附带参数'],
                                                ['label', '状态码200', '接口返回新的令牌信息']
                                            ]);
                                    }
                                }
                            });
                    } else {
                        window.showDialog(
                            '获取令牌',
                            [
                                ['input', '用户名', ''],
                                ['password', '密码', '']
                            ],
                            {
                                '获取令牌': {
                                    'click': () => {
                                        getToken($('#key_0').val(), $('#key_1').val());
                                    }
                                },
                                '查看对接说明': {
                                    'click': () => {
                                        window.showDialog(
                                            '获取令牌',
                                            [
                                                ['label', '将token配置在Header中', '"Authorization":"Bearer xxxxxxxxxxxxxx"'],
                                                ['label', '登录验证', 'POST请求/jwt/authorized接口'],
                                                ['label', '状态码401', '令牌无效或已过期, 重新请求/jwt/get-token接口，并附带参数'],
                                                ['label', '状态码200', '接口返回身份信息']
                                            ]);
                                    }
                                }
                            });
                    }
                };

                var init = () => {
                    window.addPlugIn('jwt', 'JWT-' + (data ? '已登录' : '未登录'), open, data ? '<svg t="1622600326761" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="7460" width="5em" height="5em"><path d="M870.4 0H153.6C51.2 0 0 51.2 0 153.6v716.8C0 972.8 51.2 1024 153.6 1024h716.8c102.4 0 153.6-51.2 153.6-153.6V153.6C1024 51.2 972.8 0 870.4 0zM243.2 553.6c0 35.2-6.4 60.8-22.4 80s-35.2 28.8-60.8 28.8c-12.8 0-22.4-3.2-28.8-6.4v-35.2c6.4 6.4 16 9.6 28.8 9.6 32 0 44.8-25.6 44.8-76.8v-185.6h38.4v185.6z m339.2 105.6h-44.8L480 451.2c-3.2-9.6-3.2-19.2-3.2-28.8 0 9.6-3.2 19.2-6.4 28.8l-57.6 211.2h-44.8l-83.2-291.2h41.6l60.8 220.8c3.2 9.6 3.2 19.2 3.2 28.8 0-6.4 3.2-16 6.4-28.8l64-220.8h38.4l60.8 220.8c3.2 6.4 3.2 16 6.4 28.8 0-9.6 3.2-19.2 6.4-28.8l57.6-220.8H672l-89.6 288z m307.2-256h-83.2v256H768v-256h-83.2v-32h204.8v32z" fill="#1296db" p-id="7461"></path></svg>' : '<svg t="1622600326761" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="7460" width="5em" height="5em"><path d="M870.4 0H153.6C51.2 0 0 51.2 0 153.6v716.8C0 972.8 51.2 1024 153.6 1024h716.8c102.4 0 153.6-51.2 153.6-153.6V153.6C1024 51.2 972.8 0 870.4 0zM243.2 553.6c0 35.2-6.4 60.8-22.4 80s-35.2 28.8-60.8 28.8c-12.8 0-22.4-3.2-28.8-6.4v-35.2c6.4 6.4 16 9.6 28.8 9.6 32 0 44.8-25.6 44.8-76.8v-185.6h38.4v185.6z m339.2 105.6h-44.8L480 451.2c-3.2-9.6-3.2-19.2-3.2-28.8 0 9.6-3.2 19.2-6.4 28.8l-57.6 211.2h-44.8l-83.2-291.2h41.6l60.8 220.8c3.2 9.6 3.2 19.2 3.2 28.8 0-6.4 3.2-16 6.4-28.8l64-220.8h38.4l60.8 220.8c3.2 6.4 3.2 16 6.4 28.8 0-9.6 3.2-19.2 6.4-28.8l57.6-220.8H672l-89.6 288z m307.2-256h-83.2v256H768v-256h-83.2v-32h204.8v32z" fill="#8a8a8a" p-id="7461"></path></svg>');
                };

                init();

                if (data) {
                    //定时更新令牌
                    var updating = setInterval(refreshToken, new Date(tokenInfo.Expires) - new Date() - 60 * 1000);
                }
            };

            /**
            *
            * 获取令牌
            *
            * @method getToken
            *
            * @param {string} account 用户名
            * @param {string} password 密码
            *
            */
            var getToken = (account, password) => {
                $.ajax({
                    type: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    url: "/jwt/get-token",
                    dataType: 'json',
                    data: JSON.stringify({
                        account: account,
                        password: password
                    }),
                    success: function (data) {
                        if (data.Success) {
                            tokenInfo = data.Data;
                            window.localStorage.setItem('jwt-token', JSON.stringify(tokenInfo));
                            setToken(tokenInfo.AccessToken, () => {
                                check(addBtn);
                            });
                        }
                        else
                            window.showDialog(
                                '获取令牌失败',
                                [
                                    ['H5', '错误信息', data.Message]
                                ]);
                    },
                    error: function (response) {
                        window.showDialog(
                            '接口异常',
                            [
                                ['H5', '状态码', response.status],
                                ['label', '输出内容', response.responseText]
                            ]);
                    }
                });
            };

            /**
            *
            * 刷新令牌
            *
            * @method refreshToken
            *
            */
            var refreshToken = () => {
                $.ajax({
                    type: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + tokenInfo.AccessToken
                    },
                    url: "/jwt/refresh-token",
                    dataType: 'json',
                    success: function (data) {
                        if (data.Success) {
                            tokenInfo = data.Data;
                            window.localStorage.setItem('jwt-token', JSON.stringify(tokenInfo));
                            setToken(tokenInfo.AccessToken, () => {
                                check(addBtn);
                            });
                        }
                        else
                            window.showDialog(
                                '刷新令牌失败',
                                [
                                    ['H5', '错误信息', data.Message]
                                ]);
                    },
                    error: function (response) {
                        window.showDialog(
                            '接口异常',
                            [
                                ['H5', '状态码', response.status],
                                ['label', '输出内容', response.responseText]
                            ]);
                    }
                });
            };

            /**
             * 
             * 设置令牌
             * 
             * @method setToken
             * 
             * @param {string} token 令牌
             * @param {Function} done 回调事件
             */
            var setToken = (token, done) => {
                console.info('设置令牌.');
                $('.btn.authorize').click();
                console.info('open and set.');
                $('.auth-container input').val(token);
                console.info($('.auth-container input').val());
                $('[type="submit"]').click();
                console.info($('[type="submit"]').length);
                console.info('submit.');
                $('[type="submit"]').next().click();
                console.info('close.');
                done();
            };

            if (tokenInfo)
                setToken(tokenInfo.AccessToken, () => { check(addBtn); });
            else
                addBtn(false);

            console.info('已加载功能 => JWT');
        }
    );
});