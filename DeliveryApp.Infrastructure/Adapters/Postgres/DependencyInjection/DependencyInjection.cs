using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataBaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CONNECTION_STRING");

            services.AddDbContext<ApplicationDbContext>((_, options) =>
            {
                options.UseNpgsql(connectionString,
                    sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
                options.EnableSensitiveDataLogging();
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.InitRepositories();
        }
        private static void InitRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICourierRepository, CourierRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}