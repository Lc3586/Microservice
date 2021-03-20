﻿/*
*   swagger 新增 SampleAuthentication身份验证功能
*
*   LCTR 2021-02-20
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => SampleAuthentication身份验证');

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
            * LCTR 2021-02-20
            *
            * @method check
            *
            */
            var check = (done) => {
                $.ajax({
                    type: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    url: "/sa/authorized",
                    dataType: 'json',
                    success: function (data) {
                        typeof (appOffline) != "undefined" ? appOffline = false : 1;
                        done(data);
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
            * LCTR 2021-02-20
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
                            'SA登录信息',
                            content,
                            {
                                '注销': {
                                    'click': () => {
                                        logout(1);
                                    }
                                },
                                '查看对接说明': {
                                    'click': () => {
                                        window.showDialog(
                                            '注销',
                                            [
                                                ['label', '注销登录', '跳转至注销地址/sa/logout并附带参数（可选参数, 注销后的重定向地址）'],
                                                ['label', '未登录', '浏览器重定向至指定地址'],
                                                ['label', '注销后', '浏览器重定向至指定地址']
                                            ]);
                                    }
                                }
                            });
                    } else {
                        window.showDialog(
                            'SA登录',
                            [
                                ['input', '用户名', ''],
                                ['password', '密码', '']
                            ],
                            {
                                '登录': {
                                    'click': () => {
                                        login(1, $('#key_0').val(), $('#key_1').val());
                                    }
                                },
                                '查看对接说明': {
                                    'click': () => {
                                        window.showDialog(
                                            '登录',
                                            [
                                                ['label', '登录验证', 'POST请求/sa/authorized接口'],
                                                ['label', '未登录', '接口返回状态码401, 此时应该跳转至登录页面, 之后请求/sa/login接口，并附带参数, 返回成功后跳转至指定页面'],
                                                ['label', '已登录', '接口返回状态码200, 以及身份信息']
                                            ]);
                                    }
                                }
                            });
                    }
                };

                var init = () => {
                    window.addPlugIn('sa', '简易身份认证-' + (data ? '已登录' : '未登录'), open, data ? '<svg t="1615629776270" class="icon" viewBox="0 0 1057 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="33739" width="5em" height="5em"><path d="M854.441558 1024h-651.636363C91.096104 1024 0 932.903896 0 821.194805v-618.38961C0 91.096104 91.096104 0 202.805195 0h651.636363C966.150649 0 1057.246753 91.096104 1057.246753 202.805195v618.38961c0 111.709091-91.096104 202.805195-202.805195 202.805195zM202.805195 19.948052C101.735065 19.948052 19.948052 101.735065 19.948052 202.805195v618.38961C19.948052 922.264935 101.735065 1004.051948 202.805195 1004.051948h651.636363c101.07013 0 182.857143-81.787013 182.857143-182.857143v-618.38961C1037.298701 101.735065 955.511688 19.948052 854.441558 19.948052h-651.636363z" p-id="33740" fill="#1296db"></path><path d="M13.963636 646.316883c33.246753-4.654545 53.85974-7.314286 61.838961-7.979221 16.623377-1.994805 40.561039-3.98961 73.142858-7.97922 6.649351 40.561039 19.948052 69.818182 40.561039 89.101298 20.612987 18.618182 47.875325 28.592208 82.451948 28.592208 36.571429 0 63.833766-8.644156 82.451948-25.267532 18.618182-16.623377 27.927273-36.571429 27.927272-59.179221 0-14.628571-3.98961-26.597403-11.968831-36.571429-7.979221-9.974026-21.277922-19.283117-40.561039-26.597402-13.298701-5.319481-43.220779-13.963636-91.096104-26.597403-60.509091-16.623377-103.72987-36.571429-127.667532-60.509091-34.576623-33.911688-51.864935-74.472727-51.864935-123.012987 0-31.251948 7.979221-60.509091 24.602597-87.106493s39.896104-47.875325 69.818182-61.838961c30.587013-13.963636 67.158442-21.277922 110.379221-21.277923 70.483117 0 123.677922 16.623377 158.91948 50.535065 28.592208 27.262338 38.566234 39.231169 45.215585 81.122078 1.32987 9.974026 26.597403 43.885714 26.597402 54.524676h-46.545454c-18.618182 0.664935-46.545455 1.994805-83.116883 3.98961-4.654545-31.251948-15.293506-53.85974-31.251948-67.823377s-39.896104-20.612987-71.812987-20.612987c-33.246753 0-58.514286 7.314286-77.132468 21.942858-11.968831 9.309091-17.953247 21.942857-17.953247 37.901298 0 14.628571 5.319481 26.597403 16.623377 37.236364 14.628571 13.298701 49.205195 26.597403 104.394805 40.561039 55.18961 13.963636 95.750649 28.592208 122.348052 43.885714s47.21039 35.906494 61.838961 61.838961c14.628571 26.597403 22.607792 58.514286 22.607792 97.08052 0 35.241558-8.644156 67.823377-26.597402 98.410389-17.953247 30.587013-43.220779 53.194805-75.802598 68.488312s-73.142857 22.607792-122.348052 22.607792c-71.148052 0-125.672727-17.953247-163.574026-53.85974-29.257143-27.262338-49.205195-64.498701-60.509091-111.044156-2.65974-13.298701-29.922078-24.602597-31.916883-40.561039z" p-id="33741" fill="#1296db"></path><path d="M1041.953247 840.477922h-144.955844l-49.205195-138.971428H623.709091l-46.545455 138.971428H439.522078l31.251948-38.566234 204.8-572.509091h119.688312l210.784415 571.844156 35.906494 39.231169zM811.885714 598.441558l-77.132467-226.742857L658.285714 598.441558h153.6z" p-id="33742" fill="#1296db"></path></svg>' : '<svg t="1615633087734" class="icon" viewBox="0 0 1057 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="41498" width="5em" height="5em"><path d="M854.441558 1024h-651.636363C91.096104 1024 0 932.903896 0 821.194805v-618.38961C0 91.096104 91.096104 0 202.805195 0h651.636363C966.150649 0 1057.246753 91.096104 1057.246753 202.805195v618.38961c0 111.709091-91.096104 202.805195-202.805195 202.805195zM202.805195 19.948052C101.735065 19.948052 19.948052 101.735065 19.948052 202.805195v618.38961C19.948052 922.264935 101.735065 1004.051948 202.805195 1004.051948h651.636363c101.07013 0 182.857143-81.787013 182.857143-182.857143v-618.38961C1037.298701 101.735065 955.511688 19.948052 854.441558 19.948052h-651.636363z" p-id="41499" fill="#8a8a8a"></path><path d="M13.963636 646.316883c33.246753-4.654545 53.85974-7.314286 61.838961-7.979221 16.623377-1.994805 40.561039-3.98961 73.142858-7.97922 6.649351 40.561039 19.948052 69.818182 40.561039 89.101298 20.612987 18.618182 47.875325 28.592208 82.451948 28.592208 36.571429 0 63.833766-8.644156 82.451948-25.267532 18.618182-16.623377 27.927273-36.571429 27.927272-59.179221 0-14.628571-3.98961-26.597403-11.968831-36.571429-7.979221-9.974026-21.277922-19.283117-40.561039-26.597402-13.298701-5.319481-43.220779-13.963636-91.096104-26.597403-60.509091-16.623377-103.72987-36.571429-127.667532-60.509091-34.576623-33.911688-51.864935-74.472727-51.864935-123.012987 0-31.251948 7.979221-60.509091 24.602597-87.106493s39.896104-47.875325 69.818182-61.838961c30.587013-13.963636 67.158442-21.277922 110.379221-21.277923 70.483117 0 123.677922 16.623377 158.91948 50.535065 28.592208 27.262338 38.566234 39.231169 45.215585 81.122078 1.32987 9.974026 26.597403 43.885714 26.597402 54.524676h-46.545454c-18.618182 0.664935-46.545455 1.994805-83.116883 3.98961-4.654545-31.251948-15.293506-53.85974-31.251948-67.823377s-39.896104-20.612987-71.812987-20.612987c-33.246753 0-58.514286 7.314286-77.132468 21.942858-11.968831 9.309091-17.953247 21.942857-17.953247 37.901298 0 14.628571 5.319481 26.597403 16.623377 37.236364 14.628571 13.298701 49.205195 26.597403 104.394805 40.561039 55.18961 13.963636 95.750649 28.592208 122.348052 43.885714s47.21039 35.906494 61.838961 61.838961c14.628571 26.597403 22.607792 58.514286 22.607792 97.08052 0 35.241558-8.644156 67.823377-26.597402 98.410389-17.953247 30.587013-43.220779 53.194805-75.802598 68.488312s-73.142857 22.607792-122.348052 22.607792c-71.148052 0-125.672727-17.953247-163.574026-53.85974-29.257143-27.262338-49.205195-64.498701-60.509091-111.044156-2.65974-13.298701-29.922078-24.602597-31.916883-40.561039z" p-id="41500" fill="#8a8a8a"></path><path d="M1041.953247 840.477922h-144.955844l-49.205195-138.971428H623.709091l-46.545455 138.971428H439.522078l31.251948-38.566234 204.8-572.509091h119.688312l210.784415 571.844156 35.906494 39.231169zM811.885714 598.441558l-77.132467-226.742857L658.285714 598.441558h153.6z" p-id="41501" fill="#8a8a8a"></path></svg>');
                };

                init();

                if (data) {
                    //定时检查  
                    var checking = setInterval(() => {
                        check((dataB) => { dataB ? 1 : (window.clearInterval(checking), addBtn(false)) });
                    }, 30000);
                }
            };

            /**
            *
            * 登录
            * LCTR 2021-02-20
            *
            * @method login
            *
            * @param {number} mode 方式
            * @param {string} account 用户名
            * @param {string} password 密码
            *
            */
            var login = (mode, account, password) => {
                switch (mode) {
                    case 1:
                    default:
                        $.ajax({
                            type: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            url: "/sa/login",
                            dataType: 'json',
                            data: JSON.stringify({
                                account: account,
                                password: password
                            }),
                            success: function (data) {
                                if (data.Success)
                                    location.reload();
                                else
                                    window.showDialog(
                                        '登录失败',
                                        [
                                            ['H5', '错误信息', data.Msg]
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
                        break;
                }
            };

            /**
            *
            * 注销
            * LCTR 2021-02-20
            *
            * @method logout
            *
            * @param {number} mode 方式
            *
            */
            var logout = (mode) => {
                switch (mode) {
                    case 1:
                    default:
                        location.href = $('[for="servers"] select').val() + '/sa/logout?returnUrl=' + location.href;
                        break;
                }
            };

            check(addBtn);

            console.info('已加载功能 => SampleAuthentication身份验证');
        }
    );
});