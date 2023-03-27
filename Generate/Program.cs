using Generate.Service;
using SqlSugar;
using System.Reflection;
using WebModel.RootEntity;

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
                    //.Where(t => t.Name == "Distributors")
                    .ToClassInfos();
types
    .CreateFile("Template/IRepositoryTemplate.cshtml", @"../../../../WebService", t => $"I{t.EntityName}Repository.cs")
    .CreateFile("Template/RepositoryTemplate.cshtml", @"../../../../WebService", t => $"{t.EntityName}Repository.cs")
    .CreateFile("Template/IServiceTemplate.cshtml", @"../../../../WebService", t => $"I{t.EntityName}Service.cs")
    .CreateFile("Template/ServiceTemplate.cshtml", @"../../../../WebService", t => $"{t.EntityName}Service.cs")
    .CreateFile("Template/ControllerTemplate.cshtml", @"../../../../MainCore/Controllers", t => $"{t.EntityName}Controller.cs");
#endregion

Console.ReadLine();