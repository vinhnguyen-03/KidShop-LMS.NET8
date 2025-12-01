using KidShop.Models;

namespace KidShop.ViewModel.Product
{
    public class CheckoutVM
    {
        public string? ReceiverName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public string? PaymentMethod { get; set; }
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
        public decimal TotalAmount { get; set; }

    }

    public class OrderItemVM
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}
