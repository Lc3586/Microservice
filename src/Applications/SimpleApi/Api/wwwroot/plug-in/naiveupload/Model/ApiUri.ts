/**
 * 接口地址
 * */
class ApiUri {
    /**
     * 根路径
     */
    static BaseUrl: string = (<any>window).BaseUrl;

    /**
     * SA登录验证
     * */
    static SAAuthorized: string = this.BaseUrl + '/sa/authorized';

    /**
     * SA登录
     * */
    static SALogin: string = this.BaseUrl + '/sa/login';

    /**
     * SA登出
     * @param returnUrl 登出后跳转地址
     * */
    static SALogout: (returnUrl: string) => string = (returnUrl: string) => `${this.BaseUrl}/sa/logout?ReturnUrl=${returnUrl}`;

    /**
     * JWT登录验证
     * */
    static JWTAuthorized: string = this.BaseUrl + '/jwt/authorized';

    /**
     * 获取令牌
     * */
    static GetToken: string = this.BaseUrl + '/jwt/get-token';

    /**
     * 刷新令牌
     * */
    static RefreshToken: string = this.BaseUrl + '/jwt/refresh-token';

    /**
     * 个人文件信息重命名
     * @param id Id
     * @param filename 文件名
     * */
    static PersonalFileInfoRename: (id: string, filename: string) => string = (id: string, filename: string) => `${this.BaseUrl}/personal-file-info/rename/${id}?filename=${filename}`;

    /**
     * 获取个人文件信息详情数据
     * @param id Id
     * */
    static PersonalFileInfoDetailData: (id: string) => string = (id: string) => `${this.BaseUrl}/personal-file-info/detail-data/${id}`;

    /**
     * 获取个人文件信息编辑数据
     * @param id Id
     * */
    static PersonalFileInfoEditData: (id: string) => string = (id: string) => `${this.BaseUrl}/personal-file-info/edit-data/${id}`;

    /**
     * 个人文件信息编辑
     * */
    static PersonalFileInfoEdit: string = this.BaseUrl + '/personal-file-info/edit';

    /**
     * 获取文件类型
     * @param extension 文件拓展名
     * */
    static FileTypeByExtension: (extension: string) => string = (extension: string) => `${this.BaseUrl}/file/type-by-extension/${extension}`;

    /**
     * 获取文件类型
     * @param mimetype MIME类型
     * */
    static FileTypeByMIME: (mimetype: string) => string = (mimetype: string) => `${this.BaseUrl}/file/type-by-mimetype?mimetype=${mimetype}`;

    /**
     * 获取文件类型预览图链接地址
     * @param extension 文件拓展名
     * */
    static FileTypeImageUrl: (extension: string) => string = (extension: string) => `${this.BaseUrl}/file/type-image/${extension}`;

    /**
     * 获取文件大小
     * @param length 文件字节数
     * */
    static FileSize: (length: number | string) => string = (length: number | string) => `${this.BaseUrl}/file/size/${length}`;

    /**
     * 获取文件上传配置树状列表
     * */
    static FileUploadConfigTreeList: string = this.BaseUrl + '/file-upload-config/tree-list';

    /**
     * 获取文件上传配置详情数据
     * @param Id 上传配置Id
     * */
    static FileUploadConfigDetailData: (id: string) => string = (id: string) => `${this.BaseUrl}/file-upload-config/detail-data/${id}`;

    /**
     * 获取文件上传配置数据
     * @param Id 上传配置Id
     * */
    static FileUploadConfigData: (id: string) => string = (id: string) => `${this.BaseUrl}/file-upload-config/data/${id}`;

    /**
     * 获取当前登录账号的文件上传配置授权树状列表数据
     */
    static GetCurrentAccountCFUCTree: string = this.BaseUrl + '/authorities/current-account-data-cfuc-tree';

    /**
     * 获取文件库信息
     */
    static GetLibraryInfo: string = this.BaseUrl + '/file/library-info';

    /**
     * 获取所有文件类型
     */
    static GetFileTypes: string = this.BaseUrl + '/file/filetypes';

    /**
     * 获取所有存储类型
     */
    static GetStorageTypes: string = this.BaseUrl + '/file/storagetypes';

    /**
     * 获取所有状态类型
     */
    static GetFileStates: string = this.BaseUrl + '/file/filestates';

    /**
     * 获取文件列表数据
     */
    static GetFileList: string = this.BaseUrl + '/file/list';

    /**
     * 获取文件详情数据
     * @param Id 上传配置Id
     */
    static GetFileDetail: (id: string) => string = (id: string) => `${this.BaseUrl}/file/detail-data/${id}`;

