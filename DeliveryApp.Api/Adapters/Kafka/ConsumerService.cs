using BasketConfirmed;
using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Comands.CreateOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeliveryApp.Api.Adapters.Kafka
{
    public class ConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;    
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;

        public ConsumerService(IServiceScopeFactory scopeFactory, IOptions<Settings> settings)
        {
            _scopeFactory = scopeFactory
                ?? throw new ArgumentNullException(nameof(scopeFactory));
            if (string.IsNullOrWhiteSpace(settings.Value.MessageBrokerHost))
                throw new ArgumentException(nameof(settings.Value.MessageBrokerHost));
            if (string.IsNullOrWhiteSpace(settings.Value.BasketConfirmedTopic))
                throw new ArgumentException(nameof(settings.Value.BasketConfirmedTopic));

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.Value.MessageBrokerHost,
                GroupId = "OrderConsumerGroup",
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _topic = settings.Value.BasketConfirmedTopic;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);

                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult.IsPartitionEOF) continue;

                    var basketConfirmedIntegrationEvent =
                        JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);

                    var orderCreateComanResult =
                        CreateOrderCommand.Create(Guid.Parse(basketConfirmedIntegrationEvent.BasketId),
                        basketConfirmedIntegrationEvent.Address.Street, basketConfirmedIntegrationEvent.Volume);

                    if (orderCreateComanResult.IsFailure) Console.WriteLine(orderCreateComanResult.Error);

                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetService<IMediator>();
                    
                    var sendResult = await mediator.Send(orderCreateComanResult.Value, stoppingToken);

                    if (sendResult.IsFailure) Console.WriteLine(sendResult.Error);

                    try
                    {
                        _consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}