import { ElNotification, ElTreeSelect, ElSelect } from 'element-plus';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';

const install = app => {
	app.use(ElSelect).use(ElTreeSelect);
	app.config.globalProperties.$Notic = ElNotification;
	for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
		app.component(key, component);
	}
};

export default { install };
