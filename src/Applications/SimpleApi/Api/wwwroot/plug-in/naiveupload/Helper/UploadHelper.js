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
var UploadHelper = (function () {
    function UploadHelper() {
        this.CancelTokenList = {};
        this.WorkerUnits = [];
        this.ChunkHandlerQueue = [];
        this.ChunkHandler = [];
        this.DelayTimes = 0;
        this.Finished = false;
        this.FileReaders = [];
    }
    UploadHelper.prototype.Init = function (axios, headers, concurrent, enableWorker) {
        if (axios === void 0) { axios = null; }
        if (headers === void 0) { headers = null; }
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
                                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Helper/FileReadHelper.js'
                            }
                        }
                    ], function () {
                        var setup = function () { return __awaiter(_this, void 0, void 0, function () {
                            var key, i, worker, e_1;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        if (headers != null) {
                                            for (key in headers) {
                                                this.AxiosInstance.defaults.headers.common[key] = headers[key];
                                            }
                                        }
                                        this.EnableWorker = enableWorker;
                                        this.WorkerSupported = 'undefined' !== typeof Worker;
                                        if (!(this.EnableWorker && this.WorkerSupported)) return [3, 7];
                                        i = 0;
                                        _a.label = 1;
                                    case 1:
                                        if (!(i < concurrent)) return [3, 6];
                                        worker = new Worker('Helper/UploadWorker.js');
                                        this.WorkerUnits.push({ worker: worker, used: false });
                                        _a.label = 2;
                                    case 2:
                                        _a.trys.push([2, 4, , 5]);
                                        return [4, this.WorkerPostMessage(i, { Type: "SetUpAxios", Data: { Headers: headers } })];
                                    case 3:
                                        _a.sent();
                                        return [3, 5];
                                    case 4:
                                        e_1 = _a.sent();
                                        reject(e_1);
                                        return [2];
                                    case 5:
                                        i++;
                                        return [3, 1];
                                    case 6:
                                        resolve();
                                        return [3, 8];
                                    case 7:
                                        resolve();
                                        _a.label = 8;
                                    case 8: return [2];
                                }
                            });
                        }); };
                        while (_this.FileReaders.length < concurrent) {
                            _this.FileReaders.push(new FileReadHelper());
                            _this.ChunkHandler.push(false);
                        }
                        if (axios != null) {
                            _this.Axios = axios;
                            _this.AxiosInstance = _this.Axios.create({});
                            setup();
                        }
                        else {
                            ImportHelper.ImportFile([
                                {
                                    Tag: "script",
                                    Attributes: {
                                        type: 'text/javascript',
                                        src: ApiUri.BaseUrl + '/utils/axios.min.js'
                                    }
                                }
                            ], function () {
                                _this.Axios = new window.axios;
                                _this.AxiosInstance = _this.Axios.create({});
                                setup();
                            });
                        }
                    });
                });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UseAxiosUploadFile = function (file, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var formData;
                    var _this = this;
                    return __generator(this, function (_a) {
                        formData = new FormData();
                        formData.append('file', file.File);
                        this.AxiosInstance.post(ApiUri.SingleFile(file.ConfigId, file.Name), formData, {
                            onUploadProgress: function (progress) {
                                onProgress({ PreLoaded: progress.total, Loaded: progress.loaded, Total: progress.total });
                            },
                            cancelToken: new this.Axios.CancelToken(function (cancelToken) {
                                _this.CancelTokenList[file.MD5] = cancelToken;
                            })
                        })
                            .then(function (response) {
                            delete _this.CancelTokenList[file.MD5];
                            if (response.data.Success) {
                                file.FileInfo = response.data.Data;
                                resolve();
                            }
                            else
                                reject(new Error(response.data.Message));
                        })
                            .catch(function (e) {
                            delete _this.CancelTokenList[file.MD5];
                            reject(e);
                        });
                        return [2];
                    });
                }); });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UseAxiosUploadChunkFile = function (handlerIndex, validation, chunk, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var formData;
                    var _this = this;
                    return __generator(this, function (_a) {
                        formData = new FormData();
                        formData.append('file', chunk.Blob);
                        this.AxiosInstance.post(ApiUri.UploadSingleChunkfile(validation.Key, chunk.MD5), formData, {
                            onUploadProgress: function (progress) {
                                onProgress({ PreLoaded: progress.total, Loaded: progress.loaded, Total: progress.total });
                            },
                            cancelToken: new this.Axios.CancelToken(function (cancelToken) {
                                _this.CancelTokenList[chunk.MD5] = cancelToken;
                            })
                        })
                            .then(function (response) {
                            delete _this.CancelTokenList[chunk.MD5];
                            if (response.data.Success)
                                resolve();
                            else
                                reject(new Error(response.data.Message));
                        })
                            .catch(function (e) {
                            delete _this.CancelTokenList[chunk.MD5];
                            reject(e);
                        });
                        return [2];
                    });
                }); });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UseWorkerUploadFile = function (file, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var buffer, e_2, fileInfo, e_3;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                _a.trys.push([0, 2, , 3]);
                                return [4, this.FileReaders[0].readAsArrayBuffer(file.File)];
                            case 1:
                                buffer = _a.sent();
                                return [3, 3];
                            case 2:
                                e_2 = _a.sent();
                                reject(e_2);
                                return [2];
                            case 3:
                                _a.trys.push([3, 5, , 6]);
                                return [4, this.WorkerPostMessage(0, { Type: "File", Data: { MD5: file.MD5, Buffer: buffer, Type: file.File.type, Extension: file.Extension, Name: file.Name, ConfigId: file.ConfigId } }, onProgress)];
                            case 4:
                                fileInfo = _a.sent();
                                file.FileInfo = fileInfo;
                                return [3, 6];
                            case 5:
                                e_3 = _a.sent();
                                reject(e_3);
                                return [2];
                            case 6:
                                resolve();
                                return [2];
                        }
                    });
                }); });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UseWorkerUploadChunkFile = function (handlerIndex, validation, chunk, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var buffer, e_4, e_5;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                _a.trys.push([0, 2, , 3]);
                                return [4, this.FileReaders[handlerIndex].readAsArrayBuffer(chunk.Blob)];
                            case 1:
                                buffer = _a.sent();
                                return [3, 3];
                            case 2:
                                e_4 = _a.sent();
                                reject(e_4);
                                return [2];
                            case 3:
                                _a.trys.push([3, 5, , 6]);
                                return [4, this.WorkerPostMessage(handlerIndex, { Type: "ChunkFile", Data: { Key: validation.Key, MD5: chunk.MD5, Buffer: buffer } }, onProgress)];
                            case 4:
                                _a.sent();
                                return [3, 6];
                            case 5:
                                e_5 = _a.sent();
                                reject(e_5);
                                return [2];
                            case 6:
                                resolve();
                                return [2];
                        }
                    });
                }); });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.WorkerPostMessage = function (handlerIndex, data, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var unit, promise;
            return __generator(this, function (_a) {
                unit = this.WorkerUnits[handlerIndex];
                unit.used = true;
                promise = new Promise(function (resolve, reject) {
                    unit.worker.onmessage = function (event) {
                        switch (event.data.Type) {
                            case "setup-success":
                                resolve();
                                break;
                            case "setup-fail":
                            case "setup-error":
                                reject(new Error(event.data.Message));
                                break;
                            case "file-upload-progress":
                                onProgress({ PreLoaded: event.data.Progress.total, Loaded: event.data.Progress.loaded, Total: event.data.Progress.total });
                                break;
                            case "file-upload-success":
                                if ('undefined' !== typeof event.data.FileInfo) {
                                    resolve(event.data.FileInfo);
                                }
                                else
                                    resolve();
                                break;
                            case "file-upload-fail":
                            case "file-upload-error":
                                reject(new Error(event.data.Message));
                                break;
                            case "file-upload-cancel":
                                reject();
                                break;
                            default:
                                reject("\u672A\u77E5\u7684UploadWorkerNoticeMessage.Type: " + event.data.Type + ".");
                        }
                        unit.used = false;
                    };
                    unit.worker.onerror = function (event) {
                        reject(event.error);
                        unit.used = false;
                    };
                    if (data == null || (data.Type !== "File" && data.Type !== "ChunkFile"))
                        unit.worker.postMessage(data);
                    else
                        unit.worker.postMessage(data, [data.Data.Buffer]);
                });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UploadFile = function (file, onProgress) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var validation, e_6, progress_1, upload_1, push, _i, _a, chunk, e_7;
                    var _this = this;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                _b.trys.push([0, 5, , 6]);
                                if (!file.NeedSection) return [3, 2];
                                return [4, this.PreUploadFile(file.ConfigId, file.MD5, file.Name, true, file.File.type, file.Extension, file.Specs, file.Chunks.length)];
                            case 1:
                                validation = _b.sent();
                                return [3, 4];
                            case 2: return [4, this.PreUploadFile(file.ConfigId, file.MD5, file.Name, false, file.File.type, file.Extension)];
                            case 3:
                                validation = _b.sent();
                                _b.label = 4;
                            case 4: return [3, 6];
                            case 5:
                                e_6 = _b.sent();
                                reject(e_6);
                                return [2];
                            case 6:
                                if (validation.Uploaded) {
                                    file.FileInfo = validation.FileInfo;
                                    resolve();
                                    return [2];
                                }
                                if (!file.NeedSection) return [3, 7];
                                progress_1 = { PreLoaded: 0, Loaded: 0, Total: file.Size };
                                upload_1 = function () { return __awaiter(_this, void 0, void 0, function () {
                                    var i, _a, e_8, handlerIndex, i, next, chunk, chunk_progress, handlerProgress, validation, e_9, proceed, _b;
                                    var _this = this;
                                    return __generator(this, function (_c) {
                                        switch (_c.label) {
                                            case 0:
                                                if (this.Finished) {
                                                    return [2];
                                                }
                                                if (!(this.ChunkHandlerQueue.length === 0)) return [3, 5];
                                                for (i = 0; i < this.ChunkHandler.length; i++) {
                                                    if (this.ChunkHandler[i]) {
                                                        return [2];
                                                    }
                                                }
                                                _c.label = 1;
                                            case 1:
                                                _c.trys.push([1, 3, , 4]);
                                                _a = file;
                                                return [4, this.UploadChunkFileFinished(file.MD5, file.Specs, file.Chunks.length, file.File.type, file.Extension, file.Name)];
                                            case 2:
                                                _a.FileInfo = _c.sent();
                                                this.Finished = true;
                                                resolve();
                                                return [3, 4];
                                            case 3:
                                                e_8 = _c.sent();
                                                reject(e_8);
                                                return [3, 4];
                                            case 4: return [2];
                                            case 5:
                                                handlerIndex = null;
                                                for (i = 0; i < this.ChunkHandler.length; i++) {
                                                    if (!this.ChunkHandler[i]) {
                                                        handlerIndex = i;
                                                        break;
                                                    }
                                                }
                                                if (handlerIndex == null)
                                                    return [2];
                                                this.ChunkHandler[handlerIndex] = true;
                                                next = function () {
                                                    onProgress(progress_1);
                                                    _this.ChunkHandler[handlerIndex] = false;
                                                    upload_1();
                                                };
                                                chunk = this.ChunkHandlerQueue.shift();
                                                chunk_progress = { PreLoaded: chunk.Size, Loaded: 0, Total: chunk.Size };
                                                handlerProgress = function (_chunk_progress) {
                                                    progress_1.Loaded += (_chunk_progress.Loaded - chunk_progress.Loaded);
                                                    chunk_progress.Loaded = _chunk_progress.Loaded;
                                                    onProgress(progress_1);
                                                };
                                                _c.label = 6;
                                            case 6:
                                                _c.trys.push([6, 8, , 9]);
                                                return [4, this.PreUploadChunkfile(file.MD5, chunk.MD5, chunk.Index, file.Specs, chunk.Forced)];
                                            case 7:
                                                validation = _c.sent();
                                                return [3, 9];
                                            case 8:
                                                e_9 = _c.sent();
                                                reject(e_9);
                                                return [2];
                                            case 9:
                                                progress_1.PreLoaded += chunk_progress.PreLoaded;
                                                onProgress(progress_1);
                                                proceed = function () { return __awaiter(_this, void 0, void 0, function () {
                                                    return __generator(this, function (_a) {
                                                        switch (_a.label) {
                                                            case 0:
                                                                if (!this.WorkerSupported) return [3, 2];
                                                                return [4, this.UseWorkerUploadChunkFile(handlerIndex, validation, chunk, handlerProgress)];
                                                            case 1:
                                                                _a.sent();
                                                                return [3, 4];
                                                            case 2: return [4, this.UseAxiosUploadChunkFile(handlerIndex, validation, chunk, handlerProgress)];
                                                            case 3:
                                                                _a.sent();
                                                                _a.label = 4;
                                                            case 4: return [2];
                                                        }
                                                    });
                                                }); };
                                                _b = validation.State;
                                                switch (_b) {
                                                    case "\u5141\u8BB8\u4E0A\u4F20": return [3, 10];
                                                    case "\u5168\u90E8\u8DF3\u8FC7": return [3, 12];
                                                    case "\u63A8\u8FDF\u4E0A\u4F20": return [3, 13];
                                                    case "\u8DF3\u8FC7": return [3, 14];
                                                }
                                                return [3, 15];
                                            case 10: return [4, proceed()];
                                            case 11:
                                                _c.sent();
                                                return [3, 15];
                                            case 12:
                                                this.ChunkHandlerQueue.length = 0;
                                                return [2];
                                            case 13:
                                                this.ChunkHandlerQueue.push(chunk);
                                                this.DelayTimes++;
                                                if (this.DelayTimes >= this.ChunkHandlerQueue.length) {
                                                    this.DelayTimes = 0;
                                                    chunk.Forced = true;
                                                }
                                                return [3, 15];
                                            case 14:
                                                handlerProgress({ Loaded: chunk.Size, Total: chunk.Size });
                                                return [3, 15];
                                            case 15:
                                                next();
                                                return [2];
                                        }
                                    });
                                }); };
                                push = function (chunk) {
                                    _this.ChunkHandlerQueue.push(chunk);
                                    upload_1();
                                };
                                for (_i = 0, _a = file.Chunks; _i < _a.length; _i++) {
                                    chunk = _a[_i];
                                    push(chunk);
                                }
                                return [3, 13];
                            case 7:
                                _b.trys.push([7, 12, , 13]);
                                if (!this.WorkerSupported) return [3, 9];
                                return [4, this.UseWorkerUploadFile(file, onProgress)];
                            case 8:
                                _b.sent();
                                return [3, 11];
                            case 9: return [4, this.UseAxiosUploadFile(file, onProgress)];
                            case 10:
                                _b.sent();
                                _b.label = 11;
                            case 11:
                                resolve();
                                return [3, 13];
                            case 12:
                                e_7 = _b.sent();
                                reject(e_7);
                                return [3, 13];
                            case 13: return [2];
                        }
                    });
                }); });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.PreUploadFile = function (configId, md5, name, section, type, extension, specs, total) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.AxiosInstance.get(ApiUri.PreUploadFile(configId, md5, name, section, type, extension, specs, total))
                        .then(function (response) {
                        if (response.data.Success) {
                            resolve(response.data.Data);
                        }
                        else
                            reject(new Error(response.data.Message));
                    })
                        .catch(function (e) {
                        reject(e);
                    });
                });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.PreUploadChunkfile = function (file_md5, md5, index, specs, forced) {
        if (forced === void 0) { forced = false; }
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.AxiosInstance.get(ApiUri.PreUploadChunkfile(file_md5, md5, index, specs, forced))
                        .then(function (response) {
                        if (response.data.Success) {
                            resolve(response.data.Data);
                        }
                        else
                            reject(new Error(response.data.Message));
                    })
                        .catch(function (e) {
                        reject(e);
                    });
                });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.UploadChunkFileFinished = function (file_md5, specs, total, type, extension, filename) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.AxiosInstance.get(ApiUri.UploadChunkfileFinished(file_md5, specs, total, type, extension, filename))
                        .then(function (response) {
                        if (response.data.Success) {
                            resolve(response.data.Data);
                        }
                        else
                            reject(new Error(response.data.Message));
                    })
                        .catch(function (e) {
                        reject(e);
                    });
                });
                return [2, promise];
            });
        });
    };
    UploadHelper.prototype.CloseAll = function () {
        if (this.EnableWorker && this.WorkerSupported) {
            for (var i = 0; i < this.WorkerUnits.length; i++) {
                this.Close(i);
            }
        }
        else {
            for (var i = 0; i < this.FileReaders.length; i++) {
                this.Close(i);
            }
        }
    };
    UploadHelper.prototype.Close = function (index) {
        this.FileReaders[index].Close();
        if (this.EnableWorker && this.WorkerSupported) {
            this.WorkerPostMessage(index, null);
        }
    };
    return UploadHelper;
}());
//# sourceMappingURL=UploadHelper.js.map