/**
 * 文件上传帮助类
 * */
class UploadHelper {
    /**
     * 是否启用Worker
     * 默认启用（如果浏览器支持的话）
     */
    EnableWorker: boolean;

    /**
     * 浏览器是否支持Web Worker
     */
    WorkerSupported: boolean;

    /**
     * Http请求工具
     */
    Axios: any;

    /**
     * http请求实例
     */
    AxiosInstance: any;

    /**
     * 取消令牌
     */
    CancelTokenList: any = {};

    /**
     * 子线程上传单元
     */
    WorkerUnits: {
        worker: Worker, used: boolean
    }[] = [];

    /**
     * 分片文件处理队列
     * */
    ChunkHandlerQueue: ChunkFile[] = [];

    /**
     * 分片文件处理方法
     */
    ChunkHandler: boolean[] = [];

    /**
     * 推迟次数
     * 如果所有分片文件都需要推迟，则忽略推迟操作，直接首个分片文件
     * 同时推迟次数清零
     */
    DelayTimes: number = 0;

    /**
     * 已完成
     */
    Finished: boolean = false;

    /**
     * 文件数据读取器
     */
    FileReaders: FileReadHelper[] = [];

    /**
     * 初始化
     * @param axios http请求工具
     * @param headers 请求头设置
     * @param concurrent 并发数量
     * @param enableWorker 是否启用Worker
     */
    async Init(axios: any = null, headers: Record<string, string> = {} = null, concurrent: number, enableWorker: boolean = true) {
        const promise = new Promise<void>((resolve, reject) => {
            ImportHelper.ImportFile(
                [
                    {
                        Tag: ImportFileTag.JS,
                        Attributes: {
                            type: 'text/javascript',
                            src: 'Helper/FileReadHelper.js'
                        }
                    }],
                () => {
                    const setup = async () => {
                        if (headers != null) {
                            for (const key in headers) {
                                this.AxiosInstance.defaults.headers.common[key] = headers[key];
                            }
                        }

                        this.EnableWorker = enableWorker;
                        this.WorkerSupported = 'undefined' !== typeof Worker;

                        if (this.EnableWorker && this.WorkerSupported) {
                            for (let i = 0; i < concurrent; i++) {
                                let worker = new Worker('Helper/UploadWorker.js');
                                this.WorkerUnits.push({ worker: worker, used: false });
                                try {
                                    await this.WorkerPostMessage(i, { Type: UploadWorkerMessageType.设置Axios, Data: { Headers: headers } }) as FileInfo;
                                } catch (e) {
                                    reject(e);
                                    return;
                                }
                            }
                            resolve();
                        } else
                            resolve();
                    }

                    while (this.FileReaders.length < concurrent) {
                        this.FileReaders.push(new FileReadHelper());
                        this.ChunkHandler.push(false);
                    }

                    if (axios != null) {
                        this.Axios = axios;
                        this.AxiosInstance = this.Axios.create({});
                        setup();
                    } else {
                        ImportHelper.ImportFile(
                            [
                                {
                                    Tag: ImportFileTag.JS,
                                    Attributes: {
                                        type: 'text/javascript',
                                        src: '../../../utils/axios.min.js'
                                    }
                                }],
                            () => {
                                this.Axios = new (<any>window).axios;
                                this.AxiosInstance = this.Axios.create({});
                                setup();
                            });
                    }
                });
        });

        return promise;
    }

    /**
     * 使用Axios上传文件
     * @param file
     * @param onProgress
     */
    async UseAxiosUploadFile(file: RawFile, onProgress: (progress: UploadProgress) => void) {
        const promise = new Promise<void>(async (resolve, reject) => {
            let formData = new FormData();
            formData.append('file', file.File);

            this.AxiosInstance.post(
                ApiUri.SingleFile(file.Name),
                formData,
                {
                    onUploadProgress: progress => {
                        onProgress({ PreLoaded: progress.total, Loaded: progress.loaded, Total: progress.total });
                    },
                    cancelToken: new this.Axios.CancelToken(cancelToken => {
                        this.CancelTokenList[file.MD5] = cancelToken;
                    })
                })
                .then((response: { data: ResponseData_T<FileInfo> }) => {
                    delete this.CancelTokenList[file.MD5];

                    if (response.data.Success) {
                        file.FileInfo = response.data.Data;
                        resolve();
                    }
                    else
                        reject(new Error(response.data.Message));
                })
                .catch(e => {
                    delete this.CancelTokenList[file.MD5];
                    reject(e);
                });
        });

        return promise;
    }

