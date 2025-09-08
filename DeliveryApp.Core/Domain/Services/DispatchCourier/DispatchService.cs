using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services.DispatchCourier
{
    public class DispatchService : IDispatchService
    {
        public Result<Courier, Error> Scoring(Order order, List<Courier> freeCouriers)
        {
            if (order == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(order));
            }

            if (freeCouriers == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(freeCouriers));
            }

            var couriers = freeCouriers.Where(x => x.CanTakeOrder(order).IsSuccess);

            if (!couriers.Any())
            {
                return GeneralErrors.NotFound();
            }
            var courier = couriers.OrderBy(x => x.CalculateTimeToLocation(order.Location)).ToList()[0];
            courier.TakeOrder(order);

            return courier;
        }
    }
}