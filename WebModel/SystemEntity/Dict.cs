using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.SystemEntity
{
    ///<summary>
    /// Button
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_Dict")]
    public partial class Dict : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "编码", ColumnDataType = "varchar", Length = 50)]
        public string Code { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "字典名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }

        [SugarColumn(IsIgnore = true, ColumnDescription = "字典项列表")]
        public List<DictItem>? Items { get; set; } = new List<DictItem>();
    }
}
