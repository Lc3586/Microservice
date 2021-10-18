/**
 * 分页设置
 * */
class Pagination {
    /**
     * 当前页码
     * 默认 1
     */
    PageIndex: number = 1;

    /**
     * 每页数据量
     * 默认 15
     */
    PageRows: number = 15;

    /**
     * 排序列
     * 默认 Id
     */
    SortField: string = 'Id';

    /**
     * 排序类型
     * 默认 desc
     */
    SortType: SortType = SortType.倒序;

    /**
     * 高级排序
     */
    AdvancedSort: PaginationAdvancedSort[] = [];

    /**
     * 筛选条件
     */
    Filter: PaginationFilter[] = [];

    /**
     * 别名
     */
    Alias: string;

    /**
     * 筛选条件
     */
    DynamicFilterInfo: PaginationDynamicFilterInfo[] = [];

    /**
     * 架构
     * 默认值 defaul
     */
    Schema: Schema = Schema.默认;

    /**
     * 总记录数
     */
    RecordCount: number = 0;

    /**
     * 总页数
     */
    PageCount: number = 0;
}

/**
 * 排序类型
 * */
const enum SortType {
    正序 = 'asc',
    倒序 = 'desc'
}

/**
 * 分页高级排序
 * */
class PaginationAdvancedSort {
    /**
     * 字段
     */
    Field: string;

    /**
     * 类型
     * 默认值 desc
     */
    Type: SortType = SortType.倒序;
}

/**
 * 分页筛选条件
 * */
class PaginationFilter {
    /**
     * 要比较的字段
     * 注意区分大小写
     */
    Field: string;

    /**
     * 用于比较的值
     */
    Value: any;

    /**
     * Value值是用来比较的字段
     */
    ValueIsField: boolean;

    /**
     * 比较类型
     * 默认值 eq
     */
    Compare: FilterCompare;

    /**
     * 分组设置（已弃用）
     */
    Group: FilterGroupSetting;

    /**
     * 子条件
     */
    DynamicFilterInfo: PaginationFilter[];
}

/**
 * 筛选条件比较类型
 * */
const enum FilterCompare {
    /**
     * 包含（'%Value%'）
     * */
    包含 = 'in',

    /**
     * 前段包含（'Value%'）
     * */
    前段包含 = 'inStart',

    /**
     * 后段包含（'%Value'）
     * */
    后段包含 = 'inEnd',

    /**
     * 由于Freesql不支持，此功能暂弃用
     * */
    包含于 = 'includedIn',

    /**
     * 不包含（'%Value%'）
     * */
    不包含 = 'notIn',

    /**
     * 前段不包含（'Value%'）
     * */
    前段不包含 = 'notInStart',

    /**
     * 后段不包含（'%Value'）
     * */
    后段不包含 = 'notInEnd',

    /**
     * 由于Freesql不支持，此功能暂弃用
     * */
    不包含于 = 'notIncludedIn',

    相等 = 'eq',

    不相等 = 'notEq',

    小于等于 = 'le',

    小于 = 'lt',

    大于等于 = 'ge',

    大于 = 'gt',

    /**
     * ,号分隔
     * 示例:1, 2, 3
     * */
    在集合中 = 'inSet',

    /**
     * ,号分隔
     * 示例:1, 2, 3
     * */
    不在集合中 = 'notInSet',

    /**
     * Value1, Value2
     * */
    范围 = 'range',

    /**
     * ,号分隔
     * date1, date2
     * 这是专门为日期范围查询定制的操作符，它会处理 date2 + 1，比如：
     *      当 date2 选择的是 2020-05-30，那查询的时候是 小于 2020-05-31
     *      当 date2 选择的是 2020-05，那查询的时候是 小于 2020-06
     *      当 date2 选择的是 2020，那查询的时候是 小于 2021
     *      当 date2 选择的是 2020-05-30 12，那查询的时候是 小于 2020-05-30 13
     *      当 date2 选择的是 2020-05-30 12:30，那查询的时候是 小于 2020-05-30 12:31
     *      并且 date2 只支持以上 5 种格式 (date1 没有限制)
     * */
    日期范围 = 'dateRange',
}

/**
 * 分组设置
 * */
class FilterGroupSetting {
    /**
     * 分组标识
     * 默认值 keep
     * 用于标识分组的开始和结束
     */
    Flag: FilterGroupFlag;

    /**
     * 组内关系
     * 默认值 and
     */
    Relation: FilterGroupRelation;
}

/**
 * 筛选条件分组标识
 * */
const enum FilterGroupFlag {
    开始 = 'start',
    还在分组内 = 'keep',
    结束 = 'end',
}

/**
 * 筛选条件分组关系类型
 * */
const enum FilterGroupRelation {
    并且 = 'and',
    或 = 'or',
}

/**
 * 分页筛选条件
 * */
class PaginationDynamicFilterInfo {
    /**
     * 要比较的字段
     * 注意区分大小写
     */
    Field?: string;

    /**
     * 用于比较的值
     */
    Value?: any;

    /**
     * Value值是用来比较的字段
     */
    ValueIsField?: boolean;

    /**
     * 比较类型
     * 默认值 eq
     */
    Compare?: FilterCompare;

    /**
     * 组内关系
     * 默认值 and
     */
    Relation?: FilterGroupRelation;

    /**
     * 子条件
     */
    DynamicFilterInfo?: PaginationDynamicFilterInfo[];
}

/**
 * 架构
 * */
const enum Schema {
    默认 = 'defaul',

    /**
     * https://www.layui.com/doc/modules/table.html#response
     * */
    layui = 'layui',

    /**
     * https://blog.mn886.net/jqGrid/api/jsondata/index.jsp
     * */
    jqGrid = 'jqGrid',

    /**
     * http://www.jeasyui.net/plugins/183.html
     * */
    easyui = 'easyui',

    /**
     * https://bootstrap-table.com/docs/api/table-options/
     * */
    bootstrapTable = 'bootstrapTable',

    /**
     * https://www.antdv.com/components/list-cn/#api
     * */
    AntDesign_Vue = 'antdVue',

    /**
     * https://element-plus.org/#/zh-CN/component/pagination
     * */
    element_Vue = 'elementVue',
}

/**
 * 分页设置返回数据
 * ElementVue方案
 * */
class PaginationResult_ElementVue<T> {
    /**
     * 是否成功
     */
    Success: boolean;

    /**
     * 错误代码
     */
    ErrorCode: ErrorCode;

    /**
     * 消息
     */
    Message: string;

    /**
     * 数据
     */
    Data: PaginationResult_ElementVueData<T>;
}

/**
 * 分页设置返回数据
 * ElementVue方案
 * */
class PaginationResult_ElementVueData<T> {
    /**
     * 总页数
     */
    PageTotal: number;

    /**
     * 总记录数
     */
    Total: number;

    /**
     * 当前页面
     */
    PageIndex: number;

    /**
     * 页面数据量
     */
    PageSize: number;

    /**
     * 数据集合
     */
    List: T[];
}

/**
 * 错误代码
 * */
const enum ErrorCode {
    /**
     * 无
     * */
    none = 0,

    /**
     * 未登录
     * */
    nologin,

    /**
     * 未授权
     * */
    unauthorized,

    /**
     * 权限不足
     * */
    forbidden,

    /**
     * 验证失败
     * */
    validation,

    /**
     * 业务错误
     * */
    business,

    /**
     * 系统错误
     * */
    error
}