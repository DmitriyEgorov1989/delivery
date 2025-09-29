using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.MoveCourier
{
    public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MoveCouriersHandler> _logger;

        public MoveCouriersHandler(ICourierRepository courierRepository,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            ILogger<MoveCouriersHandler> logger)
        {
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        public async Task<UnitResult<Error>> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
        {
            var ordersAssigned = _orderRepository.GetAllAssigned().ToList();
            if (ordersAssigned.Count == 0) return UnitResult.Failure(GeneralErrors.ValueIsRequired(nameof(ordersAssigned)));

            foreach (var order in ordersAssigned)
            {

                if (order.CourierId == Guid.Empty)
                    return UnitResult.Failure(GeneralErrors.ValueIsInvalid(nameof(order.CourierId)));

                var getCourierResult = await _courierRepository.GetByIdAsync((Guid)order.CourierId);

                if (getCourierResult.HasNoValue)
                {
                    return UnitResult.Failure(GeneralErrors.NotFound());
                }
                var courier = getCourierResult.Value;

                var courierMoveResult = courier.Move(order.Location);

                if (courierMoveResult.IsFailure) return courierMoveResult;
                
                _courierRepository.Update(courier);
                
                if (order.Status == null)
                {
                    Console.WriteLine("Вот тут ошибка");
                }
                if (courier.Location == order.Location)
                {
                    courier.СompleteOrder(order);
                    order.Complete();
                    _orderRepository.Update(order);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();
        }

    }
}