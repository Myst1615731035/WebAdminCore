<template>
	<vxe-form :data="data[field]" :title-width="120" style="border: 1px dashed green" :title-overflow="true" title-align="right" :prevent-submit="true">
		<vxe-form-item v-for="(item, index) in data[field]" :field="index" :title="index" :item-render="{ name: '$input' }" :span="24" :key="index"></vxe-form-item>
	</vxe-form>
</template>

<script>
export default {
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object, default: {} },
	},
	methods: {},
};
</script>

<style></style>
