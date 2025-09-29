using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.AssignOrder
{
   public class AssignOrdersCommand : IRequest<UnitResult<Error>>
    {
    }
}
