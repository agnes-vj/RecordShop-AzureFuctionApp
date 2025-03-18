using InputBindingDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InputBindingDb.Repository
{
    public interface IArtistsRepository
    {
        List<Artist>? GetAllArtists();
        Artist? FindArtistById(int id);
        Artist? FindArtistByName(string name);
        Artist CreateArtist(Artist newArtist);
        Artist ReplaceArtist(Artist artist);
        Artist UpdateArtist(Artist artist);
        void DeleteArtist(Artist artist);


    }
    public class ArtistsRepository : IArtistsRepository
    {
        RecordShopDbContext _dbContext;
        public ArtistsRepository(RecordShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Artist CreateArtist(Artist newArtist)
        {
            _dbContext.Artists.Add(newArtist);
            _dbContext.SaveChanges();
            return newArtist;
        }

        public void DeleteArtist(Artist artist)
        {
            _dbContext.Artists.Remove(artist);
            _dbContext.SaveChanges();
        }

        public List<Artist>? GetAllArtists()
        {
            return _dbContext.Artists.ToList();
        }

        public Artist? FindArtistByName(string name)
        {
            string normalisedName = normaliseString(name);
            return _dbContext.Artists.FirstOrDefault(artist => artist.Name.ToLower().Replace(" ", "").Trim() == normalisedName);
        }
        private string normaliseString(string input)
        {
            return input.Trim().ToLowerInvariant().Replace(" ", "");
        }
        public Artist?FindArtistById(int id)
        {
            return _dbContext.Artists.FirstOrDefault(artist => artist.Id == id);
        }

        public Artist ReplaceArtist(Artist artist)
        {
            _dbContext.Artists.Update(artist);
            _dbContext.SaveChanges();
            return artist;
        }

        public Artist UpdateArtist(Artist artist)
        {
            throw new NotImplementedException();
        }
    }

}
