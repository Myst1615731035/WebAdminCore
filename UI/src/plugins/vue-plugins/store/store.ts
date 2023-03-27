import { toRaw } from 'vue';
import { createStore, createLogger } from 'vuex';
import createPersistedState from 'vuex-persistedstate';

// 全局获取vuex的分区数据
const modules = {};
const files = import.meta.globEager('./modules/*.js');
if (IsNotEmpty(files)) Object.keys(files).forEach(t => (modules[`${t.match(/[^/]+(?!.*\/)+(?=.js)/gi)[0]}`] = files[t].default));
// 日志
const debug = process.env.NODE_ENV !== 'production';
// 数据持久化处理
let plugins = [createPersistedState()];
if (debug) plugins.push(createLogger());
export default createStore({
	modules,
	getters: {
		// 设置全局获取参数的方法
		get: state => (...keys) => {
			try {
				var res = 'state';
				keys.map(t => (res += `['${t}']`));
				res = eval(res);
				return toRaw(res);
			} catch (ex) {
				console.log(`调用store.getters.get()参数错误,无法获取值, 参数为:${JSON.stringify(keys)}; 错误信息:${ex}`);
				return null;
			}
		}
	},
	strict: debug,
	plugins
});
