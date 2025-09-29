using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
    public class Order : Aggregate<Guid>
    {
        /// <summary>
        /// ctr
        /// </summary>
        public Order() { }

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="location"></param>
        /// <param name="volume"></param>
        [ExcludeFromCodeCoverage]
        public Order(Guid orderId, Location location, int volume) : this()
        {
            Id = orderId;
            Status = OrderStatus.Created;
            Location = location;
            Volume = volume;
            
            RaiseDomainEvent(new OrderCreateDomainEvent(Id));
        }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Обьем Заказа
        /// </summary>
        public int Volume { get; }

        /// <summary>
        /// Статус
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// Id курьера
        /// </summary>
        public Guid? CourierId { get; private set; }

        /// <summary>
        /// Fabric Method
        /// </summary>
        /// <param name="orderid">Id заказа</param>
        /// <param name="location">Локация доставки</param>
        /// <param name="volume">Обьем заказа</param>
        /// <returns></returns>}
        public static Result<Order, Error> Create(Guid orderid, Location location, int volume)
        {
            if (orderid == Guid.Empty)
            {
                return GeneralErrors.ValueIsInvalid(nameof(orderid));
            }

            if (location == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(orderid));
            }

            if (volume <= 0)
            {
                return GeneralErrors.ValueIsRequired(nameof(volume));
            }
            return new Order(orderid, location, volume);
        }

        /// <summary>
        /// Назначение курьера
        /// </summary>
        /// <param name="courier"></param>
        /// <returns></returns>
        public UnitResult<Error> Assign(Guid? courierId)
        {
            if (courierId == null)
            {
                return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(courierId)));
            }
            if (Status != OrderStatus.Created)
            {
                return UnitResult.Failure(GeneralErrors.ValueIsRequired($"{nameof(Status)}"));
            }
            Status = OrderStatus.Assigned;
            CourierId = courierId;
       
            return UnitResult.Success<Error>();
        }
        /// <summary>
        /// Заказ выполнен
        /// </summary>
        /// <returns></returns>
        public UnitResult<Error> Complete()
        {
            if (Status != OrderStatus.Assigned)
            {
                return GeneralErrors.ValueIsInvalid(nameof(Status));
            }
            Status = OrderStatus.Completed;

            RaiseDomainEvent(new OrderCompleteDomainEvent(Id,(Guid)CourierId));
            return UnitResult.Success<Error>();
        }
    }
}