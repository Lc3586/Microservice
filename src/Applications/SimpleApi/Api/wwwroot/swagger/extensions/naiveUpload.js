/*
*   swagger 新增 文件上传功能
*
*   LCTR 2021-07-24
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 文件上传');

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
            let open = (name, e) => {
                window.showDialog(
                    '文件上传',
                    [['iframe', '/plug-in/naiveupload/index.html']],
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
                                window.open('/plug-in/naiveupload/index.html');
                            }
                        }
                    }, true, false, true);
            };

            window.addPlugIn('naiveUpload', '文件上传', open, '<svg t="1627127083995" class="icon" viewBox="0 0 1080 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2942" width="5em" height="5em"><path d="M277.267692 338.673934h784.755341c10.296264 0 17.903121 8.945934 17.014154 18.792088l-45.236044 636.196571-727.422594-503.774241-29.122109-151.214418z" fill="#1E51AD" p-id="2943"></path><path d="M51.515077 501.985055h834.469978V139.128967H51.515077z" fill="#72AFEA" p-id="2944"></path><path d="M104.369231 427.25556h834.469978V64.421978H104.369231z" fill="#A2CCF4" p-id="2945"></path><path d="M893.153055 0H164.391385v362.833582h834.469978V108.701538z" fill="#C8E4FF" p-id="2946"></path><path d="M516.984967 108.026374h339.506637v46.293802H516.984967zM308.651604 223.772132h547.84V270.065934H308.662857z" fill="#9DCCFA" p-id="2947"></path><path d="M0 993.651341V380.286593l78.836747-78.735472h422.833231l61.811341-61.755077c6.71789-6.695385 15.675077-10.285011 25.532483-10.285011h355.643077c9.418549 0 18.364484 3.589626 25.093626 10.285011l64.039385 62.193934v692.111473c0 11.185231-8.957187 20.142418-20.142417 20.142417H20.142418c-11.185231-0.45011-20.142418-9.407297-20.142418-20.592527" fill="#0090FF" p-id="2948"></path><path d="M533.098901 504.606945a11.252747 11.252747 0 0 1 15.821363 1.755429l124.736703 155.918066a11.252747 11.252747 0 0 1-8.777143 18.285714h-63.015384V798.945055a11.252747 11.252747 0 0 1-11.252748 11.252747h-100.959648a11.252747 11.252747 0 0 1-11.252747-11.252747V680.566154h-63.015385a11.252747 11.252747 0 0 1-5.772659-1.59789l-1.237802-0.866462a11.252747 11.252747 0 0 1-1.766682-15.821362L531.343473 506.373626a11.252747 11.252747 0 0 1 1.755428-1.755428z" fill="#FFFFFF" p-id="2949"></path></svg>');

            console.info('已加载功能 => 文件上传');
        });
});