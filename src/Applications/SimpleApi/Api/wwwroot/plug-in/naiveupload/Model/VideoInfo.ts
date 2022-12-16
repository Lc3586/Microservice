/**
 * 视频信息
 * */
class VideoInfo {
    /**
     * 媒体流
     */
    Streams: StreamInfo[];

    /**
     * 格式
     */
    Format: FormatInfo;

    /**
     * 章节
     */
    Chapters: ChapterInfo[];

    /**
     * 程序
     */
    Programs: ProgramInfo[];

    /**
     * 程序版本
     */
    Program_Version: ProgramVersionInfo;

    /**
     * 库版本
     */
    Library_Versions: LibraryVersionInfo[];
}

/**
 * 媒体流信息
 * */
class StreamInfo {
    /**
     * 参考帧数量
     */
    Refs: number;

    /**
     * AVC
     */
    Is_Avc: boolean;

    /**
     * 表示用几个字节表示NALU的长度
     */
    Nal_Length_Size: string;

    /**
     * 真实基础帧率
     */
    R_Frame_Rate: string;

    /**
     * 平均帧率
     */
    Avg_Frame_Rate: string;

    /**
     * 每帧时长
     */
    Time_Base: string;

    /**
     * 流开始时间
     */
    Start_Pts: number;

    /**
     * 首帧时间
     */
    Start_Time: number;

    /**
     * 首帧时间
     */
    Start_Time_Convert: string;

    /**
     * 流时长
     */
    Duration_Ts: number;

    /**
     * 时长(秒)
     */
    Duration: number;

    /**
     * 时长
     */
    Duration_Convert: string;

    /**
     * 码率
     */
    Bit_Rate: string;

    /**
     * 最大码率
     */
    Max_Bit_Rate: string;

    /**
     * 原生采样的比特数/位深
     */
    Bits_Per_Raw_Sample: number;

    /**
     * 视频流帧数
     */
    Nb_Frames: number;

    /**
     * 
     */
    Extradata: string;

    /**
     * 色度样品的位置
     */
    Chroma_Location: string;

    /**
     * 级别
     */
    Level: number;

    /**
     * 像素格式
     */
    Pix_Fmt: string;

    /**
     * 记录帧缓存大小
     * 视频的延迟帧数
     */
    Has_B_Frames: number;

    /**
     * 索引
     */
    Index: number;

    /**
     * 编码器名
     */
    Codec_Name: string;

    /**
     * 编码器名全称
     */
    Codec_Long_Name: string;

    /**
     * 简介
     */
    Profile: string;

    /**
     * 编码器类型
     */
    Codec_Type: string;

    /**
     * 编码器每帧时长
     */
    Codec_Time_Base: string;

    /**
     * 编码器标签名
     */
    Codec_Tag_String: string;

    /**
     * 编码器标签
     */
    Codec_Tag: string;

    /**
     * 配置
     */
    Disposition: DispositionInfo;

    /**
     * 采样点格式
     */
    Sample_Fmt: string;

    /**
     * 音频通道数
     */
    Channels: number;

    /**
     * 音频通道布局
     */
    Channel_Layout: string;

    /**
     * 采样点bit数
     */
    Bits_Per_Sample: number;

    /**
     * 帧宽度
     */
    Width: number;

    /**
     * 帧高度
     */
    Height: number;

    /**
     * 视频帧宽度
     */
    Coded_Width: number;

    /**
     * 视频帧高度
     */
    Coded_Height: number;

    /**
     * 
     */
    Closed_Captions: number;

    /**
     * 采样率
     */
    Sample_Rate: string;

    /**
     * 标签
     */
    Tags: StreamTagsInfo;
}

/**
 * 配置信息
 * */
class DispositionInfo {
    /**
     * 
     */
    Default: number;

    /**
     * 
     */
    Dub: number;

    /**
     * 
     */
    Original: number;

    /**
     * 
     */
    Comment: number;

    /**
     * 
     */
    Lyrics: number;

    /**
     * 
     */
    Karaoke: number;

    /**
     * 
     */
    Forced: number;

    /**
     * 
     */
    Hearing_Impaired: number;

    /**
     * 
     */
    Visual_Impaired: number;

    /**
     * 
     */
    Clean_Effects: number;

    /**
     * 
     */
    Attached_Pic: number;

    /**
     * 
     */
    Timed_Thumbnails: number;
}

/**
 * 流标签信息
 * */
class StreamTagsInfo {
    /**
     * 语言
     */
    Language: string;

    /**
     * 处理器名字
     */
    Handler_Name: string;

    /**
     * 创建时间
     */
    Creation_Time: Date;
}

/**
 * 格式信息
 * */
class FormatInfo {
    /**
     * 文件绝对路径
     */
    Filename: string;

    /**
     * 输入视频的AVStream个数
     */
    Nb_Streams: number;

    /**
     * 
     */
    Nb_Programs: number;

    /**
     * 格式名
     * 半角逗号[,]分隔
     */
    Format_Name: string;

    /**
     * 格式名全称
     */
    Format_Long_Name: string;

    /**
     * 首帧时间
     */
    Start_Time: number;

    /**
     * 首帧时间
     */
    Start_Time_Convert: string;

    /**
     * 时长(秒)
     */
    Duration: number;

    /**
     * 时长
     */
    Duration_Convert: string;

    /**
     * 文件大小
     */
    Size: number;

    /**
     * 码率
     */
    Bit_Rate: number;

    /**
     * 文件内容与文件拓展名匹配程度
     * 100为最高分, 低于25分时文件拓展名可能被串改.
     */
    Probe_Score: number;

    /**
     * 标签
     */
    Tags: StreamTagsInfo;
}

/**
 * 章节信息
 * */
class ChapterInfo {

}

/**
 * 程序信息
 * */
class ProgramInfo {

}

/**
 * 程序版本信息
 * */
class ProgramVersionInfo {
    /**
     * 
     */
    Version: string;

    /**
     * 
     */
    Copyright: string;

    /**
     * 
     */
    Compiler_Ident: string;

    /**
     * 
     */
    Configuration: string;
}

/**
 * 库版本信息
 * */
class LibraryVersionInfo {
    /**
     * 
     */
    Name: string;

    /**
     * 
     */
    Major: number;

    /**
     * 
     */
    Minor: number;

    /**
     * 
     */
    Micro: number;

    /**
     * 
     */
    Version: number;

    /**
     * 
     */
    Ident: string;
}