export default {
	namespaced: false,
	state: () => ({
		login: '/api/Login/Token',
		cache: '/api/Common/GetCache',
		user: { getInfo: '/api/SysUser/GetInfoByToken', getAuth: '/api/SysUser/GetUserAuth' },
		permission: { list: '/api/Permission/GetTree', save: '/api/Permission/Save', saveBtn: '/api/Permission/SaveBtn', saveSort: '/api/Permission/SaveSort' },
		authority: { rolePermission: '/api/SysRole/GetRolePermission', saveRoleAuth: '/api/SysRole/SaveRoleAuth' },
		dictionary: { list: '/api/Dictionary/GetList', itemList: '/api/Dictionary/GetItemList', save: '/api/Dictionary/Save' },
		sysUser: { list: '/api/SysUser/GetList', save: '/api/SysUser/Save', resetpw: '/api/SysUser/ReSetPassword', saveUserSite: '/api/SysUser/SaveUserSite' },
		sysRole: { list: '/api/SysRole/GetList', save: '/api/SysRole/Save' },
		instock: { list: '/api/Instock/GetList', save: '/api/Instock/Save' }
	})
};
