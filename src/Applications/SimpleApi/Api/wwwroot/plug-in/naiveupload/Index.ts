﻿window.onload = () => {
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

                    //延迟处理用户的输入内容
                    Main.fileNameGetAnswer = _.debounce(getFileList, 500);
                    Main.fileExtensionGetAnswer = _.debounce(getFileList, 500);
                    Main.fileContentTypeGetAnswer = _.debounce(getFileList, 500);
                    Main.fileMD5GetAnswer = _.debounce(getFileList, 500);
                    Main.fileServerKeyGetAnswer = _.debounce(getFileList, 500);
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

                    GetFolderCLass,

                    folderFileList,

                    allFileType,
                    fileTypeChange,
                    allStorageType,
                    storageTypeChange,
                    allFileState,
                    fileStateChange,
                    fileSort,
                    fileListSizeChange,
                    fileListCurrentChange,
                    fileListRowClassName,
                    fileDetail: getFileDetail,
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
                    },
                    'library.file.date'(newValue, oldValue) {
                        if (newValue[0] == oldValue[0] && newValue[1] == oldValue[1])
                            return;
                        getFileList();
                    },
                    'library.file.name'(newValue, oldValue) {
                        this.fileNameGetAnswer();
                    },
                    'library.file.extension'(newValue, oldValue) {
                        this.fileExtensionGetAnswer();
                    },
                    'library.file.contentType'(newValue, oldValue) {
                        this.fileContentTypeGetAnswer();
                    },
                    'library.file.md5'(newValue, oldValue) {
                        this.fileMD5GetAnswer();
                    },
                    'library.file.serverKey'(newValue, oldValue) {
                        this.fileServerKeyGetAnswer();
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
                    .catch((error) => {
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
                for (var i = 0; i < MultipleUploadSettings.ConcurrentFile; i++) {
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
            function GetFolderCLass(folder) {
                return `folder ${(folder.Hover ? ' hover' : '')}`;
            }

            /**
             * 文件夹详情
             * @param folder
             */
            function folderFileList(folder: FolderInfo) {
                console.info(folder.FileType);

                Main.library.file.pageSize = getPageSize();

                getFileList();
            }

            /**
             * 根据页面大小调整页面数据量
             * */
            function getPageSize() {
                var current = window.innerHeight / 100,
                    min,
                    result;
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

            /**
             * 文件列表所有文件类型
             * */
            function allFileType(val) {
                Main.library.file.fileTypes = val ? Main.config.fileTypes : [];
                Main.library.file.isFileTypeIndeterminate = false;
                getFileList();
            }

            /**
             * 文件列表更改文件类型
             * */
            function fileTypeChange(val) {
                getFileList();
            }

            /**
             * 文件列表所有文件类型
             * */
            function allStorageType(val) {
                Main.library.file.storageTypes = val ? Main.config.storageTypes : [];
                Main.library.file.isStorageTypeIndeterminate = false;
                getFileList();
            }

            /**
             * 文件列表更改文件类型
             * */
            function storageTypeChange(val) {
                getFileList();
            }

            /**
             * 文件列表所有文件状态
             * */
            function allFileState(val) {
                Main.library.file.fileStates = val ? Main.config.fileStates : [];
                Main.library.file.isFileStateIndeterminate = false;
                getFileList();
            }

            /**
             * 文件列表更改文件状态
             * */
            function fileStateChange(val) {
                getFileList();
            }

            /**
             * 获取文件列表接口参数
             * @param {any} target
             */
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

            /**
             * 获取文件库文件数据列表
             * */
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

            /**
             * 文件库文件数据列表排序
             * @param {any} val 值
             */
            function fileSort(val) {
                if (val.prop == null)
                    Main.library.file.sorts = [];
                else if (!val.order)
                    Main.library.file.sorts = Main.library.file.sorts.filter(data => data.field != val.prop);
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

            /**
             * 更改文件库文件数据列表每页数据量
             * @param {number} val
             */
            function fileListSizeChange(val) {
                Main.library.file.pageSize = val;
                getFileList();
            }

            /**
             * 跳转至文件库文件数据列表指定页
             * @param {number} val
             */
            function fileListCurrentChange(val) {
                Main.library.file.currentPage = val;
                getFileList();
            }

            /**
             * 获取文件库文件数据指定的类名
             * @param {any} param0
             */
            function fileListRowClassName({ row, rowIndex }) {
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

            /**
             * 获取文件库文件数据详情
             * @param {any} index
             * @param {any} row
             */
            function getFileDetail(index, row) {
                Main.library.file.detail.show = true;
                Main.library.file.detail.loading = true;

                Axios.get(ApiUri.GetFileDetail(row.Id))
                    .then((response) => {
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
                    .catch((error) => {
                        Main.library.file.detail.loading = false;
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
             * 关闭文件详情
             * */
            function closeFileDetail() {
                Main.library.file.detail.show = false;
                for (var item in Main.library.file.detail) {
                    if (item != 'show' && item != 'loding')
                        Main.library.file.detail[item] = '';
                }
            }

            /**
             * 预览文件
             * @param {any} index
             * @param {any} data
             */
            function previewFile(index, data) {
                window.open(ApiUri.Preview(data.Id));
            }

            /**
             * 浏览文件
             * @param {any} index
             * @param {any} data
             */
            function browseFile(index, data) {
                window.open(ApiUri.Browse(data.Id));
            }

            /**
             * 下载文件
             * @param {any} index
             * @param {any} data
             */
            function downloadFile(index, data) {
                window.open(ApiUri.Download(data.Id));
            }

            /**
             * 删除文件
             * @param {any} index
             * @param {any} data
             */
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