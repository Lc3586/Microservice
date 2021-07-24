if (!window.importFile) {
    let importedFile = [];

    /**
     * 导入文件(js/css)
     * @param {any} files 文件
     * [
     *      {
     *          tag: 'script', 标签
     *          type: 'text/javascript', 类型
     *          src: 'http://a.b.com:8080/c' 地址
     *      },
     *      {
     *          tag: 'link', 标签
     *          type: 'text/css', 类型
     *          rel: 'stylesheet', 
     *          href: 'http://a.b.com:8080/d' 地址
     *      }
     * ]
     * @param {any} done 回调
     */
    window.importFile = (files, done) => {
        var index = 0;

        var handler = () => {
            if (files.length == index) {
                done && done();
                return;
            }

            var next = () => {
                index++;
                handler();
            };

            var file = files[index];
            if (importedFile.indexOf(file.src || file.href) >= 0) {
                next();
                return;
            }

            var s = this.document.createElement(file.tag);
            for (var item in file) {
                if (item == 'tag')
                    continue;

                s[item] = file[item];
            }
            s.onload = () => {
                importedFile.push(file.src || file.href);

                next();
            };
            var h = document.getElementsByTagName("head");
            if (h && h[0]) { h[0].appendChild(s); }
        };

        handler(index);
    };
}