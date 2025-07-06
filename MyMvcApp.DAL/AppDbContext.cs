using Microsoft.EntityFrameworkCore;
using MyMvcApp.DAL.Models;

namespace MyMvcApp.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SampleEntity> SampleEntities { get; set; }
        public DbSet<User> Users { get; set; } // Usersƒe[ƒuƒ‹‚ğ’Ç‰Á
    }
}