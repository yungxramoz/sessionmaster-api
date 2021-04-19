using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionMaster.BLL.Core
{
    public interface IUnitOfWork: IDisposable
    {
        //repositories


        //others
        int Complete();
    }
}
