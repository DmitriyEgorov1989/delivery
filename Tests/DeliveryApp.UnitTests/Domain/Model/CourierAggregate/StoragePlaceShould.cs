using DeliveryApp.Core.Domain.Model.NewFolder;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class StoragePlaceShould
    {
        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnCreated()
        {
            //Arrange

            //Act
            var result = StoragePlace.Create("Рюкзак", 5);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Рюкзак");
            result.Value.TotalVolume.Should().Be(5);
            result.Value.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData(null, 0)]
        public void ReturnErrorWhenIsParamsIsCorrectOnCreated(string name, int totalVolume)
        {
            //Arrange

            //Act
            var result = StoragePlace.Create(name, totalVolume);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnStorage()
        {
            //Arrange
            var storage = StoragePlace.Create("Рюкзак", 5).Value;
            var orderId = Guid.NewGuid();

            //Act
            var result = storage.Store(orderId, 4);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 10)]
        public void ReturnErrorWhenIsParamsIsCorrectOnStorage(Guid? orderId, int volume)
        {
            //Arrange
            var storage = StoragePlace.Create("Рюкзак", 5).Value;

            //Act
            var result = storage.Store(orderId, volume);

            //Assert;
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnСlear()
        {
            //Arrange
            var storage = StoragePlace.Create("Рюкзак", 5).Value;
            var orderId = typeof(StoragePlace).GetProperty(nameof(StoragePlace.OrderId));
            orderId!.SetValue(storage, Guid.Parse("3f3c2c62-87fd-4a90-b442-6d22f7c7f882"));

            //Act
            var result = storage.Clear(Guid.Parse("3f3c2c62-87fd-4a90-b442-6d22f7c7f882"));

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnErrorWhenIsParamsIsCorrectOnСlear()
        {
            //Arrange
            var storage = StoragePlace.Create("Рюкзак", 5).Value;
            var orderId = typeof(StoragePlace).GetProperty(nameof(StoragePlace.OrderId));
            orderId!.SetValue(storage, Guid.Parse("3f3c2c62-87fd-4a90-b442-6d22f7c7f882"));

            //Act
            var result = storage.Clear(Guid.NewGuid());

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }
    }
}