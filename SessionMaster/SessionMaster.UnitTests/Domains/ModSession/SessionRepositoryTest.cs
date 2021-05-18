using SessionMaster.BLL.ModSession;
using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.Domains.ModSessionplan;
using SessionMaster.UnitTests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModSession
{
    public class SessionRepositoryTest
    {
        public class RegisterTest : SessionRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsSession()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var session = context.AddSession();

                var sut = new SessionRepository(context);

                //Act
                var result = sut.Register(user.Id, session.Id);
                context.SaveChanges();

                var sessionSaved = context.Sessions.First();
                var sessionUserSaved = sessionSaved.SessionUsers.First();

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.SessionUsers);

                Assert.Equal(session.Id, sessionSaved.Id);
                Assert.Equal(session.Date, sessionSaved.Date);
                Assert.Equal(1, result.SessionUsers.Count);
                Assert.Equal(user.Id, sessionUserSaved.UserId);
            }

            [Fact]
            public void Valid_AlreadySignedUp_ReturnsSession()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var session = context.AddSession();
                var sessionUser = context.AssignUserToSession(user.Id, session.Id);

                var sut = new SessionRepository(context);

                //Act
                var result = sut.Register(user.Id, session.Id);
                context.SaveChanges();

                var sessionSaved = context.Sessions.First();
                var sessionUserSaved = sessionSaved.SessionUsers.First();

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.SessionUsers);

                Assert.Equal(session.Id, sessionSaved.Id);
                Assert.Equal(session.Date, sessionSaved.Date);
                Assert.Equal(1, result.SessionUsers.Count);
                Assert.Equal(user.Id, sessionUserSaved.UserId);
            }

            [Fact]
            public void SessionNotExist_ThrowsNotFoundException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());

                var sut = new SessionRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.Register(Guid.NewGuid(), Guid.NewGuid()));
            }
        }

        public class CancelTest : SessionRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsSession()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var session = context.AddSession();
                var sessionUser = context.AssignUserToSession(user.Id, session.Id);

                var sut = new SessionRepository(context);

                //Act
                var result = sut.Cancel(user.Id, session.Id);
                context.SaveChanges();

                var sessionSaved = context.Sessions.First();

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.SessionUsers);

                Assert.Equal(session.Id, sessionSaved.Id);
                Assert.Equal(session.Date, sessionSaved.Date);
                Assert.Equal(0, result.SessionUsers.Count);
            }

            [Fact]
            public void Valid_NotSignedUp_ReturnsSession()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var session = context.AddSession();

                var sut = new SessionRepository(context);

                //Act
                var result = sut.Cancel(user.Id, session.Id);
                context.SaveChanges();

                var sessionSaved = context.Sessions.First();

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.SessionUsers);

                Assert.Equal(session.Id, sessionSaved.Id);
                Assert.Equal(session.Date, sessionSaved.Date);
                Assert.Equal(0, result.SessionUsers.Count);
            }

            [Fact]
            public void SessionNotExist_ThrowsNotFoundException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());

                var sut = new SessionRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.Cancel(Guid.NewGuid(), Guid.NewGuid()));
            }
        }
    }
}