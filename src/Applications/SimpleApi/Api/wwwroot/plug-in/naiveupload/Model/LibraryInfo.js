var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var LibraryInfo = (function () {
    function LibraryInfo() {
    }
    return LibraryInfo;
}());
var FolderInfo = (function (_super) {
    __extends(FolderInfo, _super);
    function FolderInfo() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Hover = false;
        _this.Open = false;
        return _this;
    }
    return FolderInfo;
}(LibraryInfo));
var FileListInfo = (function () {
    function FileListInfo() {
        this.Init = false;
        this.Loading = true;
        this.CheckAllFileType = true;
        this.IsFileTypeIndeterminate = false;
        this.CheckAllStorageType = true;
        this.IsStorageTypeIndeterminate = false;
        this.CheckAllFileState = true;
        this.IsFileStateIndeterminate = false;
        this.Filters = new FileListFilters();
        this.Pagination = new Pagination();
        this.List = [];
    }
    return FileListInfo;
}());
var FileListFilters = (function () {
    function FileListFilters() {
        this.DateRang = [new Date(new Date().setTime(new Date().getTime() - 3600 * 1000 * 24 * 7)), new Date()];
    }
    return FileListFilters;
}());
//# sourceMappingURL=LibraryInfo.js.map