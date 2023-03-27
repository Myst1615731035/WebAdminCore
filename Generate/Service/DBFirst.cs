using Generate.Model;
using SqlSugar;
using SqlSugar.Extensions;
using System.Text.RegularExpressions;
using WebUtils;

namespace Generate.Service
{
    public class DBFirst
    {
        private ISqlSugarClient _db;
        public DBFirst(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据数据库连接配置，生成实体类文件
        /// </summary>
        /// <param name="tableFilter">数据表过滤规则</param>
        /// <param name="columnFilter">列过滤规则</param>
        /// <param name="entityNameFormat">根据表名获取实体类名的生成规则</param>
        /// <param name="splitTableFilter">实体类名生成后，同名实体类名是否获取，用于存在分表的情况</param>
        /// <returns></returns>
        public List<ClassInfo> GenerateFromDB(Func<DbTableInfo, bool> tableFilter = null, Func<DbColumnInfo, bool> columnFilter = null, Func<string, string> entityNameFormat = null, bool splitTableFilter = true)
        {
            var list = new List<ClassInfo>();
            try
            {
                var tables = _db.DbMaintenance.GetTableInfoList(false);
                // 根据筛选条件过滤获取到的table信息
                if (tableFilter != null) tables = tables.Where(tableFilter).ToList();
                // 遍历表信息
                tables.ForEach(async t =>
                {
                    // 获取表信息类
                    var model = new ClassInfo()
                    {
                        EntityName = entityNameFormat != null ? entityNameFormat.Invoke(t.Name) : t.Name,
                        TableName = t.Name,
                        Description = (t.Description ?? "") == "" ? t.Name : "",
                    };

                    // 分表保留一个，若列表中已存在相同实体类名的数据，则跳过字段获取与添加过程
                    if (splitTableFilter && list.Any(t => t.EntityName == model.EntityName))
                    {
                        model.IsSplitTable = true;
                        return;
                    } 

                    // 获取表的所有字段
                    var columns = _db.DbMaintenance.GetColumnInfosByTableName(t.Name, false);
                    // 对字段进行筛选
                    if (columnFilter != null) columns = columns.Where(columnFilter).ToList();

                    #region 遍历字段信息
                    columns.ForEach(c =>
                    {
                        // 获取C#数据类型
                        var columnType = _db.Ado.DbBind.GetCsharpTypeNameByDbTypeName(c.DataType);
                        columnType = columnType == "byteArray" ? "byte[]" : columnType;
                        // 定义临时变量
                        var hasDefaultValue = c.DefaultValue != null;
                        string description = "", defaultValue = "";

                        #region 获取SqlSugar属性
                        var props = new List<string>() { $"ColumnName = \"{c.DbColumnName}\"" };
                        if (c.ColumnDescription != null && c.ColumnDescription != "")
                        {
                            props.Add($"ColumnDescription=\"{c.ColumnDescription}\"");
                            description = c.ColumnDescription;
                        }
                        else description = c.DbColumnName;
                        if (c.IsNullable) props.Add("IsNullable = true");
                        if (c.IsPrimarykey) props.Add("IsPrimaryKey = true");
                        if (c.IsIdentity) props.Add("IsIdentity = true");
                        if (c.DataType.Equals("timestamp", StringComparison.OrdinalIgnoreCase)) props.Add("IsOnlyIgnoreInsert = true,IsOnlyIgnoreUpdate = true");
                        if (columnType.Equals("string", StringComparison.OrdinalIgnoreCase))
                        {
                            var ColumnDataTypeProps = $"ColumnDataType = \"{c.DataType}\"{(c.Length < 0 ? "(MAX)" : "")}";
                            props.Add(ColumnDataTypeProps);
                        }
                        if (hasDefaultValue)
                        {
                            props.Add($"DefaultValue=\"{GetProertypeDefaultValue(c.DefaultValue)}\"");
                            if (c.DefaultValue.Equals("getdate()", StringComparison.OrdinalIgnoreCase)) defaultValue = "DateTime.Now";
                            if (c.DefaultValue.Equals("newid()", StringComparison.OrdinalIgnoreCase)) defaultValue = "Guid.NewGuid().ToString()";
                            if (c.DataType.Equals("bit", StringComparison.OrdinalIgnoreCase)) defaultValue = c.DefaultValue == "0" ? "false" : "true";
                            else defaultValue = GetPropertyTypeConvert(c.DataType, c.DefaultValue);
                        }
                        #endregion
                        // 填充表的字段信息
                        model.Columns.Add(new ColumnInfo()
                        {
                            Name = c.DbColumnName,
                            Description = description,
                            Type = columnType,
                            IsNullable = c.IsNullable,
                            Props = string.Join(", ", props),
                            HasDefaultValue = hasDefaultValue,
                            DefaultValue = defaultValue,
                        });
                    });
                    #endregion
                    list.Add(model);
                });
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return list;
            }
        }



        #region 私有辅助方法
        private string GetPropertyTypeConvert(string DataType, string DefaultValue)
        {
            var convertString = GetProertypeDefaultValue(DefaultValue);
            if (convertString == "DateTime.Now" || convertString == null)
                return convertString;
            if (convertString.ObjToString() == "newid()")
            {
                return "Guid.NewGuid().ToString()";
            }
            if (DataType == "bit")
                return (convertString == "1" || convertString.Equals("true", StringComparison.CurrentCultureIgnoreCase)).ToString().ToLower();
            string result = _db.Ado.DbBind.GetConvertString(DataType) + "(\"" + convertString + "\")";
            return result;
        }
        private string GetProertypeDefaultValue(string result)
        {
            if (result == null) return null;
            if (Regex.IsMatch(result, @"^\(\'(.+)\'\)$"))
            {
                result = Regex.Match(result, @"^\(\'(.+)\'\)$").Groups[1].Value;
            }
            if (Regex.IsMatch(result, @"^\(\((.+)\)\)$"))
            {
                result = Regex.Match(result, @"^\(\((.+)\)\)$").Groups[1].Value;
            }
            if (Regex.IsMatch(result, @"^\((.+)\)$"))
            {
                result = Regex.Match(result, @"^\((.+)\)$").Groups[1].Value;
            }
            if (result.Equals("getdate()", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "DateTime.Now";
            }
            result = result.Replace("\r", "\t").Replace("\n", "\t");
            result = result.IsIn("''", "\"\"") ? string.Empty : result;
            return result;
        }
        #endregion
    }
}
