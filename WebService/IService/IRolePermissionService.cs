using ApiModel;
using WebUtils.BaseService;
using WebModel.Entitys;

namespace WebService.IService
{
    /// <summary>
    /// IRoleMenuButtonServices
    /// </summary>	
    public partial interface IRolePermissionService : IBaseService<RolePermission>
    {
        Task<List<PermissionItem>> GetRolePermissions();
        Task<List<Menu>> GetRoleAuthTree(string id);
        Task<List<string>> GetRoleAuthLeafChecked(string roleId);
    }
}
