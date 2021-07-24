/**
 * 文件上传子线程传输信息类型
 * */
const enum UploadWorkerMessageType {
    设置Axios = 'SetUpAxios',
    文件 = 'File',
    分片文件 = 'ChunkFile',
    取消上传 = 'Cancel',
    通知 = 'Notice',
}