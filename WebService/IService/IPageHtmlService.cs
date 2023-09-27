using WebModel.Entitys;
using WebUtils.BaseService;

namespace WebService.IService
{
    /// <summary>
	/// IPageHtmlService
	/// </summary>
    public interface IPageHtmlService : IBaseService<PageHtml>
    {
        Task<bool> SavePage(PageHtml entity, string idtag, Navigation? navigation = null);
    }
}
