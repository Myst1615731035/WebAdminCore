export default {
	namespaced: false,
	state: () => ({
		cursite: {
			Id: '',
			Idtag: '',
			TestDomain: '',
			MainDomain: '',
		}
	}),
	mutations: {
		saveSite(state, data) {
			var reload = (IsEmpty(state.cursite.Id) && IsEmpty(state.cursite.Idtag) && IsEmpty(window.localStorage
				.getItem("CurSite")) || (!!data && !!data.Id && state.cursite.Id != data.Id));
			state.cursite = data;
			window.localStorage.setItem('CurSite', JSON.stringify(data));
			if (reload) window.location.reload();
		}
	}
};
