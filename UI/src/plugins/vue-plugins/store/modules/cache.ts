export default {
	namespaced: false,
	state: () => ({
		IsDelete: [{ value: false, label: '激活' }, { value: true, label: '关闭' }],
		TrueSwitch: { openValue: false, openLabel: '√', closeValue: true, closeLabel: 'X' },
		FalseSwitch: { openValue: true, openLabel: '√', closeValue: false, closeLabel: 'X' },
		storage:{}
	}),
	mutations: {
		saveCaches(state, data) {
			state.storage = data;
			window.localStorage.setItem('Storage', JSON.stringify(data));
		}
	}
};
