using Moq;
using RestSharp;
using SessionMaster.BLL.ModBoardGame;
using SessionMaster.BLL.ModUser;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.DAL;
using SessionMaster.DAL.Entities;
using SessionMaster.UnitTests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Xunit;

namespace SessionMaster.UnitTests.Domains.ModUser
{
    public class BoardGameAtlasApiRepositoryTest
    {
        public class GetByIdTest : BoardGameAtlasApiRepositoryTest
        {
            [Fact]
            public async void Valid_ResturnsBoardGameDetails()
            {
                //Arrange
                var response = new RestResponse();
                response.StatusCode = HttpStatusCode.OK;
                response.ResponseStatus = ResponseStatus.Completed;
                response.Content = BoardGameAtlasResponseTestHelper.SINGLE_BOARDGAME_DETAILS;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act
                var result = await sut.GetById(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate());

                //Assert
                Assert.IsType<BoardGameAtlasGameDetails>(result);

                Assert.NotNull(result.Id);
                Assert.NotNull(result.Name);
                Assert.NotNull(result.PublishYear);
                Assert.NotNull(result.MinPlayers);
                Assert.NotNull(result.MaxPlayers);
                Assert.NotNull(result.MinPlaytime);
                Assert.NotNull(result.MaxPlaytime);
                Assert.NotNull(result.Description);
                Assert.NotNull(result.ThumbUrl);
                Assert.NotNull(result.ImageUrl);
            }

            [Fact]
            public void NotFoundStatus_ThrowsException()
            {
                //Arrange
                var response = new RestResponse();
                response.StatusCode = HttpStatusCode.NotFound;
                response.ResponseStatus = ResponseStatus.Error;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<NotFoundException>(() => sut.GetById(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }

            [Fact]
            public void UnsuccessfullRApiRequest_ThrowsInfoException()
            {
                //Arrange
                var response = new RestResponse();
                response.ResponseStatus = ResponseStatus.Error;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<InfoException>(() => sut.GetById(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }

            [Fact]
            public void Exception_ThrowsInfoException()
            {
                var exception = new Exception();

                //Arrange
                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<InfoException>(() => sut.GetById(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }
        }

        public class GetAllTest : BoardGameAtlasApiRepositoryTest
        {
            [Fact]
            public async void Valid_ResturnsBoardGameDetailList()
            {
                //Arrange
                var response = new RestResponse();
                response.StatusCode = HttpStatusCode.OK;
                response.ResponseStatus = ResponseStatus.Completed;
                response.Content = BoardGameAtlasResponseTestHelper.TWO_BOARDGAME_DETAILS;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act
                var result = await sut.GetAll(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate());

                //Assert
                Assert.IsType<List<BoardGameAtlasGameDetails>>(result);
                Assert.Equal(2, result.Count);
            }

            [Fact]
            public void NotFoundStatus_ThrowsException()
            {
                //Arrange
                var response = new RestResponse();
                response.StatusCode = HttpStatusCode.NotFound;
                response.ResponseStatus = ResponseStatus.Error;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<NotFoundException>(() => sut.GetAll(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }

            [Fact]
            public void UnsuccessfullRApiRequest_ThrowsInfoException()
            {
                //Arrange
                var response = new RestResponse();
                response.ResponseStatus = ResponseStatus.Error;

                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<InfoException>(() => sut.GetAll(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }

            [Fact]
            public void Exception_ThrowsInfoException()
            {
                var exception = new Exception();

                //Arrange
                var client = new Mock<IRestClient>();
                client.Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

                var context = new SessionMasterContext(SessionMasterContextTestHelper.ContextOptions());
                var sut = new BoardGameAtlasApiRepository(context, client.Object);

                //Act & Assert
                Assert.ThrowsAsync<InfoException>(() => sut.GetAll(RandomStringTestHelper.Generate(), RandomStringTestHelper.Generate()));
            }
        }
    }
}