using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetUnfinishedOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DeliveryApp.Infrastructure.Adapters.Postgres.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDBPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["CONNECTION_STRING"];

            services.AddDbContext<ApplicationDbContext>((_, options) =>
            {
                options.UseNpgsql(connectionString,
                    sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
                options.EnableSensitiveDataLogging();
            });

            services.InitQueries(connectionString);
        }

        private static void InitQueries(this IServiceCollection services,string connectionString)
        {
            services.AddScoped<IRequestHandler<GetAllCouriersQuery, GetAllCouriersResponse>>(_=>
            new GetAllCouriersHandler(connectionString));
           
            services.AddScoped<IRequestHandler<GetUnfinishedOrdersQuery, GetUnfinishedOrdersResponse>>(_ => 
            new GetUnfinishedOrdersHandler(connectionString));
        }
    }
}