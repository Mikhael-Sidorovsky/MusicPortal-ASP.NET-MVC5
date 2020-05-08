using MusicPortal.Models;
using System.Collections.Generic;

namespace MusicPortal.ViewModels
{
    public class PlaylistViewModel
    {
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
    }
}