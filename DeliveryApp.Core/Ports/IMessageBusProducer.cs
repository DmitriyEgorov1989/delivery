using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports
{
    public interface IMessageBusProducer
    {
        Task PublischCreateOrderDomainEventAsync(OrderCreateDomainEvent notification,CancellationToken cancellationToken);
        Task PublischCompleteOrderDomainEventAsync(OrderCompleteDomainEvent notification, CancellationToken cancellationToken);
    }
}