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
var NaiveUpload = (function () {
    function NaiveUpload() {
        this.RawFileList = [];
        this.SelectedFileList = [];
        this.CheckQueue = [];
        this.CheckHandlerCount = 0;
        this.UploadQueue = [];
        this.UploadHandlerCount = 0;
    }
    NaiveUpload.prototype.Init = function (settings) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.Settings = settings;
                    var files = [
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Model/SelectedFile.js'
                            }
                        },
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Model/RawFile.js'
                            }
                        },
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Model/ChunkFile.js'
                            }
                        },
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Model/ApiUri.js'
                            }
                        },
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Helper/HashHelper.js'
                            }
                        },
                        {
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: 'Helper/UploadHelper.js'
                            }
                        }
                    ];
                    if (_this.Settings.Axios == null)
                        files.push({
                            Tag: "script",
                            Attributes: {
                                type: 'text/javascript',
                                src: '../../utils/axios.min.js'
                            }
                        });
                    ImportHelper.ImportFile(files, function () {
                        if (_this.Settings.Axios == null)
                            _this.Settings.Axios = new window.axios;
                        _this.CreateAxiosInstance();
                        resolve();
                    });
                });
                return [2, promise];
            });
        });
    };
    NaiveUpload.prototype.CreateAxiosInstance = function () {
        this.AxiosInstance = this.Settings.Axios.create({});
        for (var key in this.Settings) {
            this.AxiosInstance.defaults.headers.common[key] = this.Settings[key];
        }
    };
    NaiveUpload.prototype.Pause = function (index) {
    };
    NaiveUpload.prototype.Continues = function (index) {
    };
    NaiveUpload.prototype.Remove = function (index) {
        this.SelectedFileList[index].Canceled = true;
    };
    NaiveUpload.prototype.Clean = function () {
        this.SelectedFileList.length = 0;
    };
    NaiveUpload.prototype.AppendFiles = function (files) {
        var _this = this;
        if (!!this.Settings.Limit && (this.SelectedFileList.length + files.length) > this.Settings.Limit)
            return 1;
        var push = function (file, newFile) {
            var rawFile = new RawFile(file);
            rawFile.Name = newFile.Name;
            rawFile.Extension = newFile.Extension;
            _this.RawFileList.push(rawFile);
            newFile.RawIndex = _this.RawFileList.length - 1;
            _this.SelectedFileList.push(newFile);
            _this.HandleFile(_this.SelectedFileList.length - 1);
        };
        var fileList = Array.prototype.slice.call(files);
        var _loop_1 = function (file) {
            if (this_1.Limited())
                return { value: void 0 };
            var selectedFile = new SelectedFile(file);
            if (this_1.Settings.BeforeCheck) {
                var before = this_1.Settings.BeforeCheck(file);
                if (before && before.then) {
                    before.then(function (flag) {
                        if (flag)
                            push(file, selectedFile);
                    });
                }
            }
            else
                push(file, selectedFile);
        };
        var this_1 = this;
        for (var _i = 0, fileList_1 = fileList; _i < fileList_1.length; _i++) {
            var file = fileList_1[_i];
            var state_1 = _loop_1(file);
            if (typeof state_1 === "object")
                return state_1.value;
        }
    };
    NaiveUpload.prototype.HandleFile = function (selectedFileIndex) {
        var _this = this;
        var selectedFile = this.SelectedFileList[selectedFileIndex];
        selectedFile.Thumbnail = ApiUri.FileTypeImageUrl(selectedFile.ExtensionLower);
        this.GetFileType(selectedFile, function () {
            _this.CheckImage(selectedFile);
        });
        this.GetFileSize(selectedFile);
        if (this.Settings.EnableChunk)
            this.GetChunks(selectedFile);
        this.PushToCheckQueue(selectedFileIndex);
    };
    NaiveUpload.prototype.PushToCheckQueue = function (selectedFileIndex) {
        this.CheckQueue.push(selectedFileIndex);
        if (this.Settings.Mode == "AT" || this.Settings.Mode == "AMT")
            this.CheckMD5();
    };
    NaiveUpload.prototype.PushToUploadQueue = function (selectedFileIndex) {
        this.UploadQueue.push(selectedFileIndex);
        if (this.Settings.Mode == "AT")
            this.Upload();
    };
    NaiveUpload.prototype.CheckMD5 = function () {
        return __awaiter(this, void 0, void 0, function () {
            var after, next, selectedFileIndex, selectedFile, rawFile, hashHelper, close, cancel, calc, handlerData, _i, _a, chunk, appendDataPromise, md5Promise, md5_1, bufferSize, count, blob, md5;
            var _this = this;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.CheckHandlerCount >= this.Settings.ConcurrentFile || this.CheckQueue.length === 0) {
                            if (this.CheckHandlerCount == 0 && this.CheckQueue.length === 0 && this.Settings.AfterCheckAll) {
                                after = this.Settings.AfterCheckAll(this.RawFileList);
                                if (after && after.then) {
                                    return [2];
                                }
                            }
                            else
                                return [2];
                        }
                        this.CheckHandlerCount++;
                        next = function () {
                            _this.CheckHandlerCount--;
                            if (_this.Settings.AfterCheck) {
                                var after = _this.Settings.AfterCheck(rawFile);
                                if (after && after.then) {
                                    after.then(function () { _this.CheckMD5(); });
                                }
                            }
                            else
                                _this.CheckMD5();
                        };
                        selectedFileIndex = this.CheckQueue.shift();
                        selectedFile = this.SelectedFileList[selectedFileIndex];
                        if (selectedFile.Canceled) {
                            next();
                            return [2];
                        }
                        selectedFile.Checking = true;
                        rawFile = this.GetRawFile(selectedFile);
                        hashHelper = new HashHelper();
                        return [4, hashHelper.Init(rawFile.NeedSection ? 2 : 1, this.Settings.EnableWorker)];
                    case 1:
                        _b.sent();
                        close = function () {
                            hashHelper.CloseAll();
                        };
                        cancel = function () {
                            close();
                            next();
                        };
                        calc = function (end, blob, unitIndex) {
                            if (unitIndex === void 0) { unitIndex = 0; }
                            return __awaiter(_this, void 0, void 0, function () {
                                var e_1;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            _a.trys.push([0, 8, , 9]);
                                            if (!end) return [3, 5];
                                            if (!('undefined' === typeof blob)) return [3, 2];
                                            return [4, hashHelper.End(unitIndex)];
                                        case 1: return [2, _a.sent()];
                                        case 2: return [4, hashHelper.Calc(blob, unitIndex)];
                                        case 3: return [2, _a.sent()];
                                        case 4: return [3, 7];
                                        case 5: return [4, hashHelper.Append(blob, unitIndex)];
                                        case 6:
                                            _a.sent();
                                            return [2, null];
                                        case 7: return [3, 9];
                                        case 8:
                                            e_1 = _a.sent();
                                            this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e_1);
                                            throw e_1;
                                        case 9: return [2];
                                    }
                                });
                            });
                        };
                        handlerData = function (blob, end, unitIndex) {
                            if (unitIndex === void 0) { unitIndex = 0; }
                            return __awaiter(_this, void 0, void 0, function () {
                                var percent, result, e_2;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0:
                                            percent = (blob.size / rawFile.Size) * 100;
                                            selectedFile.VirtualPercent = parseFloat((selectedFile.VirtualPercent + percent).toFixed(2));
                                            _a.label = 1;
                                        case 1:
                                            _a.trys.push([1, 3, , 4]);
                                            return [4, calc(end, blob, unitIndex)];
                                        case 2:
                                            result = (_a.sent()) || true;
                                            return [3, 4];
                                        case 3:
                                            e_2 = _a.sent();
                                            this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e_2);
                                            return [2, false];
                                        case 4:
                                            switch (unitIndex) {
                                                case 0:
                                                    selectedFile.Percent = parseFloat((selectedFile.Percent + percent).toFixed(2));
                                                    break;
                                            }
                                            return [2, result];
                                    }
                                });
                            });
                        };
                        if (!rawFile.NeedSection) return [3, 7];
                        _i = 0, _a = rawFile.Chunks;
                        _b.label = 2;
                    case 2:
                        if (!(_i < _a.length)) return [3, 6];
                        chunk = _a[_i];
                        if (selectedFile.Canceled) {
                            cancel();
                            return [3, 6];
                        }
                        appendDataPromise = handlerData(chunk.Blob, false);
                        chunk.Checking = true;
                        md5Promise = handlerData(chunk.Blob, true, 1);
                        return [4, appendDataPromise];
                    case 3:
                        if (!(_b.sent()))
                            return [3, 6];
                        return [4, md5Promise];
                    case 4:
                        md5_1 = _b.sent();
                        if ('boolean' === typeof md5_1)
                            return [3, 6];
                        chunk.Checking = false;
                        chunk.MD5 = md5_1;
                        chunk.Checked = true;
                        _b.label = 5;
                    case 5:
                        _i++;
                        return [3, 2];
                    case 6: return [3, 10];
                    case 7:
                        bufferSize = 3 * 1024 * 1024;
                        count = 0;
                        _b.label = 8;
                    case 8:
                        if (!(count < rawFile.Size)) return [3, 10];
                        if (selectedFile.Canceled) {
                            cancel();
                            return [3, 10];
                        }
                        blob = rawFile.File.slice(count, Math.min(count + bufferSize, rawFile.Size));
                        count += blob.size;
                        return [4, handlerData(blob, false)];
                    case 9:
                        if (!(_b.sent()))
                            return [3, 10];
                        return [3, 8];
                    case 10: return [4, calc(true)];
                    case 11:
                        md5 = _b.sent();
                        if ('string' !== typeof md5) {
                            this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true);
                        }
                        else {
                            rawFile.MD5 = md5;
                            selectedFile.VirtualPercent = 100;
                            selectedFile.Percent = 100;
                            selectedFile.Checking = false;
                            selectedFile.Checked = true;
                            this.PushToUploadQueue(selectedFileIndex);
                        }
                        close();
                        next();
                        return [2];
                }
            });
        });
    };
    NaiveUpload.prototype.Upload = function () {
        return __awaiter(this, void 0, void 0, function () {
            var next, selectedFileIndex, selectedFile, rawFile, uploadHelper, close, cancel, e_3;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.UploadHandlerCount >= this.Settings.ConcurrentFile)
                            return [2];
                        if (this.UploadQueue.length === 0) {
                            if (this.UploadHandlerCount === 0 && this.Settings.AfterUploadAll)
                                this.Settings.AfterUploadAll(this.RawFileList);
                            return [2];
                        }
                        this.UploadHandlerCount++;
                        next = function () {
                            _this.UploadHandlerCount--;
                            if (_this.Settings.AfterUpload) {
                                var after = _this.Settings.AfterUpload(rawFile);
                                if (after && after.then) {
                                    after.then(function () { _this.Upload(); });
                                }
                            }
                            else
                                _this.Upload();
                        };
                        selectedFileIndex = this.UploadQueue.shift();
                        selectedFile = this.SelectedFileList[selectedFileIndex];
                        if (selectedFile.Canceled) {
                            next();
                            return [2];
                        }
                        selectedFile.Uploading = true;
                        selectedFile.VirtualPercent = 0;
                        selectedFile.Percent = 0;
                        rawFile = this.GetRawFile(selectedFile);
                        uploadHelper = new UploadHelper();
                        return [4, uploadHelper.Init(this.Settings.Axios, this.Settings.Headers, this.Settings.ConcurrentChunkFile, this.Settings.EnableWorker)];
                    case 1:
                        _a.sent();
                        close = function () {
                            uploadHelper.CloseAll();
                        };
                        cancel = function () {
                            close();
                            next();
                        };
                        _a.label = 2;
                    case 2:
                        _a.trys.push([2, 4, , 5]);
                        return [4, uploadHelper.UploadFile(rawFile, function (progress) {
                                selectedFile.VirtualPercent = parseFloat((progress.PreLoaded / progress.Total * 100).toFixed(2));
                                selectedFile.Percent = parseFloat((progress.Loaded / progress.Total * 100).toFixed(2));
                            })];
                    case 3:
                        _a.sent();
                        selectedFile.VirtualPercent = 100;
                        selectedFile.Percent = 100;
                        selectedFile.Uploading = false;
                        selectedFile.Uploaded = true;
                        selectedFile.Done = true;
                        return [3, 5];
                    case 4:
                        e_3 = _a.sent();
                        console.error('error', e_3.message);
                        this.UploadError(selectedFileIndex, '文件上传失败，请删除后重新上传.', true, e_3);
                        return [3, 5];
                    case 5:
                        close();
                        next();
                        return [2];
                }
            });
        });
    };
    NaiveUpload.prototype.Limited = function () {
        return !!this.Settings.Limit && this.SelectedFileList.length >= this.Settings.Limit;
    };
    NaiveUpload.prototype.Error = function (selectedFileIndex, message, retry, done) {
        var selectedFile = this.SelectedFileList[selectedFileIndex];
        if (!retry || selectedFile.ReTry - 1 >= this.Settings.Retry) {
            selectedFile.Error = true;
            selectedFile.ErrorMessage = message;
            return;
        }
        selectedFile.ReTry++;
        done && done(selectedFileIndex);
    };
    NaiveUpload.prototype.CheckError = function (selectedFileIndex, message, retry, e) {
        var _this = this;
        if (e === void 0) { e = null; }
        console.error(e);
        var selectedFile = this.SelectedFileList[selectedFileIndex];
        selectedFile.Checking = false;
        selectedFile.Uploading = false;
        this.Error(selectedFileIndex, message, retry, function (_selectedFileIndex) {
            _this.PushToCheckQueue(_selectedFileIndex);
        });
    };
    NaiveUpload.prototype.UploadError = function (selectedFileIndex, message, retry, e) {
        var _this = this;
        if (e === void 0) { e = null; }
        console.error(e);
        var selectedFile = this.SelectedFileList[selectedFileIndex];
        selectedFile.Uploading = false;
        this.Error(selectedFileIndex, message, retry, function (_selectedFileIndex) {
            _this.PushToUploadQueue(_selectedFileIndex);
        });
    };
    NaiveUpload.prototype.GetRawFile = function (selectedFile) {
        return this.RawFileList[selectedFile.RawIndex];
    };
    NaiveUpload.prototype.GetChunks = function (selectedFile) {
        var file = this.GetRawFile(selectedFile);
        if (file.Size > this.Settings.ChunkSize) {
            file.NeedSection = true;
            file.Specs = this.Settings.ChunkSize;
        }
        else
            return;
        var count = 0;
        while (count < file.Size) {
            file.ChunkIndexQueue.push(file.Chunks.length);
            file.Chunks.push(new ChunkFile(file.Chunks.length, file.File.slice(count, count + this.Settings.ChunkSize)));
            count += this.Settings.ChunkSize;
        }
    };
    NaiveUpload.prototype.GetFileType = function (selectedFile, done) {
        var file = this.GetRawFile(selectedFile);
        var api = file.File.type === null || file.File.type === '' || file.File.type === 'application/octet-stream' ? ApiUri.FileTypeByExtension(selectedFile.ExtensionLower) : ApiUri.FileTypeByMIME(file.File.type);
        this.AxiosInstance.get(api)
            .then(function (response) {
            if (response.data.Success)
                selectedFile.FileType = response.data.Data;
            else
                selectedFile.FileType = "\u672A\u77E5";
            done && done();
        })
            .catch(function (error) {
            selectedFile.FileType = "\u672A\u77E5";
        });
    };
    NaiveUpload.prototype.CheckImage = function (selectedFile) {
        if (selectedFile.FileType !== "\u56FE\u7247")
            return;
        selectedFile.Thumbnail = this.GetRawFile(selectedFile).ObjectURL;
    };
    NaiveUpload.prototype.GetFileSize = function (selectedFile) {
        this.AxiosInstance.get(ApiUri.FileSize(this.GetRawFile(selectedFile).Size))
            .then(function (response) {
            if (response.data.Success)
                selectedFile.Size = response.data.Data;
            else
                selectedFile.Size = '-';
        })
            .catch(function (error) {
            selectedFile.Size = '-';
        });
    };
    NaiveUpload.prototype.Rename = function (selectedFile) {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    var rawFile = _this.GetRawFile(selectedFile);
                    var done = function (success) {
                        if (success) {
                            rawFile.Name = selectedFile.Name;
                            if (selectedFile.Uploaded)
                                rawFile.FileInfo.Name = selectedFile.Name;
                            selectedFile.Rename = false;
                        }
                        else
                            selectedFile.Name = rawFile.Name;
                    };
                    if (selectedFile.Uploaded) {
                        _this.AxiosInstance.get(ApiUri.Rename(rawFile.FileInfo.Id, selectedFile.Name))
                            .then(function (response) {
                            done(response.data.Success);
                            if (response.data.Success)
                                resolve();
                            else
                                reject(new Error(response.data.Message));
                        })
                            .catch(function (e) {
                            console.error(e);
                            done(false);
                            reject(new Error('文件重命名时发生异常.'));
                        });
                    }
                    else {
                        done(true);
                        resolve();
                    }
                });
                return [2, promise];
            });
        });
    };
    NaiveUpload.prototype.RenameByIndex = function (index, name) {
        return __awaiter(this, void 0, void 0, function () {
            var selectedFile;
            return __generator(this, function (_a) {
                selectedFile = this.SelectedFileList[index];
                selectedFile.Name = name;
                this.Rename(selectedFile);
                return [2];
            });
        });
    };
    return NaiveUpload;
}());
//# sourceMappingURL=NaiveUpload.js.map