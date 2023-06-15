using SqlSugar;
using System.ComponentModel.DataAnnotations;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///Menu
    ///</summary>
    [DataSeed]
    [SugarTable("Sys_Menu")]
    public partial class Menu : RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "父级菜单", ColumnDataType = "varchar", Length = 50, DefaultValue = "")]
        public string Pid { get; set; } = "";

        [SugarColumn(ColumnDescription = "菜单名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "类型: 0: 目录,1: 页面", ColumnDataType = "int")]
        public int Type { get; set; }

        [SugarColumn(ColumnDescription = "路径", ColumnDataType = "varchar", Length = 50)]
        public string? Path { get; set; }

        [SugarColumn(ColumnDescription = "图标", ColumnDataType = "varchar", Length = 50)]
        public string? Icon { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 50)]
        public string? Description { get; set; }

        [SugarColumn(ColumnDescription = "是否隐藏")]
        public bool? Visiable { get; set; } = true;

        [SugarColumn(IsIgnore = true)]
        public List<Menu>? Children { get; set; } = new List<Menu>();

        [Navigate(NavigateType.OneToMany, nameof(Button.Mid))]
        public List<Button> Buttons { get; set; }
    }
}
