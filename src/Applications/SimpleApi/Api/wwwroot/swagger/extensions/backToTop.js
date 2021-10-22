/*
*   swagger 新增 回到顶部功能
*
*   LCTR 2021-03-13
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 回到顶部');

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
            let $scrollToTop = $('<i title="返回顶部" class="scrollToTop" ><svg t="1615613258516" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="32909" width="80" height="80"><path d="M869.477693 469.30536L554.231089 22.857856c-19.341263-27.429428-65.152883-27.397459-84.494147 0L154.522307 469.30536a51.598014 51.598014 0 0 0-3.676439 53.644033c8.919359 17.199338 26.534295 27.908963 45.939496 27.908963h185.740064v296.704567a23.976773 23.976773 0 0 0 47.953545 0v-319.6903a23.497237 23.497237 0 0 0-2.014048-9.494802 23.880865 23.880865 0 0 0-22.250445-15.47301H196.753395c-0.575443 0-2.30177 0-3.324779-2.014049-1.054978-2.046018-0.063938-3.452655 0.255752-3.932191l315.214636-446.447504a3.772346 3.772346 0 0 1 6.138054 0l315.246605 446.447504c0.31969 0.479535 1.342699 1.886173 0.255752 3.900222-1.054978 2.014049-2.749337 2.014049-3.324779 2.014049h-208.565952l-0.159845 0.031969-0.159845-0.031969a23.976773 23.976773 0 0 0-23.976773 23.976772v263.520715a23.976773 23.976773 0 0 0 47.953545 0V550.826387h184.90887c19.373232 0 36.988168-10.709625 45.907527-27.908963a51.470138 51.470138 0 0 0-3.64447-53.612064zM618.360963 893.534389a23.976773 23.976773 0 0 0-23.976773 23.976772v81.552996a23.976773 23.976773 0 0 0 47.953545 0V917.511161a23.976773 23.976773 0 0 0-23.976772-23.976772z" fill="#438CFF" p-id="32910"></path><path d="M577.24879 288.232774m-31.96903 0a31.96903 31.96903 0 1 0 63.93806 0 31.96903 31.96903 0 1 0-63.93806 0Z" fill="#438CFF" p-id="32911"></path></svg></i>')
                .appendTo($("body"))
                .on('click', (e) => { $(document).scrollTop(0); });

            $(window).scroll(function (e) {
                h = $(window).height();
                t = $(document).scrollTop();
                if (t > h) {
                    $scrollToTop.show();
                } else {
                    $scrollToTop.hide();
                }
            });

            console.info('已加载功能 => 回到顶部');
        });
});