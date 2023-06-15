using ApiModel;
using SqlSugar;
using SqlSugar.Extensions;
using System.Data;
using System.Linq.Expressions;

namespace WebUtils.BaseService
{
    /// <summary>
    /// ORM泛型基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        private readonly SqlSugarScope _dbBase = DBHelper.Db;

        #region SqlSugar实例
        public ISqlSugarClient Db
        {
            get
            {
                return _db;
            }
        }

        protected ISqlSugarClient _db
        {
            get
            {
                /* 如果要开启多库支持，
                 * 1、在appsettings.json 中开启MutiDBEnabled节点为true，必填
                 * 2、设置一个主连接的数据库ID，节点MainDB，对应的连接字符串的Enabled也必须true，必填
                 */
                if (DBConfig.MutilDb)
                {
                    if (typeof(TEntity).GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault((x => x.GetType() == typeof(SugarTable))) is SugarTable sugarTable && !string.IsNullOrEmpty(sugarTable.TableDescription))
                    {
                        _dbBase.ChangeDatabase(sugarTable.TableDescription.ToLower());
                    }
                    else
                    {
                        _dbBase.ChangeDatabase(DBConfig.MainDbConfigId);
                    }
                }
                return _dbBase;
            }
        }
        #endregion

        #region 事务方法
        public void BeginTran()
        {
            Db.AsTenant().BeginTran();
        }
        public void CommitTran()
        {
            Db.AsTenant().CommitTran();
        }
        public void RollbackTran()
        {
            Db.AsTenant().RollbackTran();
        }
        #endregion

        #region 单表方法
        public ISugarQueryable<TEntity> IQuerable(Expression<Func<TEntity, bool>> expression = null)
        {
            return Db.Queryable<TEntity>().WhereIF(expression.IsNotEmpty(), expression);
        }

        #region 查
        public async Task<TEntity> First(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Db.Queryable<TEntity>().Where(whereExpression).FirstAsync();
        }
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="withCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId, bool withCache = false)
        {
            var query = Db.Queryable<TEntity>().IncludesAllFirstLayer().WithCacheIF(withCache).In(objId);
            return await query.IncludesAllFirstLayer().SingleAsync();
        }
        /// <summary>
        /// 功能描述:根据ID查询数据
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIDs(object[] lstIds)
        {
            return await Db.Queryable<TEntity>().IncludesAllFirstLayer().In(lstIds).ToListAsync();
        }
        #endregion

