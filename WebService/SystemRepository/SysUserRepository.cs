using BaseRepository;
using WebModel.SystemEntity;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
    /// SysRepository
    /// </summary>	
    public partial class SysUserRepository : BaseRepository<SysUser>, ISysUserRepository
    {
        public SysUserRepository() : base()
        {
        }
    }
}
