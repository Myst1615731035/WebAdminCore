using BaseRepository;
using BaseService;
using SqlSugar;
using WebModel.SystemEntity;
using WebService.ISystemService;

namespace WebService.SystemService
{
    /// <summary>
    /// DictItemService
    /// </summary>	
    public partial class DictItemService : BaseService<DictItem>, IDictItemService
    {
        IBaseRepository<DictItem> dal;
        ISqlSugarClient db;
        public DictItemService(IBaseRepository<DictItem> _dal)
        {
            dal = _dal;
            db = dal.Db;
        }
    }
}
