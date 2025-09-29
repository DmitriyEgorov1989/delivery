using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        private bool _disposed;

        public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await PublishDomainEventAsync();

            return true;
        }

        private async Task PublishDomainEventAsync()
        {
            //Получили агрегаты в которых есть доменные события
            var domainEntities = _dbContext.ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(e => e.Entity.GetDomainEvents().Any());
            
            //Перекладываем в другую переменную
            var domainEvents = domainEntities
                .SelectMany(e=>e.Entity.GetDomainEvents())
                .ToList();

            // Очищаем списолк с доменными событиями
            domainEntities.ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            //Публикуем доменные события
            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);                          
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                _disposed = true;
            }
        }
    }
}