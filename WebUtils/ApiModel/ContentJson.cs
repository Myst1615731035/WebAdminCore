using WebUtils;

namespace ApiModel
{
    public class ContentJson
    {
        public ContentJson(string? _msg = null)
        {
            if (_msg.IsNotEmpty())
            {
                this.msg = _msg;
            }
        }

        public ContentJson(bool _success, string? _msg = null, object data = null)
        {
            if (_success)
            {
                this.success = _success;
                this.msg = _msg ?? "Successed";
                this.data = data ?? new { };
            }
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; } = 200;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool success { get; set; } = false;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; } = "Fail";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public object data { get; set; } = new { };
    }
}
