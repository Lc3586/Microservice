/**
 * 切片文件
 * */
class ChunkFile {
    /**
     * 
     * @param index 索引
     * @param blob 二进制数据
     */
    constructor(index: number, blob: Blob) {
        this.Index = index;
        this.Blob = blob;
    }

    /***
     * 索引 
     */
    Index: number;

    /***
     * 二进制数据
     */
    Blob: Blob;

    /***
     * 哈希值 
     */
    MD5: string;

    /**
     * 强制上传
     */
    Forced: boolean = false;

    /**
     * 正在校验MD5
     */
    Checking: boolean = false;

    /**
     * 已校验MD5
     */
    Checked: boolean = false;

    /**
     * 正在上传
     */
    Uploading: boolean = false;

    /**
     * 已上传
     */
    Uploaded: boolean = false;

    /***
     * 处理完毕 
     */
    Done: boolean = false;

    /**
     * 发生异常
     */
    Error: boolean = false;

    /**
     * 异常信息
     */
    ErrorMessage: string;
}