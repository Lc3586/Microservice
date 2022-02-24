var MultipleUploadSetting = (function () {
    function MultipleUploadSetting() {
        this.ConcurrentFile = 3;
        this.ConcurrentChunkFile = 3;
        this.Tip = '单击或拖动文件到此区域即可上传';
        this.Theme = "card";
        this.Mode = "AT";
        this.EnableChunk = true;
        this.ChunkSize = 2 * 1024 * 1024;
        this.Retry = 3;
        this.Headers = {};
        this.EnableWorker = true;
        return this;
    }
    return MultipleUploadSetting;
}());
//# sourceMappingURL=MultipleUploadSetting.js.map