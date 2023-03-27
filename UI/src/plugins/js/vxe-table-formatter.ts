import $store from '../vue-plugins/store/store';

// 时间格式化
const formatDate = ({ cellValue }, fmt) => {
	if (IsEmpty(fmt)) {
		return FormatDate(cellValue);
	}
	return FormatDate(cellValue, fmt);
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
const formatList = ({ cellValue }, key) => {
	if (IsNotEmpty(key)) {
		var list = $store.state.cache.storage[key];
		if (IsNotEmpty(list)) {
			var option = list.find(t => t.value == cellValue);
			return !!option ? option.label : '';
		}
	}
	return '';
};
export default {
	formatDate,
	formatDict,
	formatList
};
