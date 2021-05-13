using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Moq;
using SessionMaster.API.ModBoardGame;
using SessionMaster.API.ModBoardGame.ViewModels;
using SessionMaster.API.ModUser;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.Common.Models;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.Domains.ModUser;
using SessionMaster.UnitTests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModBoardGame
{
    public class BoardGameControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        private readonly Mock<IOptions<AppSettings>> _appSettings;
        private string _clientId;

        public BoardGameControllerTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Complete()).Returns(1);

            _mapper = new Mock<IMapper>();

            AppSettings app = new AppSettings() { BgaClientId = "Un1tTestCli3ntId" };
            _appSettings = new Mock<IOptions<AppSettings>>();
            _appSettings.Setup(ap => ap.Value).Returns(app);

            _clientId = _appSettings.Object.Value.BgaClientId;
        }

        public class GetFilteredTest : BoardGameControllerTest
        {
            [Fact]
            public async void Valid_ReturnsOk200()
            {
                //Arrange
                var bgaList = new List<BoardGameAtlasGameDetails>();
                var boardGameList = new List<BoardGameModel>();

                _unitOfWork.Setup(uow => uow.BoardGames.GetAll("", _clientId)).ReturnsAsync(bgaList);
                _mapper.Setup(m => m.Map<IList<BoardGameModel>>(bgaList)).Returns(boardGameList);
                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetFiltered();

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(boardGameList, okObjectResult.Value);
            }

            [Fact]
            public async void ValidWIthFilter_ReturnsOk200()
            {
                //Arrange
                var bgaList = new List<BoardGameAtlasGameDetails>();
                var boardGameList = new List<BoardGameModel>();
                var filter = "testFilter";

                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByName(filter), _clientId)).ReturnsAsync(bgaList);
                _mapper.Setup(m => m.Map<IList<BoardGameModel>>(bgaList)).Returns(boardGameList);
                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetFiltered(filter);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(boardGameList, okObjectResult.Value);
            }

            [Fact]
            public async void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test");

                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(It.IsAny<string>(), _clientId)).ThrowsAsync(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetFiltered(RandomStringTestHelper.Generate());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(exception.Message, badRequestObjectResult.Value);
            }
        }

        public class GetTest : BoardGameControllerTest
        {
            [Fact]
            public async void Valid_ReturnsEmptyOk200()
            {
                //Arrange
                var bgaDetails = new BoardGameAtlasGameDetails();
                var boardGameModel = new BoardGameModel();
                var id = RandomStringTestHelper.Generate();

                _unitOfWork.Setup(uow => uow.BoardGames.GetById(id, _clientId)).ReturnsAsync(bgaDetails);
                _mapper.Setup(m => m.Map<BoardGameModel>(bgaDetails)).Returns(boardGameModel);
                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.Get(id);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Same(boardGameModel, okObjectResult.Value);
            }

            [Fact]
            public async void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test");

                _unitOfWork.Setup(uow => uow.BoardGames.GetById(It.IsAny<string>(), _clientId)).ThrowsAsync(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.Get(RandomStringTestHelper.Generate());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(exception.Message, badRequestObjectResult.Value);
            }

            [Fact]
            public async void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test");

                _unitOfWork.Setup(uow => uow.BoardGames.GetById(It.IsAny<string>(), _clientId)).ThrowsAsync(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.Get(RandomStringTestHelper.Generate());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class GetCollectionTest : BoardGameControllerTest
        {
            [Fact]
            public async void ValidEmptyCollection_ReturnsEmptyOk200()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = null
                };
                var boardGameList = new List<BoardGameModel>();

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    user.Id,
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Returns(user);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetCollection(user.Id);

                //Assert
                var okResult = Assert.IsType<OkResult>(result);
            }

            [Fact]
            public async void ValidHasCollection_ReturnsOk200()
            {
                //Arrange
                var boardgameIds = new List<string>
                {
                    RandomStringTestHelper.Generate()
                };
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame
                        {
                            BoardGameId = boardgameIds[0]
                        }
                    }
                };
                var boardGameList = new List<BoardGameModel>();
                var bgaDetails = new List<BoardGameAtlasGameDetails>();
                var boardGameModels = new List<BoardGameModel>();

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    user.Id,
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).Returns(user);
                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByIds(boardgameIds), _clientId)).ReturnsAsync(bgaDetails);
                _mapper.Setup(m => m.Map<IList<BoardGameModel>>(bgaDetails)).Returns(boardGameList);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetCollection(user.Id);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(boardGameList.OrderBy(bg => bg.Name), okObjectResult.Value);
            }

            [Fact]
            public async void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test");
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame()
                    }
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Returns(user);

                //only GetAll throws InfoExceptions
                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(It.IsAny<string>(), _clientId)).ThrowsAsync(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetCollection(Guid.NewGuid());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(exception.Message, badRequestObjectResult.Value);
            }

            [Fact]
            public async void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test");

                //only GetById throws a NotFoundException
                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.GetCollection(Guid.NewGuid());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal(exception.Message, notFoundObjectResult.Value);
            }
        }

        public class PostToCollectionTest : BoardGameControllerTest
        {
            [Fact]
            public async void Valid_ReturnsOk200()
            {
                //Arrange
                var boardGameId = RandomStringTestHelper.Generate();
                var boardgameIds = new List<string>
                {
                    boardGameId
                };
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame
                        {
                            BoardGameId = boardGameId
                        }
                    }
                };
                var boardGameList = new List<BoardGameModel>();
                var bgaDetails = new List<BoardGameAtlasGameDetails>();
                var boardGameModels = new List<BoardGameModel>();
                var bgaDetail = new BoardGameAtlasGameDetails
                {
                    Id = boardGameId
                };
                var addToCollectionModel = new AddToCollectionModel
                {
                    BoardGameId = boardGameId
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    user.Id,
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).Returns(user);
                _unitOfWork.Setup(uow => uow.BoardGames.GetById(boardGameId, _clientId)).ReturnsAsync(bgaDetail);
                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByIds(boardgameIds), _clientId)).ReturnsAsync(bgaDetails);
                _mapper.Setup(m => m.Map<IList<BoardGameModel>>(bgaDetails)).Returns(boardGameList);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.PostToCollection(user.Id, addToCollectionModel);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(boardGameList.OrderBy(bg => bg.Name), okObjectResult.Value);
            }

            [Fact]
            public async void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test message");
                var addBoardGame = new AddToCollectionModel
                {
                    BoardGameId = RandomStringTestHelper.Generate()
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    null
                )).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.PostToCollection(Guid.NewGuid(), addBoardGame);

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Same(exception.Message, badRequestObjectResult.Value);
            }

            [Fact]
            public async void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test message");
                var addBoardGame = new AddToCollectionModel
                {
                    BoardGameId = RandomStringTestHelper.Generate()
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    null
                )).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.PostToCollection(Guid.NewGuid(), addBoardGame);

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }

            [Fact]
            public async void DbUpdateException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new DbUpdateException("this message will be overridden");
                var addBoardGame = new AddToCollectionModel
                {
                    BoardGameId = RandomStringTestHelper.Generate()
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).Returns(new User());
                _unitOfWork.Setup(uow => uow.BoardGames.GetById(It.IsAny<string>(), _clientId)).ReturnsAsync(new BoardGameAtlasGameDetails());
                _unitOfWork.Setup(uow => uow.Complete()).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.PostToCollection(Guid.NewGuid(), addBoardGame);

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                // the error message gets overriden / more specified
                Assert.NotEqual(exception.Message, badRequestObjectResult.Value);
            }
        }

        public class DeleteFromCollection : BoardGameControllerTest
        {
            [Fact]
            public async void Valid_ReturnsOk200()
            {
                //Arrange
                var boardGameId = RandomStringTestHelper.Generate();
                var boardgameIds = new List<string>
                {
                    boardGameId
                };
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame
                        {
                            BoardGameId = boardGameId
                        }
                    }
                };
                var boardGameList = new List<BoardGameModel>();
                var bgaDetails = new List<BoardGameAtlasGameDetails>();
                var boardGameModels = new List<BoardGameModel>();
                var bgaDetail = new BoardGameAtlasGameDetails
                {
                    Id = boardGameId
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    user.Id,
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).Returns(user);
                _unitOfWork.Setup(uow => uow.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByIds(boardgameIds), _clientId)).ReturnsAsync(bgaDetails);
                _mapper.Setup(m => m.Map<IList<BoardGameModel>>(bgaDetails)).Returns(boardGameList);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.DeleteFromCollection(user.Id, boardGameId);

                //Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(boardGameList.OrderBy(bg => bg.Name), okObjectResult.Value);
            }

            [Fact]
            public async void BoardGameNotExists_ReturnsNotFound404()
            {
                //Arrange
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame
                        {
                            BoardGameId = RandomStringTestHelper.Generate()
                        }
                    }
                };

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Returns(user);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.DeleteFromCollection(Guid.NewGuid(), RandomStringTestHelper.Generate());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                //a specified error message is returned
                Assert.NotNull(notFoundObjectResult.Value);
            }

            [Fact]
            public async void InfoException_ReturnsBadRequest400()
            {
                //Arrange
                var exception = new InfoException("test message");

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.DeleteFromCollection(Guid.NewGuid(), RandomStringTestHelper.Generate());

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Same(exception.Message, badRequestObjectResult.Value);
            }

            [Fact]
            public async void NotFoundException_ReturnsNotFound404()
            {
                //Arrange
                var exception = new NotFoundException("test message");

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()
                )).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.DeleteFromCollection(Guid.NewGuid(), RandomStringTestHelper.Generate());

                //Assert
                var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Same(exception.Message, notFoundObjectResult.Value);
            }

            [Fact]
            public async void DbUpdateException_ReturnsBadRequest400()
            {
                //Arrange
                var boardGameId = RandomStringTestHelper.Generate();
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    BoardGames = new List<UserBoardGame>
                    {
                        new UserBoardGame
                        {
                            Id = Guid.NewGuid(),
                            BoardGameId = boardGameId
                        }
                    }
                };

                var exception = new DbUpdateException("this message will be overridden");

                _unitOfWork.Setup(uow => uow.Users.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>())
                ).Returns(user);
                _unitOfWork.Setup(uow => uow.BoardGames.Remove(It.IsAny<Guid>())).Verifiable();
                _unitOfWork.Setup(uow => uow.Complete()).Throws(exception);

                var sut = new BoardGameController(_unitOfWork.Object, _mapper.Object, _appSettings.Object);

                //Act
                var result = await sut.DeleteFromCollection(Guid.NewGuid(), boardGameId);

                //Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                // the error message gets overriden / more specified
                Assert.NotEqual(exception.Message, badRequestObjectResult.Value);
            }
        }
    }
}