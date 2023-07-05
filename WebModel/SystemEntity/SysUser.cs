using SqlSugar;
using WebModel.RootEntity;
using WebUtils.Attributes;

namespace WebModel.Entitys
{
    ///<summary>
    ///SysUser
    ///</summary>
    [DataSeed]
    [SystemDefault]
    [SugarTable("Sys_SysUser")]
    public partial class SysUser : RootEntity<string>
    {
        #region 账户信息
        [SugarColumn(ColumnDescription = "账户", ColumnDataType = "varchar", Length = 50)]
        public string Account { get; set; }

        [SugarColumn(ColumnDescription = "密码", ColumnDataType = "varchar", Length = 50)]
        public string Password { get; set; }
        [SugarColumn(IsIgnore = true, ColumnDescription = "用户的新密码, 仅作为用户更新密码的验证，不做保存")]
        public string? NewPassword { get; set; }

        [SugarColumn(ColumnDescription = "昵称", ColumnDataType = "varchar", Length = 50)]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "备注", ColumnDataType = "varchar", Length = 500)]
        public string? Remark { get; set; }
        #endregion

        #region 登录相关
        [SugarColumn(ColumnDescription = "头像", ColumnDataType = "varchar", Length = 500)]
        public string? Avatar { get; set; }

        [SugarColumn(IsJson = true, ColumnDescription = "用户授权的角色ID", ColumnDataType = "nvarchar(max)")]
        public List<string> RoleIds { get; set; } = new List<string>();
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

        #region 用户登录信息
        /// <summary>
        /// 最近登录IP
        /// </summary>
        [SugarColumn(ColumnDescription = "最近登录IP", ColumnDataType = "varchar", Length = 500)]
        public string? LastLoginIP { get; set; }
        /// <summary>
        /// 最近登录时间
        /// </summary>
        [SugarColumn(ColumnDescription = "最近登录时间")]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        ///最近登录异常时间 
        /// </summary>
        [SugarColumn(ColumnDescription = "最近登录异常时间")]
        public DateTime? LastErrorTime { get; set; }

        /// <summary>
        /// 错误次数 
        /// </summary>
        [SugarColumn(ColumnDescription = "登录异常错误次数")]
        public int? ErrorCount { get; set; }
        #endregion

        #region 业务相关
        [SugarColumn(IsJson = true, ColumnDescription = "用户拥有的站点权限", ColumnDataType = "nvarchar(max)")]
        public List<string>? SiteIds { get; set; } = new List<string>();
        #endregion
    }
}
