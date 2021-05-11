using SessionMaster.BLL.ModSessionplan;
using SessionMaster.BLL.ModUser;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.Domains.ModUser;
using SessionMaster.UnitTests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModSessionplan
{
    public class SessionplanRepositoryTest
    {
        public class AddTest : SessionplanRepositoryTest
        {
            [Fact]
            public void Valid_WithUser_ReturnsSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                var sessionplan = new Sessionplan
                {
                    Name = RandomStringTestHelper.Generate(),
                    UserId = user.Id,
                    Sessions = new List<Session>
                    {
                        new Session
                        {
                            Date = DateTime.Today
                        }
                    }
                };

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Add(sessionplan);
                context.SaveChanges();

                var planFromDb = context.Sessionplans.First();

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.UserId);

                Assert.Same(sessionplan.Name, result.Name);
                Assert.Same(sessionplan.Name, planFromDb.Name);
                Assert.Same(sessionplan.Sessions, result.Sessions);
                Assert.Same(sessionplan.Sessions, planFromDb.Sessions);
                Assert.Same(user, result.User);
                Assert.Same(user, planFromDb.User);
            }

            [Fact]
            public void Valid_WithoutUser_ReturnsSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());

                var sessionplan = new Sessionplan
                {
                    Name = RandomStringTestHelper.Generate(),
                    Sessions = new List<Session>
                    {
                        new Session
                        {
                            Date = DateTime.Today
                        }
                    }
                };

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Add(sessionplan);
                context.SaveChanges();

                var planFromDb = context.Sessionplans.First();

                //Assert
                Assert.NotNull(result);

                Assert.Null(result.UserId);
                Assert.Null(planFromDb.UserId);
                Assert.Null(result.User);
                Assert.Null(planFromDb.User);

                Assert.Same(sessionplan.Name, result.Name);
                Assert.Same(sessionplan.Name, planFromDb.Name);
                Assert.Same(sessionplan.Sessions, result.Sessions);
                Assert.Same(sessionplan.Sessions, planFromDb.Sessions);
            }
        }

        public class Remove : SessionplanRepositoryTest
        {
            [Fact]
            public void Valid_DeletesSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sessionplan = context.AddSessionplan("Plan Name");

                var sut = new SessionplanRepository(context);

                //Act
                sut.Remove(sessionplan.Id);
                context.SaveChanges();

                //Assert
                Assert.Null(context.Find<Sessionplan>(sessionplan.Id));
            }

            [Fact]
            public void NotExisits_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new UserRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.Remove(Guid.NewGuid()));
            }
        }

        public class GetTest : SessionplanRepositoryTest
        {
            [Fact]
            public void ByName_ReturnsSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sessionplan = context.AddSessionplan("Test Plan");

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Get(u => u.Name == "Test Plan").FirstOrDefault();

                //Assert
                Assert.Same(sessionplan, result);
            }

            [Fact]
            public void ByUserId_ReturnsSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var sessionplan = context.AddSessionplan("Test Plan", user.Id);

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Get(u => u.UserId == user.Id).FirstOrDefault();

                //Assert
                Assert.Same(sessionplan, result);
            }

            [Fact]
            public void MultipleResults_ReturnsSessionplanList()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddUser("Julius", "Testimus", "tester", "test");
                var sessionplan1 = context.AddSessionplan("Test Plan 1", user.Id);
                var sessionplan2 = context.AddSessionplan("Test Plan 2", user.Id);

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Get(u => u.User == user);

                //Assert
                Assert.Contains(sessionplan1, result);
                Assert.Contains(sessionplan2, result);
            }

            [Fact]
            public void NoParams_ReturnsAllSessionplans()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sessionplan1 = context.AddSessionplan("Test Plan 1");
                var sessionplan2 = context.AddSessionplan("Test Plan 2");

                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Get();

                //Assert
                Assert.Contains(sessionplan1, result);
                Assert.Contains(sessionplan2, result);
            }

            [Fact]
            public void NoSessionplan_ReturnsSessionplanList()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new SessionplanRepository(context);

                //Act
                var result = sut.Get();

                //Assert
                Assert.Empty(result);
            }
        }

        public class GetByIdTest : SessionplanRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new SessionplanRepository(context);
                var sessionplan = context.AddSessionplan("Test Plan");

                //Act
                var result = sut.GetById(sessionplan.Id);

                //Assert
                Assert.Equal(sessionplan.Id, result.Id);
                Assert.Equal(sessionplan.Name, result.Name);
                Assert.Equal(sessionplan.Sessions, result.Sessions);
            }

            [Fact]
            public void NotExists_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new SessionplanRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.GetById(Guid.NewGuid()));
            }
        }

        public class UpdateTest : SessionplanRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsUpdatedSessionplan()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new SessionplanRepository(context);
                var sessionplan = context.AddSessionplan(RandomStringTestHelper.Generate());

                var updatedPlan = new Sessionplan
                {
                    Id = sessionplan.Id,
                    Name = "Updated"
                };

                //Act
                var result = sut.Update(updatedPlan);
                context.SaveChanges();

                //Assert
                Assert.NotNull(result);

                Assert.Same(updatedPlan.Name, result.Name);
            }
        }
    }
}