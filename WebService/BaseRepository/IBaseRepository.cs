using ApiModel;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System.Data;
using System.Linq.Expressions;

namespace BaseRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        ISqlSugarClient Db { get; }

        #region 单表方法
        #region 查
        Task<TEntity> First(Expression<Func<TEntity, bool>> whereExpression);
        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryByIdWithCache(object objId, bool blnUseCache = false);
        /// <summary>
        /// 根据id数组查询实体list
        /// </summary>
        /// <param name="lstIds"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryByIDs(object[] lstIds);
        #endregion

        #region 树查询
        Task<List<TEntity>> QueryTree(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null);
        #endregion

        #region 增
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> Add(TEntity model);

        /// <summary>
        /// (雪花)写入实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<long> AddLong(TEntity entity);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TEntity> AddEntity(TEntity model);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        Task<int> Add(List<TEntity> listEntity);

        /// <summary>
        /// 批量插入实体(雪花)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<long>> AddLong(List<TEntity> list);
        #endregion

        #region 改
        /// <summary>
        /// 更新model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity model);
        Task<bool> Update(List<TEntity> model, Expression<Func<TEntity, object>> columns = null);

        /// <summary>
        /// 根据model，更新，带where条件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(object operateAnonymousObjects);

        /// <summary>
        /// 根据model，更新，指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateCols = null, Expression<Func<TEntity, object>> ignoreCols = null, Expression<Func<TEntity, object>> whereCols = null);
        #endregion

        #region 增 改 一体
        Task<int> Storageable(TEntity entity);
        Task<int> Storageable(List<TEntity> entity);
        #endregion

        #region 删
        /// <summary>
        /// 根据id 删除某一实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteById(object id);

        /// <summary>
        /// 根据对象，删除某一实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Delete(TEntity model);

        /// <summary>
        /// 根据列表，删除实体集合
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Delete(List<TEntity> list);

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> Delete(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 根据id数组，删除实体list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIds(object[] ids);
        #endregion
        #endregion

        #region 高级查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> Query();

        /// <summary>
        /// 带sql where查询
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<List<TEntity>> Query(string strWhere);

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 根据表达式，指定返回对象模型，查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 根据表达式，指定返回对象模型，排序，查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="whereExpression"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression);
        Task<List<TEntity>> Query(string strWhere, string strOrderByFileds);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds);
        Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds);
        Task<List<TEntity>> QuerySql(string strSql, SugarParameter[] parameters = null);
        Task<DataTable> QueryTable(string strSql, SugarParameter[] parameters = null);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds);
        Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<Pagination> QueryPage(Expression<Func<TEntity, bool>> expression, Pagination page);

        /// <summary>
        /// 三表联查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="joinExpression"></param>
        /// <param name="selectExpression"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
        #endregion

        #region 分表方法
        #region 查询
        ISugarQueryable<TEntity> SplitQuerable(string split, Expression<Func<TEntity, bool>> expression = null);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        Task<Pagination> QueryPageSplit(Expression<Func<TEntity, bool>> expression, Pagination page, string split);
        /// <summary>
        /// 实体查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        Task<TEntity> QuerySplit(Expression<Func<TEntity, bool>> expression, string split);
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryListSplit(Expression<Func<TEntity, bool>> expression, string split);
        Task<List<TEntity>> QueryTreeSplit(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, string split, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null);
        #endregion

        Task<int> StorageableSplit(TEntity entity, string split);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        Task<bool> UpdateSplit(TEntity entity, string split);
        Task<bool> UpdateSplit(List<TEntity> list, string split, Expression<Func<TEntity, object>> columns = null);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        Task<bool> DeleteSplit(Expression<Func<TEntity, bool>> expression, string split);
        #endregion

        #region 聚合查询
        /// <summary>
        /// Any判断
        /// </summary>
        /// <summary>
        Task<bool> Any(string split = null);
        /// Any判断
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> whereExpression, string split = null);
        Task<T> Min<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        Task<T> Max<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        Task<int> Count(Expression<Func<TEntity, bool>> whereExpression = null, string split= null);
        Task<T> Sum<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null);
        #endregion
    }
}
