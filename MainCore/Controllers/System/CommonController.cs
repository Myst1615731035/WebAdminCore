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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ContentJson> Test()
        {
            return new ContentJson();
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetCache()
        {
            var res = new ContentJson(true, "获取成功");

            #region Dict
            var dict = await _db.Queryable<Dict>()
            .Select(t => new
            {
                key = t.Code,
                items = SqlFunc.Subqueryable<DictItem>()
                            .Where(i => i.Pid == t.Id)
                            .OrderBy(i => i.Sort)
                            .ToList(i => new Option() { label = i.Label, enLabel = i.EnLabel, value = i.Value })
            }).ToListAsync();
            #endregion

            res.data = new { dict };

            return res;
        }
    }
}
