using SqlSugar;
using WebModel.AppdixEntity;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///RolePermissionButton
    ///</summary>
    [DataSeed]
    [SystemDefault]
    [SugarTable("Sys_RolePermission")]
    public partial class RolePermission : RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "角色ID", ColumnDataType = "varchar", Length = 500)]
        public string RoleId { get; set; }

        [SugarColumn(ColumnDescription = "菜单ID/按钮ID", ColumnDataType = "varchar", Length = 50)]
        public string PermissionId { get; set; }
    }
}
