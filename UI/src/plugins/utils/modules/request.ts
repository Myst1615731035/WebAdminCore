import _config from '../../../../public/Settings.js';
import axios from 'axios';
import router from '../../vue-plugins/router/router.ts';
import defaultRoute from '../../vue-plugins/router/defaultRoute';
import $store from '../../vue-plugins/store/store.ts';
import { modal, saveFile } from 'vxe-table';
const { message: $message } = modal;
const loginInfo = $store.getters.get('loginInfo');

// 引入element-ui loading组件
const Loading = ElLoading;
// 根据当前运行环境，判断api请求路径
let base = process.env.NODE_ENV == 'production' ? `${TrimEnd(_config.host, '/')}/${TrimEnd(TrimStart(_config.absolutPath, '/'), '/')}` : '';

axios.defaults.timeout = 400000;
let loadingInstance;
const loadingOption = { text: '加载中...', lock: true, spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)', customClass: 'custom-loading' };
axios.interceptors.request.use(
	(config) => {
		config.url = formatUrl(config.url);
		// 处理参数为空时，axios不传Content-type的问题
		if (config.method.toLowerCase() == 'post' && IsEmpty(config.data)) config.data = {};
		// 显示数据加载中
		if (config.loading) loadingInstance = Loading.service(loadingOption);

		var curTime = new Date();
		var expiretime = new Date(Date.parse(loginInfo.tokenExpire));
		if (loginInfo.token && curTime < expiretime && loginInfo.tokenExpire) config.headers.Authorization = 'Bearer ' + loginInfo.token;
		return config;
	},
	(err) => {
		return Promise.reject(err);
	}
);

// http response 拦截器
axios.interceptors.response.use(
	(response) => {
		if (IsNotEmpty(loadingInstance)) loadingInstance.close();
		return response;
	},
	(error) => {
		if (IsNotEmpty(loadingInstance)) loadingInstance.close();
		let errInfo = { success: false, message: 'Error' };
		// 超时请求处理
		var originalRequest = error.config;
		if (error.code == 'ECONNABORTED' && error.message.indexOf('timeout') != -1 && !originalRequest._retry) {
			errInfo.message = 'Request timed out！';
			originalRequest._retry = true;
		} else if (error.response) {
			var err = error.response.data;
			// 错误处理
			switch (`${err.status}`) {
				case '401':
					var token = $store.getters.get('loginInfo', 'token');
					var expireTime = new Date($store.getters.get('loginInfo', 'tokenExpire'));

					// 在用户操作的活跃期内
					if (new Date() <= expireTime) return refreshToken({ token: token }).then((res) => res);
					else {
						// 返回 401，并且不知用户操作活跃期内 清除token信息并跳转到登录页面
						errInfo.message = 'The session has expired, please login again';
						ToLogin();
					}
					break;
				case '403':
				case '404':
				case '405':
				case '415':
				case '429':
				default:
					//其他错误参数
					errInfo.message = 'Fail！Request error:' + error.response.status;
					break;
				case '500':
					// 500 服务器异常
					errInfo.message = err.msg;
					break;
			}
		} else errInfo.message = 'Fail！Server disconnected';

		$message({ content: errInfo.message, status: 'error' });
		return errInfo; // 返回接口返回的错误信息
	}
);

// 保存刷新时间
const saveRefreshtime = (params) => {};

// 刷新Token
const refreshToken = (params) => {
	return axios
		.get(`${base}/api/login/RefreshToken`, { params: params })
		.then((res) => {
			debugger;
			if (res.success) {
				$message({ content: 'refreshToken success! loading data...', status: 'success' });
				$store.commit('saveToken', res.data.token);
				
				var curTime = new Date();
				var expiredate = new Date(curTime.setSeconds(curTime.getSeconds() + res.data.expires_in));
				$store.commit('saveTokenExpire', expiredate);

				error.config.__isRetryRequest = true;
				error.config.headers.Authorization = 'Bearer ' + res.response.token;
				return axios(error.config);
			} else {
				// 刷新tokenFail 清除token信息并跳转到登录页面
				errInfo.message = 'The session has expired, please log in again';
				ToLogin();
			}
			return res.data;
		})
		.catch(errAlert);
};

const errAlert = (ex) => {
	var msg = ex.response.data.msg || '';
	if (!!msg) $message({ content: 'Request Error!', status: 'error' });
	return;
};

const BaseApiUrl = base;

// 登录过期,跳转/login
const ToLogin = (params) => {
	router.replace({ path: '/login' });
	// if (global.IS_IDS4) {
	// 	applicationUserManager.login();
	// } else {
	// 	router.replace({ path: '/login' });
	// 	// router.replace({ path: '/login', query: { redirect: router.currentRoute.fullPath } });
	// }
};

