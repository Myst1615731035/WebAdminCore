using SqlSugar;
using SqlSugar.Extensions;
using System.Linq.Expressions;

namespace WebUtils
{
    /// <summary>
    /// 对SqlSugar.SqlFunc进行扩展，并添加到SqlSugar服务中
    /// </summary>
    public static class SqlFuncExt
    {
        public static List<SqlFuncExternal> SqlFuncExtList()
        {
            List<SqlFuncExternal> list = new List<SqlFuncExternal>();
            #region JSON Array Where
            list.Add(new SqlFuncExternal
            {
                UniqueMethodName = "JointSubQuerySql",
                MethodValue = (expInfo, dbType, expContext) => expInfo.Args[0].MemberValue.ObjToString()
            });
            #endregion
            return list;
        }

        public static T JointSubQuerySql<T>(string sql)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }


    }
}
