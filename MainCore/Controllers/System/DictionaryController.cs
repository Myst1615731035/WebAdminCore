using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebModel.Entitys;
using WebUtils;
using WebUtils.BaseService;
using WebUtils.HttpContextUser;

namespace MainCore.Controllers.System
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        #region 注入
        private IBaseService<Dict> _service;
        private IUser _user;
        public DictionaryController(IBaseService<Dict> services, IUser user)
        {
            _service = services;
            _user = user;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取字典数据列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetList(Pagination page)
        {
            var res = new ContentJson(true);
            var exp = Expressionable.Create<Dict>();
            if(page.keyword.IsNotEmpty()) 
                exp = exp.And(t=>t.Name.Contains(page.keyword)||t.Code.Contains(page.keyword) || t.Description.Contains(page.keyword));
            res.data = await _service.QueryPage(page, exp.ToExpression());
            return res;
        }

        /// <summary>
        /// 获取字典数据实体
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetEntity(object? id)
        {
            var res = new ContentJson(true);
            res.data = await _service.Query(t=> t.Id == id);
            return res;
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 获取字典数据实体
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save(Dict entity)
        {
            var res = new ContentJson("保存失败");
            //重复判断
            if (await _service.Db.Queryable<Dict>().Where(t => (t.Name == entity.Name || t.Code == entity.Code) && t.Id != entity.Id).AnyAsync()) res.msg = "已存在相同名称或编码的字典项";
            else
            {
                if(entity.Items.IsNotEmpty()) entity.Items = entity.Items?.OrderBy(t => t.Sort).ToList();
                if (await _service.Storageable(entity) > 0) res = new ContentJson(true, "保存成功", entity);
            }
            return res;
        }

        /// <summary>
        /// 获取字典数据实体
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete(string Id)
        {
            var res = new ContentJson("删除失败");
            if(await _service.DeleteById(Id)) res = new ContentJson(true, "删除成功");
            return res;
        }
        #endregion
    }
}
