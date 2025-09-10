using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Configurations.CourierAggregate
{
    public class CourierEntityConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> entityTypeBuilder)
        {

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
                    l.Property(c => c.X).HasColumnName("coordinate_x").IsRequired();
                    l.Property(c => c.Y).HasColumnName("coordinate_y").IsRequired();
                });

            entityTypeBuilder.Navigation(e => e.Location).IsRequired();

            entityTypeBuilder
                .HasMany(e => e.StoragePlaces)
                .WithOne(c => c.Courier)
                .HasForeignKey(sp => sp.CourierId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}