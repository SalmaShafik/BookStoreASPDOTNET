using Microsoft.EntityFrameworkCore;
using WebApplication1Psight.Models;

namespace WebApplication1Psight.Data
{
    public class AppDBcontext : DbContext
    {
        public AppDBcontext(DbContextOptions<AppDBcontext> options) : base(options)
        {
                

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

    }
    
}
