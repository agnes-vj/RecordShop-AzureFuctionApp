using InputBindingDb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;


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

            if (albums == null)
            {
                logger.LogInformation($"Albums not found.");
                return new NotFoundObjectResult("No Album Found");
            }

            logger.LogInformation($"Albums fetched successfully!");
            return new OkObjectResult(albums);
        }
    }
}
