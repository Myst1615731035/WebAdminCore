using SqlSugar;
using WebModel.AppdixEntity;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    /// Button
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_Dict")]
    public partial class Dict : RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "编码", ColumnDataType = "varchar", Length = 50)]
        public string Code { get; set; }

        [SugarColumn(ColumnDescription = "字典名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }

        [SugarColumn(IsJson = true, ColumnDescription = "字典项列表")]
        public List<DictItem>? Items { get; set; } = new List<DictItem>();
    }
}
