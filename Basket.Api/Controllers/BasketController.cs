using AutoMapper;
using Basket.Api.Entities;
using Basket.Api.Repositories;
using EventBusMessages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository basketRepository;
    private readonly IMapper mapper;
    private readonly IPublishEndpoint publishEndpoint;

    public BasketController(IBasketRepository basketRepository, 
                            IMapper mapper, 
                            IPublishEndpoint publishEndpoint)
    {
        this.basketRepository = basketRepository;
        this.mapper = mapper;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await basketRepository.GetBasket(userName);

        return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
      => Ok(await basketRepository.UpdateBasket(shoppingCart));


    [HttpDelete("{userName}")]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await basketRepository.DeleteBasket(userName);
        return Ok();
    }

    [HttpPost("Checkout")]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout) 
    {
        var basket = await basketRepository.GetBasket(basketCheckout.UserName);

        if (basket is null)
            return BadRequest();

        //Mapeamos el objecto de checkout a el evento
        var eventMessage = mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;

        ///Enviamos el evento a RabbitMQ
        await publishEndpoint.Publish(eventMessage);

        await basketRepository.DeleteBasket(basket.UserName);

        return Accepted();    
    }
}
