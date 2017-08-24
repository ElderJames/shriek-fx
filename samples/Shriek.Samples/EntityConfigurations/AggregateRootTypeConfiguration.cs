using Shriek.Samples.Aggregates;
using Shriek.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shriek.Samples.EntityConfigurations
{
    public class AggregateRootTypeConfiguration : IEntityTypeConfiguration<TodoAggregateRoot, TodoDbContext>
    {
        public void Configure(EntityTypeBuilder<TodoAggregateRoot> builder)
        {
            builder.HasAnnotation("Table", "Todo");
        }
    }
}