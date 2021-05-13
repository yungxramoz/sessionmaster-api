using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SessionMaster.API.ModSession.ViewModels;
using SessionMaster.API.ModSessionplan;
using SessionMaster.API.ModSessionplan.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModSessionplan
{
    public class SessionplanControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;

        public SessionplanControllerTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Complete()).Returns(1);

            _mapper = new Mock<IMapper>();
        }

        public class GetTest : SessionplanControllerTest
        {
            [Fact]
            public void All_Valid_ReturnsOk200()
            {
                //Arrange
                var userId = Guid.NewGuid();

                var sessionplans = new List<Sessionplan>();
                var sessionplanModelList = new List<SessionplanOverviewModel>();

                _mapper.Setup(m => m.Map<IList<SessionplanOverviewModel>>(sessionplans)).Returns(sessionplanModelList);
                _unitOfWork.Setup(uow => uow.Sessionplans.Get(It.IsAny<Expression<Func<Sessionplan, bool>>>(), null, null)).Returns(sessionplans);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);
                sut.SetAuthorizedUser(userId);

                //Act
                var result = sut.Get();

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(sessionplanModelList.OrderBy(sp => sp.Name), okObjectResult.Value);
            }

            [Fact]
            public void ById_Valid_ReturnsOk200()
            {
                //Arrange
                var planId = Guid.NewGuid();

                var sessionplan = new Sessionplan
                {
                    Id = planId
                };
                var sessionplanModel = new SessionplanDetailModel
                {
                    Id = planId
                };

                _mapper.Setup(m => m.Map<SessionplanDetailModel>(sessionplan)).Returns(sessionplanModel);
                _unitOfWork.Setup(uow => uow.Sessionplans.GetById(planId,
                    It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())
                ).Returns(sessionplan);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);

                //Act
                var result = sut.Get(planId);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(sessionplanModel, okObjectResult.Value);
            }

            [Fact]
            public void ById_NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test");

                _unitOfWork.Setup(uow => uow.Sessionplans.GetById(It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())
                ).Throws(exception);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);

                //Act
                var result = sut.Get(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class PostTest : SessionplanControllerTest
        {
            [Fact]
            public void Valid_Unauthorized_ReturnsOk200()
            {
                //Arrange
                var planId = Guid.NewGuid();
                var name = RandomStringTestHelper.Generate();

                var sessionplan = new Sessionplan
                {
                    Id = planId,
                    Name = name,
                    Sessions = new List<Session>()
                };
                var addModel = new AddSessionplanModel
                {
                    Name = name,
                    Sessions = new List<SessionModel>()
                };

                var sessionplanModel = new SessionplanDetailModel
                {
                    Id = planId,
                    Name = name,
                    Sessions = new List<SessionModel>()
                };

                _unitOfWork.Setup(uow => uow.Sessionplans.Add(sessionplan)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<Sessionplan>(addModel)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<SessionplanDetailModel>(sessionplan)).Returns(sessionplanModel);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Post(addModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(sessionplanModel, okObjectResult.Value);
            }

            [Fact]
            public void Valid_Authorized_ReturnsOk200()
            {
                //Arrange
                var userId = Guid.NewGuid();
                var planId = Guid.NewGuid();
                var name = RandomStringTestHelper.Generate();

                var sessionplan = new Sessionplan
                {
                    Id = planId,
                    UserId = userId,
                    Name = name,
                    Sessions = new List<Session>()
                };
                var addModel = new AddSessionplanModel
                {
                    Name = name,
                    Sessions = new List<SessionModel>()
                };

                var sessionplanModel = new SessionplanDetailModel
                {
                    Id = planId,
                    Name = name,
                    Sessions = new List<SessionModel>()
                };

                _unitOfWork.Setup(uow => uow.Sessionplans.Add(sessionplan)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<Sessionplan>(addModel)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<SessionplanDetailModel>(sessionplan)).Returns(sessionplanModel);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);
                sut.SetAuthorizedUser(userId);

                //Act
                var result = sut.Post(addModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(sessionplanModel, okObjectResult.Value);
            }
        }

        public class PutTest : SessionplanControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                var userId = Guid.NewGuid();
                var planId = Guid.NewGuid();
                var name = RandomStringTestHelper.Generate();

                var sessionplan = new Sessionplan
                {
                    Id = planId,
                    UserId = userId
                };
                var updateModel = new UpdateSessionplanModel
                {
                    Name = name
                };
                var sessionplanModel = new SessionplanDetailModel
                {
                    Id = planId,
                    Name = name,
                    Sessions = new List<SessionModel>()
                };

                _unitOfWork.Setup(uow => uow.Sessionplans.Update(sessionplan)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<Sessionplan>(updateModel)).Returns(sessionplan);
                _mapper.Setup(m => m.Map<SessionplanDetailModel>(sessionplan)).Returns(sessionplanModel);

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);
                sut.SetAuthorizedUser(userId);

                //Act
                var result = sut.Put(planId, updateModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(sessionplanModel, okObjectResult.Value);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test message");

                _unitOfWork.Setup(uow => uow.Sessionplans.Update(It.IsAny<Sessionplan>())).Throws(exception);
                _mapper.Setup(m => m.Map<Sessionplan>(It.IsAny<UpdateSessionplanModel>())).Returns(new Sessionplan());

                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);
                sut.SetAuthorizedUser(Guid.NewGuid());

                //Act
                var result = sut.Put(Guid.NewGuid(), new UpdateSessionplanModel());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class DeleteTest : SessionplanControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                _unitOfWork.Setup(uow => uow.Sessionplans.Remove(It.IsAny<Guid>())).Verifiable();
                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);

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

                _unitOfWork.Setup(uow => uow.Sessionplans.Remove(It.IsAny<Guid>())).Throws(exception);
                var sut = new SessionplanController(_unitOfWork.Object, _mapper.Object);

                //Act
                var result = sut.Delete(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }
        }
    }
}