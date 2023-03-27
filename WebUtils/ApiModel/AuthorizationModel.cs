using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ApiModel
{
    /// <summary>
    /// 接口权限类
    /// </summary>
    public class PermissionItem
    {
        /// <summary>
        /// 角色主键
        /// </summary>
        public virtual string RoleId { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public virtual string Url { get; set; }
    }
}
