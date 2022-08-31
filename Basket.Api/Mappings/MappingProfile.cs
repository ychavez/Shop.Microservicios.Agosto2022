using AutoMapper;
using Basket.Api.Entities;
using EventBusMessages.Events;

namespace Basket.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>()
                .ReverseMap();
        }
    }
}
