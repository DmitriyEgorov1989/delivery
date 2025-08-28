using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.SharedKernel
{
    public class Location : ValueObject
    {
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
            if (x < 1 || x > 10)
            {
                return GeneralErrors.ValueIsRequired(nameof(x));
            }

            if (y < 1 || y > 10)
            {
                return GeneralErrors.ValueIsRequired(nameof(y));
            }

            return new Location(x, y);
        }

        public static Location CreateRandom()
        {
            var randomValueX = Random.Shared.Next(1, 11);
            var randomValueY = Random.Shared.Next(1, 11);

            return new Location(randomValueX, randomValueY);
        }

        public Result<int, Error> DistanceTo(Location target)
        {
            if (target == null)
            {
                return GeneralErrors.NotFound();
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