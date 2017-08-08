using Shriek.ConfigCenter.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Shriek.ConfigCenter.Web.Data
{
  public class SpaDbContext : DbContext
  {
    public SpaDbContext(DbContextOptions<SpaDbContext> options)
        : base(options)
    {
      Database.EnsureCreated();
    }

    //List of DB Models - Add your DB models here
    public DbSet<User> User { get; set; }
  }
}