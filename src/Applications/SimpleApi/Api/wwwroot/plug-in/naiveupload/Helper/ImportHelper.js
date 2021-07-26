var ImportHelper = (function () {
    function ImportHelper() {
    }
    ImportHelper.ImportFile = function (files, done) {
        var index = 0;
        var handler = function () {
            if (files.length == index) {
                done && done();
                return;
            }
            var next = function () {
                index++;
                handler();
            };
            var file = files[index];
            var s = window.document.createElement(file.Tag);
            for (var key in file.Attributes) {
                s[key] = file.Attributes[key];
            }
            s.onload = function () {
                next();
            };
            var h = document.getElementsByTagName("head");
            if (h && h[0]) {
                h[0].appendChild(s);
            }
        };
        handler();
    };
    return ImportHelper;
}());
var ImportFile = (function () {
    function ImportFile() {
        this.Attributes = {};
    }
    return ImportFile;
}());
//# sourceMappingURL=ImportHelper.js.map