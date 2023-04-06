using SqlSugar;

namespace WebModel.RootEntity
{
    public class SplitRoot: RootEntity<string>
    {
        /// <summary>
        /// 分表的标志字段
        /// </summary>
        [SplitField]
        [SugarColumn(ColumnDataType = "nvarchar(100)", ColumnDescription = "分表字段，站点字号")]
        public string SplitSign { get; set; }
    }
}
