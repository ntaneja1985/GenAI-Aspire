
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Services
{
    public class BasketService(IDistributedCache cache, CatalogApiClient catalogApiClient)
    {
        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            var basket = await cache.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public async Task UpdateBasket(ShoppingCart basket)
        {
            //Before updating the shopping cart, call the Catalog Microservice's GetProductByIdAsync method
            //Get latest product information and set the Price and ProductName when adding/updating the item into the Shopping Cart

            foreach (var item in basket.Items)
            {
                var product = await catalogApiClient.GetProductByIdAsync(int.Parse(item.ProductId));
                if (product != null)
                {
                    item.Price = product.Price;
                    item.ProductName = product.Name;
                }
            }


            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                });
        }

        public async Task DeleteBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            await cache.RemoveAsync(userName);

        }
    }
}
