using BaseRepository;
using WebModel.SystemEntity;
using WebService.ISystemRepository;

namespace WebService.SystemRepository
{
    /// <summary>
	/// ButtonRepository
	/// </summary>
    public partial class ButtonRepository : BaseRepository<Button>, IButtonRepository
    {
        public ButtonRepository() : base()
        {
        }
    }
}
