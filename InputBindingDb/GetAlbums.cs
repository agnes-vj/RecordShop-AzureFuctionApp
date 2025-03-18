using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using InputBindingDb.Model;
using Microsoft.AspNetCore.Mvc;
using InputBindingDb.Services;
namespace InputBindingDb
{
    public class GetAlbumsFunction
    {
        private readonly IAlbumsService _albumsService;

        public GetAlbumsFunction(IAlbumsService albumsService)
        {
            _albumsService = albumsService;
        }

        [Function("GetAlbums")]
        public  IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "albums")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("GetAlbumsFunction");
            logger.LogInformation("Fetching albums from SQL Server Express...");
            var response = _albumsService.GetAllAlbums();
            List<AlbumDTO> albums = response.albumDTOs;
            logger.LogInformation($"{albums.Count} albums fetched!");
            return new OkObjectResult(albums);
        }

    }
}