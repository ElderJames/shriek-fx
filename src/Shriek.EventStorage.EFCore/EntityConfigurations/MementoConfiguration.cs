using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shriek.EntityFrameworkCore;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.EFCore.EntityConfigurations
{
    public class MementoConfiguration : IEntityTypeConfiguration<Memento, EventStorageSQLContext>
    {
        public void Configure(EntityTypeBuilder<Memento> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(x => x.Timestamp)
                .HasColumnName("CreationDate");
        }
    }
}