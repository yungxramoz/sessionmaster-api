using SessionMaster.BLL.ModAnonymousUser;
using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModSession;
using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;
using SessionMaster.DAL;

namespace SessionMaster.BLL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SessionMasterContext _context;

        public IUserRepository Users { get; private set; }
        public IAnonymousUserRepository AnonymousUsers { get; private set; }
        public IBoardGameRepository BoardGames { get; private set; }
        public ISessionplanRepository Sessionplans { get; private set; }
        public ISessionRepository Sessions { get; private set; }

        public UnitOfWork(SessionMasterContext context, IUserRepository users, IBoardGameRepository boardGames, ISessionplanRepository sessionplans, ISessionRepository sessions, IAnonymousUserRepository anonymousUsers)
        {
            _context = context;
            Users = users;
            AnonymousUsers = anonymousUsers;
            BoardGames = boardGames;
            Sessionplans = sessionplans;
            Sessions = sessions;
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