<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @cell-dblclick="toolClick({ code: 'edit' })" @toolbar-button-click="toolClick" @toolbar-tool-click="toolClick"></vxe-grid>
		<formPage v-model="form.show" :data="form.data"></formPage>
	</div>
</template>

<script>
import formPage from './form.vue';
export default {
	components: { formPage },
	data() {
		const query = this.$gridQuery(this.serverApi.sysRole.list);
		return {
			form: { show: false, data: null },
			gridOptions: {
				height: 'auto',
				headerAlign: 'center',
				resizable: true,
				keepSource: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, isCurrent: true, isHover: true },
				proxyConfig: { ajax: { query: query, queryAll: query } },
				pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
				toolbarConfig: {
					buttons: [
						{ code: 'add', name: '新增', icon: 'fa fa-plus' },
						{ code: 'edit', name: '编辑', icon: 'fa fa-edit' },
						{ code: 'del', name: '删除', icon: 'fa fa-trash' }
					]
				},
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ type: 'checkbox', width: 60, align: 'center' },
					{ field: 'Id', title: 'ID', visible: false },
					{ field: 'Name', title: '角色名称' },
					{ field: 'Description', title: '说明' },
					{ field: 'CreateTime', title: '创建时间', width: 180, align: 'center' },
					{ field: 'Sort', title: '排序', width: 120, align: 'center' },
					{ field: 'IsDelete', title: '有效', width: 120, align: 'center', formatter: ['formatBool', true] }
				],
				formConfig: {
					items: [
						{ field: 'keyword', span: 4, itemRender: { name: '$input', props: { placeholder: 'search...', clearable: true } } },
						{
							span: 4,
							align: 'left',
							collapseNode: false,
							itemRender: {
								name: '$buttons',
								children: [{ props: { type: 'submit', status: 'primary', icon: 'fa fa-search' } }, { props: {type: 'reset', icon: 'fas fa-redo' } }]
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
					else this.$message({ content: `请选择一行记录进行编辑`, status: 'warning' });
				},
				del: () => {
					var row = this.$refs.grid.getCurrentRecord();
					if (IsEmpty(row)) this.$message({ content: `请选择需要处理的记录`, status: 'warning' });
					else {
						this.$confirm({ content: '确认删除?' }).then(res => {
							if (res == 'confirm')
								this.$post(`${this.serverApi.sysRole.delete}?Id=${row.Id}`).then(res => {
									this.$alertRes(res);
									if (res.success) this.$refs.grid.remove(row);
								});
						});
					}
				}
			};
			if (!!funcs[code] && this.$CheckGridBtnAuth(this.$route, code)) funcs[code]();
		},
		updateRow(newRow) {
			const row = this.$refs.grid.getCurrentRecord();
			if (!!row && !!newRow) this.$refs.grid.reloadRow(row, newRow);
			else this.$refs.grid.commitProxy('query');
		}
	}
};
</script>

<style></style>
