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
window.onload = function () {
    window.importFile([
        {
            tag: 'link',
            type: 'text/css',
            rel: 'stylesheet',
            href: '../../element-plus/theme-chalk/element-plus.index.css'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../vue/vue.global.prod.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../utils/axios.min.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../element-plus/element-plus.index.full.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../element-plus/element-plus.es.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../element-plus/element-plus.zh-cn.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../element-plus/dayjs.zh-cn.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: '../../utils/lodash.min.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Model/MultipleUploadSetting.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Model/SelectedFile.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Model/RawFile.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Model/ChunkFile.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Model/ApiUri.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Helper/HashHelper.js'
        },
        {
            tag: 'script',
            type: 'text/javascript',
            src: 'Helper/UploadHelper.js'
        }
    ], function () {
        var Vue = window.Vue;
        var ElementPlus = window.ElementPlus;
        var Axios = window.axios;
        var Promise = window.Promise;
        var _ = window._;
        var Main;
        var MultipleUploadSettings = new MultipleUploadSetting();
        MultipleUploadSettings.Limit = 50;
        var RawFileList = [];
        var SelectedFileList;
        var CheckQueue = [];
        var CheckHandlerCount = 0;
        var UploadQueue = [];
        var UploadHandlerCount = 0;
        var AppData = {
            data: GetRenderData,
            created: function () {
                Main = this;
                SelectedFileList = this.multipleUpload.files;
                AddLoginFilter();
                Authorized();
            },
            methods: {
                SALogin: SALogin,
                TabsChanged: TabsChanged,
                GetSelectCLass: GetSelectCLass,
                GetSelectTitle: GetSelectTitle,
                ChosingFile: ChosingFile,
                AllowDrop: AllowDrop,
                DropFile: DropFile,
                ChoseFile: ChoseFile,
                GetListCLass: GetListCLass,
                GetContainerClass: GetContainerClass,
                GetContainerTitle: GetContainerTitle,
                GetFilename: GetFilename,
                ShowTools: ShowTools,
                ShowLoading: ShowLoading,
                GetLoadingStyle: GetLoadingStyle,
                GetLoadingTitle: GetLoadingTitle,
                EnableRename: EnableRename,
                Rename: Rename,
                RenameKeydown: RenameKeydown,
                EnableView: EnableView,
                View: View,
                Remove: Remove,
                Start: Start,
                Pause: Pause,
                Continues: Continues,
                Clean: Clean
            },
            watch: {
                'multipleUpload.settings': {
                    handler: function (val) {
                        for (var key in val) {
                            if (MultipleUploadSettings[key] !== val[key])
                                MultipleUploadSettings[key] = val[key];
                        }
                    },
                    deep: true
                },
                'multipleUpload.files': {
                    handler: function (files) {
                        var _this = this;
                        if (!this.multipleUpload.scrollLock)
                            return;
                        if (files.length === 0)
                            return;
                        this.$nextTick(function () {
                            var container;
                            var findChecking = false;
                            for (var i = 0; i < files.length; i++) {
                                var file = files[i];
                                if (!findChecking && file.Checking) {
                                    findChecking = true;
                                    container = _this.$refs['file_container_' + i];
                                }
                                if (file.Uploading) {
                                    container = _this.$refs['file_container_' + i];
                                    break;
                                }
                            }
                            if (!container)
                                return;
                            _this.$refs.uploadList.scrollTop = container.offsetTop - _this.$refs.uploadList.offsetTop;
                        });
                    },
                    deep: true
                }
            }
        };
        var App = Vue.createApp(AppData);
        ElementPlus.locale(ElementPlus.lang.zhCn);
        App.use(ElementPlus);
        App.mount("#app");
        function GetRenderData() {
            return {
                loading: true,
                sa: {
                    show: false,
                    loading: true,
                    username: '',
                    password: ''
                },
                config: {
                    type: 'Multiple'
                },
                single: {
                    settings: {},
                    file: {}
                },
                multipleUpload: {
                    settings: {
                        Accept: MultipleUploadSettings.Accept,
                        MultipleSelect: MultipleUploadSettings.MultipleSelect,
                        Explain: MultipleUploadSettings.Explain,
                        Tip: MultipleUploadSettings.Tip,
                        Theme: MultipleUploadSettings.Theme
                    },
                    scrollLock: true,
                    files: []
                }
            };
        }
        function AddLoginFilter() {
            Axios.interceptors.response.use(function (response) {
                return response;
            }, function (error) {
                if (error && error.response && error.response.status == 401)
                    Main.sa.show ? 1 : Main.sa.show = true;
                else if (error && error.response && error.response.status == 403) {
                    error.message = "无权限!";
                    return Promise.reject(error);
                }
                else
                    return Promise.reject(error);
            });
        }
        function Authorized() {
            Main.sa.loading = true;
            Axios.post(ApiUri.SAAuthorized).then(function (response) {
                Main.sa.loading = false;
                if (response.data.Success)
                    GetToken({});
            }).catch(function (error) {
                console.error(error);
                Main.sa.loading = false;
                ElementPlus.ElMessage('SA登录验证时发生异常.');
            });
        }
        function SALogin() {
            Main.sa.loading = true;
            var data = {
                Account: Main.sa.username,
                Password: Main.sa.password
            };
            Axios.post(ApiUri.SALogin, data).then(function (response) {
                if (response.data.Success) {
                    Main.sa.username = '';
                    Main.sa.Password = '';
                    GetToken(data);
                }
                else
                    ElementPlus.ElMessage(response.data.Message);
            }).catch(function (error) {
                console.error(error);
                Main.sa.loading = false;
                ElementPlus.ElMessage('SA身份验证时发生异常.');
            });
        }
        function GetToken(data) {
            Axios.post(ApiUri.GetToken, data).then(function (response) {
                Main.sa.loading = false;
                if (response.data.Success) {
                    MultipleUploadSettings.Headers['Authorization'] = response.data.Data.AccessToken;
                    setTimeout(RefreshToken, new Date(response.data.Data.Expires).getTime() - new Date().getTime() - 60 * 1000);
                    Main.sa.show = false;
                }
                else
                    ElementPlus.ElMessage(response.data.Message);
            }).catch(function (error) {
                console.error(error);
                Main.sa.loading = false;
                ElementPlus.ElMessage('获取Token时发生异常.');
            });
        }
        function RefreshToken() {
            Axios.post(ApiUri.RefreshToken).then(function (response) {
                if (response.data.Success) {
                    MultipleUploadSettings.Headers['Authorization'] = response.data.Data.AccessToken;
                    setTimeout(RefreshToken, response.data.Data.Expires.getTime() - new Date().getTime() - 60 * 1000);
                }
                else
                    ElementPlus.ElMessage(response.data.Message);
            }).catch(function (error) {
                console.error(error);
                ElementPlus.ElMessage('刷新Token时发生异常.');
            });
        }
        function TabsChanged(tab) {
            Main.config.type = tab.props.name;
            switch (tab.props.name) {
                case 'Signle':
                default:
                    break;
                case 'Multiple':
                    break;
            }
        }
        function Limited() {
            return !!MultipleUploadSettings.Limit && Main.multipleUpload.files.length >= MultipleUploadSettings.Limit;
        }
        function GetListCLass() {
            return "upload-" + Main.multipleUpload.settings.Theme + "-list";
        }
        function GetSelectCLass() {
            return "upload-drag " + (Limited() ? 'item-limited' : '');
        }
        function GetSelectTitle() {
            if (!MultipleUploadSettings.Limit)
                return null;
            return SelectedFileList.length < MultipleUploadSettings.Limit ? "\u8FD8\u53EF\u6DFB\u52A0\u4E2A" + (MultipleUploadSettings.Limit - SelectedFileList.length) + "\u6587\u4EF6" : "\u6587\u4EF6\u6570\u91CF\u5DF2\u8FBE\u4E0A\u9650";
        }
        function ChosingFile(e) {
            if (Limited())
                return;
            Main.$refs.fileInput.click();
        }
        function AllowDrop(e) {
            e.preventDefault();
        }
        function DropFile(e) {
            e.preventDefault();
            AppendFiles(e.dataTransfer.files);
        }
        function ChoseFile(e) {
            AppendFiles(Main.$refs.fileInput.files);
        }
        function AppendFiles(files) {
            if (!!MultipleUploadSettings.Limit && (SelectedFileList.length + files.length) > MultipleUploadSettings.Limit) {
                ElementPlus.ElMessage("\u6700\u591A\u53EA\u80FD\u6DFB\u52A0" + MultipleUploadSettings.Limit + "\u4E2A\u6587\u4EF6.");
            }
            var push = function (file, newFile) {
                var rawFile = new RawFile(file);
                rawFile.Name = newFile.Name;
                rawFile.Extension = newFile.Extension;
                RawFileList.push(rawFile);
                newFile.RawIndex = RawFileList.length - 1;
                SelectedFileList.push(newFile);
                HandleFile(SelectedFileList.length - 1);
            };
            var fileList = Array.prototype.slice.call(files);
            var _loop_1 = function (file) {
                if (Limited())
                    return { value: void 0 };
                var selectedFile = new SelectedFile(file);
                if (MultipleUploadSettings.BeforeUpload) {
                    var before = MultipleUploadSettings.BeforeUpload(file);
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
            for (var _i = 0, fileList_1 = fileList; _i < fileList_1.length; _i++) {
                var file = fileList_1[_i];
                var state_1 = _loop_1(file);
                if (typeof state_1 === "object")
                    return state_1.value;
            }
        }
        function HandleFile(selectedFileIndex) {
            var selectedFile = SelectedFileList[selectedFileIndex];
            selectedFile.Thumbnail = ApiUri.FileTypeImageUrl(selectedFile.ExtensionLower);
            getFileType(selectedFile, function () {
                checkImage(selectedFile);
            });
            getFileSize(selectedFile);
            if (MultipleUploadSettings.EnableChunk)
                getChunks(selectedFile);
            PushToCheckQueue(selectedFileIndex);
        }
        function PushToCheckQueue(selectedFileIndex) {
            CheckQueue.push(selectedFileIndex);
            CheckMD5();
        }
        function PushToUploadQueue(selectedFileIndex) {
            UploadQueue.push(selectedFileIndex);
            Upload();
        }
        function CheckMD5() {
            return __awaiter(this, void 0, void 0, function () {
                var next, selectedFileIndex, selectedFile, rawFile, hashHelper, close, cancel, calc, handlerData, _i, _a, chunk, appendDataPromise, md5Promise, md5_1, bufferSize, count, blob, md5;
                var _this = this;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            if (CheckHandlerCount >= MultipleUploadSettings.ConcurrentFile || CheckQueue.length === 0) {
                                return [2];
                            }
                            CheckHandlerCount++;
                            next = function () {
                                CheckHandlerCount--;
                                CheckMD5();
                            };
                            selectedFileIndex = CheckQueue.shift();
                            selectedFile = SelectedFileList[selectedFileIndex];
                            if (selectedFile.Canceled) {
                                next();
                                return [2];
                            }
                            selectedFile.Checking = true;
                            rawFile = getRawFile(selectedFile);
                            hashHelper = new HashHelper();
                            return [4, hashHelper.Init(rawFile.NeedSection ? 2 : 1, MultipleUploadSettings.EnableWorker)];
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
                                                checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e_1);
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
                                                percent = (blob.size / rawFile.File.size) * 100;
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
                                                checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e_2);
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
                            if (!(count < rawFile.File.size)) return [3, 10];
                            if (selectedFile.Canceled) {
                                cancel();
                                return [3, 10];
                            }
                            blob = rawFile.File.slice(count, Math.min(count + bufferSize, rawFile.File.size));
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
                                checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true);
                            }
                            else {
                                rawFile.MD5 = md5;
                                selectedFile.VirtualPercent = 100;
                                selectedFile.Percent = 100;
                                selectedFile.Checking = false;
                                selectedFile.Checked = true;
                                PushToUploadQueue(selectedFileIndex);
                            }
                            close();
                            next();
                            return [2];
                    }
                });
            });
        }
        function Upload() {
            return __awaiter(this, void 0, void 0, function () {
                var next, selectedFileIndex, selectedFile, rawFile, uploadHelper, close, cancel, e_3;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (UploadHandlerCount >= MultipleUploadSettings.ConcurrentFile)
                                return [2];
                            if (!(UploadQueue.length === 0)) return [3, 3];
                            if (!MultipleUploadSettings.Done) return [3, 2];
                            return [4, MultipleUploadSettings.Done(RawFileList)];
                        case 1:
                            _a.sent();
                            _a.label = 2;
                        case 2: return [2];
                        case 3:
                            UploadHandlerCount++;
                            next = function () {
                                UploadHandlerCount--;
                                if (MultipleUploadSettings.AfterUpload) {
                                    var after = MultipleUploadSettings.AfterUpload(rawFile);
                                    if (after && after.then) {
                                        after.then(Upload);
                                    }
                                }
                                else
                                    Upload();
                            };
                            selectedFileIndex = UploadQueue.shift();
                            selectedFile = SelectedFileList[selectedFileIndex];
                            if (selectedFile.Canceled) {
                                next();
                                return [2];
                            }
                            selectedFile.Uploading = true;
                            selectedFile.VirtualPercent = 0;
                            selectedFile.Percent = 0;
                            rawFile = getRawFile(selectedFile);
                            uploadHelper = new UploadHelper();
                            return [4, uploadHelper.Init(Axios, MultipleUploadSettings.Headers, MultipleUploadSettings.ConcurrentChunkFile, MultipleUploadSettings.EnableWorker)];
                        case 4:
                            _a.sent();
                            close = function () {
                                uploadHelper.CloseAll();
                            };
                            cancel = function () {
                                close();
                                next();
                            };
                            _a.label = 5;
                        case 5:
                            _a.trys.push([5, 7, , 8]);
                            return [4, uploadHelper.UploadFile(rawFile, function (progress) {
                                    selectedFile.VirtualPercent = parseFloat((progress.PreLoaded / progress.Total * 100).toFixed(2));
                                    selectedFile.Percent = parseFloat((progress.Loaded / progress.Total * 100).toFixed(2));
                                })];
                        case 6:
                            _a.sent();
                            selectedFile.VirtualPercent = 100;
                            selectedFile.Percent = 100;
                            selectedFile.Uploading = false;
                            selectedFile.Uploaded = true;
                            selectedFile.Done = true;
                            return [3, 8];
                        case 7:
                            e_3 = _a.sent();
                            console.error('error', e_3.message);
                            uploadError(selectedFileIndex, '文件上传失败，请删除后重新上传.', true, e_3);
                            return [3, 8];
                        case 8:
                            close();
                            next();
                            return [2];
                    }
                });
            });
        }
        function error(selectedFileIndex, message, retry, done) {
            var selectedFile = SelectedFileList[selectedFileIndex];
            if (!retry || selectedFile.ReTry - 1 >= MultipleUploadSettings.Retry) {
                selectedFile.Error = true;
                selectedFile.ErrorMessage = message;
                return;
            }
            selectedFile.ReTry++;
            done && done(selectedFileIndex);
        }
        function checkError(selectedFileIndex, message, retry, e) {
            if (e === void 0) { e = null; }
            console.error(e);
            var selectedFile = SelectedFileList[selectedFileIndex];
            selectedFile.Checking = false;
            selectedFile.Uploading = false;
            error(selectedFileIndex, message, retry, function (_selectedFileIndex) {
                PushToCheckQueue(_selectedFileIndex);
            });
        }
        function uploadError(selectedFileIndex, message, retry, e) {
            if (e === void 0) { e = null; }
            console.error(e);
            var selectedFile = SelectedFileList[selectedFileIndex];
            selectedFile.Uploading = false;
            error(selectedFileIndex, message, retry, function (_selectedFileIndex) {
                PushToUploadQueue(_selectedFileIndex);
            });
        }
        function getRawFile(selectedFile) {
            return RawFileList[selectedFile.RawIndex];
        }
        function getChunks(selectedFile) {
            var file = getRawFile(selectedFile);
            if (file.File.size > MultipleUploadSettings.ChunkSize) {
                file.NeedSection = true;
                file.Specs = MultipleUploadSettings.ChunkSize;
            }
            else
                return;
            var count = 0;
            while (count < file.File.size) {
                file.ChunkIndexQueue.push(file.Chunks.length);
                file.Chunks.push(new ChunkFile(file.Chunks.length, file.File.slice(count, count + MultipleUploadSettings.ChunkSize)));
                count += MultipleUploadSettings.ChunkSize;
            }
        }
        function getFileType(selectedFile, done) {
            var file = getRawFile(selectedFile);
            var api = file.File.type === null || file.File.type === '' || file.File.type === 'application/octet-stream' ? ApiUri.FileTypeByExtension(selectedFile.ExtensionLower) : ApiUri.FileTypeByMIME(file.File.type);
            Axios.get(api)
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
        }
        function checkImage(selectedFile) {
            if (selectedFile.FileType !== "\u56FE\u7247")
                return;
            selectedFile.Thumbnail = getRawFile(selectedFile).ObjectURL;
        }
        function getFileSize(selectedFile) {
            Axios.get(ApiUri.FileSize(getRawFile(selectedFile).File.size))
                .then(function (response) {
                if (response.data.Success)
                    selectedFile.Size = response.data.Data;
                else
                    selectedFile.Size = '-';
            })
                .catch(function (error) {
                selectedFile.Size = '-';
            });
        }
        function GetContainerClass(selectedFile) {
            return "item-container " + (selectedFile.Done ? ' item-done' : '') + " " + (selectedFile.Error ? ' item-error' : '') + " " + (selectedFile.Hover && !selectedFile.Rename && !selectedFile.Checking && !selectedFile.Uploading ? ' item-hover' : '') + " " + (selectedFile.Checking ? ' item-checking' : '') + " " + (selectedFile.Uploading ? ' item-uploading' : '');
        }
        function GetContainerTitle(selectedFile) {
            return (selectedFile.Done ? '上传成功' : '') + " " + (selectedFile.Error ? selectedFile.ErrorMessage : '');
        }
        function GetFilename(selectedFile) {
            return (selectedFile.Name || '-') + (selectedFile.Extension || '-');
        }
        function ShowTools(selectedFile) {
            return selectedFile.Hover && !selectedFile.Rename && !selectedFile.Checking && !selectedFile.Uploading;
        }
        function ShowLoading(selectedFile) {
            return !selectedFile.Rename && (selectedFile.Checking || selectedFile.Uploading);
        }
        function GetGradientStyle(type, color, value1, value2) {
            switch (type) {
                default:
                case 'conic':
                    return "\nbackground: conic-gradient(" + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -moz-conic-gradient(" + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -o-conic-gradient(" + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -webkit-conic-gradient(" + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;";
                case 'linear':
                    return "\nbackground: linear-gradient(left, " + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -moz-linear-gradient(left, " + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -o-linear-gradient(left, " + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;\nbackground: -webkit-linear-gradient(left, " + color + " " + value1 + "%, transparent " + value2 + "%)  repeat scroll 0% 0%;";
            }
        }
        function GetLoadingStyle(selectedFile) {
            var styleType = MultipleUploadSettings.Theme === 'card' ? 'conic' : 'linear';
            return (selectedFile.Checking ? GetGradientStyle(styleType, 'rgba(255, 236, 201, 0.5)', selectedFile.Percent, 0) : '') + " " + (selectedFile.Uploading ? GetGradientStyle(styleType, 'rgba(144, 206, 255, 0.5)', selectedFile.Percent, selectedFile.VirtualPercent) : '');
        }
        function GetLoadingTitle(selectedFile) {
            return (selectedFile.Checking ? ('扫描中...' + selectedFile.Percent + '%') : '') + " " + (selectedFile.Uploading ? ('上传中...' + selectedFile.Percent + '%') : '');
        }
        function EnableRename(selectedFile) {
            return !selectedFile.Uploading && !selectedFile.Uploaded;
        }
        function Rename(selectedFile) {
            selectedFile.Rename = true;
            Main.$nextTick(function (_) {
                Main.$refs.renameInput.focus();
            }, 100);
        }
        function RenameKeydown(selectedFile, event) {
            if (event.keyCode == 13) {
                selectedFile.Rename = false;
                var rawFile = getRawFile(selectedFile);
                rawFile.Extension = selectedFile.Extension;
                rawFile.Name = selectedFile.Name;
            }
        }
        function EnableView(selectedFile) {
            switch (selectedFile.FileType) {
                case "\u56FE\u7247":
                case "\u97F3\u9891":
                    return selectedFile.ExtensionLower !== '.flac';
                case "\u89C6\u9891":
                    return true;
                case "\u6587\u672C\u6587\u4EF6":
                    return true;
                case "\u7535\u5B50\u6587\u6863":
                    return selectedFile.ExtensionLower === '.pdf';
                default:
                    return false;
            }
        }
        function View(selectedFile) {
            var file = getRawFile(selectedFile);
            var bodyStyle = "margin:0px;text-align: center;display: flex;flex-direction: row;justify-content: center;align-items: center;";
            switch (selectedFile.FileType) {
                case "\u56FE\u7247":
                    var winImage = window.open();
                    winImage.document.write("<body style=\"" + bodyStyle + "background-color: black;\"><img src=\"" + file.ObjectURL + "\" /img></body>");
                    break;
                case "\u97F3\u9891":
                    if (selectedFile.ExtensionLower === '.flac')
                        return;
                    var winAudio = window.open();
                    winAudio.document.write("<body style=\"" + bodyStyle + "background-color: black;\"><audio src=\"" + file.ObjectURL + "\" controls=\"controls\">\u62B1\u6B49, \u6682\u4E0D\u652F\u6301</audio></body>");
                    break;
                case "\u89C6\u9891":
                    var winVideo = window.open();
                    winVideo.document.write("<body style=\"" + bodyStyle + "background-color: black;\"><video src=\"" + file.ObjectURL + "\" controls=\"controls\">\u62B1\u6B49, \u6682\u4E0D\u652F\u6301</video></body>");
                    break;
                default:
                    var win = window.open();
                    win.document.write("<body style=\"" + bodyStyle + "\"><object data=\"" + file.ObjectURL + "\" type=\"" + (selectedFile.ExtensionLower === '.txt' ? 'text/plain' : (selectedFile.ExtensionLower === '.pdf' ? 'application/pdf' : 'application/octet-stream')) + "\" width=\"100%\" height=\"100%\"><iframe src=\"" + file.ObjectURL + "\" scrolling=\"no\" width=\"100%\" height=\"100%\" frameborder=\"0\" ></iframe></object></body>");
                    break;
            }
        }
        function Remove(index) {
            SelectedFileList[index].Canceled = true;
        }
        function Start() {
        }
        function Pause() {
        }
        function Continues() {
        }
        function Clean() {
            SelectedFileList.length = 0;
        }
    });
};
//# sourceMappingURL=NaiveUpload.js.map