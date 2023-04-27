using WebUtils.BaseService;
using SqlSugar;
using WebUtils;
using ApiModel;
using System.Linq.Expressions;
using WebModel.Entitys;
using WebService.IService;
using SqlSugar.Extensions;

namespace WebService.Service
{
    /// <summary>
    /// SysUserServices
    /// </summary>	
    public partial class SysUserServices : BaseService<SysUser>, ISysUserService
    {
        #region 获取用户信息
        /// <summary>
        /// 用户登录获取Token信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TokenModelJwt> GetUserInfoToken(string account, string password)
        {
            var query = Db.Queryable<SysUser>()
                            .Where(u => u.Account == account && u.Password == password && !u.IsDelete)
                            .Select(u => new TokenModelJwt()
                            {
                                Uid = u.Id,
                                Name = u.Name,
                                Work = "",
                                Role = SqlFunc.Subqueryable<UserRole>().InnerJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
                                                 .Where((ur, r) => ur.UserId == u.Id && !ur.IsDelete && !r.IsDelete)
                                                 .SelectStringJoin((ur, r) => r.Id, ",")
                            }).MergeTable();

            return await query.FirstAsync();
        }
        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> GetUserInfo(string id)
        {
            var query = Db.Queryable<SysUser>()
                .Where(u => u.Id == id && !u.IsDelete)
                .Select(u => new
                {
                    UserId = u.Id,
                    u.Account,
                    u.Name,
                    u.Avatar,
                    u.Sex,
                    u.Email,
                    u.Remark,
                    RoleNames = SqlFunc.Subqueryable<UserRole>()
                                .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
                                .Where((ur, r) => ur.UserId == u.Id && !ur.IsDelete && !r.IsDelete)
                                .SelectStringJoin((ur, r) => r.Name, ","),
                });
            var obj = (await query.FirstAsync()).ToJson().ToJObject();
            var roles = await Db.Queryable<SysUser, UserRole, SysRole>((u, ur, r) => new JoinQueryInfos(JoinType.Left, u.Id == ur.UserId, JoinType.Left, ur.RoleId == r.Id))
                                .Where((u, ur, r) => u.Id == id)
                                .Select((u, ur, r) => r).ToListAsync();
            return obj;
        }
        /// <summary>
        /// 查询用户权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Menu>> GetUserAuth(string id)
        {
            //找出该用户的所有角色ID
            var roleIds = await Db.Queryable<SysUser, UserRole, SysRole>((u, ur, r) => new JoinQueryInfos(
                            JoinType.Inner, u.Id == ur.UserId && !u.IsDelete,
                            JoinType.Inner, ur.RoleId == r.Id && !r.IsDelete
                        ))
                        .Where((u, ur, r) => u.Id == id && !u.IsDelete && !ur.IsDelete && !r.IsDelete)
                        .Select((u, ur, r) => r.Id).Distinct().ToListAsync();

            //根据用户所有的角色信息查找出已授权的权限Id
            var query = Db.Queryable<RolePermission>().LeftJoin<Menu>((rpm, m) => rpm.PermissionId == m.Id)
                            .Where((rpm, m) => roleIds.Contains(rpm.RoleId) && !m.IsDelete)
                            .Select((rpm, m) => new Menu()
                            {
                                Id = m.Id,
                                Pid = m.Pid,
                                Name = m.Name,
                                Path = m.Path,
                                Type = m.Type,
                                Sort = m.Sort,
                                Description = m.Description,
                                Visiable = m.Visiable,
                                Buttons = SqlFunc.Subqueryable<RolePermission>()
                                                .InnerJoin<Button>((rpb, b) => rpb.PermissionId == b.Id)
                                                .Where((rpb, b) => roleIds.Contains(rpm.RoleId) && b.Mid == m.Id && !b.IsDelete)
                                                .ToList((rpb, b) => new Button()
                                                {
                                                    Id = b.Id,
                                                    Mid = b.Mid,
                                                    Name = b.Name,
                                                    Description = b.Description,
                                                    Sort = b.Sort,
                                                    IsDelete = b.IsDelete,
                                                })
                            }).MergeTable().OrderBy(t => t.Sort);
            return await query.ToTreeAsync(t => t.Children, t => t.Pid, "");
        }
        #endregion
        
        #region 获取用户列表
        public async Task<Pagination> GetUserList(Expression<Func<SysUser, bool>> expression, Pagination page)
        {
            var query = Db.Queryable<SysUser>()
                          .WhereIF(expression != null, expression)
                          .Select(t => new SysUser
                          {
                              Id = t.Id,
                              Name = t.Name,
                              Account = t.Account,
                              Password = t.Password,
                              RoleIdStr = SqlFunc.Subqueryable<UserRole>().Where(ur => ur.UserId == t.Id).SelectStringJoin(ur => ur.RoleId, ","),
                              Sex = t.Sex,
                              Birth = t.Birth,
                              IsDelete = t.IsDelete,
                          });

            if (page.isAll)
            {
                var data = await query.ToListAsync();
                data.ForEach(t => t.RoleIds = t.RoleIdStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                page.response = data;
                page.total = data.Count;
                page.pageCount = 1;
            }
            else
            {
                RefAsync<int> totalCount = 0;
                var data = await query.ToPageListAsync(page.currentPage, page.pageSize, totalCount) ?? new List<SysUser>();
                data.ForEach(t => t.RoleIds = t.RoleIdStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                page.response = data;
                int pageCount = Math.Ceiling(totalCount.ObjToDecimal() / page.pageSize.ObjToDecimal()).ObjToInt();
                page.total = totalCount;
                page.pageCount = pageCount;
            }
            return page;

        }
        #endregion

        #region 保存用户信息
        public async Task<bool> SaveUser(SysUser entity)
        {
            Db.Ado.BeginTran();
            try
            {
                var newUserRoles = new List<UserRole>();
                if (entity.Id.IsNotEmpty())
                {
                    var oldRoles = await Db.Queryable<UserRole>().Where(t => t.UserId == entity.Id).ToListAsync();
                    if (!(entity.RoleIds.All(t => oldRoles.Any(s => s.Equals(t))) && entity.RoleIds.Count == oldRoles.Count))
                        await Db.Deleteable<UserRole>().Where(t => t.UserId == entity.Id).ExecuteCommandAsync();
                }
                entity.RoleIds.ForEach(t =>
                {
                    newUserRoles.Add(new UserRole
                    {
                        UserId = entity.Id,
                        RoleId = t
                    });
                });
                await Db.Insertable(newUserRoles).ExecuteCommandAsync();
                await Storageable(entity);
                Db.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                Db.Ado.RollbackTran();
                LogHelper.LogException(ex);
                return false;
            }

        }
        #endregion
    }
}
