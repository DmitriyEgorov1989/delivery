using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Comands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application
{
    public class CreateOrderComandShould
    {
        private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
        private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        private Maybe<Order> ExistedOrder()
        {
            return Order.Create(Guid.NewGuid(), Location.CreateRandom(), 3).Value;
        }

        private Maybe<Order> EmptyOrder()
        {
            return null;
        }

        [Fact]
        public async Task CreateOrderComandCorrectly()
        {
            //Arrange
            _orderRepositoryMock.GetByIdAsync(Arg.Any<Guid>())
                                .Returns(Task.FromResult(EmptyOrder()));
            _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(true));

            //Act
            var createOrderComand = CreateOrderCommand.Create(Guid.NewGuid(), "Тест", 3);
            createOrderComand.IsSuccess.Should().BeTrue();
            var handle = new CreateOrderHandler(_orderRepositoryMock, _unitOfWorkMock);
            var result = await handle.Handle(createOrderComand.Value, new CancellationToken());

            //Assert
            result.IsSuccess.Should().BeTrue();
            await _orderRepositoryMock.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _unitOfWorkMock.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task ReturnIsSucessWhenOrderExists()
        {
            //Arrange
            _orderRepositoryMock.GetByIdAsync(Arg.Any<Guid>())
                                .Returns(Task.FromResult(ExistedOrder()));
            _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(true));

            //Act
            var createOrderComand = CreateOrderCommand.Create(Guid.NewGuid(), "Тест", 3);
            createOrderComand.IsSuccess.Should().BeTrue();
            var handle = new CreateOrderHandler(_orderRepositoryMock, _unitOfWorkMock);
            var result = await handle.Handle(createOrderComand.Value, new CancellationToken());

            //Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
