<template>
	<el-tree-select ref="treeSelect" v-model="data[field]" v-bind="config" :filter-node-method="query" @change="change" @clear="clear" @focus="focus" @blur="blur"></el-tree-select>
</template>

<script>
export default {
	name: '$treeSelect',
	props: {
		data: { type: Object, require: true },
		field: { type: String, require: true },
		props: { type: Object },
	},
	emits: ['change', 'clear', 'focus', 'blur'],
	computed: {
		config() {
			return Object.assign(
				{
					valueKey: 'Id',
					nodeKey: 'Id',
					filterable: true,
					clearable: true,
					placeholder: '',
					checkStrictly: false,
					props: { label: 'Name', value: 'Id', children: 'Children' },
				},
				this.props
			);
		},
	},
	methods: {
		query(value, data) {
			value = (value || '').toLocaleLowerCase();
			var labelKey = (this.config.props || {}).label || '';
			return (data[labelKey] || '').toLocaleLowerCase().indexOf(value) > -1;
		},
		change(val) {
			var option = this.$refs.treeSelect.getCurrentNode(val);
			this.$emit('change', { value: val, row: !!val ? option : null });
		},
		clear() {
			this.change(null);
			this.$emit('clear');
		},
		focus(event) {
			this.$emit('focus', { event });
		},
		blur(event) {
			this.$emit('blur', { event });
		},
	},
};
</script>

<style></style>
