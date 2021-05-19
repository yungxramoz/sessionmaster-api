using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

        public override Session GetById(Guid id, Func<IQueryable<Session>, IIncludableQueryable<Session, object>> include = null)
        {
            if (include != null)
            {
                return base.GetById(id, include);
            }

            return base.GetById(id, e => e.Include(s => s.SessionUsers).ThenInclude(su => su.User)
                           .Include(s => s.SessionAnonymousUsers).ThenInclude(sau => sau.AnonymousUser));
        }

        public Session Register(Guid userId, Guid sessionId)
        {
            var session = GetById(sessionId);

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

        public Session RegisterAnonymous(Guid userId, Guid sessionId)
        {
            var session = GetById(sessionId);

            if (!session.SessionAnonymousUsers.Any(s => s.AnonymousUserId == userId))
            {
                session.SessionAnonymousUsers.Add(new SessionAnonymousUser
                {
                    AnonymousUserId = userId,
                    SessionId = sessionId
                });
            }

            return session;
        }

        public Session Cancel(Guid userId, Guid sessionId)
        {
            var session = GetById(sessionId);

            var sessionUser = session.SessionUsers.FirstOrDefault(su => su.UserId == userId && su.SessionId == sessionId);

            if (sessionUser != null)
            {
                session.SessionUsers.Remove(sessionUser);
            }

            return session;
        }

        public Session CancelAnonymous(Guid userId, Guid sessionId)
        {
            var session = GetById(sessionId);

            var sessionUser = session.SessionAnonymousUsers.FirstOrDefault(su => su.AnonymousUserId == userId && su.SessionId == sessionId);

            if (sessionUser != null)
            {
                session.SessionAnonymousUsers.Remove(sessionUser);
            }

            return session;
        }
    }
}