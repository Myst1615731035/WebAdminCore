<template>
	<div class="container wrapper">
		<h1 class="welcome"><span>Welcome</span></h1>
		<el-form ref="form" :model="data" :rules="rules" label-position="left" label-width="0px" class="demo-ruleForm login-container">
			<h3 class="title">Sign In</h3>
			<el-form-item prop="account"><el-input type="text" v-model="data.account" auto-complete="off" placeholder="Account"></el-input></el-form-item>
			<el-form-item prop="pass"><el-input v-model="data.password" auto-complete="off" show-password placeholder="Password"></el-input></el-form-item>
			<el-form-item style="width:100%;">
				<el-button type="primary" style="width:100%;" @click.native.prevent="submit" @keydown.enter="submit" :loading="logining">{{ loginStr }}</el-button>
			</el-form-item>
		</el-form>
	</div>
</template>

<script>
import { h, toRaw } from 'vue';
let self;
export default {
	created() {
		self = this;
		window.localStorage.clear();
		document.onkeydown = () => {
			if (window.event.keyCode === 13) self.submit();
		};
	},
	data() {
		return {
			logining: false,
			loginStr: '登录',
			data: { account: 'admin', password: '123456' },
			rules: {
				account: [{ required: true, message: 'Please enter account', trigger: 'blur' }],
				password: [{ required: true, message: 'Please enter password', trigger: 'blur' }]
			}
		};
	},
	methods: {
		async submit() {
			const valid = await self.$refs.form.validate(valid => valid);
			if (valid) {
				self.logining = true;
				self.loginStr = '登录...';
				self.$post(self.$store.state.serverApi.login, self.data)
					.then(res => {
						if (res.success) {
							//登录成功
							self.logining = false;
							self.loginStr = '登录成功';
							self.$Notic({ title: 'Success', message: h('i', { style: 'color: teal' }, 'Welcome'), type: 'success', duration: 5000 });
							//提交数据仓库
							self.$store.commit('saveToken', res.data.token);
							self.$store.commit('saveTokenExpire', new Date(new Date().setMinutes(new Date().getMinutes() + res.data.expire)));
							// 获取用户信息
							self.GetUserInfo().then(res => {
								// 跳转到首页
								if (res.success)
									self.GetUserAuth().then(res => {
										if (!!res && res.success) this.$router.replace(((res.data || [])[0] || {}).Path || '');
									});
							});
							self.GetCache(); // 获取缓存
						} else {
							this.reset();
							this.$message({ content: res.msg, status: 'error' });
						}
					})
					.catch(ex => this.reset());
			} else this.reset();
		},
		reset() {
			this.logining = false;
			this.loginStr = '登录';
		}
	}
};
</script>

<style>
.login-container {
	-webkit-border-radius: 5px;
	border-radius: 5px;
	-moz-border-radius: 5px;
	background-clip: padding-box;
	margin: auto;
	width: 350px;
	padding: 35px 35px 15px 35px;
	background: #fff;
	border: 1px solid #eaeaea;
	box-shadow: 0 0 25px #cac6c6;
	z-index: 1;
	position: relative;
}

.login-container .title {
	margin: 0px auto 40px auto;
	text-align: center;
	color: #505458;
}

.login-container .remember {
	float: right;
	margin: 0px 0px 25px 0px;
}

.wrapper {
	background: #50a3a2;
	background: -webkit-linear-gradient(top left, #50a3a2 0%, #53e3a6 100%);
	background: linear-gradient(to bottom right, #127c7b 0, #50a3a2);
	opacity: 0.8;
	position: absolute;
	left: 0;
	width: 100%;
	height: 100%;
	overflow: hidden;
}

.welcome {
	height: 20%;
	text-align: center;
	font-size: 240%;
	vertical-align: middle;
}

.welcome span {
	position: relative;
	top: calc(50% - 30px);
	height: 60px;
	display: inline-block;
}
</style>
