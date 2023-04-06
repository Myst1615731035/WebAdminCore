using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.Extensions;
using WebModel.Entitys;
using WebService.ISystemService;
using WebUtils;
using WebUtils.HttpContextUser;

namespace MainCore.Controllers.System
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        #region 注入
        private IDictService _service;
        private IDictItemService _itemService;
        private IUser _user;
        private ISqlSugarClient _db;
        public DictionaryController(IDictService services, IUser user, IDictItemService itemService, ISqlSugarClient db)
        {
            _service = services;
            _user = user;
            _itemService = itemService;
            _db = db;
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
            res.data = await _service.QueryPage(exp.ToExpression(), page);
            return res;
        }

        [HttpPost]
        public async Task<ContentJson> GetItemList(Pagination page)
        {
            var res = new ContentJson(true);
            var exp = Expressionable.Create<DictItem>();
            if(page.form.IsNotEmpty())
            {
                if (page.form["Id"].IsNotEmpty())
                {
                    exp = exp.And(t=>t.Pid == page.form["Id"].ObjToString());
                }
            }

            res.data = await _itemService.QueryPage(exp.ToExpression(), page);
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
            _db.AsTenant().BeginTran();
            try
            {
                //重复判断
                if (await _db.Queryable<Dict>().Where(t => t.Name == entity.Name && t.Id != entity.Id).AnyAsync())
                {
                    res.msg = "已存在相同名称的字典项";
                }
                else
                {
                    entity.Id = entity.Id.IsEmpty() ? Guid.NewGuid().ToString() : entity.Id;
                    if (await _db.Storageable(entity).WhereColumns(t => t.Id).ExecuteCommandAsync() > 0)
                    {
                        if (entity.Items.Count > 0)
                        {
                            entity.Items.ForEach(t => t.Pid = entity.Id);
                            if (await _db.Storageable(entity.Items).WhereColumns(t => t.Id).ExecuteCommandAsync() > 0)
                                _db.AsTenant().CommitTran();
                            else
                                throw new Exception("提交数据保存失败");
                        }
                        else
                        {
                            _db.AsTenant().CommitTran();
                        }

                        res = new ContentJson(true, "保存成功");
                        res.data = await _service.QueryDict(t => t.Id == entity.Id);
                    }
                    else
                        throw new Exception("提交数据保存失败");
                }
            }
            catch(Exception ex)
            {
                _db.AsTenant().RollbackTran();
                res.msg = ex.Message;
                LogHelper.LogException(ex);
            }
            return res;
        }
        #endregion
    }
}
