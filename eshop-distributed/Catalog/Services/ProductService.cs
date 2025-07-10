using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services
{
    public class ProductService(ProductDbContext dbContext, IBus bus)
    {

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }
        public async Task CreateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
        {
            // if price has changed, raise ProductPriceChanged Integration Event
            if (updatedProduct.Price != inputProduct.Price)
            {
                //Publish product price changed integration event for update basket prices
                var integrationEvent = new ProductPriceChangedIntegrationEvent
                {
                    ProductId = updatedProduct.Id,
                    Name = inputProduct.Name,
                    Description = inputProduct.Description,
                    Price = inputProduct.Price, //set updated price
                    ImageUrl = inputProduct.ImageUrl
                };

                await bus.Publish(integrationEvent);

            }

            //update product with new values
            updatedProduct.Name = inputProduct.Name;
            updatedProduct.Description = inputProduct.Description;
            updatedProduct.Price = inputProduct.Price;
            updatedProduct.ImageUrl = inputProduct.ImageUrl;

            dbContext.Products.Update(updatedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return;
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }

}
