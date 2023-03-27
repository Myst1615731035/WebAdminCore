<template>
	<div>
		<el-sub-menu v-if="nav.Children && nav.Children.length > 0" :index="nav.Name">
			<template #title>
				<i v-if="!!nav.Icon" class="fa" :class="nav.Icon"></i>
				<span>{{ nav.Name }}</span>
			</template>
			<sidemenu v-for="(item, index) in nav.Children" :nav="item" :key="index"></sidemenu>
		</el-sub-menu>
		<el-menu-item v-else :index="nav.Path">
			<template #title>
				<i v-if="nav.Icon" class="fa" :class="nav.Icon"></i>
				<span>{{ nav.Name }}</span>
			</template>
		</el-menu-item>
	</div>
</template>

<script>
export default {
	name: 'sidemenu',
	props: { nav: { type: Object, require: true } },
	setup(props, context) {
		return {};
	}
};
</script>

<style>
i.fa {
	width: 0.875rem;
	height: 0.875rem;
	margin-right: 0.25rem;
}
</style>
