using DeliveryApp.Core.Application.UseCases.ComonDto;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    public class GetBusyCouriersResponse(List<CourierDto> couriers)
    {
        /// <summary>
        /// Список занятых курьеров
        /// </summary>
        public List<CourierDto> Couriers { get;} = new(couriers);
    }

    public class CourierDto
    {
        /// <summary>
        /// Идентификатор курьера
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Местоположение курьера
        /// </summary>
        public LocationDto Location { get; set; }
    }
}
