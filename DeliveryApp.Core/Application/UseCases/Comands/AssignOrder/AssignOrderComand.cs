using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Comands.AssignOrder
{
   public class AssignOrderComand:IRequest<UnitResult<Error>>
    {
    }
}
