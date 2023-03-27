// 添加缓存刷新功能
import refresh from './js/refreshLoad.js';
// 添加缓存获取的通用方法
import getcache from './js/getCache.js';
import VueClipBoard from 'vue-clipboard2';
export default {
	install: app => {
		app.use(VueClipBoard)
			.use(refresh)
			.use(getcache);
	}
};
