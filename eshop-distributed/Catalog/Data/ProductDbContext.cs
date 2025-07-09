namespace Catalog.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Product>().HasData(
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
        }
    }
}
