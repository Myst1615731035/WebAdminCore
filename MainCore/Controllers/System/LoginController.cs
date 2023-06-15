using ApiModel;
using Microsoft.AspNetCore.Mvc;
using WebUtils;
using Microsoft.AspNetCore.Authorization;
using WebService.IService;
using SqlSugar.Extensions;
using WebModel.Entitys;
using WebUtils.HttpContextUser;
using SqlSugar;

namespace MainCore.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        #region 注入
        private ISysUserService _service;
        private IUser _user;
        public LoginController(ISysUserService service, IUser user)
        {
            _service = service;
            _user = user;
        }
        #endregion

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Token([FromBody] dynamic data)
        {
            var res = new ContentJson("登录失败");
            string account = data.account, password = data.password;
            password = MD5Helper.MD5Encrypt32(password);
            var tokenModel = await _service.Db.Queryable<SysUser>()
                            .Where(u => u.Account == account && u.Password == password && !u.IsDelete)
                            .Select(u => new TokenModelJwt()
                            {
                                Uid = u.Id,
                                Name = u.Name,
                                Work = "",
                                Roles = SqlFunc.Subqueryable<SysRole>().Where(r=> SqlFunc.JsonArrayAny(u.RoleIds, r.Id) && !r.IsDelete).ToList(r => r.Id)
                            }).FirstAsync();
            if (tokenModel.IsNotEmpty() && tokenModel.Uid.IsNotEmpty())
            {
                if (tokenModel.Roles.IsEmpty() || tokenModel.Roles.Count == 0) res.msg += "; 用户角色有误，请检查";
                else res = new ContentJson()
                {
                    success = true,
                    msg = "登录成功",
                    data = new
                    {
                        token = JwtHelper.IssueJwt(tokenModel),
                        expire = AppConfig.Get("Program", "ExpiredTime").ObjToInt(),
                    }
                };
            }
            else res.msg += "; 账户或密码不正确";
            return res;
        }
        #endregion
    }
}
