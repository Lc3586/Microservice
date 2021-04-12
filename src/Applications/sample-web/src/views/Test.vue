<template>
<div>
    <span class="message">{{info}}</span>
</div>
</template>
<script>
//导入样式
import "./Test.css";
//导入js
import js from "./Test";
export default { mixins: [js] };
</script>