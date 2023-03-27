
namespace ApiModel
{
    public class ApiResponse
    {
        public int Status { get; set; } = 200;
        public string Value { get; set; } = "";
        public string Error { get; set; }
        public ContentJson MessageModel = new ContentJson();

        public ApiResponse(StatusCode apiCode, string msg = null)
        {
            switch (apiCode)
            {
                case StatusCode.CODE401:
                    {
                        Status = 401;
                        Value = "很抱歉，您无权访问该接口，请确保已经登录!";
                    }
                    break;
                case StatusCode.CODE403:
                    {
                        Status = 403;
                        Value = "很抱歉，访问受限，如有疑问请联系管理员!";
                    }
                    break;
                case StatusCode.CODE404:
                    {
                        Status = 404;
                        Value = "访问资源不存在!";
                    }
                    break;
                case StatusCode.CODE405:
                    {
                        Status = 405;
                        Value = "Http请求方法错误, 请检查!";
                    }
                    break;
                case StatusCode.CODE415:
                    {
                        Status = 415;
                        Value = "请求参数错误，请检查!";
                    }
                    break;
                case StatusCode.CODE429:
                    {
                        Status = 429;
                        Value = "服务器访问过多，请稍后再试!";
                    }
                    break;
                case StatusCode.CODE500:
                    {
                        Status = 500;
                        Value = "系统繁忙，请联系管理员";
                        Error = msg;
                    }
                    break;
            }

            MessageModel = new ContentJson()
            {
                status = Status,
                msg = Value,
                data = apiCode == StatusCode.CODE500 ? Error : null,
                success = apiCode != StatusCode.CODE200
            };
        }
    }

    public enum StatusCode
    {
        CODE200,
        CODE401,
        CODE403,
        CODE404,
        CODE405,
        CODE415,
        CODE429,
        CODE500
    }

    public class ApiWeek
    {
        public string week { get; set; }
        public string url { get; set; }
        public int count { get; set; }
    }
    public class ApiDate
    {
        public string date { get; set; }
        public int count { get; set; }
    }

    public class ActiveUserVM
    {
        public string user { get; set; }
        public int count { get; set; }
    }

    public class RequestApiWeekView
    {
        public List<string> columns { get; set; }
        public string rows { get; set; }
    }
    public class AccessApiDateView
    {
        public string[] columns { get; set; }
        public List<ApiDate> rows { get; set; }
    }
    public class RequestInfo
    {
        public string Ip { get; set; }
        public string Url { get; set; }
        public string Datetime { get; set; }
        public string Date { get; set; }
        public string Week { get; set; }
    }

    public class ApiLogAopInfo
    {
        /// <summary>
        /// 请求时间
        /// </summary>
        public string RequestTime { get; set; } = string.Empty;
        /// <summary>
        /// 操作人员
        /// </summary>
        public string OpUserName { get; set; } = string.Empty;
        /// <summary>
        /// 请求方法名
        /// </summary>
        public string RequestMethodName { get; set; } = string.Empty;
        /// <summary>
        /// 请求参数名
        /// </summary>
        public string RequestParamsName { get; set; } = string.Empty;
        /// <summary>
        /// 请求参数数据JSON
        /// </summary>
        public string RequestParamsData { get; set; } = string.Empty;
        /// <summary>
        /// 请求响应间隔时间
        /// </summary>
        public string ResponseIntervalTime { get; set; } = string.Empty;
        /// <summary>
        /// 响应时间
        /// </summary>
        public string ResponseTime { get; set; } = string.Empty;
        /// <summary>
        /// 响应结果
        /// </summary>
        public string ResponseJsonData { get; set; } = string.Empty;
    }

    public class ApiLogAopExInfo
    {
        public ApiLogAopInfo ApiLogAopInfo { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public string InnerException { get; set; } = string.Empty;
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExMessage { get; set; } = string.Empty;
    }
}
