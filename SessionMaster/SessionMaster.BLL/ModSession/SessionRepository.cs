using SessionMaster.BLL.Core;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;

namespace SessionMaster.BLL.ModSession
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(SessionMasterContext context) : base(context)
        {
        }
    }
}