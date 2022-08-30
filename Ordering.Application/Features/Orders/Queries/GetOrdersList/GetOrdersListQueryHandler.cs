using AutoMapper;
using MediatR;
using Ordering.Application.Contracts;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryHandler :
    IRequestHandler<GetOrdersListQuery, List<OrdersViewModel>>
{
    private readonly IRepositoryBase<Order> repository;
    private readonly IMapper mapper;

    public GetOrdersListQueryHandler(IRepositoryBase<Order> repository,
        IMapper mapper)
        => (this.repository, this.mapper) = (repository, mapper);

    public async Task<List<OrdersViewModel>> Handle(GetOrdersListQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await repository
            .GetAllAsync(x => x.UserName == request.UserName);

        return mapper.Map<List<OrdersViewModel>>(orders);
    }
}

