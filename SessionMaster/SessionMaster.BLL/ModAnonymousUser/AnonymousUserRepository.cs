using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SessionMaster.BLL.Core;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;
using System.Linq;

namespace SessionMaster.BLL.ModAnonymousUser
{
    public class AnonymousUserRepository : GenericRepository<AnonymousUser>, IAnonymousUserRepository
    {
        public AnonymousUserRepository(SessionMasterContext context) : base(context)
        {
        }
    }
}