using Microsoft.EntityFrameworkCore;
using SessionMaster.BLL.Core;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;
using System.Linq;

namespace SessionMaster.BLL.ModSession
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(SessionMasterContext context) : base(context)
        {
        }

        public Session Register(Guid userId, Guid sessionId)
        {
            var session =
                    GetById(sessionId,
                        e => e.Include(s => s.SessionUsers).ThenInclude(su => su.User)
                            .Include(s => s.SessionAnonymousUsers).ThenInclude(sau => sau.AnonymousUser)
                    );

            if (!session.SessionUsers.Any(s => s.UserId == userId))
            {
                session.SessionUsers.Add(new SessionUser
                {
                    UserId = userId,
                    SessionId = sessionId
                });
            }

            return session;
        }

        public Session Cancel(Guid userId, Guid sessionId)
        {
            var session =
                    GetById(sessionId,
                        e => e.Include(s => s.SessionUsers).ThenInclude(su => su.User)
                            .Include(s => s.SessionAnonymousUsers).ThenInclude(sau => sau.AnonymousUser)
                    );

            var sessionUser = session.SessionUsers.FirstOrDefault(su => su.UserId == userId && su.SessionId == sessionId);

            if (sessionUser != null)
            {
                session.SessionUsers.Remove(sessionUser);
            }

            return session;
        }
    }
}