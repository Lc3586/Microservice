/**
 * 文件上传子线程文件传输信息
 * */
class UploadWorkerFileMessage {
    /**
     * 文件哈希值
     */
    MD5: string;

    /**
     * 数据
     */
    Buffer: ArrayBuffer;

    /**
     * 文件类型
     */
    Type: string;

    /**
     * 文件拓展名
     */
    Extension: string;

    /**
     * 文件重命名
     */
    Name: string;
}