    /**
     * 获取文件详情数据
     * @param Id 上传配置Id
     * @param format 获取有关输入多媒体流的容器格式的信息
     * @param streams 获取有关输入多媒体流中包含的每个媒体流的信息
     * @param chapters 获取有关以该格式存储的章节的信息
     * @param programs 获取有关程序及其输入多媒体流中包含的流的信息
     * @param version 获取与程序版本有关的信息、获取与库版本有关的信息、获取与程序和库版本有关的信息
     */
    static GetVideoInfo: (id: string, format?: boolean, streams?: boolean, chapters?: boolean, programs?: boolean, version?: boolean) => string = (id: string, format: boolean = true, streams: boolean = false, chapters: boolean = false, programs: boolean = false, version: boolean = false) => `${this.BaseUrl}/file/video-info/${id}?format=${format}&streams=${streams}&chapters=${chapters}&programs=${programs}&version=${version}`;

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
    static PreUploadFile: (configId: string, md5: string, filename: string, section?: boolean, type?: string, extension?: string, specs?: number, total?: number) => string = (configId: string, md5: string, filename: string, section: boolean = false, type: string, extension: string, specs: number, total: number) => `${this.BaseUrl}/file-upload/pre-file/${configId}/${md5}?filename=${filename}&section=${section}&${[['type', type?.toString()], ['extension', extension?.toString()], ['specs', specs?.toString()]].map(item => { if (item[1] && item[1].length > 0) return `${item[0]}=${item[1]}`; }).join('&')}`;

    /**
     * 预备上传分片文件
     * @param file_md5 文件哈希值
     * @param md5 分片文件哈希值
     * @param index 分片文件索引
     * @param specs 分片文件规格
     * @param forced 强制上传
     */
    static PreUploadChunkfile(file_md5: string, md5: string, index: number, specs: number, forced: boolean = false): string {
        return `${this.BaseUrl}/file-upload/pre-chunkfile/${file_md5}/${md5}/${index}/${specs}?forced=${forced}`
    };

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfile(key: string, md5: string): string {
        return `${this.BaseUrl}/file-upload/single-chunkfile/${key}/${md5}`;
    }

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfileByArrayBuffer(key: string, md5: string): string {
        return `${this.BaseUrl}/file-upload/single-chunkfile-arraybuffer/${key}/${md5}`;
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
        return `${this.BaseUrl}/file-upload/chunkfile-finished/${file_md5}/${specs}/${total}?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param configId 上传配置Id
     * @param filename 文件重命名
     * */
    static SingleFile(configId: string, filename: string): string {
        return `${this.BaseUrl}/file-upload/single-file/${configId}?filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param configId 上传配置Id
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件重命名
     * */
    static SingleFileByArrayBuffer(configId: string, type: string, extension: string, filename: string): string {
        return `${this.BaseUrl}/file-upload/single-file-arraybuffer/${configId}?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 预览文件
     * @param id 文件Id
     * @param width 指定宽度 （默认值100）
     * @param height 指定高度 （默认值100）
     * @param time 视频的时间轴位置（默认值: 00:00:00.001）
     * */
    static Preview(id: string, width?: number, height?: number, time?: string) {
        return `${this.BaseUrl}/file/preview/${id}?${[['width', width?.toString()], ['height', height?.toString()], ['item', time]].map(item => { if (item[1] && item[1].length > 0) return `${item[0]}=${item[1]}`; }).join('&')}`;
    }

    /**
     * 浏览文件
     * @param id 文件Id
     * */
    static Browse(id: string) {
        return `${this.BaseUrl}/file/browse/${id}`;
    }

    /**
     * 下载文件
     * @param id 文件Id
     * */
    static Download(id: string) {
        return `${this.BaseUrl}/file/download/${id}`;
    }

    /**
     * 删除文件
     * */
    static Delete: string = this.BaseUrl + '/file/delete';

    /**
     * 预览个人文件
     * @param id 个人文件Id
     * @param width 指定宽度 （默认值100）
     * @param height 指定高度 （默认值100）
     * @param time 视频的时间轴位置（默认值: 00:00:00.001）
     * */
    static PersonalFilePreview(id: string, width?: number, height?: number, time?: string) {
        return `${this.BaseUrl}/personal-file-info/preview/${id}?${[['width', width?.toString()], ['height', height?.toString()], ['item', time]].map(item => { if (item[1] && item[1].length > 0) return `${item[0]}=${item[1]}`; }).join('&')}`;
    }

    /**
     * 浏览个人文件
     * @param id 个人文件Id
     * */
    static PersonalFileBrowse(id: string) {
        return `${this.BaseUrl}/personal-file-info/browse/${id}`;
    }

    /**
     * 下载个人文件
     * @param id 个人文件Id
     * */
    static PersonalFileDownload(id: string) {
        return `${this.BaseUrl}/personal-file-info/download/${id}`;
    }

    /**
     * 删除个人文件
     * @param id 个人文件Id
     * */
    static PersonalFileDelete(id: string) {
        return `${this.BaseUrl}/personal-file-info/delete/${id}`;
    }

    /**
     * 分片文件合并任务中心
     * */
    static CFMTHub: string = this.BaseUrl + '/cfmthub';
}