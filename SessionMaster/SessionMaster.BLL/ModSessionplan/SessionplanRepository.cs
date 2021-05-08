using SessionMaster.BLL.Core;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;

namespace SessionMaster.BLL.ModSessionplan
{
    public class SessionplanRepository : GenericRepository<Sessionplan>, ISessionplanRepository
    {
        public SessionplanRepository(SessionMasterContext context) : base(context)
        {
        }
    }
}