using Basket.Api.Entities;
using Basket.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository basketRepository;

    public BasketController(IBasketRepository basketRepository)
    {
        this.basketRepository = basketRepository;
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
        var basket = (await basketRepository.GetBasket(basketCheckout.UserName))
            ?? throw new Exception("asfgsdfg");

        if (basket is null)
            return BadRequest();

      

      

       
       
    
    }


}
