using Generate.Model;
using SqlSugar.Extensions;

namespace Generate.Service
{
    public class TemplateService
    {
        public static string GetFormVueDefultValue(ColumnInfo column)
        {
            var res = "null";
            try
            {
                if (column.IsNullable) res = "null";
                else
                {
                    res = column.DefaultValue.ObjToString() switch
                    {
                        "" => "''",
                        "False" => "false",
                        "True" => "true",
                        _=> column.DefaultValue.ObjToString()
                    };
                }
            }
            catch (Exception ex)
            {
                res = "null";
            }
            return res == "" ? "''" : res;
        }
    }
}
