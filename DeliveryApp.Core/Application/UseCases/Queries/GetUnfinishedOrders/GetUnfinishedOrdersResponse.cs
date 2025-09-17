using DeliveryApp.Core.Application.UseCases.ComonDto;
using DeliveryApp.Core.Domain.Model.SharedKernel;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetUnfinishedOrders
{
    public class GetUnfinishedOrdersResponse(List<OrderDto> orders)
    {
        public List<OrderDto> Orders = new(orders);
    }

    public class OrderDto
    { 
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Адрес заказа
        /// </summary>
        public LocationDto Location { get; set; }
    }

}