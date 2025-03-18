using Microsoft.EntityFrameworkCore;
using InputBindingDb.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InputBindingDb.Repository
{
    public interface IAlbumsRepository
    {
        public List<Album> GetAllAlbums();
        public Album? FindAlbumById(int id);
        public Album? FindAlbumByTitleAndArtist(string title, string artistName);
        public Album CreateAlbum(Album album);
        public Album UpdateAlbum(Album album);
        public Album ReplaceAlbum(Album newAlbum);
        public void DeleteAlbum(Album album);
        public List<Album> GetFilteredAlbums(AlbumsFilter filter);
    }
    public class AlbumsRepository : IAlbumsRepository
    {
        RecordShopDbContext _dbContext;
        public AlbumsRepository(RecordShopDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public List<Album> GetAllAlbums()
        {
            return _dbContext.Albums
                             .Include(album => album.AlbumArtist)
                             .ToList();     
        }
        public List<Album> GetFilteredAlbums(AlbumsFilter filter)
        {
            var query = _dbContext.Albums
                                         .Include (album => album.AlbumArtist)
                                         .AsQueryable();
            if (! string.IsNullOrEmpty(filter.Title))
                query = query.Where(albums => albums.Title == filter.Title);
            if (!string.IsNullOrEmpty(filter.ArtistName))
            {                
               string normalisedName = normaliseString(filter.ArtistName);                   
               query = query.Where(albums => albums.AlbumArtist.Name.ToLower().Replace(" ", "").Trim() == normalisedName);
            }
            
            if (filter.MusicGenre.HasValue)
                query = query.Where(albums => (int)albums.MusicGenre == filter.MusicGenre);
            if (filter.ReleaseYear.HasValue)
                query = query.Where(albums => albums.ReleaseYear == filter.ReleaseYear);
            return query.ToList();

        }
        private string normaliseString(string input)
        {
            return input.Trim().ToLowerInvariant().Replace(" ", "");
        }
        public Album? FindAlbumById(int id)
        {
            return _dbContext.Albums.Include(album => album.AlbumArtist).FirstOrDefault(album => album.Id == id);
        }

        public Album? FindAlbumByTitleAndArtist(string title, string artistName)
        {
            return _dbContext.Albums.FirstOrDefault(album => (album.Title == title && album.AlbumArtist.Name == artistName));
        }
        public Album CreateAlbum(Album album)
        {           
            _dbContext.Albums.Add(album);
            _dbContext.SaveChanges();
            return album;
        }
        public Album ReplaceAlbum(Album albumWithNewValues)
        {
            _dbContext.Update(albumWithNewValues);
            _dbContext.SaveChanges();

            return albumWithNewValues;
        }
        public void DeleteAlbum(Album album)
        {
            _dbContext.Albums.Remove(album);
            _dbContext.SaveChanges();
        }

        public Album UpdateAlbum(Album album)
        {
            throw new NotImplementedException();
        }
    }

}
