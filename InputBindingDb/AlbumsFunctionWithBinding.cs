using InputBindingDb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Extensions.Storage;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

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
        public static  async Task<OutputType> AddAlbum(
                        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addalbums")] HttpRequestData req,
                        [FromQuery] string artistName,
                        [SqlInput("SELECT COUNT(*) AS ArtistCount FROM Artists WHERE Name = @ArtistName",
                                  "SqlConnectionString",
                                  parameters: "@ArtistName={artistName}")] string artistExistsStr

                    )
        {

            JsonNode artistExistsNode = JsonNode.Parse(artistExistsStr);

            // Convert the JsonNode to JsonArray (assuming it's an array)
            JsonArray artistExistsArray = artistExistsNode.AsArray();

            // Access the first element of the array (which should be a JsonObject)
            JsonObject artistExistsObject = artistExistsArray[0].AsObject();

            // Extract the 'ArtistCount' property from the JsonObject
            int artistExists = artistExistsObject["ArtistCount"].GetValue<int>();
            Console.WriteLine($"!!!!!!!!!!!!!!!!!{artistName} {artistExistsStr}  {artistExists}");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(); 
            Album newAlbum = JsonSerializer.Deserialize<Album>(requestBody);

            if (artistExists == 0)
            {
                return new OutputType()
                {
                    addedAlbum = newAlbum,

                    HttpResponse = req.CreateResponse(System.Net.HttpStatusCode.NotModified)
                };
            }
            return new OutputType()
            {
                addedAlbum = newAlbum,

                HttpResponse = req.CreateResponse(System.Net.HttpStatusCode.Created)
            };
        }

    }
    public  class OutputType
    {
        [SqlOutput("Albums", "SqlConnectionString")]
        public  Album addedAlbum { get; set; }
        public  HttpResponseData HttpResponse { get; set; }

    }
}

