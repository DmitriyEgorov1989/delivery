using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeoClient _geoClient;

        public CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IGeoClient geoClient)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _geoClient = geoClient;
        }

        public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var getOrderResult = await _orderRepository.GetByIdAsync(request.OrderId);
            if (getOrderResult.HasValue) return UnitResult.Success<Error>();

            var getOrderLocationResult = await _geoClient.GetLocationAsync("Аналитическая", cancellationToken);
            if (getOrderLocationResult.IsFailure) return GeneralErrors.ValueIsInvalid("Аналитическая");
            var orderLocation = getOrderLocationResult.Value;
            var newOrder = Order.Create(request.OrderId, orderLocation, request.Volume);
            if(newOrder.IsFailure) 
            {
                return newOrder;
            }
            
            await _orderRepository.AddAsync(newOrder.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();         
        }
    }
}