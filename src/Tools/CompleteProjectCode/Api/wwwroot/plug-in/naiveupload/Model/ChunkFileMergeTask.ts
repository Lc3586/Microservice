/**
 * 分片文件合并任务
 * */
class ChunkFileMergeTask {
    /***
     * Id
     */
    Id: string;

    /***
     * 服务器标识
     */
    ServerKey: string;

    /***
     * 文件MD5校验值
     */
    MD5: string;

    /***
     * 名称
     */
    Name: string;

    /***
     * 内容类型
     */
    ContentType: string;

    /***
     * 文件扩展名
     */
    Extension: string;

    /***
     * 字节数
     */
    Bytes: string;

    /***
     * 文件大小
     */
    Size: string;

    /***
     * 分片规格
     */
    Specs: string;

    /***
     * 分片总数
     */
    Total: string;

    /***
     * 当前处理分片的索引
     */
    CurrentChunkIndex: number;

    /***
     * 状态
     */
    State: string;

    /***
     * 信息
     */
    Info: string;

    /***
     * 创建时间
     */
    CreateTime: string;

    /***
     * 最近更新时间
     */
    ModifyTime: string;

    /***
     * 完成时间
     */
    CompletedTime: string;

    /***
     * 分片来源信息
     */
    ChunksSources: ChunksSourceInfo[];

    /***
     * 移除
     */
    Remove: boolean;
}