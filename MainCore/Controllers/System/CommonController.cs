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
            return new ContentJson(true, "获取成功", new
            {
                // Dict
                dict = await _db.Queryable<Dict>().Select(t => new { key = t.Code, items = t.Items }).ToListAsync()
            });
        }
    }
}
