using SessionMaster.BLL.Core;
using SessionMaster.DAL.Entities;
using System;

namespace SessionMaster.BLL.ModSession
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Session Register(Guid userId, Guid sessionId);

        Session Cancel(Guid userId, Guid sessionId);
    }
}