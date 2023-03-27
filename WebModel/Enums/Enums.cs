using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebModel.Enums
{
    public enum InterfaceLimit
    {
        /// <summary>
        /// 已授权才可访问
        /// </summary>
        Authorized = 0,
        /// <summary>
        /// 只需登录校验
        /// </summary>
        Client = 1,
        /// <summary>
        /// 游客身份，面对第三方开放的接口
        /// </summary>
        Visitor = 2,
        /// <summary>
        /// 所有人都可
        /// </summary>
        Everyone = 3,
    }
}
