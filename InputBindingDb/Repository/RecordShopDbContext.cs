using Microsoft.EntityFrameworkCore;
using InputBindingDb.Model;

namespace InputBindingDb.Repository
{
    public class RecordShopDbContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public RecordShopDbContext(DbContextOptions<RecordShopDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Seed data for Artists
            modelBuilder.Entity<Artist>().HasData(
                new Artist(1, "The Beatles", "Legendary British rock band."),
                new Artist(2, "Taylor Swift", "Award-winning pop and country music artist."),
                new Artist(3, "Michael Jackson", "Known as the King of Pop, Michael Jackson was a global icon and music legend.")
                );

            modelBuilder.Entity<Album>()
                        .Property(a => a.MusicGenre)
                        .HasConversion<string>();

            // Seed data for Albums table
            modelBuilder.Entity<Album>().HasData(
                new Album
                {
                    Id = 1,
                    Title = "Abbey Road",
                    ArtistId = 1,
                    MusicGenre = Genre.ROCK,
                    ReleaseYear = 1969,
                    Stock = 10
                },
                new Album
                {
                    Id = 2,
                    Title = "Let It Be",
                    MusicGenre = Genre.ROCK,
                    ReleaseYear = 1970,
                    Stock = 40,
                    ArtistId = 1
                },
                new Album
                {
                    Id = 3,
                    Title = "1989",
                    ArtistId = 2,
                    MusicGenre = Genre.POP,
                    ReleaseYear = 2014,
                    Stock = 15
                },
                new Album
                {
                    Id = 4,
                    Title = "Thriller",
                    ArtistId = 3,
                    MusicGenre = Genre.POP,
                    ReleaseYear = 1982,
                    Stock = 10
                },
                new Album
                {
                    Id = 5,
                    Title = "Fearless",
                    ArtistId = 2,
                    MusicGenre = Genre.COUNTRY,
                    ReleaseYear = 2008,
                    Stock = 55
                },
                new Album
                {
                    Id = 6,
                    Title = "Folklore",
                    ArtistId = 2,
                    MusicGenre = Genre.CLASSICAL,
                    ReleaseYear = 2020,
                    Stock = 75
                }
            );
        }
    }
}
