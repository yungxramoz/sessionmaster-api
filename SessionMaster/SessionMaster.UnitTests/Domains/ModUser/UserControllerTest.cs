using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SessionMaster.API.ModUser;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Models;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
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

            AppSettings app = new AppSettings() { Secret = "Un1t TesT S3cR3T" };
            _appSettings = new Mock<IOptions<AppSettings>>();
            _appSettings.Setup(ap => ap.Value).Returns(app);
        }

        public class GetTest : UserControllerTest
        {
            [Fact]
            public void All_Valid_ReturnsOk200()
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

                _unitOfWork.Setup(uow => uow.Users.Get(null, null, null)).Returns(userList);
                _mapper.Setup(m => m.Map<IList<UserModel>>(It.IsAny<List<User>>())).Returns(userModelList);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Get();

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(userModelList, okObjectResult.Value);
            }

            [Fact]
            public void ById_Valid_ReturnsOk200()
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
            public void ById_NotFoundException_ReturnsNotFound404()
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
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid()
                };
                var userModel = new UserModel
                {
                    Id = user.Id
                };
                var authModel = new AuthenticateModel
                {
                    Username = "test",
                    Password = "test"
                };

                _unitOfWork.Setup(uow => uow.Users.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(user);
                _mapper.Setup(m => m.Map<UserModel>(It.IsAny<User>())).Returns(userModel);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Authenticate(authModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var resultValue = (UserModel)okObjectResult.Value;
                Assert.Equal(userModel.Id, resultValue.Id);
                Assert.NotNull(resultValue.Token);
            }

            [Fact]
            public void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test message");
                var authModel = new AuthenticateModel
                {
                    Username = "test",
                    Password = "test"
                };

                _unitOfWork.Setup(uow => uow.Users.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Authenticate(authModel);

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(exception.Message, badRequestObjectResult.Value);
            }
        }

        public class RegisterTest : UserControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid()
                };
                var registrationModel = new RegistrationModel
                {
                    Username = "test",
                    Password = "test",
                    Firstname = "Test",
                    Lastname = "Test"
                };

                _unitOfWork.Setup(uow => uow.Users.Add(user, registrationModel.Password)).Returns(user);
                _mapper.Setup(m => m.Map<User>(registrationModel)).Returns(user);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Register(registrationModel);

                //Assert
                Assert.IsType<OkResult>(result);
            }

            [Fact]
            public void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test message");
                var user = new User
                {
                    Id = Guid.NewGuid()
                };
                var registrationModel = new RegistrationModel
                {
                    Username = "test",
                    Password = "test",
                    Firstname = "Test",
                    Lastname = "Test"
                };

                _unitOfWork.Setup(uow => uow.Users.Add(user, registrationModel.Password)).Throws(exception);
                _mapper.Setup(m => m.Map<User>(registrationModel)).Returns(user);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Register(registrationModel);

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Same(exception.Message, badRequestObjectResult.Value);
            }
        }

        public class PutTest : UserControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid()
                };
                var updateModel = new UpdateUserModel
                {
                    Username = "test",
                    Password = "test",
                    Firstname = "Test",
                    Lastname = "Test"
                };
                var userModel = new UserModel
                {
                    Id = user.Id,
                    Firstname = updateModel.Firstname,
                    Lastname = updateModel.Lastname,
                    Username = updateModel.Username
                };

                _unitOfWork.Setup(uow => uow.Users.Update(user, updateModel.Password)).Returns(user);
                _mapper.Setup(m => m.Map<User>(updateModel)).Returns(user);
                _mapper.Setup(m => m.Map<UserModel>(user)).Returns(userModel);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Put(user.Id, updateModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(userModel, okObjectResult.Value);
            }

            [Fact]
            public void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test message");
                var user = new User();

                _unitOfWork.Setup(uow => uow.Users.Update(It.IsAny<User>(), It.IsAny<string>())).Throws(exception);
                _mapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserModel>())).Returns(user);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Put(user.Id, new UpdateUserModel());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Same(exception.Message, badRequestObjectResult.Value);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test message");
                var user = new User();

                _unitOfWork.Setup(uow => uow.Users.Update(It.IsAny<User>(), It.IsAny<string>())).Throws(exception);
                _mapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserModel>())).Returns(user);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Put(user.Id, new UpdateUserModel());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class DeleteTest : UserControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                _unitOfWork.Setup(uow => uow.Users.Remove(It.IsAny<Guid>())).Verifiable();
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Delete(Guid.NewGuid());

                //Assert
                Assert.IsType<OkResult>(result);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test message");
                _unitOfWork.Setup(uow => uow.Users.Remove(It.IsAny<Guid>())).Throws(exception);
                var sut = new UserController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = sut.Delete(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }
        }
    }
}