export default {
	namespaced: false,
	state: () => ({
		login: '/api/Login/Token',
		cache: '/api/Common/GetCache',
		authority: { rolePermission: '/api/SysRole/GetRolePermission', saveRoleAuth: '/api/SysRole/SaveRoleAuth' },
		user: { getInfo: '/api/SysUser/GetInfoByToken', getAuth: '/api/SysUser/GetUserAuth' },
		permission: { list: '/api/Permission/GetTree', save: '/api/Permission/Save', saveSort: '/api/Permission/SaveSort', delete: '/api/Permission/DeleteById' },
		dictionary: { list: '/api/Dictionary/GetList', save: '/api/Dictionary/Save', delete: '/api/Dictionary/DeleteById' },
		sysUser: {
			list: '/api/SysUser/GetList',
			save: '/api/SysUser/Save',
			resetpw: '/api/SysUser/ReSetPassword',
			delete: '/api/SysUser/DeleteById',
			saveUserSite: '/api/SysUser/SaveUserSite'
		},
		sysRole: { list: '/api/SysRole/GetList', save: '/api/SysRole/Save', delete: '/api/SysRole/DeleteById' },
		currentSite: '/api/WebSite/GetUserSite',
		area:{ list: '/api/Area/GetList', save: '/api/Area/Save' },
		website: { list: '/api/WebSite/GetList', save: '/api/WebSite/Save', saveDistributor: '/api/WebSite/SaveSiteDistributor' },
		navigation: { list: '/api/Navigation/GetList', save: '/api/Navigation/Save', saveSort: '/api/Navigation/SaveSort' },
		pageHtml: { list: '/api/pageHtml/GetList', save: '/api/pageHtml/Save' },
		product: {
			list: '/api/ProductInfo/GetList',
			props: '/api/ProductInfo/GetProps',
			save: '/api/ProductInfo/Save',
			upload: '/api/ProductInfo/UploadExcel',
			resetProps: '/api/ProductInfo/ResetProps',
			export: '/api/ProductInfo/ExportProduct'
		},
		pdtProps:{list:"/api/ProductProps/GetList",save:'/api/ProductProps/Save'},
		distributors: { list: '/api/Distributors/GetList', save: '/api/Distributors/Save' }
	})
};
