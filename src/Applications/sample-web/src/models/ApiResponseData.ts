/**
 * 接口输出数据
 */
interface ApiResponseData {
    /**
     * 操作结果
     */
    Success: boolean;

    /**
     * 错误码
     */
    ErrorCode: number;

    /**
     * 消息
     */
    Message: string;

    /**
     * 数据
     */
    Data?: any;

    /**
     * 异常信息
     * 仅在开发环境下使用
     */
    ExceptionInfo?: string;
}