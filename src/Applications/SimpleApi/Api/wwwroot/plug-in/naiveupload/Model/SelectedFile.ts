/**
 * 选择的文件
 * */
class SelectedFile {
    /**
     * 
     * @param file 文件
     */
    constructor(file: File) {
        const pointIndex = file.name.lastIndexOf('.');
        this.Name = file.name.substring(0, pointIndex);
        this.Extension = file.name.substring(pointIndex);
        this.ExtensionLower = (<any>window)._.toLower(this.Extension);
    }

    /**
     * 源文件索引
     */
    RawIndex: number;

    /**
     * 文件名
     */
    Name: string;

    /**
     * 拓展名
     * 包括前缀(.)
     */
    Extension: string;

    /**
     * 拓展名全小写
     * 包括前缀(.)
     */
    ExtensionLower: string;

    /**
     * 文件大小
     */
    Size: string = null;

    /**
     * 文件类型
     */
    FileType: string = FileType.未知;

    /**
     * 缩略图
     */
    Thumbnail: string = '/filetypes/empty.jpg';

    /**
     * 类名
     */
    Class: string[] = [];

    /**
     * 鼠标悬浮
     */
    Hover: boolean = false;

    /**
     * 正在重命名
     */
    Rename: boolean = false;

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

    /**
     * 处理完毕
     */
    Done: boolean = false;

    /**
     * 重试次数
     */
    ReTry: number = 0;

    /**
     * 发生异常
     */
    Error: boolean = false;

    /**
     * 异常信息
     */
    ErrorMessage: string;

    /**
     * 真实进度（百分比）
     */
    Percent: number = 0;

    /**
     * 虚拟进度（百分比）
     */
    VirtualPercent: number = 0;

    /**
     * 已暂停
     */
    Paused: boolean = false;

    /**
     * 已取消
     */
    Canceled: boolean = false;
}