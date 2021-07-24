/**
 * 文件上传子线程传输信息
 * */
class UploadWorkerMessage<T> {
    /**
     * 类型
     */
    Type: UploadWorkerMessageType;

    /**
     * 数据
     */
    Data: T;
}