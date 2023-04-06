using Generate.Model;
using SqlSugar;
using System.Reflection;
using WebUtils;

namespace Generate.Service
{
    public static class Generate
    {
        public static List<ClassInfo> FilterRootColumns(this List<ClassInfo> list, Type root = null)
        {
            if(root.IsNotEmpty())
            {
                var rootCols = root.GetProperties().Select(t => t.Name).ToList();
                list.ForEach(t => t.Columns = t.Columns.Where(c => !rootCols.Any(rc => rc == c.Name)).ToList());
            }
            return list;
        }
        /// <summary>
        /// 生成实体文件
        /// </summary>
        /// <param name="list">数据表信息列表</param>
        /// <param name="templatePath">模板目录</param>
        /// <param name="outputDir">文件输出的目录，请使用相对路径</param>
        /// <param name="fileNameFormat">文件名称格式</param>
        /// <param name="overWriteExistFile">是否重写文件</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<ClassInfo> CreateFile(this List<ClassInfo> list, string templatePath, string outputDir, Func<ClassInfo, string> fileNameFormat, bool overWriteExistFile = false)
        {
            outputDir = Path.GetFullPath(outputDir);
            if (outputDir.IsEmpty()) throw new Exception("Invalid file path");
            if (list == null || list.Count == 0) throw new Exception("Invalid list for ClassInfo");
            ($"实体信息数量：{list.Count}\r\n" +
            $"模板：{templatePath}\r\n" +
            $"是否覆写文件：{overWriteExistFile} \r\n").WriteInfoLine();
            list.ForEach(t =>
            {
                var fileName = fileNameFormat.Invoke(t);
                var filePath = Path.Combine(outputDir, fileName);
                if (!FileHelper.Exists(outputDir, fileName, true) || overWriteExistFile)
                {
                    // 根据模板生成内容
                    var text = RazorHelper.Compile(FileHelper.ReadFile(templatePath), t).Result;
                    // 写文件
                    if (text.IsNotEmpty())
                    {
                        FileHelper.WriteFile(filePath, text);
                        $"{t.EntityName}: {fileName} create success!".WriteSuccessLine();
                    }
                    else
                    {
                        $"{t.EntityName}: {fileName} create fail!".WriteErrorLine();
                        throw new Exception("代码生成失败，目标文件内容为空");
                    }
                }
                else $"{t.EntityName}: {fileName} File is exist, skip!".WriteInfoLine();
            });
            $"{templatePath} 模板已完成创建".WriteInfoLine();
            return list;
        }

        /// <summary>
        /// 实体类转类信息，不获取字段信息，主要用于CodeFirst生成服务类文件
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<ClassInfo> ToClassInfos(this IEnumerable<Type> list)
        {
            var res = new List<ClassInfo>();
            if (list == null || list.Count() == 0) throw new Exception("Invalid list for Type");
            list.ToList().ForEach(t =>
            {
                var sugarTableAttr = t.GetCustomAttribute<SugarTable>();
                var splitTableAttr = t.GetCustomAttribute<SplitTableAttribute>();
                var ci = new ClassInfo()
                {
                    EntityName = t.Name,
                    IsSplitTable = splitTableAttr != null,
                    TableName = sugarTableAttr != null ? sugarTableAttr.TableName : t.Name,
                    Description = sugarTableAttr != null ? sugarTableAttr.TableDescription : t.Name
                };
                var props = t.GetProperties().ToList();
                var entity = Activator.CreateInstance(t);
                props.ForEach(p =>
                {
                    var columnAttr = p.GetCustomAttribute<SugarColumn>()?? new SugarColumn();
                    var defaultValue = p.GetValue(entity);
                    ci.Columns.Add(new ColumnInfo
                    {
                        Name = p.Name,
                        Description = columnAttr.ColumnDescription,
                        Type = p.PropertyType.IsGenericType ? p.PropertyType.GetGenericArguments()[0].Name : p.PropertyType.Name,
                        IsNullable = (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) || new NullabilityInfoContext().Create(p).WriteState is NullabilityState.Nullable),
                        DefaultValue = defaultValue
                    });
                });
                res.Add(ci);
            });
            return res;
        }
    }
}
