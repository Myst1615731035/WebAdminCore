using WebUtils;
using WebModel.Entitys;
using WebUtils.BaseService;
using WebService.IService;

namespace WebService.Service
{
    /// <summary>
	/// PageHtmlService
	/// </summary>
    public partial class PageHtmlService : BaseService<PageHtml>, IPageHtmlService
    {
        public async Task<bool> SavePage(PageHtml entity, string idtag, Navigation? nav = null)
        {
            if (nav.IsNotEmpty())
            {
                try
                {
                    Db.Ado.BeginTran();
                    nav.Id = nav.Id.IsEmpty()? Guid.NewGuid().ToString() : nav.Id;
                    entity.Navid = nav.Id;
                    string navTableName = Db.SplitHelper<Navigation>().GetTableName(idtag), 
                            pageTableName = Db.SplitHelper<PageHtml>().GetTableName(idtag);
                    if (await Db.Insertable(nav).AS(navTableName).ExecuteCommandAsync() > 0 && await Db.Insertable(entity).AS(pageTableName).ExecuteCommandAsync() > 0)
                    {
                        Db.Ado.CommitTran();
                        return true;
                    }
                    else throw new Exception("同时保存导航与页面内容失败");
                }
                catch (Exception ex)
                {
                    Db.Ado.RollbackTran();
                    LogHelper.LogException(ex);
                    return false;
                }
            }
            else return await StorageableSplit(entity, idtag) > 0;
        }
    }
}