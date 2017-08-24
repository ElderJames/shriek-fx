using Shriek.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shriek.EntityFrameworkCore;

namespace Shriek.EventStorage.EFCore.EntityConfigurations
{
    internal class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent, EventStorageSQLContext>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.Property(c => c.Timestamp)
                .HasColumnName("CreationDate");

            builder.Property(c => c.EventType)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");

            builder.Property(c => c.AggregateId)
                .HasColumnType("char(36)");
        }
    }
}