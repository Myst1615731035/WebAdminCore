export default {
	namespaced: false,
	state: () => ({
		IsDelete: [{ value: false, label: '激活' }, { value: true, label: '关闭' }],
		IsDeleteSwitch: { openValue: false, openLabel: '激活', closeValue: true, closeLabel: '关闭' },
		storage:{}
	}),
	mutations: {
		saveCaches(state, data) {
			state.storage = data;
			window.localStorage.setItem('Storage', JSON.stringify(data));
		}
	}
};
