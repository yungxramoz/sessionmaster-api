using SessionMaster.BLL.ModUser;
using SessionMaster.DAL;

namespace SessionMaster.BLL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SessionMasterContext _context;

        public UnitOfWork(SessionMasterContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
        }

        public IUserRepository Users { get; private set; }

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
