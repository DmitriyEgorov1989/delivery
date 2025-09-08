using DeliveryApp.Core.Domain.Services.DispatchCourier;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Core.Domain.DependencyInjection
{
    /// <summary>
    /// Сервис внедрения зависимостей
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавление зависимостей сервисов DomainService
        /// </summary>
        /// <param name="services"></param>
        public static void AddDomainService(this IServiceCollection services)
        {
            services.InitService();
        }

        /// <summary>
        /// Установка вhемени жизни сервисов DomainService
        /// </summary>
        /// <param name="services"></param>
        private static void InitService(this IServiceCollection services)
        {
            services.AddTransient<IDispatchService, DispatchService>();
        }
    }
}