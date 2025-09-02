using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.SharedKernel
{
    public class OrderStatus : ValueObject
    {
        public static OrderStatus Created = new(nameof(Created).ToLowerInvariant());
        public static OrderStatus Assigned = new(nameof(Assigned).ToLowerInvariant());
        public static OrderStatus Completed = new(nameof(Completed).ToLowerInvariant());

        /// <summary>
        /// Ctr
        /// </summary>
        private OrderStatus() { }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="name">Название</param>
        [ExcludeFromCodeCoverage]
        private OrderStatus(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}