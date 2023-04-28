using SqlSugar;
using WebModel.AppdixEntity;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///Menu
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_Menu")]
    public partial class Menu : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "菜单:父级主键，最顶级菜单的父级ID为空字符串;", ColumnDataType = "varchar", Length = 50, DefaultValue = "")]
        public string Pid { get; set; } = "";

        [SugarColumn(ColumnDescription = "接口ID", ColumnDataType = "varchar", Length = 50)]
        public string? Fid { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "菜单名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "类型: 0: 目录,1: 页面", ColumnDataType = "int")]
        public int Type { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "路径", ColumnDataType = "varchar", Length = 50)]
        public string? Path { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "图标", ColumnDataType = "varchar", Length = 50)]
        public string? Icon { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "描述", ColumnDataType = "varchar", Length = 50)]
        public string? Description { get; set; }

        [SugarColumn(ColumnDescription = "是否隐藏")]
        public bool Visiable { get; set; } = true;

        [SugarColumn(IsIgnore = true, ColumnDescription = "用于角色授权")]
        public bool Selected { get; set; } = false;

        [SugarColumn(IsIgnore = true)]
        public List<Menu> Children { get; set; } = new List<Menu>();

        [SugarColumn(IsJson = true, ColumnDescription = "当前页面的按钮数据")]
        public List<Button>? Buttons { get; set; } = new List<Button>();
    }
}
