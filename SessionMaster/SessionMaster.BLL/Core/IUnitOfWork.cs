using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModUser;
using System;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork: IDisposable
    {
        //repositories
        IUserRepository Users { get; }
        IBoardGameRepository BoardGames { get; }


        //others
        int Complete();
    }
}
