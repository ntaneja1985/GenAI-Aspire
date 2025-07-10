
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
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

        internal async Task UpdateBasketItemProductPrices(int productId, decimal price)
        {
            var basket = await GetBasket("swn1");
            var item = basket!.Items.FirstOrDefault(x =>x.ProductId==productId.ToString());
            if (item != null)
            {
                item.Price = price;
                await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
            }
        }

        //internal async Task UpdateBasketItemProductPrices(int productId, decimal price)
        //{
        //    var endpoints = redis.GetEndPoints();
        //    var keys = new List<RedisKey>();

        //    foreach (var endpoint in endpoints)
        //    {
        //        var server = redis.GetServer(endpoint);
        //        var scannedKeys = server.Keys(pattern: "*", pageSize: 100); // Replace "*" with a prefix like "basket:*" if available
        //        keys.AddRange(scannedKeys);
        //    }

        //    foreach (var key in keys)
        //    {
        //        var json = await cache.GetStringAsync(key);
        //        if (string.IsNullOrWhiteSpace(json)) continue;

        //        var basket = JsonSerializer.Deserialize<ShoppingCart>(json);
        //        if (basket == null) continue;

        //        basket.Items[productId].Price = price;

        //        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket),
        //            new DistributedCacheEntryOptions
        //            {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
        //            });
        //    }
        //}
    }
}
