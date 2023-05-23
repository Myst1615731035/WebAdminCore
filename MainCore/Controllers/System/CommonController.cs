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
                {"productClass",null },
                { "products", null },
                {"warehouse",null },
                {"users",null },
            };

            await _db.ThenMapperAsync(data, async t =>
            {
                data[t.Key] = t.Key switch
                {
                    "dict" => await _db.Queryable<Dict>().Select(t => new { key = t.Code, items = t.Items }).ToListAsync(),
                    "productClass"=> await _db.Queryable<ProductClass>().Select(t => new Option{ Value = t.Id, Label = t.Name }).ToListAsync(),
                    "products"=> await _db.Queryable<Product>().Select(t => new Option { Value = t.Id, Label = t.Name }).ToListAsync(),
                    "warehouse" => await _db.Queryable<Warehouse>().Select(t => new Option { Value = t.Id, Label = t.Name }).ToListAsync(),
                    "users"=> await _db.Queryable<SysUser>().Select(t => new Option { Value = t.Id, Label = t.Name }).ToListAsync(),
                    _ => new List<object>(),
                };
            });

            return new ContentJson(true, "获取成功", data);
        }
    }
}
