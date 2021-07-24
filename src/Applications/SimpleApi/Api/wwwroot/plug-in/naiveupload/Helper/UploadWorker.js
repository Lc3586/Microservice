var UploadWorker = (function () {
    function UploadWorker(self) {
        this.CancelTokenList = {};
        this.Self = self;
        this.Axios = self.axios;
        this.AxiosInstance = self.axios.create({});
    }
    UploadWorker.prototype.SetUpAxios = function (headers) {
        this.AxiosInstance.defaults.headers.post['Content-Type'] = 'applicatoin/octet-stream';
        if (headers != null) {
            for (var key in headers) {
                this.AxiosInstance.defaults.headers.common[key] = headers[key];
            }
        }
    };
    UploadWorker.prototype.SingleFile = function (md5, buffer, type, extension, filename) {
        var _this = this;
        this.AxiosInstance.post(ApiUri.SingleFileByArrayBuffer(type, extension, filename), buffer, {
            onUploadProgress: function (progress) {
                _this.Self.postMessage({ Type: "file-upload-progress", MD5: md5, Progress: { loaded: progress.loaded, total: progress.total } });
            },
            cancelToken: new this.Axios.CancelToken(function (cancelToken) {
                _this.CancelTokenList[md5] = cancelToken;
            })
        })
            .then(function (response) {
            delete _this.CancelTokenList[md5];
            if (response.data.Success)
                _this.Self.postMessage({ Type: "file-upload-success", MD5: md5, FileInfo: response.data.Data });
            else
                _this.Self.postMessage({ Type: "file-upload-fail", MD5: md5, Message: response.data.Message });
        })
            .catch(function (e) {
            delete _this.CancelTokenList[md5];
            console.error(e);
            _this.Self.postMessage({ Type: "file-upload-error", MD5: md5, Message: e.message });
        });
    };
    UploadWorker.prototype.SingleChunkFile = function (key, md5, buffer) {
        var _this = this;
        this.AxiosInstance.post(ApiUri.UploadSingleChunkfileByArrayBuffer(key, md5), buffer, {
            onUploadProgress: function (progress) {
                _this.Self.postMessage({ Type: "file-upload-progress", MD5: md5, Progress: { loaded: progress.loaded, total: progress.total } });
            },
            cancelToken: new this.Axios.CancelToken(function (cancelToken) {
                _this.CancelTokenList[md5] = cancelToken;
            })
        })
            .then(function (response) {
            delete _this.CancelTokenList[md5];
            if (response.data.Success)
                _this.Self.postMessage({ Type: "file-upload-success", MD5: md5 });
            else
                _this.Self.postMessage({ Type: "file-upload-fail", MD5: md5, Message: response.data.Message });
        })
            .catch(function (e) {
            delete _this.CancelTokenList[md5];
            console.error(e);
            _this.Self.postMessage({ Type: "file-upload-error", MD5: md5, Message: e.message });
        });
    };
    UploadWorker.prototype.Cancel = function (md5) {
        if (md5 in this.CancelTokenList)
            this.CancelTokenList[md5]();
        this.Self.postMessage({ Type: "file-upload-cancel", MD5: md5 });
    };
    UploadWorker.prototype.OnMessage = function (event) {
        if (event.data === null) {
            this.Self.close();
            return;
        }
        var data;
        switch (event.data.Type) {
            case "SetUpAxios":
                data = event.data.Data;
                this.SetUpAxios(data.Headers);
                this.Self.postMessage({ Type: "setup-success" });
                break;
            case "File":
                data = event.data.Data;
                this.SingleFile(data.MD5, data.Buffer, data.Type, data.Extension, data.Name);
                break;
            case "ChunkFile":
                data = event.data.Data;
                this.SingleChunkFile(data.Key, data.MD5, data.Buffer);
                break;
            case "Cancel":
                data = event.data.Data;
                this.Cancel(data.MD5);
                break;
            default:
        }
    };
    return UploadWorker;
}());
if ('undefined' === typeof window
    && 'function' === typeof self.importScripts) {
    self.importScripts("../../../utils/axios.min.js", "../Model/ApiUri.js");
    self.addEventListener('message', this.onMessage);
    var worker_1 = new UploadWorker(self);
    self.addEventListener('message', function (event) { worker_1.OnMessage(event); });
}
//# sourceMappingURL=UploadWorker.js.map