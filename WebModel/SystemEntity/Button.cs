using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    [DataSeed]
    [SystemDefault]
    [SugarTable("Sys_Button")]
    public partial class Button: RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "菜单ID", ColumnDataType = "varchar", Length = 50, DefaultValue = "")]
        public string Mid { get; set; } = "";

        [SugarColumn(ColumnDescription = "按钮名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "按钮编码(Code)或对应的方法名称", ColumnDataType = "varchar", Length = 100)]
        public string Code { get; set; }

        [SugarColumn(ColumnDescription = "ICON", ColumnDataType = "varchar", Length = 100)]
        public string? Icon { get; set; }

        [SugarColumn(ColumnDescription = "说明", ColumnDataType = "varchar", Length = 1000)]
        public string? Description { get; set; }

        [SugarColumn(ColumnDescription = "排序")]
        public int? Sort { get; set; } = 0;

        [SugarColumn(ColumnDescription = "显示")]
        public bool? Visiable { get; set; } = true;
    }
}
