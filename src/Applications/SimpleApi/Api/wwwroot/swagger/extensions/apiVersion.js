/*
*   swagger 新增 自动设置接口版本功能
*
*   LCTR 2021-06-18
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => 自动设置接口版本');

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
            let setting = { on: true, version: '1.0' };

            let down = (name, e) => {
                let close = window.showDialog(
                    '自动设置接口版本 - 设置',
                    [
                        ['H5', '当前状态', setting.on ? '已开启' : '已关闭'],
                        ['input', '默认版本', setting.version, {
                            'change': (e) => {
                                setting.version = e.target.value;
                            }
                        }]
                    ],
                    {
                        '开关': {
                            'click': () => {
                                setting.on = !setting.on;
                                $('#key_0').text(setting.on ? '已开启' : '已关闭');
                            }
                        }
                    });
            };

            $(document, '.try-out__btn').on('click', e => {
                if (e.target.nodeName != 'BUTTON' || e.target.className.indexOf('cancel') == -1)
                    return;

                $(e.target.parentElement.parentElement).next().find(".parameters tr").each((index, item) => {
                    if (item.dataset['paramName'] != 'api-version')
                        return;

                    let input = $(item).find('input')[0];

                    let nativeInputValueSetter = Object.getOwnPropertyDescriptor(input.nodeName == 'TEXTAREA' ? window.HTMLTextAreaElement.prototype : window.HTMLInputElement.prototype, "value").set;
                    nativeInputValueSetter.call(input, setting.version);
                    let ev2 = new Event('input', { bubbles: true });
                    input.dispatchEvent(ev2);
                });
            });

            window.addPlugIn('apiVersion', '自动设置接口版本', down, '<svg t="1629249921933" viewBox="0 0 1174 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2249" width="5em" height="5em"><path d="M790.932851 804.175127H643.311388l25.028923-158.29746h147.617432l-25.028924 158.29746z m-411.489537 0H220.516905L92.203354 107.606633h153.269904L315.963867 595.202966a879.552182 879.552182 0 0 0 8.898008 69.660075 65.563846 56.766631 90 0 1 8.144077-15.764029c5.426696-10.534887 12.966015-28.55668 22.763096-53.896046L571.917673 107.606633h153.197333L379.443314 804.175127z" fill="#bfbfbf" p-id="2250"></path></svg>');

            console.info('已加载功能 => 自动设置接口版本');
        });
});