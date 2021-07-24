var SelectedFile = (function () {
    function SelectedFile(file) {
        this.Size = null;
        this.FileType = "\u672A\u77E5";
        this.Thumbnail = '/filetypes/empty.jpg';
        this.Class = [];
        this.Hover = false;
        this.Rename = false;
        this.Checking = false;
        this.Checked = false;
        this.Uploading = false;
        this.Uploaded = false;
        this.Done = false;
        this.ReTry = 0;
        this.Error = false;
        this.Percent = 0;
        this.VirtualPercent = 0;
        this.Paused = false;
        this.Canceled = false;
        var pointIndex = file.name.lastIndexOf('.');
        this.Name = file.name.substring(0, pointIndex);
        this.Extension = file.name.substring(pointIndex);
        this.ExtensionLower = window._.toLower(this.Extension);
    }
    return SelectedFile;
}());
//# sourceMappingURL=SelectedFile.js.map