using SqlSugar;

namespace WebModel.RootEntity
{
    public class RootEntity<T> where T : IEquatable<T>
    {
        /// <summary>
        /// 建议使用GUID作为主键
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, ColumnDataType = "varchar", Length = 50, ColumnDescription = "主键")]
        public T Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "排序", DefaultValue = "0")]
        public int Sort { get; set; }

        #region 新增
        /// <summary>
        /// 创建人Id
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true, ColumnDataType = "varchar", Length = 50, ColumnDescription = "创建人主键")]
        public T? CreateId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true, ColumnDataType = "datetime", ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }
        #endregion

        #region 更新
        /// <summary>
        /// 最后一次更新人Id
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, ColumnDataType = "varchar", Length = 50, ColumnDescription = "更新人的主键")]
        public T? ModifyId { get; set; }
        /// <summary>
        /// 最后一次更新的时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, ColumnDataType = "datetime", ColumnDescription = "创建时间")]
        public DateTime? ModifyTime { get; set; }
        #endregion

        /// <summary>
        /// 是否已删除, 默认否
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDescription = "是否逻辑删除，默认为否(0)", DefaultValue = "0")]
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 数据版本号，乐观锁
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true, ColumnDescription = "乐观锁", ColumnDataType = "timestamp", IsEnableUpdateVersionValidation = true)]
        public byte[]? Version { get; set; }
    }
}
