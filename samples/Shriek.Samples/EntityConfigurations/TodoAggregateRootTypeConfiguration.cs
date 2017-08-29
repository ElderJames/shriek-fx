using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shriek.EntityFrameworkCore;
using Shriek.Samples.Aggregates;

namespace Shriek.Samples.EntityConfigurations
{
    public class TodoAggregateRootTypeConfiguration : IEntityTypeConfiguration<TodoAggregateRoot, TodoDbContext>
    {
        public void Configure(EntityTypeBuilder<TodoAggregateRoot> builder)
        {
            builder.HasAnnotation("Table", "Todo");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.AggregateId);

            builder.Property(c => c.AggregateId)
                     .HasColumnType("char(36)");
        }
    }
}