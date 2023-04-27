import $store from '../vue-plugins/store/store';

// bool值格式化
const formatBoolWithData = ({ cellValue }, data, key, value, isReverse = false) => {
	var res = '';
	if (!!data && Array.isArray(data) && !!key && !!value) {
		var item = data.find(t => t[value] == (cellValue && !isReverse));
		res = (item || {})[key];
	}
	return res;
};
// bool值格式化
const formatBool = ({ cellValue }, isReverse = false) => {
	if (isReverse) return !!cellValue ? '×' : '√';
	else return !!cellValue ? '√' : '×';
};
// 时间格式化
const formatDate = ({ cellValue }, fmt) => {
	if (!!cellValue) return FormatDate(new Date(cellValue), fmt);
	return '';
};
// 字典数据格式化
const formatDict = ({ cellValue }, key) => {
	if (IsNotEmpty(key) && IsNotEmpty($store.state.cache.storage.dict)) {
		var list = $store.state.cache.storage.dict.find(t => t.key == key);
		if (!!list) {
			var option = list.items.find(t => t.value == cellValue);
			return !!option ? option.label : '';
		}
	}
	return '';
};
// 缓存数据格式化
const formatList = ({ cellValue }, key, list = null) => {
	if (IsNotEmpty(key)) {
		if (list == null || list == undefined) list = $store.state.cache.storage[key];
		if (IsNotEmpty(list)) {
			var option = list.find(t => t.value == cellValue);
			return !!option ? option.label : '';
		}
	}
	return '';
};
export default {
	formatBool,
	formatBoolWithData,
	formatDate,
	formatDict,
	formatList
};
