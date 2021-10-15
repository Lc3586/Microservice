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
                href: '../../element-plus/theme-chalk/element-plus.index.css'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../vue/vue.global.prod.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../utils/axios.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../element-plus/element-plus.index.full.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../element-plus/element-plus.index.full.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../element-plus/element-plus.es.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../element-plus/element-plus.zh-cn.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../utils/dayjs/dayjs.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../utils/dayjs/dayjs.zh-cn.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: '../../utils/lodash.min.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: 'NaiveUpload.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: 'Model/MultipleUploadSetting.js'
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
                src: 'Helper/ChunkFileMergeTaskHelper.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: 'Model/UploadConfigDetail.js'
            }
        },
        {
            Tag: "script",
            Attributes: {
                type: 'text/javascript',
                src: 'Model/Pagination.js'
            }
        },
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
                Main.fileNameGetAnswer = _.debounce(getFileList, 500);
                Main.fileExtensionGetAnswer = _.debounce(getFileList, 500);
                Main.fileContentTypeGetAnswer = _.debounce(getFileList, 500);
                Main.fileMD5GetAnswer = _.debounce(getFileList, 500);
                Main.fileServerKeyGetAnswer = _.debounce(getFileList, 500);
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
                GetFolderCLass: GetFolderCLass,
                folderFileList: folderFileList,
                allFileType: allFileType,
                fileTypeChange: fileTypeChange,
                allStorageType: allStorageType,
                storageTypeChange: storageTypeChange,
                allFileState: allFileState,
                fileStateChange: fileStateChange,
                fileSort: fileSort,
                fileListSizeChange: fileListSizeChange,
                fileListCurrentChange: fileListCurrentChange,
                fileListRowClassName: fileListRowClassName,
                fileDetail: getFileDetail,
                closeFileDetail: closeFileDetail,
                previewFile: previewFile,
                browseFile: browseFile,
                downloadFile: downloadFile,
                deleteFile: deleteFile
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
                },
                'library.file.date': function (newValue, oldValue) {
                    if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                        return;
                    getFileList();
                },
                'library.file.name': function (newValue, oldValue) {
                    this.fileNameGetAnswer();
                },
                'library.file.extension': function (newValue, oldValue) {
                    this.fileExtensionGetAnswer();
                },
                'library.file.contentType': function (newValue, oldValue) {
                    this.fileContentTypeGetAnswer();
                },
                'library.file.md5': function (newValue, oldValue) {
                    this.fileMD5GetAnswer();
                },
                'library.file.serverKey': function (newValue, oldValue) {
                    this.fileServerKeyGetAnswer();
                }
            }
        };
        Upload.Init(MultipleUploadSettings).then(function () { return __awaiter(_this, void 0, void 0, function () {
            var e_1, App;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4, Upload.UpdateConfig('6D9A0000-F269-0025-F631-08D98FA532D4')];
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
                    search: '',
                    folders: [],
                    error: '',
                    loading: true,
                    file: {
                        init: false,
                        loading: true,
                        error: '',
                        list: [],
                        sorts: [],
                        currentPage: 1,
                        pageSize: 15,
                        pageTotal: 0,
                        total: 0,
                        dataRangShortcuts: [
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
                        ],
                        fileTypes: [],
                        checkAllFileType: true,
                        isFileTypeIndeterminate: false,
                        storageTypes: [],
                        checkAllStorageType: true,
                        isStorageTypeIndeterminate: false,
                        fileStates: [],
                        checkAllFileState: true,
                        isFileStateIndeterminate: false,
                        date: [new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7), new Date()],
                        name: '',
                        md5: '',
                        contentType: '',
                        extension: '',
                        serverKey: '',
                        detail: {
                            show: false,
                            loading: false
                        }
                    }
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
                if (fileTypes.data.Success) {
                    Main.config.fileTypes = fileTypes.data.Data;
                    Main.library.file.fileTypes = fileTypes.data.Data;
                    Main.library.file.checkAllFileType = true;
                    Main.library.file.isFileTypeIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(fileTypes.data.Message);
                if (storageTypes.data.Success) {
                    Main.config.storageTypes = storageTypes.data.Data;
                    Main.library.file.storageTypes = storageTypes.data.Data;
                    Main.library.file.checkAllStorageType = true;
                    Main.library.file.isStorageTypeIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(storageTypes.data.Message);
                if (fileStates.data.Success) {
                    Main.config.fileStates = fileStates.data.Data;
                    Main.library.file.fileStates = fileStates.data.Data;
                    Main.library.file.checkAllFileState = true;
                    Main.library.file.isFileStateIndeterminate = false;
                }
                else
                    ElementPlus.ElMessage(fileStates.data.Message);
            }))
                .catch(function (error) {
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
                        folder.Class = GetFolderClassByType(folder.FileType);
                        folder.Pagination = new Pagination();
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
        function GetFolderCLass(folder) {
            return "folder " + (folder.Hover ? ' hover' : '');
        }
        function folderFileList(folder) {
            console.info(folder.FileType);
            Main.library.file.pageSize = getPageSize();
            getFileList();
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
        function allFileType(val) {
            Main.library.file.fileTypes = val ? Main.config.fileTypes : [];
            Main.library.file.isFileTypeIndeterminate = false;
            getFileList();
        }
        function fileTypeChange(val) {
            getFileList();
        }
        function allStorageType(val) {
            Main.library.file.storageTypes = val ? Main.config.storageTypes : [];
            Main.library.file.isStorageTypeIndeterminate = false;
            getFileList();
        }
        function storageTypeChange(val) {
            getFileList();
        }
        function allFileState(val) {
            Main.library.file.fileStates = val ? Main.config.fileStates : [];
            Main.library.file.isFileStateIndeterminate = false;
            getFileList();
        }
        function fileStateChange(val) {
            getFileList();
        }
        function getFileListParams(target) {
            var params = {
                PageIndex: target.currentPage,
                PageRows: target.pageSize,
                AdvancedSort: [
                    {
                        Field: "CreateTime",
                        Type: "desc"
                    }
                ],
                DynamicFilterInfo: []
            };
            if (target.sorts.length > 0)
                params.AdvancedSort = target.sorts;
            if (target.fileTypes.length != 0 && target.fileTypes.length != Main.config.fileTypes.length)
                params.DynamicFilterInfo.push({
                    Field: 'FileType',
                    Value: target.fileTypes,
                    Compare: 'inSet'
                });
            if (target.storageTypes.length != 0 && target.storageTypes.length != Main.config.storageTypes.length)
                params.DynamicFilterInfo.push({
                    Field: 'StorageType',
                    Value: target.storageTypes,
                    Compare: 'inSet'
                });
            if (target.name && Main.library.file.name.length > 0)
                params.DynamicFilterInfo.push({
                    Field: 'Name',
                    Value: target.name,
                    Compare: 'in'
                });
            if (target.md5 && Main.library.file.md5.length > 0)
                params.DynamicFilterInfo.push({
                    Field: 'MD5',
                    Value: target.md5,
                    Compare: 'in'
                });
            if (target.contentType && Main.library.file.contentType.length > 0)
                params.DynamicFilterInfo.push({
                    Field: 'ContentType',
                    Value: target.contentType,
                    Compare: 'in'
                });
            if (target.extension && Main.library.file.extension.length > 0)
                params.DynamicFilterInfo.push({
                    Field: 'Extension',
                    Value: target.extension,
                    Compare: 'in'
                });
            if (target.serverKey && Main.library.file.serverKey.length > 0)
                params.DynamicFilterInfo.push({
                    Field: 'ServerKey',
                    Value: target.serverKey,
                    Compare: 'in'
                });
            if (target.date && target.date.length == 2)
                params.DynamicFilterInfo.push({
                    Relation: 'and',
                    DynamicFilterInfo: [
                        {
                            Field: 'CreateTime',
                            Value: Dayjs(target.date[0]).format('YYYY-MM-DD HH:mm:ss'),
                            Compare: 'ge'
                        },
                        {
                            Field: 'CreateTime',
                            Value: Dayjs(target.date[1]).format('YYYY-MM-DD HH:mm:ss'),
                            Compare: 'le'
                        }
                    ]
                });
            return params;
        }
        function getFileList() {
            Main.library.file.loading = true;
            Axios.post(ApiUri.GetFileList, getFileListParams(Main.library.file)).then(function (response) {
                if (response.data.Success) {
                    Main.library.file.currentPage = response.data.Data.PageIndex;
                    Main.library.file.pageSize = response.data.Data.PageSize;
                    Main.library.file.pageTotal = response.data.Data.PageTotal;
                    Main.library.file.total = response.data.Data.Total;
                    Main.library.file.list = response.data.Data.List;
                }
                else {
                    Main.library.file.error = response.data.Message;
                    ElementPlus.ElMessage(response.data.Message);
                }
                Main.library.file.loading = false;
            }).catch(function (error) {
                Main.library.file.loading = false;
                Main.library.file.error = error.message;
                ElementPlus.ElMessage('获取文件列表时发生异常.');
            });
            Main.library.file.init = true;
        }
        function fileSort(val) {
            if (val.prop == null)
                Main.library.file.sorts = [];
            else if (!val.order)
                Main.library.file.sorts = Main.library.file.sorts.filter(function (data) { return data.field != val.prop; });
            else {
                for (var item in Main.library.file.sorts) {
                    if (Main.library.file.sorts[item].field == val.prop) {
                        Main.library.file.sorts[item].type = val.order == 'descending' ? 'desc' : 'asc';
                        getFileList();
                        return;
                    }
                }
                Main.library.file.sorts.push({ field: val.prop, type: val.order == 'descending' ? 'desc' : 'asc' });
            }
            getFileList();
        }
        function fileListSizeChange(val) {
            Main.library.file.pageSize = val;
            getFileList();
        }
        function fileListCurrentChange(val) {
            Main.library.file.currentPage = val;
            getFileList();
        }
        function fileListRowClassName(_a) {
            var row = _a.row, rowIndex = _a.rowIndex;
            return '';
            switch (row.Level) {
                case 'Trace':
                case 'Debug':
                case 'Info':
                default:
                    return '';
                case 'Warn':
                    return 'warning-row';
                case 'Error':
                case 'Fatal':
                    return 'error-row';
            }
        }
        function getFileDetail(index, row) {
            Main.library.file.detail.show = true;
            Main.library.file.detail.loading = true;
            Axios.get(ApiUri.GetFileDetail(row.Id))
                .then(function (response) {
                if (response.data.Success) {
                    Main.library.file.detail.StateTag = getFileStateTag(response.data.Data.State);
                    for (var item in response.data.Data) {
                        Main.library.file.detail[item] = response.data.Data[item];
                    }
                }
                else {
                    ElementPlus.ElMessage(response.data.Message);
                }
                Main.library.file.detail.loading = false;
            })
                .catch(function (error) {
                Main.library.file.detail.loading = false;
                ElementPlus.ElMessage('获取文件详情时发生异常.');
            });
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
        function closeFileDetail() {
            Main.library.file.detail.show = false;
            for (var item in Main.library.file.detail) {
                if (item != 'show' && item != 'loding')
                    Main.library.file.detail[item] = '';
            }
        }
        function previewFile(index, data) {
            window.open(ApiUri.Preview(data.Id));
        }
        function browseFile(index, data) {
            window.open(ApiUri.Browse(data.Id));
        }
        function downloadFile(index, data) {
            window.open(ApiUri.Download(data.Id));
        }
        function deleteFile(index, data) {
            Main.library.file.loading = true;
            Axios.post(ApiUri.Delete, [data.Id]).then(function (response) {
                if (response.data.Success)
                    getFileList();
                else {
                    ElementPlus.ElMessage(response.data.Message);
                    Main.library.file.loading = false;
                }
            }).catch(function (error) {
                Main.library.file.loading = false;
                ElementPlus.ElMessage('删除文件时发生异常.');
            });
        }
    });
};
//# sourceMappingURL=Index.js.map