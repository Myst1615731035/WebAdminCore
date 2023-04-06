using ApiModel;
using BaseRepository;
using BaseService;
using SqlSugar;
using System.Linq.Expressions;
using WebUtils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.AccessControl;
using WebModel.Entitys;
using WebService.ISystemService;

namespace WebService.SystemService
{
    /// <summary>
    /// DictService
    /// </summary>	
    public partial class DictService : BaseService<Dict>, IDictService
    {
        IBaseRepository<Dict> dal;
        ISqlSugarClient db;
        public DictService(IBaseRepository<Dict> _dal)
        {
            dal = _dal;
            db = dal.Db;
        }

        public async Task<Pagination> QueryDicts(Expression<Func<Dict, bool>> expression, Pagination page)
        {
            RefAsync<int> totalCount = 0;
            var query = DictQuery(expression);
            page.response = await query.ToPageListAsync(page.currentPage, page.pageSize, totalCount);
            int pageCount = Math.Ceiling(totalCount.ObjToDecimal() / page.pageSize.ObjToDecimal()).ObjToInt();
            page.total = totalCount;
            page.pageCount = pageCount;
            return page;
        }

        public async Task<Dict> QueryDict(Expression<Func<Dict, bool>> expression)
        {
            return await DictQuery(expression).FirstAsync();
        }

        private ISugarQueryable<Dict> DictQuery(Expression<Func<Dict, bool>> expression)
        {
            return db.Queryable<Dict>()
                 .WhereIF(expression.IsNotEmpty(), expression)
                 .Where(t => !t.IsDelete)
                 .Select(t => new Dict
                 {
                     Id = t.Id,
                     Name = t.Name,
                     Code = t.Code,
                     Description = t.Description,
                     Items = SqlFunc.Subqueryable<DictItem>().Where(i => i.Pid == t.Id).ToList()
                 });
        }
    }
}
