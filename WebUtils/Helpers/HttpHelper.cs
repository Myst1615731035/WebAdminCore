using Newtonsoft.Json.Linq;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebUtils.Helper
{
    /// <summary>
    /// httpclinet请求方式，请尽量使用IHttpClientFactory方式
    /// </summary>
    public class HttpHelper
    {
        #region HttpClient
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            try
            {
                string result = string.Empty;
                Uri getUrl = new Uri(url);
                using var httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(0, 0, 60);
                result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
            }
            return null;
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string requestJson = "")
        {
            try
            {
                string result = string.Empty;
                Uri postUrl = new Uri(url);
                using (HttpContent httpContent = new StringContent(requestJson))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var httpClient = new HttpClient();
                    httpClient.Timeout = new TimeSpan(0, 0, 60);
                    result = await httpClient.PostAsync(postUrl, httpContent).Result.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                }
                return result;
            }
            catch (Exception e)
            {
                LogHelper.LogException(new Exception($"Url: {url}\r\n"));
                LogHelper.LogException(e);
            }
            return null;
        }
        #endregion

        #region HttpWebRequest
        /// <summary>
        /// HttpWebRequest Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> GetWebAsync(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                string res = string.Empty;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 60000;
                if (headers.IsNotEmpty())
                {
                    foreach (var pair in headers)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        res = reader.ReadToEnd();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// HttpWebRequest Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> PostWebAsync(string url, string? json = null, Dictionary<string, string>? headers = null)
        {
            try
            {
                string res = string.Empty;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 60000;
                // 填补默认的ContentType
                if (headers.IsEmpty() || !headers.ContainsKey("Content-Type")) request.ContentType = "application/json";
                // 填充请求头
                if (headers.IsNotEmpty())
                {
                    foreach (var pair in headers)
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
                // 填充数据
                if (json.IsNotEmpty())
                {
                    using (var sm = request.GetRequestStream())
                    {
                        var bts = Encoding.UTF8.GetBytes(json);
                        sm.Write(bts, 0, bts.Length);
                    }
                }
                // 发送请求
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        res = reader.ReadToEnd();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
                return string.Empty;
            }
        }
        #endregion

        #region 拼接QueryString
        public static string JoinParamToQueryString(Dictionary<string, string> dic)
        {
            var res = "";
            if (dic.IsNotEmpty() && dic.Count > 0)
            {
                foreach (var pair in dic)
                {
                    if (pair.Key.IsNotEmpty()) res += $"{pair.Key}={pair.Value.ObjToString()}&";
                }
                res = res.TrimEnd('&');
            }
            return res;
        }
        public static string JoinParamToQueryString(JObject dic)
        {
            var res = "";
            if (dic.IsNotEmpty() && dic.Count > 0)
            {
                foreach (var pair in dic)
                {
                    if (pair.Key.IsNotEmpty()) res += $"{pair.Key.ObjToString()}={pair.Value.ObjToString()}&";
                }
                res = res.TrimEnd('&');
            }
            return res;
        }
        #endregion
    }
}