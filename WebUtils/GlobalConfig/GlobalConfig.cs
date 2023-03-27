using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtils.GlobalConfig
{
    public static class GConfig
    {
        /// <summary>
        /// JWT Token 令牌秘钥
        /// </summary>
        public const string JWTSecret = "HSoI42T0wFbSyIoByoxFvIq9bReJjX2W";

        /// <summary>
        /// 权限校验策略名称，主要用于全局的默认权限校验策略
        /// </summary>
        public const string PermissionAuthorizePolicyName = "Permission";
        /// <summary>
        /// 全局路由前缀
        /// </summary>
        public const string RoutePrefix = "";

        /// <summary>
        /// 全局文件导入路径
        /// </summary>
        public static string UploadFileDir = Path.Combine(AppConfig.WebRootPath, "Upload", "Files");

        /// <summary>
        /// 全局图片导入路径
        /// </summary>
        public static string UploadImgDir = Path.Combine(AppConfig.WebRootPath, "Upload", "Images");
    }
}
