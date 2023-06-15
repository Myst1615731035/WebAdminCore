using WebUtils.BaseService;
using WebModel.Entitys;

namespace WebService.IService
{
    /// <summary>
    /// ISysServices
    /// </summary>	
    public partial interface ISysUserService : IBaseService<SysUser>
    {
        #region 用户登录以及用户信息获取
        Task<List<Menu>> GetUserAuth(string id);
        #endregion
    }
}
