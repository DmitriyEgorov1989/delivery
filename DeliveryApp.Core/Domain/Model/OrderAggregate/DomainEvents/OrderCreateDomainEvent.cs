using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents
{
    public record OrderCreateDomainEvent(Guid orderId) : DomainEvent;
}