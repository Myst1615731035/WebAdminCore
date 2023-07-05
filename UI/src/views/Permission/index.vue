<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @cell-dblclick="toolClick({ code: 'edit' })" @toolbar-button-click="toolClick" @toolbar-tool-click="toolClick">
			<template #btns="{ row }">
				<vxe-button v-for="t in row.Buttons" :key="row.Id + t.Code" :content="t.Name"></vxe-button>
			</template>
		</vxe-grid>
		<pageForm v-model="form.show" :data="form.data"></pageForm>
		<sortModal v-model="sortConfig.show"></sortModal>
	</div>
</template>

<script>
import pageForm from './form.vue';
import sortModal from './sort.vue';
export default {
	components: { pageForm, sortModal },
	data() {
		const query = this.$treeGridQuery(this.serverApi.permission.list);
		return {
			form: { show: false, data: null },
			sortConfig: { show: false },
			gridOptions: {
				height: 'auto',
				headerAlign: 'center',
				resizable: true,
				keepSource: true,
				tooltipConfig: { showAll: true },
				rowConfig: { useKey: true, keyField: 'Id', isCurrent: true, isHover: true },
				proxyConfig: { props: { result: 'data' }, ajax: { query: query, queryAll: query } },
				treeConfig: { rowField: 'Id', parentField: 'Pid', children: 'Children', trigger: 'row', accordion: true, line: true, reserve: true, indent: 30 },
				toolbarConfig: {
					buttons: [
						{ code: 'add', name: '新增', icon: 'fa fa-plus' },
						{ code: 'edit', name: '编辑', icon: 'fa fa-edit' },
						{ code: 'del', name: '删除', icon: 'fa fa-trash' },
						{ code: 'sort', name: '排序', icon: 'fa fa-sort' }
					]
				},
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Id', title: 'Id', visible: false },
					{ field: 'Name', title: '菜单/按钮名称', treeNode: true },
					{ field: 'Path', title: '菜单路由地址' },
					{ field: 'Buttons', title: '按钮', slots: { default: 'btns' } },
					{ field: 'Type', title: '类型', width: 120, align: 'center', formatter: ['formatDict', 'PermissionType'] },
					{ field: 'Sort', title: '排序', width: 120, align: 'center' },
					{ field: 'Visiable', title: '可见', width: 120, align: 'center', formatter: ['formatBool'] },
					{ field: 'IsDelete', title: '启用', width: 120, align: 'center', formatter: ['formatBool', true] }
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
			const delFunc = (code, row) => {
				if (code == 'confirm') {
					this.$post(`${this.serverApi.permission.delete}?Id=${row.Id}`).then((res) => {
						this.$alertRes(res);
						if (res.success) this.$refs.grid.remove(row);
					});
				}
			};
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
						this.$confirm({ content: '确认删除?' }).then((res) => {
							if (!!row.Children && row.Children.length > 0) {
								// 二次确认
								this.$confirm({ content: '该菜单下还存在子菜单，是否一起删除? 确认则一起提交删除！' }).then((res2) => delFunc(res2, row));
							} else delFunc(res, row);
						});
					}
				},
				sort: () => {
					this.$refs.grid.clearCurrentRow();
					this.sortConfig.show = true;
				}
			};
			if (!!funcs[code] && this.$CheckGridBtnAuth(this.$route, code)) funcs[code]();
		},
		updateRow(newRow) {
			const row = this.$refs.grid.getCurrentRecord();
			if (IsEmpty(row)) this.$refs.grid.commitProxy('query');
			else this.$refs.grid.reloadRow(row, Object.assign(row, newRow));
		}
	}
};
</script>

<style></style>
