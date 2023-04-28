
namespace WebModel.AppdixEntity
{
    public partial class Button
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Function { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public int Sort { get; set; } = 0;
        public bool Visiable { get; set; } = true;
    }
}
