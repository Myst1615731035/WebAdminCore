<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @toolbar-button-click="toolBtnClick" @toolbar-tool-click="toolBtnClick"></vxe-grid>
		<pageForm v-model="form.show" :data="form.data"></pageForm>
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
		resolve(self.$postPage(self.$store.state.serverApi.sysRole.list, page));
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
			form: { show: false, data: null },
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
				toolbarConfig: { buttons: [{ code: 'add', name: '新增', icon: 'fa fa-plus' }, { code: 'edit', name: '编辑', icon: 'fa fa-edit' }] },
				proxyConfig: { ajax: { query: query, queryAll: query } },
				pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
				columns: [
					{ type: 'seq', title: '序号', width: 60, align: 'center' },
					{ type: 'checkbox', width: 60, align: 'center' },
					{ field: 'Id', title: 'ID', visible: false },
					{ field: 'Name', title: '角色名称' },
					{ field: 'Description', title: '说明' },
					{ field: 'CreateTime', title: '创建时间', width: 180, align: 'center' },
					{ field: 'Sort', title: '排序', width: 120, align: 'center' },
					{ field: 'IsDelete', title: '有效', width: 120, align: 'center' }
				],
				formConfig: {
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
					this.form = { show: true, data: null };
					break;
				case 'edit':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) this.form = { show: true, data: row };
					else self.$message({ content: `请选择一行记录进行编辑`, status: 'warning' });
					break;
			}
		},
		cellDbClick() {
			this.toolBtnClick({ code: 'edit' });
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
