import _config from '../../../../public/Settings.js';

import { createRouter, createWebHistory } from 'vue-router';
import $store from '../store/store';

import Home from '../../../views/Home/index.vue';
import Login from '../../../views/Home/login.vue';
import Personal from '../../../views/Home/personal.vue';
import Page404 from '../../../views/Error/404.vue';

// 导入所有页面的路径
const views = import.meta.glob('../../../views/**/*.vue');
const pages = import.meta.glob('../../../pages/**/*.vue');
const modules = Object.assign(views, pages);
// 默认路由
const routes = [
	{ path: '/login', component: Login, name: 'login', iconCls: 'fa-address-card', meta: { title: 'Sign In', notTab: true, notLayout: true } },
	{ path: '/personal', component: Personal, name: '个人信息', hidden: true },
	{ path: '/:pathMatch(.*)', hidden: true, redirect: { path: '/404' } },
	{ path: '/404', component: Page404, name: 'NoPage', meta: { title: 'NoPage', requireAuth: false, NoTabPage: true, notLayout: true }, hidden: true }
];
const router = createRouter({ history: createWebHistory(_config.absolutPath), routes });

// 全局路由公约，在路由跳转之前
router.beforeEach(async (to, from, next) => {
	if (to.path == '/login') next();
	else if (IsNotEmpty(window.localStorage.Token))
		if (to.matched.length > 0) next();
		else next('/404');
	else next('/login');
});

// 全局路由公约，在路由跳转之后
router.afterEach(async (to, from, failure) => {
	if (IsEmpty(failure) && IsNotEmpty(to)) $store.commit('saveCurrentRoute', { name: to.name, path: to.path });
});

// 对获取到的后台数据进行筛选后添加路由
const filterRouter = list => {
	list.filter(t => {
		if (IsNotEmpty(t.Path) && t.Children.length == 0) {
			if (t.Path == '/') router.addRoute({ name: t.Name, path: t.Path, component: Home });
			else {
				try {
					var index = Object.keys(modules).find(f => f.toLocaleLowerCase().indexOf(t.Path.toLocaleLowerCase()) > -1);
					if (!!index) router.addRoute({ name: t.Name, path: t.Path, component: modules[index], params: { buttons: t.Buttons || [] } });
				} catch (error) {
					console.info(error);
				}
			}
		}
		if (t.Children.length > 0) filterRouter(t.Children);
		return true;
	});
};
router.filterRouter = filterRouter;
export default router;
