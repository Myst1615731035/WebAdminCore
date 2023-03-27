using SqlSugar.Extensions;

namespace WebUtils
{
    public sealed class SmptHelper
    {
        #region 单例模式
        private SmptHelper() { }
        private static object lockObj = new object();
        private static SmptHelper? _instance;
        public static SmptHelper instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new SmptHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 字段
        private static readonly string Host = AppConfig.Get("Smtp", "Host");
        private static readonly int Port = AppConfig.Get("Smtp", "Port").ObjToInt();
        private static readonly bool EnableSsl = AppConfig.Get("Smtp", "SSL").ObjToBool();
        private static readonly string UserName = AppConfig.Get("Smtp", "UserName");
        private static readonly string PassWord = AppConfig.Get("Smtp", "Password");
        private static readonly string FromAddress = AppConfig.Get("Smtp", "FromAddress").ObjToString();
        private static readonly List<string> InquiryTo = AppConfig.GetList<string>("Smtp", "InquiryTo");
        private static readonly List<string> CarerrTo = AppConfig.GetList<string>("Smtp", "CarerrTo");
        private static readonly List<string> CC = AppConfig.GetList<string>("Smtp", "CC");
        #endregion
    }
}
