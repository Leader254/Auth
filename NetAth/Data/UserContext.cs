using Microsoft.EntityFrameworkCore;
using NetAth.Model;

namespace NetAth.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options): base(options) { }

        public DbSet<LoginModel>? LoginModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel
            {
                Id = 1,
                UserName = "johndoe",
                Password = "def@123"
            });
        }
    }
}
