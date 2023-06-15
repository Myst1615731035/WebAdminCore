<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @show="open" @confirm="confirm" :before-hide-method="beforeHideMethod">
		<el-tree ref="sitelist" node-key="Id" :show-checkbox="true" :highlight-current="true" :props="listConfig.props" :data="listConfig.data"></el-tree>
	</vxe-modal>
</template>

<script>
export default {
	props: ['show'],
	props: {
		userId: { type: String, required: true },
		userName: { type: String, required: true },
		siteids: { type: Array, required: true },
	},
	data() {
		return {
			modalShow: this.show,
			modalOption: { title: this.modalTitle, type: 'confirm', showFooter: true, width: window.innerWidth * 0.6, height: window.innerHeight * 0.8 },
			listConfig: { data: [], props: { label: 'Name' } },
		};
	},
	watch: {
		modalShow: {
			immediate: true,
			handler: function (val) {
				this.$emit('update:show', val);
				this.modalOption.title = `${this.userName} - 站点授权`;
			},
		},
	},
	methods: {
		async open() {
			this.$post(this.serverApi.website.list, { isAll: true, isOption: true }).then((res) => {
				this.listConfig.data = res.data.response;
				this.$refs.sitelist.setCheckedKeys(this.siteids);
			});
		},
		async confirm() {
			var siteIds = this.$refs.sitelist.getCheckedKeys();
			this.$post(`${this.serverApi.sysUser.saveUserSite}?userId=${this.userId}`, siteIds).then((res) => this.$formSubmitAfter(this, res));
		},
	},
};
</script>

<style></style>
