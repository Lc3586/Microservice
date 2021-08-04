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
     * 文件重命名
     * @param id Id
     * @param filename 文件名
     * */
    static Rename: (id: string, filename: string) => string = (id: string, filename: string) => `/file/rename/${id}?filename=${filename}`;

    /**
     * 获取文件类型
     * @param extension 文件拓展名
     * */
    static FileTypeByExtension: (extension: string) => string = (extension: string) => `/file/file-type-by-extension/${extension}`;

    /**
     * 获取文件类型
     * @param mimetype MIME类型
     * */
    static FileTypeByMIME: (mimetype: string) => string = (mimetype: string) => `/file/file-type-by-mimetype?mimetype=${mimetype}`;

    /**
     * 获取文件类型预览图链接地址
     * @param extension 文件拓展名
     * */
    static FileTypeImageUrl: (extension: string) => string = (extension: string) => `/file/file-type-image/${extension}`;

    /**
     * 获取文件大小
     * @param length 文件字节数
     * */
    static FileSize: (length: number | string) => string = (length: number | string) => `/file/file-size/${length}`;

    /**
     * 预备上传文件
     * @param md5 文件哈希值
     * @param filename 文件重命名
     * @param section 是否分片处理（默认否）
     * @param type 文件类型（单文件上传时忽略此参数）
     * @param extension 文件拓展名（单文件上传时忽略此参数）
     * @param specs 分片文件规格（单文件上传时忽略此参数）
     * @param total 分片文件总数（单文件上传时忽略此参数）
     * */
    static PreUploadFile: (md5: string, filename: string, section?: boolean, type?: string, extension?: string, specs?: number, total?: number) => string = (md5: string, filename: string, section: boolean = false, type: string | null = null, extension: string | null = null, specs: number | null = null, total: number | null = null) => `/file/pre-upload-file/${md5}?filename=${filename}&section=${section}&type=${type}&extension=${extension}&specs=${specs}&total=${total}`;

    /**
     * 预上传分片文件
     * @param file_md5 文件哈希值
     * @param md5 分片文件哈希值
     * @param index 分片文件索引
     * @param specs 分片文件规格
     * @param forced 强制上传
     */
    static PreUploadChunkfile(file_md5: string, md5: string, index: number, specs: number, forced: boolean = false): string {
        return `/file/pre-upload-chunkfile/${file_md5}/${md5}/${index}/${specs}?forced=${forced}`
    };

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfile(key: string, md5: string): string {
        return `/file/upload-single-chunkfile/${key}/${md5}`;
    }

    /**
     * 单个分片文件上传
     * @param key 上传标识
     * @param md5 分片文件哈希值
     * */
    static UploadSingleChunkfileByArrayBuffer(key: string, md5: string): string {
        return `/file/upload-single-chunkfile-arraybuffer/${key}/${md5}`;
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
        return `/file/upload-chunkfile-finished/${file_md5}/${specs}/${total}?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param filename 文件重命名
     * */
    static SingleFile(filename: string): string {
        return `/file/upload-single-file?filename=${filename}`;
    }

    /**
     * 上传单个文件
     * @param type 文件类型
     * @param extension 文件拓展名
     * @param filename 文件重命名
     * */
    static SingleFileByArrayBuffer(type: string, extension: string, filename: string): string {
        return `/file/upload-single-file-arraybuffer?type=${type}&extension=${extension}&filename=${filename}`;
    }

    /**
     * 预览文件
     * @param id 文件Id
     * @param width 指定宽度 （默认值500）
     * @param height 指定高度 （默认值500）
     * @param time 视频的时间轴位置（默认值: 00:00:00.001）
     * */
    static Preview(id: string, width?: number, height?: number, time?: string) {
        return `/file/preview/${id}?width=${width}&height=${height}&time=${time}`;
    }

    /**
     * 浏览文件
     * @param id 文件Id
     * */
    static Browse(id: string) {
        return `/file/browse/${id}`;
    }
}