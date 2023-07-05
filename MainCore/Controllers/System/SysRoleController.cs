using ApiModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils;
using WebUtils.HttpContextUser;
using Newtonsoft.Json.Linq;
using SqlSugar.Extensions;
using WebModel.Entitys;
using WebService.IService;
using WebUtils.BaseService;

namespace MainCore.Controllers
{
    /// <summary>
    /// SysController
    /// </summary>	
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SysRoleController : ControllerBase
    {
        #region IOC&DI
        private readonly IBaseService<SysRole> _service;
        private readonly IBaseService<RolePermission> _auth;
        private readonly IBaseService<Menu> _menu;
        private readonly IUser _user;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public SysRoleController(IBaseService<SysRole> service, IBaseService<Menu> menu, IUser user, IBaseService<RolePermission> auth)
        {
            _service = service;
            _menu = menu;
            _user = user;
            _auth = auth;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetList([FromBody] Pagination page)
        {
            var res = new ContentJson(true);
            var exp = Expressionable.Create<SysRole>();
            // 增加查询条件
            if (page.keyword.IsNotEmpty())
                exp = exp.And(t => t.Name.Contains(page.keyword) || t.Description.Contains(page.keyword));
            res.data = page.isOption ?
                await _service.QueryPage(page, t => new SysRole { Id = t.Id, Name = t.Name }, exp.ToExpression())
                : await _service.QueryPage(page, exp.ToExpression());
            return res;
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
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save([FromBody] SysRole entity)
        {
            //结果定义
            var res = new ContentJson("保存失败");
            if(await _service.Storageable(entity) > 0)
            {
                res = new ContentJson(true, "保存成功");
                res.data = entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null;
            }
            return res;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromQuery] string Id)
        {
            //结果定义
            var res = new ContentJson("删除失败");
            var entity = await _service.QueryById(Id);
            if (entity.IsNotEmpty())
            {
                //entity.IsDelete = true;
                if (await _service.Delete(entity)) res = new ContentJson(true, "数据已删除");
            }
            return res;
        }
        #endregion

        #region 授权
        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetRolePermission([FromQuery] string roleId)
        {
            var res = new ContentJson("该角色权限数据获取失败", new List<string>());
            if (roleId.IsNotEmpty()) res = new ContentJson(true, "获取成功", await _auth.Db.Queryable<RolePermission>().Where(t => t.RoleId == roleId).Select(t => t.PermissionId).ToListAsync());
            return res;
        }
        /// <summary>
        /// 保存权限列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> SaveRoleAuth([FromBody] List<string> data, [FromQuery] string roleId)
        {
            var res = new ContentJson("授权失败");
            if (data.IsEmpty()) res.msg = "未获取到授权的权限信息";
            if (roleId.IsNotEmpty() && await _service.Any(t => t.Id == roleId))
            {
                var list = new List<RolePermission>();
                data.ForEach(t => list.Add(new RolePermission { RoleId = roleId, PermissionId = t }));
                _service.BeginTran();
                try
                {
                    await _auth.Delete(t => t.RoleId == roleId);
                    if(list.Count > 0) await _auth.Insert(list);
                    _service.CommitTran();
                    res = new ContentJson(true, "授权成功");
                }
                catch (Exception ex)
                {
                    LogHelper.LogException(ex);
                    _service.RollbackTran();
                }
            }
            else res.msg = "无效的角色信息";
            return res;
        }
        #endregion
    }
}
