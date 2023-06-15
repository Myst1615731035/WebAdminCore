<template>
	<el-select
		ref="elSelect"
		v-model="data[field]"
		v-bind="config"
		:filter-method="query"
		@change="change"
		@clear="clear"
		@focus="focus"
		@blur="blur"
		@remove-tag="removeTag"
		@visible-change="visibleChange">
		<el-option v-for="item in options" :key="item[optionProps.value]" :label="item[optionProps.label]" :value="item[optionProps.value]"></el-option>
	</el-select>
</template>

<script>
export default {
	name: 'vxe-elSelect',
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object, default: {} },
	},
	emits: ['change', 'clear', 'focus', 'blur', 'removeTag', 'visibleChange'],
	data() {
		return {
			searchKey: '',
		};
	},
	computed: {
		config() {
			return Object.assign(
				{
					clearable: true,
					filterable: true,
					collapseTags: true,
					maxCollapseTags: 3,
					defaultFirstOption: true,
					automaticDropdown: true,
				},
				this.props
			);
		},
		options() {
			var arr = this.props.options || [];
			var labelKey = this.optionProps.label || '';
			if (IsNotEmpty(this.searchKey)) {
				var val = (this.searchKey || '').toLocaleLowerCase();
				arr = arr.filter((t) => t[labelKey].toLocaleLowerCase().indexOf(val) > -1);
			}
			return arr;
		},
		optionProps() {
			return Object.assign({ label: 'label', value: 'value' }, this.props.optionProps || {});
		},
	},
	methods: {
		query(val) {
			this.searchKey = val;
		},
		change(val) {
			this.$emit('change', { value: val });
		},
		clear() {
			this.$emit('clear');
		},
		focus(event) {
			this.$emit('focus', { event });
		},
		blur(event) {
			this.$emit('blur', { event });
		},
		removeTag(val) {
			this.$emit('removeTag', { val });
		},
		visibleChange(val) {
			this.$emit('visibleChange', { show: val });
		},
	},
};
</script>

<style></style>
