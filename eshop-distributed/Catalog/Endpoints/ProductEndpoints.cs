namespace Catalog.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/products");
            group.MapGet("/", async (ProductService productService) =>
            {
                var products = await productService.GetProductsAsync();
                return Results.Ok(products);
            })
            .WithName("GetAllProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);

            group.MapGet("/{id}", async (int id, ProductService productService) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                return product is not null ? Results.Ok(product) : Results.NotFound();
            })
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);


            group.MapPost("/", async (Product product, ProductService productService) =>
            {
                await productService.CreateProductAsync(product);
                return Results.Created($"/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .Produces<Product>(StatusCodes.Status201Created); 


            group.MapPut("/{id}", async (int id, Product inputProduct, ProductService productService) =>
            {
                if (id != inputProduct.Id)
                {
                    return Results.BadRequest();
                }
                var upatedProduct = await productService.GetProductByIdAsync(id);
                await productService.UpdateProductAsync(upatedProduct,inputProduct);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            ; 


            group.MapDelete("/{id}", async (int id, ProductService productService) =>
            {
                await productService.DeleteProductAsync(id);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);


            // Support AI
            group.MapGet("/support/${query}", async (string query, ProductAIService productAIService) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Results.BadRequest("Query cannot be empty.");
                }
                var response = await productAIService.SupportAsync(query);
                return Results.Ok(response);
            })
            .WithName("ProductSupport")
            .Produces(StatusCodes.Status200OK);


            // Traditional Search
            group.MapGet("search/{query}", async (string query, ProductService service) =>
            {
                var products = await service.SearchProductsAsync(query);

                return Results.Ok(products);
            })
            .WithName("SearchProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);

            // AI Search
            group.MapGet("aisearch/{query}", async (string query, ProductAIService service) =>
            {
                var products = await service.SearchProductsAsync(query);

                return Results.Ok(products);
            })
            .WithName("AISearchProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);
        }
    }
}
