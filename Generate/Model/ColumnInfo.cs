namespace Generate.Model
{
    public class ColumnInfo
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 烈描述
        /// </summary>
        public string Description { get; set; }
        private string _type { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type
        {
            get
            {
                return $"{_type}{(IsNullable ? "?" : "")}";
            }
            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// Nullable
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// SqlSugar的字段属性，拼装完成后的
        /// </summary>
        public string Props { get; set; }
        /// <summary>
        /// 是否存在默认值
        /// </summary>
        public bool HasDefaultValue { get; set; } = false;
        private string _defaultValue { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue
        {
            get
            {
                return $"{(HasDefaultValue ? $" = {_defaultValue};" : "")}";
            }
            set
            {
                _defaultValue = value;
            }
        }
    }
}
