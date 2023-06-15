using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Extensions;
using System.Reflection;
using WebUtils;
using WebUtils.Attributes;
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
            // 实体类
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
                    // CodeFirst 初始化数据库表
                    entitys.ForEach(e => CreateTable(e));
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

            #region 生成表
            if ((SplitTableAttr.IsNotEmpty() && DBConfig.OriginSplitTableInit) || SplitTableAttr.IsEmpty())
            {
                // 无分表配置，则直接生成
                _db.CodeFirst.InitTables(type);
                $"{type.Name}({SugarTableAttr?.TableName}) created or update successfully!".WriteSuccessLine();
            }
            #endregion

            #region 生成分表
            if (SplitTableAttr.IsNotEmpty())
            {
                var tables = _db.SplitHelper(type).GetTables();
                if (tables != null && tables.Any())
                {
                    tables.ForEach(t =>
                    {
                        _db.MappingTables.Add(type.Name, t.TableName);
                        _db.CodeFirst.InitTables(type);
                    });
                    $"{tables.Count} SplitTable for {type.Name} {(DBConfig.OriginSplitTableInit ? " And Origin table" : "")} created or update successfully!".WriteSuccessLine();
                }
            }
            #endregion

            #region 系统数据初始化
             DataAsync(type);
            #endregion
        }
        /// <summary>
        /// 数据导入
        /// </summary>
        private void DataAsync(Type type)
        {
            var SystemAuthTableAttr = type.GetCustomAttribute<DataSeedAttribute>();
            if (SystemAuthTableAttr.IsNotEmpty() && AppConfig.Get("DataBase", "SetUp", "CreateData").ObjToBool())
            {
                var path = Path.Combine(jsonDir, $"{type.Name}.json");
                if (File.Exists(path))
                {
                    Type typeList = typeof(List<>).MakeGenericType(type);
                    object data = JsonConvert.DeserializeObject(File.ReadAllText(path), typeList);
                    //调用对象的方法
                    if (!_db.Queryable<dynamic>().AsType(type).Any() && data.IsNotEmpty())
                    {
                        _db.InsertableByObject(data).ExecuteCommand();
                        $"{type.Name}: Data initialization succeeded".WriteInfoLine();
                    }
                }
            }
        }
        #endregion
    }
}
