/**
 * 文件库信息
 * */
class LibraryInfo {
    /**
     * 文件类型
     */
    FileType: string;

    /**
     * 文件总数
     */
    Total: number;

    /**
     * 文件总字节数
     */
    Bytes: string;

    /**
     * 文件占用存储空间
     */
    Size: string;
}

/**
 * 文件夹信息
 * */
class FolderInfo extends LibraryInfo {
    /**
     * 类名
     */
    Class: string;

    /**
     * 鼠标悬浮状态
     * 默认 false
     */
    Hover: boolean = false;

    /**
     * 展开状态
     * 默认 false
     */
    Open: boolean = false;

    /**
     * 文件列表
     */
    Files: FileListInfo;
}

/**
 * 文件列表信息
 * */
class FileListInfo {
    /**
     * 初始化状态
     * 默认 false
     */
    Init: boolean = false;

    /**
     * 加载状态
     * 默认 true
     */
    Loading: boolean = true;

    /**
     * 异常信息
     */
    Error: string;

    /**
     * 文件类型全选状态
     * 默认 true
     */
    CheckAllFileType: boolean = true;

    /**
     * 文件类型未全选状态
     * 默认 false
     */
    IsFileTypeIndeterminate: boolean = false;

    /**
     * 存储类型全选状态
     * 默认 true
     */
    CheckAllStorageType: boolean = true;

    /**
     * 存储类型未全选状态
     * 默认 false
     */
    IsStorageTypeIndeterminate: boolean = false;

    /**
     * 文件状态全选状态
     * 默认 true
     */
    CheckAllFileState: boolean = true;

    /**
     * 文件状态未全选状态
     * 默认 false
     */
    IsFileStateIndeterminate: boolean = false;

    /**
     * 筛选条件
     */
    Filters: FileListFilters = new FileListFilters();

    /**
     * 分页设置
     */
    Pagination: Pagination = new Pagination();

    /**
     * 列表数据
     */
    List: FileInfo[] = [];
}

/**
 * 文件列表筛选条件
 * */
class FileListFilters {
    /**
     * 文件类型集合
     */
    FileType: string[];

    /**
     * 存储类型集合
     */
    StorageType: string[];

    /**
     * 文件状态集合
     */
    State: string[];

    /**
     * 搜索日期范围
     * 默认 近7天
     */
    DateRang: Date[] = [new Date(new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7)), new Date()];

    /**
     * 搜索文件名
     */
    Name: string;

    /**
     * 搜索MD5校验值
     */
    MD5: string;

    /**
     * 搜索文件内容类型
     */
    ContentType: string;

    /**
     * 搜索文件拓展名
     */
    Extension: string;

    /**
     * 搜索服务器标识
     */
    ServerKey: string;
}