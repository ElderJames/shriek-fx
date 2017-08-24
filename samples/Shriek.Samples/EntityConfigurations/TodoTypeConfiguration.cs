using Shriek.Samples.Model;
using Shriek.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Shriek.Samples.EntityConfigurations
{
    internal class TodoTypeConfiguration : IEntityTypeConfiguration<Todo, TodoDbContext>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.HasAnnotation("Table", "Todo");
            builder.Property(c => c.AggregateId)
                     .HasColumnType("char(36)");
        }
    }
}