using BaseRepository;
using BaseService;
using SqlSugar;
using WebModel.Entitys;
using WebService.ISystemService;

namespace WebService.SystemService
{
    /// <summary>
	/// ButtonService
	/// </summary>
    public partial class ButtonService : BaseService<Button>, IButtonService
    {
        IBaseRepository<Button> dal;
        ISqlSugarClient db;
        public ButtonService(IBaseRepository<Button> _dal)
        {
            dal = _dal;
            db = dal.Db;
        }
    }
}