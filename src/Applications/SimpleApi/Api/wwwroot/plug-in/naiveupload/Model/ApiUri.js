var ApiUri = (function () {
    function ApiUri() {
    }
    ApiUri.PreUploadChunkfile = function (file_md5, md5, index, specs, forced) {
        if (forced === void 0) { forced = false; }
        return "/file-upload/pre-chunkfile/" + file_md5 + "/" + md5 + "/" + index + "/" + specs + "?forced=" + forced;
    };
    ;
    ApiUri.UploadSingleChunkfile = function (key, md5) {
        return "/file-upload/single-chunkfile/" + key + "/" + md5;
    };
    ApiUri.UploadSingleChunkfileByArrayBuffer = function (key, md5) {
        return "/file-upload/single-chunkfile-arraybuffer/" + key + "/" + md5;
    };
    ApiUri.UploadChunkfileFinished = function (file_md5, specs, total, type, extension, filename) {
        return "/file-upload/chunkfile-finished/" + file_md5 + "/" + specs + "/" + total + "?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.SingleFile = function (configId, filename) {
        return "/file-upload/single-file/" + configId + "?filename=" + filename;
    };
    ApiUri.SingleFileByArrayBuffer = function (configId, type, extension, filename) {
        return "/file-upload/single-file-arraybuffer/" + configId + "?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.Preview = function (id, width, height, time) {
        return "/file/preview/" + id + "?width=" + (width !== null && width !== void 0 ? width : 0) + "&height=" + (height !== null && height !== void 0 ? height : 0) + "&time=" + (time !== null && time !== void 0 ? time : '');
    };
    ApiUri.Browse = function (id) {
        return "/file/browse/" + id;
    };
    ApiUri.Download = function (id) {
        return "/file/download/" + id;
    };
    ApiUri.SAAuthorized = '/sa/authorized';
    ApiUri.SALogin = '/sa/login';
    ApiUri.GetToken = '/jwt/get-token';
    ApiUri.RefreshToken = '/jwt/refresh-token';
    ApiUri.PersonalFileInfoRename = function (id, filename) { return "/personal-file-info/rename/" + id + "?filename=" + filename; };
    ApiUri.PersonalFileInfoEditData = function (id) { return "/personal-file-info/edit-data/" + id; };
    ApiUri.PersonalFileInfoEdit = '/personal-file-info/edit';
    ApiUri.FileTypeByExtension = function (extension) { return "/file/type-by-extension/" + extension; };
    ApiUri.FileTypeByMIME = function (mimetype) { return "/file/type-by-mimetype?mimetype=" + mimetype; };
    ApiUri.FileTypeImageUrl = function (extension) { return "/file/type-image/" + extension; };
    ApiUri.FileSize = function (length) { return "/file/size/" + length; };
    ApiUri.FileUploadConfigTreeList = '/file-upload-config/tree-list';
    ApiUri.FileUploadConfigDetailData = function (id) { return "/file-upload-config/detail-data/" + id; };
    ApiUri.FileUploadConfigData = function (id) { return "/file-upload-config/data/" + id; };
    ApiUri.GetCurrentAccountCFUCTree = '/authorities/current-account-data-cfuc-tree';
    ApiUri.GetLibraryInfo = '/file/library-info';
    ApiUri.GetFileTypes = '/file/filetypes';
    ApiUri.GetStorageTypes = '/file/storagetypes';
    ApiUri.GetFileStates = '/file/filestates';
    ApiUri.GetFileList = '/file/list';
    ApiUri.GetFileDetail = function (id) { return "/file/detail-data/" + id; };
    ApiUri.PreUploadFile = function (configId, md5, filename, section, type, extension, specs, total) {
        if (section === void 0) { section = false; }
        return "/file-upload/pre-file/" + configId + "/" + md5 + "?filename=" + filename + "&section=" + section + "&type=" + (type || '') + "&extension=" + (extension || '') + "&specs=" + (specs || '') + "&total=" + (total || '');
    };
    ApiUri.Delete = '/file/list';
    ApiUri.CFMTHub = '/cfmthub';
    return ApiUri;
}());
//# sourceMappingURL=ApiUri.js.map