export default {
	namespaced: false,
	state: () => ({
		login: '/api/Login/Token',
		cache: '/api/Common/GetCache',
		authority: { rolePermission: '/api/SysRole/GetRolePermission', saveRoleAuth: '/api/SysRole/SaveRoleAuth' },
		user: { getInfo: '/api/SysUser/GetInfoByToken', getAuth: '/api/SysUser/GetUserAuth' },
		permission: { list: '/api/Permission/GetTree', save: '/api/Permission/Save', saveSort: '/api/Permission/SaveSort', delete: '/api/Permission/Delete' },
		dictionary: { list: '/api/Dictionary/GetList', save: '/api/Dictionary/Save', delete: '/api/Dictionary/Delete' },
		sysUser: { list: '/api/SysUser/GetList', save: '/api/SysUser/Save', resetpw: '/api/SysUser/ReSetPassword', delete: '/api/SysUser/Delete' },
		sysRole: { list: '/api/SysRole/GetList', save: '/api/SysRole/Save', delete: '/api/SysRole/Delete' },
		instock: { list: '/api/Instock/GetList', save: '/api/Instock/Save' },
		outstock: { list: '/api/Outstock/GetList', save: '/api/Outstock/Save' },
		product: { list: '/api/product/GetList', save: '/api/product/Save' },
		productClass: { list: '/api/productClass/GetList', save: '/api/productClass/Save' },
		warehouse: { list: '/api/warehouse/GetList', save: '/api/warehouse/Save' },
		gene: { list: '/api/gene/GetList', save: '/api/gene/Save' },
	})
};
