using ApiModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WebUtils;
using WebUtils.HttpContextUser;
using SqlSugar.Extensions;
using WebService.IService;
using WebModel.Entitys;
using WebUtils.BaseService;

namespace WebExtention.Authentication
{
    /// <summary>
    /// 权限校验插件
    /// </summary>
    public class PermissionHandler: AuthorizationHandler<AuthorizationModel>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IBaseService<RolePermission> _rolePermissionService;
        private readonly IHttpContextAccessor _accessor;
        private readonly ISysUserService _userServices;
        private readonly IUser _user;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="rolePermissionService"></param>
        /// <param name="accessor"></param>
        /// <param name="userServices"></param>
        /// <param name="user"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes, IBaseService<RolePermission> rolePermissionService, IHttpContextAccessor accessor, ISysUserService userServices, IUser user)
        {
            _accessor = accessor;
            _userServices = userServices;
            _user = user;
            Schemes = schemes;
            _rolePermissionService = rolePermissionService;
        }

        /// <summary>
        /// 权限校验处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationModel requirement)
        {
            var httpContext = _accessor.HttpContext;

            #region 获取所有角色的接口权限列表
            if (!requirement.Permissions.Any())
            {
                requirement.Permissions = new List<PermissionItem>();
            }
            #endregion

            #region 权限验证
            if (httpContext.IsNotEmpty())
            {
                var questUrl = httpContext.Request.Path.Value.ToLower();
                // 整体结构类似认证中间件UseAuthentication的逻辑，具体查看开源地址
                // https://github.com/dotnet/aspnetcore/blob/master/src/Security/Authentication/Core/src/AuthenticationMiddleware.cs
                httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = httpContext.Request.Path,
                    OriginalPathBase = httpContext.Request.PathBase
                });
                // Give any IAuthenticationRequestHandler schemes a chance to handle the request
                // 主要作用是: 判断当前是否需要进行远程验证，如果是就进行远程验证
                var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        context.Fail();
                        return;
                    }
                }

                //判断请求是否拥有凭据，即有没有登录
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                    
                    //result?.Principal不为空即登录成功
                    if (result?.Principal != null)
                    {
                        httpContext.User = result.Principal;

                        // 获取当前用户的角色信息
                        var currentUserRoles = new List<string>();
                        // jwt
                        currentUserRoles = (from item in httpContext.User.Claims
                                            where item.Type == requirement.ClaimType
                                            select item.Value).ToList();
                        //验证权限
                        if (currentUserRoles.Count <= 0)
                        {
                            context.Fail();
                            return;
                        }
                        currentUserRoles = currentUserRoles.FirstOrDefault()?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

                        var isMatchRole = false;
                        var permisssionRoles = requirement.Permissions.Where(w => currentUserRoles.Contains(w.RoleId));
                        foreach (var item in permisssionRoles)
                        {
                            try
                            {
                                if (Regex.Match(questUrl, item.Url?.ObjToString().ToLower())?.Value == questUrl)
                                {
                                    isMatchRole = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }

                        //验证权限
                        if (!isMatchRole)
                        {
                            context.Fail();
                            return;
                        }

                        // 判断token是否过期，过期则重新登录
                        var isExp = false;
                        // jwt
                        isExp = (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now;

                        if (!isExp)
                        {
                            context.Fail(new AuthorizationFailureReason(this, "授权已过期,请重新授权"));
                            return;
                        }

                        //校验签发时间
                        var value = httpContext.User.Claims.SingleOrDefault(s => s.Type == JwtRegisteredClaimNames.Iat)?.Value;
                        if (value != null)
                        {
                            var user = await _userServices.QueryById(_user.ID, true);
                            if (user.LastLoginTime > value.ObjToDate())
                            {
                                var res = new ApiResponse(StatusCode.CODE401, "很抱歉,授权已失效,请重新授权").MessageModel;
                                context.Fail(new AuthorizationFailureReason(this, res.msg));
                                return;
                            }
                        }

                        context.Succeed(requirement);
                        return;
                    }
                }

                //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
                if (!(questUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) && (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType)))
                {
                    context.Fail();
                    return;
                }
            }
            #endregion
        }
    }
}
