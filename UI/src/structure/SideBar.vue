<template>
	<el-container>
		<el-header class="logo">
			<div v-if="layout.collapse">
				<i v-if="layout.logo.icon != ''" :class="layout.logo.icon"></i>
				{{ layout.logo.title }}
			</div>
			<div v-else>{{ layout.logo.subTitle }}</div>
		</el-header>
		<el-main style="overflow-y: auto">
			<multiTenant></multiTenant>
			<el-menu :unique-opened="false" :default-active="layout.currentRoute.path" :background-color="'#304156'" :text-color="'#fff'" :router="true">
				<sideMenu v-for="(nav, index) in layout.menu" :nav="nav" :key="index"></sideMenu>
			</el-menu>
		</el-main>
	</el-container>
</template>

<script>
import { useStore } from 'vuex';
import sideMenu from './SideMenu.vue';
import multiTenant from './MultiTenant.vue';
export default {
	name: 'sidebar',
	components: { sideMenu, multiTenant },
	setup() {
		const $store = useStore();
		const { layout } = $store.state;
		return { layout };
	},
};
</script>

<style>
.logo {
	height: 3.125rem !important;
	line-height: 3.125rem;
}
.el-menu {
	border-right: solid 0 var(--el-menu-bg-color) !important;
}
</style>
