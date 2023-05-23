using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using SqlSugar.Extensions;
using WebUtils.GlobalConfig;

namespace WebUtils
{
    public class JwtHelper
    {
        public static readonly string iss = AppConfig.Get("Program", "Issuer");
        public static readonly string aud = AppConfig.Get("Program", "Audience");
        public static readonly int expired = AppConfig.Get("Program", "ExpiredTime").ObjToInt();
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJwt(TokenModelJwt tokenModel)
        {
            string secret = GConfig.JWTSecret;
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, tokenModel.Name.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(expired)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(expired).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,iss),
                new Claim(JwtRegisteredClaimNames.Aud,aud),
            };
            // 可以将一个用户的多个角色全部赋予；
            claims.AddRange(tokenModel.Roles.Select(s => new Claim(ClaimTypes.Role, s)));
            
            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(issuer: iss,claims: claims,signingCredentials: creds);
            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);
            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TokenModelJwt SerializeJwt(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenModelJwt tokenModelJwt = new TokenModelJwt();
            // token校验
            if (token.IsNotEmpty() && jwtHandler.CanReadToken(token) && VerifyToken(token))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out object roles);
                tokenModelJwt = new TokenModelJwt
                {
                    Uid = jwtToken.Id.ObjToString(),
                    Roles = roles != null ? roles.ObjToString().Split(",").ToList() : new List<string>(),
                };
            }
            return tokenModelJwt;
        }

        public static bool VerifyToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var symmetricKeyAsBase64 = GConfig.JWTSecret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = jwtHandler.ReadJwtToken(token);
            return jwt.RawSignature == Microsoft.IdentityModel.JsonWebTokens.JwtTokenUtilities.CreateEncodedSignature(jwt.RawHeader + "." + jwt.RawPayload, signingCredentials);
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public object? Uid { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public object? Name { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
        /// <summary>
        /// 职能
        /// </summary>
        public string? Work { get; set; }
    }
}
