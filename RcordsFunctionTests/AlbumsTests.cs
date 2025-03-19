
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging.Abstractions;
using InputBindingDb;
using Moq;
using InputBindingDb.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using InputBindingDb.Model;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RecordsFunctionTests
{
    public class AlbumsTests
    {
        private readonly Mock<IAlbumsService> _mockAlbumsService;
        private readonly GetAlbumsFunction _function;
        private readonly DefaultHttpContext _httpContext;

        public AlbumsTests()
        {

            _mockAlbumsService = new Mock<IAlbumsService>();

            _function = new GetAlbumsFunction(_mockAlbumsService.Object);

            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void TestGetAlbumsSuccess()
        {
            //Arrange

            var requestMock = new Mock<HttpRequestData>();
            var expectedAlbums = new List<AlbumDTO>
                {
                    new AlbumDTO { Id = 1, Title = "Album 1", ArtistName ="Artist 1", MusicGenre = "Genre 1", ReleaseYear = 2001, Stock = 5},
                    new AlbumDTO { Id = 2, Title = "Album 2", ArtistName ="Artist 1", MusicGenre = "Genre 2", ReleaseYear = 2021, Stock = 15}
                };

            var response = (status : ExecutionStatus.SUCCESS, albumDTOs : expectedAlbums);

            _mockAlbumsService.Setup(service => service.GetAllAlbums()).Returns(response);

            //Act
            var result = _function.GetAllAlbums(requestMock.Object, new Mock<FunctionContext>().Object) as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedAlbums = Assert.IsType<List<AlbumDTO>>(result.Value);
            Assert.Equal(expectedAlbums.Count, returnedAlbums.Count);

        }

        
    }
}