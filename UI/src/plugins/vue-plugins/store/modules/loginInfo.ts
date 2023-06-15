export default {
	namespaced: false,
	state: () => ({
		token: null,
		tokenExpire: null,
		userInfo: null,
		activeTime: null,
	}),
	getters: {},
	mutations: {
		saveToken(state, data) {
			state.token = data;
		},
		saveTokenExpire(state, data) {
			state.tokenExpire = data;
		},
		saveUserInfo(state, data) {
			state.userInfo = data;
		},
		saveActiveTime(state, data) {
			state.activeTime = data;
		},
	},
	actions: {},
};
