import VXETable from 'vxe-table';
import $store from '../vue-plugins/store/store';
const menus = $store.state.layout.menu;
// 遍历菜单树，查找菜单
const FilterMenu = (path, list = menus) => {
	if (!!list && list.length > 0) {
		for (var t of list) {
			if (t.Path.toLocaleLowerCase() == path) return t;
			if (!!t.Children && t.Children.length > 0) return FilterMenu(path, t.Children);
		}
	}
	return null;
};

// 获取表格有权限的按钮列表
const GetGridBtnListByAuth = (route, btnConfigs) => {
	var menu = FilterMenu(route.path.toLocaleLowerCase());
	if (!!menu && !!(menus || {}).Buttons) {
		var btns = (menus || {}).Buttons || [];
		if (btns.length > 0 && !!btns.find(t => t.Code == code)) {
			var codes = btns.map(t => t.Code).filter(t => !!t);
			return btnConfigs.filter(t => codes.indexOf(t.Code));
		}
	}
	return [];
};

// 校验按钮是否具有权限
const CheckGridBtnAuth = (route, code) => {
	return true;
	var menu = FilterMenu(route.path.toLocaleLowerCase());
	if (!!menu && !!(menus || {}).Buttons) {
		var btns = (menus || {}).Buttons || [];
		if (btns.length > 0 && !!btns.find(t => t.Code == code)) return true;
	}
	VXETable.modal.message({ content: '暂无权限', status: 'warning' });
	return false;
};

const install = app => {
	app.config.globalProperties.$GetGridAuthBtnList = GetGridBtnListByAuth;
	app.config.globalProperties.$CheckGridBtnAuth = CheckGridBtnAuth;
};

export default { install };
