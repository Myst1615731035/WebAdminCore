import $store from '../../vue-plugins/store/store';
import $router from '../../vue-plugins/router/router';
import request from './../../utils/modules/request';
const { GetUserInfo } = request;
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
	},
};
