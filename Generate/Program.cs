using Generate.Model;
using Generate.Service;
using System.Reflection;
using WebModel.RootEntity;
using WebUtils;
using WebUtils.Attributes;

#region DBFirst
//var db = new SqlSugarClient(new ConnectionConfig()
//{
//    ConnectionString = "Data Source=127.0.0.1;Initial Catalog=StockCore;User ID=sa;Password=123456;Encrypt=True;TrustServerCertificate=True;",
//    DbType = DbType.SqlServer,
//    IsAutoCloseConnection = true
//});

//var dbFirst = new DBFirst(db);

//dbFirst.GenerateFromDB(entityNameFormat: t => t.Substring(t.IndexOf("_") + 1))
//        .FilterRootColumns(typeof(RootEntity<string>))
//        .CreateFile("Template/EntityTemplate.cshtml", "../../../../WebModel/WorkEntity", t => $"{t.EntityName}.cs")
//.CreateFile("Template/IRepositoryTemplate.cshtml", @"../../../../WebService", t => $"I{t.EntityName}Repository.cs")
//.CreateFile("Template/RepositoryTemplate.cshtml", @"../../../../WebService", t => $"{t.EntityName}Repository.cs")
//.CreateFile("Template/IServiceTemplate.cshtml", @"../../../../WebService", t => $"I{t.EntityName}Service.cs")
//.CreateFile("Template/ServiceTemplate.cshtml", @"../../../../WebService", t => $"{t.EntityName}Service.cs")
//.CreateFile("Template/ControllerTemplate.cshtml", @"../../../../MainCore/Controllers", t => $"{t.EntityName}Controller.cs");
#endregion

#region CodeFirst
var types = Directory.GetFiles(AppContext.BaseDirectory, "WebModel.dll")
                    .Select(Assembly.LoadFrom).ToArray()
                    .SelectMany(a => a.DefinedTypes).Select(type => type.AsType())
                    .Where(t => t.Namespace == "WebModel.Entitys")
                    .ToClassInfos();

Func<ClassInfo, bool> noCreateSelectFunc = t => t.Type.IsNotEmpty() && t.Type.GetCustomAttribute<SystemAuthTable>().IsNotEmpty();

types
    .CreateFile("Template/IRepositoryTemplate.cshtml", @"../../../../WebService/IWorkRepository", t => $"I{t.EntityName}Repository.cs")
    .CreateFile("Template/RepositoryTemplate.cshtml", @"../../../../WebService/WorkRepository", t => $"{t.EntityName}Repository.cs")
    .CreateFile("Template/IServiceTemplate.cshtml", @"../../../../WebService/IWorkService", t => $"I{t.EntityName}Service.cs")
    .CreateFile("Template/ServiceTemplate.cshtml", @"../../../../WebService/WorkService", t => $"{t.EntityName}Service.cs")
    .CreateFile("Template/ControllerTemplate.cshtml", @"../../../../MainCore/Controllers", t => $"{t.EntityName}Controller.cs");
types.FilterRootColumns(typeof(RootEntity<string>))
    .CreateFile("Template/GridVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/index.vue", noCreateSelectFunc: noCreateSelectFunc)
    .CreateFile("Template/FormVue.cshtml", @"../../../../UI/src/pages", t => $"{t.EntityName}/form.vue", noCreateSelectFunc: noCreateSelectFunc);
#endregion

Console.ReadLine();