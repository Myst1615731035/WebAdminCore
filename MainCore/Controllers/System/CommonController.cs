using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebModel.Entitys;

namespace MainCore.Controllers.System
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommonController : ControllerBase
    {
        #region 注入
        public readonly ISqlSugarClient _db;
        public CommonController(ISqlSugarClient db)
        {
            _db = db;
        }
        #endregion

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetCache()
        {
            var data = new Dictionary<string, object?>
            {
                {"dict", null },
                { "area", null }
            };

            await _db.ThenMapperAsync(data, async t =>
            {
                data[t.Key] = t.Key switch
                {
                    "dict" => await _db.Queryable<Dict>().Select(t => new { key = t.Code, items = t.Items }).ToListAsync(),
                    "area" => await _db.Queryable<Area>().Select(t => new Option{ Label = t.Name, Value = t.Id }).ToListAsync(),
                    _ => new List<object>(),
                };
            });

            return new ContentJson(true, "获取成功", data);
        }
    }
}
