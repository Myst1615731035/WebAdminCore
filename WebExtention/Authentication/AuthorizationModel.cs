using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace WebExtention.Authentication
{
    /// <summary>
    /// 实现授权类接口
    /// </summary>
    public class AuthorizationModel: IAuthorizationRequirement
    {
        /// <summary>
        /// 权限校验参数类
        /// </summary>
        /// <param name="claimType">授权策略，默认基于角色</param>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">听众</param>
        /// <param name="signingCredentials">签名凭据</param>
        /// <param name="expiration">接口的过期时间</param>
        /// <param name="deniedAction">拒绝授权的跳转地址（目前无用）</param>
        /// <param name="permissions">权限列表</param>
        public AuthorizationModel(string claimType, string issuer, string audience, SigningCredentials signingCredentials, TimeSpan expiration, string deniedAction, List<PermissionItem> permissions) 
        {
            ClaimType = claimType;
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            SigningCredentials = signingCredentials;
            DeniedAction = deniedAction;
            Permissions = permissions;
        }
        /// <summary>
        /// 请求路径
        /// </summary>
        public string LoginPath { get; set; } = "/Api/Login";
        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }
        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
        /// <summary>
        /// 无权限action
        /// </summary>
        public string DeniedAction { get; set; }
        /// <summary>
        /// 用户权限集合，一个订单包含了很多详情，
        /// 同理，一个网站的认证发行中，也有很多权限详情(这里是Role和URL的关系)
        /// </summary>
        public List<PermissionItem> Permissions { get; set; } = new List<PermissionItem>();
    }
}
