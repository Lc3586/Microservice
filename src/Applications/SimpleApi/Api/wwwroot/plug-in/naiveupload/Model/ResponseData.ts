/**
 * 接口输出数据
 * */
class ResponseData {
    /**
     * 操作结果
     */
    Success: boolean;

    /**
     *错误代码
     */
    ErrorCode: number;

    /**
     *消息
     */
    Message: string;
}

/**
 * 接口输出数据
 * */
class ResponseData_T<T> extends ResponseData {
    /**
     * 数据
     */
    Data: T;
}