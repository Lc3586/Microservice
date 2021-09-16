/*
*   swagger 新增 CAS身份验证功能
*
*   LCTR 2020-03-13
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => CAS身份验证');

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
            /**
            *
            * 检查登录状态
            * LCTR 2020-12-07
            *
            * @method check
            *
            */
            let check = (done) => {
                //方式一
                $.ajax({
                    type: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    url: "/cas/authorized",
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
                        } else {
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

                //方式二
                //let casInfo = window.location.request('casInfo');
                //if (casInfo)
                //    addBtn(JSON.parse(decodeURI(casInfo).replace(/%3A/g, ':')));
                //else
                //    addBtn(false);
            };
            /**
            *
            * 添加操作按钮
            * LCTR 2020-12-07
            *
            * @method addBtn
            *
            * @param {string} data 登录信息
            * 
            */
            let addBtn = (data) => {
                let open = (name, e) => {
                    if (data) {
                        let content = [];
                        for (let item in data) {
                            content.push(['input-readonly', item, data[item]]);
                        }
                        window.showDialog(
                            'CAS登录信息',
                            content,
                            {
                                '注销（方式一）': {
                                    'click': () => {
                                        logout(1);
                                    }
                                },
                                '查看对接说明（方式一）': {
                                    'click': () => {
                                        window.showDialog(
                                            '注销方式一',
                                            [
                                                ['label', '注销登录', '跳转至注销地址/cas/logout?returnUrl=，并附带参数（登录后的重定向地址）'],
                                                ['label', '未登录', '浏览器重定向至指定地址'],
                                                ['label', '注销后', '浏览器重定向至指定地址'],
                                                ['label', '如果要单点注销', '附加参数logoutCAS=true']
                                            ]);
                                    }
                                },
                                '注销（方式二）': {
                                    'click': () => {
                                        logout(2);
                                    }
                                },
                                '查看对接说明（方式二）': {
                                    'click': () => {
                                        window.showDialog(
                                            '注销方式二',
                                            [
                                                ['label', '注销登录', 'POST请求/cas/logout接口'],
                                                ['label', '未登录', '接口返回状态码401'],
                                                ['label', '注销后', '接口返回状态码200']
                                            ]);
                                    }
                                }
                            });
                    } else {
                        window.showDialog(
                            'CAS登录',
                            null,
                            {
                                '登录（方式一）': {
                                    'click': () => {
                                        login(1);
                                    }
                                },
                                '查看对接说明（方式一）': {
                                    'click': () => {
                                        window.showDialog(
                                            '登录方式一',
                                            [
                                                ['label', '登录验证', 'POST请求/cas/authorized接口'],
                                                ['label', '未登录', '接口返回状态码401, 此时应该跳转至登录地址/cas/login?returnUrl=，并附带参数（登录后的重定向地址）'],
                                                ['label', '已登录', '接口返回状态码200, 以及身份信息']
                                            ]);
                                    }
                                },
                                '登录（方式二）': {
                                    'click': () => {
                                        login(2);
                                    }
                                },
                                '查看对接说明（方式二）': {
                                    'click': () => {
                                        window.showDialog(
                                            '登录方式二',
                                            [
                                                ['label', '登录验证', '跳转至验证地址/cas/authorize?returnUrl=，并附带参数（登录后的重定向地址）'],
                                                ['label', '未登录', '浏览器重定向至登录地址'],
                                                ['label', '登录后', '浏览器重定向指指定地址，地址栏附带身份信息'],
                                                ['label', '获取身份信息', 'JSON.parse(decodeURI(window.location.request("casInfo")).replace(/%3A/g, ":"))'],
                                                ['label', '注释', 'window.location.request()是本页面的一个封装方法']
                                            ]);
                                    }
                                },
                            });
                    }
                };

                let init = () => {
                    window.addPlugIn('cas', '统一身份认证-' + (data ? '已登录' : '未登录'), open, data ? '<svg t="1615632900894" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="37768" width="5em" height="5em"><path d="M512 943.559111l-9.443556-3.640889c-15.473778-6.030222-380.814222-149.276444-380.814222-365.795555V193.422222l15.587556-6.940444c8.988444-3.982222 204.8-58.595556 367.729778-103.765334l6.940444-2.275555 390.257778 109.795555v383.886223c0 216.177778-365.340444 359.765333-380.814222 365.795555zM174.08 229.376v344.746667C174.08 737.962667 456.362667 864.028444 512 887.466667c55.637333-22.755556 337.92-149.276444 337.92-313.116445V229.831111L512 134.826667c-118.101333 32.768-284.444444 78.961778-337.92 94.549333z" fill="#0590DF" p-id="37769"></path><path d="M460.913778 686.762667L265.443556 480.028444l38.001777-35.953777 157.582223 166.570666 259.527111-273.408 38.001777 36.067556-297.642666 313.457778z" fill="#1296db" p-id="37770"></path></svg>' : '<svg t="1615632900894" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="37768" width="5em" height="5em"><path d="M512 943.559111l-9.443556-3.640889c-15.473778-6.030222-380.814222-149.276444-380.814222-365.795555V193.422222l15.587556-6.940444c8.988444-3.982222 204.8-58.595556 367.729778-103.765334l6.940444-2.275555 390.257778 109.795555v383.886223c0 216.177778-365.340444 359.765333-380.814222 365.795555zM174.08 229.376v344.746667C174.08 737.962667 456.362667 864.028444 512 887.466667c55.637333-22.755556 337.92-149.276444 337.92-313.116445V229.831111L512 134.826667c-118.101333 32.768-284.444444 78.961778-337.92 94.549333z" fill="#8a8a8a" p-id="37769"></path><path d="M460.913778 686.762667L265.443556 480.028444l38.001777-35.953777 157.582223 166.570666 259.527111-273.408 38.001777 36.067556-297.642666 313.457778z" fill="#8a8a8a" p-id="37770"></path></svg>');
                };

                init();

                if (data) {
                    //定时检查  
                    let checking = setInterval(() => {
                        check((dataB) => { dataB ? 1 : (window.clearInterval(checking), addBtn(false)) });
                    }, 30000);
                }
            };

            /**
            *
            * 登录
            * LCTR 2020-12-07
            *
            * @method login
            *
            * @param {number} mode 方式
            *
            */
            let login = (mode) => {
                switch (mode) {
                    case 2:
                        location.href = "/cas/authorize?returnUrl=" + location.href;
                        break;
                    case 1:
                    default:
                        location.href = $('[for="servers"] select').val() + '/cas/login?returnUrl=' + location.href;
                        break;
                }
            };

            /**
            *
            * 注销
            * LCTR 2020-12-07
            *
            * @method logout
            *
            * @param {number} mode 方式
            *
            */
            let logout = (mode) => {
                switch (mode) {
                    case 1:
                    default:
                        let done = (logoutCAS) => {
                            location.href = $('[for="servers"] select').val() + '/cas/logout?returnUrl=' + location.href + '&logoutCAS=' + logoutCAS;
                        };

                        window.showDialog(
                            '是否单点注销',
                            [
                                ['label', '单点注销', '当前登录的所有应用都会注销']
                            ],
                            {
                                '是': {
                                    'click': () => {
                                        done('true');
                                    }
                                },
                                '否': {
                                    'click': () => {
                                        done('false');
                                    }
                                }
                            },
                            false);
                        break;
                    case 2:
                        $.ajax({
                            type: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            url: "/cas/logout",
                            dataType: 'json',
                            success: function (data) {
                                window.location.reload();
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
                        break;
                }
            };

            let checking = () => {
                check(addBtn);
            };

            window.onInformationChanged(() => {
                window.delayedEvent(checking, null, 150, 'cas-check');
            });

            window.delayedEvent(checking, null, 100, 'cas-check');

            console.info('已加载功能 => CAS身份验证');
        });
});