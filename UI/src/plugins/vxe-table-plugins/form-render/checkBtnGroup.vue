<template>
	<div class="check-btn-group">
		<vxe-checkbox
			v-for="(item, index) in options"
			v-model="data[item[optionProp.value]]"
			:content="item[optionProp.label]"
			:key="`cbg_${item[optionProp.value]}_${index}`"
			class="check-btn-label"
		></vxe-checkbox>
	</div>
</template>

<script>
export default {
	name: 'checkBtnGroup',
	props: {
		data: { type: Object, required: true, default: {} },
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

<style>
.check-btn-group {
	display: inline-block;
	vertical-align: middle;
	line-height: 1;
	font-size: 0;
}

.check-btn-label {
	font-size: 14px;
	display: inline-block;
	vertical-align: middle;
	white-space: nowrap;
	user-select: none;
	line-height: 1;
	cursor: pointer;
	color: #606266;
	margin: 0 !important;
}

.check-btn-label .vxe-checkbox--icon {
	display: none !important;
}

.check-btn-label .vxe-checkbox--label {
	padding: 0 1em;
	line-height: 32px;
	display: inline-block;
	border-style: solid;
	border-color: #dcdfe6;
	border-width: 1px 1px 1px 0;
	max-width: 50em;
}
.check-btn-group .check-btn-label:first-child .vxe-checkbox--label {
	border-left: 1px solid #dcdfe6;
	border-radius: 4px 0 0 4px;
}
.check-btn-group .check-btn-label:last-child .vxe-checkbox--label {
	border-radius: 0 4px 4px 0;
}
.vxe-checkbox > input:checked + .vxe-checkbox--icon + .vxe-checkbox--label {
	color: #606266;
}
.check-btn-group .vxe-checkbox > input:checked + .vxe-checkbox--icon + .vxe-checkbox--label {
	background-color: #409eff;
	color: white;
}
</style>
