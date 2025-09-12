using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Порт взаимодействия с заказами
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Добавление заказа
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns></returns>
        Task AddAsync(Order order);

        /// <summary>
        /// Обновление заказа
        /// </summary>
        /// <param name="order">Закаp</param>
        /// <returns></returns>
        void Update(Order order);

        /// <summary>
        /// Получение закааза по Id
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <returns></returns>
        Task<Maybe<Order>> GetByIdAsync(Guid orderId);

        /// <summary>
        /// Получение любого заказа со статусом Created
        /// </summary>
        /// <returns></returns>
        Task<Maybe<Order>> GetCreated();

        /// <summary>
        /// Получение списка назначенных заказов
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetAllAssigned();
    }
}