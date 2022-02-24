/**
 * 分片文件合并任务帮助类
 * */
class ChunkFileMergeTaskHelper {
    /**
     * 即时通讯工具
     */
    private SignalR: any;

    /**
     * 连接对象
     */
    private Connection: any;

    /**
     * 状态
     */
    State: HubState;

    /**
     * 信息
     */
    Info: string;

    /**
     * 新增任务
     * @param task 任务信息
     */
    AddTask?: (task: ChunkFileMergeTask) => Promise<void>;

    /**
     * 更新任务
     * @param id 文件
     * @param data 数据
     */
    UpdateTask?: (id: string, data: Record<string, any>) => Promise<void>;

    /**
     * 移除任务
     * @param md5 文件MD5值
     */
    RemoveTask?: (md5: string) => Promise<void>;

    /**
     * 更新分片来源信息
     * @param md5 文件MD5值
     * @param chunksSource 分片来源信息
     */
    UpdateChunksSource?: (md5: string, chunksSource: ChunksSourceInfo) => Promise<void>;

    /**
     * 初始化
     * @param md5 文件MD5值
     */
    async Init() {
        const promise = new Promise<void>((resolve, reject) => {
            ImportHelper.ImportFile(
                [
                    {
                        Tag: ImportFileTag.JS,
                        Attributes: {
                            type: 'text/javascript',
                            src: ApiUri.BaseUrl + '/utils/signalr.min.js'
                        }
                    }],
                () => {
                    this.SignalR = (<any>window).signalR;
                    resolve();
                });
        });

        return promise;
    }

    /**
     * 连接
     * */
    async Connect() {
        const promise = new Promise<void>((resolve, reject) => {
            this.Connection = new this.SignalR.HubConnectionBuilder()
                .withUrl(ApiUri.CFMTHub)
                .withAutomaticReconnect()
                .configureLogging(this.SignalR.LogLevel.Information)
                .build();

            this.Connection.onreconnecting(error => {
                this.State = HubState.已断开;
                this.Info = '正在尝试重新连接...';
            });

            this.Connection.onreconnected(connectionId => {
                console.info(connectionId);
                this.State = HubState.已连接;
                this.Info = '已重新连接至服务器';

                this.RegisterHubMethod();
            });

            this.Connection.onclose(() => {
                this.State = HubState.已关闭;
                this.Info = '连接已关闭';
            });

            this.Connection.on(CFMTHubMethod.新增任务, task => {
                if (this.AddTask) {
                    const method = this.AddTask(task);
                    if (method && method.then)
                        method.then();
                }
            });

            this.Connection.on(CFMTHubMethod.更新任务, (id, data) => {
                if (this.UpdateTask) {
                    const method = this.UpdateTask(id, data);
                    if (method && method.then)
                        method.then();
                }
            });

            this.Connection.on(CFMTHubMethod.移除任务, md5 => {
                if (this.RemoveTask) {
                    const method = this.RemoveTask(md5);
                    if (method && method.then)
                        method.then();
                }
            });

            this.Connection.on(CFMTHubMethod.更新分片来源信息, (md5, chunksSource) => {
                if (this.UpdateChunksSource) {
                    const method = this.UpdateChunksSource(md5, chunksSource);
                    if (method && method.then)
                        method.then();
                }
            });

            this.Connection
                .start()
                .then(() => {
                    this.State = HubState.已连接;
                    this.Info = '已连接至服务器';

                    this.RegisterHubMethod();

                    resolve();
                });
        });

        return promise;
    }

    /**
     * 注册集线器方法
     * */
    RegisterHubMethod() {

    }
}