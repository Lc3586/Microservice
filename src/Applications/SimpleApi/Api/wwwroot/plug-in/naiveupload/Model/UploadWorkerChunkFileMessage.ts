/**
 * 文件上传子线程分片文件传输信息
 * */
class UploadWorkerChunkFileMessage {
    /**
     * 上传标识
     */
    Key: string;

    /**
     * 文件哈希值
     */
    MD5: string;

    /**
     * 数据
     */
    Buffer: ArrayBuffer;
}