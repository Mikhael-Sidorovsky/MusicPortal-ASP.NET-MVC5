using MusicPortal.Models;
using MusicPortal.Models.Repository;
using MusicPortal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MusicPortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IRepository repository;
        public AdminController(IRepository _repository)
        {
            repository = _repository;
        }
        #region Confirm registration region
        [HttpGet]
        public async Task<ActionResult> ConfirmUsersRegisration()
        {
            SetUserRole();
            Session["NewRequestCount"] = (await repository.GetListAsync<User>(x => x.IsRegistered == false)).ToList<User>().Count();
            ViewBag.NewRequestCount = Session["NewRequestCount"].ToString();
            List<User> users = (await repository.GetListAsync<User>(x => x.IsRegistered == false)).ToList<User>();
            return View(users);
        }
        [HttpPost]
        public async Task<PartialViewResult> ConfirmRegisration(int id)
        {
            User user = await repository.GetAsync<User>(x => x.Id == id);
            if (user != null)
            {
                user.IsRegistered = true;
                await repository.UpdateAsync<User>(user);
            }
            List<User> users = (await repository.GetListAsync<User>(x => x.IsRegistered == false)).ToList<User>();
            Session["NewRequestCount"] = users.Count;
            return PartialView("_ConfirmUsersRegisrationPartial", users);
        }
        [HttpPost]
        public async Task<ActionResult> RejectionRegisration(int id)
        {
            User user = await repository.GetAsync<User>(x => x.Id == id);
            if (user != null)
                await repository.DeleteAsync<User>(user);
            List<User> users = (await repository.GetListAsync<User>(x => x.IsRegistered == false)).ToList<User>();
            Session["NewRequestCount"] = users.Count;            
            return View("ConfirmUsersRegisration", users);
        }
        #endregion

        #region User management region
        [HttpGet]
        public async Task<ActionResult> UsersManagement()
        {
            SetUserRole();
            List<User> models = (await repository.GetListAsync<User>(x => x.Login != "admin")).ToList<User>();
            Session["NewRequestCount"] = models.Where(x => x.IsRegistered == false).Count();
            return View(models);
        }

        [HttpGet]
        public async Task<PartialViewResult> EditUser(int id)
        {
            User user = await repository.GetAsync<User>(x => x.Id == id);
            UserViewModel model = await GetUserViewModelFromUser(user);
            return PartialView("_EditUserPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(UserViewModel model)
        {
            User user = await repository.GetAsync<User>(x => x.Id == model.User.Id);
            Role role = await repository.GetAsync<Role>(x => x.Name == model.Role);
            user.RoleId = role.Id;
            user.IsRegistered = model.User.IsRegistered;
            await repository.UpdateAsync<User>(user);
            List<User> models = (await repository.GetListAsync<User>(x => x.Login != "admin")).ToList<User>();
            Session["NewRequestCount"] = models.Where(x => x.IsRegistered == false).Count();
            return View("UsersManagement", models);
        }

        [HttpGet]
        public async Task<PartialViewResult> DeleteUser(int id)
        {
            User user = await repository.GetAsync<User>(x => x.Id == id);
            UserViewModel model = await GetUserViewModelFromUser(user);
            return PartialView("_DeleteUserPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(UserViewModel model)
        {
            User user = await repository.GetAsync<User>(x => x.Id == model.User.Id);
            if (user != null)
                await repository.DeleteAsync<User>(user);
            List<User> models = (await repository.GetListAsync<User>(x => x.Login != "admin")).ToList<User>();
            Session["NewRequestCount"] = models.Where(x => x.IsRegistered == false).Count();
            ViewBag.Sender = "delete";
                return View("UsersManagement", models);
        }
        #endregion

        #region Song's management region
        public async Task<ActionResult> SongsManagement()
        {
            SongsManagementViewModel model = new SongsManagementViewModel();
            model.Artists = (await repository.GetListAsync<Artist>()).ToList<Artist>();
            model.Genres = (await repository.GetListAsync<Genre>()).ToList<Genre>();
            return View(model);
        }

        public async Task<ActionResult> ShowArtists()
        {
            List<Artist> artists = (await repository.GetListAsync<Artist>()).ToList<Artist>();
            return PartialView("_ShowArtistsPartial", artists);
        }
        public async Task<ActionResult> ShowGenres()
        {
            List<Genre> genres = (await repository.GetListAsync<Genre>()).ToList<Genre>();
            return PartialView("_ShowGenresPartial", genres);
        }

        [HttpGet]
        public async Task<ActionResult> EditArtist(int id)
        {
            Artist artist = await repository.GetAsync<Artist>(x => x.Id == id);
            return PartialView("_EditArtistPartial", artist);
        }

        [HttpPost]
        public async Task<ActionResult> EditArtist(Artist artist)
        {
            await repository.UpdateAsync<Artist>(artist);
            return RedirectToAction("SongsManagement");
        }

        [HttpGet]
        public async Task<ActionResult> DeleteArtist(int id)
        {
            Artist artist = await repository.GetAsync<Artist>(x => x.Id == id);
            return PartialView("_DeleteArtistPartial", artist); 
        }

        [HttpPost]
        public async Task<ActionResult> DeleteArtist(Artist artist)
        {
            Artist artistToDelete = await repository.GetAsync<Artist>(x => x.Id == artist.Id);
            await repository.DeleteAsync<Artist>(artistToDelete);
            return RedirectToAction("SongsManagement");
        }

        [HttpGet]
        public async Task<ActionResult> EditGenre(int id)
        {
            Genre genre = await repository.GetAsync<Genre>(x => x.Id == id);
            return PartialView("_EditGenrePartial", genre);
        }

        [HttpPost]
        public async Task<ActionResult> EditGenre(Genre genre)
        {
            await repository.UpdateAsync<Genre>(genre);
            return RedirectToAction("SongsManagement");
        }

        [HttpGet]
        public async Task<ActionResult> DeleteGenre(int id)
        {
            Genre genre = await repository.GetAsync<Genre>(x => x.Id == id);
            return PartialView("_DeleteGenrePartial", genre);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGenre(Genre genre)
        {
            Genre genreToDelete = await repository.GetAsync<Genre>(x => x.Id == genre.Id); 
            await repository.DeleteAsync<Genre>(genreToDelete);
            return RedirectToAction("SongsManagement");
        }
        #endregion

        #region Services methods region
        async Task<UserViewModel> GetUserViewModelFromUser(User user)
        {
            List<Role> roles = (await repository.GetListAsync<Role>()).ToList<Role>();
            UserViewModel model = new UserViewModel();
            model.User = user;
            model.Role = user.Role.Name;
            foreach (var role in roles)
                model.Roles.Add(new SelectListItem { Text = role.Name, Value = role.Name, Selected = (role.Id == model.User.RoleId) });
            return model;
        }
        async void SetUserRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session["UserRole"] == null)
                {
                    User user = await repository.GetAsync<User>(x => x.Login == User.Identity.Name);
                    Session["UserRole"] = user.Role.Name;
                }
                if ((string)Session["UserRole"] == "Admin" && Session["NewRequestCount"] == null)
                    Session["NewRequestCount"] = (await repository.GetListAsync<User>(x => x.IsRegistered == false)).ToList<User>().Count();
            }
            ViewBag.UserRole = Session["UserRole"] ?? "";
        }
        #endregion
    }
}