/**
 * 文件上传子线程类
 * */
class UploadWorker {
    constructor(self: any) {
        this.Self = self;
        this.Axios = self.axios;
        this.AxiosInstance = self.axios.create({});
    }

    /**
     * 当前Worker对象
     */
    Self: any;

    /**
     * http请求工具
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
     * 设置Axios
     * @param headers 请求头设置
     */
    SetUpAxios(headers: Record<string, string>) {
        this.AxiosInstance.defaults.headers.post['Content-Type'] = 'applicatoin/octet-stream';
        if (headers != null) {
            for (const key in headers) {
                this.AxiosInstance.defaults.headers.common[key] = headers[key];
            }
        }
    }

    /**
     * 上传单个文件
     * @param md5 文件哈希指
     * @param buffer 数据
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件重命名
     */
    SingleFile(configId: string, md5: string, buffer: ArrayBuffer, type: string, extension: string, filename: string) {
        this.AxiosInstance.post(
            ApiUri.SingleFileByArrayBuffer(configId, type, extension, filename),
            buffer,
            {
                onUploadProgress: progress => {
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传进度, MD5: md5, Progress: { loaded: progress.loaded, total: progress.total } });
                },
                cancelToken: new this.Axios.CancelToken(cancelToken => {
                    this.CancelTokenList[md5] = cancelToken;
                })
            })
            .then((response: { data: ResponseData_T<PersonalFileInfo> }) => {
                delete this.CancelTokenList[md5];

                if (response.data.Success)
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传成功, MD5: md5, FileInfo: response.data.Data });
                else
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传失败, MD5: md5, Message: response.data.Message });
            })
            .catch((e: Error) => {
                console.error(e);
                delete this.CancelTokenList[md5];
                this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传异常, MD5: md5, Message: e.message });
            });
    }

    /**
     * 上传单个分片文件
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * @param buffer 数据
     */
    SingleChunkFile(key: string, md5: string, buffer: ArrayBuffer) {
        this.AxiosInstance.post(
            ApiUri.UploadSingleChunkfileByArrayBuffer(key, md5),
            buffer,
            {
                onUploadProgress: progress => {
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传进度, MD5: md5, Progress: { loaded: progress.loaded, total: progress.total } });
                },
                cancelToken: new this.Axios.CancelToken(cancelToken => {
                    this.CancelTokenList[md5] = cancelToken;
                })
            })
            .then((response: { data: ResponseData_T<PersonalFileInfo> }) => {
                delete this.CancelTokenList[md5];

                if (response.data.Success)
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传成功, MD5: md5 });
                else
                    this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传失败, MD5: md5, Message: response.data.Message });
            })
            .catch(e => {
                console.error(e);
                delete this.CancelTokenList[md5];
                this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.文件上传异常, MD5: md5, Message: e.message });
            });
    }

    /**
     * 取消上传
     * @param md5 文件哈希值/分片文件哈希值
     */
    Cancel(md5: string) {
        if (md5 in this.CancelTokenList)
            this.CancelTokenList[md5]();

        this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.取消文件上传, MD5: md5 });
    }

    /**
     * 接收数据
     * @param this
     * @param event
     */
    OnMessage(this: UploadWorker, event: MessageEvent<UploadWorkerMessage<UploadWorkerSetUpAxiosMessage | UploadWorkerFileMessage | UploadWorkerChunkFileMessage | UploadWorkerCancelMessage> | null>) {
        // do some work here 
        if (event.data === null) {
            this.Self.close();
            return;
        }

        let data;

        switch (event.data.Type) {
            case UploadWorkerMessageType.设置Axios:
                data = event.data.Data as UploadWorkerSetUpAxiosMessage;
                this.SetUpAxios(data.Headers);
                this.Self.postMessage({ Type: UploadWorkerNoticeMessageType.设置成功 });
                break;
            case UploadWorkerMessageType.文件:
                data = event.data.Data as UploadWorkerFileMessage;
                this.SingleFile(data.ConfigId, data.MD5, data.Buffer, data.Type, data.Extension, data.Name);
                break;
            case UploadWorkerMessageType.分片文件:
                data = event.data.Data as UploadWorkerChunkFileMessage;
                this.SingleChunkFile(data.Key, data.MD5, data.Buffer);
                break;
            case UploadWorkerMessageType.取消上传:
                data = event.data.Data as UploadWorkerCancelMessage;
                this.Cancel(data.MD5);
                break;
            default:
        }
    }
}

// proper initialization
if ('undefined' === typeof window
    && 'function' === typeof (<any>self).importScripts) {
    (<any>self).importScripts(
        "../../../utils/axios.min.js",
        "../../../utils/baseUrl.js",
        "../Model/ApiUri.js");
    (<any>self).addEventListener('message', this.onMessage);

    let worker = new UploadWorker(self);

    (<any>self).addEventListener('message', (event: MessageEvent<any>) => { worker.OnMessage(event); });
}
