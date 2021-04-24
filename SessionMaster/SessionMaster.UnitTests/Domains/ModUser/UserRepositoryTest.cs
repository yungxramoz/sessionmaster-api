using SessionMaster.BLL.ModUser;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.Domains.Core;
using System;
using System.Linq;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModUser
{
    public class UserRepositoryTest
    {
        public class AuthenticateTest : UserRepositoryTest
        {
            [Fact]
            public void Valid_ResturnsUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                //Act
                var result = sut.Authenticate("tester", "test");

                //Assert
                Assert.Equal(user, result);
            }

            [Fact]
            public void NullUsername_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                context.AddUser("Julius", "Testimus", "tester", "test");

                //Act & Assert
                Assert.Throws<InfoException>(() => sut.Authenticate(null, "test"));
            }

            [Fact]
            public void EmptyPassword_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                context.AddUser("Julius", "Testimus", "tester", "test");

                //Assert & Act
                Assert.Throws<InfoException>(() => sut.Authenticate("tester", ""));
            }

            [Fact]
            public void WrongPassword_ThrowsException()
            {

                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                context.AddUser("Julius", "Testimus", "tester", "test");


                //Assert & Act
                Assert.Throws<InfoException>(() => sut.Authenticate("tester", "TEST"));
            }

            [Fact]
            public void UserNotExists_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);

                //Assert & Act
                Assert.Throws<InfoException>(() => sut.Authenticate("tester", "test"));
            }
        }

        public class AddTest : UserRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = new User
                {
                    Firstname = "Julius",
                    Lastname = "Testimus",
                    Username = "tester"
                };

                //Act
                var result =sut.Add(user, "test");

                //Assert
                Assert.NotNull(result);
                Assert.NotNull(result.PasswordHash);
                Assert.NotNull(result.PasswordSalt);

                Assert.Same(user.Firstname, result.Firstname);
                Assert.Same(user.Lastname, result.Lastname);
                Assert.Same(user.Username, result.Username);
            }

            [Fact]
            public void EmptyPassword_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = new User
                {
                    Firstname = "Julius",
                    Lastname = "Testimus",
                    Username = "tester"
                };

                //Act & Assert
                Assert.Throws<InfoException>(() => sut.Add(user, ""));
            }

            [Fact]
            public void UsernameExisits_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                context.AddUser("Julius", "Testimus", "tester", "test");

                var user = new User
                {
                    Firstname = "Julius",
                    Lastname = "Testimus",
                    Username = "tester"
                };

                //Act & Assert
                Assert.Throws<InfoException>(() => sut.Add(user, "test"));
            }
        }

        public class Remove : UserRepositoryTest
        {
            [Fact]
            public void Valid_DeletesUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                //Act
                sut.Remove(user.Id);
                context.SaveChanges();

                //Assert
                Assert.Null(context.Find<User>(user.Id));
            }

            [Fact]
            public void NotExisits_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.Remove(Guid.NewGuid()));
            }
        }

        public class FindTest : UserRepositoryTest
        {
            [Fact]
            public void ByFirstname_ReturnsUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                //Act
                var result = sut.Find(u => u.Firstname == "Julius").FirstOrDefault();

                //Assert
                Assert.Same(user, result);
            }

            [Fact]
            public void ByFirstnameAndLastname_ReturnsUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                //Act
                var result = sut.Find(u => u.Firstname == "Julius" && u.Username == "tester").FirstOrDefault();

                //Assert
                Assert.Same(user, result);
            }

            [Fact]
            public void MultipleResults_ReturnsUserList()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user1 = context.AddUser("Julius", "Testimus", "tester", "test");
                var user2 = context.AddUser("Markus", "Testimus", "tester2", "test2");

                //Act
                var result = sut.Find(u => u.Lastname == "Testimus");

                //Assert
                Assert.Contains(user1, result);
                Assert.Contains(user2, result);
            }
        }

        public class GetAllTest : UserRepositoryTest
        {
            [Fact]
            public void HasUsers_ReturnsUserList()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user1 = context.AddUser("Julius", "Testimus", "tester", "test");
                var user2 = context.AddUser("Markus", "Testimus", "tester2", "test2");

                //Act
                var result = sut.GetAll();

                //Assert
                Assert.Contains(user1, result);
                Assert.Contains(user2, result);
            }

            [Fact]
            public void NoUsers_ReturnsUserList()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);

                //Act
                var result = sut.GetAll();

                //Assert
                Assert.Empty(result);
            }
        }

        public class GetByIdTest : UserRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                //Act
                var result = sut.GetById(user.Id);

                //Assert
                Assert.Same(user, result);
            }

            [Fact]
            public void NotExists_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.GetById(Guid.NewGuid()));
            }
        }

        public class UpdateTest : UserRepositoryTest
        {
            [Fact]
            public void Valid_ReturnsUpdatedUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user = context.AddUser("Julius", "Testimus", "tester", "test");

                var updatedUser = new User
                {
                    Id = user.Id,
                    Firstname = "Julius Updated",
                    Lastname = "Testimus Updated",
                    Username = "tester_updated"
                };

                //Act
                var result = sut.Update(updatedUser, "test_updated");
                context.SaveChanges();

                //Assert
                Assert.NotNull(result);

                Assert.True(PasswordHelper.VerifyPasswordHash("test_updated", result.PasswordHash, result.PasswordSalt));

                Assert.Same(updatedUser.Firstname, result.Firstname);
                Assert.Same(updatedUser.Lastname, result.Lastname);
                Assert.Same(updatedUser.Username, result.Username);
            }

            [Fact]
            public void UsernameTaken_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var user1 = context.AddUser("Julius", "Testimus", "tester", "test");
                var user2 = context.AddUser("Maximus", "Testimus", "tester2", "test2");
                var updatedUser = new User
                {
                    Id = user1.Id,
                    Username = "tester2"
                };

                //Assert & Act
                Assert.Throws<InfoException>(() => sut.Update(updatedUser, "test"));
            }

            [Fact]
            public void NotExists_ThrowsException()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextHelper.ContextOptions());
                var sut = new UserRepository(context);
                var updatedUser = new User
                {
                    Id = Guid.NewGuid()
                };

                //Act & Assert
                Assert.Throws<NotFoundException>(() => sut.Update(updatedUser));
            }
        }
    }
}
