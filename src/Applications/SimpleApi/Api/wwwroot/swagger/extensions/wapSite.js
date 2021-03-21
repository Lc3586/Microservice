/*
*   swagger 新增 用户端页面导航功能
*
*   LCTR 2021-03-14
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 用户端页面导航');

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
                window.open('/webVue/index.html');
            };

            window.addPlugIn('wapSite', '用户端', open, '<svg t="1616296885406" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="8644" width="5em" height="5em"><path d="M677.7 56.323H360.109c-68.642 0-124.276 55.634-124.276 124.275v648.995c0 68.642 55.634 124.276 124.276 124.276H677.7c68.641 0 124.276-55.634 124.276-124.276V180.598c0-68.641-55.635-124.275-124.276-124.275zM518.905 898.635c-19.07 0-34.52-15.451-34.52-34.52 0-19.07 15.45-34.522 34.52-34.522s34.521 15.452 34.521 34.521c0 19.07-15.451 34.521-34.52 34.521z m214.03-110.467h-428.06V222.023h428.06v566.145z" p-id="8645" fill="#d81e06"></path></svg>');

            console.info('已加载功能 => 用户端页面导航');
        });
});