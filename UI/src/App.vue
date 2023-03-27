<template>
	<div class="container">
		<el-container v-if="route.meta.notLayout">
			<el-main><router-view></router-view></el-main>
		</el-container>
		<el-container v-else>
			<el-aside class="theme sidebar"><sidebar class="sidebar"></sidebar></el-aside>
			<el-main><container class="container"></container></el-main>
		</el-container>
	</div>
</template>

<script>
import sidebar from './structure/SideBar.vue';
import container from './structure/Container.vue';
import { useRoute } from 'vue-router';

export default {
	components: { sidebar, container },
	setup() {
		const route = useRoute();
		return { route };
	}
};
</script>

<style>
* {
	box-sizing: border-box;
	padding: 0%;
	margin: 0;
}
::-webkit-scrollbar {
	width: 8px;
	height: 8px;
	background-color: #ffffff;
}
::-webkit-scrollbar-thumb {
	background-color: #c1c1c1;
	border-radius: 4px;
}
html,
body,
#app {
	font-family: Avenir, Helvetica, Arial, sans-serif;
	-webkit-font-smoothing: antialiased;
	-moz-osx-font-smoothing: grayscale;
	text-align: center;
	color: #2c3e50;
	width: 100%;
	height: 100%;
	padding: 0;
}
.sidebar {
	width: 12.5rem !important;
	height: 100%;
}
.container {
	height: 100%;
}
.el-main {
	padding: 0 !important;
	overflow: hidden;
}
.el-container {
	height: 100%;
	width: 100%;
}
.el-select {
	width: 100%;
}
.el-select-dropdown__item {
	text-align: left;
}
.theme {
	color: rgb(255, 255, 255);
	background-color: rgb(48, 65, 86);
}
</style>

<style>
/* 重写vxe-table  form样式 开始*/
.vxe-form--item-inner {
	height: 100%;
}
.vxe-form--item-inner .vxe-form--item-content {
	height: 100%;
}

.vxe-form--item-inner .vxe-form--item-content .vxe-textarea {
	height: 100%;
}

.vxe-form--item-inner .vxe-form--item-content .ueditor {
	height: 100% !important;
}
.vxe-form--item-inner .vxe-form--item-content .ueditor > div {
	height: 100% !important;
}
.vxe-form--item-inner .vxe-form--item-content .ueditor > div > div {
	height: 100% !important;
}
.vxe-form--item-inner .vxe-form--item-content .ueditor > div > div > div {
	height: 100% !important;
}
.vxe-form--item-inner .vxe-form--item-content div.edui-editor {
	width: 100% !important;
}
.vxe-form--item-inner .vxe-form--item-content .edui-editor-iframeholder {
	width: 100% !important;
	height: calc(100% - 132px) !important;
}
.vxe-form--item textarea {
	height: 100%;
}
/* 重写vxe-table form样式 结束 */

/*	重写vxe-table grid样式 开始	*/
.grid-toolbar {
	border: solid #ededed 1px !important;
	background-color: #ededed !important;
	padding-left: 5px !important;
	padding-right: 5px !important;
}
.grid-toolbar .vxe-tools--wrapper button {
	margin-right: 0.875rem;
}
.grid-toolbar .vxe-tools--wrapper button.is--circle i.fa {
	margin-right: 0;
}
.vxe-table--render-default {
	color: black !important;
	font-weight: 400;
}
/* 重写vxe-table grid样式 结束*/

/* 重写vxe-table modal样式 结束*/
.vxe-modal--footer {
	background-color: #f8f8f8;
	border-top: 1px solid #ebeef5;
}
/* 重写vxe-table modal样式 结束*/

.form-content {
	/* height: 100%; */
}
</style>
<style>
/* 修复部分vxe-table的样式问题 */
.vxe-toolbar .vxe-buttons--wrapper {
	display: flex;
}
</style>
