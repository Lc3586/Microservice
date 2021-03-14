﻿/*
*   swagger 新增 用户端页面跳转功能
*
*   LCTR 2021-03-14
*/
$(function () {
    var open = (name, e) => {
        window.open('/webVue/index.html');
    };

    var init = () => {
        window.addPlugIn('webSite', '用户端', open, '<svg t="1615698828391" class="icon" viewBox="0 0 1025 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="5821" width="5em" height="5em"><path d="M512.026875 0C229.23903 0 0 229.23903 0 512.026875c0 282.744095 229.23903 511.973125 512.026875 511.973125 282.769095 0 512.006875-229.22903 512.006875-511.973125C1024.035 229.23903 794.80597 0 512.026875 0zM604.673238 718.712127 427.490522 718.712127c-7.246259 0-13.125016-5.877507-13.125016-13.118766 0-7.257509 5.878757-13.130016 13.125016-13.130016l177.177716 0c7.251259 0 13.130016 5.872507 13.130016 13.130016C617.798254 712.83462 611.919497 718.712127 604.673238 718.712127zM728.574639 594.478226c0 25.123781-20.371275 45.490056-45.493806 45.490056L340.952916 639.968281c-25.122531 0-45.488806-20.366275-45.488806-45.490056L295.464111 350.776678c0-25.122531 20.366275-45.488806 45.488806-45.488806l342.134168 0c25.122531 0 45.483806 20.366275 45.488806 45.488806L728.575889 594.478226z" p-id="5822" fill="#87c38f"></path><path d="M335.70791 327.212899l352.62293 0c10.947513 0 19.823774 8.876261 19.823774 19.828774l0 17.308771-392.275479 0 0-17.308771C315.879136 336.08916 324.756646 327.212899 335.70791 327.212899L335.70791 327.212899z" p-id="5823" fill="#87c38f"></path><path d="M688.33084 618.044504 335.70791 618.044504c-10.951263 0-19.828774-8.882511-19.828774-19.830024l0-30.433787 392.275479 0 0 30.433787C708.154614 609.160744 699.278354 618.044504 688.33084 618.044504L688.33084 618.044504z" p-id="5824" fill="#87c38f"></path><path d="M688.33084 618.044504" p-id="5825" fill="#87c38f"></path><path d="M467.05682 410.239251" p-id="5826" fill="#87c38f"></path><path d="M315.879136 377.475461l0 177.178966 392.275479 0L708.154614 377.475461 315.879136 377.475461zM434.800531 511.163124l-3.998755 10.268763c-1.011251 2.597503-3.491254 4.178755-6.118757 4.178755-0.791251 0-1.596252-0.14125-2.377503-0.448751-3.378754-1.313752-5.048756-5.117506-3.731255-8.49626l4.000005-10.273763c1.317502-3.373754 5.141256-5.048756 8.49501-3.730005C434.44928 503.975615 436.123032 507.78437 434.800531 511.163124zM437.461784 492.818102c-0.786251 0-1.597502-0.13625-2.378753-0.448751-3.373754-1.313752-5.048756-5.112506-3.735005-8.48751l27.212533-69.906335c1.317502-3.378754 5.112506-5.063756 8.49626-3.736255 3.378754 1.313752 5.053756 5.117506 3.735005 8.49126l-27.207533 69.902585C442.56929 491.2306 440.088037 492.818102 437.461784 492.818102zM488.789347 485.474343l-18.612523 47.797558c-1.011251 2.587503-3.491254 4.175005-6.118757 4.175005-0.791251 0-1.596252-0.14125-2.377503-0.447501-3.378754-1.315002-5.048756-5.118756-3.731255-8.49751l18.613773-47.792558c1.308752-3.368754 5.102506-5.062506 8.49126-3.730005C488.428096 478.291834 490.103098 482.095588 488.789347 485.474343zM496.978107 464.439317l-1.782502 4.565006c-1.015001 2.597503-3.491254 4.180005-6.117507 4.180005-0.791251 0-1.597502-0.14125-2.378753-0.450001-3.378754-1.312502-5.048756-5.121256-3.730005-8.49501l1.777502-4.571256c1.318752-3.373754 5.131256-5.048756 8.50126-3.730005C496.626856 457.251808 498.291858 461.060563 496.978107 464.439317z" p-id="5827" fill="#87c38f"></path><path d="M512.026875 999.04372" p-id="5828" fill="#87c38f"></path></svg>');
    };

    init();
});