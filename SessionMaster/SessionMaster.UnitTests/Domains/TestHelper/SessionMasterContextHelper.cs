using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using System;

namespace SessionMaster.UnitTests.Domains.Core
{
    public static class SessionMasterContextHelper
    {
        //New Instance for every test, so they don't share the same context
        public static DbContextOptions<SessionMasterContext> ContextOptions
            () => new DbContextOptionsBuilder<SessionMasterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        #region User Test Helper
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
        #endregion
    }
}
