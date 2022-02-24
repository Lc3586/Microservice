/**
 * 预备上传分片文件输出信息
 * */
class PreUploadChunkFileResponse {
    /**
     * 状态
     */
    State: PreUploadChunkFileState;

    /**
     * 上传标识
     */
    Key: string;
}