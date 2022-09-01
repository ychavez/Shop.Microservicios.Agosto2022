using AutoMapper;
using EventBusMessages.Events;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
