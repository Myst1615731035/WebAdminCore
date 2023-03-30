import 'xe-utils';
import VXETable from 'vxe-table';
import VXETablePluginExportXLSX from 'vxe-table-plugin-export-xlsx';
import VXETablePluginExportPDF from 'vxe-table-plugin-export-pdf';
import VXETablePluginRenderer from 'vxe-table-plugin-renderer';
import VXETablePluginMenus from 'vxe-table-plugin-menus';
import VXETablePluginElement from 'vxe-table-plugin-element';
import formatOptions from './vxe-table-formatter';
import '../vxe-table-plugins'; // Vxe-Table 自建扩展插件
import http from '../utils/http';
const { postPage: $postPage } = http;

VXETable.setup({
	size: 'medium', // 全局尺寸
	zIndex: 10,
	version: 0,
	loadingText: '加载中...', // 自定义loading提示内容，如果为null则不显示文本
	table: {
		showHeader: true,
		keepSource: true,
		showOverflow: true,
		showHeaderOverflow: true,
		showFooterOverflow: true,
		size: 'medium',
		autoResize: true,
		stripe: false,
		border: true,
		round: true,
		emptyText: '暂无数据',
		rowConfig: { keyField: '_X_ROW_KEY' },
		radioConfig: { trigger: 'default' },
		checkboxConfig: { strict: false, highlight: false, range: false, trigger: 'default' },
		sortConfig: { remote: false, trigger: 'default', orders: ['asc', 'desc', null], sortMethod: null },
		filterConfig: { remote: false, filterMethod: null },
		expandConfig: { trigger: 'default', showIcon: true },
		treeConfig: { rowField: 'id', parentField: 'parentId', children: 'children', hasChild: 'hasChild', mapChildren: '_X_ROW_CHILD', indent: 20, showIcon: true },
		tooltipConfig: { enterable: true },
		menuConfig: { visibleMethod() {} },
		editConfig: { mode: 'cell', showAsterisk: true },
		importConfig: { modes: ['insert', 'covering'] },
		exportConfig: { modes: ['current', 'selected'] },
		customConfig: { storage: false },
		scrollX: { gt: 60 },
		scrollY: { gt: 100 }
	},
	grid: {
		size: 'medium',
		height: 'auto',
		border: true,
		headerAlign: 'center',
		resizable: true,
		showHeaderOverflow: true,
		showOverflow: true,
		keepSource: true,
		tooltipConfig: { showAll: true },
		zoomConfig: { escRestore: true },
		rowConfig: { isCurrent: true, isHover: true },
		pagerConfig: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
		toolbarConfig: { className: 'grid-toolbar', refresh: false, zoom: true, custom: true },
		formConfig: { preventSubmit: false },
		proxyConfig: {
			seq: true,
			sort: true,
			filter: true,
			form: true,
			props: { list: 'response', result: 'response', total: 'total' },
			beforeItem: null,
			beforeColumn: null,
			beforeQuery: null,
			afterQuery: null,
			beforeDelete: null,
			afterDelete: null,
			beforeSave: null,
			afterSave: null
		},
		editConfig: { trigger: 'click', mode: 'row', showStatus: true }
	},
	pager: { align: 'center', border: true, background: true, perfect: true, pageSize: 50, pageSizes: [50, 100, 200] },
	form: {
		size: 'medium',
		className: 'form-content',
		titleOverflow: true,
		titleWidth: 100,
		titleAlign: 'right',
		titleColon: true,
		preventSubmit: true,
		validConfig: { autoPos: true },
		tooltipConfig: { enterable: true },
		titleAsterisk: true
	},
	input: {
		size: 'medium',
		transfer: false,
		parseFormat: 'yyyy-MM-dd HH:mm:ss',
		labelFormat: 'yyyy-MM-dd HH:mm:ss',
		valueFormat: 'yyyy-MM-dd HH:mm:ss',
		startDay: 0,
		digits: 2,
		controls: false
	},
	textarea: { size: 'medium', showWordCount: true, autosize: { minRows: 1, maxRows: 10 } },
	select: { size: 'medium', transfer: false, clearable: true, optionConfig: { keyField: '_X_OPTION_KEY' }, multiCharOverflow: 20 },
	toolbar: { size: 'medium', import: { mode: 'covering' }, export: { types: ['csv', 'html', 'xml', 'txt'] }, custom: { isFooter: true }, buttons: [], tools: [] },
	button: { size: 'medium', transfer: false, round: true },
	radio: { size: 'medium' },
	checkbox: { size: 'medium' },
	switch: { size: 'medium' },
	modal: {
		size: 'medium',
		id: `modal-${Math.random()}`,
		modelValue: false,
		mask: true,
		marginSize: -1,
		title: '',
		resize: true,
		escClosable: true,
		confirmButtonText: '保存',
		cancelButtonText: '取消',
		showZoom: true,
		lockView: false
		// showFooter: true,
	},
	list: { scrollY: { gt: 100 } }
});
// 请求结束，显示提示
const alertResponse = res => {
	res = res || {};
	if (!!res.msg && res.success != undefined && res.success != null) VXETable.modal.message({ content: `${res.msg}`, status: res.success ? 'success' : 'error' });
};
const formSubmitResponse = (node, res = {}) => {
	if (!!node) {
		var parent = node.$parent;
		if (!!parent && !!parent.updateRow && typeof parent.updateRow == 'function' && !!res.success) parent.updateRow(res.response || res.data);
		alertResponse(res);
		var modal = node.$refs.modal;
		if (!!modal && !!modal.close && typeof modal.close == 'function') modal.close();
	}
};
const gridQueryParam = (page, sorts, filters, form) => {
	return Object.assign(Object.assign({ isAll: page == undefined }, page), {
		keyword: form.keyword,
		form: Object.assign({}, form),
		sorts: Object.assign({}, sorts),
		filters: Object.assign({}, filters)
	});
};
const install = app => {
	// 补充插件
	VXETable.use(VXETablePluginExportXLSX);
	VXETable.use(VXETablePluginExportPDF);
	VXETable.use(VXETablePluginRenderer);
	VXETable.use(VXETablePluginMenus);
	VXETable.use(VXETablePluginElement);
	VXETable.formats.mixin(formatOptions);
	app.use(VXETable);
	// 弹窗
	app.config.globalProperties.$modal = VXETable.modal.open;
	app.config.globalProperties.$alert = VXETable.modal.alert;
	app.config.globalProperties.$confirm = VXETable.modal.confirm;
	app.config.globalProperties.$message = VXETable.modal.message;
	// 文件操作
	app.config.globalProperties.$print = VXETable.print;
	app.config.globalProperties.$saveFile = VXETable.saveFile;
	app.config.globalProperties.$readFile = VXETable.readFile;
	// 自定义快捷功能
	// 表格请求的公用方法
	app.config.globalProperties.$gridQuery = url => {
		return ({ page, sorts, filters, form }) => new Promise((resolve, reject) => resolve($postPage(url, gridQueryParam(page, sorts, filters, form)))).then(res => res || {});
	};
	app.config.globalProperties.$treeGridQuery = url => {
		return ({ page, sorts, filters, form }) => new Promise((resolve, reject) => resolve($postPage(url, gridQueryParam(page, sorts, filters, form)))).then(res => res || {});
	};
	app.config.globalProperties.$alertRes = alertResponse;
	app.config.globalProperties.$formSubmitAfter = formSubmitResponse;
	app.config.globalProperties.$fromValidErrorMsg = () => VXETable.modal.message({ content: `数据错误，请检查`, status: 'warning' });
	//定义弹窗公用取消函数
	app.config.globalProperties.beforeHideMethod = fromFormValid => {
		if (fromFormValid === true || fromFormValid.type == 'confirm') return new Error();
	};
};

export default {
	install
};
