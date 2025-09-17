using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.MoveCourier
{
    public class MoveCourierHandler : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MoveCourierHandler(ICourierRepository courierRepository, IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UnitResult<Error>> Handle(MoveCourierCommand request, CancellationToken cancellationToken)
        {
            var ordersAssigned = _orderRepository.GetAllAssigned().ToList();
            if (ordersAssigned.Count == 0) return GeneralErrors.ValueIsRequired(nameof(ordersAssigned));

           foreach (var order in ordersAssigned) 
            {

                if (order.CourierId==null) return GeneralErrors.ValueIsInvalid(nameof(order.CourierId));
                
                var courier = await _courierRepository.GetByIdAsync((Guid)order.CourierId)
                                                      .GetValueOrThrow($"Courier with id {order.CourierId} not found");

                var courierMoveResult = courier.Move(order.Location);

                if (courierMoveResult.IsFailure) return courierMoveResult;

                if (courier.Location == order.Location)
                {
                    courier.СompleteOrder(order);
                    order.Complete();
                }
                _courierRepository.Update(courier);
                _orderRepository.Update(order);
            };
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return UnitResult.Success<Error>();
        }
    }
}