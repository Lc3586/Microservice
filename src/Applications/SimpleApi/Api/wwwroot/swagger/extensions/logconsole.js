﻿/*
*   swagger 新增 日志控制台功能
*
*   LCTR 2021-03-12
*/
$(function () {
    var open = (name, e) => {
        console.info(name);

        window.showDialog(
            '日志控制台',
            [['iframe', '/plug-in/logconsole/logconsole.html']],
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
                }
            }, true, false, true);
    };

    var init = () => {
        window.addPlugIn('logconsole', '日志控制台', open, '<svg style="width: 5em; height: 5em; vertical-align: middle; background-color: #dfdfdf; border-radius: 39px;" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="8291"><path d="M196.6 188h-35.1v-35c0-6.5-5.2-11.7-11.7-11.7-6.5 0-11.7 5.2-11.7 11.7v35.1h-35c-6.5 0-11.7 5.2-11.7 11.7 0 6.5 5.2 11.7 11.7 11.7h35v35c0 6.5 5.2 11.7 11.7 11.7 6.5 0 11.7-5.2 11.7-11.7v-35h35c6.5 0 11.7-5.2 11.7-11.7S203 188 196.6 188z" fill="#EF4870" p-id="8292"></path><path d="M91 330c-5.3-5.3-14-5.3-19.3 0l-4.1 4.1-4.1-4.1c-5.3-5.3-14-5.3-19.3 0-5.3 5.3-5.3 14 0 19.3L58 363.1c5.3 5.3 14 5.3 19.3 0L91 349.3c5.3-5.3 5.3-14 0-19.3z" fill="#EF4870" opacity=".47" p-id="8293"></path><path d="M743.3 784.4c-0.4-10.5-9.3-18.8-19.8-18.4l-8.2 0.3-0.3-8.2c-0.4-10.5-9.3-18.8-19.8-18.4-10.5 0.4-18.8 9.3-18.4 19.8l0.9 27.3c0.4 10.5 9.3 18.8 19.8 18.4l27.3-0.9c10.6-0.5 18.9-9.4 18.5-19.9z" fill="#EF4870" p-id="8294"></path><path d="M318.8 176.1c-16.1 0-29.2-13.1-29.2-29.2s13.1-29.2 29.2-29.2 29.2 13.1 29.2 29.2-13.1 29.2-29.2 29.2z m0-46.7c-9.7 0-17.5 7.9-17.5 17.5 0 9.7 7.9 17.5 17.5 17.5s17.5-7.9 17.5-17.5-7.9-17.5-17.5-17.5z" fill="#EF4870" opacity=".57" p-id="8295"></path><path d="M491.7 403.7c29.3-34.8 73.3-56.9 122.5-56.9 88.6 0 160.5 71.9 160.5 160.5 0 49.3-37.8 108.9-67.6 143.6-50.7 59.1-215.4 216.5-215.4 216.5S326.9 710 276.3 650.9c-29.8-34.7-67.6-94.3-67.6-143.6 0-88.6 71.9-160.5 160.5-160.5 49.1 0 93.2 22.1 122.5 56.9" fill="#EF4870" opacity=".47" p-id="8296"></path><path d="M614.2 346.8c99.5 5.1 160.5 71.9 160.5 160.5 0 49.3-37.8 108.9-67.6 143.6-50.7 59.1-237.6 233.7-248.7 233.7C536.1 740.3 869 462.8 614.2 346.8z" fill="#EF4870" p-id="8297"></path><path d="M276.3 448.2c-2.5 0-5-0.8-7.1-2.4-5.1-3.9-6.1-11.3-2.1-16.4 25.4-32.9 63.6-54 104.8-57.9 6.5-0.6 12.1 4.1 12.7 10.5 0.6 6.4-4.1 12.1-10.5 12.7-34.8 3.3-67 21.1-88.5 48.9-2.4 3.1-5.8 4.6-9.3 4.6z" fill="#FFFFFF" p-id="8298"></path><path d="M463.9 359.3c-3.3 0-6.6-1.4-8.9-4.2-33-39.2-81.4-61.7-132.8-61.7-5.2 0-26.5 2-30.8 2.7-6.3 1.2-12.4-3.1-13.5-9.5-1.1-6.3 3.1-12.4 9.5-13.5 5.7-1 28.2-3.1 34.9-3.1 58.3 0 113.2 25.5 150.6 70 4.2 4.9 3.5 12.3-1.4 16.5-2.3 1.9-4.9 2.8-7.6 2.8zM198.1 341c-3.2 0-6.4-1.3-8.7-3.9-4.3-4.8-3.9-12.2 0.9-16.5 11.9-10.7 25.2-20 39.3-27.5 5.7-3 12.8-0.9 15.8 4.8s0.9 12.8-4.8 15.8c-12.5 6.6-24.1 14.8-34.7 24.3-2.2 2-5 3-7.8 3z" p-id="8299"></path><path d="M463.9 900.2l-8.1-7.7c-7.8-7.4-191.7-183.3-250-251.3-39.2-45.7-81-115.3-81-173.7 0-30.3 6.7-59.3 19.9-86.4 2.9-6 6.2-11.9 9.7-17.6 3.4-5.5 10.6-7.2 16.1-3.8 5.5 3.4 7.2 10.6 3.8 16.1-3.1 5-6 10.2-8.6 15.5-11.6 23.8-17.5 49.5-17.5 76.2 0 52.1 40.6 118 75.4 158.5 50.7 59.2 203.5 206.5 240.3 241.9 36.9-35.4 189.6-182.7 240.3-241.9 34.8-40.6 75.4-106.4 75.4-158.5 0-95.9-78.1-174-174-174-51.3 0-99.7 22.5-132.8 61.7-4.2 4.9-11.5 5.6-16.5 1.4-4.9-4.2-5.6-11.5-1.4-16.5 37.5-44.5 92.4-70 150.6-70C714.4 270 803 358.6 803 467.4c0 58.4-41.8 128.1-81 173.7-58.4 68.1-242.2 243.9-250 251.4l-8.1 7.7z" p-id="8300"></path><path d="M518 740.7c-30 0-39.6-53.9-51.9-122-6.2-34.4-17.7-98.5-29.1-102.8-11.3 0-21.5 32.7-28.9 56.5-11.2 35.8-22.7 72.8-51.5 72.8-21.3 0-32.1-2-42.5-4-9.8-1.8-19.1-3.6-38.2-3.6-28.4 0-40-31.9-51.4-62.8-8.1-22.2-17.4-47.4-29.4-47.4-12.1 0-21.3 24.6-29.4 46.2-10.8 28.8-23 61.4-51.3 61.4-6.5 0-11.7-5.2-11.7-11.7 0-6.5 5.2-11.7 11.7-11.7 12.1 0 21.3-24.6 29.4-46.2 10.8-28.8 23-61.4 51.3-61.4 28.4 0 40 31.9 51.4 62.8 8.1 22.2 17.4 47.4 29.4 47.4 21.3 0 32.1 2 42.5 4 9.8 1.8 19.1 3.6 38.2 3.6 11.6 0 21.8-32.6 29.2-56.4 11.2-35.8 22.7-72.9 51.5-72.9 29.9 0 39.6 53.8 51.8 122 6.2 34.4 17.7 98.5 29.1 102.8 10.8 0 22.5-46.6 28.7-71.6 11.5-45.9 22.4-89.2 51.7-89.2 11.9 0 21.6-27.4 29.3-49.5 11.3-32 22.9-65.1 51.4-65.1 28 0 40.3 30.2 51.2 56.9 8.5 20.8 17.2 42.3 29.5 42.3 28.1 0 40.4 31.2 51.2 58.7 8.1 20.6 17.2 43.9 29.5 43.9 17.7 0 26-3.7 35.6-8 10.5-4.7 22.5-10 45.1-10 25.2 0 38.3 13.7 48.8 24.7 9.7 10.2 16.7 17.5 31.9 17.5 6.5 0 11.7 5.2 11.7 11.7 0 6.5-5.2 11.7-11.7 11.7-25.2 0-38.3-13.7-48.8-24.7-9.7-10.2-16.7-17.5-31.9-17.5-17.7 0-26 3.7-35.6 8-10.5 4.7-22.5 10-45.1 10-28.1 0-40.4-31.2-51.2-58.7-8.1-20.6-17.2-43.9-29.5-43.9-28 0-40.3-30.2-51.2-56.9-8.5-20.8-17.2-42.3-29.5-42.3-11.9 0-21.6 27.4-29.3 49.5-11.3 32-22.9 65.1-51.4 65.1-11.1 0-22.7 46.6-29 71.6-11.4 45.9-22.3 89.2-51.6 89.2z" p-id="8301"></path></svg>');
    };

    init();
});