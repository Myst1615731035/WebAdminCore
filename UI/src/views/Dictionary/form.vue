<template>
	<vxe-modal ref="modal" v-model="show" v-bind="modalOption" @show="modalShow" @confirm="confirm" @hide="modalHide" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
		<vxe-grid ref="itemGrid" v-bind="gridOption" @toolbar-button-click="toolBtnClick" @cell-click="cellClick"></vxe-grid>
	</vxe-modal>
</template>

<script>
import { toRaw } from 'vue';
const data = { Id: '', Code: '', Name: '', Description: '', Sort: 0, IsDelete: false, Items: [] };
const item = { Id: '', Pid: '', Label: '', EnLabel: '', Value: null, Description: '', Sort: 0, IsDelete: false };
const pageTitle = '字典';
let self;
const query = async ({ page, sorts, filters, form }) => {
	return await new Promise((resolve, reject) => {
		page = Object.assign(Object.assign({ isAll: page == undefined }, page), {
			form: Object.assign({ Id: self.params.data.Id }, form),
			sorts: Object.assign({}, sorts),
			filters: Object.assign({}, filters)
		});
		resolve(self.$postPage(self.$store.state.serverApi.dictionary.itemList, page));
	}).then(res => res);
};
export default {
	created() {
		self = this;
	},
	props: { params: { type: Object, required: true } },
	data() {
		return {
			show: false,
			modalOption: { title: '', type: 'confirm', showFooter: true, width: window.innerWidth * 0.6, height: window.innerHeight * 0.8 },
			formOption: {
				data: {},
				className: 'form-content',
				titleOverflow: true,
				titleWidth: 100,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Name', title: '字典名称', span: 12, itemRender: { name: '$input' } },
					{ field: 'Description', title: '字典描述', span: 12, itemRender: { name: '$input' } },
					{ field: 'Code', title: '字典Key', span: 12, itemRender: { name: '$input' } },
					{ field: 'Sort', title: '排序', span: 6, itemRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'IsDelete', title: '状态', span: 6, itemRender: { name: '$switch', props: this.$store.state.cache.IsDeleteSwitch } }
				],
				rules: { Name: [{ required: true, message: '请输入字典名称' }], Code: [{ required: true, message: '请输入字典key' }] }
			},
			gridOption: {
				border: true,
				headerAlign: 'center',
				resizable: true,
				showHeaderOverflow: true,
				showOverflow: true,
				highlightHoverRow: true,
				keepSource: true,
				height: '85%',
				highlightCurrentRow: true,
				highlightHoverRow: true,
				showOverflow: true,
				tooltipConfig: { showAll: true },
				toolbarConfig: { buttons: [{ code: 'add', name: '新增', icon: 'fa fa-plus' }, { code: 'del', name: '删除', icon: 'fa fa-trash' }] },
				editConfig: { enabled: true, trigger: 'click', mode: 'row', showStatus: true },
				proxyConfig: { ajax: { query: query, queryAll: query } },
				pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 100, pageSizes: [100] },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Label', title: '项标识', editRender: { name: '$input' } },
					{ field: 'EnLabel', title: '项标识(En)', editRender: { name: '$input' } },
					{ field: 'Value', title: '值', align: 'center', editRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'Description', title: '说明', editRender: { name: '$input' } },
					{ field: 'Sort', title: '排序', align: 'center', editRender: { name: '$input', props: { type: 'number' } } },
					{
						field: 'IsDelete',
						title: '状态',
						align: 'center',
						formatter: ({ cellValue }) => {
							if (cellValue) return '关闭';
							else return '激活';
						},
						editRender: { name: '$switch', props: this.$store.state.cache.IsDeleteSwitch }
					}
				],
				editRules: {
					Label: [{ required: true, message: '请输入项标识' }],
					Value: [{ required: true, message: '请输入值' }]
				}
			}
		};
	},
	watch: {
		params: {
			immediate: true,
			deep: true,
			handler: function(val) {
				if (val.show) {
					// 新增
					if (IsEmpty(val.data)) {
						self.formOption.data = DeepClone(data);
						self.modalOption.title = `新增${pageTitle}`;
					} else {
						// 编辑
						self.formOption.data = DeepClone(val.data);
						self.modalOption.title = `编辑${pageTitle}`;
					}
					self.show = true;
				}
			}
		}
	},
	methods: {
		async modalShow() {
			self.$refs.itemGrid.commitProxy('query');
		},
		async confirm() {
			if (await self.dataValidate()) {
				var records = self.$refs.itemGrid.getRecordset();
				var data = self.formOption.data;
				data.Items = [];
				Object.values(records).forEach(t => {
					if (t.length > 0) {
						t.forEach(i => {
							data.Items.push(toRaw(i));
						});
					}
				});
				self.$post(self.$store.state.serverApi.dictionary.save, data).then(res => {
					if (res.success) {
						// 执行父页面方法后关闭窗口
						self.$parent.updateRow(res.response);
						self.$refs.modal.close();
					} else {
						// 如果保存失败，阻止关闭窗口，提示
						self.$refs.modal.beforeHideMethod(true);
					}
					self.$message({ content: `${res.msg}`, status: res.success ? 'success' : 'error' });
				});
			} else {
				self.$message({ content: `参数不符，请检查`, status: 'warning' });
				self.$refs.modal.beforeHideMethod(true);
				return;
			}
		},
		async dataValidate() {
			const self = this;
			let form = await self.$refs.form.validate().catch(form => form);
			let grid = await self.$refs.itemGrid.validate().catch(grid => grid);
			var res = true;
			if (IsNotEmpty(form) || IsNotEmpty(grid)) {
				res = false;
			}
			return res;
		},
		modalHide() {
			this.formOption.data = {};
			self.$refs.form.clearValidate();
			self.$refs.itemGrid.clearValidate();
		},
		cellClick({ row, column }) {
			console.log({ row, column });
		},
		toolBtnClick({ code }) {
			if (code == 'add') {
				self.$refs.itemGrid.insert(item);
			}
			if (code == 'del') {
				var row = self.$refs.itemGrid.getCurrentRecord();
				if (IsEmpty(row)) {
					self.$message({ content: `请选择记录进行删除`, status: 'warning' });
				}
				if (IsEmpty(row.Id)) {
					self.$refs.itemGrid.removeCurrentRow();
				} else {
					self.$message({ content: `请手动设置该项的状态为‘关闭’`, status: 'warning' });
					self.$refs.itemGrid.setEditCell(row, 'IsDelete');
				}
			}
		}
	}
};
</script>

<style></style>
