using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.SharedKernel
{
    public class Location : ValueObject
    {
        private const int coordinateMin = 1;
        private const int coordinateMax = 10;
        public int X { get; }
        public int Y { get; }

        [ExcludeFromCodeCoverage]
        private Location() { }

        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < coordinateMin || x > coordinateMax)
            {
                return GeneralErrors.ValueIsRequired(nameof(x));
            }

            if (y < coordinateMin || y > coordinateMax)
            {
                return GeneralErrors.ValueIsRequired(nameof(y));
            }

            return new Location(x, y);
        }

        public static Location CreateRandom()
        {
            var randomValueX = Random.Shared.Next(coordinateMin, coordinateMax + 1);
            var randomValueY = Random.Shared.Next(coordinateMin, coordinateMax + 1);

            return new Location(randomValueX, randomValueY);
        }

        public Result<int, Error> DistanceTo(Location target)
        {
            if (target == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(target));
            }

            var distance = Math.Abs(X - target.X) + Math.Abs(Y - target.Y);

            return distance;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}