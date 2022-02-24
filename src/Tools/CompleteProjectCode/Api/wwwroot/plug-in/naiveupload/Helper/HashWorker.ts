/**
 * 哈希值计算类
 * */
class HasWorker {
    constructor(self: any) {
        this.Self = self;
        this.Spark = new self.SparkMD5.ArrayBuffer();
    }

    /**
     * 当前Worker对象
     */
    Self: any;

    /**
     * 计算器
     */
    Spark: any;

    /**
     * 接收数据
     * @param event
     */
    OnMessage(this: HasWorker, event: MessageEvent<{ buffer?: ArrayBuffer, end?: boolean, close?: boolean }>) {
        // do some work here 
        let data = event.data;

        if ('undefined' !== typeof data.buffer)
            this.Spark.append(data.buffer);

        if ('undefined' !== typeof data.end) {
            if (data.end) {
                this.Self.postMessage(<string>(this.Spark.end()));
                this.Spark.reset();
            } else
                this.Self.postMessage(null);
        }

        if ('undefined' !== typeof data.close) {
            this.Spark.destroy();
            this.Self.close();
        }
    }
}

// proper initialization
if ('undefined' === typeof window && 'function' === typeof (<any>self).importScripts) {
    (<any>self).importScripts("../../../utils/spark-md5.min.js");

    let worker = new HasWorker(self);

    (<any>self).addEventListener('message', (event: MessageEvent<{ buffer?: ArrayBuffer, end?: boolean, close?: boolean }>) => { worker.OnMessage(event); });
}