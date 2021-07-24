/**
 * MD5校验输出信息
 * */
class ValidationMD5Response {
    /**
     * 是否已上传过了
     * 如已上传,则返回文件信息
     */
    Uploaded: boolean;

    /**
     * 文件信息
     */
    FileInfo: FileInfo;
}