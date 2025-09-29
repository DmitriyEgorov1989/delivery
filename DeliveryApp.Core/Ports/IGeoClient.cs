using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IGeoClient
    {
       Task<Result<Location,Error>> GetLocationAsync(string street,CancellationToken cancellationToken);
    }
}