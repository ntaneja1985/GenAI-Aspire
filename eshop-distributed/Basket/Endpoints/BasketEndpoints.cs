namespace Basket.Endpoints
{
    public static class BasketEndpoints
    {
        public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("basket");

            group.MapGet("/{userName}", async (string userName, BasketService basketService) =>
            {
                var basket = await basketService.GetBasket(userName);
                return basket is not null ? Results.Ok(basket) : Results.NotFound();
            })
            .WithName("GetBasket")
            .Produces<ShoppingCart>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(); // Ensure the user is authenticated

            group.MapPost("/", async (ShoppingCart basket, BasketService basketService) =>
            {
                await basketService.UpdateBasket(basket);
                return Results.Created("GetBasket", basket);
            })
            .WithName("UpdateBasket")
            .Produces<ShoppingCart>(StatusCodes.Status201Created)
            .RequireAuthorization(); // Ensure the user is authenticated


            group.MapDelete("/{userName}", async (string userName, BasketService basketService) =>
            {
                await basketService.DeleteBasket(userName);
                return Results.NoContent();
            })
            .WithName("DeleteBasket")
            .Produces<ShoppingCart>(StatusCodes.Status204NoContent)
            .RequireAuthorization(); // Ensure the user is authenticated
        }
    }
}
