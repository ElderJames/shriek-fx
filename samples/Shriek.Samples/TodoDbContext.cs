using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;

namespace Shriek.Samples
{
    public class TodoDbContext : BaseDbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }
    }
}