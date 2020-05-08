using MusicPortal.Infrastructure;
using System.Data.Entity;

namespace MusicPortal.Models
{
    public class DbInitializer : CreateDatabaseIfNotExists<MusicPortalContext>
    {
        protected override void Seed(MusicPortalContext db)
        {            
            Role role = new Role { Name = "Admin" };
            Role role1 = new Role { Name = "User" };
            User user = new User { FirstName = "Admin", LastName = "Admin", Email = "admin@admin.mp", 
                                   Login = "admin", Role = role, Password = EncryptionUtility.EncryptData("123456"),
                                   IsRegistered = true};
            db.Roles.Add(role);
            db.Roles.Add(role1);
            db.Users.Add(user);
            Genre[] genres = new Genre[] { new Genre { Name = "None" }, new Genre { Name = "Rock" } };
            db.Genres.AddRange(genres);
            Artist[] artists = new Artist[] { 
                                             new Artist { Name = "Queen"},
                                             new Artist { Name = "Sia" },
                                             new Artist { Name = "Linkin Park" },
                                             new Artist { Name = "Evanescence" },
                                             new Artist { Name = "Пикник"},
                                             new Artist { Name = "Король и Шут"}};
            db.Artists.AddRange(artists);
            db.SaveChanges();
            base.Seed(db);
        }
    }
}