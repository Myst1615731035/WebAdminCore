using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///SysUser
    ///</summary>
    [SystemAuthTable]
    [SugarTable("Sys_SysUser")]
    public partial class SysUser : RootEntity<string>
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "账户", ColumnDataType = "varchar", Length = 50)]
        public string Account { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "密码", ColumnDataType = "varchar", Length = 50)]
        public string Password { get; set; }
        [SugarColumn(IsIgnore = true, ColumnDescription = "用户的新密码, 仅作为用户更新密码的验证，不做保存")]
        public string? NewPassword { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "昵称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "备注", ColumnDataType = "varchar", Length = 500)]
        public string? Remark { get; set; }

        #region 登录相关
        [SugarColumn(IsNullable = true, ColumnDescription = "头像", ColumnDataType = "varchar", Length = 500)]
        public string? Avatar { get; set; }

        [SugarColumn(IsIgnore = true, ColumnDescription = "角色名称")]
        public string? RoleNames { get; set; } = "";
        [SugarColumn(IsIgnore = true, ColumnDescription = "角色主键")]
        public string? RoleIdStr { get; set; } = "";
        [SugarColumn(IsIgnore = true, ColumnDescription = "角色主键")]
        public List<string>? RoleIds { get; set; } = new List<string>();
        #endregion

        #region 个人信息
        [SugarColumn(ColumnDescription = "性别", ColumnDataType = "int")]
        public int? Sex { get; set; }

        [SugarColumn(ColumnDescription = "生日")]
        public DateTime? Birth { get; set; }

        [SugarColumn(ColumnDescription = "身份证明", ColumnDataType = "varchar", Length = 50)]
        public string? IDCard { get; set; }

        [SugarColumn(ColumnDescription = "联系方式", ColumnDataType = "nvarchar", Length = 50)]
        public string? Tel { get; set; }

        [SugarColumn(ColumnDescription = "邮箱", ColumnDataType = "nvarchar", Length = 50)]
        public string? Email { get; set; }

        [SugarColumn(ColumnDescription = "住址", ColumnDataType = "nvarchar", Length = 500)]
        public string? Address { get; set; }
        #endregion

        #region 用户验证信息
        /// <summary>
        /// 关键业务修改时间
        /// </summary>
        public DateTime? CriticalModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        ///最后异常时间 
        /// </summary>
        public DateTime? LastErrorTime { get; set; } = DateTime.Now;

        /// <summary>
        ///错误次数 
        /// </summary>
        public int? ErrorCount { get; set; }
        #endregion
    }
}