    /**
     * 使用Axios上传分片文件
     * @param handlerIndex
     * @param validation
     * @param chunk
     * @param onProgress
     */
    async UseAxiosUploadChunkFile(
        handlerIndex: number,
        validation: PreUploadChunkFileResponse,
        chunk: ChunkFile,
        onProgress: (progress: UploadProgress) => void) {
        const promise = new Promise<void>(async (resolve, reject) => {
            let formData = new FormData();
            formData.append('file', chunk.Blob);

            this.AxiosInstance.post(
                ApiUri.UploadSingleChunkfile(validation.Key, chunk.MD5),
                formData,
                {
                    onUploadProgress: progress => {
                        onProgress({ PreLoaded: progress.total, Loaded: progress.loaded, Total: progress.total });
                    },
                    cancelToken: new this.Axios.CancelToken(cancelToken => {
                        this.CancelTokenList[chunk.MD5] = cancelToken;
                    })
                })
                .then((response: { data: ResponseData }) => {
                    delete this.CancelTokenList[chunk.MD5];

                    if (response.data.Success)
                        resolve();
                    else
                        reject(new Error(response.data.Message));
                })
                .catch(e => {
                    delete this.CancelTokenList[chunk.MD5];
                    reject(e);
                });
        });

        return promise;
    }

    /**
     * 使用上传单元上传文件
     * @param file
     * @param onProgress
     */
    async UseWorkerUploadFile(file: RawFile, onProgress: (progress: UploadProgress) => void): Promise<void> {
        const promise = new Promise<void>(async (resolve, reject) => {
            let buffer: ArrayBuffer;
            try {
                buffer = await this.FileReaders[0].readAsArrayBuffer(file.File);
            } catch (e) {
                reject(e);
                return;
            }

            try {
                let fileInfo = await this.WorkerPostMessage(0, { Type: UploadWorkerMessageType.文件, Data: { MD5: file.MD5, Buffer: buffer, Type: file.File.type, Extension: file.Extension, Name: file.Name } }, onProgress) as FileInfo;

                file.FileInfo = fileInfo;
            } catch (e) {
                reject(e);
                return;
            }

            resolve();
        });

        return promise;
    }

    /**
     * 使用上传单元上传分片文件
     * @param handlerIndex
     * @param validation
     * @param chunk
     * @param onProgress
     */
    async UseWorkerUploadChunkFile(
        handlerIndex: number,
        validation: PreUploadChunkFileResponse,
        chunk: ChunkFile,
        onProgress: (progress: UploadProgress) => void) {
        const promise = new Promise<void>(async (resolve, reject) => {
            let buffer: ArrayBuffer;
            try {
                buffer = await this.FileReaders[handlerIndex].readAsArrayBuffer(chunk.Blob);
            } catch (e) {
                reject(e);
                return;
            }

            try {
                await this.WorkerPostMessage(
                    handlerIndex,
                    { Type: UploadWorkerMessageType.分片文件, Data: { Key: validation.Key, MD5: chunk.MD5, Buffer: buffer } },
                    onProgress);
            } catch (e) {
                reject(e);
                return;
            }

            resolve();
        });

        return promise;
    }

