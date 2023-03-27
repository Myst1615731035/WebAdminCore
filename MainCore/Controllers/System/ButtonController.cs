using ApiModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils;
using WebUtils.HttpContextUser;
using Microsoft.AspNetCore.Authorization;
using WebModel.SystemEntity;
using WebService.ISystemService;

namespace MainCore.Controllers.System
{
    /// <summary>
    /// ButtonController
    /// </summary>	
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ButtonController : ControllerBase
    {
        #region IOC&DI
        private readonly IButtonService _service;
        private readonly IUser _user;

        public ButtonController(IButtonService service, IUser user)
        {
            _service = service;
            _user = user;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetList([FromBody] Pagination pagination)
        {
            var exp = Expressionable.Create<Button>();
            // 增加查询条件

            return new ContentJson()
            {
                msg = "success",
                success = true,
                data = await _service.QueryPage(exp.ToExpression(), pagination)
            };
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetEntity([FromBody] object id)
        {
            var exp = Expressionable.Create<Button>();
            // 增加查询条件

            var entity = await _service.QueryById(id);
            return new ContentJson()
            {
                msg = "success",
                success = true,
                data = entity
            };
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save([FromBody] Button entity)
        {
            var res = new ContentJson("保存失败");
            if (await _service.Storageable(entity) > 0)
            {
                res = new ContentJson(true, "保存成功");
                res.data = entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null;
            }
            return res;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromBody] Button entity)
        {
            //结果定义
            var res = new ContentJson("保存失败");
            if (entity.Id.IsNotEmpty())
            {
                if (await _service.Delete(entity))
                {
                    res.msg = "数据已删除";
                    res.success = true;
                }
            }
            return res;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> DeleteById([FromBody] object Id)
        {
            //结果定义
            var res = new ContentJson("保存失败");
            if (Id.IsNotEmpty())
            {
                if (await _service.DeleteById(Id))
                {
                    res.msg = "数据已删除";
                    res.success = true;
                }
            }
            return res;
        }
        #endregion
    }
}