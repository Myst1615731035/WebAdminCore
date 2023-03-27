<template>
	<div class="container-grid">
		<vxe-grid ref="grid" v-bind="gridOptions" @dblclick="cellDbClick" @toolbar-button-click="toolBtnClick" @toolbar-tool-click="toolBtnClick">
			<template #dictItems="{row}">
				<span>asdfasfasdfaf</span>
			</template>
		</vxe-grid>
		<pageForm :params="params"></pageForm>
	</div>
</template>

<script>
const query = async ({ page, sorts, filters, form }) => {
	return await new Promise((resolve, reject) => {
		page = Object.assign(Object.assign({ isAll: page == undefined }, page), {
			keyword: form.keyword,
			form: Object.assign({}, form),
			sorts: Object.assign({}, sorts),
			filters: Object.assign({}, filters)
		});
		resolve(self.$postPage(self.$store.state.serverApi.dictionary.list, page));
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
					{ field: 'Id', title: 'ID', visible: false },
					{ field: 'Name', title: '字典名称', width: 300, slots: { content: 'dictItems' } },
					{ field: 'Code', title: '标识', width: 300, slots: { content: 'dictItems' } },
					{ field: 'Description', title: '描述' },
					{ field: 'Sort', title: '排序', width: 120, align: 'center' },
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
					this.$refs.grid.clearCurrentRow();
					this.params = { show: true };
					break;
				case 'edit':
					var row = this.$refs.grid.getCurrentRecord();
					if (IsNotEmpty(row)) {
						this.params = { show: true, data: row };
					} else {
						self.$message({ content: `请选择一行记录进行编辑`, status: 'warning' });
					}
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
