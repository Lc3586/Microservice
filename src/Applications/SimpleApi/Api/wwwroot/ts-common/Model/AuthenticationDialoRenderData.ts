/**
 * 身份验证对话框渲染数据
 * */
class AuthenticationDialoRenderData {
    /**
     * 
     * @param show 是否显示
     */
    constructor(show: boolean = false) {
        this.Show = show;
        this.Loading = true;
        this.Username = '';
        this.Password = '';
    }

    /**
     * 显示状态
     */
    Show: boolean;

    /**
     * 加载状态
     */
    Loading: boolean;

    /**
     * 用户名
     */
    Username: string;

    /**
     * 密码
     */
    Password: string;
}