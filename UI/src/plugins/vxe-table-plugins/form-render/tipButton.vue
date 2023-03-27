<template>
	<el-tooltip><vxe-button v-bind="props" @click="click"></vxe-button></el-tooltip>
</template>

<script>
export default {
	name: 'tipButton',
	props: { props: { type: Object, default: {} } },
	emits: ['click'],
	methods: {
		click() {
			this.$emit('click');
		}
	}
};
</script>

<style></style>
