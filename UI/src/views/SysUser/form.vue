<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @show="open" @confirm="confirm" @close="close" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
	</vxe-modal>
</template>

<script>
export default {
	props: ['show'],
	props: { data: { type: Object, default: {} } },
	data() {
		return {
			modalShow: this.show,
			modalOption: { title: '', type: 'confirm', showFooter: true, width: window.innerWidth * 0.4, height: window.innerHeight * 0.6, confirmButtonText: '保存' },
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
				this.modalOption.title = `${IsEmpty(val) ? '新增' : '编辑'}`;
			}
		}
	},
	methods: {
		async open() {
			if (IsEmpty(this.data)) this.$refs.form.reset();
			this.$postPage(this.serverApi.sysRole.list, { isAll: true }).then(res => {
				var item = this.formOption.items.find(t => t.field == 'RoleIds');
				if (!!item) item.itemRender.props.options = res.response;
			});
		},
		async confirm() {
			let err = await this.$refs.form.validate().catch(err => err);
			if (IsEmpty(err)) this.$post(this.serverApi.sysUser.save, this.formOption.data).then(res => this.$formSubmitAfter(this, res));
			else this.$fromValidErrorMsg();
		},
		async close() {}
	}
};
</script>

<style></style>
