using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Comands.AssignOrder;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services.DispatchCourier;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
namespace DeliveryApp.UnitTests.Application
{
    public class AssignOrderComandShould
    {
        private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
        private readonly ICourierRepository _courierRepositoryMock = Substitute.For<ICourierRepository>();
        private readonly IDispatchService _dispatchServiceMock = Substitute.For<IDispatchService>();
        private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        private Maybe<Order> ExistedOrder()
        {
            return Order.Create(Guid.NewGuid(), Location.CreateRandom(), 3).Value;
        }

        private List<Courier> FreeCouriers()
        {
            return [Courier.Create("Тест", 2, Location.CreateRandom()).Value];
        }

        private List<Courier> EmptyListCouriers()
        {
            return [];
        }

        [Fact]
        public async Task AssignOrderComandCorrectly()
        {
            //Arrange
            var existedOrder = ExistedOrder();
            var freeCouriers = FreeCouriers();

            _orderRepositoryMock.GetCreatedAsync()
                                .Returns(Task.FromResult(existedOrder));
            _courierRepositoryMock.GetAllFree()
                                  .Returns(freeCouriers);
            _dispatchServiceMock.Scoring(existedOrder.Value, Arg.Any<IReadOnlyCollection<Courier>>())
                                .Returns(Result.Success<Courier, Error>(freeCouriers[0]));
            _unitOfWorkMock.SaveChangesAsync().Returns(true);

            //Act
            var comand = new AssignOrderComand();
            var handler = new AssignOrderHandler(_orderRepositoryMock, _courierRepositoryMock, _unitOfWorkMock, _dispatchServiceMock);
            var result = await handler.Handle(comand, new CancellationToken());

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnFailureIfCourierNotFound()
        {
            //Arrange
            _orderRepositoryMock.GetCreatedAsync()
                                           .Returns(Task.FromResult(ExistedOrder()));
            _courierRepositoryMock.GetAllFree()
                                  .Returns(EmptyListCouriers());
            _dispatchServiceMock
                                .Scoring(Arg.Any<Order>(), Arg.Any<IReadOnlyCollection<Courier>>())
                                .Returns(Result.Failure<Courier, Error>(GeneralErrors.NotFound()));
            _unitOfWorkMock.SaveChangesAsync().Returns(true);

            //Act
            var comand = new AssignOrderComand();
            var handler = new AssignOrderHandler(_orderRepositoryMock, _courierRepositoryMock, _unitOfWorkMock, _dispatchServiceMock);
            var result = await handler.Handle(comand, new CancellationToken());

            //Assert
            result.IsFailure.Should().BeTrue();
            _courierRepositoryMock.Received(1).GetAllFree();
            _dispatchServiceMock.Received(0).Scoring(Arg.Any<Order>(), Arg.Any<IReadOnlyCollection<Courier>>());
        }
    }
}