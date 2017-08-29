using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shriek.EntityFrameworkCore;
using Shriek.Samples.Models;

namespace Shriek.Samples.EntityConfigurations
{
    public class TodoTypeConfiguration : IEntityTypeConfiguration<Todo, TodoDbContext>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.HasAnnotation("Table", "Todo");

            builder.HasKey(c => c.AggregateId);

            builder.Property(c => c.AggregateId)
                     .HasColumnType("char(36)");
        }
    }
}