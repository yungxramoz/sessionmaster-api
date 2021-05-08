using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;
using System;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork : IDisposable
    {
        //repositories
        IUserRepository Users { get; }

        IBoardGameRepository BoardGames { get; }

        ISessionplanRepository Sessionplans { get; }

        //others
        int Complete();
    }
}