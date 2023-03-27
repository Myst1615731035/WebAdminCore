using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using NPOI.SS.Formula.Functions;
using SqlSugar.Extensions;
using System.Text.RegularExpressions;

namespace WebExtention.MiddleWare
{
    public class URLRewrite : IRule
    {
        private readonly Regex regex = new Regex("/api/|/.*(\\.).*$|/swagger", RegexOptions.IgnoreCase);
        public void ApplyRule(RewriteContext context)
        {
            // 获取当前的请求地址
            var request = context.HttpContext.Request;
            var url = request.Path.ObjToString();
            if ((!regex.IsMatch(url)) || url == "/") 
            {
                request.Path = new PathString("/index.html");
                context.Result = RuleResult.ContinueRules;
            }else context.Result = RuleResult.SkipRemainingRules;
        }
    }
}
