using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System.IO;
using System.Reflection;
using WebUtils;
using WebUtils.Attributes;

namespace MainCore.Controllers
{
    /// <summary>
    /// 用于生成数据种子
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class MigrateController : ControllerBase
    {
        #region IOC&DI
        public readonly ISqlSugarClient _db;
        public MigrateController(ISqlSugarClient db)
        {
            _db = db;
        }
        #endregion

        #region 生成数据种子
        /// <summary>
        /// 系统基础数据导出
        /// </summary>
        /// <returns></returns>
        public async Task<ContentJson> SaveDataSeed()
        {
            var res = new ContentJson("生成失败");
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            try
            {
                /// 获取权限基础类所属的程序集
                var modelDll = Path.Combine(AppContext.BaseDirectory, "WebModel.dll");
                var modelAssembly = Directory.GetFiles(AppContext.BaseDirectory, "WebModel.dll").Select(Assembly.LoadFrom).ToArray();
                // 项目框架实体类
                var models = modelAssembly.SelectMany(a => a.DefinedTypes)
                                .Select(type => type.AsType())
                                .Where(x => x.IsClass && x.Namespace != null && x.Namespace.Equals("WebModel.Entitys") && x.GetCustomAttribute<SystemAuthTable>().IsNotEmpty())
                                .ToList();
                // 对实体类进行循环获取
                models.ForEach(async t =>
                {
                    /// 查询未删除的数据
                    var data = await _db.Queryable<dynamic>().AsType(t).Where("IsDelete = 0").ToListAsync();

                    FileHelper.WriteFile(Path.Combine(AppConfig.ContentRootPath, "DataSeedJson", $"{t.Name}.json"), data.ToJson(jsonSetting));
                });
                res = new ContentJson(true, "生成成功");
            }
            catch (Exception ex)
            {
                res.msg = $"{res.msg}, {ex.Message}";
            }
            return res;
        }
        #endregion
    }
}
