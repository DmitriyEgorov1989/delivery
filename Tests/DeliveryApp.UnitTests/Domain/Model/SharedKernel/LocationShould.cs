using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationShould
    {
        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnCreated()
        {
            //Arrange

            //Act
            var location = Location.Create(5, 5);

            //Assert
            location.IsSuccess.Should().BeTrue();
            location.Value.X.Should().Be(5);
            location.Value.Y.Should().Be(5);
        }

        [Theory]
        [InlineData(0, 11)]
        [InlineData(11, 0)]
        public void ReturnErrorWhenIsParamsIsCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var firstLocation = Location.Create(5, 5).Value;
            var secondLocation = Location.Create(5, 5).Value;

            //Act
            var location = firstLocation == secondLocation;

            //Assert
            location.Should().BeTrue();
        }

        [Fact]
        public void BeNotEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var firstLocation = Location.Create(5, 1).Value;
            var secondLocation = Location.Create(1, 5).Value;

            //Act
            var location = firstLocation == secondLocation;

            //Assert
            location.Should().BeFalse();
        }

        [Fact]
        public void ReturnDistanceWhenIsParamsIsCorrectOnDistanceTo()
        {
            //Arrange
            var locationCurier = Location.Create(1, 1).Value;
            var locationTarget = Location.Create(3, 3).Value;

            //Act
            var result = locationCurier.DistanceTo(locationTarget);

            //Assert
            result.Value.Should().Be(4);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnErrorWhenTargetNullOnDistanceTo()
        {
            //Arrange
            var locationCurier = Location.Create(1, 1).Value;

            //Act
            var result = locationCurier.DistanceTo(null);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(GeneralErrors.NotFound());
        }

        [Fact]
        public void BeCorrectWhenIsParamsIsCorrectOnCreatedRandom()
        {
            //Arrange

            //Act
            var location = Location.CreateRandom();

            //Assert
            location.X.Should().BeInRange(1, 10);
            location.X.Should().BeInRange(1, 10);
        }
    }
}