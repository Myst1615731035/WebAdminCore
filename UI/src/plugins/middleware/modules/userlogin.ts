import $store from '../../vue-plugins/store/store';
import $router from '../../vue-plugins/router/router.ts';
import request from './../../utils/modules/request';
import defaultRoute from '../../vue-plugins/router/defaultRoute';
import { modal, saveFile } from 'vxe-table';
const { message: $message } = modal;
const { $post } = request;

// 获取用户信息
const GetUserInfo = () => {
	return $post($store.state.serverApi.user.getInfo, { token: $store.state.loginInfo.token }).then((res) => {
		if (res.success) $store.commit('saveUserInfo', res.data);
		else $message({ content: res.msg, status: 'error' });
		return res;
	});
};
// 获取用户权限
const GetUserAuth = () => {
	return $post($store.state.serverApi.user.getAuth).then((res) => {
		if (res.success) {
			if (res.data == null || res.data == undefined || res.data.length == 0) res.data = defaultRoute;
			$router.filterRouter(res.data);
			$store.commit('savePermission', res.data);
		} else $message({ content: res.msg, status: 'error' });
		return res;
	});
};

const GetCache = () => {
	return $post($store.state.serverApi.cache).then((res) => {
		if (res.success) {
			if (!!$store.state.cache.storage) $message({ content: '缓存已刷新', status: 'success' });
			$store.commit('saveCaches', res.data);
		}
	});
};
// 添加缓存刷新功能
export default {
	install: (app) => {
		// 判断登录状态
		if (IsNotEmpty($store.getters.get('loginInfo', 'token'))) {
			// 获取本地菜单缓存
			var permission = $store.getters.get('layout', 'menu');
			if (IsNotEmpty(permission)) $router.filterRouter(permission);
			// 本地菜单缓存失效;从服务器获取权限菜单
			else GetUserInfo();
		}
		// 登录状态失效则跳转到登录页面
		else $router.push('/login');
		// 用户信息获取方法
		app.config.globalProperties.GetUserInfo = GetUserInfo;
		app.config.globalProperties.GetUserAuth = GetUserAuth;
		app.config.globalProperties.GetCache = GetCache;
	},
};
