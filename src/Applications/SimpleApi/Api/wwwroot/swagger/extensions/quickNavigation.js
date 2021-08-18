/*
*   swagger 新增 接口标签快速导航功能
*
*   LCTR 2021-03-13
*/
window.onInformationLoaded(() => {
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
            $('.operation-filter-input').attr('placeholder', '标签名称（区分大小写）')
                .on('input', (e) => {
                    window.delayedEvent(generateTags, null, 100, 'generate-Tags');
                });

            //生成左侧标签
            var generateTags = () => {
                var $tagBody = $('.tagsBody');
                if ($tagBody.length == 0)
                    $tagBody = $('<div class="tagsBody"></div>')
                        .appendTo($('body'));
                else
                    $('.tagsBody').empty();

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

                var changeTags = (state) => {
                    if (state == 'hidden' || state == 'show') {
                        $changeTags.data('state', state == 'show' ? 'unlocked' : 'hidden');

                        $changeTags.animate({
                            'left': state == 'hidden' ? '0px' : `${$('.tagsBody span').width() + 50}px`
                        }, () => {
                            $changeTags
                                .attr('title', state == 'hidden' ? '显示导航标签' : '锁定导航标签')
                                .empty()
                                .html(state == 'hidden' ? '<svg t="1623299549301" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="3903" width="50" height="50"><path d="M838.101333 552.96l-239.061333 277.461333c-23.893333 27.733333-65.194667 29.866667-91.306667 8.533334a63.445333 63.445333 0 0 1-8.661333-89.6l204.288-236.928-204.288-236.885334c-23.893333-27.733333-19.541333-68.266667 8.704-89.6 28.245333-23.509333 69.546667-19.242667 91.306667 8.533334l239.018666 277.418666c10.88 10.666667 15.232 25.6 15.232 40.533334s-4.352 29.866667-15.232 40.533333z" fill="#8a8a8a" p-id="3904"></path><path d="M496.768 552.96L257.706667 830.464c-23.893333 27.733333-65.194667 29.866667-91.306667 8.533333a63.445333 63.445333 0 0 1-8.661333-89.6l204.288-236.928-204.288-236.885333c-23.893333-27.733333-19.541333-68.266667 8.704-89.6 28.245333-23.509333 69.546667-19.242667 91.306666 8.533333l239.018667 277.418667c10.88 10.666667 15.232 25.6 15.232 40.533333s-4.352 29.866667-15.232 40.533334z" fill="#8a8a8a" p-id="3905"></path></svg>' : '<svg width="50" height="50" fill="#1296db"><use href="#unlocked"></use></svg>');
                        });

                        $('.tagsBody').animate({
                            'left': state == 'hidden' ? `-${$('.tagsBody span').css('width')}` : '0px'
                        });
                    } else if (state == 'unlocked') {
                        $changeTags.data('state', 'unlocked');
                        $changeTags
                            .attr('title', '锁定导航标签')
                            .empty()
                            .html('<svg width="50" height="50" fill="#1296db"><use href="#unlocked"></use></svg>');
                    } else if (state == 'locked') {
                        $changeTags.data('state', 'locked');
                        $changeTags
                            .attr('title', '解锁导航标签')
                            .empty()
                            .html('<svg width="50" height="50" fill="#d81e06"><use href="#locked"></use></svg>');
                    }
                };

                $('.tagsBody').animate({
                    'left': `-${$('.tagsBody span').css('width')}`
                });

                var $changeTags = $('.changeTags');
                if ($changeTags.length == 0)
                    $changeTags = $('<i title="显示导航标签" class="changeTags"><svg t="1623299549301" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="3903" width="50" height="50"><path d="M838.101333 552.96l-239.061333 277.461333c-23.893333 27.733333-65.194667 29.866667-91.306667 8.533334a63.445333 63.445333 0 0 1-8.661333-89.6l204.288-236.928-204.288-236.885334c-23.893333-27.733333-19.541333-68.266667 8.704-89.6 28.245333-23.509333 69.546667-19.242667 91.306667 8.533334l239.018666 277.418666c10.88 10.666667 15.232 25.6 15.232 40.533334s-4.352 29.866667-15.232 40.533333z" fill="#8a8a8a" p-id="3904"></path><path d="M496.768 552.96L257.706667 830.464c-23.893333 27.733333-65.194667 29.866667-91.306667 8.533333a63.445333 63.445333 0 0 1-8.661333-89.6l204.288-236.928-204.288-236.885333c-23.893333-27.733333-19.541333-68.266667 8.704-89.6 28.245333-23.509333 69.546667-19.242667 91.306666 8.533333l239.018667 277.418667c10.88 10.666667 15.232 25.6 15.232 40.533333s-4.352 29.866667-15.232 40.533334z" fill="#8a8a8a" p-id="3905"></path></svg></i>')
                        .data('state', 'hidden')
                        .css('left', '0px')
                        .appendTo($("body"))
                        .on('click', e => {
                            window.delayedEvent(changeTags, $changeTags.data('state') == 'locked' ? 'unlocked' : 'locked', 100, 'change-Tags');
                        });
                else
                    $changeTags.data('state', 'unlocked');

                $("body")
                    .on('mouseover', e => {
                        if (e.offsetX < 100) {
                            if ($changeTags.data('state') == 'hidden')
                                window.delayedEvent(changeTags, 'show', 100, 'change-Tags');
                            else if ($changeTags.data('state') == 'locked')
                                window.delayedEvent(() => {
                                    $changeTags.animate({
                                        'opacity': 1
                                    });
                                }, null, 100, 'change-Tags');
                        }
                    })
                    .on('mouseout', e => {
                        if (e.offsetX > $('.tagsBody span').width() + 100) {
                            if ($changeTags.data('state') == 'unlocked')
                                window.delayedEvent(changeTags, 'hidden', 100, 'change-Tags');
                            else if ($changeTags.data('state') == 'locked')
                                window.delayedEvent(() => {
                                    $changeTags.animate({
                                        'opacity': 0.5
                                    });
                                }, null, 100, 'change-Tags');
                        }
                    });
            };

            window.domMutationObserver(
                $(".swagger-ui")[0],
                () => {
                    window.delayedEvent(generateTags, null, 150, 'generate-Tags');
                });

            window.delayedEvent(generateTags, null, 100, 'generate-Tags');

            console.info('已加载功能 => 接口标签快速导航');
        });
});