<template>
	<vxe-modal ref="modal" v-model="modalShow" v-bind="modalOption" @close="modalClose" @confirm="confirm" :before-hide-method="beforeHideMethod">
		<vxe-grid ref="grid" v-bind="gridOption"></vxe-grid>
	</vxe-modal>
</template>

<script>
let self;
const query = ({ page, sorts, filters, form }) => {
	return new Promise((resolve, reject) => {
		page = Object.assign(Object.assign({ isAll: page == undefined }, page), {
			form: Object.assign({}, form),
			sorts: Object.assign({}, sorts),
			filters: Object.assign({}, filters)
		});
		resolve(self.$postPage(self.$store.state.serverApi.website.list, page));
	}).then(res => res);
};
export default {
	props: {
		userId: { type: String, required: true },
		userName: { type: String, required: true },
		show: { type: Boolean, required: true },
		siteids: { type: Array, required: true }
	},
	created() {
		self = this;
	},
	data() {
		return {
			modalShow: this.show,
			modalOption: { title: '', type: 'confirm', showFooter: true, width: window.innerWidth * 0.6, height: window.innerHeight * 0.8 },
			gridOption: {
				border: true,
				headerAlign: 'center',
				resizable: true,
				showHeaderOverflow: true,
				showOverflow: true,
				highlightHoverRow: true,
				keepSource: true,
				height: 'auto',
				highlightCurrentRow: true,
				highlightHoverRow: true,
				showOverflow: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, keyField: 'Id' },
				proxyConfig: { ajax: { query: query, queryAll: query } },
				pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ type: 'checkbox', width: 80, align: 'center' },
					{ field: 'Id', title: 'ID', visible: false },
					{ field: 'Name', title: '站点名称' },
					{ field: 'Idtag', title: '站点字号' },
					{ field: 'MainDomain', title: '主站域名' },
					{ field: 'TestDomain', title: '测试域名' },
					{ field: 'IsDelete', title: '状态', width: 120, align: 'center' }
				],
				checkboxConfig: { checkRowKeys: [] }
			}
		};
	},
	watch: {
		show(val) {
			this.modalShow = val;
		},
		userName() {
			this.modalOption.title = `${this.userName} 站点授权`;
		},
		siteids: {
			immediate: true,
			deep: true,
			handler: function(val) {
				this.gridOption.checkboxConfig.checkRowKeys = val;
			}
		}
	},
	methods: {
		async confirm() {
			var checkList = this.$refs.grid.getCheckboxRecords(true);
			var siteIds = !!checkList ? checkList.map(t=>t.Id):[];
			self.$post(self.$store.state.serverApi.sysUser.saveUserSite, { userId:this.userId, siteIds }).then(res => {
				if (res.success) {
					// 执行父页面方法后关闭窗口
					self.$parent.updateRow(res.response);
					self.$refs.modal.close();
				}
				// 如果保存失败，阻止关闭窗口，提示
				else self.$refs.modal.beforeHideMethod(true);
				self.$message({ content: `${res.msg}`, status: res.success ? 'success' : 'error' });
			});
		},
		modalClose() {
			this.$emit('update:assing.show', false);
		}
	}
};
</script>

<style></style>
