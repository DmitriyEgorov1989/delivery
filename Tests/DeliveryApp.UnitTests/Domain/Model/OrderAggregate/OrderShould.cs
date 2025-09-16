using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate
{
    public class OrderShould
    {
        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnCreated()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();

            //Act
            var result = Order.Create(orderId, location, 3);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(orderId);
            result.Value.Location.Should().Be(location);
            result.Value.Volume.Should().Be(3);
            result.Value.Status.Should().Be(OrderStatus.Created);
        }

        [Theory]
        [InlineData(null, 0)]

        public void ReturnErrorWhenIsParamsIsCorrectOnCreated(Location location, int volume)
        {
            //Arrange

            //Act
            var result = Order.Create(Guid.Empty, location, volume);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsSucessWhenStatusChangeOnAssign()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var corierId = Guid.NewGuid();
            var order = Order.Create(orderId, location, 3).Value;

            //Act
            var result = order.Assign(corierId);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnIsFailureWhenCourierIdNullsStatusChangeAssign()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var corierId = Guid.Empty;
            var order = Order.Create(orderId, location, 3).Value;

            //Act
            var result = order.Assign(corierId);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsFailureWhenStatusNotCreatedStatusChangeAssign()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var corierId = Guid.Empty;
            var order = Order.Create(orderId, location, 3).Value;
            var changeStatus = order.Assign(corierId);

            //Act
            var result = order.Assign(corierId);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsSucessWhenOrderComplete()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var courierId = Guid.NewGuid();
            var order = Order.Create(orderId, location, 3).Value;
            var changeStatus = order.Assign(courierId);

            //Act
            var result = order.Complete();

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnIsFailureWhenStatusNotAssignOrderComplete()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var order = Order.Create(orderId, location, 3).Value;

            //Act
            var result = order.Complete();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }
    }
}