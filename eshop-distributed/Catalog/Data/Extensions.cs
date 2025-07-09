namespace Catalog.Data
{
    public static class Extensions
    {
        public static void UseMigration(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            dbContext.Database.Migrate();
            DataSeeder.Seed(dbContext);
        }
    }

    public class DataSeeder
    {
        public static void Seed(ProductDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Models.Product
                    {
                        Id = 1,
                        Name = "Product 1",
                        Description = "Description for Product 1",
                        Price = 9.99m,
                        ImageUrl = "https://example.com/product1.jpg"
                    },
                    new Models.Product
                    {
                        Id = 2,
                        Name = "Product 2",
                        Description = "Description for Product 2",
                        Price = 19.99m,
                        ImageUrl = "https://example.com/product2.jpg"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
