using InputBindingDb.Model;
using InputBindingDb.Repository;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
namespace InputBindingDb.Services
{
    public interface IAlbumsService
    {
        (ExecutionStatus status, List<AlbumDTO>? albumDTOs) GetAllAlbums();
        (ExecutionStatus status, AlbumDTO? albumDTO) GetAlbumById(int id);
        (ExecutionStatus status, AlbumDTO? albumDTO) AddAlbum(AlbumDTO albumDTO);
        (ExecutionStatus status, AlbumDTO albumDTO) UpdateAlbum(AlbumDTO albumDTO);
        (ExecutionStatus status, AlbumDTO albumDTO) ReplaceAlbum(int id,AlbumDTO albumDTO);
        ExecutionStatus  DeleteAlbum(int id);
        (ExecutionStatus status, List<AlbumDTO>? albumDTOs) GetFilteredAlbums(AlbumsFilter filter);
    }
    public class AlbumsService : IAlbumsService
    {
        IAlbumsRepository _albumsRepository;
        IArtistsService _artistsService;
        ILogger<IAlbumsService> _logger;
        public AlbumsService(ILogger<IAlbumsService> logger, IAlbumsRepository albumsRepository, IArtistsService artistsService) 
        {
           _albumsRepository = albumsRepository;
            _artistsService = artistsService;
            _logger = logger;   
        }
        public (ExecutionStatus status, List<AlbumDTO>? albumDTOs) GetAllAlbums()
        {
            try
            {
                List<Album> albums = _albumsRepository.GetAllAlbums();

                if (!albums.Any())
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, albums.Select(a => mapToAlbumDTO(a))
                                                       .ToList());
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }                                       
        }
        public (ExecutionStatus status, AlbumDTO? albumDTO) GetAlbumById(int id)
        {
            try
            {
                var album =_albumsRepository.FindAlbumById(id);

                if (album == null)
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, mapToAlbumDTO(album));                                                       
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }
       public (ExecutionStatus status, List<AlbumDTO>? albumDTOs) GetFilteredAlbums(AlbumsFilter filter)
        {
            try
            {
                List<Album> albums = _albumsRepository.GetFilteredAlbums(filter);

                if (!albums.Any())
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, albums.Select(a => mapToAlbumDTO(a))
                                                       .ToList());
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }

        }    
    public (ExecutionStatus status, AlbumDTO? albumDTO) AddAlbum(AlbumDTO albumDTO)
        {
            try
            {
                if (_artistsService.GetArtistByName(albumDTO.ArtistName).artist == null)
                    return (ExecutionStatus.ARTIST_NOT_FOUND, null);

                bool albumAlreadyExists = (_albumsRepository.FindAlbumByTitleAndArtist(albumDTO.Title, albumDTO.ArtistName) == null) ? false : true ;

                if (albumAlreadyExists)
                    return (ExecutionStatus.ALREADY_EXISTS, null);   
                
                var newAlbum = _albumsRepository.CreateAlbum(mapToAlbum(albumDTO));
                return (ExecutionStatus.SUCCESS, mapToAlbumDTO(newAlbum));
                
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }

        public (ExecutionStatus status, AlbumDTO? albumDTO) ReplaceAlbum(int id, AlbumDTO albumDTO)
        {
            try
            { 
                Artist newArtist = null;
                var albumToUpdate = _albumsRepository.FindAlbumById(id);
                if (albumToUpdate == null)
                     return (ExecutionStatus.NOT_FOUND, null);
                var newArtistId = _artistsService.GetArtistByName(albumDTO.ArtistName).artist? .Id ?? -1;

                if (newArtistId == -1)
                    return (ExecutionStatus.ARTIST_NOT_FOUND, null);
                albumToUpdate.Title = albumDTO.Title;
                albumToUpdate.ArtistId = newArtistId;
                if (Enum.TryParse<Genre>(albumDTO.MusicGenre, ignoreCase: true, out Genre parsedGenre))
                    albumToUpdate.MusicGenre = parsedGenre;
                else           
                    albumToUpdate.MusicGenre = Genre.UNKNOWN;
                

                albumToUpdate.ReleaseYear = albumDTO.ReleaseYear;
                albumToUpdate.Stock = albumDTO.Stock;

                var newAlbum = _albumsRepository.ReplaceAlbum(albumToUpdate);
                return (ExecutionStatus.SUCCESS, mapToAlbumDTO(newAlbum));
                
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }
        public ExecutionStatus DeleteAlbum(int id)
        {
            try
            {
                var albumToDelete = _albumsRepository.FindAlbumById(id);
                if (albumToDelete != null)
                {
                    _albumsRepository.DeleteAlbum(albumToDelete);
                    return ExecutionStatus.SUCCESS;
                }
                return ExecutionStatus.NOT_FOUND;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"******{ex.Message}*******");
                return ExecutionStatus.INTERNAL_SERVER_ERROR;
            }
        }
        public (ExecutionStatus status, AlbumDTO albumDTO) UpdateAlbum(AlbumDTO albumDTO)
        {
            throw new NotImplementedException();
        }
        private Album mapToAlbum(AlbumDTO albumDTO)
        {
            Album album = new Album()
            {
                Id = albumDTO.Id,
                Title = albumDTO.Title,
                AlbumArtist = _artistsService.GetArtistByName(albumDTO.ArtistName).artist,
                MusicGenre = (Enum.TryParse<Genre>(albumDTO.MusicGenre, ignoreCase: true, out Genre parsedGenre))?parsedGenre: Genre.UNKNOWN,
                ReleaseYear = albumDTO.ReleaseYear,
                Stock = albumDTO.Stock
            };
            return album;
        }
        private AlbumDTO mapToAlbumDTO(Album album)
        {
            AlbumDTO albumDTO = new AlbumDTO()
            {
                Id = album.Id,
                Title = album.Title,
                ArtistName = album.AlbumArtist?.Name ?? "Unknown",
                MusicGenre = album.MusicGenre.ToString(),
                ReleaseYear = album.ReleaseYear,
                Stock = album.Stock
            };
            return albumDTO;
        }
    }

}
