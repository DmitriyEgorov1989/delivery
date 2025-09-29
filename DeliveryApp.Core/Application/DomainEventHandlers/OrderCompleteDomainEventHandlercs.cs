using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public class OrderCompleteDomainEventHandler : INotificationHandler<OrderCompleteDomainEvent>
    {
        private readonly IMessageBusProducer _messageBusProducer;

        public OrderCompleteDomainEventHandler(IMessageBusProducer messageBusProducer)
        {
            _messageBusProducer = messageBusProducer;
        }

        public async Task Handle(OrderCompleteDomainEvent notification, CancellationToken cancellationToken)
        {
            await _messageBusProducer.PublischCompleteOrderDomainEventAsync(notification, cancellationToken); 
        }
    }
}