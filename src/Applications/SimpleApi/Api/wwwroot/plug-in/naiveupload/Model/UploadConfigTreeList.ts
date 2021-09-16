/**
 * 文件上传配置树状列表
 * */
class UploadConfigTreeList {
    /**
     * Id
     */
    Id: string;

    /**
     * 根Id
     */
    RootId: string;

    /**
     * 父级Id
     */
    ParentId: string;

    /**
     * 层级
     */
    Level: number;

    /**
     * 编码
     */
    Code: string;

    /**
     * 引用的上传配置Id
     * 引用文件MIME类型，会合并当前数据以及引用数据
     */
    ReferenceId: string;

    /**
     * 级联引用
     * 使用引用的上传配置以及它的所有子集配置
     */
    ReferenceTree: boolean;

    /**
     * 名称
     */
    Name: string;

    /**
     * 显示名称
     */
    DisplayName: string;

    /**
     * 公共配置
     * 无需授权
     */
    Public: boolean;

    /**
     * 文件数量下限
     */
    LowerLimit: number;

    /**
     * 文件数量上限
     */
    UpperLimit: number;

    /**
     * 排序值
     */
    Sort: string;

    /**
     * 启用
     */
    Enable: boolean;

    /**
     * 创建时间
     */
    CreateTime: string;

    /**
     * 最近编辑时间
     */
    ModifyTime: string;

    /**
     * 是否拥有子级
     */
    HasChildren: boolean;

    /**
     * 子级数量
     */
    ChildrenCount: number;

    /**
     * 子级
     */
    Children: UploadConfigTreeList[];
}