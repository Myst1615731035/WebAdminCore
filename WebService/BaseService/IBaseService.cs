using ApiModel;
using BaseRepository;
using SqlSugar;
using System.Data;
using System.Linq.Expressions;

namespace BaseService
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        #region 单表方法
        #region 查
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] lstIds);
        #endregion

        #region 树查询
        Task<List<TEntity>> QueryTree(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null);
        #endregion

        #region 增
        Task<int> Add(TEntity model);
        Task<TEntity> AddEntity(TEntity model);

        Task<int> Add(List<TEntity> listEntity);
        #endregion

        #region 改
        Task<bool> Update(TEntity model);
        Task<bool> Update(List<TEntity> model, Expression<Func<TEntity, object>> columns = null);
        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(object operateAnonymousObjects);
        Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateCols = null, Expression<Func<TEntity, object>> ignoreCols = null, Expression<Func<TEntity, object>> whereCols = null);
        #endregion

        #region 增改一体
        Task<int> Storageable(TEntity entity);
        Task<int> Storageable(List<TEntity> entity);
        #endregion

        #region 删
        Task<bool> DeleteById(object id);

        Task<bool> Delete(TEntity model);

        Task<bool> Delete(List<TEntity> list);
        Task<int> Delete(Expression<Func<TEntity, bool>> expression);

        Task<bool> DeleteByIds(object[] ids);
        #endregion
        #endregion 

        #region 高级查询
        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string strWhere);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression);
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(string strWhere, string strOrderByFileds);
        Task<List<TEntity>> QuerySql(string strSql, SugarParameter[] parameters = null);
        Task<DataTable> QueryTable(string strSql, SugarParameter[] parameters = null);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds);
        Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds);
        Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds);
        Task<Pagination> QueryPage(Expression<Func<TEntity, bool>> expression, Pagination page);
        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
        #endregion

        #region 分表方法
        ISugarQueryable<TEntity> SplitQuerable(string split, Expression<Func<TEntity, bool>> expression = null);
        Task<Pagination> QueryPageSplit(Expression<Func<TEntity, bool>> expression, Pagination page, string split);
        Task<TEntity> QuerySplit(Expression<Func<TEntity, bool>> expression, string split);
        Task<List<TEntity>> QueryListSplit(Expression<Func<TEntity, bool>> expression, string split);
        Task<List<TEntity>> QueryTreeSplit(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, string split, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null);
        Task<int> StorageableSplit(TEntity entity, string split);
        Task<bool> UpdateSplit(TEntity entity, string split);
        Task<bool> UpdateSplit(List<TEntity> list, string split, Expression<Func<TEntity,object>> columns = null);
        Task<bool> DeleteSplit(Expression<Func<TEntity, bool>> expression, string split);

        #endregion

        #region 聚合查询
        Task<bool> Any(string split = null);
        Task<bool> Any(Expression<Func<TEntity, bool>> whereExpression, string split = null);
        Task<T> Min<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        Task<T> Max<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        Task<int> Count(Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        Task<T> Sum<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        #endregion
    }
}
