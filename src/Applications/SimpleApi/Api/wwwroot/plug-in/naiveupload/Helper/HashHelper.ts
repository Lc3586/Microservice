/**
 * 哈希值计算帮助类
 * */
class HashHelper {
    /**
     * 是否启用Worker
     * 默认启用（如果浏览器支持的话）
     */
    EnableWorker: boolean;

    /**
     * 浏览器是否支持Web Worker
     */
    WorkerSupported: boolean;

    /**
     * Spark计算单元
     */
    SparkUnits: any[] = [];

    /**
     * 子线程计算单元
     */
    WorkerUnits: Worker[] = [];

    /**
     * 文件数据读取器
     */
    FileReaders: FileReadHelper[] = [];

    /**
     * 初始化
     * @param count 计算单位数量
     * @param enableWorker 是否启用Worker
     */
    async Init(count: number = 1, enableWorker: boolean = true) {
        const promise = new Promise<void>((resolve, reject) => {
            (<any>window).importFile([{
                tag: 'script',
                type: 'text/javascript',
                src: 'Helper/FileReadHelper.js'
            }], () => {
                while (this.FileReaders.length < count) {
                    this.FileReaders.push(new FileReadHelper());
                }

                this.EnableWorker = enableWorker;
                this.WorkerSupported = 'undefined' !== typeof Worker;

                if (this.EnableWorker && this.WorkerSupported) {
                    while (this.WorkerUnits.length < count) {
                        this.WorkerUnits.push(new Worker('Helper/HashWorker.js'));
                    }

                    resolve();
                } else {
                    (<any>window).importFile([{
                        tag: 'script',
                        type: 'text/javascript',
                        src: '../../../utils/spark-md5.min.js'
                    }], () => {
                        while (this.SparkUnits.length < count) {
                            this.SparkUnits.push(new (<any>window).SparkMD5.ArrayBuffer());
                        }
                        resolve();
                    });
                }
            });
        });

        return promise;
    }

    /**
     * 返回数据的计算结果
     * @param blob 数据
     * @param index 要使用的计算单元的索引
     */
    async Calc(blob: Blob, index: number): Promise<string>;

    /**
     * 返回数据的计算结果
     * @param buffer 数据
     * @param index 要使用的计算单元的索引
     */
    async Calc(buffer: ArrayBuffer, index: number): Promise<string>;
    async Calc(data, index: number): Promise<string> {
        if (data instanceof Blob) {
            const buffer = await this.FileReaders[index].readAsArrayBuffer(data);
            await this.Append(buffer, index);
        }
        else if (data instanceof ArrayBuffer)
            await this.Append(data, index);

        return await this.End(index);
    }


    /**
     * 追加数据
     * @param blob 数据
     * @param index 要使用的计算单元的索引
     */
    async Append(blob: Blob, index: number): Promise<void>;

    /**
     * 追加数据
     * @param buffer 数据
     * @param index 要使用的计算单元的索引
     */
    async Append(buffer: ArrayBuffer, index: number): Promise<void>;
    async Append(data, index: number): Promise<void> {
        let buffer: ArrayBuffer;
        if (data instanceof Blob) {
            buffer = await this.FileReaders[index].readAsArrayBuffer(data);
        }
        else if (data instanceof ArrayBuffer)
            buffer = data;

        const promise = new Promise<void>((resolve, reject) => {
            if (this.WorkerSupported) {
                this.WorkerUnits[index].onmessage = (event: MessageEvent<string>) => {
                    resolve();
                }

                this.WorkerUnits[index].onerror = event => {
                    reject(event.error);
                }

                this.WorkerUnits[index].postMessage({ buffer: buffer, end: false }, [buffer]);
            } else {
                this.SparkUnits[index].append(buffer);
                resolve();
            }
        });

        return promise;
    }

    /**
     * 返回已有数据的计算结果
     * @param index 要使用的计算单元的索引
     */
    async End(index: number) {
        const promise = new Promise<string>((resolve, reject) => {
            if (this.WorkerSupported) {
                this.WorkerUnits[index].onmessage = (event: MessageEvent<string>) => {
                    resolve(event.data);
                }

                this.WorkerUnits[index].onerror = event => {
                    reject(event.error);
                }

                this.WorkerUnits[index].postMessage({ end: true });
            }
            else {
                const md5: string = this.SparkUnits[index].end();
                this.SparkUnits[index].reset();
                resolve(md5);
            }
        });

        return promise;
    }

    /**
     * 关闭全部计算单元
     */
    CloseAll() {
        if (this.EnableWorker && this.WorkerSupported) {
            for (let i = 0; i < this.WorkerUnits.length; i++) {
                this.Close(i);
            }
        } else {
            for (let i = 0; i < this.SparkUnits.length; i++) {
                this.Close(i);
            }
        }
    }

    /**
     * 关闭计算单元
     * @param index 计算单元的索引
     */
    Close(index: number) {
        this.FileReaders[index].Close();

        if (this.EnableWorker && this.WorkerSupported) {
            this.WorkerUnits[index].postMessage({ close: true });
        } else {
            this.SparkUnits[index].destroy();
        }
    }
}