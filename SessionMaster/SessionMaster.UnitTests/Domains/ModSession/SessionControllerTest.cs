using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SessionMaster.API.ModSession;
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

namespace SessionMaster.UnitTests.Domains.ModSession
{
    public class SessionControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;

        public SessionControllerTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Complete()).Returns(1);

            _mapper = new Mock<IMapper>();
        }

        public class GetTest : SessionControllerTest
        {
            [Fact]
            public void Valid_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = "Zulius",
                    Lastname = "Testimus"
                };
                var anonymous = new AnonymousUser
                {
                    Id = Guid.NewGuid(),
                    Name = "Anonymous"
                };

                var session = new Session
                {
                    Id = sessionId,
                };
                session.SessionUsers = new List<SessionUser>
                {
                    new SessionUser
                    {
                        Session = session,
                        SessionId = sessionId,
                        User = user,
                        UserId = user.Id
                    }
                };
                session.SessionAnonymousUsers = new List<SessionAnonymousUser>
                {
                    new SessionAnonymousUser
                    {
                        Session = session,
                        SessionId = sessionId,
                        AnonymousUser = anonymous,
                        AnonymousUserId = anonymous.Id
                    }
                };

                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };

                _mapper.Setup(m => m.Map<SessionModel>(session)).Returns(sessionModel);
                _unitOfWork.Setup(uow => uow.Sessions.GetById(sessionId, null)).Returns(session);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);

                //Act
                var result = sut.Get(sessionId);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);

                Assert.Equal(anonymous.Name, returnModel.Users.First().Name);
                Assert.Null(returnModel.Users.First().Id);

                Assert.Equal($"{user.Firstname} {user.Lastname}", returnModel.Users.Last().Name);
                Assert.Equal(user.Id, returnModel.Users.Last().Id);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test");

                _unitOfWork.Setup(uow => uow.Sessions.GetById(It.IsAny<Guid>(), null)).Throws(exception);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);

                //Act
                var result = sut.Get(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class RegisterTest : SessionControllerTest
        {
            [Fact]
            public void Valid_Unauthorized_New_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var anonymous = new AnonymousUser
                {
                    Id = Guid.NewGuid(),
                    Name = "Anonymous"
                };
                var session = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                var sessionWithUser = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>
                    {
                        new SessionAnonymousUser
                        {
                            Session = session,
                            SessionId = sessionId,
                            AnonymousUser = anonymous,
                            AnonymousUserId = anonymous.Id
                        }
                    }
                };
                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };
                var sessionplans = new List<Sessionplan>
                {
                    new Sessionplan
                    {
                        Id = Guid.NewGuid(),
                        Name = RandomStringTestHelper.Generate(),
                        Sessions = new List<Session>(),
                    }
                };
                sessionplans.First().Sessions.Add(session);

                _mapper.Setup(m => m.Map<SessionModel>(sessionWithUser)).Returns(sessionModel);

                _unitOfWork.Setup(uow => uow.Sessionplans.Get(
                        It.IsAny<Expression<Func<Sessionplan, bool>>>(),
                        null,
                        It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())).Returns(sessionplans);
                _unitOfWork.Setup(uow => uow.AnonymousUsers.Add(It.IsAny<AnonymousUser>())).Returns(anonymous);
                _unitOfWork.Setup(uow => uow.Sessions.RegisterAnonymous(anonymous.Id, sessionId)).Returns(sessionWithUser);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Register(sessionId, new UserResponseSessionModel
                {
                    Name = anonymous.Name
                });

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);
                Assert.Equal(anonymous.Name, returnModel.Users.First().Name);
            }

            [Fact]
            public void Valid_Unauthorized_Exisiting_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var anonymous = new AnonymousUser
                {
                    Id = Guid.NewGuid(),
                    Name = "Anonymous"
                };
                var session = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                session.SessionAnonymousUsers = new List<SessionAnonymousUser>
                {
                    new SessionAnonymousUser
                    {
                        Session = session,
                        SessionId = sessionId,
                        AnonymousUser = anonymous,
                        AnonymousUserId = anonymous.Id
                    }
                };
                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };
                var sessionplans = new List<Sessionplan>
                {
                    new Sessionplan
                    {
                        Id = Guid.NewGuid(),
                        Name = RandomStringTestHelper.Generate(),
                        Sessions = new List<Session>(),
                    }
                };
                sessionplans.First().Sessions.Add(session);

                _mapper.Setup(m => m.Map<SessionModel>(session)).Returns(sessionModel);

                _unitOfWork.Setup(uow => uow.Sessionplans.Get(
                        It.IsAny<Expression<Func<Sessionplan, bool>>>(),
                        null,
                        It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())).Returns(sessionplans);
                _unitOfWork.Setup(uow => uow.Sessions.RegisterAnonymous(anonymous.Id, sessionId)).Returns(session);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Register(sessionId, new UserResponseSessionModel
                {
                    Name = anonymous.Name
                });

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);
                Assert.Equal(anonymous.Name, returnModel.Users.First().Name);
            }

            [Fact]
            public void NoBody_Unauthorized_ReturnsBadRequest400()
            {
                //Arrange
                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Register(Guid.NewGuid());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.NotNull(badRequestObjectResult.Value);
            }

            [Fact]
            public void Valid_Authorized_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = "Zulius",
                    Lastname = "Testimus"
                };
                var session = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                session.SessionUsers = new List<SessionUser>
                {
                    new SessionUser
                    {
                        Session = session,
                        SessionId = sessionId,
                        User = user,
                        UserId = user.Id
                    }
                };
                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };

                _mapper.Setup(m => m.Map<SessionModel>(session)).Returns(sessionModel);
                _unitOfWork.Setup(uow => uow.Sessions.Register(user.Id, sessionId)).Returns(session);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();
                sut.SetAuthorizedUser(user.Id);

                //Act
                var result = sut.Register(sessionId);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);
                Assert.Equal($"{user.Firstname} {user.Lastname}", returnModel.Users.First().Name);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException(RandomStringTestHelper.Generate());
                var user = new User
                {
                    Id = Guid.NewGuid()
                };

                _unitOfWork.Setup(uow => uow.Sessions.Register(It.IsAny<Guid>(), It.IsAny<Guid>())).Throws(exception);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();
                sut.SetAuthorizedUser(user.Id);

                //Act
                var result = sut.Register(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class CancelTest : SessionControllerTest
        {
            [Fact]
            public void Valid_Unauthorized_LastSession_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var anonymous = new AnonymousUser
                {
                    Id = Guid.NewGuid(),
                    Name = "Anonymous",
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                var session = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                var sessionAnonymousUser = new SessionAnonymousUser
                {
                    Session = session,
                    SessionId = sessionId,
                    AnonymousUser = anonymous,
                    AnonymousUserId = anonymous.Id
                };
                var sessionWithUser = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>
                    {
                        sessionAnonymousUser
                    }
                };
                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };
                var sessionplans = new List<Sessionplan>
                {
                    new Sessionplan
                    {
                        Id = Guid.NewGuid(),
                        Name = RandomStringTestHelper.Generate(),
                        Sessions = new List<Session>(),
                    }
                };
                sessionplans.First().Sessions.Add(sessionWithUser);
                anonymous.SessionAnonymousUsers.Add(sessionAnonymousUser);
                var calledRemove = false;

                _mapper.Setup(m => m.Map<SessionModel>(session)).Returns(sessionModel);

                _unitOfWork.Setup(uow => uow.Sessionplans.Get(
                        It.IsAny<Expression<Func<Sessionplan, bool>>>(),
                        null,
                        It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())).Returns(sessionplans);
                _unitOfWork.Setup(uow => uow.Sessions.CancelAnonymous(anonymous.Id, sessionId)).Returns(session);

                _unitOfWork.Setup(uow => uow.AnonymousUsers.Remove(anonymous.Id)).Callback(() => calledRemove = true).Verifiable();

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Cancel(sessionId, new UserResponseSessionModel
                {
                    Name = anonymous.Name
                });

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);
                Assert.Empty(returnModel.Users);
                Assert.True(calledRemove);
            }

            [Fact]
            public void NoBody_Unauthorized_ReturnsBadRequest400()
            {
                //Arrange
                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Cancel(Guid.NewGuid());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.NotNull(badRequestObjectResult.Value);
            }

            [Fact]
            public void UnknownName_Unauthorized_ReturnsNotFound404()
            {
                //Arrange
                _unitOfWork.Setup(uow => uow.Sessionplans.Get(
                        It.IsAny<Expression<Func<Sessionplan, bool>>>(),
                        null,
                        It.IsAny<Func<IQueryable<Sessionplan>, IIncludableQueryable<Sessionplan, object>>>())).Returns(new List<Sessionplan>());
                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();

                //Act
                var result = sut.Cancel(Guid.NewGuid(), new UserResponseSessionModel
                {
                    Name = RandomStringTestHelper.Generate()
                });

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.NotNull(notFoundObjectResult.Value);
            }

            [Fact]
            public void Valid_Authorized_ReturnsOk200()
            {
                //Arrange
                var sessionId = Guid.NewGuid();
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = "Zulius",
                    Lastname = "Testimus"
                };
                var session = new Session
                {
                    Id = sessionId,
                    SessionUsers = new List<SessionUser>(),
                    SessionAnonymousUsers = new List<SessionAnonymousUser>()
                };
                var sessionModel = new SessionModel
                {
                    Id = sessionId
                };

                _mapper.Setup(m => m.Map<SessionModel>(session)).Returns(sessionModel);
                _unitOfWork.Setup(uow => uow.Sessions.Cancel(user.Id, sessionId)).Returns(session);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();
                sut.SetAuthorizedUser(user.Id);

                //Act
                var result = sut.Cancel(sessionId);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnModel = (SessionModel)okObjectResult.Value;
                Assert.Equal(sessionModel.Id, returnModel.Id);
                Assert.Empty(returnModel.Users);
            }

            [Fact]
            public void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException(RandomStringTestHelper.Generate());
                var user = new User
                {
                    Id = Guid.NewGuid()
                };

                _unitOfWork.Setup(uow => uow.Sessions.Cancel(It.IsAny<Guid>(), It.IsAny<Guid>())).Throws(exception);

                var sut = new SessionController(_unitOfWork.Object, _mapper.Object);
                sut.SetDefaultHttpContext();
                sut.SetAuthorizedUser(user.Id);

                //Act
                var result = sut.Cancel(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }
    }
}