using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///RolePermissionButton
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_RolePermission")]
    public partial class RolePermission : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "角色ID", ColumnDataType = "varchar", Length = 500)]
        public string RoleId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "权限ID", ColumnDataType = "varchar", Length = 50)]
        public string PermissionId { get; set; }
    }
}
