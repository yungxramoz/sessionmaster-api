using Microsoft.EntityFrameworkCore;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;

namespace SessionMaster.UnitTests.TestHelper
{
    public static class SessionMasterContextTestHelper
    {
        //New Instance for every test, so they don't share the same context
        public static DbContextOptions<SessionMasterContext> ContextOptions
            () => new DbContextOptionsBuilder<SessionMasterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        #region Entity Test Helper

        public static User AddUser(this SessionMasterContext context, string firstname, string lastname, string username, string password)
        {
            PasswordHelper.CreatePasswordHash(password, out var passwordHasch, out var passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Firstname = firstname,
                Lastname = lastname,
                Username = username,
                PasswordHash = passwordHasch,
                PasswordSalt = passwordSalt
            };

            context.Add(user);

            context.SaveChanges();

            return user;
        }

        public static AnonymousUser AddAnonymousUser(this SessionMasterContext context, string name)
        {
            var user = new AnonymousUser
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            context.Add(user);

            context.SaveChanges();

            return user;
        }

        public static Sessionplan AddSessionplan(this SessionMasterContext context, string name, Guid? userId = null)
        {
            var sessionplan = new Sessionplan
            {
                Id = Guid.NewGuid(),
                Name = name,
                UserId = userId
            };

            context.Add(sessionplan);

            context.SaveChanges();

            return sessionplan;
        }

        public static Session AddSession(this SessionMasterContext context, DateTime? date = null)
        {
            var dateNotNull = date ?? DateTime.UtcNow;

            var session = new Session
            {
                Id = Guid.NewGuid(),
                Date = dateNotNull
            };

            context.Add(session);

            context.SaveChanges();

            return session;
        }

        public static SessionUser AssignUserToSession(this SessionMasterContext context, Guid userId, Guid sessionId)
        {
            var sessionUser = new SessionUser
            {
                UserId = userId,
                SessionId = sessionId
            };

            context.Add(sessionUser);
            context.SaveChanges();

            return sessionUser;
        }

        public static SessionAnonymousUser AssignAnonymousUserToSession(this SessionMasterContext context, Guid userId, Guid sessionId)
        {
            var sessionAnonymousUser = new SessionAnonymousUser
            {
                AnonymousUserId = userId,
                SessionId = sessionId
            };

            context.Add(sessionAnonymousUser);
            context.SaveChanges();

            return sessionAnonymousUser;
        }

        #endregion Entity Test Helper
    }
}