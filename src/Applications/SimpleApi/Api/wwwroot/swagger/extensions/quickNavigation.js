/*
*   swagger 新增 接口标签快速导航功能
*
*   LCTR 2021-03-13
*/
window.onDomLoaded(() => {
    console.info('正在加载功能 => 接口标签快速导航');

    window.importFile(
        [
            {
                tag: 'script',
                type: 'text/javascript',
                src: '../../jquery/jquery-2.2.4.min.js'
            }
        ],
        () => {
            window.onInformationLoaded(() => {
                $('.operation-filter-input').attr('placeholder', '标签名称（区分大小写）')
                    .on('change', (e) => {
                        generateTags();
                    });

                //生成左侧标签
                var generateTags = () => {
                    var $tagBody = $('.tagsBody');
                    if ($tagBody.length == 0)
                        $tagBody = $('<div class="tagsBody"></div>')
                            .appendTo($('body'));

                    $('.opblock-tag').each((index, item) => {
                        var $tag = $('<span title="{1}">{0}</span>'.format(item.dataset['tag'], $(item).find('.renderedMarkdown p').text()))
                            .css({ 'top': (item.offsetTop + 5) + 'px' })
                            .appendTo($tagBody)
                            .on('click', (e) => {
                                $(document).scrollTop(item.offsetTop);
                            });

                        $(window).scroll(function (e) {
                            var current = index * 75;
                            var valueA = item.offsetTop - $(document).scrollTop();
                            var valueB = valueA - current;
                            $tag.css({
                                'top': (valueB > 0 ? valueA : current + 10) + 'px'
                            });

                            if (valueB <= 0 && current >= window.outerHeight) {
                                $('.tagsBody span').each((indexB, itemB) => {
                                    $(itemB).css({ 'top': (indexB - 1) * 75 - current + window.outerHeight + 'px' });
                                });
                            }
                        });
                    });
                };

                //监听HTML结构发生的变化
                var mutationObserver = window.MutationObserver
                    || window.WebKitMutationObserver
                    || window.MozMutationObserver;//浏览器兼容

                var observer = new mutationObserver((mutations) => {
                    for (var index in mutations) {
                        if (mutations[index].type == 'childList') {
                            $('.tagsBody').empty();
                            generateTags();
                            break;
                        }
                    }
                });
                observer.observe($(".opblock-tag-section")[0].parentElement.parentElement, { attributes: false, childList: true });

                generateTags();

                console.info('已加载功能 => 接口标签快速导航');
            });
        });
});