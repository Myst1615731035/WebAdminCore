import { toRaw }from 'vue';
export default {
	namespaced: false,
	state: () => ({
		token: null,
		tokenExpire: null,
		userInfo: null
	}),
	getters: {},
	mutations: {
		saveToken(state, data) {
			state.token = data;
			window.localStorage.setItem('Token', data);
		},
		saveTokenExpire(state, data) {
			state.tokenExpire = data;
			window.localStorage.setItem('TokenExpire', data);
		},
		saveUserInfo(state, data) {
			state.userInfo = data;
			window.localStorage.setItem('UserInfo', JSON.stringify(data));
		},
	},
	actions: {}
};
