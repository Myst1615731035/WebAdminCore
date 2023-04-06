using ApiModel;
using BaseRepository;
using NPOI.POIFS.Storage;
using SqlSugar;
using System.Data;
using System.Linq.Expressions;
using WebUtils;

namespace BaseService
{
    public class BaseService<TEntity>: IBaseService<TEntity> where TEntity : class, new()
    {
        #region 构造实例
        public IBaseRepository<TEntity> BaseDal { get; set; }//通过在子类的构造函数中注入，这里是基类，不用构造函数
        #endregion

        #region 单表方法
        #region 查询
        public async Task<TEntity> QueryById(object objId)
        {
            return await BaseDal.QueryById(objId);
        }
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId, bool blnUseCache = false)
        {
            return await BaseDal.QueryByIdWithCache(objId, blnUseCache);
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIDs(object[] lstIds)
        {
            return await BaseDal.QueryByIDs(lstIds);
        }
        #endregion

        #region 树查询
        public async Task<List<TEntity>> QueryTree(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            return await BaseDal.QueryTree(childExp, parentExp, rootValue, whereExp, orderExp);
        }
        #endregion

        #region 增加
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity)
        {
            return await BaseDal.Add(entity);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<long> AddLong(TEntity entity)
        {
            return await BaseDal.AddLong(entity);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<TEntity> AddEntity(TEntity entity)
        {
            return await BaseDal.AddEntity(entity);
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> Add(List<TEntity> listEntity)
        {
            return await BaseDal.Add(listEntity);
        }

        /// <summary>
        /// 批量插入实体(雪花)
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<List<long>> AddLong(List<TEntity> listEntity)
        {
            return await BaseDal.AddLong(listEntity);
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity)
        {
            return await BaseDal.Update(entity);
        }
        public async Task<bool> Update(List<TEntity> list, Expression<Func<TEntity, object>> columns = null)
        {
            return await BaseDal.Update(list, columns);
        }
        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await BaseDal.Update(entity, strWhere);
        }
        public async Task<bool> Update(object operateAnonymousObjects)
        {
            return await BaseDal.Update(operateAnonymousObjects);
        }

        /// <summary>
        /// 更新部分列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateCols = null, Expression<Func<TEntity, object>> ignoreCols = null, Expression<Func<TEntity, object>> whereCols = null)
        {
            return await BaseDal.Update(entity, updateCols, ignoreCols, whereCols);
        }
        #endregion

        #region 增改一体
        public async Task<int> Storageable(TEntity entity)
        {
            return await BaseDal.Storageable(entity);
        }

        public async Task<int> Storageable(List<TEntity> entity)
        {
            return await BaseDal.Storageable(entity);
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity entity)
        {
            return await BaseDal.Delete(entity);
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete(List<TEntity> list)
        {
            return await BaseDal.Delete(list);
        }


        public async Task<int> Delete(Expression<Func<TEntity, bool>> expression)
        {
            return await BaseDal.Delete(expression);
        }
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            return await BaseDal.DeleteById(id);
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await BaseDal.DeleteByIds(ids);
        }
        #endregion
        #endregion

        #region 高级查询
        /// <summary>
        /// 功能描述:查询所有数据
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query()
        {
            return await BaseDal.Query();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere)
        {
            return await BaseDal.Query(strWhere);
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await BaseDal.Query(whereExpression);
        }

        /// <summary>
        /// 功能描述:按照特定列查询数据列表
        /// 作　　者:CommonApi
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return await BaseDal.Query(expression);
        }

        /// <summary>
        /// 功能描述:按照特定列查询数据列表带条件排序
        /// 作　　者:CommonApi
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="whereExpression">过滤条件</param>
        /// <param name="expression">查询实体条件</param>
        /// <param name="strOrderByFileds">排序条件</param>
        /// <returns></returns>
        public async Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await BaseDal.Query(expression, whereExpression, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByExpression">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression)
        {
            return await BaseDal.Query(whereExpression, orderByExpression);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await BaseDal.Query(whereExpression, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFileds)
        {
            return await BaseDal.Query(strWhere, strOrderByFileds);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>泛型集合</returns>
        public async Task<List<TEntity>> QuerySql(string strSql, SugarParameter[] parameters = null)
        {
            return await BaseDal.QuerySql(strSql, parameters);

        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(string strSql, SugarParameter[] parameters = null)
        {
            return await BaseDal.QueryTable(strSql, parameters);

        }
        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await BaseDal.Query(whereExpression, intTop, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds)
        {
            return await BaseDal.Query(strWhere, intTop, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await BaseDal.Query(
              whereExpression,
              intPageIndex,
              intPageSize,
              strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:AZLinli.CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await BaseDal.Query(
            strWhere,
            intPageIndex,
            intPageSize,
            strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<Pagination> QueryPage(Expression<Func<TEntity, bool>> expression, Pagination page)
        {
            return await BaseDal.QueryPage(expression, page);
        }

        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            return await BaseDal.QueryMuch(joinExpression, selectExpression, whereLambda);
        }
        #endregion

        #region 分表方法
        public ISugarQueryable<TEntity> SplitQuerable(string split, Expression<Func<TEntity, bool>> expression = null)
        {
            return BaseDal.SplitQuerable(split, expression);
        }

        public async Task<Pagination> QueryPageSplit(Expression<Func<TEntity, bool>> expression, Pagination page, string split)
        {
            return await BaseDal.QueryPageSplit(expression, page, split);
        }
        public async Task<TEntity> QuerySplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            return await BaseDal.QuerySplit(expression, split);
        }
        public async Task<List<TEntity>> QueryListSplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            return await BaseDal.QueryListSplit(expression, split);
        }
        public async Task<List<TEntity>> QueryTreeSplit(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, string split, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            return await BaseDal.QueryTreeSplit(childExp, parentExp, rootValue, split, whereExp, orderExp);
        }
        public async Task<int> StorageableSplit(TEntity entity, string split)
        {
            return await BaseDal.StorageableSplit(entity, split);
        }
        public async Task<bool> UpdateSplit(TEntity entity, string split)
        {
            return await BaseDal.UpdateSplit(entity, split);
        }
        public async Task<bool> UpdateSplit(List<TEntity> list, string split, Expression<Func<TEntity, object>> columns = null)
        {
            return await BaseDal.UpdateSplit(list, split, columns);
        }
        public async Task<bool> DeleteSplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            return await BaseDal.DeleteSplit(expression, split);
        }
        #endregion

        #region 聚合查询
        public async Task<bool> Any(string split = null)
        {
            return await BaseDal.Any(split);
        }
        public async Task<bool> Any(Expression<Func<TEntity, bool>> whereExpression, string split = null)
        {
            return await BaseDal.Any(whereExpression, split);
        }

        public async Task<T> Min<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            return await BaseDal.Min(resultExp, whereExpression,split);
        }
        public async Task<T> Max<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            return await BaseDal.Max(resultExp, whereExpression, split);
        }
        public async Task<int> Count(Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            return await BaseDal.Count(whereExpression, split);
        }
        public async Task<T> Sum<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            return await BaseDal.Sum(resultExp, whereExpression, split);
        }
        #endregion
    }
}
