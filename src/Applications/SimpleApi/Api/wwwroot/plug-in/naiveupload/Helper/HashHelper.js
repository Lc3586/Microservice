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
var HashHelper = (function () {
    function HashHelper() {
        this.SparkUnits = [];
        this.WorkerUnits = [];
        this.FileReaders = [];
    }
    HashHelper.prototype.Init = function (count, enableWorker) {
        if (count === void 0) { count = 1; }
        if (enableWorker === void 0) { enableWorker = true; }
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
                                src: 'Helper/FileReadHelper.js'
                            }
                        }
                    ], function () {
                        while (_this.FileReaders.length < count) {
                            _this.FileReaders.push(new FileReadHelper());
                        }
                        _this.EnableWorker = enableWorker;
                        _this.WorkerSupported = 'undefined' !== typeof Worker;
                        if (_this.EnableWorker && _this.WorkerSupported) {
                            while (_this.WorkerUnits.length < count) {
                                _this.WorkerUnits.push(new Worker('Helper/HashWorker.js'));
                            }
                            resolve();
                        }
                        else {
                            ImportHelper.ImportFile([
                                {
                                    Tag: "script",
                                    Attributes: {
                                        type: 'text/javascript',
                                        src: '../../../utils/spark-md5.min.js'
                                    }
                                }
                            ], function () {
                                while (_this.SparkUnits.length < count) {
                                    _this.SparkUnits.push(new window.SparkMD5.ArrayBuffer());
                                }
                                resolve();
                            });
                        }
                    });
                });
                return [2, promise];
            });
        });
    };
    HashHelper.prototype.Calc = function (data, index) {
        return __awaiter(this, void 0, void 0, function () {
            var buffer;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(data instanceof Blob)) return [3, 3];
                        return [4, this.FileReaders[index].readAsArrayBuffer(data)];
                    case 1:
                        buffer = _a.sent();
                        return [4, this.Append(buffer, index)];
                    case 2:
                        _a.sent();
                        return [3, 5];
                    case 3:
                        if (!(data instanceof ArrayBuffer)) return [3, 5];
                        return [4, this.Append(data, index)];
                    case 4:
                        _a.sent();
                        _a.label = 5;
                    case 5: return [4, this.End(index)];
                    case 6: return [2, _a.sent()];
                }
            });
        });
    };
    HashHelper.prototype.Append = function (data, index) {
        return __awaiter(this, void 0, void 0, function () {
            var buffer, promise;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(data instanceof Blob)) return [3, 2];
                        return [4, this.FileReaders[index].readAsArrayBuffer(data)];
                    case 1:
                        buffer = _a.sent();
                        return [3, 3];
                    case 2:
                        if (data instanceof ArrayBuffer)
                            buffer = data;
                        _a.label = 3;
                    case 3:
                        promise = new Promise(function (resolve, reject) {
                            if (_this.WorkerSupported) {
                                _this.WorkerUnits[index].onmessage = function (event) {
                                    resolve();
                                };
                                _this.WorkerUnits[index].onerror = function (event) {
                                    reject(event.error);
                                };
                                _this.WorkerUnits[index].postMessage({ buffer: buffer, end: false }, [buffer]);
                            }
                            else {
                                _this.SparkUnits[index].append(buffer);
                                resolve();
                            }
                        });
                        return [2, promise];
                }
            });
        });
    };
    HashHelper.prototype.End = function (index) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    if (_this.WorkerSupported) {
                        _this.WorkerUnits[index].onmessage = function (event) {
                            resolve(event.data);
                        };
                        _this.WorkerUnits[index].onerror = function (event) {
                            reject(event.error);
                        };
                        _this.WorkerUnits[index].postMessage({ end: true });
                    }
                    else {
                        var md5 = _this.SparkUnits[index].end();
                        _this.SparkUnits[index].reset();
                        resolve(md5);
                    }
                });
                return [2, promise];
            });
        });
    };
    HashHelper.prototype.CloseAll = function () {
        if (this.EnableWorker && this.WorkerSupported) {
            for (var i = 0; i < this.WorkerUnits.length; i++) {
                this.Close(i);
            }
        }
        else {
            for (var i = 0; i < this.SparkUnits.length; i++) {
                this.Close(i);
            }
        }
    };
    HashHelper.prototype.Close = function (index) {
        this.FileReaders[index].Close();
        if (this.EnableWorker && this.WorkerSupported) {
            this.WorkerUnits[index].postMessage({ close: true });
        }
        else {
            this.SparkUnits[index].destroy();
        }
    };
    return HashHelper;
}());
//# sourceMappingURL=HashHelper.js.map