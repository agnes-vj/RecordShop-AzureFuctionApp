using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace InputBindingDb.Model
{
    public class ArtistDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;

        public List<int> AlbumIds { get; set; } = new List<int>();
    }
}
