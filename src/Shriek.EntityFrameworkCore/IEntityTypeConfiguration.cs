using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Shriek.EntityFrameworkCore
{
    public interface IEntityTypeConfiguration<TEntity, TDbContext> : IEntityTypeConfiguration<TEntity> where TEntity : class where TDbContext : BaseDbContext
    {
    }
}