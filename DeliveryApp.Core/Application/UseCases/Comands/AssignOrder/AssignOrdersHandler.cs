using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services.DispatchCourier;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.AssignOrder
{
    public class AssignOrdersHandler : IRequestHandler<AssignOrdersCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatchService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignOrdersHandler(IOrderRepository orderRepository,
            ICourierRepository courierRepository,
            IUnitOfWork unitOfWork,
            IDispatchService dispatchService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        public async Task<UnitResult<Error>> Handle(AssignOrdersCommand request, CancellationToken cancellationToken)
        {
            {
                var maybeOrder = await _orderRepository.GetCreatedAsync();
                
                if (maybeOrder.HasNoValue)
                {
                    return UnitResult.Success<Error>();
                }
                var order = maybeOrder.Value;
                
                var couriers = _courierRepository.GetAllFree().ToList();

                if (couriers.Count == 0)
                {
                    return new Error("Not.Free.Couriers","Free couriers nit found");
                }

                //Выбираем подходящего курьера
                var scoringResult = _dispatchService.Scoring(order, couriers);

                //Если курьер не найден возвращаем неудачу
                if (scoringResult.IsFailure)
                {
                    return scoringResult;
                }
                var courierSuitable = scoringResult.Value;
                
                var orderAssign = order.Assign(courierSuitable.Id);
                var courierAssign = courierSuitable.TakeOrder(order);

                if (orderAssign.IsFailure)
                {
                    return orderAssign;
                }

                if (courierAssign.IsFailure)
                {
                    return courierAssign;
                }
                
                _courierRepository.Update(courierSuitable);
                _orderRepository.Update(order);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return UnitResult.Success<Error>();
            }
        }
    }
}