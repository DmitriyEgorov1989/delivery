using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.CreateOrder
{
    public class CreateOrderCommand : IRequest<UnitResult<Error>>
    {
        /// <summary>
        /// Ctr
        /// <summary>
        public CreateOrderCommand(Guid orderId, string street, int volume)
        {
            OrderId = orderId;
            Street = street;
            Volume = volume;
        }

        /// <summary>
        ///     Идентификатор заказа
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        ///     Улица
        /// </summary>
        /// <remarks>Корзина содержала полный Address, но для упрощения мы будем использовать только Street из Address</remarks>
        public string Street { get; }

        /// <summary>
        ///     Объем
        /// </summary>
        public int Volume { get; }

        /// <summary>
        /// Fabric Method
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <param name="street"> Аддрес</param>
        /// <param name="volume">Размер заказа</param>
        /// <returns></returns>
        public static Result<CreateOrderCommand, Error> Create(Guid orderId, string street, int volume)
        {
            if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(OrderId));
            if (string.IsNullOrEmpty(street)) return GeneralErrors.ValueIsRequired(nameof(Street));
            if (volume <= 0) return GeneralErrors.ValueIsRequired(nameof(Volume));

            return new CreateOrderCommand(orderId, street, volume);
        }
    }
}