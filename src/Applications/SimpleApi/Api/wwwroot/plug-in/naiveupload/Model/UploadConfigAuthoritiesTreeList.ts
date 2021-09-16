/**
 * 文件上传配置授权信息树状列表
 * */
class UploadConfigAuthoritiesTreeList {
    /**
     * Id
     */
    Id: string;

    /**
     * 编码
     */
    Code: string;

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
     * 已授权
     */
    Authorized: boolean;

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
    Children: UploadConfigAuthoritiesTreeList[];
}