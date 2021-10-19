window.onload = () => {
    ImportHelper.ImportFile(
        [
            {
                Tag: ImportFileTag.CSS,
                Attributes: {
                    type: 'text/css',
                    rel: 'stylesheet',
                    href: '../../element-plus/theme-chalk/element-plus.index.css'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../vue/vue.global.prod.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../utils/axios.min.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../element-plus/element-plus.index.full.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../element-plus/element-plus.index.full.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../element-plus/element-plus.es.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../element-plus/element-plus.zh-cn.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../utils/dayjs/dayjs.min.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../utils/dayjs/dayjs.zh-cn.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: '../../utils/lodash.min.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'NaiveUpload.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Model/MultipleUploadSetting.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Model/ApiUri.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Helper/ChunkFileMergeTaskHelper.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Model/UploadConfigDetail.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Model/Pagination.js'
                }
            },
            {
                Tag: ImportFileTag.JS,
                Attributes: {
                    type: 'text/javascript',
                    src: 'Model/LibraryInfo.js'
                }
            }
        ],
        () => {
            const Vue = (<any>window).Vue;

            const ElementPlus = (<any>window).ElementPlus;

            const Axios = (<any>window).axios;

            const Promise = (<any>window).Promise;

            const _ = (<any>window)._;

            const Dayjs = (<any>window).dayjs;

            /**
             * 相当于vue app中的this对象
             * */
            let Main: any;

            /**
             * 文件批量上传设置
             * */
            let MultipleUploadSettings = new MultipleUploadSetting();
            MultipleUploadSettings.Axios = Axios;
            MultipleUploadSettings.Mode = UploadMode.手自一体;

            MultipleUploadSettings.AfterCheckAll = async (files: RawFile[]) => {
                ElementPlus.ElMessage('所有文件校验完毕.');
            }

            MultipleUploadSettings.AfterUploadAll = async (files: RawFile[]) => {
                ElementPlus.ElMessage('所有文件上传完毕.');
            }

            /**
             * 上传组件
             * */
            const Upload: NaiveUpload = new NaiveUpload();

            /**
             * 应用数据
             */
            const AppData = {
                data: GetRenderData,
                created() {
                    Main = this;

                    Upload.SelectedFileList = this.multipleUpload.files;

                    AddLoginFilter();
                    Authorized();

                    getConfig();

                    //延迟响应用户的搜索条件更改事件
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
                    SALogin,
                    TabsChanged,

                    LoadConfig,
                    FilterConfig,
                    ConfigDetail,
                    ApplyConfig,

                    GetSelectCLass,
                    GetSelectTitle,
                    ChosingFile,
                    AllowDrop,
                    DropFile,
                    ChoseFile,

                    GetListCLass,
                    GetContainerClass,
                    GetContainerTitle,
                    GetFilename,
                    ShowTools,
                    ShowLoading,
                    GetLoadingStyle,
                    GetLoadingTitle,
                    EnableRename,
                    Rename,
                    RenameKeydown,
                    RenameDone,
                    EnableView,
                    View,
                    Remove,

                    Start,
                    Pause,
                    Continues,
                    Clean,

                    GetFolderClass,

                    OpenFolder,
                    CloseFolder,
                    ActiveCollapseChange,
                    fileListNameChange,
                    fileListExtensionChange,
                    fileListServerKeyChange,
                    fileListContentTypeChange,
                    fileListMD5Change,
                    handleFileListCommand,

                    allFileType,
                    fileTypeChange,
                    allStorageType,
                    storageTypeChange,
                    allFileState,
                    fileStateChange,
                    fileListDateRangeChange,
                    fileSort,
                    fileListSizeChange,
                    fileListCurrentChange,
                    fileDetail,
                    getFileStateTag,
                    closeFileDetail,
                    previewFile,
                    browseFile,
                    downloadFile,
                    deleteFile
                },
                watch: {
                    /**
                     * 监听设置变化
                     */
                    'multipleUpload.settings': {
                        handler(val) {
                            for (const key in val) {
                                if (key !== 'Config' && MultipleUploadSettings[key] !== val[key])
                                    MultipleUploadSettings[key] = val[key];
                            }
                        },
                        deep: true
                    },
                    /**
                     * 文件状态变化时自动滚动列表, 
                     * 以显示正在上传的文件
                     */
                    'multipleUpload.files': {
                        handler(files: SelectedFile[]) {
                            if (!this.multipleUpload.scrollLock)
                                return;

                            if (files.length === 0)
                                return;

                            this.$nextTick(() => {
                                let container: HTMLDivElement;

                                let findChecking = false;

                                for (let i = 0; i < files.length; i++) {
                                    let file = files[i];

                                    if (!findChecking && file.Checking) {
                                        findChecking = true;
                                        container = this.$refs['file_container_' + i];
                                    }

                                    if (file.Uploading) {
                                        container = this.$refs['file_container_' + i];
                                        break;
                                    }
                                }

                                if (!container)
                                    return;

                                this.$refs.uploadList.scrollTop = container.offsetTop - this.$refs.uploadList.offsetTop - 20;
                            });
                        },
                        deep: true
                    },
                    'multipleUpload.config.filter': {
                        handler(val) {
                            this.$refs.configTree.filter(val);
                        }
                    }
                }
            };

            Upload.Init(MultipleUploadSettings).then(async () => {
                //设置默认配置
                try {
                    await Upload.UpdateConfig('6D9A0000-F269-0025-F631-08D98FA532D4');
                } catch (e) {
                    console.error(e);
                    ElementPlus.ElMessage(e.message);
                }

                const App = Vue.createApp(AppData);
                App.use(ElementPlus, {
                    locale: ElementPlus.lang.zhCn
                });
                App.mount("#app");
            });

            /**
             * 获取渲染数据
             * 需要双向绑定的数据
             */
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
                                value: (() => {
                                    const end = new Date()
                                    const start = new Date()
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
                                    return [start, end]
                                })(),
                            },
                            {
                                text: '最近一个月',
                                value: (() => {
                                    const end = new Date()
                                    const start = new Date()
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 30)
                                    return [start, end]
                                })(),
                            },
                            {
                                text: '最近三个月',
                                value: (() => {
                                    const end = new Date()
                                    const start = new Date()
                                    start.setTime(start.getTime() - 3600 * 1000 * 24 * 90)
                                    return [start, end]
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
                        /**
                         * 锁定滚动条
                         */
                        scrollLock: true,
                        /*文件集合*/
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
                        }
                    }
                };
            }

            /**
             * 添加登录拦截器
             * */
            function AddLoginFilter() {
                Axios.interceptors.response.use((response) => {
                    return response;
                }, (error) => {
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

            /**
             * 登录验证
             * */
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

            /**
             * SA身份验证
             * */
            function SALogin() {
                Main.sa.loading = true;
                let data = {
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

            /**
             * 获取令牌
             * */
            function GetToken(data: any) {
                Axios.post(ApiUri.GetToken, data).then(function (response: { data: ResponseData_T<TokenInfo> }) {
                    Main.sa.loading = false;
                    if (response.data.Success) {
                        MultipleUploadSettings.Headers['Authorization'] = response.data.Data.AccessToken;
                        //定时更新令牌
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

            /**
             * 刷新令牌
             * */
            function RefreshToken() {
                Axios.post(ApiUri.RefreshToken).then(function (response: { data: ResponseData_T<TokenInfo> }) {
                    if (response.data.Success) {
                        MultipleUploadSettings.Headers['Authorization'] = response.data.Data.AccessToken;

                        //定时更新令牌
                        setTimeout(RefreshToken, new Date(response.data.Data.Expires).getTime() - new Date().getTime() - 60 * 1000);
                    }
                    else
                        ElementPlus.ElMessage(response.data.Message);
                }).catch(function (error) {
                    console.error(error);
                    ElementPlus.ElMessage('刷新Token时发生异常.');
                });
            }

            /**
             * 获取配置
             * */
            function getConfig() {
                Axios.all([getFileTypes(), getStorageTypes(), getFileStates()])
                    .then(Axios.spread((fileTypes, storageTypes, fileStates) => {
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
                    .catch((error) => {
                        console.error(error);
                        Main.loading = false;
                        if (!Main.sa.show)
                            ElementPlus.ElMessage('获取配置时发生异常.');
                    });
            }

            /**
             * 获取所有文件类型
             * 
             * */
            function getFileTypes() {
                return Axios.get(ApiUri.GetFileTypes);
            }

            /**
             * 获取所有文件存储类型
             * 
             * */
            function getStorageTypes() {
                return Axios.get(ApiUri.GetStorageTypes);
            }

            /**
             * 获取所有文件状态
             * 
             * */
            function getFileStates() {
                return Axios.get(ApiUri.GetFileStates);
            }

            /**
             * 加载文件上传配置下拉选择框数据
             * @param node
             * @param resolve
             */
            function LoadConfig(node, resolve) {
                Axios.post(ApiUri.GetCurrentAccountCFUCTree,
                    {
                        ParentId: node.data?.Id,
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
                    }).then(function (response: { data: ResponseData_T<UploadConfigAuthoritiesTreeList[]> }) {
                        if (response.data.Success) {
                            response.data.Data.forEach((item, index) => {
                                (<any>item).Leaf = !item.HasChildren;
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

            /**
             * 删选文件上传配置
             * @param value
             * @param data
             */
            function FilterConfig(value, data) {
                if (!value) return true
                return data.Name.indexOf(value) !== -1 || data.Code.indexOf(value) !== -1 || data.DisplayName.indexOf(value) !== -1;
            }

            /**
             * 文件上传配置详情
             * @param node
             * @param data
             */
            function ConfigDetail(node, data) {
                if (node.detailLoading === false)
                    return;
                node.detailLoading = true;
                Axios.post(ApiUri.FileUploadConfigDetailData(data.Id))
                    .then(function (response: { data: ResponseData_T<UploadConfigDetail[]> }) {
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

            /**
             * 应用文件上传配置
             * @param node
             * @param data
             */
            function ApplyConfig(node, data) {
                const done = (_data, updated) => {
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
                } else if (node.configLoading === false) {
                    done(node.configData, false);
                    return;
                }

                node.configLoading = true;
                Upload.UpdateConfig(data.Id)
                    .then((_data) => {
                        node.configData = _data;
                        done(_data, true);
                    })
                    .catch(error => {
                        console.error(error);
                        ElementPlus.ElMessage(`应用文件上传配置失败, ${error.message}`);
                    });
            }

            /**
             * 切换标签时的点击事件
             * @param {any} tab
             */
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

            /**
             * 获取列表类名
             * */
            function GetListCLass() {
                return `upload-${Main.multipleUpload.settings.Theme}-list`;
            }

            /**
             * 获取文件选择框类名
             * */
            function GetSelectCLass() {
                return `upload-drag ${(Upload.Limited() ? 'item-limited' : '')}`;
            }

            /**
             * 获取文件选择框的提示语
             * */
            function GetSelectTitle() {
                if (!MultipleUploadSettings.Config.UpperLimit)
                    return null;

                return Upload.SelectedFileList.length < MultipleUploadSettings.Config.UpperLimit ? `还可添加个${MultipleUploadSettings.Config.UpperLimit - Upload.SelectedFileList.length}文件` : `文件数量已达上限`;
            }

            /**
             * 选择文件
             * @param {any} e
             * */
            function ChosingFile(e) {
                if (Upload.Limited())
                    return;

                Main.$refs.fileInput.click();
            }

            /**
             * 允许拖动
             * @param {any} e
             */
            function AllowDrop(e) {
                e.preventDefault();
            }

            /**
             * 拖动文件
             * @param {any} e
             */
            function DropFile(e) {
                e.preventDefault();
                CheckAppendFilesResult(Upload.AppendFiles(e.dataTransfer.files));
            }

            /**
             * 已选择文件
             * @param {any} e
             */
            function ChoseFile(e) {
                CheckAppendFilesResult(Upload.AppendFiles(Main.$refs.fileInput.files));
            }

            function CheckAppendFilesResult(status: AppendFileResultStatus) {
                switch (status) {
                    case AppendFileResultStatus.成功:

                        break;
                    case AppendFileResultStatus.文件类型不合法:
                        ElementPlus.ElMessage('文件类型不合法');
                        break;
                    case AppendFileResultStatus.超出数量限制:
                        ElementPlus.ElMessage('超出数量限制');
                        break;
                    default:
                }
            }

            /**
             * 获取容器类名
             * @param {any} selectedFile
             */
            function GetContainerClass(selectedFile: SelectedFile) {
                return `item-container ${(selectedFile.Done ? ' item-done' : '')} ${(selectedFile.Error ? ' item-error' : '')} ${(selectedFile.Hover && !selectedFile.Rename && !selectedFile.Checking && !selectedFile.Uploading ? ' item-hover' : '')} ${(selectedFile.Checking ? ' item-checking' : '')} ${(selectedFile.Uploading ? ' item-uploading' : '')}`;
            }

            /**
             * 获取容器提示语
             * @param {any} selectedFile
             */
            function GetContainerTitle(selectedFile: SelectedFile) {
                return `${(selectedFile.Done ? '上传成功' : '')} ${(selectedFile.Error ? selectedFile.ErrorMessage : '')}`;
            }

            /**
             * 获取文件名
             * @param {any} selectedFile
             */
            function GetFilename(selectedFile: SelectedFile) {
                return (selectedFile.Name || '-') + (selectedFile.Extension || '-');
            }

            /**
             * 是否显示工具栏
             * @param {any} selectedFile
             */
            function ShowTools(selectedFile: SelectedFile) {
                return selectedFile.Hover && !selectedFile.Rename && !selectedFile.Checking && !selectedFile.Uploading;
            }

            /**
             * 显示加载层
             * @param {any} selectedFile
             */
            function ShowLoading(selectedFile: SelectedFile) {
                return !selectedFile.Rename && (selectedFile.Checking || selectedFile.Uploading);
            }

            /**
             * 获取渐变色样式
             * @param {string} type 类型 conic锥形渐变,linear线性渐变
             * @param {string} color 颜色
             * @param {number} value1 值1
             * @param {number} value2 值2
             */
            function GetGradientStyle(type, color, value1, value2) {
                switch (type) {
                    default:
                    case 'conic':
                        return `
background: conic-gradient(${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -moz-conic-gradient(${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -o-conic-gradient(${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -webkit-conic-gradient(${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;`;
                    case 'linear':
                        return `
background: linear-gradient(left, ${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -moz-linear-gradient(left, ${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -o-linear-gradient(left, ${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;
background: -webkit-linear-gradient(left, ${color} ${value1}%, transparent ${value2}%)  repeat scroll 0% 0%;`;
                }
            }

            /**
             * 获取加载层样式
             * @param {any} selectedFile
             */
            function GetLoadingStyle(selectedFile: SelectedFile) {
                const styleType = MultipleUploadSettings.Theme === 'card' ? 'conic' : 'linear';

                return `${(selectedFile.Checking ? GetGradientStyle(styleType, 'rgba(255, 236, 201, 0.5)', selectedFile.Percent, 0) : '')} ${(selectedFile.Uploading ? GetGradientStyle(styleType, 'rgba(144, 206, 255, 0.5)', selectedFile.Percent, selectedFile.VirtualPercent) : '')}`;
            }

            /**
             * 获取加载层提示语
             * @param {any} selectedFile
             */
            function GetLoadingTitle(selectedFile: SelectedFile) {
                return `${(selectedFile.Checking ? ('扫描中...' + selectedFile.Percent + '%') : '')} ${(selectedFile.Uploading ? ('上传中...' + selectedFile.Percent + '%') : '')}`;
            }

            /**
             * 能否重命名
             * @param {any} selectedFile
             */
            function EnableRename(selectedFile: SelectedFile) {
                return !selectedFile.Uploading;
            }

            /**
             * 重命名
             * @param {any} selectedFile
             */
            function Rename(selectedFile: SelectedFile) {
                selectedFile.Rename = true;

                Main.$nextTick(_ => {
                    //Main.$refs.renameInput.focus();
                    (<HTMLInputElement>document.getElementsByClassName('item-rename-input')[0]).focus();
                }, 100);
            }

            /**
             * 确认重命名
             * @param {any} selectedFile
             * @param {any} event
             */
            function RenameKeydown(selectedFile: SelectedFile, event) {
                if (event.keyCode == 13) {
                    RenameDone(selectedFile);
                }
            }

            /**
             * 重命名结束
             * @param selectedFile
             */
            function RenameDone(selectedFile: SelectedFile) {
                Upload.Rename(selectedFile).catch(e => {
                    console.error(e);
                    ElementPlus.ElMessage(e.Message);
                });
            }

            /**
             * 能否查看
             * @param {any} selectedFile
             */
            function EnableView(selectedFile: SelectedFile) {
                switch (selectedFile.FileType) {
                    case FileType.图片:
                    case FileType.音频:
                        return selectedFile.ExtensionLower !== '.flac';
                    case FileType.视频:
                        return true;
                    case FileType.文本文件:
                        return true;
                    case FileType.电子文档:
                        return selectedFile.ExtensionLower === '.pdf';
                    default:
                        return false;
                }
            }

            /**
             * 查看
             * @param {any} selectedFile
             */
            function View(selectedFile: SelectedFile) {
                const file = Upload.GetRawFile(selectedFile);
                const bodyStyle = "margin:0px;text-align: center;display: flex;flex-direction: row;justify-content: center;align-items: center;";

                switch (selectedFile.FileType) {
                    case FileType.图片:
                        let winImage = window.open();
                        winImage.document.write(`<body style="${bodyStyle}background-color: black;"><img src="${file.ObjectURL}" /img></body>`);
                        break;
                    case FileType.音频:
                        if (selectedFile.ExtensionLower === '.flac')
                            return;

                        let winAudio = window.open();
                        winAudio.document.write(`<body style="${bodyStyle}background-color: black;"><audio src="${file.ObjectURL}" controls="controls">抱歉, 暂不支持</audio></body>`);
                        break;
                    case FileType.视频:
                        let winVideo = window.open();
                        winVideo.document.write(`<body style="${bodyStyle}background-color: black;"><video src="${file.ObjectURL}" controls="controls">抱歉, 暂不支持</video></body>`);
                        break;
                    default:
                        let win = window.open();
                        win.document.write(`<body style="${bodyStyle}"><object data="${file.ObjectURL}" type="${(selectedFile.ExtensionLower === '.txt' ? 'text/plain' : (selectedFile.ExtensionLower === '.pdf' ? 'application/pdf' : 'application/octet-stream'))}" width="100%" height="100%"><iframe src="${file.ObjectURL}" scrolling="no" width="100%" height="100%" frameborder="0" ></iframe></object></body>`);
                        break;
                }
            }

            /**
             * 删除
             * @param {number} index
             */
            function Remove(index) {
                Upload.Remove(index);
            }

            /**
             * 开始
             */
            function Start() {
                for (let i = 0; i < MultipleUploadSettings.ConcurrentFile; i++) {
                    Upload.Upload();
                }
            }

            /**
             * 暂停
             */
            function Pause() {
                Upload.Pause();
            }

            /**
             * 恢复
             */
            function Continues() {
                Upload.Continues();
            }

            /**
             * 清空
             */
            function Clean() {
                Upload.Clean();
            }

            /**
             * 初始化分片文件合并任务列表
             * */
            async function InitChunkFileMergeTaskList() {
                if (Main.chunkFileMergeTask.init)
                    return;

                Main.chunkFileMergeTask.init = true;

                const chunkFileMergeTaskHelper = new ChunkFileMergeTaskHelper();
                await chunkFileMergeTaskHelper.Init();

                chunkFileMergeTaskHelper.AddTask = async task => {
                    //console.info(task);

                    task.ChunksSources = [];
                    Main.chunkFileMergeTask.list.push(task);
                };

                chunkFileMergeTaskHelper.UpdateTask = async (id, data) => {
                    //console.info(id, data);
                    //console.info(Main.chunkFileMergeTask.list);

                    for (let task of Main.chunkFileMergeTask.list) {
                        if (task.Id !== id)
                            continue;

                        for (let key in data) {
                            task[key] = data[key];
                        }
                        break;
                    }
                };

                chunkFileMergeTaskHelper.RemoveTask = async md5 => {
                    for (let task of Main.chunkFileMergeTask.list) {
                        if (task.MD5 !== md5)
                            continue;

                        task.Remove = true;
                        break;
                    }
                };

                chunkFileMergeTaskHelper.UpdateChunksSource = async (md5, chunksSource) => {
                    //console.info(md5, chunksSource);

                    for (let task of Main.chunkFileMergeTask.list) {
                        if (task.MD5 !== md5)
                            continue;

                        for (let source of task.ChunksSources) {
                            if (source.Specs == chunksSource.Specs
                                && source.Total == chunksSource.Total) {
                                source.Activitys = chunksSource.Activitys;
                                return;
                            }
                        }

                        task.ChunksSources.push(chunksSource);
                        break;
                    }
                };

                await chunkFileMergeTaskHelper.Connect();

                Main.chunkFileMergeTask.loading = false;
            }

            /**
             * 获取文件库信息
             * @param node
             * @param data
             */
            function GetLibraryInfo() {
                Main.library.loading = true;
                Axios.get(ApiUri.GetLibraryInfo)
                    .then(function (response: { data: ResponseData_T<LibraryInfo[]> }) {
                        Main.library.loading = false;
                        if (response.data.Success) {
                            Main.library.folders = response.data.Data.map((item, index): FolderInfo => {
                                let folder = item as FolderInfo;
                                folder.Hover = false;
                                folder.Open = false;
                                folder.Class = GetFolderClassByType(folder.FileType);
                                folder.Files = new FileListInfo();
                                folder.Files.Filters.FileType = [folder.FileType]; //Main.config.fileTypes;
                                folder.Files.CheckAllFileType = false;
                                folder.Files.IsFileTypeIndeterminate = true;

                                folder.Files.Filters.StorageType = Main.config.storageTypes;
                                folder.Files.CheckAllStorageType = true;
                                folder.Files.IsStorageTypeIndeterminate = false;

                                folder.Files.Filters.State = Main.config.fileStates;
                                folder.Files.CheckAllFileState = true;
                                folder.Files.IsFileStateIndeterminate = false;

                                folder.Files.Pagination.AdvancedSort.push({ Field: 'CreateTime', Type: SortType.倒序 });

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

            /**
             * 获取文件类型对于的文件夹图标样式名
             * @param fileType
             */
            function GetFolderClassByType(fileType: string): string {
                switch (fileType) {
                    case FileType.音频:
                        return 'icon-folder-audio';
                    case FileType.视频:
                        return 'icon-folder-video';
                    case FileType.图片:
                        return 'icon-folder-picture';
                    case FileType.文本文件:
                        return 'icon-folder-text';
                    case FileType.电子表格:
                        return 'icon-folder-excel';
                    case FileType.电子文档:
                        return 'icon-folder-word';
                    case FileType.压缩包:
                        return 'icon-folder-zip';
                    case FileType.外链资源:
                        return 'icon-folder-uri';
                    case FileType.未知:
                    default:
                        return 'icon-folder-none';
                }
            }

            /**
             * 获取页面文件夹容器类名
             * @param folder
             */
            function GetFolderClass(folder: FolderInfo) {
                return `folder ${(folder.Hover ? ' hover' : '')}`;
            }

            /**
             * 打开文件夹
             * @param folder
             * @param index
             */
            function OpenFolder(folder: FolderInfo, index: number) {
                Main.library.currentOpenFolder = folder;
                folder.Files.Pagination.PageRows = getPageSize();
                folder.Open = true;
                folder.Files.Filters.FileType = [folder.FileType];
                Main.library.activeCollapse = 'files';

                if (!folder.Files.Init)
                    getFileList(folder);
            }

            /**
             * 关闭文件夹
             * @param folder
             * @param index
             */
            function CloseFolder(folder: FolderInfo, index: number) {
                folder.Open = false;
                Main.library.currentOpenFolder = null;
            }

            /**
             * 文件库折叠面板变更事件
             * @param val
             */
            function ActiveCollapseChange(val) {
                if (val !== 'folders')
                    return;

                if (!Main.library.currentOpenFolder)
                    return;

                Main.library.currentOpenFolder.Open = false;
                Main.library.currentOpenFolder = null;
            }

            /**
             * 根据页面大小调整页面数据量
             * */
            function getPageSize(): number {
                let current = window.innerHeight / 100,
                    min,
                    result;
                for (const i in Main.config.page.sizes) {
                    let size = Main.config.page.sizes[i];
                    let abs = Math.abs(current - size);
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

            /**
             * 文件列表所有文件类型
             * @param folder
             * @param val
             * */
            function allFileType(folder: FolderInfo, val) {
                folder.Files.Filters.FileType = val ? Main.config.fileTypes : [];
                folder.Files.IsFileTypeIndeterminate = false;
                Main.fileListFileTypeGetAnswer(folder);
            }

            /**
             * 文件列表更改文件类型
             * @param folder
             * @param val
             * */
            function fileTypeChange(folder: FolderInfo, val) {
                folder.Files.IsFileTypeIndeterminate = val.length != Main.config.fileTypes.length;
                Main.fileListFileTypeGetAnswer(folder);
            }

            /**
             * 文件列表所有文件类型
             * @param folder
             * @param val
             * */
            function allStorageType(folder: FolderInfo, val) {
                folder.Files.Filters.StorageType = val ? Main.config.storageTypes : [];
                folder.Files.IsStorageTypeIndeterminate = false;
                Main.fileListStorageTypeGetAnswer(folder);
            }

            /**
             * 文件列表更改文件类型
             * @param folder
             * @param val
             * */
            function storageTypeChange(folder: FolderInfo, val) {
                folder.Files.IsStorageTypeIndeterminate = val.length != Main.config.storageTypes.length;
                Main.fileListStorageTypeGetAnswer(folder);
            }

            /**
             * 文件列表所有文件状态
             * @param folder
             * @param val
             * */
            function allFileState(folder: FolderInfo, val) {
                folder.Files.Filters.State = val ? Main.config.fileStates : [];
                folder.Files.IsFileStateIndeterminate = false;
                Main.fileListFileStateGetAnswer(folder);
            }

            /**
             * 文件列表更改文件状态
             * @param folder
             * @param val
             * */
            function fileStateChange(folder: FolderInfo, val) {
                folder.Files.IsFileStateIndeterminate = val.length != Main.config.fileStates.length;
                Main.fileListFileStateGetAnswer(folder);
            }

            /**
             * 文件列表更改日期范围
             * @param folder
             * @param val
             * */
            function fileListDateRangeChange(folder: FolderInfo, val) {
                Main.fileListDataRangeGetAnswer(folder);
            }

            /**
             * 文件列表更改名称
             * @param folder
             * @param val
             * */
            function fileListNameChange(folder: FolderInfo, val) {
                Main.fileListNameGetAnswer(folder);
            }

            /**
             * 文件列表更改拓展名
             * @param folder
             * @param val
             * */
            function fileListExtensionChange(folder: FolderInfo, val) {
                Main.fileListExtensionGetAnswer(folder);
            }

            /**
             * 文件列表更改服务器标识
             * @param folder
             * @param val
             * */
            function fileListServerKeyChange(folder: FolderInfo, val) {
                Main.fileListServerKeyGetAnswer(folder);
            }

            /**
             * 文件列表更改内容类型
             * @param folder
             * @param val
             * */
            function fileListContentTypeChange(folder: FolderInfo, val) {
                Main.fileListContentTypeGetAnswer(folder);
            }

            /**
             * 文件列表更改文件MD5校验值
             * @param folder
             * @param val
             * */
            function fileListMD5Change(folder: FolderInfo, val) {
                Main.fileListMD5GetAnswer(folder);
            }

            /**
             * 更新文件列表分页设置
             * @param folder
             */
            function updateFileListPagination(folder: FolderInfo) {
                for (const field in folder.Files.Filters) {
                    let _continue = false;
                    const value = folder.Files.Filters[field];

                    for (var i = 0; i < folder.Files.Pagination.DynamicFilterInfo.length; i++) {
                        let filter = folder.Files.Pagination.DynamicFilterInfo[i];
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
                        let filter: PaginationDynamicFilterInfo = field == 'DateRang' ?
                            {
                                LocalState: field,
                                Relation: FilterGroupRelation.并且,
                                DynamicFilterInfo: [
                                    {
                                        Field: 'CreateTime',
                                        Value: Dayjs(value[0]).format('YYYY-MM-DD HH:mm:ss'),
                                        Compare: FilterCompare.大于等于
                                    },
                                    {
                                        Field: 'CreateTime',
                                        Value: Dayjs(value[1]).format('YYYY-MM-DD HH:mm:ss'),
                                        Compare: FilterCompare.小于等于
                                    }
                                ]
                            } :
                            {
                                LocalState: field,
                                Field: field,
                                Value: value,
                                Compare: Array.isArray(value) ? FilterCompare.在集合中 : FilterCompare.包含
                            };

                        folder.Files.Pagination.DynamicFilterInfo.push(filter);
                    }
                }

                getFileList(folder);
            }

            /**
             * 获取文件库文件数据列表
             * @param folder
             * */
            function getFileList(folder: FolderInfo) {
                folder.Files.Loading = true;
                Axios.post(ApiUri.GetFileList, folder.Files.Pagination).then(function (response: { data: PaginationResult_ElementVue<FileInfo> }) {
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

            /**
             * 文件库文件数据列表排序
             * @param {any} val 值
             */
            function fileSort(val) {
                let folder: FolderInfo = Main.library.currentOpenFolder;

                if (val.prop == null)
                    folder.Files.Pagination.AdvancedSort = [];
                else if (!val.order)
                    folder.Files.Pagination.AdvancedSort = folder.Files.Pagination.AdvancedSort.filter(data => data.Field != val.prop);
                else {
                    const order = val.order == 'descending' ? SortType.倒序 : SortType.正序;
                    for (let sort of folder.Files.Pagination.AdvancedSort) {
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

            /**
             * 更改文件库文件数据列表每页数据量
             * @param {number} val
             */
            function fileListSizeChange(val) {
                let folder: FolderInfo = Main.library.currentOpenFolder;
                folder.Files.Pagination.PageRows = val;
                getFileList(folder);
            }

            /**
             * 跳转至文件库文件数据列表指定页
             * @param {number} val
             */
            function fileListCurrentChange(val) {
                let folder: FolderInfo = Main.library.currentOpenFolder;
                folder.Files.Pagination.PageIndex = val;
                getFileList(folder);
            }

            /**
             * 获取文件库文件数据详情
             * @param {any} row
             * @param {any} index
             */
            function fileDetail(row, index) {
                Main.library.fileDetail.show = true;
                Main.library.fileDetail.loading = true;

                Axios.get(ApiUri.GetFileDetail(row.Id))
                    .then((response: { data: ResponseData_T<FileInfo> }) => {
                        if (response.data.Success) {
                            Main.library.fileDetail.detail = response.data.Data;
                        }
                        else {
                            ElementPlus.ElMessage(response.data.Message);
                        }
                        Main.library.fileDetail.loading = false;
                    })
                    .catch((error) => {
                        Main.library.fileDetail.loading = false;
                        ElementPlus.ElMessage('获取文件详情时发生异常.');
                    });
            }

            /**
             * 获取文件状态指定的标签类型
             * @param {any} state
             */
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

            /**
             * 预览文件
             * @param {string} cmd
             * @param {any} data
             * @param {number} index
             */
            function handleFileListCommand(cmd: string, data, index = null) {
                switch (cmd) {
                    case 'detail':
                        fileDetail(data, index);
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

            /**
             * 关闭文件详情
             * */
            function closeFileDetail() {
                Main.library.fileDetail.show = false;
                Main.library.fileDetail.detail = {};
            }

            /**
             * 预览文件
             * @param {any} data
             * @param {number} index
             */
            function previewFile(data, index = null) {
                window.open(ApiUri.Preview(data.Id));
            }

            /**
             * 浏览文件
             * @param {any} data
             * @param {number} index
             */
            function browseFile(data, index = null) {
                window.open(ApiUri.Browse(data.Id));
            }

            /**
             * 下载文件
             * @param {any} data
             * @param {number} index
             */
            function downloadFile(data, index = null) {
                window.open(ApiUri.Download(data.Id));
            }

            /**
             * 删除文件
             * @param {any} data
             * @param {number} index
             */
            function deleteFile(data, index = null) {
                let folder: FolderInfo = Main.library.currentOpenFolder;

                folder.Files.Loading = true;
                Axios.post(ApiUri.Delete, [data.Id]).then(function (response: { data: ResponseData }) {
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