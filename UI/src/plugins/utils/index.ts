const modules = import.meta.globEager('./modules/*.ts');
export default {
	install: app =>{
		for (var t in modules) {
		    if(!!t && !!modules[t] && !!modules[t].default) app.use(modules[t].default)
		}
	}
};
