using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModUser;
using SessionMaster.DAL;

namespace SessionMaster.BLL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SessionMasterContext _context;

        public IUserRepository Users { get; private set; }
        public IBoardGameRepository BoardGames { get; private set; }

        public UnitOfWork(SessionMasterContext context, IUserRepository users, IBoardGameRepository boardGames)
        {
            _context = context;
            Users = users;
            BoardGames = boardGames;
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