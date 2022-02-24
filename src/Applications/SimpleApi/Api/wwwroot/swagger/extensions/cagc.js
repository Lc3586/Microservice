﻿/*
*   swagger 新增 代码自动生成功能
*
*   LCTR 2021-06-25
*/
window.onInformationLoaded(() => {
    console.info('正在加载功能 => 代码自动生成');

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
                    '代码自动生成',
                    [['iframe', window.baseUri + '/plug-in/cagc/cagc.html']],
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
                                window.open(window.baseUri + '/plug-in/cagc/cagc.html');
                            }
                        }
                    }, true, false, true);
            };

            window.addPlugIn('cagc', '代码自动生成', open, '<svg t="1624595441811" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2411" width="5em" height="5em"><path d="M512.689143 1024h-9.21584c-66.046857-1.023982-131.581724-15.359734-192.50867-41.471283l-45.567212-19.455663c-6.655885-2.559956-8.191858-11.263805-3.583938-16.383717l110.078096-121.3419c16.895708-18.431681 45.567212-19.967655 64.510884-3.071947 18.431681 16.895708 19.967655 45.567212 3.071947 64.510884l-30.207477 33.279424c86.014512 22.015619 176.636944 15.871725 261.115482-17.91969 215.03628-87.038494 319.482473-332.794243 232.443979-547.830522-12.287787-31.23146-28.671504-60.414955-48.127167-87.550486-13.823761-18.943672-10.751814-45.567212 7.167876-60.926946 19.967655-17.407699 51.199114-14.335752 67.070839 7.167876C1080.48732 424.970363 1049.767852 721.925226 849.579315 897.026197a510.250373 510.250373 0 0 1-336.890172 126.973803z m-347.641985-204.284466c-19.967655 17.91969-51.199114 14.847743-67.07084-7.167876-153.597343-210.42836-124.925839-507.383222 74.238715-684.020166C319.668483-1.518259 529.07286-36.333656 710.317724 39.953024l51.199115 21.503628c7.167876 3.071947 8.703849 11.775796 3.071946 16.895708l-127.485794 115.198007c-18.431681 16.895708-47.615176 15.359734-64.510884-3.071947-16.895708-18.431681-15.359734-47.615176 3.071947-64.510884l27.647522-25.087566C376.499499 51.216829 151.735388 194.574349 101.560256 421.386425c-26.111548 118.269954 0 241.147828 70.142786 337.914154 13.823761 18.943672 10.751814 45.055221-6.655884 60.414955z" p-id="2412" fill="#26e2de"></path><path d="M263.861448 622.598944l122.877874-279.547164c12.799779-28.671504 53.247079-28.671504 65.534866 0l121.853892 279.035173c10.239823 25.087566-8.191858 52.735088-35.327388 52.735088-15.359734 0-29.183495-9.215841-34.815398-23.039602L481.457684 599.047352H356.531845l-22.52761 52.735087c-6.143894 14.335752-19.967655 23.039601-34.815398 23.039602-27.647522 0.511991-45.567212-27.135531-35.327389-52.223097zM409.778924 448.009965l-31.743451 77.822653c-2.559956 6.655885 2.047965 14.335752 9.727832 14.335752H450.738215c7.167876 0 12.287787-7.167876 9.215841-14.335752l-31.743451-77.822653c-3.071947-8.191858-14.847743-8.191858-18.431681 0z m221.692164 191.996678V380.427134c0-19.455663 15.871725-35.327389 35.327389-35.327389 19.455663 0 35.327389 15.871725 35.327389 35.327389V640.006643c0 19.455663-15.871725 35.327389-35.327389 35.327389-19.455663 0-35.327389-15.871725-35.327389-35.327389z" p-id="2413" fill="#26e2de"></path></svg>', (name, e) => { window.open(window.baseUri + '/plug-in/cagc/cagc.html'); });

            console.info('已加载功能 => 代码自动生成');
        });
});