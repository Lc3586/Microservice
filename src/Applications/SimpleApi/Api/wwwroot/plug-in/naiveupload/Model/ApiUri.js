var ApiUri = (function () {
    function ApiUri() {
    }
    ApiUri.PreUploadChunkfile = function (file_md5, md5, index, specs, forced) {
        if (forced === void 0) { forced = false; }
        return "".concat(this.BaseUrl, "/file-upload/pre-chunkfile/").concat(file_md5, "/").concat(md5, "/").concat(index, "/").concat(specs, "?forced=").concat(forced);
    };
    ;
    ApiUri.UploadSingleChunkfile = function (key, md5) {
        return "".concat(this.BaseUrl, "/file-upload/single-chunkfile/").concat(key, "/").concat(md5);
    };
    ApiUri.UploadSingleChunkfileByArrayBuffer = function (key, md5) {
        return "".concat(this.BaseUrl, "/file-upload/single-chunkfile-arraybuffer/").concat(key, "/").concat(md5);
    };
    ApiUri.UploadChunkfileFinished = function (file_md5, specs, total, type, extension, filename) {
        return "".concat(this.BaseUrl, "/file-upload/chunkfile-finished/").concat(file_md5, "/").concat(specs, "/").concat(total, "?type=").concat(type, "&extension=").concat(extension, "&filename=").concat(filename);
    };
    ApiUri.SingleFile = function (configId, filename) {
        return "".concat(this.BaseUrl, "/file-upload/single-file/").concat(configId, "?filename=").concat(filename);
    };
    ApiUri.SingleFileByArrayBuffer = function (configId, type, extension, filename) {
        return "".concat(this.BaseUrl, "/file-upload/single-file-arraybuffer/").concat(configId, "?type=").concat(type, "&extension=").concat(extension, "&filename=").concat(filename);
    };
    ApiUri.Preview = function (id, width, height, time) {
        return "".concat(this.BaseUrl, "/file/preview/").concat(id, "?width=").concat(width !== null && width !== void 0 ? width : '', "&height=").concat(height !== null && height !== void 0 ? height : '', "&time=").concat(time !== null && time !== void 0 ? time : '');
    };
    ApiUri.Browse = function (id) {
        return "".concat(this.BaseUrl, "/file/browse/").concat(id);
    };
    ApiUri.Download = function (id) {
        return "".concat(this.BaseUrl, "/file/download/").concat(id);
    };
    var _a;
    _a = ApiUri;
    ApiUri.BaseUrl = window.BaseUrl;
    ApiUri.SAAuthorized = _a.BaseUrl + '/sa/authorized';
    ApiUri.SALogin = _a.BaseUrl + '/sa/login';
    ApiUri.GetToken = _a.BaseUrl + '/jwt/get-token';
    ApiUri.RefreshToken = _a.BaseUrl + '/jwt/refresh-token';
    ApiUri.PersonalFileInfoRename = function (id, filename) { return "".concat(_a.BaseUrl, "/personal-file-info/rename/").concat(id, "?filename=").concat(filename); };
    ApiUri.PersonalFileInfoEditData = function (id) { return "".concat(_a.BaseUrl, "/personal-file-info/edit-data/").concat(id); };
    ApiUri.PersonalFileInfoEdit = _a.BaseUrl + '/personal-file-info/edit';
    ApiUri.FileTypeByExtension = function (extension) { return "".concat(_a.BaseUrl, "/file/type-by-extension/").concat(extension); };
    ApiUri.FileTypeByMIME = function (mimetype) { return "".concat(_a.BaseUrl, "/file/type-by-mimetype?mimetype=").concat(mimetype); };
    ApiUri.FileTypeImageUrl = function (extension) { return "".concat(_a.BaseUrl, "/file/type-image/").concat(extension); };
    ApiUri.FileSize = function (length) { return "".concat(_a.BaseUrl, "/file/size/").concat(length); };
    ApiUri.FileUploadConfigTreeList = _a.BaseUrl + '/file-upload-config/tree-list';
    ApiUri.FileUploadConfigDetailData = function (id) { return "".concat(_a.BaseUrl, "/file-upload-config/detail-data/").concat(id); };
    ApiUri.FileUploadConfigData = function (id) { return "".concat(_a.BaseUrl, "/file-upload-config/data/").concat(id); };
    ApiUri.GetCurrentAccountCFUCTree = _a.BaseUrl + '/authorities/current-account-data-cfuc-tree';
    ApiUri.GetLibraryInfo = _a.BaseUrl + '/file/library-info';
    ApiUri.GetFileTypes = _a.BaseUrl + '/file/filetypes';
    ApiUri.GetStorageTypes = _a.BaseUrl + '/file/storagetypes';
    ApiUri.GetFileStates = _a.BaseUrl + '/file/filestates';
    ApiUri.GetFileList = _a.BaseUrl + '/file/list';
    ApiUri.GetFileDetail = function (id) { return "".concat(_a.BaseUrl, "/file/detail-data/").concat(id); };
    ApiUri.GetVideoInfo = function (id, format, streams, chapters, programs, version) {
        if (format === void 0) { format = true; }
        if (streams === void 0) { streams = false; }
        if (chapters === void 0) { chapters = false; }
        if (programs === void 0) { programs = false; }
        if (version === void 0) { version = false; }
        return "".concat(_a.BaseUrl, "/file/video-info/").concat(id, "?format=").concat(format, "&streams=").concat(streams, "&chapters=").concat(chapters, "&programs=").concat(programs, "&version=").concat(version);
    };
    ApiUri.PreUploadFile = function (configId, md5, filename, section, type, extension, specs, total) {
        if (section === void 0) { section = false; }
        return "".concat(_a.BaseUrl, "/file-upload/pre-file/").concat(configId, "/").concat(md5, "?filename=").concat(filename, "&section=").concat(section, "&type=").concat(type || '', "&extension=").concat(extension || '', "&specs=").concat(specs || '', "&total=").concat(total || '');
    };
    ApiUri.Delete = _a.BaseUrl + '/file/delete';
    ApiUri.CFMTHub = _a.BaseUrl + '/cfmthub';
    return ApiUri;
}());
//# sourceMappingURL=ApiUri.js.map