using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Reflection;
using WebUtils.HttpContextUser;

namespace WebUtils
{
    public class DBHelper
    {
        public static SqlSugarScope Db => GetSqlSugarScope();
        private static SqlSugarScope GetSqlSugarScope()
        {
            // 数据库链接配置
            var list = new List<ConnectionConfig>();

            #region 从库配置
            var slaveList = new List<SlaveConnectionConfig>();
            DBConfig.MutilDbs.slaveDbs.ForEach(s =>
            {
                slaveList.Add(new SlaveConnectionConfig()
                {
                    HitRate = s.HitRate,
                    ConnectionString = s.ConnectionString
                });
            });
            #endregion

            #region 所有库配置
            DBConfig.MutilDbs.allDbs.ForEach(t =>
            {
                list.Add(new ConnectionConfig()
                {
                    ConfigId = t.ConfigId,
                    ConnectionString = t.ConnectionString,
                    DbType = t.DbType,
                    IsAutoCloseConnection = true,
                    SlaveConnectionConfigs = slaveList,
                    InitKeyType = InitKeyType.Attribute,

                    #region AOP 日志
                    AopEvents = new AopEvents()
                    {
                        #region 数据过滤
                        DataExecuting = (oldValue, entityInfo) =>
                        {
                            //无法获取使用AddScoped注入的对象，但可以获取AddTransient和AddSingleton注入的对象
                            //因此：手动获取请求生命周期内的注入对象
                            var accessor = AppConfig.serviceProvider.GetRequiredService<IHttpContextAccessor>();
                            var user = new AspNetUser(accessor);

                            #region 插入数据时
                            if (entityInfo.OperationType == DataFilterType.InsertByObject)
                            {
                                switch (entityInfo.PropertyName)
                                {
                                    case "Id":
                                        if (entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue).IsEmpty()) entityInfo.SetValue(Guid.NewGuid().ToString());
                                        break;
                                    case "CreateTime":
                                        entityInfo.SetValue(DateTime.Now);
                                        break;
                                    case "CreateId":
                                        entityInfo.SetValue(user.ID);
                                        break;
                                    case "CreateName":
                                        entityInfo.SetValue(user.Name);
                                        break;
                                }
                            }
                            #endregion

                            #region 更新数据时
                            if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                            {
                                switch (entityInfo.PropertyName)
                                {
                                    case "ModifyId":
                                        entityInfo.SetValue(user.ID);
                                        break;
                                    case "ModifyName":
                                        entityInfo.SetValue(user.Name);
                                        break;
                                    case "ModifyTime":
                                        entityInfo.SetValue(DateTime.Now);
                                        break;
                                }
                            }
                            #endregion
                        },
                        #endregion

                        #region sql，param过滤器
                        OnExecutingChangeSql = (sql, paras) =>
                        {
                            return new KeyValuePair<string, SugarParameter[]>(sql, paras);
                        },
                        #endregion

                        #region SQL执行前
                        OnLogExecuting = (sql, paras) =>
                        {
                            LogHelper.WriteLog("SqlDebug",sql);
                        },
                        #endregion

                        #region SQL执行后
                        OnLogExecuted = (sql, paras) => { },
                        #endregion

                        #region 差异日志
                        OnDiffLogEvent = d => 
                        {
                        },
                        #endregion

                        #region 错误日志
                        OnError = err =>
                        {
                            LogHelper.LogException(err);
                        },
                        #endregion
                    },
                    #endregion

                    #region MoreSettings
                    MoreSettings = new ConnMoreSettings()
                    {
                        //IsWithNoLockQuery = true,
                        IsAutoRemoveDataCache = true
                    },
                    #endregion

                    #region ConfigureExternalServices
                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        EntityNameService = (type, entityInfo) => { },
                        EntityService = (prop, entityInfo) =>
                        {
                            // int?  decimal?这种 isnullable=true 不支持string
                            if ((prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) || new NullabilityInfoContext().Create(prop).WriteState is NullabilityState.Nullable)
                                entityInfo.IsNullable = true;
                            else entityInfo.IsNullable = false;
                        },
                        SqlFuncServices = SqlFuncExt.SqlFuncExtList(),
                        SplitTableService = new SplitTableService()
                    }
                    #endregion
                });
            });
            #endregion

            return new SqlSugarScope(list);
        }
    }
}
