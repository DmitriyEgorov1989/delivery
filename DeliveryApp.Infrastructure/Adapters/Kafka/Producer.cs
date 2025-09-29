using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderStatusChanged;

namespace DeliveryApp.Infrastructure.Adapters.Kafka
{
    public class Producer : IMessageBusProducer
    {
        private readonly ProducerConfig _config;

        private readonly string _topic;

        public Producer(IOptions<Settings> options)
        {
            if(string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost))
                throw new ArgumentNullException(nameof(options.Value.MessageBrokerHost));
            if(string.IsNullOrWhiteSpace(options.Value.OrderStatusChangedTopic))
                throw new ArgumentNullException(nameof(options.Value.OrderStatusChangedTopic));

            _config = new ProducerConfig
            {
                BootstrapServers = options.Value.MessageBrokerHost
            };

            _topic = options.Value.OrderStatusChangedTopic;
        }

        public async Task PublischCompleteOrderDomainEventAsync(OrderCompleteDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderCompletedIntegrationEvent = new OrderCompletedIntegrationEvent
            {
                EventId = notification.EventId.ToString(),
                OccurredAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(notification.OccurredAt),
                OrderId = notification.orderId.ToString(),
                CourierId = notification.courierid.ToString()
            };

            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonConvert.SerializeObject(orderCompletedIntegrationEvent)
            };

            using var producer =
                new ProducerBuilder<string, string>(_config).Build();

            await producer.ProduceAsync(_topic, message, cancellationToken);
        }

        public async Task PublischCreateOrderDomainEventAsync(OrderCreateDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent
            {
                EventId = notification.EventId.ToString(),
                OccurredAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(notification.OccurredAt),
                OrderId = notification.orderId.ToString()
            };

            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonConvert.SerializeObject(orderCreatedIntegrationEvent)
            };

            using var producer = 
                new ProducerBuilder<string,string>(_config).Build(); 

            await producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}