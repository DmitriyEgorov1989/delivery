using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Configurations.OutboxMessage
{
    public class OutboxMessageTypeConfiguration : IEntityTypeConfiguration<Entities.OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<Entities.OutboxMessage> entityTypeBuilder)
        {
            entityTypeBuilder
                .ToTable("outbox");

            entityTypeBuilder
                .Property(p => p.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entityTypeBuilder
                .Property(p => p.Type)
                .HasColumnName("type")
                .IsRequired();

            entityTypeBuilder
                .Property(p => p.Content)
                .HasColumnName("content")
                .IsRequired();

            entityTypeBuilder
                .Property(p => p.OccurredOnUtc)
                .HasColumnName("occured_on_utc")
                .IsRequired();

            entityTypeBuilder
                .Property(p => p.ProcessedOnUtc)
                .HasColumnName("processed_on_utc");
        }
    }
}