/**
 * 分片文件合并任务中心前端方法
 * */
const enum CFMTHubMethod {
    新增任务 = 'AddTask',
    更新任务 = 'UpdateTask',
    移除任务 = 'RemoveTask',
    更新分片来源信息 = 'UpdateChunksSource'
}