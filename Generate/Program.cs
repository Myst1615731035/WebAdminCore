using Generate.Service;
using System.Reflection;
using WebModel.RootEntity;
using WebUtils;
using WebUtils.Attributes;

var systemDefaultEntitys = Directory.GetFiles(AppContext.BaseDirectory, "WebModel.dll")
                            .Select(Assembly.LoadFrom).ToArray()
                            .SelectMany(a => a.DefinedTypes).Select(type => type.AsType())
                            .Where(t => t.Namespace == "WebModel.Entitys" && t.GetCustomAttribute<SystemDefaultAttribute>().IsNotEmpty())
                            .ToClassInfos();

#region DBFirst
//var db = new SqlSugarClient(new ConnectionConfig()
//{
//    ConnectionString = "Data Source=127.0.0.1;Initial Catalog=StockCore;User ID=sa;Password=123456;Encrypt=True;TrustServerCertificate=True;",
//    DbType = DbType.SqlServer,
//    IsAutoCloseConnection = true
//});

//var dbFirst = new DBFirst(db);

//var entitys = dbFirst.GenerateFromDB(tableFilter: dt => !systemDefaultEntitys.Any(t => t.TableName == dt.Name || t.EntityName == dt.Name));
//entitys.CreateFile("Template/Entity.cshtml", @"../../../../WebModel/WorkEntity", t => $"{t.EntityName}.cs", overWriteExistFile: true)
//    .CreateFile("Template/Controller.cshtml", @"../../../../MainCore/Controllers", t => $"{t.EntityName}Controller.cs", overWriteExistFile: true)
//    .CreateFile("Template/GridVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/index.vue", overWriteExistFile: true)
//    .CreateFile("Template/FormVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/form.vue", overWriteExistFile: true);

//entitys.CreateFileForAllClass("Template/WorkApi.cshtml", @"../../../../UI/src/pages/workApi.ts", true);
#endregion

#region CodeFirst
var types = Directory.GetFiles(AppContext.BaseDirectory, "WebModel.dll")
                    .Select(Assembly.LoadFrom).ToArray()
                    .SelectMany(a => a.DefinedTypes).Select(type => type.AsType())
                    .Where(t => t.Namespace == "WebModel.Entitys" && t.GetCustomAttribute<DataSeedAttribute>().IsEmpty())
                    .ToClassInfos();
// 生成Controller
types.CreateFile("Template/Controller.cshtml", @"../../../../MainCore/Controllers", t => $"{t.EntityName}Controller.cs");

// 生成UI 基本表格模式, 无下级字段
types.Where(t => !t.Type.GetProperties().Any(p => p.Name.Equals("Children", StringComparison.OrdinalIgnoreCase))).ToList()
    .FilterRootColumns(typeof(RootEntity<string>))
    .CreateFile("Template/GridVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/index.vue")
    .CreateFile("Template/FormVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/form.vue");

// 生成UI 树形表格模式
types.Where(t => t.Type.GetProperties().Any(p => p.Name.Equals("Children", StringComparison.OrdinalIgnoreCase))).ToList()
    .FilterRootColumns(typeof(RootEntity<string>))
    .CreateFile("Template/TreeGridVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/index.vue")
    .CreateFile("Template/TreeFormVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/form.vue");
// 生成业务模块的接口文件
types.CreateFileForAllClass("Template/WorkApi.cshtml", @"../../../../UI/src/pages/workApi.ts", true);
#endregion

"生成完成".WriteSuccessLine();