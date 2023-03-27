using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.SystemEntity
{
    ///<summary>
    /// Button
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_Button")]
    public partial class Button : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "所属菜单", ColumnDataType = "varchar", Length = 50)]
        public string Mid { get; set; }

        [SugarColumn(ColumnDescription = "访问的接口ID", ColumnDataType = "varchar", Length = 50)]
        public string? Fid { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "按钮名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "调用方法名称", ColumnDataType = "varchar", Length = 50)]
        public string? Function { get; set; }
        [SugarColumn(ColumnDescription = "按钮的编码，用于vxe-table组件", ColumnDataType = "varchar", Length = 50)]
        public string? Code { get; set; }

        [SugarColumn(ColumnDescription = "图标", ColumnDataType = "varchar", Length = 50)]
        public string? Icon { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }

        [SugarColumn(ColumnDescription = "是否隐藏")]
        public bool Visiable { get; set; } = false;

        [SugarColumn(IsIgnore = true, ColumnDescription = "用于角色授权")]
        public bool Selected { get; set; } = false;
    }
}
