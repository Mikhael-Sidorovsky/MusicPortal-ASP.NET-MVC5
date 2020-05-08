using MusicPortal.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MusicPortal.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public string Role { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public UserViewModel()
        {
            Roles = new List<SelectListItem>();
        }
    }
}