
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
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var contextMock = new Mock<FunctionContext>();
            contextMock.SetupProperty(c => c.InstanceServices, serviceProvider);

            var request = new FakeHttpRequestData(contextMock.Object);

            var expectedAlbums = new List<AlbumDTO>
                {
                    new AlbumDTO { Id = 1, Title = "Album 1", ArtistName ="Artist 1", MusicGenre = "Genre 1", ReleaseYear = 2001, Stock = 5},
                    new AlbumDTO { Id = 2, Title = "Album 2", ArtistName ="Artist 1", MusicGenre = "Genre 2", ReleaseYear = 2021, Stock = 15}
                };

            var response = (status : ExecutionStatus.SUCCESS, albumDTOs : expectedAlbums);

            _mockAlbumsService.Setup(service => service.GetAllAlbums()).Returns(response);

            //Act
            var result = _function.GetAllAlbums(request, contextMock.Object) as OkObjectResult;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedAlbums = Assert.IsType<List<AlbumDTO>>(result.Value);
            Assert.Equal(expectedAlbums.Count, returnedAlbums.Count);  

        }
        
    }
    [ExcludeFromCodeCoverage]
    public class FakeHttpResponseData : HttpResponseData
    {
        public FakeHttpResponseData(FunctionContext functionContext) : base(functionContext) { }

        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies { get; }
    }
    [ExcludeFromCodeCoverage]
    public class FakeHttpRequestData : HttpRequestData
    {
        public FakeHttpRequestData(FunctionContext functionContext) : base(functionContext)
        {

        }
        public override Stream Body { get; } = new MemoryStream();
        public override HttpHeadersCollection Headers { get; } = new HttpHeadersCollection();
        public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
        public override Uri Url => new Uri("https://localhost/api/albums");
        public override IEnumerable<ClaimsIdentity> Identities { get; }
        public override string Method { get; }
        public override HttpResponseData CreateResponse()
        {
            return new FakeHttpResponseData(FunctionContext);
        }
    }
}