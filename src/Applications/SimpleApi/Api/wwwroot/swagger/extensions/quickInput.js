/*
*   swagger 新增 文本框&文本域快速输入功能
*
*   LCTR 2021-06-18
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => 文本框&文本域快速输入');

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
            let flag = true;

            let down = (name, e) => {
                let close = window.showDialog(
                    '文本框&文本域快速输入 - 设置',
                    [
                        ['H5', '当前状态', flag ? '已开启' : '已关闭']
                    ],
                    {
                        '开关': {
                            'click': () => {
                                flag = !flag;
                                $('#key_0').text(flag ? '已开启' : '已关闭');
                            }
                        }
                    });
            };

            $(document, '.block-desktop').on('focus', 'input,textarea', e => {
                if (!flag || (e.target.type != 'text' && e.target.type != 'textarea') || e.target.className.indexOf('noQuickInput') > -1)
                    return;

                let $input = e.target.nodeName == 'TEXTAREA'
                    ? $(`<textarea class="noQuickInput ${e.target.className}" title="${e.target.title}"></textarea>`)
                    : $(`<input type="text" class="noQuickInput ${e.target.className}" title="${e.target.title}" placeholder="${e.target.placeholder}">`);

                let setValue = (value) => {
                    //设置input值并触发onchange事件
                    let nativeInputValueSetter = Object.getOwnPropertyDescriptor(e.target.nodeName == 'TEXTAREA' ? window.HTMLTextAreaElement.prototype : window.HTMLInputElement.prototype, "value").set;
                    nativeInputValueSetter.call(e.target, value);
                    let ev2 = new Event('input', { bubbles: true });
                    e.target.dispatchEvent(ev2);
                };

                $input.val(e.target.value)
                    .on('blur', _e => {
                        setValue(_e.target.value);

                        _e.target.remove();
                        $(e.target).show();
                    })
                    .focus();

                $(e.target).hide().after($input);

                $input.parents('.opblock-section')
                    .next()
                    .find('.btn.execute')
                    .off('mouseenter')
                    .on('mouseenter', _e => {
                        setValue($input.val());
                    });
            });

            window.addPlugIn('quickInput', '文本框&文本域快速输入', down, '<svg t="1624006192258" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="5149" width="5em" height="5em"><path d="M512 0C229.12 0 0 229.12 0 512s229.12 512 512 512 512-229.12 512-512S794.88 0 512 0z m0 857.6c-190.72 0-345.6-154.88-345.6-345.6s154.88-345.6 345.6-345.6 345.6 154.88 345.6 345.6-154.88 345.6-345.6 345.6z" p-id="5150" fill="#07f7f2"></path><path d="M512 166.4c-190.72 0-345.6 154.88-345.6 345.6s154.88 345.6 345.6 345.6 345.6-154.88 345.6-345.6-154.88-345.6-345.6-345.6z m-5.12 104.96c11.52-7.68 25.6-8.96 37.12-1.28 10.24 8.96 14.08 23.04 11.52 37.12-5.12 14.08-11.52 26.88-21.76 37.12l-24.32 33.28-6.4 8.96-12.8 16.64c-1.28 1.28-2.56 2.56-2.56 3.84l-1.28 5.12-52.48-42.24 2.56-3.84c8.96-14.08 40.96-57.6 46.08-64l10.24-15.36c3.84-5.12 8.96-10.24 14.08-15.36zM396.8 354.56l3.84-5.12 60.16-83.2c2.56-6.4 8.96-10.24 15.36-10.24 7.68 0 14.08 5.12 15.36 12.8 1.28 5.12 0 10.24-3.84 14.08l-3.84 5.12-48.64 66.56c-8.96 11.52-16.64 26.88-26.88 26.88-7.68 0-14.08-5.12-15.36-11.52-1.28-5.12 0-10.24 3.84-15.36z m-94.72 240.64v-8.96c3.84-21.76 10.24-43.52 20.48-62.72 2.56-5.12 5.12-10.24 8.96-14.08l70.4-97.28c7.68-11.52 14.08-19.2 15.36-20.48 3.84 1.28 49.92 39.68 51.2 42.24l-2.56 3.84c-16.64 23.04-83.2 116.48-93.44 126.72-12.8 12.8-25.6 24.32-40.96 33.28l-5.12 2.56c-2.56 1.28-5.12 2.56-6.4 2.56-2.56 0-5.12 1.28-6.4 2.56-2.56 3.84-6.4 8.96-10.24 11.52-3.84 1.28-7.68 0-10.24-3.84 0-1.28-1.28-2.56 0-3.84 1.28-5.12 7.68-8.96 8.96-14.08z m446.72 81.92c-7.68 6.4-17.92 5.12-24.32-2.56-28.16-35.84-56.32-53.76-83.2-53.76H640c-21.76 0-48.64 16.64-72.96 33.28l-17.92 12.8c-12.8 8.96-24.32 17.92-35.84 25.6-28.16 16.64-58.88 24.32-92.16 24.32h-6.4c-40.96-1.28-81.92-16.64-113.92-42.24-7.68-6.4-8.96-16.64-2.56-24.32 6.4-7.68 16.64-8.96 24.32-2.56 26.88 21.76 58.88 34.56 93.44 35.84 28.16 1.28 55.04-5.12 79.36-19.2 6.4-3.84 12.8-8.96 19.2-12.8l26.88-20.48c32-23.04 66.56-44.8 99.84-43.52 38.4 1.28 74.24 23.04 108.8 66.56 6.4 6.4 5.12 17.92-1.28 23.04z" p-id="5151" fill="#07f7f2"></path><path d="M512 76.8C271.36 76.8 76.8 271.36 76.8 512s194.56 435.2 435.2 435.2 435.2-194.56 435.2-435.2S752.64 76.8 512 76.8z m0 780.8c-190.72 0-345.6-154.88-345.6-345.6s154.88-345.6 345.6-345.6 345.6 154.88 345.6 345.6-154.88 345.6-345.6 345.6z" p-id="5152" fill="#e5f707"></path></svg>');

            console.info('已加载功能 => 文本框&文本域快速输入');
        });
});