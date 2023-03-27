using SqlSugar;

namespace WebUtils.Virtual
{
    public interface IDataSeedBase
    {
        public virtual void DbAsync()
        {
            Console.WriteLine("请创建一个新的类以继承该类并重新实现此创建数据库的方法");
        }
    }
}
