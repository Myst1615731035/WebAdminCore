<template>
	<el-container>
		<el-header class="topbar" style="height: auto;padding: 0;">
			<el-row class="top-header theme">
				<el-col :span="16">
					<i class="fa fa-bars" @click="collapse"></i>
					{{ $store.state.layout.currentRoute.name }}
				</el-col>
				<el-col :span="5"></el-col>
				<el-col :span="2" style="text-align: right;">{{ ($store.state.loginInfo.userInfo || {}).Name || '' }}</el-col>
				<el-col :span="1" style="display: flex; justify-content: center; align-items: center;">
					<el-dropdown>
						<el-avatar :src="'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png'"></el-avatar>
						<template #dropdown>
							<el-dropdown-menu>
								<el-dropdown-item @click="GetCache">刷新缓存</el-dropdown-item>
								<el-dropdown-item divided @click="ToPersonal">个人信息</el-dropdown-item>
								<el-dropdown-item divided @click="ToLogin">退出登录</el-dropdown-item>
							</el-dropdown-menu>
						</template>
					</el-dropdown>
				</el-col>
			</el-row>
			<el-row class="top-tags">
				<el-col :span="12" style="text-align: left;">
					<!-- 左侧标签栏 -->
					<el-tag v-for="tag in tags" :key="tag.name" closable :type="tag.type">{{ tag.name }}</el-tag>
				</el-col>
				<el-col :span="12" style="text-align: right;">
					<!-- 右侧标签栏 -->
					<vxe-button size="mini" content="任务栏" @click="drawerConfig.show = true"></vxe-button>
				</el-col>
			</el-row>
		</el-header>
		<el-main><router-view class="main"></router-view></el-main>
		<el-drawer ref="drawer" v-model="drawerConfig.show" v-bind="drawerConfig" style="text-align: left;">
			<template #header>
				<h3>任务栏</h3>
			</template>
			<template #default>
				<div v-if="false"></div>
			</template>
		</el-drawer>
	</el-container>
</template>

<script>
export default {
	name: 'container',
	data() {
		return {
			siteUpload: { show: false, type: '', props: { multiple: true, width: window.innerWidth * 0.3, height: window.innerHeight * 0.8, title: '' } },
			tags: [],
			drawerConfig: { show: false, title: '任务栏', direction: 'rtl', modal: false, size: '20%' }
		};
	},
	methods: {
		collapse() {
			this.$store.commit('layout/saveCollapse', !this.$store.state.layout.collapse);
		},
		ToPersonal() {
			this.$router.replace({ path: '/personal' });
		},
		ImageUploadClick() {
			this.siteUpload.type = 'image';
			this.siteUpload.props.title = '上传图片';
			this.siteUpload.show = true;
		},
		FileUploadClick() {
			this.siteUpload.type = 'file';
			this.siteUpload.props.title = '上传文件';
			this.siteUpload.show = true;
		},
		async ResetProductProps() {
			this.$post(this.serverApi.product.resetProps).then(res => {
				this.$alertRes(res);
			});
		}
	}
};
</script>

<style>
.main {
	height: 100%;
	padding: 0.3125rem;
	padding-top: 0;
}
.top-header {
	height: 3.125rem;
	line-height: 3.125rem;
	text-align: left;
	padding: 0 1.25rem;
}
.top-tags {
	padding: 0 1.25rem;
	padding-top: 0.25rem;
	padding-bottom: 0.25rem;
	border-bottom: 1px solid #eee;
}
.el-tag {
	height: 1.75rem !important;
}
</style>
