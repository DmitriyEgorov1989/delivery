using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services.DispatchCourier;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.AssignOrder
{
    public class AssignOrderHandle : IRequestHandler<AssignOrderComand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatchService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignOrderHandle(IOrderRepository orderRepository, ICourierRepository courierRepository, IUnitOfWork unitOfWork, IDispatchService dispatchService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        public async Task<UnitResult<Error>> Handle(AssignOrderComand request, CancellationToken cancellationToken)
        {
            {
                var order = await _orderRepository.GetCreatedAsync().GetValueOrThrow("Created order not found");
                var couriers = _courierRepository.GetAllFree().ToList();

                if (couriers.Count == 0)
                {
                    return UnitResult.Failure<Error>(GeneralErrors.ValueIsInvalid("Free couriers nit found"));
                }

                //Выбираем подходящего курьера
                var courierSuitable = _dispatchService.Scoring(order, couriers);

                //Если курьер не найден возвращаем неудачу
                if (courierSuitable.IsFailure)
                {
                    return courierSuitable;
                }
                
                var orderAssign = order.Assign(courierSuitable.Value.Id);
                var courierAssign = courierSuitable.Value.TakeOrder(order);

                if (courierAssign.IsFailure)
                {
                    return courierAssign;
                }
                if (orderAssign.IsFailure)
                {
                    return courierAssign;
                }
                
                _courierRepository.Update(courierSuitable.Value);
                _orderRepository.Update(order);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return UnitResult.Success<Error>();
            }
        }
    }
}