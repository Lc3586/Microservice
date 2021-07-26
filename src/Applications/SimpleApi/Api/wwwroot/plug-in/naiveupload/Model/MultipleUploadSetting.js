var MultipleUploadSetting = (function () {
    function MultipleUploadSetting() {
        this.Accept = ['image/*', 'audio/*', 'video/*', '.txt', '.pdf', '.doc', 'docx', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.xls', 'xlsx', '.zip', '.rar', '.7z'];
        this.MultipleSelect = true;
        this.Limit = 10;
        this.ConcurrentFile = 3;
        this.ConcurrentChunkFile = 3;
        this.Explain = '单击或拖动文件到此区域即可上传';
        this.Tip = '严禁上传存在违法违规内容的文件';
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