﻿/*
*   swagger 新增 系统用户绑定微信功能
*
*   LCTR 2021-03-20
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 系统用户绑定微信');

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
            var open = (name, e) => {
                window.showDialog(
                    '输入参数',
                    [
                        ['input', '用户ID', ''],
                        ['input', '是否同步微信信息至用户信息', 'false'],
                        ['input', '二维码尺寸', '500']
                    ],
                    {
                        '确认': {
                            'click': () => {
                                var url = '/plug-in/wechat/bindqrcode.html?userId=' + $('#key_0').val() + '&asyncUserInfo=' + $('#key_1').val() + '&size=' + $('#key_2').val();
                                window.showDialog(
                                    '系统用户绑定微信',
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
                                                var state = $('#key_0').data('state');
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

            window.addPlugIn('wechatUserBind', '系统用户绑定微信', open, '<svg t="1616253959453" class="icon" viewBox="0 0 1181 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="6647" width="5em" height="5em"><path d="M702.621538 689.073231a33.398154 33.398154 0 0 1 45.449847 0c11.027692 10.397538 12.366769 26.466462 4.174769 38.281846l-4.174769 4.726154-13.154462 12.603077c-22.843077 21.661538-40.487385 55.296-40.487385 86.252307s12.603077 59.943385 35.446154 81.683693c22.921846 21.661538 53.563077 33.634462 86.173539 33.634461 28.593231 0 55.532308-9.137231 77.193846-25.993846l8.900923-7.640615 18.116923-17.250462a33.398154 33.398154 0 0 1 45.528615 0 29.380923 29.380923 0 0 1 4.096 38.281846l-4.096 4.726154-18.195692 17.329231a185.265231 185.265231 0 0 1-61.518769 38.596923 195.268923 195.268923 0 0 1-70.025846 12.839385c-24.182154 0-47.734154-4.332308-70.025846-12.839385a185.265231 185.265231 0 0 1-61.44-38.596923 174.237538 174.237538 0 0 1-40.802462-58.368 168.566154 168.566154 0 0 1 0-132.726154c7.798154-18.353231 22.055385-38.281846 36.785231-54.114461l8.900923-8.979693 13.233231-12.445538zM585.097846 28.514462c285.144615 0 510.188308 184.871385 510.188308 412.435692 0 8.034462-0.315077 15.911385-0.787692 23.788308a236.307692 236.307692 0 0 0-260.253539 8.743384l-13.469538 10.397539-178.333539 146.904615a236.307692 236.307692 0 0 0-82.707692 221.814154l-26.072616-2.284308c-42.850462-4.962462-85.779692-15.123692-128.630153-25.284923l-165.021539 85.307077 44.977231-142.178462C164.864 682.771692 74.830769 569.028923 74.830769 440.950154 74.830769 213.464615 314.919385 28.514462 585.019077 28.514462z m387.938462 658.668307a35.131077 35.131077 0 0 1-0.945231 51.436308L866.776615 834.953846a39.936 39.936 0 0 1-54.19323-0.866461 35.131077 35.131077 0 0 1 0.94523-51.436308l105.235693-96.334769a39.936 39.936 0 0 1 54.19323 0.866461z m-6.537846-174.867692c24.182154 0 47.655385 4.332308 69.947076 12.839385 23.158154 8.900923 43.795692 21.897846 61.597539 38.596923 17.644308 16.856615 31.350154 36.470154 40.723692 58.368a168.566154 168.566154 0 0 1 0 132.726153 172.898462 172.898462 0 0 1-30.641231 47.970462l-10.082461 10.397538-18.116923 17.171693a33.398154 33.398154 0 0 1-45.528616 0 29.223385 29.223385 0 0 1-4.096-38.281846l4.096-4.726154 18.195693-17.250462a111.694769 111.694769 0 0 0 0-163.288615 124.455385 124.455385 0 0 0-86.094769-33.634462 125.243077 125.243077 0 0 0-77.193847 25.993846l-8.979692 7.640616-18.116923 17.250461a33.398154 33.398154 0 0 1-45.449846 0 29.223385 29.223385 0 0 1-4.096-38.281846l4.096-4.726154 18.116923-17.32923c17.723077-16.699077 38.439385-29.696 61.597538-38.596923 22.291692-8.507077 45.843692-12.839385 70.025847-12.839385zM750.040615 256.078769c-44.977231 0-74.988308 28.435692-74.988307 56.871385s29.932308 56.950154 74.988307 56.950154c30.011077 0 60.022154-28.514462 60.022154-56.950154 0-28.356923-29.932308-56.871385-60.022154-56.871385z m-330.043077 0c-45.056 0-75.067077 28.435692-75.067076 56.871385s30.011077 56.950154 74.988307 56.950154c30.011077 0 60.022154-28.514462 60.022154-56.950154 0-28.356923-29.932308-56.871385-60.022154-56.871385z" p-id="6648" fill="#13227a"></path></svg>');

            console.info('已加载功能 => 系统用户绑定微信');
        });
});