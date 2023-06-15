import { h } from 'vue';
import { Loading } from '@element-plus/icons-vue';
import { ElNotification, ElTreeSelect, ElSelect } from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
export default {
	install: (app) => {
		app.use(ElSelect).use(ElTreeSelect);

		app.config.globalProperties.$notify = ElNotification;
		app.config.globalProperties.$loadingNotify = (option) => {
			option = Object.assign(
				{ title: 'Info', icon: h('i', { class: 'el-icon is-loading' }, h(Loading)), duration: 0, dangerouslyUseHTMLString: true, message: '' },
				option || {}
			);
			return ElNotification(option);
		};
		app.config.globalProperties.$sucNotify = (option) => {
			option = Object.assign({ title: 'Info', duration: 3000, dangerouslyUseHTMLString: true, message: '', type: 'success' }, option || {});
			return ElNotification(option);
		};
		app.config.globalProperties.$errNotify = (option) => {
			option = Object.assign({ title: 'Info', duration: 3000, dangerouslyUseHTMLString: true, message: '', type: 'error' }, option || {});
			return ElNotification(option);
		};

		for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
			app.component(key, component);
		}
	},
};
