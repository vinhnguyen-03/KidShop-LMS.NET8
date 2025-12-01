using KidShop.Models;

namespace KidShop.ViewModel.Product
{
    public class RevenueProductVM
    {
        // ====== Tổng quan ======
        public decimal TotalRevenue { get; set; }      // Tổng doanh thu
        public decimal TotalCost { get; set; }         // Tổng giá vốn
        public decimal TotalProfit { get; set; }       // Tổng lợi nhuận

        public int TotalOrders { get; set; }           // Tổng đơn hàng
        public int TotalSoldProducts { get; set; }     // Tổng số lượng sản phẩm đã bán

        // ====== Hôm nay ======
        public decimal RevenueToday { get; set; }
        public decimal CostToday { get; set; }
        public decimal ProfitToday { get; set; }

        // ====== Tháng này ======
        public decimal RevenueThisMonth { get; set; }
        public decimal CostThisMonth { get; set; }
        public decimal ProfitThisMonth { get; set; }

        // ====== Chi tiết theo sản phẩm ======
        public List<ProductRevenueVM> ProductRevenue { get; set; } = new List<ProductRevenueVM>();

        // ====== Chi tiết theo tháng ======
        public List<MonthlyRevenueVM> MonthlyRevenue { get; set; } = new List<MonthlyRevenueVM>();

        // ====== Dashboard kho ======
        public int TotalProducts { get; set; }         // Tổng số sản phẩm
        public int TotalStock { get; set; }            // Tổng tồn kho
        public int TotalImportThisMonth { get; set; }  // Tổng nhập tháng này
        public int TotalExportThisMonth { get; set; }  // Tổng xuất tháng này
        // ====== Đơn hàng chờ xử lý ======
        public List<PendingOrderVM> PendingOrders { get; set; } = new List<PendingOrderVM>();
        public decimal TotalPendingAmount { get; set; }

    }

    // Chi tiết doanh thu theo sản phẩm
    public class ProductRevenueVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public int QuantitySold { get; set; }
    }

    // Chi tiết doanh thu theo tháng
    public class MonthlyRevenueVM
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
    }
    // don hang cho xử lsy
    public class PendingOrderVM
    {
        public int OrderID { get; set; }
        public string? ReceiverName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

}
