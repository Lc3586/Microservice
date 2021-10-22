﻿/*
*   swagger 新增 系统用户微信登录后台功能
*
*   LCTR 2021-03-20
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 系统用户微信登录后台');

    window.importFile(
        [
            {
                tag: 'script',
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/jquery/jquery-2.2.4.min.js'
            },
            {
                tag: 'script',
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/jquery/jquery-ui.min.js'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: ApiUri.BaseUrl + '/jquery/jquery-ui.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: ApiUri.BaseUrl + '/jquery/jquery-ui.theme.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: ApiUri.BaseUrl + '/jquery/jquery-ui.structure.min.css'
            }
        ],
        () => {
            let open = (name, e) => {
                let close = window.showDialog(
                    '输入参数',
                    [
                        ['input', '二维码尺寸', '500']
                    ],
                    {
                        '确认': {
                            'click': () => {
                                close();
                                let url = '/plug-in/wechat/qrcode.html?method=login&size=' + $('#key_0').val();
                                window.showDialog(
                                    '系统用户微信登录',
                                    [
                                        ['iframe', url]
                                    ],
                                    {
                                        '刷新': {
                                            'click': () => {
                                                $('#key_0')[0].contentWindow.location.reload();
                                            }
                                        },
                                        '调整大小': {
                                            'click': () => {
                                                let state = $('#key_0').data('state');
                                                $('#key_0').data('state', state == 1 ? 0 : 1);
                                                //.animate({ 'height': (state == 1 ? 900 : 900) + 'px' });
                                                $('.modal-ux').animate({ 'width': state == 1 ? '100%' : '35%' });
                                            }
                                        },
                                        '在新页面中打开': {
                                            'click': () => {
                                                window.open(url);
                                            }
                                        }
                                    }, true, false, true);
                            }
                        }
                    });
            };

            window.addPlugIn('wechatUserLogin', '系统用户微信登录', open, '<svg t="1616230809296" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2687" width="5em" height="5em"><path d="M352.814545 385.396364m-33.512727 0a33.512727 33.512727 0 1 0 67.025455 0 33.512727 33.512727 0 1 0-67.025455 0Z" fill="#50B674" p-id="2688"></path><path d="M502.690909 384.465455m-33.512727 0a33.512727 33.512727 0 1 0 67.025454 0 33.512727 33.512727 0 1 0-67.025454 0Z" fill="#50B674" p-id="2689"></path><path d="M576.232727 534.341818m-23.272727 0a23.272727 23.272727 0 1 0 46.545455 0 23.272727 23.272727 0 1 0-46.545455 0Z" fill="#50B674" p-id="2690"></path><path d="M694.458182 536.203636m-23.272727 0a23.272727 23.272727 0 1 0 46.545454 0 23.272727 23.272727 0 1 0-46.545454 0Z" fill="#50B674" p-id="2691"></path><path d="M512 0C229.003636 0 0 229.003636 0 512s229.003636 512 512 512 512-229.003636 512-512S794.996364 0 512 0z m-87.505455 630.225455c-26.996364 0-48.407273-5.585455-75.403636-11.17091l-75.403636 37.236364 21.410909-64.232727c-53.992727-37.236364-85.643636-85.643636-85.643637-145.221818 0-102.4 96.814545-182.458182 215.04-182.458182 105.192727 0 198.283636 64.232727 216.901819 150.807273-6.516364-0.930909-13.963636-0.930909-20.48-0.93091-102.4 0-182.458182 76.334545-182.458182 170.356364 0 15.825455 2.792727 30.72 6.516363 44.683636-7.447273 0-13.963636 0.930909-20.48 0.93091z m314.647273 75.403636l15.825455 53.992727-58.647273-32.581818c-21.410909 5.585455-42.821818 11.170909-64.232727 11.170909-102.4 0-182.458182-69.818182-182.458182-155.461818s80.058182-155.461818 182.458182-155.461818c96.814545 0 182.458182 69.818182 182.458182 155.461818 0 47.476364-31.650909 90.298182-75.403637 122.88z" fill="#50B674" p-id="2692"></path></svg>');

            console.info('已加载功能 => 系统用户微信登录后台');
        });
});