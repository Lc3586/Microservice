﻿/*
*   swagger 新增 管理端页面导航功能
*
*   LCTR 2021-03-14
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => 管理端页面导航功能');

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
            let open = (name, e) => {
                window.open(window.baseUri + '/adminVue/index.html');
            };

            window.addPlugIn('adminSite', '管理端', open, '<svg t="1615694627304" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="4076" width="5em" height="5em"><path d="M204.8 921.6h614.4a51.2 51.2 0 0 1 0 102.4H204.8a51.2 51.2 0 0 1 0-102.4zM51.2 0h921.6a51.2 51.2 0 0 1 51.2 51.2v716.8a51.2 51.2 0 0 1-51.2 51.2H51.2a51.2 51.2 0 0 1-51.2-51.2V51.2a51.2 51.2 0 0 1 51.2-51.2z m409.6 591.104V665.6h102.4v-74.3936c10.4448-7.2192 21.504-15.8208 32.6656-25.2928l60.928 60.928 72.448-72.448-60.928-60.928c9.472-11.1616 18.0736-22.2208 25.2928-32.6656H768V358.4h-74.3936a426.5984 426.5984 0 0 0-25.344-32.6656l60.928-60.928-72.3968-72.448-60.928 60.9792A428.288 428.288 0 0 0 563.2 228.0448V153.6H460.8v74.4448c-10.4448 7.2192-21.504 15.8208-32.6656 25.2928l-60.928-60.928-72.448 72.3968 60.928 60.928A426.5984 426.5984 0 0 0 330.4448 358.4H256v102.4h74.4448c7.168 10.4448 15.8208 21.504 25.2928 32.6656l-60.928 60.928 72.3968 72.448 60.928-61.0304c11.264 9.472 22.2208 18.0736 32.6656 25.2928z" p-id="4077" fill="#13227a"></path></svg>');

            console.info('已加载功能 => 管理端页面导航功能');
        });
});