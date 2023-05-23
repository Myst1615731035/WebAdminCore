import $store from '../../vue-plugins/store/store';
// 获取缓存中某一项
const GetCacheItem = (key, selectFunc = null) => {
	var res;
	try {
		res = $store.state.cache.storage.dict.find(t => t.key == key).items;
	} catch (e) {
		res = [];
	}
	return res;
};

// 获取字典数据
const GetCacheDictItem = (key, selectFunc = null) => {
	var obj = $store.state.cache.storage.dict.find(t => t.key == key);
	if (!!obj) {
		if (typeof selectFunc == 'function' && selectFunc != null) return selectFunc(obj.items);
		else return obj.items;
	}
	return [];
};

const install = app => {
	app.config.globalProperties.GetCacheItem = GetCacheItem;
	app.config.globalProperties.GetCacheDictItem = GetCacheDictItem;
	app.config.globalProperties.serverApi = $store.state.serverApi;
};
export default { install };
