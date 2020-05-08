using MusicPortal.Models;
using MusicPortal.Models.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MusicPortal.Controllers
{
    public class HomeController : Controller
    {
        IRepository repository;
        public HomeController(IRepository _repository)
        {
            repository = _repository;
        }

        public async Task<ActionResult> Index()
        {
            await Task.Run(() => SetUserRole());
            return View();
        }
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
    }
}