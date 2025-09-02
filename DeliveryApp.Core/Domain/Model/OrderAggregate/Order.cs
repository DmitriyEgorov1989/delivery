using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
    public class Order : Aggregate<Guid>
    {
        public Order() { }

        public Location Location { get; set; }

        public int Volume { get; }

        public OrderStatus Status { get; }

        public Guid? CourierId { get; }
    }
}
