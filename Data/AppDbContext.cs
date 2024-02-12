using Microsoft.EntityFrameworkCore;


namespace Laya.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
         
        public DbSet<Registration> Registration { get; set; }

  
    
    }
}
