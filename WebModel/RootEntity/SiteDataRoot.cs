using SqlSugar;

namespace WebModel.RootEntity
{
    public class SiteDataRoot: RootEntity<string>
    {
        /// <summary>
        /// 数据状态，枚举：RootStatus
        /// </summary>
        [SugarColumn(ColumnDataType = "int", ColumnDescription = "数据状态")]
        public int Status { get; set; }

        /// <summary>
        /// 所属站点，分表字段
        /// </summary>
        [SplitField]
        [SugarColumn(ColumnDataType = "nvarchar(100)", ColumnDescription = "分表字段，站点字号")]
        public string Idtag { get; set; }
    }
}
