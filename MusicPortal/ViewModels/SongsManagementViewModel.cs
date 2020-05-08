using MusicPortal.Models;
using System.Collections.Generic;

namespace MusicPortal.ViewModels
{
    public class SongsManagementViewModel
    {
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public SongsManagementViewModel()
        {
            Artists = new List<Artist>();
            Genres = new List<Genre>();
        }
    }
}