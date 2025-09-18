using DeliveryApp.Core.Domain.Model.NewFolder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Configurations.CourierAggregate
{
    public class StoragePlaceEntityConfiguration : IEntityTypeConfiguration<StoragePlace>
    {
        public void Configure(EntityTypeBuilder<StoragePlace> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("storage_places");

            entityTypeBuilder.HasKey(x => x.Id);

            entityTypeBuilder
                .Property(e => e.Id)
                .HasColumnType("uuid")
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            entityTypeBuilder
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired();

            entityTypeBuilder
                .Property(e => e.TotalVolume)
                .HasColumnName("total_volume")
                .IsRequired();

            entityTypeBuilder
                .Property(e => e.OrderId)
                .HasColumnName("order_id")
                .HasColumnType("uuid");

            entityTypeBuilder
                .Property("CourierId")
                .HasColumnName("courier_id")
                .HasColumnType("uuid")
                .IsRequired();
        }
    }
}