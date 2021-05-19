using SessionMaster.BLL.ModAnonymousUser;
using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModSession;
using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;
using System;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork : IDisposable
    {
        //repositories
        IUserRepository Users { get; }

        IAnonymousUserRepository AnonymousUsers { get; }

        IBoardGameRepository BoardGames { get; }

        ISessionplanRepository Sessionplans { get; }

        ISessionRepository Sessions { get; }

        //others
        int Complete();
    }
}