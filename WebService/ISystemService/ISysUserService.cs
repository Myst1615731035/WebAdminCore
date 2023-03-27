using BaseService;
using WebUtils;
using System.Linq.Expressions;
using ApiModel;
using WebModel.SystemEntity;

namespace WebService.ISystemService
{
    /// <summary>
    /// ISysServices
    /// </summary>	
    public partial interface ISysUserService : IBaseService<SysUser>
    {
        #region 用户登录以及用户信息获取
        Task<TokenModelJwt> GetUserInfoToken(string account, string password);

        Task<object> GetUserInfo(string id);

        Task<List<Menu>> GetUserAuth(string id);
        Task<Pagination> GetUserList(Expression<Func<SysUser, bool>> expression, Pagination page);
        Task<bool> SaveUser(SysUser entity);
        #endregion
    }
}
