namespace ECommerce.Web.Models
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class AddToCartViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
