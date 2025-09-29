using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Configurations.CourierAggregate
{
    public class CourierEntityConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("couriers");

            entityTypeBuilder.HasKey(e => e.Id);

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
                .Property(e => e.Speed)
                .HasColumnName("speed")
                .IsRequired();

            entityTypeBuilder
                .OwnsOne(e => e.Location, l =>
                {
                    l.Property(c => c.X).HasColumnName("location_x").IsRequired();
                    l.Property(c => c.Y).HasColumnName("location_y").IsRequired();
                });

            entityTypeBuilder.Navigation(e => e.Location).IsRequired();
        }
    }
}