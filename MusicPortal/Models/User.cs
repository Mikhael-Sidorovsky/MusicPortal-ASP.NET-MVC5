

namespace MusicPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public bool IsRegistered { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}