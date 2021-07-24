/**
 * 通知类型
 * */
const enum UploadWorkerNoticeMessageType {
    设置成功 = 'setup-success',
    设置失败 = 'setup-fail',
    设置异常 = 'setup-error',
    文件上传进度 = 'file-upload-progress',
    文件上传成功 = 'file-upload-success',
    文件上传失败 = 'file-upload-fail',
    文件上传异常 = 'file-upload-error',
    取消文件上传 = 'file-upload-cancel'
}