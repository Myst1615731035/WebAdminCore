using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    /// Button
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_DictItem")]
    public partial class DictItem : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "项的归属", ColumnDataType = "varchar", Length = 50)]
        public string Pid { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "项标识", ColumnDataType = "nvarchar", Length = 50)]
        public string Label { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "项标识英文", ColumnDataType = "nvarchar", Length = 200)]
        public string EnLabel { get; set; }

        [SugarColumn(ColumnDescription = "项的值")]
        public int Value { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "项描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }
    }
}
