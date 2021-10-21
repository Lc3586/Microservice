var Pagination = (function () {
    function Pagination() {
        this.PageIndex = 1;
        this.PageRows = 15;
        this.SortField = 'Id';
        this.SortType = "desc";
        this.AdvancedSort = [];
        this.Filter = [];
        this.DynamicFilterInfo = [];
        this.Schema = "defaul";
        this.RecordCount = 0;
        this.PageCount = 0;
    }
    return Pagination;
}());
var PaginationAdvancedSort = (function () {
    function PaginationAdvancedSort() {
        this.Type = "desc";
    }
    return PaginationAdvancedSort;
}());
var PaginationFilter = (function () {
    function PaginationFilter() {
    }
    return PaginationFilter;
}());
var FilterGroupSetting = (function () {
    function FilterGroupSetting() {
    }
    return FilterGroupSetting;
}());
var PaginationDynamicFilterInfo = (function () {
    function PaginationDynamicFilterInfo() {
    }
    return PaginationDynamicFilterInfo;
}());
var PaginationResult_ElementVue = (function () {
    function PaginationResult_ElementVue() {
    }
    return PaginationResult_ElementVue;
}());
var PaginationResult_ElementVueData = (function () {
    function PaginationResult_ElementVueData() {
    }
    return PaginationResult_ElementVueData;
}());
//# sourceMappingURL=Pagination.js.map