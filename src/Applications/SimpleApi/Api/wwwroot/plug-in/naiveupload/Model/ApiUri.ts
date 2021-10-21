/**
 * 接口地址
 * */
class ApiUri {
    /**
     * SA登录验证
     * */
    static SAAuthorized: string = '/sa/authorized';

    /**
     * SA登录
     * */
    static SALogin: string = '/sa/login';

    /**
     * 获取令牌
     * */
    static GetToken: string = '/jwt/get-token';

    /**
     * 刷新令牌
     * */
    static RefreshToken: string = '/jwt/refresh-token';

    /**
     * 个人文件信息重命名
     * @param id Id
     * @param filename 文件名
     * */
    static PersonalFileInfoRename: (id: string, filename: string) => string = (id: string, filename: string) => `/personal-file-info/rename/${id}?filename=${filename}`;

    /**
     * 获取个人文件信息编辑数据
     * @param id Id
     * */
    static PersonalFileInfoEditData: (id: string) => string = (id: string) => `/personal-file-info/edit-data/${id}`;

    /**
     * 个人文件信息编辑
     * */
    static PersonalFileInfoEdit: string = '/personal-file-info/edit';

    /**
     * 获取文件类型
     * @param extension 文件拓展名
     * */
    static FileTypeByExtension: (extension: string) => string = (extension: string) => `/file/type-by-extension/${extension}`;

    /**
     * 获取文件类型
     * @param mimetype MIME类型
     * */
    static FileTypeByMIME: (mimetype: string) => string = (mimetype: string) => `/file/type-by-mimetype?mimetype=${mimetype}`;

    /**
     * 获取文件类型预览图链接地址
     * @param extension 文件拓展名
     * */
    static FileTypeImageUrl: (extension: string) => string = (extension: string) => `/file/type-image/${extension}`;

    /**
     * 获取文件大小
     * @param length 文件字节数
     * */
    static FileSize: (length: number | string) => string = (length: number | string) => `/file/size/${length}`;

    /**
     * 获取文件上传配置树状列表
     * */
    static FileUploadConfigTreeList: string = '/file-upload-config/tree-list';

    /**
     * 获取文件上传配置详情数据
     * @param Id 上传配置Id
     * */
    static FileUploadConfigDetailData: (id: string) => string = (id: string) => `/file-upload-config/detail-data/${id}`;

    /**
     * 获取文件上传配置数据
     * @param Id 上传配置Id
     * */
    static FileUploadConfigData: (id: string) => string = (id: string) => `/file-upload-config/data/${id}`;

    /**
     * 获取当前登录账号的文件上传配置授权树状列表数据
     */
    static GetCurrentAccountCFUCTree: string = '/authorities/current-account-data-cfuc-tree';

    /**
     * 获取文件库信息
     */
    static GetLibraryInfo: string = '/file/library-info';

    /**
     * 获取所有文件类型
     */
    static GetFileTypes: string = '/file/filetypes';

    /**
     * 获取所有存储类型
     */
    static GetStorageTypes: string = '/file/storagetypes';

    /**
     * 获取所有状态类型
     */
    static GetFileStates: string = '/file/filestates';

    /**
     * 获取文件列表数据
     */
    static GetFileList: string = '/file/list';

    /**
     * 获取文件详情数据
     * @param Id 上传配置Id
     */
    static GetFileDetail: (id: string) => string = (id: string) => `/file/detail-data/${id}`;

    /**
     * 获取文件详情数据
     * @param Id 上传配置Id
     * @param format 获取有关输入多媒体流的容器格式的信息
     * @param streams 获取有关输入多媒体流中包含的每个媒体流的信息
     * @param chapters 获取有关以该格式存储的章节的信息
     * @param programs 获取有关程序及其输入多媒体流中包含的流的信息
     * @param version 获取与程序版本有关的信息、获取与库版本有关的信息、获取与程序和库版本有关的信息
     */
    static GetVideoInfo: (id: string, format?: boolean, streams?: boolean, chapters?: boolean, programs?: boolean, version?: boolean) => string = (id: string, format: boolean = true, streams: boolean = false, chapters: boolean = false, programs: boolean = false, version: boolean = false) => `/file/video-info/${id}?format=${format}&streams=${streams}&chapters=${chapters}&programs=${programs}&version=${version}`;

    /**
     * 预备上传文件
     * @param configId 上传配置Id
     * @param md5 文件哈希值
     * @param filename 文件重命名
     * @param section 是否分片处理（默认否）
     * @param type 文件类型（单文件上传时忽略此参数）
     * @param extension 文件拓展名（单文件上传时忽略此参数）
     * @param specs 分片文件规格（单文件上传时忽略此参数）
     * @param total 分片文件总数（单文件上传时忽略此参数）
     * */
    static PreUploadFile: (configId: string, md5: string, filename: string, section?: boolean, type?: string, extension?: string, specs?: number, total?: number) => string = (configId: string, md5: string, filename: string, section: boolean = false, type: string, extension: string, specs: number, total: number) => `/file-upload/pre-file/${configId}/${md5}?filename=${filename}&section=${section}&type=${type || ''}&extension=${extension || ''}&specs=${specs || ''}&total=${total || ''}`;

    /**
     * 预备上传分片文件
     * @param file_md5 文件哈希值
     * @param md5 分片文件哈希值
     * @param index 分片文件索引
     * @param specs 分片文件规格
     * @param forced 强制上传
     */
    static PreUploadChunkfile(file_md5: string, md5: string, index: number, specs: number, forced: boolean = false): string {
        return `/file-upload/pre-chunkfile/${file_md5}/${md5}/${index}/${specs}?forced=${forced}`
    };

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfile(key: string, md5: string): string {
        return `/file-upload/single-chunkfile/${key}/${md5}`;
    }

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfileByArrayBuffer(key: string, md5: string): string {
        return `/file-upload/single-chunkfile-arraybuffer/${key}/${md5}`;
    }

    /**
     * 分片文件全部上传完毕
     * @param file_md5 文件哈希值
     * @param specs 分片文件规格
     * @param total 分片文件总数
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件重命名
     * */
    static UploadChunkfileFinished(file_md5: string, specs: number, total: number, type: string, extension: string, filename: string): string {
        return `/file-upload/chunkfile-finished/${file_md5}/${specs}/${total}?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param configId 上传配置Id
     * @param filename 文件重命名
     * */
    static SingleFile(configId: string, filename: string): string {
        return `/file-upload/single-file/${configId}?filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param configId 上传配置Id
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件重命名
     * */
    static SingleFileByArrayBuffer(configId: string, type: string, extension: string, filename: string): string {
        return `/file-upload/single-file-arraybuffer/${configId}?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 预览文件
     * @param id 文件Id
     * @param width 指定宽度 （默认值500）
     * @param height 指定高度 （默认值500）
     * @param time 视频的时间轴位置（默认值: 00:00:00.001）
     * */
    static Preview(id: string, width?: number, height?: number, time?: string) {
        return `/file/preview/${id}?width=${width ?? ''}&height=${height ?? ''}&time=${time ?? ''}`;
    }

    /**
     * 浏览文件
     * @param id 文件Id
     * */
    static Browse(id: string) {
        return `/file/browse/${id}`;
    }

    /**
     * 下载文件
     * @param id 文件Id
     * */
    static Download(id: string) {
        return `/file/download/${id}`;
    }

    /**
     * 删除文件
     * */
    static Delete: string = '/file/list';

    /**
     * 分片文件合并任务中心
     * */
    static CFMTHub: string = '/cfmthub';
}