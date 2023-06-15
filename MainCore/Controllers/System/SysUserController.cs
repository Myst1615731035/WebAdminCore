using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils;
using WebUtils.HttpContextUser;
using SqlSugar.Extensions;
using WebModel.Entitys;
using WebService.IService;

namespace MainCore.Controllers
{
    /// <summary>
    /// SysController
    /// </summary>	
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SysUserController : ControllerBase
    {
        #region 接口构造
        private readonly ISysUserService _service;
        private readonly IUser _user;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public SysUserController(ISysUserService service, IUser user)
        {
            _service = service;
            _user = user;
        }
        #endregion

        #region 用户信息
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ContentJson> GetInfoByToken([FromBody] dynamic param)
        {
            var res = new ContentJson("用户信息获取失败");
            string token = param.token;
            if (token.IsNotEmpty())
            {
                var model = JwtHelper.SerializeJwt(token);
                if (model.IsNotEmpty() && model.Uid.IsNotEmpty())
                {
                    string userId = model.Uid.ObjToString();
                    var user = await _service.Db.Queryable<SysUser>().Where(u => u.Id == userId && !u.IsDelete)
                                    .Select(u => new
                                    {
                                        UserId = u.Id,
                                        u.Account,
                                        u.Name,
                                        u.Avatar,
                                        u.Sex,
                                        u.Email,
                                        u.Remark,
                                        RoleNames = SqlFunc.Subqueryable<SysRole>()
                                                    .Where(r => SqlFunc.JsonArrayAny(u.RoleIds, r.Id) && !r.IsDelete).ToList(r => r.Name)
                                    }).FirstAsync();
                    if (user.IsNotEmpty()) res = new ContentJson(true, "获取成功", user);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取用户权限数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ContentJson> GetUserAuth()
        {
            var res = new ContentJson("用户权限获取失败");
            if (_user.ID.IsNotEmpty())
            {
                res = new ContentJson()
                {
                    success = true,
                    msg = "获取成功",
                    data = await _service.GetUserAuth(_user.ID)
                };
            }
            return res;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetList([FromBody] Pagination page)
        {
            var exp = Expressionable.Create<SysUser>();
            // 增加查询条件
            if (page.keyword.IsNotEmpty()) exp = exp.And(t => t.Name.Contains(page.keyword) || t.Account.Contains(page.keyword) || t.Email.Contains(page.keyword));
            return new ContentJson(true, "获取成功", await _service.QueryPage(page, exp.ToExpression()));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetEntity([FromBody] object id)
        {
            var exp = Expressionable.Create<SysUser>();
            // 增加查询条件

            var entity = await _service.QueryById(id);
            return new ContentJson()
            {
                msg = "success",
                success = true,
                data = entity
            };
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save([FromBody] SysUser entity)
        {
            var res = new ContentJson("保存失败");
            // 新增
            if (entity.Id.IsEmpty()) entity.Password = MD5Helper.MD5Encrypt32(entity.Password);
            else
            {
                // 更新数据
                // 获取原用户信息
                var user = await _service.QueryById(entity.Id);
                // 处理新密码
                if (entity.NewPassword.IsNotEmpty())
                {
                    var password = MD5Helper.MD5Encrypt32(entity.Password);
                    if (!await _service.Any(t => t.Id == entity.Id && t.Password == password))
                    {
                        res.msg = "原始密码验证错误，请确认";
                        return res;
                    }
                    // 处理其他数据
                    entity.Password = MD5Helper.MD5Encrypt32(entity.NewPassword);
                }
                entity = user.UpdateProp(entity);
            }

            entity.RoleIds = entity.RoleIds ?? new List<string>();
            if (await _service.Storageable(entity) > 0)
            {
                res = new ContentJson(true, "保存成功", entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null);
            }
            return res;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ContentJson> ReSetPassword(string Id)
        {
            var res = new ContentJson("无效用户");
            var user = await _service.QueryById(Id);
            if (user.IsNotEmpty() && user.Id.IsNotEmpty())
            {
                var newPw = RandomHelper.GetRandomString(12);
                user.Password = MD5Helper.MD5Encrypt32(newPw);
                if (await _service.Update(user)) res = new ContentJson(true, "重置成功", newPw);
                else res.msg = "重置失败";
            }
            return res;
        }
        
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromQuery] string Id)
        {
            var res = new ContentJson("删除失败");
            if (await _service.DeleteById(Id)) res = new ContentJson(true, "删除成功");
            return res;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> DeleteById([FromBody] List<string> Ids)
        {
            var res = new ContentJson("删除失败");
            if (await _service.Delete(t => Ids.Contains(t.Id)) > 0) res = new ContentJson(true, "删除成功");
            return res;
        }
        #endregion

        #region 业务功能
        /// <summary>
        /// 保存用户站点权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="siteIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> SaveUserSite([FromQuery] string userId, [FromBody] List<string>? siteIds)
        {
            var res = new ContentJson("保存失败");
            var user = await _service.QueryById(userId);
            if (user.IsEmpty()) res.msg = $"{res.msg}, 未获取到需要设置的用户信息";
            else
            {
                user.SiteIds = siteIds;
                if (await _service.Update(user)) res = new ContentJson(true, "保存成功", user);
            }
            return res;
        }
        #endregion
    }
}
