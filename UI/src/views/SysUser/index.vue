<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @toolbar-button-click="toolBtnClick" @toolbar-tool-click="toolBtnClick"></vxe-grid>
		<pageForm :params="params"></pageForm>
	</div>
</template>

<script>
const query = ({ page, sorts, filters, form }) => {
	return new Promise((resolve, reject) => {
		page = Object.assign(Object.assign({ isAll: page == undefined }, page), {
			keyword: form.keyword,
			form: Object.assign({}, form),
			sorts: Object.assign({}, sorts),
			filters: Object.assign({}, filters)
		});
		resolve(self.$postPage(self.$store.state.serverApi.sysUser.list, page));
	}).then(res => res);
};
import pageForm from './form.vue';
let self;
export default {
	created() {
		self = this;
	},
	components: { pageForm },
	data() {
		return {
			params: {},
			assign: { userId: '', userName: '', show: false, siteids: [] },
			gridOptions: {
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
				toolbarConfig: {
					buttons: [
						{ code: 'add', name: '新增', icon: 'fa fa-plus' },
						{ code: 'edit', name: '编辑', icon: 'fa fa-edit' },
						{ code: 'assign', name: '站点授权', icon: 'fa fa-tags' }
					],
					tools: [{ code: 'resetpw', name: '重置密码', icon: 'fa fa-circle' }]
				},
				proxyConfig: { ajax: { query: query, queryAll: query } },
				pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Id', title: 'ID', visible: false },
					{ field: 'Name', title: '用户名称' },
					{ field: 'Account', title: '账户' },
					{ field: 'Sex', title: '性别', width: 120 },
					{ field: 'Birth', title: '生日', width: 140 },
					{ field: 'IsDelete', title: '状态', width: 120, align: 'center' }
				],
				formConfig: {
					titleWidth: 100,
					titleAlign: 'right',
					titleOverflow: true,
					items: [
						{ field: 'keyword', span: 4, itemRender: { name: '$input', props: { placeholder: 'search...', clearable: true } } },
						{
							span: 20,
							align: 'right',
							collapseNode: false,
							itemRender: {
								name: '$buttons',
								children: [{ props: { type: 'submit', content: 'search', status: 'primary' } }, { props: { type: 'reset', content: 'reset' } }]
							}
						}
					]
				}
			}
		};
	},
	methods: {
		toolBtnClick({ code }) {
			switch (code) {
				case 'add':
					this.$refs.grid.clearCurrentRow();
					this.params = { show: true };
					break;
				case 'edit':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) this.params = { show: true, data: row };
					else self.$message({ content: `请选择一行记录进行编辑`, status: 'warning' });
					break;
				case 'assign':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) this.assign = { show: true, userId: row.Id, userName: row.Name, siteids: Object.freeze(row.SiteIds) };
					else self.$message({ content: `请选择用户`, status: 'warning' });
					break;
				case 'resetpw':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row))
						this.$confirm({ content: `是否重置用户: ${row.Name} 的密码?`, confirmButtonText: '重置' }).then(res => {
							if (res == 'confirm') {
								this.$get(`${this.serverApi.sysUser.resetpw}?Id=${row.Id}`).then(res => {
									this.$alertRes(res);
									if (res.success) {
										this.$alert({
											content: `密码已更新，请点击"复制"按钮，获取新密码到剪贴板`,
											status: 'success',
											showClose: false,
											maskClosable: false,
											escClosable: false,
											cancelButtonText: null,
											confirmButtonText: '复制'
										}).then(code =>
											this.$copyText(res.data).then(
												s => this.$message({ content: '复制成功', status: 'success' }),
												e => this.$message({ content: '复制失败', status: 'error' })
											)
										);
									}
								});
							}
						});
					else self.$message({ content: `请选择用户`, status: 'warning' });
					break;
			}
		},
		cellDbClick() {
			this.toolBtnClick({ code: 'edit' });
		},
		updateRow(newRow) {
			const row = this.$refs.grid.getCurrentRecord();
			if (IsEmpty(row)) {
				this.$refs.grid.commitProxy('query');
			} else {
				this.$refs.grid.reloadRow(row, newRow);
			}
		}
	}
};
</script>

<style></style>
