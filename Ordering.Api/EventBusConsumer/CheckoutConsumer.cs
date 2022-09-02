using AutoMapper;
using EventBusMessages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.Api.EventBusConsumer;

public class CheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public CheckoutConsumer(IMediator mediator, IMapper mapper)
    {
        this.mediator = mediator;
        this.mapper = mapper;
    }
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var command = mapper.Map<CheckoutOrderCommand>(context.Message);

        _ = await mediator.Send(command);
    }
}

