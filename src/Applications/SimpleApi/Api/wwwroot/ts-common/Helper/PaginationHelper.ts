/**
 * 分页设置帮助类
 * */
class PaginationHelper {
    /**
     * 创建分页设置
     * @param paging 是否分页
     * @returns 分页设置
     */
    static createPagination(paging: boolean = true): Pagination {
        var pagination = new Pagination();
        pagination.PageIndex = paging ? 1 : -1;
        pagination.PageRows = 10;
        pagination.SortField = 'Id';
        pagination.SortType = SortType.倒序;
        pagination.AdvancedSort = [];
        pagination.Filter = [];
        pagination.DynamicFilterInfo = [];
        pagination.Schema = Schema.默认;
        pagination.RecordCount = 0;
        pagination.PageCount = 0;
        return pagination;
    }

    /**
     * 默认页面数据量选项
     */
    static DefaultPageSizeOptions: number[] = [5, 10, 15, 20, 50, 100, 150, 200, 300, 400, 500];

    /**
     * 计算当前页面列表数据量的自适应值
     * @param rowHeight 行高(默认100px)
     * @param pageSizeOptions 页面数据量选项（可选）
     * @returns {number} 数据量值
     * */
    static culcCurrentViewPageRows(rowHeight: number = 100, pageSizeOptions?: number[]): number {
        let current = window.innerHeight / rowHeight,
            min,
            result;

        let _pageSizeOptions = pageSizeOptions ?? PaginationHelper.DefaultPageSizeOptions;

        for (const i in _pageSizeOptions) {
            let size = _pageSizeOptions[i];
            let abs = Math.abs(current - size);
            if (!min)
                min = abs;
            else if (abs <= min)
                min = abs;
            else
                continue;

            result = size;
        }

        return result;
    }
}