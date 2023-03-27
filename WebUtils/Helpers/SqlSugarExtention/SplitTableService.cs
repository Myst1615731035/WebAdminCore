using SqlSugar;
using System.Reflection;
using WebUtils;

namespace WebUtils
{
    public class SplitTableService: ISplitTableService
    {
        /// <summary>
        /// 根据实体类数据获取所有相关分表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entityInfo"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo entityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> list = new List<SplitTableInfo>();
            var SplitTableAttr = entityInfo.Type.GetCustomAttribute<SplitTableAttribute>();
            // 判断实体类是否设置了分表属性
            if (SplitTableAttr.IsNotEmpty())
            {
                // 查询该实体类下所有的分表
                var tables = tableInfos.Where(t => t.Name.Contains($"_{entityInfo.EntityName}")).ToList();
                // 获取所有分表的表名数据
                if (tables.IsNotEmpty() && tables.Count() > 0) list = tables.Select(t=> new SplitTableInfo() { TableName = t.Name }).ToList();
            }
            return list.OrderBy(t => t.TableName).ToList();
        }
        /// <summary>
        /// 获取默认表名
        /// </summary>
        /// <param name="db">链接对象</param>
        /// <param name="entityInfo">实体类数据</param>
        /// <returns></returns>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo)
        {
            return entityInfo.DbTableName;
        }
        /// <summary>
        /// 获取默认表名
        /// </summary>
        /// <param name="db">链接对象</param>
        /// <param name="entityInfo">实体类数据</param>
        /// <param name="type">分表类型</param>
        /// <returns></returns>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType type)
        {
            return entityInfo.DbTableName;
        }
        /// <summary>
        /// 根据分表字段获取表名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entityInfo"></param>
        /// <param name="splitType"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            // 分表遵循特殊命名前置规则，实体类名后置
            return fieldValue.IsEmpty() ? entityInfo.DbTableName: $"{fieldValue}_{entityInfo.DbTableName}"; 
        }
        /// <summary>
        /// 获取分表字段的值
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entityInfo"></param>
        /// <param name="splitType"></param>
        /// <param name="entityValue"></param>
        /// <returns></returns>
        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
            return value;
        }
    }
}
