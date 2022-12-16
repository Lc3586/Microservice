/**
 * 导入帮助类
 * */
class ImportHelper {
    /**
     * 导入文件(js/css)
     * @param files 文件
     * @param done 回调
     */
    static ImportFile(files: ImportFile[], done: () => void) {
        let index = 0;

        const handler = () => {
            if (files.length == index) {
                done && done();
                return;
            }

            const next = () => {
                index++;
                handler();
            };

            const file = files[index];

            const s = window.document.createElement(file.Tag);
            for (const key in file.Attributes) {
                s[key] = file.Attributes[key];
            }

            s.onload = () => {
                next();
            };

            const h = document.getElementsByTagName("head");
            if (h && h[0]) { h[0].appendChild(s); }
        };

        handler();
    }
}

/**
 * 导入文件
 * */
class ImportFile {
    /**
     * 标签
     * */
    Tag: ImportFileTag;

    /**
     * 属性
     */
    Attributes: Record<string, string> = {};
}

/**
 * 文件标签
 * */
const enum ImportFileTag {
    JS = 'script',
    CSS = 'link'
}