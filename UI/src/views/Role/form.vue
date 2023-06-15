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
					{ field: 'Id', title: '主键', visible: false, resetValue: '', itemRender: { name: '$text' } },
					{ field: 'Name', title: '角色名称', resetValue: '', span: 24, itemRender: { name: '$input' } },
					{ field: 'Sort', title: '排序', resetValue: 0, span: 24, itemRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'IsDelete', title: '状态', resetValue: false, span: 24, itemRender: { name: '$switch', props: this.$store.state.cache.TrueSwitch } },
					{ field: 'Description', title: '说明', resetValue: '', span: 24, itemRender: { name: '$textarea', props: { rows: 5 } } }
				],
				rules: { Name: [{ required: true, message: '请输入角色名称' }], IsDelete: [{ required: true, message: '请选择角色状态' }] }
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
				this.formOption.data = DeepClone(val || {});
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
			if (IsEmpty(err)) this.$post(this.$store.state.serverApi.sysRole.save, this.formOption.data).then(res => this.$formSubmitAfter(this, res));
			else this.$fromValidErrorMsg();
		},
		async close() {}
	}
};
</script>

<style></style>
