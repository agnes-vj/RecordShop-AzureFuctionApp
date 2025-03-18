using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace InputBindingDb.Model
{
    public class Artist
    {
        public Artist(int id, string name, string about)
        {
            Id = id;
            Name = name;
            About = about;    
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;

        public ICollection<Album> Albums { get; set; } = new List<Album>();

    }
}