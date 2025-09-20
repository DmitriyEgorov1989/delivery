using DeliveryApp.Core.Application.UseCases.Comands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetUnfinishedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http.Contract
{
    public class DeliveryController : DefaultApiController
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Добавить курьера
        /// </summary>
        /// <remarks>Позволяет добавить курьера</remarks>
        /// <param name="newCourier">Курьер</param>
        /// <response code="201">Успешный ответ</response>
        /// <response code="400">Ошибка валидации</response>
        /// <response code="409">Ошибка выполнения бизнес логики</response>
        /// <response code="0">Ошибка</response>
        public override Task<IActionResult> CreateCourier([FromBody] NewCourier newCourier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создать заказ
        /// </summary>
        /// <remarks>Позволяет создать заказ с целью тестирования</remarks>
        /// <response code="201">Успешный ответ</response>
        /// <response code="0">Ошибка</response>
        public override async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand = new CreateOrderCommand(orderId, street, 3);
            var response = await _mediator.Send(createOrderCommand);
            if (response.IsSuccess) return Ok();
            return Conflict();
        }

        /// <summary>
        /// Получить всех курьеров
        /// </summary>
        /// <remarks>Позволяет получить всех курьеров</remarks>
        /// <response code="200">Успешный ответ</response>
        /// <response code="0">Ошибка</response>
        public override async Task<IActionResult> GetCouriers()
        {
            var query = new GetAllCouriersQuery();
            var response = await _mediator.Send(query);

            if (response is null) return NotFound();

            var couriers = response.Couriers.Select(c => new Courier
            {
                Id = c.Id,
                Name = c.Name,
                Location = new Location { X = c.Location.X, Y = c.Location.Y },
            });
            return Ok(couriers);
        }

        /// <summary>
        /// Получить все незавершенные заказы
        /// </summary>
        /// <remarks>Позволяет получить все незавершенные заказы</remarks>
        /// <response code="200">Успешный ответ</response>
        /// <response code="0">Ошибка</response>
        public override async Task<IActionResult> GetOrders()
        {
            var query = new GetUnfinishedOrdersQuery();
            var response = await _mediator.Send(query);

            if (response is null) return NotFound();

            var orders = response.Orders.Select(o => new Order
            {
                Id = o.Id,
                Location = new Location { X = o.Location.X, Y = o.Location.Y },
            });
            return Ok(orders);
        }
    }
}