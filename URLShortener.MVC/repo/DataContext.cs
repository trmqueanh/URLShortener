using Microsoft.EntityFrameworkCore;
using URLShortener.MVC.Models;

namespace URLShortener.MVC.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<HomeModel> HomeModels { get; set; }
    }
}
