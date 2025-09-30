using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using JsonNet.ContractResolvers;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.BackGroundsJob
{
    public class ProcessOutboxMessageJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public ProcessOutboxMessageJob(ApplicationDbContext context, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //Ищем не отправленнын сообщения,сортируем по дате создания,и получаем пачками.
            var outboxMessages = await _context
                .Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(20)
                .ToListAsync();
            
            if (outboxMessages.Any())
            {
                foreach (var outboxMessage in outboxMessages)
                {
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new PrivateSetterContractResolver(),
                        TypeNameHandling = TypeNameHandling.All
                    };

                    var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content, settings);

                    await _mediator.Publish(domainEvent, context.CancellationToken);

                    // Если предыдущий метод не вернул ошибку, значит отправка была успешной
                    // Ставим дату отправки, это будет признаком, что сообщение отправлять больше не нужно 
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}