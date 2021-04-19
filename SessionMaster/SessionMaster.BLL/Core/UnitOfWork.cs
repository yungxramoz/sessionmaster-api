using SessionMaster.DAL;

namespace SessionMaster.BLL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SessionMasterContext _context;

        public UnitOfWork(SessionMasterContext context)
        {
            _context = context;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