        #region 树查询
        /// <summary>
        /// 树查询
        /// </summary>
        /// <param name="childExp">子集字段</param>
        /// <param name="parentExp">父级字段</param>
        /// <param name="rootValue">根数据</param>
        /// <param name="whereExp">筛选条件</param>
        /// <param name="orderExp">排序条件</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryTree(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            return await Db.Queryable<TEntity>()
                            .IncludesAllFirstLayer()
                            .WhereIF(whereExp != null, whereExp)
                            .OrderByIF(orderExp != null, orderExp)
                            .ToTreeAsync(childExp, parentExp, rootValue);
        }

        public async Task<List<TResult>> QueryTree<TResult>(Expression<Func<TEntity, TResult>> resultExp, Expression<Func<TResult, IEnumerable<object>>> childExp, Expression<Func<TResult, object>> parentExp, object rootValue, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            return await Db.Queryable<TEntity>().IncludesAllFirstLayer()
                            .WhereIF(whereExp != null, whereExp)
                            .OrderByIF(orderExp != null, orderExp)
                            .Select(resultExp)
                            .ToTreeAsync(childExp, parentExp, rootValue);
        }
        #endregion

        #region 增
        /// <summary>
        /// (自增)写入实体数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity)
        {
            return await Db.Insertable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// (雪花)写入实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<long> AddLong(TEntity entity)
        {
            return await Db.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<TEntity> AddEntity(TEntity entity)
        {
            return await Db.Insertable(entity).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 写入实体数据(写入部分列)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">指定只插入列</param>
        /// <returns>返回自增量列</returns>
        public async Task<int> Add(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null)
        {
            var insert = Db.Insertable(entity);
            if (insertColumns == null) return await insert.ExecuteReturnIdentityAsync();
            else return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> Insert(List<TEntity> list)
        {
            if (list.IsNotEmpty() && list.Count > 0)
            {
                return await Db.Insertable(list).ExecuteCommandAsync();
            }
            return 0;
        }

        /// <summary>
        /// 批量插入实体(雪花)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<long>> AddLong(List<TEntity> list)
        {
            if (list.IsNotEmpty() && list.Count > 0)
            {
                return await Db.Insertable(list).ExecuteReturnSnowflakeIdListAsync();
            }
            return new List<long>();
        }
        #endregion

        #region 改
        /// <summary>
        /// 更新实体数据 以主键为条件
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity)
        {
            return await Db.Updateable(entity).ExecuteCommandHasChangeAsync();
        }
        public async Task<bool> Update(List<TEntity> list, Expression<Func<TEntity, object>> columns = null)
        {
            return await Db.Updateable(list).UpdateColumnsIF(columns != null, columns).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await Db.Updateable(entity).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update(string strSql, SugarParameter[] parameters = null)
        {
            return await Db.Ado.ExecuteCommandAsync(strSql, parameters) > 0;
        }

        public async Task<bool> Update(object operateAnonymousObjects)
        {
            return await Db.Updateable<TEntity>(operateAnonymousObjects).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateCols = null, Expression<Func<TEntity, object>> ignoreCols = null, Expression<Func<TEntity, object>> whereCols = null)
        {
            var query = Db.Updateable(entity);
            if (whereCols.IsNotEmpty()) query.WhereColumns(whereCols);
            return await query.UpdateColumnsIF(updateCols != null, updateCols)
                        .IgnoreColumnsIF(ignoreCols != null, ignoreCols)
                        .ExecuteCommandHasChangeAsync();
        }
        #endregion

        #region 增 改一体
        public async Task<int> Storageable(TEntity entity)
        {
            return await Db.Storageable(entity).ExecuteCommandAsync();
        }
        public async Task<int> Storageable(List<TEntity> entity)
        {
            return await Db.Storageable(entity).ExecuteCommandAsync();
        }
        #endregion

        #region 删
        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity entity)
        {
            return await Db.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete(List<TEntity> list)
        {
            return await Db.Deleteable(list).ExecuteCommandHasChangeAsync();
        }

        public async Task<int> Delete(Expression<Func<TEntity, bool>> expression)
        {
            return await Db.Deleteable(expression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            //var i = await Task.Run(() => Db.Deleteable<TEntity>(id).ExecuteCommand());
            //return i > 0;
            return await Db.Deleteable<TEntity>(id).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            //var i = await Task.Run(() => Db.Deleteable<TEntity>().In(ids).ExecuteCommand());
            //return i > 0;
            return await Db.Deleteable<TEntity>().In(ids).ExecuteCommandHasChangeAsync();
        }
        #endregion
        #endregion

        #region 高级查询
        /// <summary>
        /// 功能描述:查询所有数据
        /// 作　　者:CommonApi
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query()
        {
            var query = Db.Queryable<TEntity>();
            return await query.ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere)
        {
            //return await Task.Run(() => Db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
            return await Db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
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
        public async Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> resultExp, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            return await Db.Queryable<TEntity>().OrderByIF(orderExp.IsNotEmpty(), orderExp).WhereIF(whereExp.IsNotEmpty(), whereExp).Select(resultExp).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(strOrderByFileds != null, strOrderByFileds).ToListAsync();
        }
        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression)
        {
            return await Db.Queryable<TEntity>().IncludesAllFirstLayer().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFileds)
        {
            return await Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFileds)
        {
            //return await Task.Run(() => Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToList());
            return await Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>泛型集合</returns>
        public async Task<List<TEntity>> QuerySql(string strSql, SugarParameter[] parameters = null)
        {
            return await Db.Ado.SqlQueryAsync<TEntity>(strSql, parameters);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(string strSql, SugarParameter[] parameters = null)
        {
            return await Db.Ado.GetDataTableAsync(strSql, parameters);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:CommonApi
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await Db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 分页查询[使用版本，其他分页未测试]
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<Pagination> QueryPage(Pagination pageModel, Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderExp = null)
        {
            RefAsync<int> totalCount = 0;
            var list = Db.Queryable<TEntity>()
                         .IncludesAllFirstLayer()
                         .WhereIF(expression != null, expression)
                         .OrderByIF(orderExp.IsNotEmpty(), orderExp);

            if (pageModel.isAll)
            {
                var data = await list.ToListAsync();
                pageModel.response = data;
                pageModel.total = data.Count;
                pageModel.pageCount = 1;
            }
            else
            {
                pageModel.response = await list.ToPageListAsync(pageModel.currentPage, pageModel.pageSize, totalCount);
                int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / pageModel.pageSize.ObjToDecimal())).ObjToInt();
                pageModel.total = totalCount;
                pageModel.pageCount = pageCount;
            }
            return pageModel;
        }


        /// <summary>
        /// 分页查询[使用版本，其他分页未测试]
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<Pagination> QueryPage<TResult>(Pagination pageModel, Expression<Func<TEntity, TResult>> resultExp, Expression<Func<TEntity, bool>> whereExp, Expression<Func<TEntity, object>> orderExp = null)
        {
            RefAsync<int> totalCount = 0;
            var list = Db.Queryable<TEntity>()
                         .IncludesAllFirstLayer()
                         .WhereIF(whereExp != null, whereExp)
                         .OrderByIF(orderExp.IsNotEmpty(), orderExp)
                         .Select(resultExp);

            if (pageModel.isAll)
            {
                var data = await list.ToListAsync();
                pageModel.response = data;
                pageModel.total = data.Count;
                pageModel.pageCount = 1;
            }
            else
            {
                pageModel.response = await list.ToPageListAsync(pageModel.currentPage, pageModel.pageSize, totalCount);
                int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / pageModel.pageSize.ObjToDecimal())).ObjToInt();
                pageModel.total = totalCount;
                pageModel.pageCount = pageCount;
            }
            return pageModel;
        }


        /// <summary> 
        ///查询-多表查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam> 
        /// <typeparam name="T2">实体2</typeparam> 
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param> 
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param> 
        /// <returns>值</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await Db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await Db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }
        #endregion

        #region 分表方法
        public ISugarQueryable<TEntity> SplitQuerable(string split, Expression<Func<TEntity, bool>> expression = null)
        {
            var tableName = GetSplitTableName<TEntity>(split);
            return Db.Queryable<TEntity>().WhereIF(expression != null, expression)
                    .SplitTable(t => t.ContainsTableNames(tableName));
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<Pagination> QueryPageSplit(Expression<Func<TEntity, bool>> expression, Pagination page, string split)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            RefAsync<int> totalCount = 0;
            var list = Db.Queryable<TEntity>()
                         .WhereIF(expression != null, expression)
                         .SplitTable(t => t.ContainsTableNames(tableName))
                         .OrderByIF(!string.IsNullOrEmpty(page.sort), page.sort);

            if (page.isAll)
            {
                var data = await list.ToListAsync();
                page.response = data;
                page.total = data.Count;
                page.pageCount = 1;
            }
            else
            {
                page.response = await list.ToPageListAsync(page.currentPage, page.pageSize, totalCount);
                int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / page.pageSize.ObjToDecimal())).ObjToInt();
                page.total = totalCount;
                page.pageCount = pageCount;
            }
            return page;
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<TEntity> QuerySplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            return await Db.Queryable<TEntity>()
                .WhereIF(expression != null, expression)
                .SplitTable(t => t.ContainsTableNames(tableName))
                .FirstAsync();
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryListSplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            return await Db.Queryable<TEntity>()
                .WhereIF(expression != null, expression)
                .SplitTable(t => t.ContainsTableNames(tableName))
                .ToListAsync();
        }
        
        /// <summary>
        /// 分表查询树
        /// </summary>
        /// <param name="childExp">子集字段</param>
        /// <param name="parentExp">父级字段</param>
        /// <param name="rootValue">跟数据</param>
        /// <param name="split">分表标签</param>
        /// <param name="whereExp">筛选条件</param>
        /// <param name="orderExp">排序条件</param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryTreeSplit(Expression<Func<TEntity, IEnumerable<object>>> childExp, Expression<Func<TEntity, object>> parentExp, object rootValue, string split, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null)
        {
            var tableName = GetSplitTableName<TEntity>(split);
            return (await Db.Queryable<TEntity>().WhereIF(whereExp != null, whereExp)
                           .SplitTable(t => t.ContainsTableNames(tableName))
                           .OrderByIF(orderExp != null, orderExp)
                           .ToTreeAsync(childExp, parentExp, rootValue)) ?? new List<TEntity>();
        }

        /// <summary>
        /// 分表查询树 + 字段筛选
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultExp"></param>
        /// <param name="childExp"></param>
        /// <param name="parentExp"></param>
        /// <param name="rootValue"></param>
        /// <param name="split"></param>
        /// <param name="whereExp"></param>
        /// <param name="orderExp"></param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryTreeSplit<TResult>(Expression<Func<TEntity, TResult>> resultExp, Expression<Func<TResult, IEnumerable<object>>> childExp, Expression<Func<TResult, object>> parentExp, object rootValue, string split, Expression<Func<TEntity, bool>> whereExp = null, Expression<Func<TEntity, object>> orderExp = null) 
        {
            var tableName = GetSplitTableName<TEntity>(split);
            return (await Db.Queryable<TEntity>().WhereIF(whereExp != null, whereExp)
                           .SplitTable(t => t.ContainsTableNames(tableName))
                           .OrderByIF(orderExp != null, orderExp)
                           .Select(resultExp)
                           .ToTreeAsync(childExp, parentExp, rootValue)) ?? new List<TResult>();
        }

        /// <summary>
        /// 分表新增或更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<int> StorageableSplit(TEntity entity, string split)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            return await Db.Storageable(entity).As(tableName).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSplit(TEntity entity, string split)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            return (await Db.Updateable(entity).SplitTable(t => t.ContainsTableNames(tableName)).ExecuteCommandAsync()) > 0;
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSplit(List<TEntity> entity, string split, Expression<Func<TEntity, object>> columns = null)
        {
            var tableName = CreateSplitTable<TEntity>(split);
            var query = Db.Updateable(entity)
                        .UpdateColumnsIF(columns != null, columns)
                        .SplitTable(t => t.ContainsTableNames(tableName));
            return (await query.ExecuteCommandAsync()) > 0;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSplit(Expression<Func<TEntity, bool>> expression, string split)
        {
            return (await Db.Deleteable<TEntity>().Where(expression).SplitTable(tab => tab.ContainsTableNames(split)).ExecuteCommandAsync()) > 0;
        }

        /// <summary>
        /// 根据实体类创建对应分表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="split"></param>
        /// <returns></returns>
        public string CreateSplitTable<T>(string split) where T : class, new()
        {
            var tableName = GetSplitTableName<T>(split);
            var type = typeof(T);
            if (!Db.DbMaintenance.IsAnyTable(tableName))
            {
                Db.MappingTables.Add(type.Name, tableName);
                Db.CodeFirst.InitTables<T>();
            }
            return tableName;
        }

        public string GetSplitTableName<T>(string split) where T : class, new()
        {
            return Db.SplitHelper<T>().GetTableName(split);
        }
        #endregion

        #region 聚合查询
        public async Task<bool> Any(string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.AnyAsync();
        }
        public async Task<bool> Any(Expression<Func<TEntity, bool>> whereExpression, string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.AnyAsync(whereExpression);
        }
        public async Task<T> Min<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.WhereIF(whereExpression != null, whereExpression).MinAsync(resultExp);

        }
        public async Task<T> Max<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.WhereIF(whereExpression != null, whereExpression).MaxAsync(resultExp);
        }
        public async Task<int> Count(Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.WhereIF(whereExpression != null, whereExpression).CountAsync();
        }
        public async Task<T> Sum<T>(Expression<Func<TEntity, T>> resultExp, Expression<Func<TEntity, bool>> whereExpression = null, string split = null)
        {
            var query = Db.Queryable<TEntity>();
            if (split.IsNotEmpty())
            {
                var tableName = CreateSplitTable<TEntity>(split);
                query = query.SplitTable(t => t.ContainsTableNames(tableName));
            }
            return await query.WhereIF(whereExpression != null, whereExpression).SumAsync(resultExp);
        }
        #endregion
    }
}
