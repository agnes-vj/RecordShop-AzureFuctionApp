using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InputBindingDb.Model
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int ArtistId { get; set; }
        [ForeignKey(nameof(ArtistId))]
        public Artist AlbumArtist { get; set;}
        [Required]
        public Genre MusicGenre {get; set;}
        [ReleaseYearValidation]
        public int ReleaseYear {get; set;}
        public int Stock {get; set;}
        public Album() : base()
        {

        }
        public Album(int id, string title, int artistId, Genre genre, int releaseYear, int stock)
        {
            Id = id;
            Title = title;
            ArtistId = artistId;
            MusicGenre = genre;
            ReleaseYear = releaseYear;
            Stock = stock;
        }
    }
}
