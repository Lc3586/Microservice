var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var ChunkFileMergeTaskHelper = (function () {
    function ChunkFileMergeTaskHelper() {
    }
    ChunkFileMergeTaskHelper.prototype.Init = function () {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    ImportHelper.ImportFile([
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: ApiUri.BaseUrl + '/utils/signalr.min.js'
                            }
                        }
                    ], function () {
                        _this.SignalR = window.signalR;
                        resolve();
                    });
                });
                return [2, promise];
            });
        });
    };
    ChunkFileMergeTaskHelper.prototype.Connect = function () {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.Connection = new _this.SignalR.HubConnectionBuilder()
                        .withUrl(ApiUri.CFMTHub)
                        .withAutomaticReconnect()
                        .configureLogging(_this.SignalR.LogLevel.Information)
                        .build();
                    _this.Connection.onreconnecting(function (error) {
                        _this.State = "\u5DF2\u65AD\u5F00";
                        _this.Info = '正在尝试重新连接...';
                    });
                    _this.Connection.onreconnected(function (connectionId) {
                        console.info(connectionId);
                        _this.State = "\u5DF2\u8FDE\u63A5";
                        _this.Info = '已重新连接至服务器';
                        _this.RegisterHubMethod();
                    });
                    _this.Connection.onclose(function () {
                        _this.State = "\u5DF2\u5173\u95ED";
                        _this.Info = '连接已关闭';
                    });
                    _this.Connection.on("AddTask", function (task) {
                        if (_this.AddTask) {
                            var method = _this.AddTask(task);
                            if (method && method.then)
                                method.then();
                        }
                    });
                    _this.Connection.on("UpdateTask", function (id, data) {
                        if (_this.UpdateTask) {
                            var method = _this.UpdateTask(id, data);
                            if (method && method.then)
                                method.then();
                        }
                    });
                    _this.Connection.on("RemoveTask", function (md5) {
                        if (_this.RemoveTask) {
                            var method = _this.RemoveTask(md5);
                            if (method && method.then)
                                method.then();
                        }
                    });
                    _this.Connection.on("UpdateChunksSource", function (md5, chunksSource) {
                        if (_this.UpdateChunksSource) {
                            var method = _this.UpdateChunksSource(md5, chunksSource);
                            if (method && method.then)
                                method.then();
                        }
                    });
                    _this.Connection
                        .start()
                        .then(function () {
                        _this.State = "\u5DF2\u8FDE\u63A5";
                        _this.Info = '已连接至服务器';
                        _this.RegisterHubMethod();
                        resolve();
                    });
                });
                return [2, promise];
            });
        });
    };
    ChunkFileMergeTaskHelper.prototype.RegisterHubMethod = function () {
    };
    return ChunkFileMergeTaskHelper;
}());
//# sourceMappingURL=ChunkFileMergeTaskHelper.js.map