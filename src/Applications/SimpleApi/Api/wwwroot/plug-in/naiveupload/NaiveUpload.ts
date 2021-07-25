window.onload = () => {
    (<any>window).importFile(
        [
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
            MultipleUploadSettings.Limit = 50;

            /**
             * 源文件集合
             */
            let RawFileList: RawFile[] = [];

            /**
             * 选中的文件集合
             */
            let SelectedFileList: SelectedFile[];

            /**
             * 校验文件队列
             * 数据项为selectedFileList的索引
             * */
            let CheckQueue: number[] = [];

            let CheckHandlerCount = 0;

            /**
             * 上传文件队列
             * 数据项为selectedFileList的索引
             * */
            let UploadQueue: number[] = [];

            let UploadHandlerCount = 0;

            /**
             * 应用数据
             */
            const AppData = {
                data: GetRenderData,
                created() {
                    Main = this;
                    SelectedFileList = this.multipleUpload.files;
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

                                this.$refs.uploadList.scrollTop = container.offsetTop - this.$refs.uploadList.offsetTop;
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
                        type: 'Multiple'
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
                    if (response.data.Success)
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
                    case 'Signle':
                    default:

                        break;
                    case 'Multiple':

                        break;
                }
            }

            /**
             * 是否已达上限
             * */
            function Limited() {
                return !!MultipleUploadSettings.Limit && Main.multipleUpload.files.length >= MultipleUploadSettings.Limit;
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
                return `upload-drag ${(Limited() ? 'item-limited' : '')}`;
            }

            /**
             * 获取文件选择框的提示语
             * */
            function GetSelectTitle() {
                if (!MultipleUploadSettings.Limit)
                    return null;

                return SelectedFileList.length < MultipleUploadSettings.Limit ? `还可添加个${MultipleUploadSettings.Limit - SelectedFileList.length}文件` : `文件数量已达上限`;
            }

            /**
             * 选择文件
             * @param {any} e
             * */
            function ChosingFile(e) {
                if (Limited())
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
                AppendFiles(e.dataTransfer.files);
            }

            /**
             * 已选择文件
             * @param {any} e
             */
            function ChoseFile(e) {
                AppendFiles(Main.$refs.fileInput.files);
            }

            /**
             * 追加文件
             * @param {any} files
             */
            function AppendFiles(files: File[]) {
                if (!!MultipleUploadSettings.Limit && (SelectedFileList.length + files.length) > MultipleUploadSettings.Limit) {
                    ElementPlus.ElMessage(`最多只能添加${MultipleUploadSettings.Limit}个文件.`);
                }

                let push = (file: File, newFile: SelectedFile) => {
                    let rawFile = new RawFile(file);
                    rawFile.Name = newFile.Name;
                    rawFile.Extension = newFile.Extension;
                    RawFileList.push(rawFile);

                    newFile.RawIndex = RawFileList.length - 1;

                    SelectedFileList.push(newFile);

                    HandleFile(SelectedFileList.length - 1);
                };

                const fileList: File[] = Array.prototype.slice.call(files);

                for (let file of fileList) {
                    if (Limited())
                        return;

                    let selectedFile = new SelectedFile(file);

                    if (MultipleUploadSettings.BeforeUpload) {
                        const before = MultipleUploadSettings.BeforeUpload(file);
                        if (before && before.then) {
                            before.then(flag => {
                                if (flag)
                                    push(file, selectedFile);
                            });
                        }
                    } else
                        push(file, selectedFile);
                }
            }

            /**
             * 处理文件
             * @param selectedFileIndex
             */
            function HandleFile(selectedFileIndex: number) {
                const selectedFile = SelectedFileList[selectedFileIndex];

                selectedFile.Thumbnail = ApiUri.FileTypeImageUrl(selectedFile.ExtensionLower);

                getFileType(selectedFile, () => {
                    checkImage(selectedFile);
                });

                getFileSize(selectedFile);

                if (MultipleUploadSettings.EnableChunk)
                    getChunks(selectedFile);

                PushToCheckQueue(selectedFileIndex);
            }

            /**
             * 推送至校验队列
             * @param selectedFileIndex
             */
            function PushToCheckQueue(selectedFileIndex: number) {
                CheckQueue.push(selectedFileIndex);
                CheckMD5();
            }

            /**
             * 推送至上传队列
             * @param selectedFileIndex
             */
            function PushToUploadQueue(selectedFileIndex: number) {
                UploadQueue.push(selectedFileIndex);
                Upload();
            }

            /**
             * 校验MD5
             * */
            async function CheckMD5() {
                if (CheckHandlerCount >= MultipleUploadSettings.ConcurrentFile || CheckQueue.length === 0) {
                    return;
                }

                CheckHandlerCount++;

                /**
                 * 校验下一个文件
                 * */
                const next = () => {
                    CheckHandlerCount--;

                    CheckMD5();
                };

                const selectedFileIndex = CheckQueue.shift();
                const selectedFile = SelectedFileList[selectedFileIndex];
                if (selectedFile.Canceled) {
                    next();
                    return;
                }
                selectedFile.Checking = true;
                const rawFile = getRawFile(selectedFile);

                const hashHelper = new HashHelper();
                await hashHelper.Init(rawFile.NeedSection ? 2 : 1, MultipleUploadSettings.EnableWorker);

                /**
                 * 关闭
                 * */
                const close = () => {
                    hashHelper.CloseAll();
                }

                /**
                 * 取消
                 * */
                const cancel = () => {
                    close();

                    next();
                }

                /**
                 * 发送数据
                 * @param blob 数据
                 * @param end 是否立即返回计算结果
                 * @param unitIndex 计算单元索引
                 */
                const calc = async (end: boolean, blob?: Blob, unitIndex: number = 0): Promise<string | null> => {
                    try {
                        if (end) {
                            if ('undefined' === typeof blob)
                                return await hashHelper.End(unitIndex);
                            else
                                return await hashHelper.Calc(blob, unitIndex);
                        }
                        else {
                            await hashHelper.Append(blob, unitIndex);
                            return null;
                        }
                    } catch (e) {
                        checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e);
                        throw e;
                    }
                }

                /**
                 * 读取数据
                 * @param blob 文件数据
                 * @param end 结束计算并发送哈希值
                 * @param unitIndex 计算单元索引
                 */
                const handlerData = async (blob: Blob, end: boolean, unitIndex: number = 0): Promise<boolean | string> => {
                    const percent = (blob.size / rawFile.Size) * 100;

                    selectedFile.VirtualPercent = parseFloat((selectedFile.VirtualPercent + percent).toFixed(2));

                    let result: string | boolean;
                    try {
                        result = await calc(end, blob, unitIndex) || true;
                    } catch (e) {
                        checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e);
                        return false;
                    }

                    switch (unitIndex) {
                        case 0:
                            selectedFile.Percent = parseFloat((selectedFile.Percent + percent).toFixed(2));
                            break;
                    }

                    return result;
                }

                if (rawFile.NeedSection) {
                    for (const chunk of rawFile.Chunks) {
                        if (selectedFile.Canceled) {
                            cancel();
                            break;
                        }

                        //计算完整文件的md5
                        const appendDataPromise = handlerData(chunk.Blob, false);

                        //计算分片文件的md5
                        chunk.Checking = true;
                        const md5Promise = handlerData(chunk.Blob, true, 1);

                        if (!await appendDataPromise)
                            break;

                        const md5 = await md5Promise;
                        if ('boolean' === typeof md5)
                            break;

                        chunk.Checking = false;
                        chunk.MD5 = md5;
                        chunk.Checked = true;
                    }
                } else {
                    const bufferSize = 3 * 1024 * 1024;
                    let count = 0;
                    while (count < rawFile.Size) {
                        if (selectedFile.Canceled) {
                            cancel();
                            break;
                        }

                        const blob = rawFile.File.slice(count, Math.min(count + bufferSize, rawFile.Size));
                        count += blob.size;
                        if (!await handlerData(blob, false))
                            break;
                    }
                }

                //传输结束
                const md5 = await calc(true);

                if ('string' !== typeof md5) {
                    checkError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true);
                } else {
                    rawFile.MD5 = md5;
                    selectedFile.VirtualPercent = 100;
                    selectedFile.Percent = 100;
                    selectedFile.Checking = false;
                    selectedFile.Checked = true;

                    //推送至上传队列
                    PushToUploadQueue(selectedFileIndex);
                }

                close();

                next();
            }

            /**
             * 上传
             * */
            async function Upload() {
                if (UploadHandlerCount >= MultipleUploadSettings.ConcurrentFile)
                    return;

                if (UploadQueue.length === 0) {
                    if (MultipleUploadSettings.Done) {
                        await MultipleUploadSettings.Done(RawFileList);
                    }
                    return;
                }

                UploadHandlerCount++;

                /**
                 * 上传下一个文件
                 * */
                const next = () => {
                    UploadHandlerCount--;

                    if (MultipleUploadSettings.AfterUpload) {
                        const after = MultipleUploadSettings.AfterUpload(rawFile);
                        if (after && after.then) {
                            after.then(Upload);
                        }
                    } else
                        Upload();
                };

                const selectedFileIndex = UploadQueue.shift();
                const selectedFile = SelectedFileList[selectedFileIndex];
                if (selectedFile.Canceled) {
                    next();
                    return;
                }
                selectedFile.Uploading = true;
                selectedFile.VirtualPercent = 0;
                selectedFile.Percent = 0;
                const rawFile = getRawFile(selectedFile);

                const uploadHelper = new UploadHelper();
                await uploadHelper.Init(Axios, MultipleUploadSettings.Headers, MultipleUploadSettings.ConcurrentChunkFile, MultipleUploadSettings.EnableWorker);

                /**
                 * 关闭
                 * */
                const close = () => {
                    uploadHelper.CloseAll();
                }

                /**
                 * 取消
                 * */
                const cancel = () => {
                    close();

                    next();
                }

                try {
                    await uploadHelper.UploadFile(rawFile, (progress: UploadProgress) => {
                        selectedFile.VirtualPercent = parseFloat((progress.PreLoaded / progress.Total * 100).toFixed(2));
                        selectedFile.Percent = parseFloat((progress.Loaded / progress.Total * 100).toFixed(2));
                    });
                    selectedFile.VirtualPercent = 100;
                    selectedFile.Percent = 100;
                    selectedFile.Uploading = false;
                    selectedFile.Uploaded = true;
                    selectedFile.Done = true;
                } catch (e) {
                    console.error('error', e.message);
                    uploadError(selectedFileIndex, '文件上传失败，请删除后重新上传.', true, e);
                }

                close();

                next();
            }

            /**
             * 异常
             * @param selectedFileIndex
             * @param message 异常信息
             * @param retry 是否允许重试
             * @param done 回调
             */
            function error(selectedFileIndex: number, message: string, retry: boolean, done: (selectedFileIndex: number) => void | null) {
                const selectedFile = SelectedFileList[selectedFileIndex];

                if (!retry || selectedFile.ReTry - 1 >= MultipleUploadSettings.Retry) {
                    selectedFile.Error = true;
                    selectedFile.ErrorMessage = message;
                    return;
                }

                //等待重试
                selectedFile.ReTry++;

                done && done(selectedFileIndex);
            }

            /**
             * 校验时异常
             * @param selectedFileIndex
             * @param message 说明
             * @param retry 是否允许重试
             * @param e 异常
             */
            function checkError(selectedFileIndex: number, message: string, retry: boolean, e: Error = null) {
                console.error(e);
                const selectedFile = SelectedFileList[selectedFileIndex];
                selectedFile.Checking = false;
                selectedFile.Uploading = false;
                error(selectedFileIndex, message, retry, (_selectedFileIndex) => {
                    PushToCheckQueue(_selectedFileIndex);
                });
            }

            /**
             * 上传时异常
             * @param selectedFileIndex
             * @param message 说明
             * @param retry 是否允许重试
             * @param e 异常
             */
            function uploadError(selectedFileIndex: number, message: string, retry: boolean, e: Error = null) {
                console.error(e);
                const selectedFile = SelectedFileList[selectedFileIndex];
                selectedFile.Uploading = false;
                error(selectedFileIndex, message, retry, (_selectedFileIndex) => {
                    PushToUploadQueue(_selectedFileIndex);
                });
            }

            /**
             * 获取原始文件
             * @param selectedFile
             */
            function getRawFile(selectedFile: SelectedFile) {
                return RawFileList[selectedFile.RawIndex];
            }

            /**
             * 文件切片
             * @param file
             */
            function getChunks(selectedFile: SelectedFile) {
                let file = getRawFile(selectedFile);
                if (file.Size > MultipleUploadSettings.ChunkSize) {
                    file.NeedSection = true;
                    file.Specs = MultipleUploadSettings.ChunkSize;
                }
                else
                    return;

                let count = 0;
                while (count < file.Size) {
                    //切片索引添加至队列
                    file.ChunkIndexQueue.push(file.Chunks.length);

                    file.Chunks.push(new ChunkFile(file.Chunks.length, file.File.slice(count, count + MultipleUploadSettings.ChunkSize)));

                    count += MultipleUploadSettings.ChunkSize;
                }
            }

            /**
             * 获取文件类型
             * @param selectedFile
             * @param done
             */
            function getFileType(selectedFile: SelectedFile, done: () => void) {
                const file = getRawFile(selectedFile);
                const api = file.File.type === null || file.File.type === '' || file.File.type === 'application/octet-stream' ? ApiUri.FileTypeByExtension(selectedFile.ExtensionLower) : ApiUri.FileTypeByMIME(file.File.type);

                Axios.get(api)
                    .then((response) => {
                        if (response.data.Success)
                            selectedFile.FileType = response.data.Data;
                        else
                            selectedFile.FileType = FileType.未知;

                        done && done();
                    })
                    .catch((error) => {
                        selectedFile.FileType = FileType.未知;
                    });
            }

            /**
             * 检查图像
             * @param {any} selectedFile
             */
            function checkImage(selectedFile: SelectedFile) {
                if (selectedFile.FileType !== FileType.图片)
                    return;

                selectedFile.Thumbnail = getRawFile(selectedFile).ObjectURL;
            }

            /**
             * 获取文件大小
             * @param {any} selectedFile
             */
            function getFileSize(selectedFile: SelectedFile) {
                Axios.get(ApiUri.FileSize(getRawFile(selectedFile).Size))
                    .then((response) => {
                        if (response.data.Success)
                            selectedFile.Size = response.data.Data;
                        else
                            selectedFile.Size = '-';
                    })
                    .catch((error) => {
                        selectedFile.Size = '-';
                    });
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
                    Main.$refs.renameInput.focus();
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
                let rawFile = getRawFile(selectedFile);

                const done = (success) => {
                    if (success) {
                        rawFile.Name = selectedFile.Name;
                        rawFile.FileInfo.Name = selectedFile.Name;
                        selectedFile.Rename = false;
                    }
                    else
                        selectedFile.Name = rawFile.Name;
                }

                if (selectedFile.Uploaded) {
                    Axios.get(ApiUri.Rename(rawFile.FileInfo.Id, selectedFile.Name))
                        .then((response) => {
                            if (!response.data.Success)
                                ElementPlus.ElMessage(response.data.Message);
                            done(response.data.Success);
                        })
                        .catch((error) => {
                            console.error(error);
                            ElementPlus.ElMessage('文件重命名时发生异常.');
                            done(false);
                        });
                } else
                    done(true);
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
                const file = getRawFile(selectedFile);
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
                SelectedFileList[index].Canceled = true;
                //SelectedFileList.splice(index, 1);
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