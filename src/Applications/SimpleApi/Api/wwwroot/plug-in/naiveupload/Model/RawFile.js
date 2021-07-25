var RawFile = (function () {
    function RawFile(file) {
        this.NeedSection = false;
        this.Chunks = [];
        this.ChunkIndexQueue = [];
        this.File = file;
        this.Size = file.size;
        this.ObjectURL = URL.createObjectURL(file);
    }
    return RawFile;
}());
//# sourceMappingURL=RawFile.js.map