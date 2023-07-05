namespace WebUtils.Attributes
{
    /// <summary>
    /// 利用该特性对需要生成种子数据的实体类进行标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataSeedAttribute : Attribute
    {
        public DataSeedAttribute() { }
    }
}
