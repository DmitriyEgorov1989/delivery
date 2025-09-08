using DeliveryApp.Core.Domain.Model.CourierAggregate;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Порт для работы с курьерами
    /// </summary>
    public interface ICourierRepository
    {
        /// <summary>
        /// Добавление курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns></returns>
        Task AddAsync(Courier courier);

        /// <summary>
        /// Обновление данных курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns></returns>
        void Update(Courier courier);

        /// <summary>
        /// Получение курьера по Id
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns></returns>
        Task<Courier> GetByIdAsync(Guid courierId);

        /// <summary>
        /// Получение всех свободных курьеров
        /// </summary>
        /// <returns></returns>
        Task<List<Courier>> GetAllFreeAsync();
    }
}