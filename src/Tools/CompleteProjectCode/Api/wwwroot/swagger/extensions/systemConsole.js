/*
*   swagger 新增 系统控制台功能
*
*   LCTR 2021-06-08
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => 系统控制台');

    window.importFile(
        [
            {
                tag: 'script',
                type: 'text/javascript',
                src: window.baseUri + '/jquery/jquery-2.2.4.min.js'
            },
            {
                tag: 'script',
                type: 'text/javascript',
                src: window.baseUri + '/jquery/jquery-ui.min.js'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: window.baseUri + '/jquery/jquery-ui.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: window.baseUri + '/jquery/jquery-ui.theme.min.css'
            },
            {
                tag: 'link',
                type: 'text/css',
                rel: 'stylesheet',
                href: window.baseUri + '/jquery/jquery-ui.structure.min.css'
            }
        ],
        () => {
            var open = (name, e) => {
                window.showDialog(
                    '系统控制台',
                    [['iframe', window.baseUri + '/plug-in/systemconsole/systemconsole.html']],
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
                                window.open(window.baseUri + '/plug-in/systemconsole/systemconsole.html');
                            }
                        }
                    }, true, false, true);
            };

            window.addPlugIn('systemConsole', '系统控制台', open, '<svg t="1623136819721" class="icon" viewBox="0 0 1097 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2022" width="5em" height="5em"><path d="M585.375449 804.57023v146.280918h182.855344a36.574426 36.574426 0 0 1 0 73.148852H329.371252a36.574426 36.574426 0 0 1 0-73.148852h182.87213v-146.280918H73.383841c-81.407052 0-73.148852-71.101087-73.148852-71.101087V73.148852S-8.02321 0 73.383841 0h950.851148s72.712443 0 72.712443 73.148852V733.502713c0-5.119413 8.778534 71.101087-72.712443 71.101087zM439.09453 511.941253h402.285115a36.574426 36.574426 0 1 1 0 73.148851H439.09453v36.574426h-109.723278v-36.524071h-73.132066a36.574426 36.574426 0 0 1 0-73.148851h73.148851v-36.557641h109.706493z m219.42977-292.511483v-36.574426h109.706493v36.574426h73.148852a36.574426 36.574426 0 1 1 0 73.148852h-73.148852v36.574426h-109.706493v-36.574426H256.239186a36.574426 36.574426 0 0 1 0-73.148852zM73.383841 73.148852V733.502713h950.851148V73.148852z" fill="#515151" p-id="2023"></path></svg>', (name, e) => { window.open(window.baseUri + '/plug-in/systemconsole/systemconsole.html'); });

            console.info('已加载功能 => 系统控制台');
        });
});