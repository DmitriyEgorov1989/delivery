using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents
{
    public  record OrderCompleteDomainEvent(Guid orderId,Guid courierid):DomainEvent;
}