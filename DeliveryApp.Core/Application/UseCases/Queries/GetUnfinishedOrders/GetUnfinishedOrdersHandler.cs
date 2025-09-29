using Dapper;
using DeliveryApp.Core.Application.UseCases.ComonDto;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetUnfinishedOrders
{
    public class GetUnfinishedOrdersHandler : IRequestHandler<GetUnfinishedOrdersQuery, GetUnfinishedOrdersResponse>
    {
        private readonly string _connectionString;
        public GetUnfinishedOrdersHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }
        public async Task<GetUnfinishedOrdersResponse> Handle(GetUnfinishedOrdersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<dynamic>(
                @"Select id,location_x,location_y from public.orders where order_status <> 'completed'");

            if (result.AsList().Count == 0)
            {
                return null;
            }
            List<OrderDto> orders = [];

            foreach (var item in result)
            {
                var location = new LocationDto { X = item.location_x, Y = item.location_y };

                orders.Add(new OrderDto { Id = item.id, Location = location });
            }

            return new GetUnfinishedOrdersResponse(orders);
        }
    }
}