import { createApp } from "vue";
import App from "./App.vue";
import "@/styles/index.scss";
import router from "./router";

// 移动端 rem 单位适配
if (isMobileOrPc()) {
    var width = window.screen.width;
    window.document.getElementsByTagName("html")[0].style.fontSize =
        width / 7.5 + "px";
}

function isMobileOrPc() {
    if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)) {
        return true;
    } else {
        return false;
    }
}

createApp(App).use(router).mount("#app");