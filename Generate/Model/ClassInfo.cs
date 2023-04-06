
namespace Generate.Model
{
    /// <summary>
    /// 表信息记录类
    /// </summary>
    public class ClassInfo
    {
        /// <summary>
        /// 实体类
        /// </summary>
        public Type? Type { get; set; }
        /// <summary>
        /// 实体类名称
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 是否为分表
        /// </summary>
        public bool IsSplitTable { get; set; } = false;
        /// <summary>
        /// 实体类描述/表描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 字段信息
        /// </summary>
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
    }
}
