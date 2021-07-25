var ApiUri = (function () {
    function ApiUri() {
    }
    ApiUri.PreUploadChunkfile = function (file_md5, md5, index, specs, forced) {
        if (forced === void 0) { forced = false; }
        return "/file/pre-upload-chunkfile/" + file_md5 + "/" + md5 + "/" + index + "/" + specs + "?forced=" + forced;
    };
    ;
    ApiUri.UploadSingleChunkfile = function (key, md5) {
        return "/file/upload-single-chunkfile/" + key + "/" + md5;
    };
    ApiUri.UploadSingleChunkfileByArrayBuffer = function (key, md5) {
        return "/file/upload-single-chunkfile-arraybuffer/" + key + "/" + md5;
    };
    ApiUri.UploadChunkfileFinished = function (file_md5, specs, total, type, extension, filename) {
        return "/file/upload-chunkfile-finished/" + file_md5 + "/" + specs + "/" + total + "?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.SingleFile = function (filename) {
        return "/file/upload-single-file?filename=" + filename;
    };
    ApiUri.SingleFileByArrayBuffer = function (type, extension, filename) {
        return "/file/upload-single-file-arraybuffer?type=" + type + "&extension=" + extension + "&filename=" + filename;
    };
    ApiUri.Preview = function (id, width, height, time) {
        return "/file/preview/" + id + "?width=" + width + "&height=" + height + "&time=" + time;
    };
    ApiUri.Browse = function (id) {
        return "/file/browse/" + id;
    };
    ApiUri.SAAuthorized = '/sa/authorized';
    ApiUri.SALogin = '/sa/login';
    ApiUri.GetToken = '/jwt/get-token';
    ApiUri.RefreshToken = '/jwt/refresh-token';
    ApiUri.Rename = function (id, filename) { return "/file/rename/" + id + "?filename=" + filename; };
    ApiUri.FileTypeByExtension = function (extension) { return "/file/file-type-by-extension/" + extension; };
    ApiUri.FileTypeByMIME = function (mimetype) { return "/file/file-type-by-mimetype?mimetype=" + mimetype; };
    ApiUri.FileTypeImageUrl = function (extension) { return "/file/file-type-image/" + extension; };
    ApiUri.FileSize = function (length) { return "/file/file-size/" + length; };
    ApiUri.ValidationFileMD5 = function (md5, filename) { return "/file/validation-file-md5/" + md5 + "?filename=" + filename; };
    return ApiUri;
}());
//# sourceMappingURL=ApiUri.js.map