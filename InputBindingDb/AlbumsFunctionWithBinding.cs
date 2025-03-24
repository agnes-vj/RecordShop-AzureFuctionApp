using InputBindingDb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Extensions.Storage;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InputBindingDb
{
    public static class AlbumsFunctionWithBinding
    {
        [Function("GetAlbumsUsingBindings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "allalbums")] HttpRequestData req,
            [SqlInput("SELECT * FROM Albums", "SqlConnectionString")] List<AlbumDTO> albums,
            FunctionContext context)
        {
            var logger = context.GetLogger("GetAlbumFunctionWithInputBinding");

            if (albums == null || albums.Count == 0)
            {
                logger.LogInformation($"Albums not found.");
                return new NotFoundObjectResult("No Album Found");
            }

            logger.LogInformation($"Albums fetched successfully!");
            return new OkObjectResult(albums);
        }

        [Function("GetAlbumByIdUsingBindings")]
        public static IActionResult GetAlbumByIdUsingInputBinding(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "binding/albums/{id}")] HttpRequestData req,
            [SqlInput("SELECT * FROM Albums WHERE Id = @Id",
                      "SqlConnectionString",
                      parameters: "@Id={id}")] AlbumDTO album,
            FunctionContext context, int id)
        {
            var logger = context.GetLogger("GetAlbumByIdFunctionUsingBindings");

            if (album == null)
            {
                logger.LogInformation($"Album with Id {id} not found.");
                return new NotFoundObjectResult("No Album Found");
            }

            logger.LogInformation($"Album {album.Title} fetched successfully!");
            return new OkObjectResult(album);
        }

        [Function("AddAlbum")]
        [SqlOutput("Albums", "SqlConnectionString")] 
        [QueueOutput("album-logs", Connection = "AzureWebJobsStorage")]
        public static (AlbumDTO, string) AddAlbum(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = "albums")] HttpRequestData req,

                [SqlInput("SELECT COUNT(*) FROM Artists WHERE Name = @ArtistName",
                          "SqlConnectionString",
                          parameters: "@ArtistName={artistName}")] int artistExists            
               )
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            AlbumDTO newAlbum = JsonSerializer.Deserialize<AlbumDTO>(requestBody);
            

            if (artistExists == 0)
            {
                return (null, null);  
            }

            string queueMessage = $"New album '{newAlbum.Title}' by '{newAlbum.ArtistName}' added on {DateTime.UtcNow}.";

            return (newAlbum, queueMessage);  
        }

    }
}

