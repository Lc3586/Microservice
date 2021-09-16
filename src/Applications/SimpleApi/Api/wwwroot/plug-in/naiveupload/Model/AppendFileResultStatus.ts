/**
 * 追加文件返回信息状态
 * */
const enum AppendFileResultStatus {
    成功 = 'OK',
    超出数量限制 = 'LIMIT',
    文件类型不合法 = 'TYPE'
}