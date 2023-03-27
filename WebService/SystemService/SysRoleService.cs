using BaseRepository;
using BaseService;
using SqlSugar;
using WebModel.SystemEntity;
using WebService.ISystemService;

namespace WebService.SystemService
{
    /// <summary>
    /// SysUserServices
    /// </summary>	
    public partial class SysRoleService : BaseService<SysRole>, ISysRoleService
    {
        IBaseRepository<SysRole> dal;
        ISqlSugarClient db;
        public SysRoleService(IBaseRepository<SysRole> _dal)
        {
            dal = _dal;
            db = dal.Db;
        }
    }
}
