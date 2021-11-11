/**
 * 上传组件
 * */
class NaiveUpload {
    /**
     * 修改配置
     * */
    async Init(this: NaiveUpload, settings: MultipleUploadSetting) {
        const promise = new Promise<void>(async (resolve, reject) => {
            this.Settings = settings;

            let files: ImportFile[] = [
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/UploadConfig.js'
                    }
                },
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/SelectedFile.js'
                    }
                },
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/RawFile.js'
                    }
                },
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Model/ChunkFile.js'
                    }
                },
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Helper/HashHelper.js'
                    }
                },
                {
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/plug-in/naiveupload/Helper/UploadHelper.js'
                    }
                }
            ];

            if (this.Settings.Axios == null)
                files.push({
                    Tag: ImportFileTag.JS,
                    Attributes: {
                        type: 'text/javascript',
                        src: ApiUri.BaseUrl + '/utils/axios.min.js'
                    }
                });

            ImportHelper.ImportFile(
                files,
                () => {
                    if (this.Settings.Axios == null)
                        this.Settings.Axios = new (<any>window).axios;
                    this.CreateAxiosInstance();
                    resolve();
                });
        });

        return promise;
    }

    /**
     * 创建Axios实例
     * */
    private CreateAxiosInstance() {
        this.AxiosInstance = this.Settings.Axios.create({});
        for (const key in this.Settings.Headers) {
            this.AxiosInstance.defaults.headers.common[key] = this.Settings.Headers[key];
        }
    }

    /**
     * 设置
     * */
    Settings: MultipleUploadSetting;

    /**
     * 源文件集合
     */
    RawFileList: RawFile[] = [];

    /**
     * 选中的文件集合
     */
    SelectedFileList: SelectedFile[] = [];

    /**
     * 校验文件队列
     * 数据项为this.SelectedFileList的索引
     * */
    private CheckQueue: number[] = [];

    private CheckHandlerCount = 0;

    /**
     * 上传文件队列
     * 数据项为this.SelectedFileList的索引
     * */
    private UploadQueue: number[] = [];

    private UploadHandlerCount = 0;

    /**
     * http请求实例
     */
    private AxiosInstance: any;

    /**
     * 暂停
     * @param {number} index 
     * SelectedFileList索引, 
     * 未设置此参数时暂停全部
     */
    Pause(this: NaiveUpload, index?: number) {

    }

    /**
     * 恢复
     * @param {number} index
     * SelectedFileList索引,
     * 未设置此参数时恢复全部
     */
    Continues(this: NaiveUpload, index?: number) {

    }

    /**
     * 删除
     * @param {number} index SelectedFileList索引
     */
    Remove(this: NaiveUpload, index: number) {
        this.SelectedFileList[index].Canceled = true;
    }

    /**
     * 清空
     * */
    Clean(this: NaiveUpload) {
        for (let file of this.SelectedFileList) {
            file.Canceled = true;
        }
        //this.SelectedFileList.length = 0;
    }

    /**
     * 追加文件
     * @param {any} files 文件
     * @returns {any} 追加文件返回信息状态
     */
    AppendFiles(this: NaiveUpload, files: File[]): AppendFileResultStatus {
        if (!!this.Settings.Config.UpperLimit && (this.SelectedFileList.length + files.length) > this.Settings.Config.UpperLimit)
            return AppendFileResultStatus.超出数量限制;

        let push = (file: File, newFile: SelectedFile) => {
            let rawFile = new RawFile(file);
            rawFile.Name = newFile.Name;
            rawFile.Extension = newFile.Extension;
            rawFile.ConfigId = this.Settings.Config.Id;
            this.RawFileList.push(rawFile);

            newFile.RawIndex = this.RawFileList.length - 1;

            this.SelectedFileList.push(newFile);

            this.HandleFile(this.SelectedFileList.length - 1);
        };

        const fileList: File[] = Array.prototype.slice.call(files);

        for (let file of fileList) {
            if (this.Limited())
                return AppendFileResultStatus.超出数量限制;

            const extension = file.name.substring(file.name.lastIndexOf('.'));

            if (this.Settings.Config.AllowedTypeList.length !== 0) {
                var flag = false;
                for (const type of this.Settings.Config.AllowedTypeList) {
                    if ((type[0] === '.' && type === extension) || new RegExp(type.replace('/*', '//*')).test(file.type)) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    console.warn(file.type);
                    return AppendFileResultStatus.文件类型不合法;
                }
            }

            if (this.Settings.Config.ProhibitedTypeList.length !== 0) {
                for (const type of this.Settings.Config.ProhibitedTypeList) {
                    if ((type[0] === '.' && type === extension) || new RegExp(type.replace('/*', '//*')).test(file.type))
                        return AppendFileResultStatus.文件类型不合法;
                }
            }

            let selectedFile = new SelectedFile(file);

            if (this.Settings.BeforeCheck) {
                const before = this.Settings.BeforeCheck(file);
                if (before && before.then) {
                    before.then(flag => {
                        if (flag)
                            push(file, selectedFile);
                    });
                }
            } else
                push(file, selectedFile);
        }

        return AppendFileResultStatus.成功;
    }

    /**
     * 处理文件
     * @param selectedFileIndex
     */
    private HandleFile(selectedFileIndex: number) {
        const selectedFile = this.SelectedFileList[selectedFileIndex];

        selectedFile.Thumbnail = ApiUri.FileTypeImageUrl(selectedFile.ExtensionLower);

        this.GetFileType(selectedFile, () => {
            this.CheckImage(selectedFile);
        });

        this.GetFileSize(selectedFile);

        if (this.Settings.EnableChunk)
            this.GetChunks(selectedFile);

        this.PushToCheckQueue(selectedFileIndex);
    }

    /**
     * 推送至校验队列
     * @param selectedFileIndex
     */
    private PushToCheckQueue(selectedFileIndex: number) {
        this.CheckQueue.push(selectedFileIndex);

        if (this.Settings.Mode == UploadMode.自动挡 || this.Settings.Mode == UploadMode.手自一体)
            this.CheckMD5();
    }

    /**
     * 推送至上传队列
     * @param selectedFileIndex
     */
    private PushToUploadQueue(selectedFileIndex: number) {
        this.UploadQueue.push(selectedFileIndex);

        if (this.Settings.Mode == UploadMode.自动挡)
            this.Upload();
    }

    /**
     * 开始校验MD5
     * */
    async CheckMD5(this: NaiveUpload) {
        if (this.CheckHandlerCount >= this.Settings.ConcurrentFile || this.CheckQueue.length === 0) {
            if (this.CheckHandlerCount == 0 && this.CheckQueue.length === 0 && this.Settings.AfterCheckAll) {
                const after = this.Settings.AfterCheckAll(this.RawFileList);
                if (after && after.then) {
                    return;
                }
            }
            else
                return;
        }

        this.CheckHandlerCount++;

        /**
         * 校验下一个文件
         * */
        const next = () => {
            this.CheckHandlerCount--;

            if (this.Settings.AfterCheck) {
                const after = this.Settings.AfterCheck(rawFile);
                if (after && after.then) {
                    after.then(() => { this.CheckMD5(); });
                }
            } else
                this.CheckMD5();
        };

        const selectedFileIndex = this.CheckQueue.shift();
        const selectedFile = this.SelectedFileList[selectedFileIndex];
        if (selectedFile.Canceled) {
            next();
            return;
        }
        selectedFile.Checking = true;
        const rawFile = this.GetRawFile(selectedFile);

        const hashHelper = new HashHelper();
        await hashHelper.Init(rawFile.NeedSection ? 2 : 1, this.Settings.EnableWorker);

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
                this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e);
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
                this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true, e);
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
            this.CheckError(selectedFileIndex, '文件校验失败，请删除后重新上传.', true);
        } else {
            rawFile.MD5 = md5;
            selectedFile.VirtualPercent = 100;
            selectedFile.Percent = 100;
            selectedFile.Checking = false;
            selectedFile.Checked = true;

            //推送至上传队列
            this.PushToUploadQueue(selectedFileIndex);
        }

        close();

        next();
    }

    /**
     * 开始上传已经校验过了的文件
     * */
    async Upload(this: NaiveUpload) {
        if (this.UploadHandlerCount >= this.Settings.ConcurrentFile)
            return;

        if (this.UploadQueue.length === 0) {
            if (this.UploadHandlerCount === 0 && this.Settings.AfterUploadAll)
                this.Settings.AfterUploadAll(this.RawFileList);
            return;
        }

        this.UploadHandlerCount++;

        /**
         * 上传下一个文件
         * */
        const next = () => {
            this.UploadHandlerCount--;

            if (this.Settings.AfterUpload) {
                const after = this.Settings.AfterUpload(rawFile);
                if (after && after.then) {
                    after.then(() => { this.Upload(); });
                }
            } else
                this.Upload();
        };

        const selectedFileIndex = this.UploadQueue.shift();
        const selectedFile = this.SelectedFileList[selectedFileIndex];
        if (selectedFile.Canceled) {
            next();
            return;
        }
        selectedFile.Uploading = true;
        selectedFile.VirtualPercent = 0;
        selectedFile.Percent = 0;
        const rawFile = this.GetRawFile(selectedFile);

        const uploadHelper = new UploadHelper();
        await uploadHelper.Init(this.Settings.Axios, this.Settings.Headers, this.Settings.ConcurrentChunkFile, this.Settings.EnableWorker);

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
            this.UploadError(selectedFileIndex, `文件上传失败，${e.message}.`, true, e);
        }

        close();

        next();
    }

    /**
     * 是否已达上限
     * */
    Limited(this: NaiveUpload) {
        return !!this.Settings.Config.UpperLimit && this.SelectedFileList.length >= this.Settings.Config.UpperLimit;
    }

    /**
     * 更新配置
     * @param this
     * @param configId 文件上传配置Id
     */
    async UpdateConfig(this: NaiveUpload, configId: string) {
        const promise = new Promise<UploadConfig>((resolve, reject) => {
            if (this.Settings.Config == null)
                this.Settings.Config = new UploadConfig();

            this.AxiosInstance.post(ApiUri.FileUploadConfigData(configId), {})
                .then((response: { data: ResponseData_T<UploadConfig> }) => {
                    if (response.data.Success) {
                        this.UpdateConfigData(response.data.Data);
                        resolve(response.data.Data);
                    }
                    else
                        reject(new Error(response.data.Message));
                })
                .catch((error) => {
                    console.error(error);

                    reject(new Error('获取文件上传配置时发生异常.'));
                });
        });

        return promise;
    }

    /**
     * 更新配置
     * @param this
     * @param configId 文件上传配置Id
     */
    UpdateConfigData(this: NaiveUpload, config: UploadConfig) {
        for (let key in config) {
            this.Settings.Config[key] = config[key];
        }
    }

    /**
     * 异常
     * @param selectedFileIndex
     * @param message 异常信息
     * @param retry 是否允许重试
     * @param done 回调
     */
    private Error(selectedFileIndex: number, message: string, retry: boolean, done: (selectedFileIndex: number) => void | null) {
        const selectedFile = this.SelectedFileList[selectedFileIndex];

        if (!retry || selectedFile.ReTry - 1 >= this.Settings.Retry) {
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
    private CheckError(selectedFileIndex: number, message: string, retry: boolean, e: Error = null) {
        console.error(e);
        const selectedFile = this.SelectedFileList[selectedFileIndex];
        selectedFile.Checking = false;
        selectedFile.Uploading = false;
        this.Error(selectedFileIndex, message, retry, (_selectedFileIndex) => {
            this.PushToCheckQueue(_selectedFileIndex);
        });
    }

    /**
     * 上传时异常
     * @param selectedFileIndex
     * @param message 说明
     * @param retry 是否允许重试
     * @param e 异常
     */
    private UploadError(selectedFileIndex: number, message: string, retry: boolean, e: Error = null) {
        console.error(e);
        const selectedFile = this.SelectedFileList[selectedFileIndex];
        selectedFile.Uploading = false;
        this.Error(selectedFileIndex, message, retry, (_selectedFileIndex) => {
            this.PushToUploadQueue(_selectedFileIndex);
        });
    }

    /**
     * 获取原始文件
     * @param selectedFile
     */
    GetRawFile(this: NaiveUpload, selectedFile: SelectedFile) {
        return this.RawFileList[selectedFile.RawIndex];
    }

    /**
     * 文件切片
     * @param file
     */
    private GetChunks(selectedFile: SelectedFile) {
        let file = this.GetRawFile(selectedFile);
        if (file.Size > this.Settings.ChunkSize) {
            file.NeedSection = true;
            file.Specs = this.Settings.ChunkSize;
        }
        else
            return;

        let count = 0;
        while (count < file.Size) {
            //切片索引添加至队列
            file.ChunkIndexQueue.push(file.Chunks.length);

            file.Chunks.push(new ChunkFile(file.Chunks.length, file.File.slice(count, count + this.Settings.ChunkSize)));

            count += this.Settings.ChunkSize;
        }
    }

    /**
     * 获取文件类型
     * @param selectedFile
     * @param done
     */
    private GetFileType(selectedFile: SelectedFile, done: () => void) {
        const file = this.GetRawFile(selectedFile);
        const api = file.File.type === null || file.File.type === '' || file.File.type === 'application/octet-stream' ? ApiUri.FileTypeByExtension(selectedFile.ExtensionLower) : ApiUri.FileTypeByMIME(file.File.type);

        this.AxiosInstance.get(api)
            .then((response: { data: ResponseData_T<string> }) => {
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
    private CheckImage(selectedFile: SelectedFile) {
        if (selectedFile.FileType !== FileType.图片)
            return;

        selectedFile.Thumbnail = this.GetRawFile(selectedFile).ObjectURL;
    }

    /**
     * 获取文件大小
     * @param {any} selectedFile
     */
    private GetFileSize(selectedFile: SelectedFile) {
        this.AxiosInstance.get(ApiUri.FileSize(this.GetRawFile(selectedFile).Size))
            .then((response: { data: ResponseData_T<string> }) => {
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
     * 文件重命名
     * @param {any} selectedFile
     */
    async Rename(this: NaiveUpload, selectedFile: SelectedFile) {
        const promise = new Promise<void>((resolve, reject) => {
            let rawFile = this.GetRawFile(selectedFile);

            const done = (success) => {
                if (success) {
                    rawFile.Name = selectedFile.Name;
                    if (selectedFile.Uploaded)
                        rawFile.FileInfo.Name = selectedFile.Name;
                    selectedFile.Rename = false;
                }
                else
                    selectedFile.Name = rawFile.Name;
            }

            if (selectedFile.Uploaded) {
                this.AxiosInstance.get(ApiUri.PersonalFileInfoRename(rawFile.FileInfo.Id, selectedFile.Name))
                    .then((response: { data: ResponseData }) => {
                        done(response.data.Success);
                        if (response.data.Success)
                            resolve();
                        else
                            reject(new Error(response.data.Message));
                    })
                    .catch((error) => {
                        console.error(error);

                        done(false);
                        reject(new Error('文件重命名时发生异常.'));
                    });
            } else {
                done(true);
                resolve();
            }
        });

        return promise;
    }

    /**
     * 文件重命名
     * @param {number} index SelectedFileList索引
     * @param {string} name 文件名
     */
    async RenameByIndex(this: NaiveUpload, index: number, name: string) {
        let selectedFile = this.SelectedFileList[index];
        selectedFile.Name = name;
        this.Rename(selectedFile);
    }
}