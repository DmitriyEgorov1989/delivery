using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services.DispatchCourier;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services.DispatchCourier
{
    public class DispatchServiceTest
    {
        private readonly IDispatchService _dispatcherService;

        public DispatchServiceTest()
        {
            _dispatcherService = new DispatchService();
        }

        [Fact]
        public void GettingCourierAfterScoring()
        {
            //Arrange
            var locationCourier1 = Location.Create(1, 1).Value;
            var locationCourier2 = Location.Create(2, 2).Value;
            var courier1 = Courier.Create("Дальний", 2, locationCourier1).Value;
            var courier2 = Courier.Create("Ближний", 2, locationCourier2).Value;
            var orderLocation = Location.Create(3, 3).Value;
            var order = Order.Create(Guid.NewGuid(), orderLocation, 2).Value;
            IReadOnlyCollection<Courier> couriers = [courier1, courier2];

            //Act
            var result = _dispatcherService.Scoring(order, couriers);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Ближний");
        }

        /// <summary>
        /// Нет курьеров с достаточным местом в хранилище
        /// </summary>
        [Fact]
        public void ReturnIsFailureWhenNotFoundCourier()
        {
            //Arrange
            var locationCourier1 = Location.Create(1, 1).Value;
            var locationCourier2 = Location.Create(2, 2).Value;
            var courier1 = Courier.Create("Дальний", 2, locationCourier1).Value;
            var courier2 = Courier.Create("Ближний", 2, locationCourier2).Value;
            var orderLocation = Location.Create(3, 3).Value;
            var order = Order.Create(Guid.NewGuid(), orderLocation, 11).Value;
            IReadOnlyCollection<Courier> couriers = [courier1, courier2];

            //Act
            var result = _dispatcherService.Scoring(order, couriers);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }
    }
}