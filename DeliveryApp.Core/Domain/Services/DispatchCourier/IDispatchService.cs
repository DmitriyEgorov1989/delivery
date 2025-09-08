using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services.DispatchCourier
{

    /// <summary>
    /// Интерфейс назначения курьеров
    /// </summary>
    public interface IDispatchService
    {
        /// <summary>
        /// Скоринг курьеров
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="couriers">Список свободных курьеров(CanTakeOrder()==true)</param>
        /// <returns></returns>
        Result<Courier, Error> Scoring(Order order, List<Courier> couriers);
    }
}