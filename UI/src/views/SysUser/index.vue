<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @cell-dblclick="toolClick({ code: 'edit' })" @toolbar-button-click="toolClick" @toolbar-tool-click="toolClick"></vxe-grid>
		<pageForm v-model="form.show" :data="form.data"></pageForm>
	</div>
</template>

<script>
import pageForm from './form.vue';
export default {
	components: { pageForm },
	data() {
		const query = this.$gridQuery(this.serverApi.sysUser.list);
		return {
			form: { show: false, data: null },
			gridOptions: {
				height: 'auto',
				headerAlign: 'center',
				resizable: true,
				keepSource: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, isCurrent: true, isHover: true },
				toolbarConfig: {
					buttons: [
						{ code: 'add', name: '新增', icon: 'fa fa-plus' },
						{ code: 'edit', name: '编辑', icon: 'fa fa-edit' },
						{ code: 'del', name: '删除', icon: 'fa fa-trash' },
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
					{ field: 'IsDelete', title: '状态', width: 120, align: 'center', formatter: ['formatBool', true] }
				],
				formConfig: {
					titleWidth: 100,
					titleAlign: 'right',
					titleOverflow: true,
					items: [
						{ field: 'keyword', span: 4, itemRender: { name: '$input', props: { placeholder: 'search...', clearable: true } } },
						{
							span: 4,
							align: 'left',
							collapseNode: false,
							itemRender: {
								name: '$buttons',
								children: [{ props: { type: 'submit', status: 'primary', icon: 'fa fa-search' } }, { props: { type: 'reset', icon: 'fas fa-redo' } }]
							}
						}
					]
				}
			}
		};
	},
	methods: {
		toolClick({ code }) {
			const funcs = {
				add: () => {
					this.$refs.grid.clearCurrentRow();
					this.form = { show: true, data: null };
				},
				edit: () => {
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) this.form = { show: true, data: row };
					else this.$message({ content: `请选择需要处理的记录`, status: 'warning' });
				},
				del: () => {
					var row = this.$refs.grid.getCurrentRecord();
					if (IsEmpty(row)) this.$message({ content: `请选择需要处理的记录`, status: 'warning' });
					else {
						this.$confirm({ content: '确认删除?' }).then(res => {
							if (res == 'confirm')
								this.$post(`${this.serverApi.sysUser.delete}?Id=${row.Id}`).then(res => {
									this.$alertRes(res);
									if (res.success) this.$refs.grid.remove(row);
								});
						});
					}
				},
				resetpw: () => {
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) {
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
					} else this.$message({ content: `请选择用户`, status: 'warning' });
				}
			};
			if (!!funcs[code] && this.$CheckGridBtnAuth(this.$route, code)) funcs[code]();
		},
		updateRow(newRow) {
			const row = this.$refs.grid.getCurrentRecord();
			if (IsEmpty(row)) this.$refs.grid.commitProxy('query');
			else this.$refs.grid.reloadRow(row, newRow);
		}
	}
};
</script>

<style></style>
