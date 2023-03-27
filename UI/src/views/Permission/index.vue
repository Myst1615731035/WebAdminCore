<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @toolbar-button-click="toolBtnClick" @toolbar-tool-click="toolBtnClick"></vxe-grid>
		<pageForm :params="params"></pageForm>
		<sortModal v-model="sortConfig.show"></sortModal>
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
		resolve(self.$postPage(self.$store.state.serverApi.permission.list, page));
	}).then(res => res);
};
import pageForm from './form.vue';
import sortModal from './sort.vue';
let self;
export default {
	created() {
		self = this;
	},
	components: { pageForm, sortModal },
	data() {
		return {
			params: { show: false, data: null, menuTree: [] },
			sortConfig: { show: false },
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
				treeConfig: { rowField: 'Id', parentField: 'Pid', children: 'Children', trigger: 'row', accordion: true, line: true, reserve: true, indent: 30 },
				toolbarConfig: {
					buttons: [
						{ code: 'add', name: '新增', icon: 'fa fa-plus' },
						{ code: 'edit', name: '编辑', icon: 'fa fa-edit' },
						{ code: 'sort', name: '排序', icon: 'fa fa-sort' }
					]
				},
				proxyConfig: { props: { result: 'data' }, ajax: { query: query, queryAll: query } },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ field: 'Id', title: 'Id', visible: false },
					{ field: 'Name', title: '菜单/按钮名称', treeNode: true },
					{ field: 'Path', title: '菜单路由地址' },
					{ field: 'Url', title: 'Api接口' },
					{ field: 'Function', title: '按钮事件' },
					{ field: 'Type', title: '类型', width: 120, align: 'center', formatter: ['formatDict', 'PermissionType'] },
					{ field: 'Sort', title: '排序', width: 120, align: 'center' },
					{ field: 'Visiable', title: '是否可见', width: 120, align: 'center', formatter: ['formatDict', 'Visisable'] },
					{ field: 'IsDelete', title: '有效', width: 120, align: 'center' }
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
					self.$refs.grid.clearCurrentRow();
					self.GetMenuTree().then(res => (self.params = { show: true, menuTree: Object.freeze(res.data) }));
					break;
				case 'edit':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) self.GetMenuTree().then(res => (self.params = { show: true, data: row, menuTree: Object.freeze(res.data) }));
					else self.$message({ content: `请选择一行记录进行编辑`, status: 'warning' });
					break;
				case 'sort':
					self.$refs.grid.clearCurrentRow();
					this.sortConfig.show = true;
					break;
			}
		},
		cellDbClick() {
			self.toolBtnClick({ code: 'edit' });
		},
		updateRow(newRow) {
			const row = this.$refs.grid.getCurrentRecord();
			if (IsEmpty(row)) {
				this.$refs.grid.commitProxy('query');
			} else {
				this.$refs.grid.reloadRow(row, newRow);
			}
		},
		GetMenuTree() {
			return self.$post(self.$store.state.serverApi.permission.list);
		}
	}
};
</script>

<style></style>
