using System.Data.Entity;

namespace MusicPortal.Models
{
    public class MusicPortalContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Artist> Artists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>().HasMany(s => s.Songs).WithOptional(s => s.Artist);
            modelBuilder.Entity<Genre>().HasMany(s => s.Songs).WithOptional(s => s.Genre);
            base.OnModelCreating(modelBuilder);
        }
    }
}