using SessionMaster.BLL.Core;
using SessionMaster.DAL.Entities;

namespace SessionMaster.BLL.ModUser
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User Authenticate(string username, string password);

        public User Add(User user, string password);

        User Update(User user, string password);
    }
}