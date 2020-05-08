using MusicPortal.Infrastructure;
using MusicPortal.Models;
using MusicPortal.Models.Repository;
using MusicPortal.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace MusicPortal.Controllers
{
    public class AccountController : Controller
    {
        IRepository repository;
        public AccountController(IRepository _repository)
        {
            repository = _repository;
        }

        #region Registration region

        [HttpGet]
        public ActionResult Registration()
        {
            if (!User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("PlayList", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registration(RegistrationViewModel model)
        {
            string pattern = @"^[a-zA-Zа-яА-Я0-9_-]+$";
            string loginPattern = "[\\w| !\"§$% \\&/ () =\\-?\\@\\*\\.]*";
            string emailPattern = "[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
            string passwordPattern = "[\\w| !\"§$% \\&/ () =\\-?\\@\\*]*";
            if (!User.Identity.IsAuthenticated)
            {
                if(String.IsNullOrEmpty(model.FirstName))
                    ModelState.AddModelError("FirstName", "Required field First name.");
                if (!String.IsNullOrEmpty(model.FirstName) && !Regex.IsMatch(model.FirstName, pattern))
                    ModelState.AddModelError("FirstName", "The field contains invalid characters.");
                if (!String.IsNullOrEmpty(model.FirstName) && (model.FirstName.Length < 2 || model.FirstName.Length > 20))
                    ModelState.AddModelError("FirstName", "First name length must be between 2 and 20 characters");
                if (String.IsNullOrEmpty(model.LastName))
                    ModelState.AddModelError("LastName", "Required field Last name.");
                if (!String.IsNullOrEmpty(model.LastName) && !Regex.IsMatch(model.LastName, pattern))
                    ModelState.AddModelError("LastName", "The field contains invalid characters.");
                if (!String.IsNullOrEmpty(model.LastName) && (model.LastName.Length < 2 || model.LastName.Length > 20))
                    ModelState.AddModelError("LastName", "Last name length must be between 2 and 20 characters");
                if (String.IsNullOrEmpty(model.Login))
                    ModelState.AddModelError("Login", "Required field Login.");
                if (!String.IsNullOrEmpty(model.Login) && !Regex.IsMatch(model.Login, loginPattern))
                    ModelState.AddModelError("Login", "The field contains invalid characters.");
                if (!String.IsNullOrEmpty(model.Login) && (model.Login.Length < 2 || model.Login.Length > 20))
                    ModelState.AddModelError("Login", "Login length must be between 2 and 20 characters");
                if (!repository.IsUniqueUserValue(model.Login))
                    ModelState.AddModelError("Login", "A user with this login already exists.");
                if (String.IsNullOrEmpty(model.Email))
                    ModelState.AddModelError("Email", "Required field Email.");
                if (!String.IsNullOrEmpty(model.Email) && !Regex.IsMatch(model.Email, emailPattern))
                    ModelState.AddModelError("Email", "Invalid email input.");
                if (!repository.IsUniqueUserValue(model.Email))
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                if (String.IsNullOrEmpty(model.Password))
                    ModelState.AddModelError("Password", "Required field Password.");
                if (!String.IsNullOrEmpty(model.Password) && !Regex.IsMatch(model.Password, passwordPattern))
                    ModelState.AddModelError("Password", "The field contains invalid characters.");
                if (!String.IsNullOrEmpty(model.Password) && (model.Password.Length < 6 || model.Password.Length > 20))
                    ModelState.AddModelError("Password", "Password length must be between 6 and 20 characters.");
                if (String.IsNullOrEmpty(model.ConfirmPassword))
                    ModelState.AddModelError("ConfirmPassword", "Required field Confirm password.");
                if(model.Password != model.ConfirmPassword)
                    ModelState.AddModelError("ConfirmPassword", "Password mismatch.");
                if (ModelState.IsValid)
                {
                    User user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Login = model.Login,
                        Password = EncryptionUtility.EncryptData(model.Password),
                        IsRegistered = false,
                        RoleId = 2
                    };
                    if (await repository.CreateAsync<User>(user))
                        return RedirectToAction("Login");
                    else
                    {
                        ModelState.AddModelError("", "Error for registration");
                        return View(model);
                    }
                }
                return View(model);
            }
            else
                return RedirectToAction("PlayList", "Home");
        }
        #endregion

        #region Login region

        public ActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (String.IsNullOrEmpty(model.Login) || String.IsNullOrEmpty(model.Password))
                    ModelState.AddModelError("", "All fields must be filled.");
                if (ModelState.IsValid)
                {
                    User user = await repository.GetAsync<User>(x => x.Login == model.Login || x.Email == model.Login);
                    if (user != null && EncryptionUtility.DecryptDate(user.Password) == model.Password)
                        if (user.IsRegistered)
                        {
                            Session["UserRole"] = user.Role.Name;
                            FormsAuthentication.SetAuthCookie(model.Login, false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                            ModelState.AddModelError("", "Sorry, your registration has not been verified by the administrator yet.");
                    else
                        ModelState.AddModelError("", "Invalid login or password.");
                }
                return View(model); 
            }
            else
                return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Logout region
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // очищаем куки аутентификации
            HttpCookie formsСookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            formsСookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(formsСookie);

            // очищаем куки сессии
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie sessionCookie = new HttpCookie(sessionStateSection.CookieName, "");
            sessionCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(sessionCookie);
            return RedirectToAction("Login");
        }
        #endregion
    }
}