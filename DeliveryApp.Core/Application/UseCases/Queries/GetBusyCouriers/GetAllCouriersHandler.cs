using Dapper;
using DeliveryApp.Core.Application.UseCases.ComonDto;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    public class GetAllCouriersHandler : IRequestHandler<GetAllCouriersQuery, GetAllCouriersResponse>
    {
        private readonly string _connectionString;
        public GetAllCouriersHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<GetAllCouriersResponse> Handle(GetAllCouriersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT id,name,location_x,location_y from public.couriers", new { });
            
            if (result.AsList().Count == 0)
            {
                return null;
            }

            List<CourierDto> couriers = [];

            foreach (var item in result)
            {
                couriers.Add(MapToCourierDto(item));
            }
            
            return new GetAllCouriersResponse(couriers);
        }

        private CourierDto MapToCourierDto(dynamic response)
        {
            var location  = new LocationDto { X = response.location_x ,Y = response.location_y};

            return new CourierDto
            {
                Id = response.id,
                Name = response.name,
                Location = location,
            };
        }
    }
}