using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetUnfinishedOrders
{
    public class GetUnfinishedOrdersQuery : IRequest<GetUnfinishedOrdersResponse>
    {
    }
}