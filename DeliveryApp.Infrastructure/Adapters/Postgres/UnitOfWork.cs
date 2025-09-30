using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SaveDomainEventsInOutboxEventMessage();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task SaveDomainEventsInOutboxEventMessage()
        {
            var outboxMessage = _dbContext.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(e => e.Entity)
                .SelectMany(aggregate =>
                {
                    //Получаем список доменных событий
                    var domainEvents = aggregate.GetDomainEvents();

                    aggregate.ClearDomainEvents();

                    return domainEvents;
                })
                .Select(domaimEvent => new OutboxMessage
                {
                    Id = domaimEvent.EventId,
                    Type = domaimEvent.GetType().Name,
                    OccurredOnUtc = DateTime.UtcNow,
                    Content = JsonConvert.SerializeObject(domaimEvent,
                    new JsonSerializerSettings
                    {
                        //Нужно чтобы знать какой тип возвращать
                        TypeNameHandling = TypeNameHandling.All
                    })

                }).ToList();

            // Добавяляем OutboxMessages в dbContext
            // После выполнения этой строки в DbContext будут находится сам Aggregate и OutboxMessages
            await _dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessage);
        }
    }
}