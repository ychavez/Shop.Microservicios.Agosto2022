using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository repository;

        public CatalogController(IProductRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product) 
        {
            await repository.CreateProduct(product);
            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Getproducts()
            => Ok(await repository.GetProducts());

        
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await repository.GetProduct(id);
           
            if (product is null) return NotFound();

            return product;
        }


    }
}
