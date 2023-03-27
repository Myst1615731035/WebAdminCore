<template>
	<vxe-modal ref="modal" v-model="show" v-bind="modalOption" @show="modalShow" @confirm="confirm" @hide="modalHide" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
	</vxe-modal>
</template>

<script>
const pageTitle = '用户';
const data = {
	Id: '',
	Account: '',
	Password: '',
	Name: '',
	Remark: null,
	RoleIds: [],
	IDCard: null,
	Tel: null,
	Email: null,
	Address: null,
	Sex: null,
	Birth: null,
	Sort: 0,
	IsDelete: false
};
let self, IsDeleteSwitch;
export default {
	created() {
		self = this;
	},
	props: { params: { type: Object, required: true } },
	data() {
		return {
			show: false,
			modalOption: { title: '', type: 'confirm', showFooter: true, width: window.innerWidth * 0.4, height: window.innerHeight * 0.6 },
			formOption: {
				data: {},
				className: 'form-content',
				titleOverflow: true,
				titleWidth: 100,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Account', title: '账户', span: 24, itemRender: { name: '$input' } },
					{
						field: 'Password',
						title: '密码',
						span: 24,
						visibleMethod: ({ data }) => IsEmpty(data.Password),
						itemRender: { name: '$input', props: { type: 'password' } }
					},
					{ field: 'Name', title: '昵称', span: 24, itemRender: { name: '$input' } },
					{
						field: 'RoleIds',
						title: '角色',
						span: 24,
						itemRender: { name: '$elSelect', props: { options: [], optionProps: { label: 'Name', value: 'Id' }, multiple: true } }
					},
					{ field: 'Birth', title: '生日', span: 12, itemRender: { name: '$input', props: { type: 'date', labelFormat: 'yyyy-MM-dd' } } },
					{ field: 'Age', title: '年龄', span: 12, itemRender: { name: '$input', props: { readonly: true, disabled: true } } },
					// { field: 'Sex', title: '性别', span: 12, itemRender: { name: '$radio' } },
					{ field: 'IDCard', title: '证件号', span: 24, itemRender: { name: '$input' } },
					{ field: 'Address', title: '地址', span: 24, itemRender: { name: '$input' } },
					{ field: 'Remark', title: '备注', span: 24, itemRender: { name: '$textarea', props: { rows: 3 } } }
				],
				rules: {
					Name: [{ required: true, message: '请输入昵称' }],
					Account: [{ required: true, message: '请输入账户' }],
					Password: [{ required: true, message: '请输入密码' }]
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
						this.formOption.data = DeepClone(data);
						this.modalOption.title = `新增${pageTitle}`;
					} else {
						// 编辑
						this.formOption.data = DeepClone(val.data);
						this.modalOption.title = `编辑${pageTitle}`;
					}
					this.show = true;
				}
			}
		}
	},
	methods: {
		async confirm() {
			if (await self.dataValidate()) {
				self.$post(self.$store.state.serverApi.sysUser.save, this.formOption.data).then(res => {
					if (res.success) {
						self.$parent.updateRow(res.response);
						self.$refs.modal.close();
					} else self.$refs.modal.beforeHideMethod(true);
					self.$message({ content: `${res.msg}`, status: res.success ? 'success' : 'error' });
				});
			} else {
				self.$message({ content: `参数不符，请检查`, status: 'warning' });
				self.$refs.modal.beforeHideMethod(true);
				return;
			}
		},
		async dataValidate() {
			let form = await self.$refs.form.validate().catch(form => form);
			return IsEmpty(form);
		},
		async modalShow() {
			this.$postPage(this.serverApi.sysRole.list, { isAll: true }).then(res => {
				var item = this.formOption.items.find(t => t.field == 'RoleIds');
				if (!!item) item.itemRender.props.options = res.response;
			});
		},
		modalHide() {
			this.formOption.data = {};
		}
	}
};
</script>

<style></style>
