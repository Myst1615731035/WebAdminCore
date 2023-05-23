using ApiModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils.HttpContextUser;
using WebUtils;
using Microsoft.AspNetCore.Authorization;
using WebModel.Entitys;
using WebUtils.BaseService;

namespace MainCore.Controllers.System
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PermissionController : ControllerBase
    {
        #region IOC&DI
        private IBaseService<Menu> _service;
        private IBaseService<Button> _button;
        private IUser _user;
        public PermissionController(IBaseService<Menu> service, IUser user, IBaseService<Button> button)
        {
            _service = service;
            _button = button;
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
        public async Task<ContentJson> GetTree(Pagination? page)
        {
            var exp = Expressionable.Create<Menu>();
            // 增加查询条件
            var list = new List<Menu>();
            if (page.IsNotEmpty() && page.keyword.IsNotEmpty())
            {
                exp = exp.And(t => t.Name.Contains(page.keyword) || t.Description.Contains(page.keyword));
                list = await _service.Query(exp.ToExpression(), t => t.Sort);
            }
            else list = await _service.QueryTree(t => t.Children, t => t.Pid, "", null, t => t.Sort);

            return new ContentJson()
            {
                msg = "获取成功",
                success = true,
                data = list ?? new List<Menu>()
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
            if (await _service.Any(t=>t.Id == entity.Pid && t.Type == 1))
            {
                result.msg = "只允许在目录下添加目录/页面, 请确认上级目录选择是否正确";
                return result;
            }
            #endregion

            _service.BeginTran();
            if (await _service.Storageable(entity) > 0 && await _button.Storageable(entity.Buttons) > 0)
            {
                _service.CommitTran();
                result = new ContentJson(true, "保存成功");
                result.data = entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null;
            }else _service.RollbackTran();
            return result;
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
