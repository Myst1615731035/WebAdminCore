import workApi from '../../../../pages/workApi';
export default {
	namespaced: false,
	state: () => {
		var sysApi = {
			login: '/api/Login/Token',
			cache: '/api/Common/GetCache',
			authority: { rolePermission: '/api/SysRole/GetRolePermission', saveRoleAuth: '/api/SysRole/SaveRoleAuth' },
			user: { getInfo: '/api/SysUser/GetInfoByToken', getAuth: '/api/SysUser/GetUserAuth' },
			permission: { list: '/api/Permission/GetTree', save: '/api/Permission/Save', saveSort: '/api/Permission/SaveSort', delete: '/api/Permission/Delete' },
			dictionary: { list: '/api/Dictionary/GetList', save: '/api/Dictionary/Save', delete: '/api/Dictionary/Delete' },
			sysUser: {
				list: '/api/SysUser/GetList',
				save: '/api/SysUser/Save',
				resetpw: '/api/SysUser/ReSetPassword',
				delete: '/api/SysUser/Delete',
				saveUserSite: '/api/SysUser/SaveUserSite',
			},
			sysRole: { list: '/api/SysRole/GetList', save: '/api/SysRole/Save', delete: '/api/SysRole/Delete' },
		};
		if (!!workApi) return Object.assign(sysApi, workApi.apiList || {});
		else return sysApi;
	},
};
