using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using SqlSugar.Extensions;

namespace WebUtils
{
    public static class UtilConvert
    {
        #region ToJObject
        /// <summary>
        /// 字符串转JObject
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string str)
        {
            if (str.IsEmpty()) return new JObject();
            JObject obj = new JObject();
            try
            {
                obj = JObject.Parse(str);
            }
            catch (Exception ex)
            {
            }
            return obj;
        }
        #endregion

        #region ToList || ToTree
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name=""></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<DataColumn> ToList(this DataColumnCollection columns)
        {
            var list = new List<DataColumn>();
            foreach(DataColumn column in columns)
            {
                list.Add(column);
            }
            return list;
        }

        /// <summary>
        /// json转List<TResult>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json)
        {
            if (!json.IsNotEmpty())
            {
                return new List<T>();
            }
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// IEnumerator 转 List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }

        /// <summary>
        /// 将列表转换为树形结构
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">数据</param>
        /// <param name="rootwhere">根条件</param>
        /// <param name="childswhere">节点条件</param>
        /// <param name="addchilds">添加子节点</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this List<T> list, Func<T, T, bool> rootwhere, Func<T, T, bool> childswhere, Action<T, IEnumerable<T>> addchilds, T entity = default(T))
        {
            var treelist = new List<T>();
            // 空树 是否存在根节点，如果不存在，直接返回空数组
            if (list == null || list.Count == 0 || !list.Any(e => rootwhere(entity, e))) return treelist;
            //树根
            if (list.Any(e => rootwhere(entity, e))) treelist.AddRange(list.Where(e => rootwhere(entity, e)));
            //树叶
            foreach (var item in treelist)
            {
                if (list.Any(e => childswhere(item, e)))
                {
                    var nodedata = list.Where(e => childswhere(item, e)).ToList();
                    foreach (var child in nodedata)
                    {
                        //添加子集
                        var data = list.ToTree(childswhere, childswhere, addchilds, child);
                        addchilds(child, data);
                    }
                    addchilds(item, nodedata);
                }
            }
            return treelist;
        }
        #endregion

        #region ToString
        /// <summary>
        /// 流转字符串
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public static string ToString(this Stream sm)
        {
            if (sm.Length.IsNotEmpty())
            {
                return new StreamReader(sm).ReadToEnd();
            }
            return "";
        }

        /// <summary>
        /// byte数组转字符串
        /// </summary>
        /// <param name="byteArr"></param>
        /// <returns></returns>
        public static string ToString(this byte[] byteArr)
        {
            return Encoding.Default.GetString(byteArr);
        }
        #endregion

        #region ToJson
        /// <summary>
        /// 对象转Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool indented = false)
        {
            if (obj.IsNotEmpty())
            {
                return indented ? JsonConvert.SerializeObject(obj, Formatting.Indented): JsonConvert.SerializeObject(obj);
            }
            return JsonConvert.SerializeObject(new { });
        }
        public static string ToJson(this object obj, JsonSerializerSettings settings)
        {
            if (obj.IsNotEmpty())
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            return JsonConvert.SerializeObject(new { });
        }
        #endregion

        #region Type Extention
        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 字符串中多个连续空格转为一个空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        public static string MergeSpace(this string str)
        {
            if (str != string.Empty && str != null && str.Length > 0)
            {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 对象字段值继承
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T ExtendPartProperty<T, T1>(T1 data)
        {
            T entity = Activator.CreateInstance<T>();
            Type type = entity.GetType();
            var pros = data.GetType().GetProperties().ToList();
            foreach (var prop in type.GetProperties())
            {
                var pro = pros.Find(t => t.Name == prop.Name);
                if (pro != null)
                {
                    prop.SetValue(entity, pro.GetValue(data, null));
                }
            }
            return entity;
        }
        #endregion

        #region 判空
        public static bool IsNotEmpty(this object? thisValue)
        {
            return thisValue.ObjToString() != "" && thisValue.ObjToString() != "undefined" && thisValue.ObjToString() != "null";
        }
        public static bool IsEmpty(this object? thisValue)
        {
            return !IsNotEmpty(thisValue);
        }
        #endregion

        #region ToInt64
        #endregion
        
        #region ToDouble
        public static double ObjToMoney(this object thisValue)
        {
            double reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return 0;
        }
        public static double ObjToMoney(this object thisValue, double errorValue)
        {
            double reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }
        #endregion

        #region ToString
        public static string ObjToString(this DateTime time)
        {
            if (time.IsNotEmpty())
            {
                return time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "";
        }
        public static string ObjToString(this object thisValue, string errorValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return errorValue;
        }
        #endregion

        #region ToDateTime
        public static DateTime ObjToDate(this object thisValue, DateTime errorValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        ///  时间戳转本地时间-时间戳精确到秒
        /// </summary> 
        public static DateTime ToLocalTimeDateBySeconds(long unix)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(unix);
            return dto.ToLocalTime().DateTime;
        }
        /// <summary>
        ///  时间戳转本地时间-时间戳精确到毫秒
        /// </summary> 
        public static DateTime ToLocalTimeDateByMilliseconds(long unix)
        {
            var dto = DateTimeOffset.FromUnixTimeMilliseconds(unix);
            return dto.ToLocalTime().DateTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string time)
        {
            time = time.Substring(0, 10);
            double timestamp = Convert.ToInt64(time);
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }
        #endregion

        #region ToTimeStamp
        /// <summary>
        ///  时间转时间戳Unix-时间戳精确到秒
        /// </summary> 
        public static long ToUnixTimestampBySeconds(DateTime dt)
        {
            DateTimeOffset dto = new DateTimeOffset(dt);
            return dto.ToUnixTimeSeconds();
        }

        /// <summary>
        ///  时间转时间戳Unix-时间戳精确到毫秒
        /// </summary> 
        public static long ToUnixTimestampByMilliseconds(DateTime dt)
        {
            DateTimeOffset dto = new DateTimeOffset(dt);
            return dto.ToUnixTimeMilliseconds();
        }
        #endregion

        #region ToTimeSpanString
        public static string DateToTimeStamp(this DateTime thisValue)
        {
            TimeSpan ts = thisValue - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region 时间差
        /// <summary>
        /// 是哦检查
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static string TimeSubTract(DateTime time1, DateTime time2)
        {
            TimeSpan subTract = time1.Subtract(time2);
            return $"{subTract.Days} 天 {subTract.Hours} 时 {subTract.Minutes} 分 ";
        }
        #endregion

        #region ByteArray
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Serialize(this object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);

            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(this byte[] value)
        {
            if (value == null)
            {
                return default(TEntity);
            }
            var jsonString = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject<TEntity>(jsonString);
        }
        #endregion

        #region ToList
        public static List<T> ToList<T>(this JToken? data)
        {
            List<T> res = default(List<T>);
            if(!data.IsEmpty())
            {
                res = JsonConvert.DeserializeObject<List<T>>(data.ObjToString());
            }
            return res;
        }
        #endregion

        #region Update Type Object
        /// <summary>
        /// 泛型获取数据字段的变化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        public static T UpdateProp<T>(this T obj, T newObj) where T : class, new()
        {
            if (newObj.IsEmpty()) return obj;
            Type type = typeof(T);
            type.GetProperties().ToList().ForEach(t =>
            {
                var newVal = t.GetValue(newObj);
                var oldVal = t.GetValue(obj);
                if (newVal.IsNotEmpty() && newVal != oldVal) t.SetValue(obj, newVal);
            });
            return obj;
        }
        #endregion
    }
}
