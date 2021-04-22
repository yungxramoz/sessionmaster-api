using SessionMaster.BLL.ModUser;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.Domains.Core;
using System;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModUser
{
    public class UserRepositoryTest : SessionMasterContextTest
    {
        private UserRepository _sut;

        public UserRepositoryTest()
        {
            _sut = new UserRepository(_context);
        }

        public class AddTest : UserRepositoryTest
        {
            [Fact]
            public void Valid()
            {
                var id = Guid.NewGuid();
                var user = new User
                {
                    Id = id
                };

                _sut.Add(user);

                Assert.NotNull(_context.Find<User>(id));
            }
        }

        public class Remove : UserRepositoryTest
        {
        }

        public class FindTest : UserRepositoryTest
        {
        }

        public class GetAllTest : UserRepositoryTest
        {
        }

        public class GetByIdTest : UserRepositoryTest
        {
        }

        public class UpdateTest : UserRepositoryTest
        {
        }
    }
}
