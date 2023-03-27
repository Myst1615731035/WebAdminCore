// 此路由作为系统初始化时的路由数据，在不存在路由数据的情况下使用，用于在页面上录入路由数据
export default {
	defaultRoute: [
		{ Name: '首页', Path: '/', Children: [] },
		{
			Name: '系统管理',
			Children: [
				{ Name: '字典管理', Path: '/Dictionary/index', Children: [] },
				{ Name: '菜单管理', Path: '/Permission/index', Children: [] },
				{ Name: '系统角色', Path: '/Role/index', Children: [] },
				{ Name: '角色授权', Path: '/Auth/index', Children: [] },
				{ Name: '用户管理', Path: '/SysUser/index', Children: [] }
			]
		}
	]
};
