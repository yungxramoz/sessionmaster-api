using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SessionMaster.API.Controllers;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Models;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModUser
{
    public class UserControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        private readonly Mock<IOptions<AppSettings>> _appSettings;

        public UserControllerTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Complete()).Returns(1);

            _mapper = new Mock<IMapper>();

            AppSettings app = new AppSettings() { Secret = "TEST" };
            _appSettings = new Mock<IOptions<AppSettings>>();
            _appSettings.Setup(ap => ap.Value).Returns(app);
        }

        public class GetTest : UserControllerTest
        {
            [Fact]
            public void All_Valid_ReturnsOk()
            {
                //Arrange
                var userList = new List<User>{
                    new User
                    {
                        Id = Guid.NewGuid()
                    }
                };
                var userModelList = new List<UserModel>{
                    new UserModel
                    {
                        Id = Guid.NewGuid()
                    } 
                };

                _unitOfWork.Setup(uow => uow.Users.GetAll()).Returns(userList);
                _mapper.Setup(m => m.Map<IList<UserModel>>(It.IsAny<List<User>>())).Returns(userModelList);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Get();

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(userModelList, okObjectResult.Value);
            }

            [Fact]
            public void ById_Valid_ReturnsOk()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid()
                };
                var userModel = new UserModel
                {
                    Id = Guid.NewGuid()
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(user.Id)).Returns(user);
                _mapper.Setup(m => m.Map<UserModel>(user)).Returns(userModel);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Get(user.Id);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(userModel, okObjectResult.Value);
            }

            [Fact]
            public void ById_NotFoundException_ReturnsNotFound()
            {
                //Arrange
                var exception = new NotFoundException("test");

                _unitOfWork.Setup(uow => uow.Users.GetById(It.IsAny<Guid>())).Throws(exception);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Get(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class AuthenticateTest : UserControllerTest
        {

        }
        public class RegisterTest : UserControllerTest
        {

        }
        public class PutTest : UserControllerTest
        {

        }
        public class DeleteTest : UserControllerTest
        {

        }
    }
}
