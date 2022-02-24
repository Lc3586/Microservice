var HasWorker = (function () {
    function HasWorker(self) {
        this.Self = self;
        this.Spark = new self.SparkMD5.ArrayBuffer();
    }
    HasWorker.prototype.OnMessage = function (event) {
        var data = event.data;
        if ('undefined' !== typeof data.buffer)
            this.Spark.append(data.buffer);
        if ('undefined' !== typeof data.end) {
            if (data.end) {
                this.Self.postMessage((this.Spark.end()));
                this.Spark.reset();
            }
            else
                this.Self.postMessage(null);
        }
        if ('undefined' !== typeof data.close) {
            this.Spark.destroy();
            this.Self.close();
        }
    };
    return HasWorker;
}());
if ('undefined' === typeof window && 'function' === typeof self.importScripts) {
    self.importScripts("../../../utils/spark-md5.min.js");
    var worker_1 = new HasWorker(self);
    self.addEventListener('message', function (event) { worker_1.OnMessage(event); });
}
//# sourceMappingURL=HashWorker.js.map