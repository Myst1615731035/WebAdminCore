using ApiModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils;
using WebUtils.HttpContextUser;
using Newtonsoft.Json.Linq;
using SqlSugar.Extensions;
using WebModel.Entitys;
using WebService.ISystemService;

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
        private readonly ISysRoleService _service;
        private readonly IUser _user;
        private readonly IRolePermissionService _auth;
        private readonly ISqlSugarClient _db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public SysRoleController(ISysRoleService service, IUser user, IRolePermissionService auth, ISqlSugarClient db)
        {
            _service = service;
            _user = user;
            _auth = auth;
            _db = db;
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
            res.data = await _service.QueryPage(exp.ToExpression(), page);
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
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromBody] SysRole entity)
        {
            //结果定义
            var result = new ContentJson()
            {
                msg = "操作失败",
                success = false,
                data = ""
            };

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

        #region 授权
        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> GetRolePermission([FromBody] JObject data)
        {
            var res = new ContentJson(true, "获取成功");
            var roleId = data["id"].ObjToString();
            if (roleId.IsNotEmpty())
            {
                res = new ContentJson(true, "获取成功", new 
                {
                    permissionTree = await _auth.GetRoleAuthTree(roleId),
                    hasAuthed = await _auth.GetRoleAuthLeafChecked(roleId),
                });
            }
            return res;
        }
        /// <summary>
        /// 保存权限列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> SaveRoleAuth([FromBody] JObject data)
        {
            var result = new ContentJson("授权失败");
            var roleId = data["roleId"].ObjToString();
            var perIds = data["list"].ToList<string>();
            if (roleId.IsNotEmpty())
            {
                if(await _service.Any(t=>t.Id == roleId))
                {
                    var list = new List<RolePermission>();
                    perIds.ForEach(t => list.Add(new RolePermission() { RoleId = roleId, PermissionId = t }));
                    _db.AsTenant().BeginTran();
                    try
                    {
                        await _auth.Delete(t => t.RoleId == roleId);
                        await _auth.Add(list);
                        result = new ContentJson(true, "授权成功");
                        _db.AsTenant().CommitTran();
                    }
                    catch(Exception ex)
                    {
                        LogHelper.LogException(ex);
                        _db.AsTenant().RollbackTran();
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
