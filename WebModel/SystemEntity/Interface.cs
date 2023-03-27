using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.SystemEntity
{
    ///<summary>
    /// Button
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_Interface")]
    public partial class Interface : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "访问路径", ColumnDataType = "varchar", Length = 50)]
        public string Url { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string Description { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "接口校验规则(枚举类: InterfaceLimit)")]
        public int Limit { get; set; }
    }
}
