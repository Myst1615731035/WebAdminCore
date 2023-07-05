using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    /// Button
    ///</summary>
    [DataSeed]
    [SystemDefault]
    [SugarTable("Sys_Interface")]
    public partial class Interface : RootEntity<string>
    {
        [SugarColumn(ColumnDescription = "访问路径", ColumnDataType = "varchar", Length = 50)]
        public string Url { get; set; }

        [SugarColumn(ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500)]
        public string? Description { get; set; }

        [SugarColumn(ColumnDescription = "接口校验规则(枚举类: InterfaceLimit)")]
        public int? Limit { get; set; }
    }
}
