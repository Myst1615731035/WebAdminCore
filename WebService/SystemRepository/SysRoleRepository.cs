using BaseRepository;
using WebModel.Entitys;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// SysRepository
    /// </summary>	
    public partial class SysRoleRepository : BaseRepository<SysRole>, ISysRoleRepository
    {
        public SysRoleRepository() : base()
        {
        }
    }
}
