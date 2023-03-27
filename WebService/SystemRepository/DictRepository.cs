using BaseRepository;
using WebModel.SystemEntity;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// PermissionRepository
    /// </summary>	
    public partial class DictRepository : BaseRepository<Dict>, IDictRepository
    {
        public DictRepository() : base()
        {
        }
    }
}
