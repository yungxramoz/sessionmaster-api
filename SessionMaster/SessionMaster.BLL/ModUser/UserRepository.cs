using SessionMaster.BLL.Core;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;

namespace SessionMaster.BLL.ModUser
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SessionMasterContext context) : base(context)
        {
        }
    }
}
