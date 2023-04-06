using ApiModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebUtils;
using WebUtils.HttpContextUser;
using SqlSugar.Extensions;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebModel.Entitys;
using WebService.ISystemService;

namespace MainCore.Controllers
{
    /// <summary>
    /// SysController
    /// </summary>	
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SysUserController : ControllerBase
    {
        #region 接口构造
        private readonly ISysUserService _service;
        private readonly IUser _user;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public SysUserController(ISysUserService service, IUser user)
        {
            _service = service;
            _user = user;
        }
        #endregion

        #region 用户信息
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ContentJson> GetInfoByToken([FromBody] dynamic param)
        {
            var res = new ContentJson("用户信息获取失败");
            string token = param.token;
            if (token.IsNotEmpty())
            {
                var model = JwtHelper.SerializeJwt(token);
                if (model.IsNotEmpty() && model.Uid.IsNotEmpty())
                {
                    var user = await _service.GetUserInfo(model.Uid.ObjToString());
                    if (user.IsNotEmpty())
                    {
                        res = new ContentJson()
                        {
                            success = true,
                            msg = "获取成功",
                            data = user
                        };
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取用户权限数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ContentJson> GetUserAuth()
        {
            var res = new ContentJson("用户权限获取失败");
            if (_user.ID.IsNotEmpty())
            {
                res = new ContentJson()
                {
                    success = true,
                    msg = "获取成功",
                    data = await _service.GetUserAuth(_user.ID)
                };
            }
            return res;
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
            var exp = Expressionable.Create<SysUser>();
            // 增加查询条件
            if (page.keyword.IsNotEmpty()) exp = exp.And(t => t.Name.Contains(page.keyword) || t.Account.Contains(page.keyword) || t.Email.Contains(page.keyword));
            return new ContentJson()
            {
                msg = "success",
                success = true,
                data = await _service.GetUserList(exp.ToExpression(), page)
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
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Save([FromBody] SysUser entity)
        {
            var res = new ContentJson("保存失败");
            // 新增
            if (entity.Id.IsEmpty()) entity.Password = MD5Helper.MD5Encrypt32(entity.Password);

            // 更新数据
            if (entity.Id.IsNotEmpty())
            {
                // 获取原用户信息
                var user = await _service.QueryById(entity.Id);
                // 处理新密码
                if (entity.NewPassword.IsNotEmpty())
                {
                    var password = MD5Helper.MD5Encrypt32(entity.Password);
                    if (!await _service.Any(t => t.Id == entity.Id && t.Password == password))
                    {
                        res.msg = "原始密码验证错误，请确认";
                        return res;
                    }
                }
                // 处理其他数据
                entity.Password = MD5Helper.MD5Encrypt32(entity.NewPassword);
                entity = user.UpdateProp(entity);
            }
            
            if (await _service.SaveUser(entity))
            {
                res = new ContentJson(true, "保存成功");
                res.data = entity.Id.IsNotEmpty() ? await _service.QueryById(entity.Id) : null;
            }
            return res;
        }

        [HttpGet]
        public async Task<ContentJson> ReSetPassword(string Id)
        {
            var res = new ContentJson("无效用户");
            var user = await _service.QueryById(Id);
            if(user.IsNotEmpty() && user.Id.IsNotEmpty())
            {
                var newPw = RandomHelper.GetRandomString(12);
                user.Password = MD5Helper.MD5Encrypt32(newPw);
                if (await _service.Update(user)) res = new ContentJson(true, "重置成功", newPw);
                else res.msg = "重置失败";
            }
            return res;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentJson> Delete([FromBody] SysUser entity)
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

            // Delete
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
