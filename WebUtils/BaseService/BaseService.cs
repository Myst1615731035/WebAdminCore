using SqlSugar;

namespace WebUtils.BaseService
{
    /// <summary>
    /// 继承仓储，数据库链接实例注入
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseService<TEntity> : OrmService<TEntity> where TEntity : class, new()
    {
        public override ISqlSugarClient Db
        {
            get
            {
                return _db;
            }
        }

        private readonly SqlSugarScope _dbBase = DBHelper.Db;
        protected ISqlSugarClient _db
        {
            get
            {
                /* 如果要开启多库支持，
                 * 1、在appsettings.json 中开启MutiDBEnabled节点为true，必填
                 * 2、设置一个主连接的数据库ID，节点MainDB，对应的连接字符串的Enabled也必须true，必填
                 */
                if (DBConfig.MutilDb)
                {
                    if (typeof(TEntity).GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault((x => x.GetType() == typeof(SugarTable))) is SugarTable sugarTable && !string.IsNullOrEmpty(sugarTable.TableDescription))
                    {
                        _dbBase.ChangeDatabase(sugarTable.TableDescription.ToLower());
                    }
                    else
                    {
                        _dbBase.ChangeDatabase(DBConfig.MainDbConfigId);
                    }
                }
                return _dbBase;
            }
        }
    }
}
