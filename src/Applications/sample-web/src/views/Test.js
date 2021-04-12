//导入
// import {  } from '';

//对外接口
export default {
    created() {
        main = this;
        importState();
    },
    methods: {

    },
    data: getData
}

//vue实例
var main;

/**
 * 导入成功
 */
function importState() {
    console.info("import success");
}

/**
 * 渲染数据
 */
var data = {
    info: 'Test page.'
};

/**
 * 获取渲染数据
 */
function getData() {
    return data;
}