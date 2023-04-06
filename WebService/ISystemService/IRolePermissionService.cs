using ApiModel;
using BaseService;
using WebModel.Entitys;

namespace WebService.ISystemService
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
