<template>
	<vxe-radio-group ref="radioBtnGroup" v-model="data[field]">
		<vxe-radio-button v-for="(item, index) in options" :key="item[optionProp.value]" :label="item[optionProp.value]" :content="item[optionProp.label]"></vxe-radio-button>
	</vxe-radio-group>
</template>

<script>
export default {
	name: 'radioBtnGroup',
	props: {
		data: { type: Object, required: true },
		field: { type: String, required: true },
		props: { type: Object, default: {} }
	},
	data() {
		return {
			options: this.props.options || [],
			optionProp: this.props.optionProp || { label: 'label', value: 'value' }
		};
	}
};
</script>
<style></style>
