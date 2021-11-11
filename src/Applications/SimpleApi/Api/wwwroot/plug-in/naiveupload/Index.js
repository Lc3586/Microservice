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
var _this = this;
window.onload = function () {
    ImportHelper.ImportFile([
        {
            Tag: "link",
            Attributes: {
                type: 'text/css',
                rel: 'stylesheet',
                href: ApiUri.BaseUrl + '/element-plus/theme-chalk/element-plus.index.css'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/vue/vue.global.prod.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/utils/axios.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/element-plus/element-plus.index.full.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/element-plus/element-plus.index.full.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/element-plus/element-plus.es.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/element-plus/element-plus.zh-cn.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/utils/dayjs/dayjs.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/utils/dayjs/dayjs.zh-cn.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/utils/lodash.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/ApiUri.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/NaiveUpload.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/MultipleUploadSetting.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Helper/ChunkFileMergeTaskHelper.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/UploadConfigDetail.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/Pagination.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/LibraryInfo.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/VideoInfo.js'
            }
        }
    ], function () {
        var Vue = window.Vue;
        var ElementPlus = window.ElementPlus;
        var Axios = window.axios;
        var Promise = window.Promise;
        var _ = window._;
        var Dayjs = window.dayjs;
        var Main;
        var MultipleUploadSettings = new MultipleUploadSetting();
        MultipleUploadSettings.Axios = Axios;
        MultipleUploadSettings.Mode = "AMT";
        MultipleUploadSettings.AfterCheckAll = function (files) { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                ElementPlus.ElMessage('所有文件校验完毕.');
                return [2];
            });
        }); };
        MultipleUploadSettings.AfterUploadAll = function (files) { return __awaiter(_this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                ElementPlus.ElMessage('所有文件上传完毕.');
                return [2];
            });
        }); };
        var Upload = new NaiveUpload();
        var AppData = {
            data: GetRenderData,
            created: function () {
                Main = this;
                Upload.SelectedFileList = this.multipleUpload.files;
                AddLoginFilter();
                Authorized();
                getConfig();
                Main.fileListFileTypeGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListStorageTypeGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListFileStateGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListDataRangeGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListNameGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListExtensionGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListContentTypeGetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListMD5GetAnswer = _.debounce(updateFileListPagination, 500);
                Main.fileListServerKeyGetAnswer = _.debounce(updateFileListPagination, 500);
            },
            methods: {
                SALogin: SALogin,
                TabsChanged: TabsChanged,
                LoadConfig: LoadConfig,
                FilterConfig: FilterConfig,
                ConfigDetail: ConfigDetail,
                ApplyConfig: ApplyConfig,
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
                RenameDone: RenameDone,
                EnableView: EnableView,
                View: View,
                Remove: Remove,
                Start: Start,
                Pause: Pause,
                Continues: Continues,
                Clean: Clean,
                GetFolderClass: GetFolderClass,
                OpenFolder: OpenFolder,
                CloseFolder: CloseFolder,
                ActiveCollapseChange: ActiveCollapseChange,
                fileListNameChange: fileListNameChange,
                fileListExtensionChange: fileListExtensionChange,
                fileListServerKeyChange: fileListServerKeyChange,
                fileListContentTypeChange: fileListContentTypeChange,
                fileListMD5Change: fileListMD5Change,
                handleFileListCommand: handleFileListCommand,
                allFileType: allFileType,
                fileTypeChange: fileTypeChange,
                allStorageType: allStorageType,
                storageTypeChange: storageTypeChange,
                allFileState: allFileState,
                fileStateChange: fileStateChange,
                fileListDateRangeChange: fileListDateRangeChange,
                fileSort: fileSort,
                fileListSizeChange: fileListSizeChange,
                fileListCurrentChange: fileListCurrentChange,
                fileDetail: fileDetail,
                getFileStateTag: getFileStateTag,
                closeFileDetail: closeFileDetail,
                previewFile: previewFile,
                browseFile: browseFile,
                downloadFile: downloadFile,
                deleteFile: deleteFile,
                videoInfo: videoInfo,
                filePreviewStart: filePreviewStart,
                videoPreviewNext: videoPreviewNext,
                filePreviewEnd: filePreviewEnd
            },
            watch: {
                'multipleUpload.settings': {
                    handler: function (val) {
                        for (var key in val) {
                            if (key !== 'Config' && MultipleUploadSettings[key] !== val[key])
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
                            _this.$refs.uploadList.scrollTop = container.offsetTop - _this.$refs.uploadList.offsetTop - 20;
                        });
                    },
                    deep: true
                },
                'multipleUpload.config.filter': {
                    handler: function (val) {
                        this.$refs.configTree.filter(val);
                    }
                }
            }
        };
        Upload.Init(MultipleUploadSettings).then(function () { return __awaiter(_this, void 0, void 0, function () {
            var e_1, App;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4, Upload.UpdateConfig('default')];
                    case 1:
                        _a.sent();
                        return [3, 3];
                    case 2:
                        e_1 = _a.sent();
                        console.error(e_1);
                        ElementPlus.ElMessage(e_1.message);
                        return [3, 3];
                    case 3:
                        App = Vue.createApp(AppData);
                        App.use(ElementPlus, {
                            locale: ElementPlus.lang.zhCn
                        });
                        App.mount("#app");
                        return [2];
                }
            });
        }); });
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
                    type: 'Upload',
                    fileTypes: [],
                    storageTypes: [],
                    fileStates: [],
                    page: {
                        sizes: [5, 10, 15, 20, 50, 100, 150, 200, 300, 400, 500]
                    },
                    dateRangShortcuts: [
                        {
                            text: '最近一周',
                            value: (function () {
                                var end = new Date();
                                var start = new Date();
                                start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                                return [start, end];
                            })(),
                        },
                        {
                            text: '最近一个月',
                            value: (function () {
                                var end = new Date();
                                var start = new Date();
                                start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                                return [start, end];
                            })(),
                        },
                        {
                            text: '最近三个月',
                            value: (function () {
                                var end = new Date();
                                var start = new Date();
                                start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                                return [start, end];
                            })(),
                        }
                    ]
                },
                multipleUpload: {
                    settings: {
                        Config: MultipleUploadSettings.Config,
                        Tip: MultipleUploadSettings.Tip,
                        Theme: MultipleUploadSettings.Theme,
                        Mode: MultipleUploadSettings.Mode
                    },
                    config: {
                        props: {
                            label: 'DisplayName',
                            children: 'Children',
                            isLeaf: 'Leaf',
                        },
                        data: [],
                        filter: ''
                    },
                    scrollLock: true,
                    files: []
                },
                chunkFileMergeTask: {
                    init: false,
                    search: '',
                    list: [],
                    error: '',
                    loading: true
                },
                library: {
                    init: false,
                    activeCollapse: 'folders',
                    folders: [],
                    error: '',
                    loading: true,
                    currentOpenFolder: null,
                    fileDetail: {
                        show: false,
                        loading: true,
                        detail: {}
                    },
                    videoInfo: {
                        activeTab: '',
                        enable: false,
                        loading: true,
                        error: '',
                        detail: {},
                        fileId: '',
                        previewId: '',
                        previewTicks: 0,
                        previewTimespan: [0, 0, 0, 1],
                        previewState: false
                    },
                    previewImages: []
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
                if (response && response.data && response.data.Success)
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
                    window.location.reload();
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
                    setTimeout(RefreshToken, new Date(response.data.Data.Expires).getTime() - new Date(response.data.Data.Created).getTime() - 60 * 1000);
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
                    setTimeout(RefreshToken, new Date(response.data.Data.Expires).getTime() - new Date().getTime() - 60 * 1000);
                }
                else
                    ElementPlus.ElMessage(response.data.Message);
            }).catch(function (error) {
                console.error(error);
                ElementPlus.ElMessage('刷新Token时发生异常.');
            });
        }
        function getConfig() {
            Axios.all([getFileTypes(), getStorageTypes(), getFileStates()])
                .then(Axios.spread(function (fileTypes, storageTypes, fileStates) {
                Main.loading = false;
                if (fileTypes.data.Success)
                    Main.config.fileTypes = fileTypes.data.Data;
                else
                    ElementPlus.ElMessage(fileTypes.data.Message);
                if (storageTypes.data.Success)
                    Main.config.storageTypes = storageTypes.data.Data;
                else
                    ElementPlus.ElMessage(storageTypes.data.Message);
                if (fileStates.data.Success)
                    Main.config.fileStates = fileStates.data.Data;
                else
                    ElementPlus.ElMessage(fileStates.data.Message);
            }))
                .catch(function (error) {
                console.error(error);
                Main.loading = false;
                if (!Main.sa.show)
                    ElementPlus.ElMessage('获取配置时发生异常.');
            });
        }
        function getFileTypes() {
            return Axios.get(ApiUri.GetFileTypes);
        }
        function getStorageTypes() {
            return Axios.get(ApiUri.GetStorageTypes);
        }
        function getFileStates() {
            return Axios.get(ApiUri.GetFileStates);
        }
        function LoadConfig(node, resolve) {
            var _a;
            Axios.post(ApiUri.GetCurrentAccountCFUCTree, {
                ParentId: (_a = node.data) === null || _a === void 0 ? void 0 : _a.Id,
                Rank: 0,
                Pagination: {
                    PageIndex: -1,
                    SortField: 'Sort',
                    SortType: 'asc',
                    DynamicFilterInfo: [
                        {
                            Field: 'Enable',
                            Value: true,
                            Compare: 'eq'
                        }
                    ]
                }
            }).then(function (response) {
                if (response.data.Success) {
                    response.data.Data.forEach(function (item, index) {
                        item.Leaf = !item.HasChildren;
                    });
                    resolve(response.data.Data);
                }
                else
                    ElementPlus.ElMessage(response.data.Message);
            }).catch(function (error) {
                console.error(error);
                ElementPlus.ElMessage('加载文件上传配置时发生异常.');
            });
        }
        function FilterConfig(value, data) {
            if (!value)
                return true;
            return data.Name.indexOf(value) !== -1 || data.Code.indexOf(value) !== -1 || data.DisplayName.indexOf(value) !== -1;
        }
        function ConfigDetail(node, data) {
            if (node.detailLoading === false)
                return;
            node.detailLoading = true;
            Axios.post(ApiUri.FileUploadConfigDetailData(data.Id))
                .then(function (response) {
                if (response.data.Success) {
                    node.detailLoading = false;
                    node.detailData = response.data.Data;
                }
                else {
                    node.detailError = response.data.Message;
                }
            }).catch(function (error) {
                console.error(error);
                node.detailError = '加载文件上传配置详情时发生异常.';
            });
        }
        function ApplyConfig(node, data) {
            var done = function (_data, updated) {
                if (!updated)
                    Upload.UpdateConfigData(_data);
                Main.multipleUpload.settings.Config = _data;
                node.configLoading = false;
                ElementPlus.ElMessage('配置应用成功.');
            };
            if (node.detailLoading === false) {
                node.configData = node.detailData;
                done(node.detailData, false);
                return;
            }
            else if (node.configLoading === false) {
                done(node.configData, false);
                return;
            }
            node.configLoading = true;
            Upload.UpdateConfig(data.Id)
                .then(function (_data) {
                node.configData = _data;
                done(_data, true);
            })
                .catch(function (error) {
                console.error(error);
                ElementPlus.ElMessage("\u5E94\u7528\u6587\u4EF6\u4E0A\u4F20\u914D\u7F6E\u5931\u8D25, " + error.message);
            });
        }
        function TabsChanged(tab) {
            Main.config.type = tab.props.name;
            switch (tab.props.name) {
                case 'Upload':
                    if (Upload.SelectedFileList.length > 1) {
                        Upload.Clean();
                        MultipleUploadSettings.Config.UpperLimit = 1;
                    }
                    else
                        MultipleUploadSettings.Config.UpperLimit = 50;
                    break;
                case 'Bigger':
                    InitChunkFileMergeTaskList();
                    break;
                case 'Library':
                    if (Main.library.init)
                        return;
                    GetLibraryInfo();
                    break;
                default:
                    break;
            }
        }
        function GetListCLass() {
            return "upload-" + Main.multipleUpload.settings.Theme + "-list";
        }
        function GetSelectCLass() {
            return "upload-drag " + (Upload.Limited() ? 'item-limited' : '');
        }
        function GetSelectTitle() {
            if (!MultipleUploadSettings.Config.UpperLimit)
                return null;
            return Upload.SelectedFileList.length < MultipleUploadSettings.Config.UpperLimit ? "\u8FD8\u53EF\u6DFB\u52A0\u4E2A" + (MultipleUploadSettings.Config.UpperLimit - Upload.SelectedFileList.length) + "\u6587\u4EF6" : "\u6587\u4EF6\u6570\u91CF\u5DF2\u8FBE\u4E0A\u9650";
        }
        function ChosingFile(e) {
            if (Upload.Limited())
                return;
            Main.$refs.fileInput.click();
        }
        function AllowDrop(e) {
            e.preventDefault();
        }
        function DropFile(e) {
            e.preventDefault();
            CheckAppendFilesResult(Upload.AppendFiles(e.dataTransfer.files));
        }
        function ChoseFile(e) {
            CheckAppendFilesResult(Upload.AppendFiles(Main.$refs.fileInput.files));
        }
        function CheckAppendFilesResult(status) {
            switch (status) {
                case "OK":
                    break;
                case "TYPE":
                    ElementPlus.ElMessage('文件类型不合法');
                    break;
                case "LIMIT":
                    ElementPlus.ElMessage('超出数量限制');
                    break;
                default:
            }
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
            return !selectedFile.Uploading;
        }
        function Rename(selectedFile) {
            selectedFile.Rename = true;
            Main.$nextTick(function (_) {
                document.getElementsByClassName('item-rename-input')[0].focus();
            }, 100);
        }
        function RenameKeydown(selectedFile, event) {
            if (event.keyCode == 13) {
                RenameDone(selectedFile);
            }
        }
        function RenameDone(selectedFile) {
            Upload.Rename(selectedFile).catch(function (e) {
                console.error(e);
                ElementPlus.ElMessage(e.Message);
            });
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
            var file = Upload.GetRawFile(selectedFile);
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
            Upload.Remove(index);
        }
        function Start() {
            for (var i = 0; i < MultipleUploadSettings.ConcurrentFile; i++) {
                Upload.Upload();
            }
        }
        function Pause() {
            Upload.Pause();
        }
        function Continues() {
            Upload.Continues();
        }
        function Clean() {
            Upload.Clean();
        }
        function InitChunkFileMergeTaskList() {
            return __awaiter(this, void 0, void 0, function () {
                var chunkFileMergeTaskHelper;
                var _this = this;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (Main.chunkFileMergeTask.init)
                                return [2];
                            Main.chunkFileMergeTask.init = true;
                            chunkFileMergeTaskHelper = new ChunkFileMergeTaskHelper();
                            return [4, chunkFileMergeTaskHelper.Init()];
                        case 1:
                            _a.sent();
                            chunkFileMergeTaskHelper.AddTask = function (task) { return __awaiter(_this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    task.ChunksSources = [];
                                    Main.chunkFileMergeTask.list.push(task);
                                    return [2];
                                });
                            }); };
                            chunkFileMergeTaskHelper.UpdateTask = function (id, data) { return __awaiter(_this, void 0, void 0, function () {
                                var _i, _a, task, key;
                                return __generator(this, function (_b) {
                                    for (_i = 0, _a = Main.chunkFileMergeTask.list; _i < _a.length; _i++) {
                                        task = _a[_i];
                                        if (task.Id !== id)
                                            continue;
                                        for (key in data) {
                                            task[key] = data[key];
                                        }
                                        break;
                                    }
                                    return [2];
                                });
                            }); };
                            chunkFileMergeTaskHelper.RemoveTask = function (md5) { return __awaiter(_this, void 0, void 0, function () {
                                var _i, _a, task;
                                return __generator(this, function (_b) {
                                    for (_i = 0, _a = Main.chunkFileMergeTask.list; _i < _a.length; _i++) {
                                        task = _a[_i];
                                        if (task.MD5 !== md5)
                                            continue;
                                        task.Remove = true;
                                        break;
                                    }
                                    return [2];
                                });
                            }); };
                            chunkFileMergeTaskHelper.UpdateChunksSource = function (md5, chunksSource) { return __awaiter(_this, void 0, void 0, function () {
                                var _i, _a, task, _b, _c, source;
                                return __generator(this, function (_d) {
                                    for (_i = 0, _a = Main.chunkFileMergeTask.list; _i < _a.length; _i++) {
                                        task = _a[_i];
                                        if (task.MD5 !== md5)
                                            continue;
                                        for (_b = 0, _c = task.ChunksSources; _b < _c.length; _b++) {
                                            source = _c[_b];
                                            if (source.Specs == chunksSource.Specs
                                                && source.Total == chunksSource.Total) {
                                                source.Activitys = chunksSource.Activitys;
                                                return [2];
                                            }
                                        }
                                        task.ChunksSources.push(chunksSource);
                                        break;
                                    }
                                    return [2];
                                });
                            }); };
                            return [4, chunkFileMergeTaskHelper.Connect()];
                        case 2:
                            _a.sent();
                            Main.chunkFileMergeTask.loading = false;
                            return [2];
                    }
                });
            });
        }
        function GetLibraryInfo() {
            Main.library.loading = true;
            Axios.get(ApiUri.GetLibraryInfo)
                .then(function (response) {
                Main.library.loading = false;
                if (response.data.Success) {
                    Main.library.folders = response.data.Data.map(function (item, index) {
                        var folder = item;
                        folder.Hover = false;
                        folder.Open = false;
                        folder.Class = GetFolderClassByType(folder.FileType);
                        folder.Files = new FileListInfo();
                        folder.Files.Filters.FileType = [folder.FileType];
                        folder.Files.CheckAllFileType = false;
                        folder.Files.IsFileTypeIndeterminate = true;
                        folder.Files.Filters.StorageType = Main.config.storageTypes;
                        folder.Files.CheckAllStorageType = true;
                        folder.Files.IsStorageTypeIndeterminate = false;
                        folder.Files.Filters.State = Main.config.fileStates;
                        folder.Files.CheckAllFileState = true;
                        folder.Files.IsFileStateIndeterminate = false;
                        folder.Files.Pagination.AdvancedSort.push({ Field: 'CreateTime', Type: "desc" });
                        return folder;
                    });
                    if (!Main.library.init)
                        Main.library.init = true;
                }
                else {
                    Main.library.error = response.data.Message;
                    ElementPlus.ElMessage(Main.library.error);
                }
            }).catch(function (error) {
                Main.library.loading = false;
                console.error(error);
                Main.library.error = '加载文件上传配置详情时发生异常.';
                ElementPlus.ElMessage(Main.library.error);
            });
        }
        function GetFolderClassByType(fileType) {
            switch (fileType) {
                case "\u97F3\u9891":
                    return 'icon-folder-audio';
                case "\u89C6\u9891":
                    return 'icon-folder-video';
                case "\u56FE\u7247":
                    return 'icon-folder-picture';
                case "\u6587\u672C\u6587\u4EF6":
                    return 'icon-folder-text';
                case "\u7535\u5B50\u8868\u683C":
                    return 'icon-folder-excel';
                case "\u7535\u5B50\u6587\u6863":
                    return 'icon-folder-word';
                case "\u538B\u7F29\u5305":
                    return 'icon-folder-zip';
                case "\u5916\u94FE\u8D44\u6E90":
                    return 'icon-folder-uri';
                case "\u672A\u77E5":
                default:
                    return 'icon-folder-none';
            }
        }
        function GetFolderClass(folder) {
            return "folder " + (folder.Hover ? ' hover' : '');
        }
        function OpenFolder(folder, index) {
            Main.library.currentOpenFolder = folder;
            folder.Files.Pagination.PageRows = getPageSize();
            folder.Open = true;
            folder.Files.Filters.FileType = [folder.FileType];
            Main.library.activeCollapse = 'files';
            updateFileListPagination(folder);
        }
        function CloseFolder(folder, index) {
            folder.Open = false;
            Main.library.currentOpenFolder = null;
        }
        function ActiveCollapseChange(val) {
            if (val !== 'folders')
                return;
            if (!Main.library.currentOpenFolder)
                return;
            Main.library.currentOpenFolder.Open = false;
            Main.library.currentOpenFolder = null;
        }
        function getPageSize() {
            var current = window.innerHeight / 100, min, result;
            for (var i in Main.config.page.sizes) {
                var size = Main.config.page.sizes[i];
                var abs = Math.abs(current - size);
                if (!min)
                    min = abs;
                else if (abs <= min)
                    min = abs;
                else
                    continue;
                result = size;
            }
            return result;
        }
        function allFileType(folder, val) {
            folder.Files.Filters.FileType = val ? Main.config.fileTypes : [];
            folder.Files.IsFileTypeIndeterminate = false;
            Main.fileListFileTypeGetAnswer(folder);
        }
        function fileTypeChange(folder, val) {
            folder.Files.IsFileTypeIndeterminate = val.length != Main.config.fileTypes.length;
            Main.fileListFileTypeGetAnswer(folder);
        }
        function allStorageType(folder, val) {
            folder.Files.Filters.StorageType = val ? Main.config.storageTypes : [];
            folder.Files.IsStorageTypeIndeterminate = false;
            Main.fileListStorageTypeGetAnswer(folder);
        }
        function storageTypeChange(folder, val) {
            folder.Files.IsStorageTypeIndeterminate = val.length != Main.config.storageTypes.length;
            Main.fileListStorageTypeGetAnswer(folder);
        }
        function allFileState(folder, val) {
            folder.Files.Filters.State = val ? Main.config.fileStates : [];
            folder.Files.IsFileStateIndeterminate = false;
            Main.fileListFileStateGetAnswer(folder);
        }
        function fileStateChange(folder, val) {
            folder.Files.IsFileStateIndeterminate = val.length != Main.config.fileStates.length;
            Main.fileListFileStateGetAnswer(folder);
        }
        function fileListDateRangeChange(folder, val) {
            Main.fileListDataRangeGetAnswer(folder);
        }
        function fileListNameChange(folder, val) {
            Main.fileListNameGetAnswer(folder);
        }
        function fileListExtensionChange(folder, val) {
            Main.fileListExtensionGetAnswer(folder);
        }
        function fileListServerKeyChange(folder, val) {
            Main.fileListServerKeyGetAnswer(folder);
        }
        function fileListContentTypeChange(folder, val) {
            Main.fileListContentTypeGetAnswer(folder);
        }
        function fileListMD5Change(folder, val) {
            Main.fileListMD5GetAnswer(folder);
        }
        function updateFileListPagination(folder) {
            for (var field in folder.Files.Filters) {
                var _continue = false;
                var value = folder.Files.Filters[field];
                for (var i = 0; i < folder.Files.Pagination.DynamicFilterInfo.length; i++) {
                    var filter = folder.Files.Pagination.DynamicFilterInfo[i];
                    if (filter.LocalState == field) {
                        if (!value) {
                            folder.Files.Pagination.DynamicFilterInfo.splice(i, 1);
                            i--;
                        }
                        else
                            filter.Value = value;
                        _continue = true;
                        break;
                    }
                }
                if (!_continue && value) {
                    var filter = field == 'DateRang' ?
                        {
                            LocalState: field,
                            Relation: "and",
                            DynamicFilterInfo: [
                                {
                                    Field: 'CreateTime',
                                    Value: Dayjs(value[0]).format('YYYY-MM-DD HH:mm:ss'),
                                    Compare: "ge"
                                },
                                {
                                    Field: 'CreateTime',
                                    Value: Dayjs(value[1]).format('YYYY-MM-DD HH:mm:ss'),
                                    Compare: "le"
                                }
                            ]
                        } :
                        {
                            LocalState: field,
                            Field: field,
                            Value: value,
                            Compare: Array.isArray(value) ? "inSet" : "in"
                        };
                    folder.Files.Pagination.DynamicFilterInfo.push(filter);
                }
            }
            getFileList(folder);
        }
        function getFileList(folder) {
            folder.Files.Loading = true;
            Axios.post(ApiUri.GetFileList, folder.Files.Pagination).then(function (response) {
                if (response.data.Success) {
                    folder.Files.Pagination.PageIndex = response.data.Data.PageIndex;
                    folder.Files.Pagination.PageRows = response.data.Data.PageSize;
                    folder.Files.Pagination.PageCount = response.data.Data.PageTotal;
                    folder.Files.Pagination.RecordCount = response.data.Data.Total;
                    folder.Files.List = response.data.Data.List;
                    if (!folder.Files.Init)
                        folder.Files.Init = true;
                }
                else {
                    folder.Files.Error = response.data.Message;
                    ElementPlus.ElMessage(response.data.Message);
                }
                folder.Files.Loading = false;
            }).catch(function (error) {
                folder.Files.Loading = false;
                folder.Files.Error = error.message;
                ElementPlus.ElMessage('获取文件列表时发生异常.');
            });
        }
        function fileSort(val) {
            var folder = Main.library.currentOpenFolder;
            if (val.prop == null)
                folder.Files.Pagination.AdvancedSort = [];
            else if (!val.order)
                folder.Files.Pagination.AdvancedSort = folder.Files.Pagination.AdvancedSort.filter(function (data) { return data.Field != val.prop; });
            else {
                var order = val.order == 'descending' ? "desc" : "asc";
                for (var _i = 0, _a = folder.Files.Pagination.AdvancedSort; _i < _a.length; _i++) {
                    var sort = _a[_i];
                    if (sort.Field == val.prop) {
                        sort.Type = order;
                        getFileList(folder);
                        return;
                    }
                }
                folder.Files.Pagination.AdvancedSort.push({ Field: val.prop, Type: order });
            }
            getFileList(folder);
        }
        function fileListSizeChange(val) {
            var folder = Main.library.currentOpenFolder;
            folder.Files.Pagination.PageRows = val;
            getFileList(folder);
        }
        function fileListCurrentChange(val) {
            var folder = Main.library.currentOpenFolder;
            folder.Files.Pagination.PageIndex = val;
            getFileList(folder);
        }
        function fileDetail(row, index) {
            Main.library.fileDetail.show = true;
            Main.library.fileDetail.loading = true;
            Axios.get(ApiUri.GetFileDetail(row.Id))
                .then(function (response) {
                if (response.data.Success) {
                    Main.library.fileDetail.detail = response.data.Data;
                }
                else {
                    ElementPlus.ElMessage(response.data.Message);
                }
                Main.library.fileDetail.loading = false;
            })
                .catch(function (error) {
                Main.library.fileDetail.loading = false;
                ElementPlus.ElMessage('获取文件详情时发生异常.');
            });
        }
        function videoInfo(data) {
            Main.library.videoInfo.show = true;
            if (Main.library.videoInfo.fileId === data.Id)
                return;
            Main.library.videoInfo.loading = true;
            Axios.get(ApiUri.GetVideoInfo(data.Id, true, true, true, true, true))
                .then(function (response) {
                if (response.data.Success) {
                    Main.library.videoInfo.fileId = data.Id;
                    Main.library.videoInfo.detail = response.data.Data;
                    Main.library.videoInfo.activeTab = 'Streams';
                }
                else {
                    Main.library.videoInfo.error = response.data.Message;
                    ElementPlus.ElMessage(response.data.Message);
                }
                Main.library.videoInfo.loading = false;
            })
                .catch(function (error) {
                Main.library.videoInfo.loading = false;
                ElementPlus.ElMessage('获取视频信息时发生异常.');
            });
        }
        function filePreviewStart(data, index) {
            Main.library.previewImages[index] = ApiUri.Preview(data.Id);
            if (data.FileType === "\u89C6\u9891") {
                Main.library.videoInfo.previewId = data.Id;
                Main.library.videoInfo.previewState = true;
            }
        }
        function videoPreviewNext(data, index) {
            if (Main.library.videoInfo.previewId != data.Id)
                return;
            if (data.FileType === "\u89C6\u9891") {
                if (!Main.library.videoInfo.previewState)
                    return;
                Main.library.videoInfo.previewTicks = setTimeout(videoPreview, 200, data, index);
            }
        }
        function videoPreview(data, index) {
            if (data.FileType === "\u89C6\u9891") {
                if (!Main.library.videoInfo.previewState)
                    return;
                Axios.get(ApiUri.Preview(data.Id, 500, 500, Main.library.videoInfo.previewTimespan[0].toString().padStart(2, '0') + ":" + Main.library.videoInfo.previewTimespan[1].toString().padStart(2, '0') + ":" + Main.library.videoInfo.previewTimespan[2].toString().padStart(2, '0') + "." + Main.library.videoInfo.previewTimespan[3].toString().padStart(3, '0')), {
                    responseType: "blob",
                    onDownloadProgress: function (progressEvent) {
                    }
                }).then(function (response) {
                    if (response.status !== 200)
                        throw new Error('请求图片失败.');
                    if (response.headers['content-length'] === '0') {
                        Main.library.videoInfo.previewTimespan = [0, 0, 0, 1];
                    }
                    else {
                        Main.library.videoInfo.previewTimespan[3] += 500;
                        if (Main.library.videoInfo.previewTimespan[3] >= 999) {
                            Main.library.videoInfo.previewTimespan[3] = 1;
                            Main.library.videoInfo.previewTimespan[2]++;
                            if (Main.library.videoInfo.previewTimespan[2] >= 59) {
                                Main.library.videoInfo.previewTimespan[2] = 0;
                                Main.library.videoInfo.previewTimespan[1]++;
                                if (Main.library.videoInfo.previewTimespan[1] >= 59) {
                                    Main.library.videoInfo.previewTimespan[1] = 0;
                                    Main.library.videoInfo.previewTimespan[0]++;
                                }
                            }
                        }
                        var blob = new Blob([response.data], { type: response.headers['content-type'] });
                        Main.library.previewImages[index] = URL.createObjectURL(blob);
                    }
                }).catch(function (error) {
                    console.error(error);
                    ElementPlus.ElMessage('获取视频图像时发生异常.');
                    filePreviewEnd(data, index);
                });
            }
        }
        function filePreviewEnd(data, index) {
            if (data.FileType === "\u89C6\u9891") {
                clearTimeout(Main.library.videoInfo.previewTicks);
                Main.library.videoInfo.previewState = false;
            }
        }
        function getFileStateTag(state) {
            switch (state) {
                case '未上传':
                    return 'info';
                case '上传中':
                case '处理中':
                    return 'warning';
                case '可用':
                default:
                    return 'primary';
                case '已删除':
                    return 'danger';
            }
        }
        function handleFileListCommand(cmd, data, index) {
            if (index === void 0) { index = null; }
            switch (cmd) {
                case 'detail':
                    fileDetail(data, index);
                    break;
                case 'videoInfo':
                    videoInfo(data);
                    break;
                case 'preview':
                    previewFile(data, index);
                    break;
                case 'browse':
                    browseFile(data, index);
                    break;
                case 'download':
                    downloadFile(data, index);
                    break;
                case 'delete':
                    deleteFile(data, index);
                    break;
                default:
            }
        }
        function closeFileDetail() {
            Main.library.fileDetail.show = false;
            Main.library.fileDetail.detail = {};
        }
        function previewFile(data, index) {
            if (index === void 0) { index = null; }
            window.open(ApiUri.Preview(data.Id));
        }
        function browseFile(data, index) {
            if (index === void 0) { index = null; }
            window.open(ApiUri.Browse(data.Id));
        }
        function downloadFile(data, index) {
            if (index === void 0) { index = null; }
            window.open(ApiUri.Download(data.Id));
        }
        function deleteFile(data, index) {
            if (index === void 0) { index = null; }
            var folder = Main.library.currentOpenFolder;
            folder.Files.Loading = true;
            Axios.post(ApiUri.Delete, [data.Id]).then(function (response) {
                if (response.data.Success)
                    getFileList(folder);
                else {
                    ElementPlus.ElMessage(response.data.Message);
                    folder.Files.Loading = false;
                }
            }).catch(function (error) {
                folder.Files.Loading = false;
                ElementPlus.ElMessage('删除文件时发生异常.');
            });
        }
    });
};
//# sourceMappingURL=Index.js.map