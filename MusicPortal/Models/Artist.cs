using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required field.")]
        [RegularExpression("^\\S[a-zA-Zа-яА-Я0-9_ -]+$", ErrorMessage = "The name contains invalid characters.")]
        [StringLength(maximumLength: 50, MinimumLength = 1,
            ErrorMessage = "Name's length must be between 1 and 50 characters")]
        public string Name { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
        public Artist()
        {
            Songs = new List<Song>();
        }
    }
}