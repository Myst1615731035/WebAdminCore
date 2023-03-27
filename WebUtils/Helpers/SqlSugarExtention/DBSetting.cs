using SqlSugar;

namespace WebUtils
{
    /// <summary>
    /// 数据库配置类
    /// </summary>
    public class DBSetting
    {
        public bool Enable { get; set; }
        public string? ConfigId { get; set; }
        public DbType DbType { get; set; }
        public string? ConnectionString { get; set; }
        public int HitRate { get; set; }
        public string DbTypeName
        {
            get { return Enum.GetName(typeof(DbType), DbType); }
        }
    }
}
