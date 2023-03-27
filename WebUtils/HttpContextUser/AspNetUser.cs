using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar.Extensions;
using WebUtils;

namespace WebUtils.HttpContextUser
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly HttpContext _context;

        #region 构造方法
        public AspNetUser(HttpContext context)
        {
            _context = context;
        }

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _context = accessor.HttpContext;
        }
        #endregion

        public string Name => GetUserInfoByToken("name").FirstOrDefault().ObjToString();
        public string ID => GetUserInfoByToken("jti").FirstOrDefault().ObjToString();

        #region 辅助方法
        public List<string> GetUserInfoByToken(string ClaimType)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = GetToken();
            // token校验
            if (token.IsNotEmpty() && jwtHandler.CanReadToken(token) && JwtHelper.VerifyToken(token))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);
                return (from item in jwtToken.Claims
                        where item.Type == ClaimType
                        select item.Value).ToList();
            }
            return new List<string>() { };
        }
        public string GetToken()
        {
            return _context?.Request?.Headers?.Authorization.ObjToString().Replace("Bearer ", "");
        }
        public bool IsAuthenticated()
        {
            return _context.User.Identity.IsAuthenticated;
        }
        #endregion
    }
}
