export default {
	namespaced: false,
	state: () => ({
		cursite: {
			Id: '',
			Idtag: '',
			TestDomain: '',
			MainDomain: '',
		},
	}),
	mutations: {
		saveSite(state, data) {
			var notReload = (data.Id == state.cursite.Id && data.Idtag == state.cursite.Idtag && !!state.cursite.Id) || !!!state.cursite.Id;
			state.cursite = data;
			if (!notReload) window.location.reload();
		},
	},
};
