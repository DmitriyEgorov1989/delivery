using DeliveryApp.Core.Domain.Model.NewFolder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Configurations.OrderAggregate
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("orders");

            entityTypeBuilder.HasKey(x => x.Id);

            entityTypeBuilder
                .Property(e => e.Id)
                .HasColumnType("uuid")
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            entityTypeBuilder
                .OwnsOne(e => e.Location, l =>
                {
                    l.Property(c => c.X).HasColumnName("location_x").IsRequired();
                    l.Property(c => c.Y).HasColumnName("location_y").IsRequired();
                });

            entityTypeBuilder
                .OwnsOne(e => e.Status, l =>
                {
                    l.Property(s => s.Name).HasColumnName("order_status").HasDefaultValue("created").IsRequired();
                    l.WithOwner();
                });
            entityTypeBuilder.Navigation(e => e.Status).IsRequired();

            entityTypeBuilder
                .Property(e => e.Volume)
                .HasColumnName("volume")
                .IsRequired();

            entityTypeBuilder
                .Property(e => e.CourierId)
                .HasColumnName("courier_id")
                .HasColumnType("uuid");
        }
    }
}