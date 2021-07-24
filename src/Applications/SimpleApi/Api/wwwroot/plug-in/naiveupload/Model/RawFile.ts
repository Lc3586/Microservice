/**
 * 源文件
 * */
class RawFile {
    /**
     * 
     * @param file 源文件
     */
    constructor(file: File) {
        this.File = file;
        this.ObjectURL = URL.createObjectURL(file);
    }

    /**
     * 文件
     */
    File: File;

    /**
     * 资源地址
     */
    ObjectURL: string;

    /**
     * 哈希值
     */
    MD5: string;

    /**
     * 文件拓展名
     */
    Extension: string;

    /**
     * 文件重命名
     */
    Name: string;

    /**
     * 需要切片处理
     */
    NeedSection: boolean = false;

    /**
     * 分片规格
     */
    Specs: number;

    /**
     * 分片文件上传标识
     */
    Key: string;

    /**
     * 切片集合
     */
    Chunks: ChunkFile[] = [];

    /**
     * 切片索引队列
     */
    ChunkIndexQueue: number[] = [];

    /**
     * 文件信息
     */
    FileInfo: FileInfo;
}