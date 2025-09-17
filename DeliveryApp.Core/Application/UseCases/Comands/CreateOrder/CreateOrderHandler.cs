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
            var newOrder = Order.Create(request.OrderId, Location.CreateRandom(), request.Volume);
            
            await _orderRepository.AddAsync(newOrder.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();         
        }
    }
}
