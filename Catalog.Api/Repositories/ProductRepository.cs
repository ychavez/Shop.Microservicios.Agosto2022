﻿using Catalog.Api.Data;
using Catalog.Api.Entities;
using MongoDB.Driver;

namespace Catalog.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext context;

        public ProductRepository(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task CreateProduct(Product product)
            => await context.Products.InsertOneAsync(product);


        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            DeleteResult deleteResult = await context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Product> GetProduct(string id)
         => await context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetProducts()
        => await context.Products.Find(p => true).ToListAsync();

        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResults = await context.Products
                .ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);

            return updateResults.IsAcknowledged && updateResults.ModifiedCount > 0;
        }
    }
}
