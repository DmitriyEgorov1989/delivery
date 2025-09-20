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

                var createOrder = maybeOrder.Value;

                var couriers = _courierRepository.GetAllFree().ToList();

                if (couriers.Count == 0)
                {
                    return new Error("Not_Available_Couriers", "Нет свободных курьеров");
                }

                //Выбираем подходящего курьера
                var courierSuitable = _dispatchService.Scoring(createOrder, couriers);

                //Если курьер не найден возвращаем неудачу
                if (courierSuitable.IsFailure)
                {
                    return courierSuitable;
                }

                var orderAssign = createOrder.Assign(courierSuitable.Value.Id);

                if (orderAssign.IsFailure)
                {
                    return orderAssign;
                }

                var courierAssign = courierSuitable.Value.TakeOrder(createOrder);

                if (courierAssign.IsFailure)
                {
                    return courierAssign;
                }

                _courierRepository.Update(courierSuitable.Value);
                _orderRepository.Update(createOrder);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return UnitResult.Success<Error>();
            }
        }
    }
}