    /**
     * 发送数据给子线程上传单元
     * @param data
     */
    async WorkerPostMessage(
        handlerIndex: number,
        data: UploadWorkerMessage<UploadWorkerSetUpAxiosMessage | UploadWorkerFileMessage | UploadWorkerChunkFileMessage | UploadWorkerCancelMessage> | null,
        onProgress?: (progress: UploadProgress) => void): Promise<FileInfo | void> {

        let unit = this.WorkerUnits[handlerIndex];
        unit.used = true;

        const promise = new Promise<FileInfo | void>((resolve, reject) => {
            unit.worker.onmessage = (event: MessageEvent<UploadWorkerNoticeMessage>) => {
                switch (event.data.Type) {
                    case UploadWorkerNoticeMessageType.设置成功:
                        resolve();
                        break;
                    case UploadWorkerNoticeMessageType.设置失败:
                    case UploadWorkerNoticeMessageType.设置异常:
                        reject(new Error(event.data.Message));
                        break;
                    case UploadWorkerNoticeMessageType.文件上传进度:
                        onProgress({ PreLoaded: event.data.Progress.total, Loaded: event.data.Progress.loaded, Total: event.data.Progress.total });
                        break;
                    case UploadWorkerNoticeMessageType.文件上传成功:
                        if ('undefined' !== typeof event.data.FileInfo) {
                            resolve(event.data.FileInfo);
                        } else
                            resolve();
                        break;
                    case UploadWorkerNoticeMessageType.文件上传失败:
                    case UploadWorkerNoticeMessageType.文件上传异常:
                        reject(new Error(event.data.Message));
                        break;
                    case UploadWorkerNoticeMessageType.取消文件上传:
                        reject();
                        break;
                    default:
                        reject(`未知的UploadWorkerNoticeMessage.Type: ${event.data.Type}.`);
                }

                unit.used = false;
            }

            unit.worker.onerror = event => {
                reject(event.error);
                unit.used = false;
            }

            if (data == null || (data.Type !== UploadWorkerMessageType.文件 && data.Type !== UploadWorkerMessageType.分片文件))
                unit.worker.postMessage(data);
            else
                unit.worker.postMessage(data, [(<any>data.Data).Buffer]);
        });

        return promise;
    }

    /**
     * 上传文件
     * @param file 文件
     * @param onProgress 监听进度
     */
    async UploadFile(file: RawFile, onProgress: (progress: UploadProgress) => void): Promise<void> {
        const promise = new Promise<void>(async (resolve, reject) => {
            //文件MD5校验
            let validation: ValidationMD5Response;
            try {
                validation = await this.ValidationFileMD5(file.MD5, file.Name);
            } catch (e) {
                reject(e);
                return;
            }

            if (validation.Uploaded) {
                file.FileInfo = validation.FileInfo;
                resolve();
                return;
            }

            if (file.NeedSection) {
                /**
                 * 整体进度
                 * */
                let progress: UploadProgress = { PreLoaded: 0, Loaded: 0, Total: file.Size };

                /**
                 * 上传分片文件
                 * */
                const upload = async () => {
                    if (this.Finished) {
                        return;
                    }

                    if (this.ChunkHandlerQueue.length === 0) {
                        for (let i = 0; i < this.ChunkHandler.length; i++) {
                            if (this.ChunkHandler[i]) {
                                return;
                            }
                        }

                        //所有分片全部上传完毕
                        try {
                            file.FileInfo = await this.UploadChunkFileFinished(file.MD5, file.Specs, file.Chunks.length, file.File.type, file.Extension, file.Name);

                            this.Finished = true;
                            resolve();
                        } catch (e) {
                            reject(e);
                        }
                        return;
                    }

                    let handlerIndex: number | null = null;
                    for (let i = 0; i < this.ChunkHandler.length; i++) {
                        if (!this.ChunkHandler[i]) {
                            handlerIndex = i;
                            break;
                        }
                    }

                    if (handlerIndex == null)
                        return;

                    this.ChunkHandler[handlerIndex] = true;

                    /**
                     * 上传下一个分片文件
                     * */
                    const next = () => {
                        onProgress(progress);

                        this.ChunkHandler[handlerIndex] = false;

                        upload();
                    };

                    const chunk = this.ChunkHandlerQueue.shift();

                    /**
                     * 分片进度
                     * */
                    let chunk_progress: UploadProgress = { PreLoaded: chunk.Size, Loaded: 0, Total: chunk.Size };

                    /**
                     * 处理分片进度
                     * @param _chunk_progress
                     */
                    const handlerProgress = (_chunk_progress: UploadProgress) => {
                        progress.Loaded += (_chunk_progress.Loaded - chunk_progress.Loaded);

                        chunk_progress.Loaded = _chunk_progress.Loaded;

                        onProgress(progress);
                    }

                    let validation: PreUploadChunkFileResponse;
                    try {
                        validation = await this.PreUploadChunkfile(file.MD5, chunk.MD5, chunk.Index, file.Specs, chunk.Forced);
                    } catch (e) {
                        reject(e);
                        return;
                    }

                    progress.PreLoaded += chunk_progress.PreLoaded;
                    onProgress(progress);

                    /**
                     * 继续上传
                     * */
                    const proceed = async () => {
                        if (this.WorkerSupported) {
                            await this.UseWorkerUploadChunkFile(handlerIndex, validation, chunk, handlerProgress);
                        } else {
                            await this.UseAxiosUploadChunkFile(handlerIndex, validation, chunk, handlerProgress);
                        }
                    };

                    switch (validation.State) {
                        case PreUploadChunkFileState.允许上传:
                            await proceed();
                            break;
                        case PreUploadChunkFileState.全部跳过:
                            this.ChunkHandlerQueue.length = 0;
                            return;
                        case PreUploadChunkFileState.推迟上传:
                            this.ChunkHandlerQueue.push(chunk);

                            this.DelayTimes++;
                            if (this.DelayTimes >= this.ChunkHandlerQueue.length) {
                                this.DelayTimes = 0;
                                chunk.Forced = true;
                            }
                            break;
                        case PreUploadChunkFileState.跳过:
                            handlerProgress({ Loaded: chunk.Size, Total: chunk.Size });
                            break;
                    }

                    next();
                }

                /**
                 * 添加至队列
                 * */
                const push = (chunk: ChunkFile) => {
                    this.ChunkHandlerQueue.push(chunk);
                    upload();
                }

                for (let chunk of file.Chunks) {
                    push(chunk);
                }
            } else {
                try {
                    if (this.WorkerSupported) {
                        await this.UseWorkerUploadFile(file, onProgress);
                    } else {
                        await this.UseAxiosUploadFile(file, onProgress);
                    }
                    resolve();
                } catch (e) {
                    reject(e);
                }
            }
        });
        return promise;
    }

