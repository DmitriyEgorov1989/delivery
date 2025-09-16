using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierShould
    {
        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnCreated()
        {
            //Arrange
            var location = Location.CreateRandom();

            //Act
            var result = Courier.Create("Дима", 2, location);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Дима");
            result.Value.Speed.Should().Be(2);
            result.Value.Location.Should().Be(location);
        }

        [Theory]
        [InlineData("", 0)]

        public void ReturnErrorWhenIsParamsIsCorrectOnCreated(string name, int speed)
        {
            //Arrange

            //Act
            var result = Courier.Create(name, speed, null);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsSucessWhenIsParamsIsCorrectAddStoragePlace()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 2, location).Value;

            //Act
            var result = courier.AddStoragePlace("Рюкзак", 5);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [InlineData("", 0)]
        public void ReturnIsFailureWhenIsParamsIsCorrectAddStoragePlace(string name, int volume)
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 2, location).Value;

            //Act
            var result = courier.AddStoragePlace(name, volume);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnTrueWhenIsParamsIsCorrectCanTakeOrder()
        {
            //Arrange
            var location = Location.CreateRandom();
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, location, 3);
            var courier = Courier.Create("Дима", 5, location).Value;

            //Act
            var result = courier.CanTakeOrder(order.Value);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Theory]
        [InlineData(11)]
        public void ReturnFalseWhenIsParamsIsCorrectCanTakeOrder(int volume)
        {
            //Arrange
            var location = Location.CreateRandom();
            var orderId = Guid.Parse("3f7c8f63-0b72-4d59-a46d-1e62f3e64a6a\r\n");
            var order = Order.Create(orderId, location, volume);
            var courier = Courier.Create("Дима", 3, location).Value;

            //Act
            var result = courier.CanTakeOrder(order.Value);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(false);
        }

        [Fact]
        public void ReturnIsSucessWhenIsTakeOrder()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 2, location).Value;
            var order = Order.Create(Guid.NewGuid(), location, 5);

            //Act
            var result = courier.TakeOrder(order.Value);
            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnIsFailureWhenOrderSizeLargeStorageIsTakeOrder()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 2, location).Value;
            var order = Order.Create(Guid.NewGuid(), location, 11);


            //Act
            var result = courier.TakeOrder(order.Value);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsFailureWhenOrderIdInStorageNotNullTakeOrder()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 5, location).Value;
            var orderOne = Order.Create(Guid.NewGuid(), location, 3);
            var orderSecond = Order.Create(Guid.NewGuid(), location, 4);
            courier.TakeOrder(orderOne.Value);

            //Act
            var result = courier.TakeOrder(orderSecond.Value);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnIsSuccessWhenOrderComplete()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("Дима", 5, location).Value;
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, location, 3);
            courier.TakeOrder(order.Value);

            //Act
            var result = courier.СompleteOrder(order.Value);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeCorrectWhenIsParamsIsCalculateTimeToLocation()
        {
            //Arrange
            var locationCourier = Location.Create(1, 1).Value;
            var target = Location.Create(5, 5).Value;
            var courier = Courier.Create("Дима", 2, locationCourier).Value;

            //Act
            var result = courier.CalculateTimeToLocation(target);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(4);
        }
    }
}