export default {
	namespaced: false,
	state: () => ({
		logo: { icon: 'fa fa-sitemap', title: '内容管理系统', subTitle: 'CMS' },
		tagsList: [],
		collapse: true,
		menu: [],
		currentRoute: {name:'',path:''}
	}),
	getters: {},
	mutations: {
		saveTagsData(state, data) {
			state.tagsStoreList = data;
			window.localStorage.setItem('Tags', data);
		},
		saveCollapse(state, data) {
			state.collapse = data;
		},
		savePermission(state, data) {
			state.menu = data;
			window.localStorage.setItem('Permission', JSON.stringify(data));
		},
		saveCurrentRoute(state, data){
			state.currentRoute = data;
			window.localStorage.setItem('CurrentRoute', JSON.stringify(data));
		}
	},
	actions: {}
};
