using BaseRepository;
using WebModel.Entitys;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// RolePermissionButtonRepository
    /// </summary>	
    public partial class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository() : base()
        {
        }
    }
}
