using SessionMaster.BLL.ModUser;
using System;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork: IDisposable
    {
        //repositories
        IUserRepository Users { get; }


        //others
        int Complete();
    }
}
