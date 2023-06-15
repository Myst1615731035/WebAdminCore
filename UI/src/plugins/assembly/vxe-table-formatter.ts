import $store from '../vue-plugins/store/store';

// bool值格式化
const formatBoolWithData = ({ cellValue }, data, key, value, isReverse = false) => {
	var res = '';
	if (!!data && Array.isArray(data) && !!key && !!value) {
		var item = data.find((t) => t[value] == (cellValue && !isReverse));
		res = (item || {})[key];
	}
	return res;
};
// bool值格式化
const formatBool = ({ cellValue }, isReverse = false) => {
	if (isReverse) return !!cellValue ? 'x' : '√';
	else return !!cellValue ? '√' : 'x';
};
// 时间格式化
const formatDate = ({ cellValue }, fmt) => {
	if (!!cellValue) return FormatDate(new Date(cellValue), fmt);
	return '';
};
// 字典数据格式化
const formatDict = ({ cellValue }, key) => {
	if (IsNotEmpty(key) && IsNotEmpty($store.state.cache.storage.dict)) {
		var obj = $store.state.cache.storage.dict.find((t) => t.key == key);
		if (!!obj && !!obj.items) {
			var option = obj.items.find((t) => t.Value == cellValue);
			return !!option ? option.Label : '';
		}
	}
	return '';
};
// 缓存数据格式化
const formatList = ({ cellValue }, key, list = null) => {
	if (IsNotEmpty(key)) {
		if (list == null || list == undefined) list = $store.state.cache.storage[key];
		if (IsNotEmpty(list)) {
			var option = list.find((t) => t.Value == cellValue);
			return !!option ? option.Label : '';
		}
	}
	return '';
};
// 自定义数据列表格式化
const formatDynamicList = ({ cellValue }, list, key, label) => {
	if (!!cellValue) {
		if (!!key && !!list && list.length > 0 && Array.isArray(list) && !!label) {
			var pair = list.find((t) => t[key] == cellValue);
			return (pair || {})[label] || '';
		}
	}
	return '';
};

export default {
	formatBool,
	formatBoolWithData,
	formatDate,
	formatDict,
	formatList,
	formatDynamicList
};
