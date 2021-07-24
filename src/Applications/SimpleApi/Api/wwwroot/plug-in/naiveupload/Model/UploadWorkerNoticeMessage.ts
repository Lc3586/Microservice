/**
 * 文件上传子线程通知传输信息
 * */
class UploadWorkerNoticeMessage {
    /**
     * 类型
     */
    Type: UploadWorkerNoticeMessageType;

    /**
     * 文件哈希值
     */
    MD5: string;

    /**
     * 进度信息
     */
    Progress?: any;

    /**
     * 文件信息
     */
    FileInfo?: any;

    /**
     * 错误信息
     */
    Message?: string;
}