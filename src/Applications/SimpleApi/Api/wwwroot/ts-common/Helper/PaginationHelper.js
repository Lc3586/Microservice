var PaginationHelper = (function () {
    function PaginationHelper() {
    }
    PaginationHelper.createPagination = function (paging) {
        if (paging === void 0) { paging = true; }
        var pagination = new Pagination();
        pagination.PageIndex = paging ? 1 : -1;
        pagination.PageRows = 10;
        pagination.SortField = 'Id';
        pagination.SortType = "desc";
        pagination.AdvancedSort = [];
        pagination.Filter = [];
        pagination.DynamicFilterInfo = [];
        pagination.Schema = "defaul";
        pagination.RecordCount = 0;
        pagination.PageCount = 0;
        return pagination;
    };
    PaginationHelper.culcCurrentViewPageRows = function (rowHeight, pageSizeOptions) {
        if (rowHeight === void 0) { rowHeight = 100; }
        var current = window.innerHeight / rowHeight, min, result;
        var _pageSizeOptions = pageSizeOptions !== null && pageSizeOptions !== void 0 ? pageSizeOptions : PaginationHelper.DefaultPageSizeOptions;
        for (var i in _pageSizeOptions) {
            var size = _pageSizeOptions[i];
            var abs = Math.abs(current - size);
            if (!min)
                min = abs;
            else if (abs <= min)
                min = abs;
            else
                continue;
            result = size;
        }
        return result;
    };
    PaginationHelper.DefaultPageSizeOptions = [5, 10, 15, 20, 50, 100, 150, 200, 300, 400, 500];
    return PaginationHelper;
}());
//# sourceMappingURL=PaginationHelper.js.map