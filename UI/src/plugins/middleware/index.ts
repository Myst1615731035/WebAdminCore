// 剪贴板方法
import VueClipBoard from 'vue-clipboard2';
const modules = import.meta.globEager('./modules/*.ts');

export default {
	install: app => {
		app.use(VueClipBoard);
		for (var t in modules) {
			if (!!t && !!modules[t] && !!modules[t].default) app.use(modules[t].default);
		}
	}
};
