

namespace MusicPortal.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public string FilePath { get; set; }
        public int? GenreId { get; set; }
        public virtual Genre Genre { get; set; }
        public int? ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
}