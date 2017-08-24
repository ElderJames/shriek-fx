using Shriek.Samples.Aggregates;
using Shriek.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shriek.Samples.EntityConfigurations
{
    public class TodoAggregateRootTypeConfiguration : IEntityTypeConfiguration<TodoAggregateRoot, TodoDbContext>
    {
        public void Configure(EntityTypeBuilder<TodoAggregateRoot> builder)
        {
            builder.HasAnnotation("Table", "Todo");
            builder.Property(c => c.AggregateId)
                     .HasColumnType("char(36)");
        }
    }
}