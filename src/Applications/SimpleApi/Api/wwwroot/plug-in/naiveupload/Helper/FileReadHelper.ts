/**
 * 文件读取帮助类
 * */
class FileReadHelper {
    constructor() {
        if ('undefined' === typeof FileReader) {
            throw new Error('当前浏览器不支持FileReader.');
        }
        this.Reader = new FileReader();
    }

    /**
     * 读取器
     */
    Reader: FileReader;

    /**
     * 读取数据
     * @param reader
     * @param blob
     */
    async readAsArrayBuffer(blob: Blob): Promise<ArrayBuffer> {
        const promise = new Promise<ArrayBuffer>((resolve, reject) => {
            this.Reader.onload = event => {
                const buffer = event.target.result as ArrayBuffer;
                resolve(buffer);
            }

            this.Reader.onerror = event => {
                reject(event);
            }

            this.Reader.readAsArrayBuffer(blob);
        });

        return promise;
    }

    /**
     * 关闭
     * */
    Close() {
        this.Reader.abort();
    }
}