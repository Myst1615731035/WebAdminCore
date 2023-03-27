using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Extensions;
using System.Reflection;
using WebUtils;
using WebUtils.Virtual;

namespace WebModel.DBSeed
{
    public class DBSeed: IDataSeedBase
    {
        protected static readonly ISqlSugarClient _db = DBHelper.Db;
        protected static readonly SqlSugarScope _scope = _db as SqlSugarScope;
        private static readonly string jsonDir = Path.Combine(AppConfig.ContentRootPath, "DataSeedJson");

        #region 库表生成 & 数据导入
        /// <summary>
        /// 库表生成
        /// </summary>
        public void  DbAsync()
        {
            var referencedAssemblies = Directory.GetFiles(AppContext.BaseDirectory, "WebModel.dll").Select(Assembly.LoadFrom).ToArray();
            // 项目框架实体类
            var models = referencedAssemblies.SelectMany(a => a.DefinedTypes).Select(type => type.AsType())
                            .Where(x => x.IsClass && x.Namespace != null && x.Namespace.Equals("WebModel.Entitys")).ToList();

            // 业务实体类
            var entitys = referencedAssemblies.SelectMany(a => a.DefinedTypes).Select(type => type.AsType())
                            .Where(x => x.IsClass && x.Namespace != null && x.Namespace.Equals("WebModel.Entitys")).ToList();

            #region 循环创建所有库、表
            try
            {
                //创建所有需要的数据库
                DBConfig.MutilDbs.allDbs.ForEach(t =>
                {
                    if (AppConfig.Get("DataBase", "SetUp", "CreateDB").ObjToBool())
                    {
                        $"Create DataBase: {AppConfig.Get("DataBase", "SetUp", "CreateDB")}".WriteInfoLine();
                        string log = $"{t.ConfigId}({t.DbTypeName}):  {t.ConnectionString}; ";
                        #region Oracle处理
                        //Oracle 数据库不支持该操作
                        if (t.DbType == DbType.Oracle)
                        {
                            log += $"  {false}\r\n Error: Oracle 数据库不支持该操作，可手动创建Oracle数据库!";
                            log.WriteErrorLine();
                            return;
                        }
                        #endregion
                        //创建数据库
                        ISqlSugarClient db = _scope.GetConnectionScope(t.ConfigId);
                        log += $"  {db.DbMaintenance.CreateDatabase()}";
                        log.WriteSuccessLine();
                    }
                });

                if(AppConfig.Get("DataBase", "SetUp", "CreateTable").ObjToBool())
                {
                    $"Create DataTables (CodeFirst): {AppConfig.Get("DataBase", "SetUp", "CreateTable")}".WriteInfoLine();
                    #region 主表生成数据表
                    models.ForEach(m =>
                    {
                        CreateTable(m);
                        // 判断是否需要初始化数据
                        if (AppConfig.Get("DataBase", "SetUp", "CreateData").ObjToBool()) DataAsync(m);
                    });
                    entitys.ForEach(e => CreateTable(e));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库 / 数据表 / 基础数据 初始化失败, 错误原因：{ex.Message}");
            }
            #endregion
        }


        /// <summary>
        /// 根据实体类生成表，包括分表
        /// </summary>
        /// <param name="type"></param>
        private void CreateTable(Type type)
        {
            var SugarTableAttr = type.GetCustomAttribute<SugarTable>();
            var SplitTableAttr = type.GetCustomAttribute<SplitTableAttribute>();
            
            // 存在分表配置的实体类，是否要生成源表
            if (SplitTableAttr.IsNotEmpty())
            {
                if(DBConfig.OriginSplitTableInit) _db.CodeFirst.SplitTables().InitTables(type);
                else
                {
                    var tables = _db.SplitHelper(type).GetTables();
                    if (tables != null && tables.Any())
                    {
                        tables.ForEach(t =>
                        {
                            _db.MappingTables.Add(type.Name, t.TableName);
                            _db.CodeFirst.InitTables(type);
                        });
                    }
                }
                $"Split Table for {type.Name} created or update successfully!".WriteSuccessLine();
            }
            else
            {
                // 无分表配置，则直接生成
                _db.CodeFirst.InitTables(type);
                $"{type.Name}({SugarTableAttr?.TableName}) created or update successfully!".WriteSuccessLine();
            }
        }
        /// <summary>
        /// 数据导入
        /// </summary>
        private void DataAsync(Type type)
        {
            var path = Path.Combine(jsonDir, $"{type.Name}.json");
            if(File.Exists(path))
            {
                Type typeList = typeof(List<>).MakeGenericType(type);
                object data = JsonConvert.DeserializeObject(FileHelper.ReadFile(path), typeList);
                //调用对象的方法
                if (!_db.Queryable<dynamic>().AsType(type).Any() && data.IsNotEmpty())
                {
                    _db.InsertableByObject(data).ExecuteCommand();
                    $"{type.Name}: Data initialization succeeded".WriteInfoLine();
                }
            }
        }
        #endregion
    }
}
