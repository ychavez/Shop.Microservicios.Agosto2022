using Catalog.Api.Entities;
using MongoDB.Driver;

namespace Catalog.Api.Data;

public class CatalogContext : ICatalogContext
{
    private readonly IConfiguration _configuration;

    public CatalogContext(IConfiguration configuration)
    {
        _configuration = configuration;
       
        var client = new MongoClient(
            _configuration
                .GetValue<string>("DatabaseSettings:ConnectionString"));

        var database = client.GetDatabase(_configuration
            .GetValue<string>("DatabaseSettings:DatabaseName"));

        Products = database.GetCollection<Product>(_configuration
            .GetValue<string>("DatabaseSettings:CollectionName"));
    }

    public IMongoCollection<Product> Products { get; set; }
}

