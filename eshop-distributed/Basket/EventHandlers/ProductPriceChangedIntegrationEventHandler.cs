using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.EventHandlers
{
    public class ProductPriceChangedIntegrationEventHandler(BasketService basketService) : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            // find products on basket and update price
            await basketService.UpdateBasketItemProductPrices(context.Message.ProductId, context.Message.Price);
        }
    }
}
