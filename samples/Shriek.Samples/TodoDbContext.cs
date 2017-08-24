using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;
using Shriek.Samples.Aggregates;

namespace Shriek.Samples
{
    public class TodoDbContext : BaseDbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }
    }
}