using ApiModel;
using Microsoft.AspNetCore.Mvc;
using WebUtils;
using Microsoft.AspNetCore.Authorization;
using WebService.ISystemService;

namespace MainCore.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        #region 注入
        private ISysUserService services;
        public LoginController(ISysUserService _services)
        {
            services = _services;
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
            var tokenModel = await services.GetUserInfoToken(account, password);
            if (tokenModel.IsNotEmpty() && tokenModel.Uid.IsNotEmpty())
            {
                if (tokenModel.Role.IsEmpty()) res.msg += "; 用户角色有误，请检查";
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
