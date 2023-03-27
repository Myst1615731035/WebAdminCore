using BaseRepository;
using WebModel.SystemEntity;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// PermissionRepository
    /// </summary>	
    public partial class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository() : base()
        {
        }
    }
}
