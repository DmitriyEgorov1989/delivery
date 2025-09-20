using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderComand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UnitResult<Error>> Handle(CreateOrderComand request, CancellationToken cancellationToken)
        {
            var getOrderResult = await _orderRepository.GetByIdAsync(request.OrderId);
            if (getOrderResult.HasValue) return UnitResult.Success<Error>();

            var createResultOrder = Order.Create(request.OrderId, Location.CreateRandom(), request.Volume);
            if(createResultOrder.IsFailure) 
            {
                return createResultOrder;
            }
            var newOrder = createResultOrder.Value;
            
            await _orderRepository.AddAsync(newOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();         
        }
    }
}