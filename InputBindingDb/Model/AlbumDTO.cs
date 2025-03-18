using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InputBindingDb.Model
{
    public class AlbumDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Album Title Required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Artist Name Required.")]
        public string ArtistName { get; set; }
        
       
        public  string MusicGenre { get; set; }
        [ReleaseYearValidation(ErrorMessage = "Release year should not be greater than current year")]
        public int ReleaseYear { get; set; }
        [Required(ErrorMessage = " Stock Value Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Stock Value.")]
        public int Stock { get; set; }
    }
}