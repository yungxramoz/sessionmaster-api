using System;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork: IDisposable
    {
        //repositories


        //others
        int Complete();
    }
}
