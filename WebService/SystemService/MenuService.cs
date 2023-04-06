using BaseRepository;
using BaseService;
using SqlSugar;
using WebModel.Entitys;
using WebService.ISystemService;

namespace WebService.SystemService
{
    /// <summary>
    /// PermissionServices
    /// </summary>	
    public class MenuService : BaseService<Menu>, IMenuService
    {
        IBaseRepository<Menu> dal;
        ISqlSugarClient db;
        public MenuService(IBaseRepository<Menu> _dal)
        {
            dal = _dal;
            db = dal.Db;
        }
    }
}
