/**
 * 文件批量上传设置
 * */
class MultipleUploadSetting {
    constructor() {
        return this;
    }

    /**
     * 允许的文件类型
     */
    Accept: string[] = ['image/*', 'audio/*', 'video/*', '.txt', '.pdf', '.doc', 'docx', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.xls', 'xlsx', '.zip', '.rar', '.7z'];

    /**
     * 是否允许多选
     * 默认允许
     */
    MultipleSelect: boolean = true;

    /**
     * 文件数量上限
     * 默认10个
     */
    Limit: number = 10;

    /**
     * 文件上传并发数
     * 默认3个
     */
    ConcurrentFile: number = 3;

    /**
     * 分片文件上传并发数
     * 默认3个
     */
    ConcurrentChunkFile: number = 3;

    /**
     * 说明文字
     */
    Explain: string = '单击或拖动文件到此区域即可上传';

    /**
     * 附加说明
     */
    Tip: string = '严禁上传存在违法违规内容的文件';

    /**
     * 主题
     * 默认卡片主题
     */
    Theme: string = UploadTheme.卡片主题;

    /**
     * 模式
     * 默认自动挡
     */
    Mode: string = UploadMode.自动挡;

    /**
     * 是否启用文件切片
     * 默认启用
     */
    EnableChunk: boolean = true;

    /**
     * 切片文件字节数
     * 默认2M
     */
    ChunkSize: number = 2 * 1024 * 1024;

    /**
     * 发生错误时重试次数
     * 默认3次
     */
    Retry: number = 3;

    /**
     * Http请求工具
     */
    Axios: any;

    /**
     * 请求头设置
     */
    Headers: Record<string, string> = {};//此类型等同于 { [P in string]: string }

    /**
     * 文件校验前执行
     * @param file 文件
     * @returns 是否保留该文件
     */
    BeforeCheck?: (file: File) => Promise<boolean>;

    /**
     * 文件校验结束后执行
     * @param file 文件
     */
    AfterCheck?: (rawFile: RawFile) => Promise<void>;

    /**
     * 文件校验全部校验结束后执行
     * @param files 文件集合
     */
    AfterCheckAll?: (rawFiles: RawFile[]) => Promise<void>;

    /**
     * 文件上传后执行
     * @param rawFile 文件
     */
    AfterUpload?: (rawFile: RawFile) => Promise<void>;

    /**
     * 所有文件上传后执行
     * 此方法不会等待
     * @param rawFiles 文件集合
     */
    AfterUploadAll?: (rawFiles: RawFile[]) => Promise<void>;

    /**
     * 是否启用Worker
     * 默认启用（如果浏览器支持的话）
     */
    EnableWorker: boolean = true;
}