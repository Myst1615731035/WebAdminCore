namespace WebUtils.Attributes
{
    /// <summary>
    /// 利用该特性筛选出的权限数据表
    /// 应用于代码生成器中
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SystemDefaultAttribute : Attribute
    {
        public SystemDefaultAttribute() { }
    }
}
