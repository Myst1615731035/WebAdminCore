using System.Security.Claims;

namespace WebUtils.HttpContextUser
{
    public interface IUser
    {
        /// <summary>
        /// UserName
        /// </summary>
        string Name { get; }
        /// <summary>
        /// UserId
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 权限判断
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();
        string GetToken();
        List<string> GetUserInfoByToken(string ClaimType);
    }
}
