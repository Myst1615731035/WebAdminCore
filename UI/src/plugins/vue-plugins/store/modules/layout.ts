export default {
	namespaced: false,
	state: () => ({
		logo: { icon: 'fa fa-sitemap', title: 'Inventory Management System', subTitle: 'CRM' },
		tagsList: [],
		collapse: true,
		menu: [],
		currentRoute: { name: '', path: '' },
	}),
	getters: {},
	mutations: {
		saveTagsData(state, data) {
			state.tagsStoreList = data;
		},
		saveCollapse(state, data) {
			state.collapse = data;
		},
		savePermission(state, data) {
			state.menu = data;
		},
		saveCurrentRoute(state, data) {
			state.currentRoute = data;
		},
	},
	actions: {},
};
