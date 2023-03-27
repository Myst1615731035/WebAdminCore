using ApiModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils.HttpContextUser;
using WebUtils;
using Microsoft.AspNetCore.Authorization;
using WebModel.SystemEntity;
using WebService.ISystemService;

namespace MainCore.Controllers.System
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PermissionController : ControllerBase
    {
        #region IOC&DI
        private IMenuService _service;
        private IButtonService _button;
        private IUser _user;
        public PermissionController(IMenuService services, IUser user, IButtonService button)
        {
            _service = services;
            _user = user;
            _button = button;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetTree(Pagination? page)
        {
            var exp = Expressionable.Create<Menu>();
            // 增加查询条件
            if (page.IsNotEmpty() &&  page.keyword.IsNotEmpty())
                exp = exp.And(t => t.Name.Contains(page.keyword) || t.Description.Contains(page.keyword));

            return new ContentJson()
            {
                msg = "获取成功",
                success = true,
                data = await _service.QueryTree(t => t.Children, t => t.Pid, "", exp.ToExpression(), t => t.Sort)
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
            var exp = Expressionable.Create<SysUser>();
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
        /// 新增或更新菜单实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save([FromBody] Menu entity)
        {
            //结果定义
            var result = new ContentJson("保存失败");
            #region 异常处理
            if (await _service.Any(t => t.Name == entity.Name && t.Pid == entity.Pid && t.Id != entity.Id))
            {
                result.msg = $"该上级目录下已存在\"{entity.Name}\"的同名目录/页面,请确认";
                return result;
            }
            var parent = await _service.QueryById(entity.Pid);
            if (parent.IsNotEmpty() && parent.Type == 1)
            {
                result.msg = "只允许在目录下添加目录/页面, 请确认上级目录选择是否正确";
                return result;
            }
            #endregion

            if (await _service.Storageable(entity) > 0)
            {
                result = new ContentJson(true, "保存成功");
                result.data = entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null;
            }
            return result;
        }
        /// <summary>
        /// 保存为按钮
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> SaveBtn([FromBody] Button btn)
        {
            var res = new ContentJson("保存失败");
            // 异常情况：
            // 1. 按钮的归属页面为空
            if (btn.Mid.IsEmpty() || !await _service.Any(t => t.Id == btn.Mid)) res.msg = $"{res.msg}，按钮的归属页面未知，无法保存";
            // 2. 同一个页面下，存在相同编码，相同名称，相同方法的按钮
            if (btn.Id.IsNotEmpty() && await _button.Any(t => t.Mid == btn.Mid && t.Id != btn.Id &&
            ((!SqlFunc.IsNullOrEmpty(t.Name) && t.Name == btn.Name) || (!SqlFunc.IsNullOrEmpty(t.Code) && t.Code == btn.Code) || (!SqlFunc.IsNullOrEmpty(t.Function) && t.Function == btn.Function))))
                res.msg = $"{res.msg}，按钮的归属页面未知，无法保存";

            if (await _button.Storageable(btn) > 0)
            {
                res = new ContentJson(true, "保存成功");
                res.data = btn.Id.IsNotEmpty() ? await _service.QueryById(btn.Id) : null;
            }
            return res;
        }
        /// <summary>
        /// 保存列表排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> SaveSort([FromBody] List<Menu> list)
        {
            var res = new ContentJson("更新失败");
            if (list.IsNotEmpty() && list.Count > 0)
            {
                if (await _service.Update(list, t => new { t.Pid, t.Sort })) res = new ContentJson(true, "更新成功");
            }
            return res;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromBody] Menu entity)
        {
            //结果定义
            var result = new ContentJson("删除失败");
            #region
            if (await _service.Any(t => t.Pid == entity.Id && !t.IsDelete))
            {
                result.msg = "该目录下仍然存在未删除的目录或页面,无法删除此目录";
                return result;
            }
            #endregion
            // Delete
            if (entity.Id.IsNotEmpty())
            {
                if (await _service.Delete(entity))
                {
                    result.msg = "数据已删除";
                    result.success = true;
                }

            }
            return result;
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
            var result = new ContentJson()
            {
                msg = "操作失败",
                success = false,
                data = ""
            };

            // Delete
            if (Id.IsNotEmpty())
            {
                if (await _service.DeleteById(Id))
                {
                    result.msg = "数据已删除";
                    result.success = true;
                }
            }
            return result;
        }
        #endregion
    }
}
