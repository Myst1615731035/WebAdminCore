using ApiModel;
using BaseService;
using System.Linq.Expressions;
using WebModel.Entitys;

namespace WebService.ISystemService
{
    /// <summary>
    /// IDictService
    /// </summary>	
    public partial interface IDictService : IBaseService<Dict>
    {
        Task<Pagination> QueryDicts(Expression<Func<Dict, bool>> expression, Pagination page);
        Task<Dict> QueryDict(Expression<Func<Dict, bool>> expression);
    }
}
