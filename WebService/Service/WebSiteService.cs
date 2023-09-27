using ApiModel;
using SqlSugar;
using WebModel.Entitys;
using WebService.IService;
using System.Reflection;
using WebUtils.BaseService;
using WebUtils;

namespace WebService.Service
{
    /// <summary>
	/// WebSiteService
	/// </summary>
    public partial class WebSiteService : BaseService<WebSite>, IWebSiteService
	{
        public async Task<ContentJson> AddWebSite(WebSite entity)
        {
            var res = new ContentJson("新增站点失败");
            try
            {
                // 重复性判断
                if (await Any(t => t.Idtag == entity.Idtag)) res.msg = $"{res.msg}, 已存在相同关键词的站点, 无法建立数据分表，请检查";
                else
                {
                    BeginTran();
                    var type = typeof(WebSite);
                    var models = type.Assembly.GetTypes()
                                    .Where(t => t.IsClass && t.Namespace == type.Namespace && t.GetCustomAttribute<SplitTableAttribute>() != null)
                                    .ToList();
                    // 创建分表
                    models.ForEach(t =>
                    {
                        var tableName = Db.SplitHelper(t).GetTableName(entity.Idtag);
                        if (!Db.DbMaintenance.IsAnyTable(tableName, false))
                        {
                            Db.MappingTables.Add(t.Name, tableName);
                            Db.CodeFirst.InitTables(t);
                        }
                    });
                    if (await Add(entity) > 0)
                    {
                        CommitTran();
                        res = new ContentJson(true, "添加成功");
                    }
                    else throw new Exception("添加站点信息失败");
                }
                return res;
            }
            catch(Exception ex)
            {
                RollbackTran();
                return new ContentJson(ex.Message);
            }
        }

        public async Task<ContentJson> UpdateWebSite(WebSite entity)
        {
            var res = new ContentJson("更新站点失败");
            try
            {
                if (await Any(t => t.Idtag == entity.Idtag && t.Id != entity.Id)) res.msg = $"{res.msg}，已存在相同关键词的站点，无法重命名表名称";
                else
                {
                    Db.AsTenant().BeginTran();
                    var oldEntity = await QueryById(entity.Id);
                    #region 标记不一样，则更新表名
                    string tempMsg = "";
                    var type = typeof(WebSite);
                    var models = type.Assembly.GetTypes()
                                    .Where(t => t.IsClass && t.Namespace == type.Namespace && t.GetCustomAttribute<SplitTableAttribute>() != null)
                                    .ToList();
                    models.ForEach(t =>
                    {
                        var context = Db.SplitHelper(t);
                        var tableName = context.GetTableName(entity.Idtag);
                        // 重命名数据表
                        if (oldEntity.Idtag != entity.Idtag)
                        {
                            var oldTableName = context.GetTableName(oldEntity.Idtag);
                            if (Db.DbMaintenance.IsAnyTable(oldTableName, false) && Db.DbMaintenance.IsAnyTable(tableName, false)) Db.DbMaintenance.RenameTable(oldTableName, tableName);
                            tempMsg = ", 站点数据更改关键字，分表名称会同时改变，请注意网站中的表名映射";
                        }
                        // 修复数据表结构
                        Db.MappingTables.Add(t.Name, tableName);
                        Db.CodeFirst.InitTables(t);
                    });

                    #endregion
                    if(await Update(entity))
                    {
                        res = new ContentJson(true, "更新成功");
                        Db.AsTenant().CommitTran();
                    }
                    else throw new Exception("更新站点信息失败");

                    Db.AsTenant().CommitTran();
                }
                return res;
            }
            catch(Exception ex)
            {
                Db.AsTenant().RollbackTran();
                LogHelper.LogException(ex);
                return res;
            }
        }
    }
}