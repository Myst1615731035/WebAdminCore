<template>
	<vxe-form :data="data[field]" :title-width="120" style="padding-left: 5%">
		<vxe-form-item v-for="(t, i) in data[field] || {}" :field="i" :item-render="{ name: '$editor', props: props }" :span="24" :key="i"></vxe-form-item>
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
