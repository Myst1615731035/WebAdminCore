<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @show="open" @confirm="confirm" @close="close" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
		<vxe-grid v-show="menuType == 1" ref="grid" v-bind="gridOption" @toolbar-button-click="toolClick" @toolbar-tool-click="toolClick"></vxe-grid>
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
				className: 'form-content',
				titleOverflow: true,
				titleWidth: 120,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false, resetValue: '', itemRender: { name: '$text' } },
					{
						field: 'Type',
						title: '类型',
						resetValue: null,
						span: 24,
						itemRender: { name: '$radioBtnGroup', props: { options: this.GetCacheDictItem('PermissionType'), optionProp: { label: 'Label', value: 'Value' } } }
					},
					{ field: 'Pid', title: '父级菜单', resetValue: '', span: 24, itemRender: { name: '$treeSelect', props: { data: [], checkStrictly: true } } },
					{ field: 'Name', title: '菜单/按钮名称', resetValue: '', span: 24, itemRender: { name: '$input' } },
					{ field: 'Path', title: '路由地址', resetValue: null, span: 24, itemRender: { name: '$input' } },
					{ field: 'IsDelete', title: '启用', resetValue: false, span: 8, itemRender: { name: '$switch', props: this.$store.state.cache.TrueSwitch } },
					{ field: 'Visiable', title: '可见', resetValue: true, span: 8, itemRender: { name: '$switch', props: this.$store.state.cache.FalseSwitch } },
					{ field: 'Description', title: '描述', resetValue: null, span: 24, itemRender: { name: '$textarea' } }
				],
				rules: { Type: [{ required: true, message: '请先选择类型' }], Name: [{ required: true, message: '请输入菜单名称' }] }
			},
			gridOption: {
				data: [],
				height: 500,
				headerAlign: 'center',
				resizable: true,
				keepSource: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, isCurrent: true, isHover: true },
				toolbarConfig: {
					buttons: [{ name: '按钮列表', type: 'text' }],
					tools: [
						{ code: 'insert_actived', name: '新增', icon: 'fa fa-plus' },
						{ code: 'del', name: '删除', icon: 'fa fa-trash' }
					],
					zoom: false,
					custom: false
				},
				editConfig: { enabled: true, trigger: 'click', mode: 'row', showStatus: true },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Id', title: '主键', visible: false, resetValue: '', itemRender: { name: '$text' } },
					{ field: 'Mid', title: '菜单主键', resetValue: '', visible: false },
					{ field: 'Name', title: '按钮名称', width: 160, editRender: { name: '$input' } },
					{ field: 'Code', title: '方法名称/编码', align: 'center', width: 160, editRender: { name: '$input' } },
					{ field: 'Sort', title: '排序', align: 'center', width: 120, editRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'Description', title: '说明', editRender: { name: '$input' } }
				],
				editRules: { Name: [{ required: true, message: '请输入按钮名称' }], Code: [{ required: true, message: '请输入编码' }] }
			}
		};
	},
	computed: {
		menuType() {
			return (this.formOption.data || {}).Type || null;
		}
	},
	watch: {
		modalShow: {
			immediate: true,
			handler: function (val) {
				this.$emit('update:show', val);
			}
		},
		data: {
			immediate: true,
			deep: true,
			handler: function (val) {
				this.formOption.data = DeepClone(val || {});
				this.gridOption.data = this.formOption.data.Buttons || [];
				this.modalOption.title = `${IsEmpty(val) ? '新增' : '编辑'}`;
			}
		}
	},
	mounted() {},
	methods: {
		async open() {
			this.$post(this.$store.state.serverApi.permission.list, { isOption: true }).then((res) => {
				this.formOption.items.find((t) => t.field == 'Pid').itemRender.props.data = res.data || [];
				if (IsEmpty(this.data)) this.$refs.form.reset();
			});
		},
		async confirm() {
			let err1 = await this.$refs.form.validate().catch((err1) => err1),
				err2 = await this.$refs.grid.validate().catch((err2) => err2);
			if (IsEmpty(err1) && IsEmpty(err2)) {
				this.formOption.data.Buttons = this.$refs.grid.getTableData().fullData;
				this.$post(this.serverApi.permission.save, this.formOption.data).then((res) => this.$formSubmitAfter(this, res));
			} else this.$fromValidErrorMsg();
		},
		async close() {},
		toolClick({ code }) {
			const funcs = {
				insert_actived: () => {
					var rows = this.$refs.grid.getInsertRecords();
					if (!!rows && rows.length > 0)
						rows.forEach((t) => {
							t.Id = t.Id || '';
							t.Mid = t.Mid || this.data.Id;
						});
					console.log(this.$refs.grid.getTableData().fullData);
				},
				del: () => {
					var row = this.$refs.grid.getCurrentRecord();
					if (IsEmpty(row)) this.$message({ content: `请选择记录进行删除`, status: 'warning' });
					else this.$refs.grid.removeCurrentRow();
				}
			};
			if (!!funcs[code]) funcs[code]();
		}
	}
};
</script>

<style></style>
