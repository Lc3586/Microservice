var DateRangShortcutHelper = (function () {
    function DateRangShortcutHelper() {
    }
    DateRangShortcutHelper.后天 = function () {
        var end = new Date();
        end.setTime(end.getTime() + 3600 * 1000 * 24 * 3);
        var start = new Date();
        start.setTime(start.getTime() + 3600 * 1000 * 24 * 2);
        return [start, end];
    };
    DateRangShortcutHelper.明天 = function () {
        var end = new Date();
        end.setTime(end.getTime() + 3600 * 1000 * 24 * 2);
        var start = new Date();
        start.setTime(start.getTime() + 3600 * 1000 * 24 * 1);
        return [start, end];
    };
    DateRangShortcutHelper.今天 = function () {
        var end = new Date();
        end.setTime(end.getTime() + 3600 * 1000 * 24 * 1);
        var start = new Date();
        return [start, end];
    };
    DateRangShortcutHelper.昨天 = function () {
        var end = new Date();
        var start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 1);
        return [start, end];
    };
    ;
    DateRangShortcutHelper.最近三天 = function () {
        var end = new Date();
        var start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 3);
        return [start, end];
    };
    ;
    DateRangShortcutHelper.最近一周 = function () {
        var end = new Date();
        var start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
        return [start, end];
    };
    ;
    DateRangShortcutHelper.最近一个月 = function () {
        var end = new Date();
        var start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
        return [start, end];
    };
    ;
    DateRangShortcutHelper.最近三个月 = function () {
        var end = new Date();
        var start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
        return [start, end];
    };
    ;
    return DateRangShortcutHelper;
}());
//# sourceMappingURL=DateRangShortcutHelper.js.map