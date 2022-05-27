/**
 * 模型验证错误信息
 * */
class ModelErrorsInfo {
    /**
     * 字段路径
     */
    FullKey: string;

    /**
     * 字段名称
     */
    Key: string;

    /**
     * 值
     */
    Value: string;

    /**
     * 错误信息
     */
    Errors: string[];
}