var ApiUri = (function () {
    function ApiUri() {
    }
    ApiUri.PreUploadChunkfile = function (file_md5, md5, index, specs, forced) {
        if (forced === void 0) { forced = false; }
        return this.BaseUrl + "/file-upload/pre-chunkfile/" + file_md5 + "/" + md5 + "/" + index + "/" + specs + "?forced=" + forced;
    };
    ;
    ApiUri.UploadSingleChunkfile = function (key, md5) {
        return this.BaseUrl + "/file-upload/single-chunkfile/" + key + "/" + md5;
    };
    ApiUri.UploadSingleChunkfileByArrayBuffer = function (key, md5) {
        return this.BaseUrl + "/file-upload/single-chunkfile-arraybuffer/" + key + "/" + md5;
    };
    ApiUri.UploadChunkfileFinished = function (file_md5, specs, total, type, extension, filename) {
        return this.BaseUrl + "/file-upload/chunkfile-finished/" + file_md5 + "/" + specs + "/" + total + "?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.SingleFile = function (configId, filename) {
        return this.BaseUrl + "/file-upload/single-file/" + configId + "?filename=" + filename;
    };
    ApiUri.SingleFileByArrayBuffer = function (configId, type, extension, filename) {
        return this.BaseUrl + "/file-upload/single-file-arraybuffer/" + configId + "?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.Preview = function (id, width, height, time) {
        return this.BaseUrl + "/file/preview/" + id + "?width=" + (width !== null && width !== void 0 ? width : '') + "&height=" + (height !== null && height !== void 0 ? height : '') + "&time=" + (time !== null && time !== void 0 ? time : '');
    };
    ApiUri.Browse = function (id) {
        return this.BaseUrl + "/file/browse/" + id;
    };
    ApiUri.Download = function (id) {
        return this.BaseUrl + "/file/download/" + id;
    };
    var _a;
    _a = ApiUri;
    ApiUri.BaseUrl = window.BaseUrl;
    ApiUri.SAAuthorized = _a.BaseUrl + '/sa/authorized';
    ApiUri.SALogin = _a.BaseUrl + '/sa/login';
    ApiUri.GetToken = _a.BaseUrl + '/jwt/get-token';
    ApiUri.RefreshToken = _a.BaseUrl + '/jwt/refresh-token';
    ApiUri.PersonalFileInfoRename = function (id, filename) { return _a.BaseUrl + "/personal-file-info/rename/" + id + "?filename=" + filename; };
    ApiUri.PersonalFileInfoEditData = function (id) { return _a.BaseUrl + "/personal-file-info/edit-data/" + id; };
    ApiUri.PersonalFileInfoEdit = _a.BaseUrl + '/personal-file-info/edit';
    ApiUri.FileTypeByExtension = function (extension) { return _a.BaseUrl + "/file/type-by-extension/" + extension; };
    ApiUri.FileTypeByMIME = function (mimetype) { return _a.BaseUrl + "/file/type-by-mimetype?mimetype=" + mimetype; };
    ApiUri.FileTypeImageUrl = function (extension) { return _a.BaseUrl + "/file/type-image/" + extension; };
    ApiUri.FileSize = function (length) { return _a.BaseUrl + "/file/size/" + length; };
    ApiUri.FileUploadConfigTreeList = _a.BaseUrl + '/file-upload-config/tree-list';
    ApiUri.FileUploadConfigDetailData = function (id) { return _a.BaseUrl + "/file-upload-config/detail-data/" + id; };
    ApiUri.FileUploadConfigData = function (id) { return _a.BaseUrl + "/file-upload-config/data/" + id; };
    ApiUri.GetCurrentAccountCFUCTree = _a.BaseUrl + '/authorities/current-account-data-cfuc-tree';
    ApiUri.GetLibraryInfo = _a.BaseUrl + '/file/library-info';
    ApiUri.GetFileTypes = _a.BaseUrl + '/file/filetypes';
    ApiUri.GetStorageTypes = _a.BaseUrl + '/file/storagetypes';
    ApiUri.GetFileStates = _a.BaseUrl + '/file/filestates';
    ApiUri.GetFileList = _a.BaseUrl + '/file/list';
    ApiUri.GetFileDetail = function (id) { return _a.BaseUrl + "/file/detail-data/" + id; };
    ApiUri.GetVideoInfo = function (id, format, streams, chapters, programs, version) {
        if (format === void 0) { format = true; }
        if (streams === void 0) { streams = false; }
        if (chapters === void 0) { chapters = false; }
        if (programs === void 0) { programs = false; }
        if (version === void 0) { version = false; }
        return _a.BaseUrl + "/file/video-info/" + id + "?format=" + format + "&streams=" + streams + "&chapters=" + chapters + "&programs=" + programs + "&version=" + version;
    };
    ApiUri.PreUploadFile = function (configId, md5, filename, section, type, extension, specs, total) {
        if (section === void 0) { section = false; }
        return _a.BaseUrl + "/file-upload/pre-file/" + configId + "/" + md5 + "?filename=" + filename + "&section=" + section + "&type=" + (type || '') + "&extension=" + (extension || '') + "&specs=" + (specs || '') + "&total=" + (total || '');
    };
    ApiUri.Delete = _a.BaseUrl + '/file/list';
    ApiUri.CFMTHub = _a.BaseUrl + '/cfmthub';
    return ApiUri;
}());
//# sourceMappingURL=ApiUri.js.map