/**
 * 文件上传配置
 * */
class UploadConfig {
    /**
     * Id
     */
    Id: string;

    /**
     * 文件数量下限
     */
    LowerLimit: number;

    /**
     * 文件数量上限
     */
    UpperLimit: number;

    /**
     * 允许的MIME类型
     * [,]逗号分隔
     * 此值为空时未禁止即允许
     */
    AllowedTypeList: string[];

    /**
     * 禁止的MIME类型
     * [,]逗号分隔
     * 此值为空时皆可允许
     */
    ProhibitedTypeList: string[];

    /**
     * 说明文字
     */
    Explain: string;
}