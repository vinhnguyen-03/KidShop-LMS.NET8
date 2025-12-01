namespace KidShop.ViewModel.Product
{
    public class ProductStatisticsVM
    {
        // Tổng quan tồn kho
        public int TotalProducts { get; set; }
        public int OutOfStock { get; set; }
        public int LowStock { get; set; }

        // Danh sách hết hàng + sắp hết
        public List<ProductItem> OutOfStockProducts { get; set; }
        public List<ProductItem> LowStockProducts { get; set; }

        // Top bán chạy trong 1 tháng
        public List<TopProductItem> Top5Products { get; set; }

        // Bán chậm trong 3 tháng
        public List<SlowProductItem> SlowProducts { get; set; }
    }

    public class TopProductItem
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int TotalSold { get; set; }
    }

    public class SlowProductItem
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int? Stock { get; set; }
    }
    public class ProductItem
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
    }
}
