using SessionMaster.BLL.ModAnonymousUser;
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

namespace SessionMaster.UnitTests.Domains.ModAnonymousUser
{
    public class AnonymousUserRepositoryTest
    {
        //All methods are tested by other unit tests, because the AnonymousUserRepository only uses the GenericRepository implementation
        public class GetTest : AnonymousUserRepositoryTest
        {
            [Fact]
            public void ById_Valid_ReturnsAnonymousUser()
            {
                //Arrange
                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var user = context.AddAnonymousUser(RandomStringTestHelper.Generate());

                var sut = new AnonymousUserRepository(context);

                //Act
                var result = sut.GetById(user.Id);

                //Assert
                Assert.Equal(result.Name, user.Name);
                Assert.Equal(result.Id, user.Id);
            }
        }
    }
}