    /**
     * 文件MD5值校验
     * @param md5 文件MD5值
     * @param name 文件重命名
     */
    async ValidationFileMD5(md5: string, name: string)
        : Promise<ValidationMD5Response> {
        const promise = new Promise<ValidationMD5Response>((resolve, reject) => {
            this.AxiosInstance.get(ApiUri.ValidationFileMD5(md5, name))
                .then(function (response: { data: ResponseData_T<ValidationMD5Response> }) {
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

        return promise;
    }

    /**
     * 分片文件MD5值校验
     * @param file_md5 文件MD5值
     * @param md5 分片文件MD5值
     * @param index 分片文件索引
     * @param specs 分片文件规格
     * @param forced 强制上传
     */
    async PreUploadChunkfile(file_md5: string, md5: string, index: number, specs: number, forced: boolean = false)
        : Promise<PreUploadChunkFileResponse> {
        const promise = new Promise<PreUploadChunkFileResponse>((resolve, reject) => {
            this.AxiosInstance.get(ApiUri.PreUploadChunkfile(file_md5, md5, index, specs, forced))
                .then(function (response: { data: ResponseData_T<PreUploadChunkFileResponse> }) {
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

        return promise;
    }

    /**
     * 分片文件全部上传完毕
     * @param file_md5 文件MD5值
     * @param specs 分片文件规格
     * @param total 分片文件总数
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件名
     */
    async UploadChunkFileFinished(file_md5: string, specs: number, total: number, type: string, extension: string, filename: string)
        : Promise<FileInfo> {
        const promise = new Promise<FileInfo>((resolve, reject) => {
            this.AxiosInstance.get(ApiUri.UploadChunkfileFinished(file_md5, specs, total, type, extension, filename))
                .then(function (response: { data: ResponseData_T<FileInfo> }) {
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

        return promise;
    }

    /**
     * 关闭全部上传单元
     */
    CloseAll() {
        if (this.EnableWorker && this.WorkerSupported) {
            for (let i = 0; i < this.WorkerUnits.length; i++) {
                this.Close(i);
            }
        } else {
            for (let i = 0; i < this.FileReaders.length; i++) {
                this.Close(i);
            }
        }
    }

    /**
     * 关闭上传单元
     * @param index 计算单元的索引
     */
    Close(index: number) {
        this.FileReaders[index].Close();

        if (this.EnableWorker && this.WorkerSupported) {
            this.WorkerPostMessage(index, null)
        }
    }
}