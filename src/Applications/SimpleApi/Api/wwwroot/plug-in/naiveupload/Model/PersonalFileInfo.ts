/**
 * 个人文件信息
 * */
class PersonalFileInfo {
    /**
     * Id
     */
    Id: string;

    /**
     * 文件Id
     */
    FileId: string;

    /**
     * 文件上传配置Id
     */
    ConfigId: string;

    /**
     * 文件重命名
     */
    Name: string;

    /**
     * 文件扩展名
     */
    Extension: string;

    /**
     * 类别
     */
    Category: string;

    /**
     * 星级
     */
    Star: string;

    /**
     * 标签
     */
    Tags: string;

    /**
     * 状态
     */
    State: string;

    /**
     * 创建时间
     */
    CreateTime: Date;

    /**
     * 最近编辑时间
     */
    ModifyTime?: Date;

}