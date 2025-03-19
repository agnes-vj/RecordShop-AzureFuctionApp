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
using Microsoft.AspNetCore.Http;
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
        public  IActionResult GetAllAlbums(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "albums")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("GetAlbumsFunction");
            logger.LogInformation("Fetching albums from SQL Server Express...");
            var response = _albumsService.GetAllAlbums();
            List<AlbumDTO> albums = response.albumDTOs;
            logger.LogInformation($"{albums.Count} albums fetched!");
            return response.status switch
            {
                ExecutionStatus.SUCCESS => new OkObjectResult(albums),
                ExecutionStatus.NOT_FOUND => new NotFoundObjectResult("No Albums Found"),
                _ => new ObjectResult("Internal Server error")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }
        [Function("GetAlbumById")]
        public IActionResult GetAlbumById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "albums/{id}")] HttpRequestData req,
        FunctionContext context, int id)
        {
            var logger = context.GetLogger("GetAlbumsFunction");
            logger.LogInformation($"Fetching albums from SQL Server Express with Id {id}.....");
            var response = _albumsService.GetAlbumById(id);
            AlbumDTO album = response.albumDTO;
            logger.LogInformation($"{album?.Title ?? "0 album"} fetched!");
            return response.status switch
            {
                ExecutionStatus.SUCCESS => new OkObjectResult(album),
                ExecutionStatus.NOT_FOUND => new NotFoundObjectResult("No Albums Found"),
                _ => new ObjectResult("Internal Server error")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }
    }
}