<template>
	<vxe-select :options="siteList" v-model="siteId" :option-props="optionProp" :visible="siteList.length > 0" placeholder="选择站点"></vxe-select>
</template>

<script>
// 多租户处理
export default {
	data() {
		return { siteId: '', optionProp: { label: 'Name', value: 'Id' }, siteList: [] };
	},
	mounted() {
		this.GetMySite(true);
	},
	watch: {
		siteId(newval, oldval) {
			if (newval != oldval) {
				var site = this.siteList.find((t) => t.Id == newval);
				site = site || { Id: '', Idtag: '', TestDomain: '', MainDomain: '' };
				this.$store.commit('saveSite', site);
			}
		},
	},
	methods: {
		GetMySite() {
			if (!!this.$store.state.loginInfo.token) {
				this.$post(this.serverApi.currentSite).then((res) => {
					this.siteList = Object.freeze(res.data);
					var siteId = this.$store.state.website.cursite.Id;
					if (!!res.data && res.data.length > 0) {
						if (!!!siteId || !!!this.siteList.find((t) => t.Id == siteId)) siteId = res.data[0].Id;
					} else siteId = '';
					this.siteId = siteId;
				});
			}
		},
	},
};
</script>

<style></style>
