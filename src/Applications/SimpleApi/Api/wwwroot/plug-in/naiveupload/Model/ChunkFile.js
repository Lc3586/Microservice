var ChunkFile = (function () {
    function ChunkFile(index, blob) {
        this.Forced = false;
        this.Checking = false;
        this.Checked = false;
        this.Uploading = false;
        this.Uploaded = false;
        this.Done = false;
        this.Error = false;
        this.Index = index;
        this.Blob = blob;
    }
    return ChunkFile;
}());
//# sourceMappingURL=ChunkFile.js.map