//通用的请求方式
const formatUrl = (url) => {
	var res = '';
	if (/(^((?!\/\/|http:|https:|localhost|\.com(\/?)|\.org(\/?)|\.net(\/?)|\.gov(\/?)|\.info(\/?)).)*$)/gi.test(url))
		res = Trim(base, '/') != '' ? `${TrimEnd(base, '/')}/${TrimStart(url, '/')}` : url;
	else res = url;
	return res;
};
const getFile = (url, fileName, fileType) => {
	fetch(formatUrl(url))
		.then((res) => res.blob())
		.then((blob) => saveFile({ filename: fileName, type: Trim(fileType, '.'), content: blob }))
		.catch(errAlert);
};
const reqRes = (res) => res.data || {};
const putFile = (url, params) => {
	return axios
		.post(url, params, { headers: { 'Content-Type': 'multipart/form-data' }, timeout: null })
		.then(reqRes)
		.catch(errAlert);
};

const get = (url, params, config = {}) => {
	return axios.get(url, params, config).then(reqRes).catch(errAlert);
};

const post = (url, params, config = {}) => {
	return axios.post(url, params, config).then(reqRes).catch(errAlert);
};

const form = (url, params, config = {}) => {
	config = Object.assign({ headers: { 'Content-Type': 'multipart/form-data' }, timeout: 30000 }, config);
	return axios.post(url, params, config).then(reqRes).catch(errAlert);
};

//通用的分页请求方式,分页请求也可以使用，返回的数据量在后台接口处控制
const pageRes = (res) => res.data.data || {};
const getPage = (url, params, config = {}) => {
	return axios.get(url, params).then(pageRes).catch(errAlert);
};

const postPage = (url, params) => {
	return axios.post(url, params).then(pageRes).catch(errAlert);
};

// 节流请求fetch, 用于联想搜索
let fetchGetController;
const fetchGet = async (url, params) => {
	fetchGetController && fetchGetController.abort();
	fetchGetController = new AbortController();
	try {
		return await fetch(formatUrl(url), { method: 'get', body: params, signal: fetchGetController.signal }).then((res) => res.json());
	} catch (e) {
		console.log(e);
	}
};
let fetchPostController;
const fetchPost = async (url, params) => {
	fetchPostController && fetchPostController.abort();
	fetchPostController = new AbortController();
	try {
		return await fetch(formatUrl(url), { method: 'post', body: params, signal: fetchPostController.signal }).then((res) => res.json());
	} catch (e) {
		console.log(e);
	}
};

// 获取用户信息
const GetUserInfo = () => {
	return post($store.state.serverApi.user.getInfo, { token: $store.state.loginInfo.token }).then((res) => {
		if (res.success) $store.commit('saveUserInfo', res.data);
		else $message({ content: res.msg, status: 'error' });
		return res;
	});
};
// 获取用户权限
const GetUserAuth = () => {
	return post($store.state.serverApi.user.getAuth).then((res) => {
		if (res.success) {
			if (res.data == null || res.data == undefined || res.data.length == 0) res.data = defaultRoute;

			router.filterRouter(res.data);
			$store.commit('savePermission', res.data);
		} else $message({ content: res.msg, status: 'error' });
		return res;
	});
};

const GetCache = () => {
	return post($store.state.serverApi.cache).then((res) => {
		if (res.success) {
			if (!!$store.state.cache.storage) $message({ content: '缓存已刷新', status: 'success' });
			$store.commit('saveCaches', res.data);
		}
	});
};

const install = (app) => {
	// 通用方法
	app.config.globalProperties.baseUrl = BaseApiUrl;
	app.config.globalProperties.$formatUrl = formatUrl;
	app.config.globalProperties.$get = get;
	app.config.globalProperties.$post = post;
	app.config.globalProperties.$getPage = getPage;
	app.config.globalProperties.$postPage = postPage;
	app.config.globalProperties.$form = form;
	app.config.globalProperties.$putFile = putFile;
	app.config.globalProperties.$getFile = getFile;
	app.config.globalProperties.$fetchGet = fetchGet;
	app.config.globalProperties.$fetchPost = fetchPost;

	// 用户信息获取方法
	app.config.globalProperties.GetUserInfo = GetUserInfo;
	app.config.globalProperties.GetUserAuth = GetUserAuth;
	app.config.globalProperties.GetCache = GetCache;

	// 退出登录
	app.config.globalProperties.ToLogin = ToLogin;
};
export default { install, postPage, GetUserInfo, GetUserAuth, GetCache };
