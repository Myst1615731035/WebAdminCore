using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    /// Roles
    ///</summary>
    [DataSeed]
    [SystemDefault]
    [SugarTable("Sys_Role")]
    public partial class SysRole : RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "角色名称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "角色的默认页面路径Id", ColumnDataType = "varchar", Length = 500)]
        public string? DefaultRouteId { get; set; }

        [SugarColumn(IsIgnore = true, ColumnDescription = "角色的默认页面路径", ColumnDataType = "varchar", Length = 500)]
        public string? DefaultRoute { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }
    }
}
