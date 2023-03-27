using BaseRepository;
using WebModel.SystemEntity;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// IDictItemRepository
    /// </summary>	
    public partial class DictItemRepository : BaseRepository<DictItem>, IDictItemRepository
    {
        public DictItemRepository() : base()
        {
        }
    }
}
