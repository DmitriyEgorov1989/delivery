using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public class OrderCreateDomainEventHandler : INotificationHandler<OrderCreateDomainEvent>
    {
        private readonly IMessageBusProducer _messageBusProducer;

        public OrderCreateDomainEventHandler(IMessageBusProducer messageBusProducer)
        {
            _messageBusProducer = messageBusProducer;
        }

        public async Task Handle(OrderCreateDomainEvent notification, CancellationToken cancellationToken)
        {
            await _messageBusProducer.PublischCreateOrderDomainEventAsync(notification, cancellationToken);
        }
    }
}