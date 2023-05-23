<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @show="open" @confirm="confirm" @close="close" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
		<vxe-grid ref="grid" v-bind="gridOption" @toolbar-button-click="toolClick" @toolbar-tool-click="toolClick"></vxe-grid>
	</vxe-modal>
</template>

<script>
export default {
	props: ['show'],
	props: { data: { type: Object, default: {} } },
	data() {
		return {
			modalShow: this.show,
			modalOption: { title: '', type: 'confirm', showFooter: true, width: window.innerWidth * 0.6, height: window.innerHeight * 0.8, confirmButtonText: '保存' },
			formOption: {
				data: {},
				titleWidth: 100,
				titleAlign: 'right',
				titleOverflow: true,
				preventSubmit: true,
				className: 'form-content',
				items: [
					{ field: 'Id', title: '主键', resetValue: '', visible: false },
					{ field: 'Name', title: '字典名称', resetValue: '', span: 24, itemRender: { name: '$input' } },
					{ field: 'Description', title: '字典描述', resetValue: '', span: 24, itemRender: { name: '$input' } },
					{ field: 'Code', title: '字典Key', resetValue: '', span: 24, itemRender: { name: '$input' } },
					{ field: 'Sort', title: '排序', resetValue: 0, span: 12, itemRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'IsDelete', title: '状态', resetValue: false, span: 12, itemRender: { name: '$switch', props: this.$store.state.cache.IsDeleteSwitch } }
				],
				rules: { Name: [{ required: true, message: '请输入字典名称' }], Code: [{ required: true, message: '请输入字典key' }] }
			},
			gridOption: {
				data: [],
				height: 500,
				headerAlign: 'center',
				resizable: true,
				keepSource: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, keyField: 'Id', isCurrent: true, isHover: true },
				toolbarConfig: {
					buttons: [{ code: 'insert_actived', name: '新增', icon: 'fa fa-plus' }, { code: 'del', name: '删除', icon: 'fa fa-trash' }],
					zoom: false,
					custom: false
				},
				editConfig: { enabled: true, trigger: 'click', mode: 'row', showStatus: true },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Label', title: '名称', width: 160, editRender: { name: '$input' } },
					{ field: 'Value', title: '值', align: 'center', width: 160, editRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'Sort', title: '排序', align: 'center', width: 120, editRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'Description', title: '说明', editRender: { name: '$input' } }
				],
				editRules: { Label: [{ required: true, message: '请输入项标识' }], Value: [{ required: true, message: '请输入值' }] }
			}
		};
	},
	watch: {
		modalShow: {
			immediate: true,
			handler: function(val) {
				this.$emit('update:show', val);
			}
		},
		data: {
			immediate: true,
			deep: true,
			handler: function(val) {
				this.formOption.data = val || {};
				this.gridOption.data = (val || {}).Items || [];
				this.modalOption.title = `${IsEmpty(val) ? '新增' : '编辑'}`;
			}
		}
	},
	methods: {
		async open() {
			if (IsEmpty(this.data)) this.$refs.form.reset();
		},
		async confirm() {
			let err = await this.$refs.form.validate().catch(err => err);
			err = await this.$refs.grid.validate().catch(err => err);
			this.formOption.data.Items = this.$refs.grid.getTableData().fullData;
			if (IsEmpty(err)) this.$post(this.serverApi.dictionary.save, this.formOption.data).then(res => this.$formSubmitAfter(this, res));
			else this.$fromValidErrorMsg();
		},
		async close() {},
		toolClick({ code }) {
			if (code == 'del') {
				var row = this.$refs.grid.getCurrentRecord();
				if (IsEmpty(row)) this.$message({ content: `请选择记录进行删除`, status: 'warning' });
				else this.$refs.grid.removeCurrentRow();
			}
		}
	}
};
</script>

<style></style>
