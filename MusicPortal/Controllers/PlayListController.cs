using MusicPortal.Models;
using MusicPortal.Models.Repository;
using MusicPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MusicPortal.Controllers
{
    public class PlayListController : Controller
    {
        IRepository repository;
        public PlayListController(IRepository _repository)
        {
            repository = _repository;
        }
        #region Index region
        
        public async Task<ActionResult> Index(string status = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                PlaylistViewModel playlistViewModel = await IndexViewParameterInitializer();                
                List<Song> songs = (await repository.GetListAsync<Song>()).ToList<Song>();
                ViewBag.SongList = songs;
                ViewBag.RequestState = status;
                return View(playlistViewModel);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public async Task<PartialViewResult> GetPlayList(string genre = "All...", string author = "All...")
        {
            List<Song> songs = new List<Song>();
            if (genre == "All..." && author == "All...")
            {
                songs = (await repository.GetListAsync<Song>()).ToList<Song>();
            }
            else
            {
                if (genre == "All...")
                {
                    songs = (await repository.GetListAsync<Song>(x => x.Artist.Name == author)).ToList<Song>();
                }
                else if (author == "All...")
                {
                    songs = (await repository.GetListAsync<Song>(x => x.Genre.Name == genre)).ToList<Song>();
                }
                else
                {
                    songs = (await repository.GetListAsync<Song>(x => (x.Artist.Name == author && x.Genre.Name == genre))).ToList<Song>();
                }
            }
            return PartialView("_IndexPartial", songs);
        }
        #endregion

        #region Add song region
        [HttpGet]
        public async Task<ActionResult> AddSong()
        {
            SongViewModel model = await GetSongViewModelFromSong();
            return PartialView("_AddSongPartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSong(SongViewModel model)
        {
            if (model.File != null && !String.IsNullOrEmpty(model.Name))
            {
                if (Path.GetExtension(model.File.FileName).ToLower() != ".mp3")
                    return RedirectToAction("Index", new { status = "bad" });

                if (await repository.GetAsync<Song>(x => x.FilePath == "/Music/" + model.File.FileName) == null)
                {
                    if (await repository.GetAsync<Song>(x => x.Name == model.Name && x.Artist.Name == model.Artist) != null)
                        return RedirectToAction("Index", new { status = "repeat" });

                    model.File.SaveAs(Server.MapPath("~/Music/" + model.File.FileName));
                    Song sng = await GetSongFromSongViewModel(model);
                    await repository.CreateAsync<Song>(sng);
                }
                else
                    return RedirectToAction("Index", new { status = "repeat" });
            }
            else
                return RedirectToAction("Index", new { status = "required" });

            return RedirectToAction("Index", new { status = "success" });
        }
        #endregion

        #region Edit song region
        [HttpGet]
        public async Task<ActionResult> EditSong(int id)
        {
            Song song = await repository.GetAsync<Song>(x => x.Id == id);
            SongViewModel model = await GetSongViewModelFromSong(song);
            return PartialView("_EditSongPartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSong(SongViewModel model)
        {
            if(String.IsNullOrEmpty(model.Name))
                return RedirectToAction("Index", new { status = "required" });
            if(await repository.GetAsync<Song>(x => x.Name == model.Name && x.Artist.Name == model.Artist) != null)
                return RedirectToAction("Index", new { status = "repeat" });
            Song song = await GetSongFromSongViewModel(model);
            await repository.UpdateAsync<Song>(song);
            return RedirectToAction("Index", new { status = "success" });
        }
        #endregion

        #region Delete song region
        [HttpGet]
        public async Task<ActionResult> DeleteSong(int id)
        {
            Song song = await repository.GetAsync<Song>(x => x.Id == id);
            SongViewModel model = await GetSongViewModelFromSong(song);
            return PartialView("_DeleteSongPartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSong(SongViewModel model)
        {
            Song song = await repository.GetAsync<Song>(x => x.Id == model.Id);
            string path = Server.MapPath(song.FilePath);
            
            FileInfo file = new FileInfo(path);
            if(file.Exists)
            {
                file.Delete();
                await repository.DeleteAsync<Song>(song);
            }
            return RedirectToAction("Index", new { status = "success" });
        }
        #endregion

        #region Add genre region
        [HttpGet]
        public ActionResult AddGenre()
        {
            return PartialView("_AddGenrePartial");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddGenre(Genre genre)
        {
            if (String.IsNullOrEmpty(genre.Name))
                return RedirectToAction("Index", new { status = "required" });
            if (await repository.GetAsync<Genre>(x => x.Name == genre.Name) != null)
                return RedirectToAction("Index", new { status = "repeat" });
            await repository.CreateAsync<Genre>(genre);
            return RedirectToAction("Index", new { status = "success" });
        }
        #endregion

        #region Add artist region
        [HttpGet]
        public ActionResult AddArtist()
        {
            return PartialView("_AddArtistPartial");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddArtist(Artist artist)
        {
            if (String.IsNullOrEmpty(artist.Name))
                return RedirectToAction("Index", new { status = "required" });
            if (await repository.GetAsync<Artist>(x => x.Name == artist.Name) != null)
                return RedirectToAction("Index", new { status = "repeat" });
            await repository.CreateAsync<Artist>(artist);
            return RedirectToAction("Index", new { status = "success" });
        }
        #endregion

        #region Service methog region
        void SetUserRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session["UserRole"] == null)
                {
                    User user = repository.GetAsync<User>(x => x.Login == User.Identity.Name).Result;
                    Session["UserRole"] = user.Role.Name;
                }
                if ((string)Session["UserRole"] == "Admin" && Session["NewRequestCount"] == null)
                    Session["NewRequestCount"] = repository.GetListAsync<User>(x => x.IsRegistered == false).Result.ToList<User>().Count();
            }
            else
                ViewBag.UserRole = "";
        }
        async Task<Song> GetSongFromSongViewModel(SongViewModel model)
        {
            Song song = new Song();
            if (model.File == null)
                song = await repository.GetAsync<Song>(x => x.Id == model.Id);
            if (String.IsNullOrEmpty(model.Name))
                song.Name = "No name";
            else
                song.Name = model.Name;
            song.ArtistId = (await repository.GetAsync<Artist>(x => x.Name == model.Artist)).Id;
            song.GenreId = model.Genre != null ? (await repository.GetAsync<Genre>(x => x.Name == model.Genre)).Id : 1;
            if (model.File != null)
                song.FilePath = "/Music/" + Path.GetFileName(model.File.FileName);
            return song;
        }

        async Task<SongViewModel> GetSongViewModelFromSong(Song song = null)
        {
            SongViewModel model = new SongViewModel();
            List<Artist> artistsDB = (await repository.GetListAsync<Artist>()).ToList<Artist>();
            List<Genre> genresDB = (await repository.GetListAsync<Genre>()).ToList<Genre>();
            List<SelectListItem> genres = new List<SelectListItem>();
            List<SelectListItem> artists = new List<SelectListItem>();
            foreach (var genre in genresDB)
                genres.Add(new SelectListItem { Text = genre.Name, Value = genre.Name,
                    Selected = (song != null && song.Genre != null ? genre.Id == song.GenreId : false) });
            foreach (var artist in artistsDB)
                artists.Add(new SelectListItem { Text = artist.Name, Value = artist.Name, 
                    Selected = (song != null && song.Artist != null ?  artist.Id == song.ArtistId : false) });
            model.Name = song != null ? song.Name : "";
            model.Artist = song != null && song.Artist != null ? song.Artist.Name : "No artist";
            model.Genre = song != null && song.Genre != null ? song.Genre.Name : "No genre";
            model.Artists = artists;
            model.Genres = genres;
            return model;
        }

        async Task<PlaylistViewModel> IndexViewParameterInitializer()
        {
            SetUserRole();
            PlaylistViewModel playlistViewModel = new PlaylistViewModel();
            playlistViewModel.Artists = (await repository.GetListAsync<Artist>()).ToList<Artist>();
            playlistViewModel.Genres = (await repository.GetListAsync<Genre>()).ToList<Genre>();
            return playlistViewModel;            
        }
        #endregion
    }
}