using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shriek.EntityFrameworkCore
{
    public static class EntityTypeConfigurationExtensions
    {
        public static void Map<TEntity>(this IEntityTypeConfiguration<TEntity> configuration, EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            configuration.Configure(builder);
        }
    }
}