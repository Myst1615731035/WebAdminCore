<template>
	<vxe-modal ref="modal" v-model="show" v-bind="modalOption" @confirm="confirm" @hide="modalHide" :before-hide-method="beforeHideMethod">
		<vxe-form ref="form" v-bind="formOption"></vxe-form>
	</vxe-modal>
</template>

<script>
import { toRaw } from 'vue';
import {useStore} from 'vuex';
const pageTitle = '角色';
const data = { Id: '', Name: '', Description: '', Sort: 0, IsDelete: false };
let self, IsDeleteSwitch;
export default {
	setup(){
		const $store = useStore();
		IsDeleteSwitch = $store.state.cache.IsDeleteSwitch;
	},
	created() {
		self = this;
	},
	props: { params: { type: Object, required: true } },
	data() {
		return {
			show: false,
			isDelete: { openLabel: '激活', openValue: false, closeLabel: '关闭', closeValue: true },
			modalOption: {
				title: '',
				type: 'confirm',
				showFooter: true,
				width: window.innerWidth * 0.5,
				height: window.innerHeight * 0.6
			},
			formOption: {
				data: {},
				className: 'form-content',
				titleOverflow: true,
				titleWidth: 100,
				titleAlign: 'right',
				preventSubmit: true,
				items: [
					{ field: 'Id', title: '主键', visible: false },
					{ field: 'Name', title: '角色名称', span: 14, itemRender: { name: '$input' } },
					{ field: 'IsDelete', title: '状态', span: 5, itemRender: { name: '$switch', props: IsDeleteSwitch } },
					{ field: 'Sort', title: '排序', span: 5, itemRender: { name: '$input', props: { type: 'number' } } },
					{ field: 'Description', title: '说明', span: 24, itemRender: { name: '$textarea', props: { rows: 5 } } }
				],
				rules: { Name: [{ required: true, message: '请输入角色名称' }], IsDelete: [{ required: true, message: '请选择角色状态' }] }
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
			const self = this;
			if (await self.dataValidate()) {
				self.$post(self.$store.state.serverApi.sysRole.save, this.formOption.data).then(res => {
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
			var res = true;
			if (IsNotEmpty(form)) {
				res = false;
			}
			return res;
		},
		modalHide() {
			this.formOption.data = {};
		},
		createOption() {
			return { openLabel: '激活', openValue: false, closeLabel: '关闭', closeValue: true };
		}
	}
};
</script>

<style></style>
