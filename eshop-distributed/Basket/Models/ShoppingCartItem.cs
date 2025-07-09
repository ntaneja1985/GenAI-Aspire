namespace Basket.Models
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string Color { get; set; } = default!;

        //comes from the catalog module
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}
