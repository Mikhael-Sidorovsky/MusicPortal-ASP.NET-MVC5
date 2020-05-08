using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MusicPortal.ViewModels
{
    public class SongViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public HttpPostedFileBase File { get; set; }
        [Required]
        public string Artist { get; set; }
        [Required]
        public string Genre { get; set; }
        public List<SelectListItem> Artists { get; set; }
        public List<SelectListItem> Genres { get; set; }
        public SongViewModel()
        {
            Artists = new List<SelectListItem>();
            Genres = new List<SelectListItem>();
        }
    }
}