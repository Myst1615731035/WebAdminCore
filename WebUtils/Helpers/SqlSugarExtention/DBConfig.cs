using SqlSugar;
using SqlSugar.Extensions;

namespace WebUtils
{
    /// <summary>
    /// 数据库配置读取
    /// </summary>
    public static class DBConfig
    {
        public static readonly string MainDbConfigId = AppConfig.Get("DataBase", "MainDb");//主库配置
        public static readonly bool MutilDb = AppConfig.Get("DataBase", "MutilDb").ObjToBool();//多库?
        public static readonly bool CQRSEnabled = AppConfig.Get("DataBase", "CQRSEnabled").ObjToBool();//读写分离?
        public static readonly bool OriginSplitTableInit = AppConfig.Get("DataBase", "OriginSplitTableInit").ObjToBool();//分表是否生成源表
        public static (List<DBSetting> allDbs, List<DBSetting> slaveDbs) MutilDbs => GetMutilDbs();//多库配置

        private static (List<DBSetting>, List<DBSetting>) GetMutilDbs()
        {
            List<DBSetting> allDbs = AppConfig.GetList<DBSetting>("DataBase", "DBS").Where(t => t.Enable).ToList();
            List<DBSetting> aloneDb = new List<DBSetting>();//单库
            List<DBSetting> slaveDbs = new List<DBSetting>();
            allDbs.ForEach(t => t = SpecialDbString(t));

            if (allDbs.Count > 0)
            {
                if (!MutilDb)
                {
                    #region 单库模式

                    if (CQRSEnabled)
                    {
                        #region 读写分离, 获取从库
                        slaveDbs = allDbs.Where(d => d.ConfigId != MainDbConfigId).ToList();
                        #endregion
                    }
                    else
                    {
                        #region 常规
                        allDbs = allDbs.Where(t => t.ConfigId == MainDbConfigId).ToList();
                        if (allDbs.Count != 1)
                        {
                            throw new Exception("请确认单库情况下主库配置!");
                        }
                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region 多库模式
                    #endregion
                }
                return (allDbs, slaveDbs);
            }
            throw new Exception("请配置数据库链接!");
        }

        /// <summary>
        /// 用于对数据库链接进行处理，例如加解密，保存到单独文件等
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static DBSetting SpecialDbString(DBSetting setting)
        {
            switch (setting.DbType)
            {
                case DbType.Sqlite:
                    setting.ConnectionString = $"DataSource={Path.Combine(Environment.CurrentDirectory, setting.ConnectionString)}";
                    break;
                case DbType.MySql:
                case DbType.SqlServer:
                case DbType.Oracle:
                case DbType.Access:
                default:
                    break;
            }
            return setting;
        }
    }
}
