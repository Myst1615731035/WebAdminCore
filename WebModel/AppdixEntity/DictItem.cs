namespace WebModel.AppdixEntity
{
    /// <summary>
    /// 字典子类
    /// </summary>
    public partial class DictItem
    {
        public string? Label { get; set; }
        public int Value { get; set; }
        public string? Description { get; set; }
        public int Sort { get; set; } = 0;
    }
}
