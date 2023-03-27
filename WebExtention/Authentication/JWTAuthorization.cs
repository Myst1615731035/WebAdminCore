using ApiModel;
using Autofac.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebUtils;
using WebUtils.GlobalConfig;

namespace WebExtention.Authentication
{
    /// <summary>
    /// JWT权限校验类
    /// </summary>
    public static class JWTAuthorization
    {
        public static void AddJWTAuthorization(this IServiceCollection service)
        {
            //PS: 默认的授权策略，请在

            #region 基于角色
            // [Authorize(Roles = "Admin,System")]
            #endregion

            #region 手动设置权限策略， 基于策略
            //service.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
            //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
            //    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
            //    options.AddPolicy("A_S_O", policy => policy.RequireRole("Admin", "System", "Others"));
            //});
            #endregion

            #region 自定义复杂的策略授权（由数据库生成权限校验策略）
            var issuer = AppConfig.Get("Program", "Issuer" );
            var audience = AppConfig.Get(new string[] { "Program", "Audience" });
            var expireTime = AppConfig.Get(new string[] { "Program", "ExpiredTime" }).ObjToInt();
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GConfig.JWTSecret)), SecurityAlgorithms.HmacSha256);
            // 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
            var permission = new List<PermissionItem>();

            var permissionRequirement = new AuthorizationModel(ClaimTypes.Role, issuer, audience, signingCredentials, expiration: TimeSpan.FromMinutes(expireTime), "/api/denied", permission);
            
            service.AddAuthorization(options =>
            {
                options.AddPolicy(GConfig.PermissionAuthorizePolicyName, policy => policy.Requirements.Add(permissionRequirement));
            });
            #endregion

            #region 基于第三方的授权 基于Scope策略授权
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Scope_BlogModule_Policy", builder =>
            //    {
            //        //客户端Scope中包含blog.core.api.BlogModule才能访问
            //        // 同时引用nuget包：IdentityServer4.AccessTokenValidation
            //        builder.RequireScope("blog.core.api.BlogModule");
            //    });
            //    // 其他 Scope 策略
            //    // ...
            //});
            #endregion

            // 注入权限校验插件
            service.AddSingleton(permissionRequirement);
            service.AddScoped<IAuthorizationHandler, PermissionHandler>();
        }
    }
}
