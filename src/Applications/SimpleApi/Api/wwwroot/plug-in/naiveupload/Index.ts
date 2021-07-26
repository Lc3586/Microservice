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
                    src: '../../element-plus/dayjs.zh-cn.js'
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
        ],
        () => {
            const Vue = (<any>window).Vue;

            const ElementPlus = (<any>window).ElementPlus;

            const Axios = (<any>window).axios;

            const Promise = (<any>window).Promise;

            const _ = (<any>window)._;

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

                    if (this.config.type === 'Signle')
                        MultipleUploadSettings.Limit = 1;
                    else if (this.config.type === 'Multiple')
                        MultipleUploadSettings.Limit = 50;

                    Upload.Init(MultipleUploadSettings).then(() => {
                        Upload.SelectedFileList = this.multipleUpload.files;
                    });

                    AddLoginFilter();
                    Authorized();
                },
                methods: {
                    SALogin,
                    TabsChanged,

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
                    Clean
                },
                watch: {
                    /**
                     * 监听设置变化
                     */
                    'multipleUpload.settings': {
                        handler(val) {
                            for (const key in val) {
                                if (MultipleUploadSettings[key] !== val[key])
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
                    }
                }
            };

            const App = Vue.createApp(AppData);
            ElementPlus.locale(ElementPlus.lang.zhCn);
            App.use(ElementPlus);
            App.mount("#app");

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
                        type: 'Single'
                    },
                    single: {
                        settings: {

                        },
                        file: {

                        }
                    },
                    multipleUpload: {
                        settings: {
                            Accept: MultipleUploadSettings.Accept,
                            MultipleSelect: MultipleUploadSettings.MultipleSelect,
                            Explain: MultipleUploadSettings.Explain,
                            Tip: MultipleUploadSettings.Tip,
                            Theme: MultipleUploadSettings.Theme
                        },
                        /**
                         * 锁定滚动条
                         */
                        scrollLock: true,
                        /*文件集合*/
                        files: []
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
                        setTimeout(RefreshToken, response.data.Data.Expires.getTime() - new Date().getTime() - 60 * 1000);
                    }
                    else
                        ElementPlus.ElMessage(response.data.Message);
                }).catch(function (error) {
                    console.error(error);
                    ElementPlus.ElMessage('刷新Token时发生异常.');
                });
            }

            /**
             * 切换标签时的点击事件
             * @param {any} tab
             */
            function TabsChanged(tab) {
                Main.config.type = tab.props.name;
                switch (tab.props.name) {
                    case 'Single':
                        if (Upload.SelectedFileList.length > 1)
                            Upload.Clean();
                        MultipleUploadSettings.Limit = 1;
                        break;
                    default:

                        break;
                    case 'Multiple':
                        MultipleUploadSettings.Limit = 50;
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
                if (!MultipleUploadSettings.Limit)
                    return null;

                return Upload.SelectedFileList.length < MultipleUploadSettings.Limit ? `还可添加个${MultipleUploadSettings.Limit - Upload.SelectedFileList.length}文件` : `文件数量已达上限`;
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
                Upload.AppendFiles(e.dataTransfer.files);
            }

            /**
             * 已选择文件
             * @param {any} e
             */
            function ChoseFile(e) {
                Upload.AppendFiles(Main.$refs.fileInput.files);
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
                Upload.Upload();
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
        });
};