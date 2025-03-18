using InputBindingDb.Model;
using InputBindingDb.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InputBindingDb.Services
{
    public interface IArtistsService
    {
        (ExecutionStatus status, List<Artist>? artists) GetAllArtists();
        (ExecutionStatus status, Artist? artist) GetArtistById(int id);
        (ExecutionStatus status, Artist? artist) GetArtistByName(string name);
        (ExecutionStatus status, Artist? artist) AddArtist(Artist artist);
        (ExecutionStatus status, Artist artist) UpdateArtist(Artist artist);
        public (ExecutionStatus status, Artist artist) DeleteArtist(int id);
    }
    public class ArtistsService : IArtistsService
    {
        IArtistsRepository _artistsRepository;
        
        public ArtistsService(IArtistsRepository artistsRepository)
        {
            _artistsRepository = artistsRepository;
        }
        public (ExecutionStatus status, Artist? artist) AddArtist(Artist artist)
        {
            try
            {
                var artistAlreadyExists = _artistsRepository.FindArtistByName(artist.Name);
                if (artistAlreadyExists == null)
                {
                    var newArtist = _artistsRepository.CreateArtist(artist);
                    return (ExecutionStatus.SUCCESS, newArtist);

                }
                return (ExecutionStatus.ALREADY_EXISTS, null);
            }
            catch (Exception ex)
            {
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }

        public (ExecutionStatus status, Artist artist) DeleteArtist(int id)
        {
            throw new NotImplementedException();
        }

        public (ExecutionStatus status, List<Artist>? artists) GetAllArtists()
        {
            try
            {
                List<Artist> artists = _artistsRepository.GetAllArtists();

                if (!artists.Any())
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, artists);
            }
            catch (Exception ex)
            {
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }

        public (ExecutionStatus status, Artist? artist) GetArtistById(int id)
        {
            try
            {
                var artist = _artistsRepository.FindArtistById(id);

                if (artist == null)
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, artist);
            }
            catch (Exception ex)
            {
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }
        public (ExecutionStatus status, Artist? artist) GetArtistByName(string name)
        {
            try
            {
              var artist =  _artistsRepository.FindArtistByName(name);
                if (artist == null)
                {
                    return (ExecutionStatus.NOT_FOUND, null);
                }

                return (ExecutionStatus.SUCCESS, artist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "##############");
                return (ExecutionStatus.INTERNAL_SERVER_ERROR, null);
            }
        }
        public (ExecutionStatus status, Artist artist) UpdateArtist(Artist artist)
        {
            throw new NotImplementedException();
        }
        //private Artist mapToArtist(ArtistDTO artistDTO)
        //{
        //    Artist artist = new Artist()
        //    {
        //        Id = artistDTO.Id,
        //        Name = artistDTO.Name,
        //        About = artistDTO.About
        //    };
        //    return artist;
        //}
        //private ArtistDTO mapToArtistDTO(Artist artist)
        //{
        //    ArtistDTO artistDTO = new ArtistDTO()
        //    {
        //        Id = artist.Id,
        //        Name = artist.Name,
        //        About = artist.About,
        //        AlbumIds = artist.Albums.Select(album => album.Id).ToList()
        //    };
        //    return artistDTO;
        //}
    }

}
