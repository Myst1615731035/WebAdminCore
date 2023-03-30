<template>
	<vxe-modal ref="modal" v-model="show" v-bind="modalOption" @confirm="confirm" @hide="modalHide" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
	</vxe-modal>
</template>

<script>
const data = { Id: '', Fid: '', Name: '', Path: '', Icon: '', Description: '', Type: null, Sort: 0, IsDelete: false };
let self;
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
				titleWidth: 120,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Type', title: '类型', span: 24, itemRender: { name: '$radioBtnGroup', props: { options: this.GetCacheDictItem('PermissionType') } } },
					{ field: 'Name', title: '菜单/按钮名称', span: 12, itemRender: { name: '$input' } },
					{ field: 'Path', title: '路由地址', span: 12, itemRender: { name: '$input' } },
					{
						field: 'Pid',
						title: '父级菜单',
						span: 12,
						itemRender: {
							name: '$treeSelect',
							props: { data: [], checkStrictly: true }
						}
					},
					{ field: 'Fid', title: '接口地址', span: 12, itemRender: { name: '$input' } },
					{ field: 'IsDelete', title: '状态', span: 8, itemRender: { name: '$switch', props: this.$store.state.cache.IsDeleteSwitch } },
					{ field: 'Visiable', title: '隐藏', span: 8, itemRender: { name: '$switch' } },
					{ field: 'Sort', title: '排序', span: 8, itemRender: { name: '$input' } },
					{ field: 'Description', title: '描述', span: 24, itemRender: { name: '$textarea' } }
				],
				rules: {
					Type: [{ required: true, message: '请先选择类型' }],
					Name: [{ required: true, message: '请输入菜单名称' }]
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
					this.formOption.items.find(t => t.field == 'Pid').itemRender.props.data = val.menuTree;
					var isAdd = IsEmpty(val.data);
					this.formOption.data = isAdd ? DeepClone(data) : DeepClone(val.data);
					this.modalOption.title = isAdd ? '新增菜单' : '编辑菜单';
					this.show = true;
				}
			}
		}
	},
	methods: {
		async confirm() {
			if (await self.dataValidate()) {
				var url = self.formOption.data.Type == 2 ? self.serverApi.permission.saveBtn :self.serverApi.permission.save;
				self.$post(url, this.formOption.data).then(res => {
					if (res.success) {
						// 执行父页面方法后关闭窗口
						self.$parent.updateRow(res.data);
						self.$refs.modal.close();
					} else {
						// 如果保存失败，阻止关闭窗口，提示
						self.$refs.modal.beforeHideMethod(true);
					}
					self.$alertRes(res);
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
			var res = true;
			if (IsNotEmpty(form)) {
				res = false;
			}
			return res;
		},
		modalHide() {
			this.formOption.data = {};
		}
	}
};
</script>

<style></style>
