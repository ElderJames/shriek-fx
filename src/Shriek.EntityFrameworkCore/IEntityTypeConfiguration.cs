using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.EntityFrameworkCore
{
    public interface IEntityTypeConfiguration<TEntity, TDbContext> : Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<TEntity> where TEntity : class where TDbContext : BaseDbContext
    {
    }
}