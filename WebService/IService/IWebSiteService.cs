using ApiModel;
using WebModel.Entitys;
using WebUtils.BaseService;

namespace WebService.IService
{
    /// <summary>
	/// IWebSiteService
	/// </summary>
    public interface IWebSiteService : IBaseService<WebSite>
    {
        Task<ContentJson> AddWebSite(WebSite entity);
        Task<ContentJson> UpdateWebSite(WebSite entity);
    }
}
