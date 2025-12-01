using KidShop.Models;

namespace KidShop.Services
{
    public class InventoryService
    {
        private readonly DataContext _context;
        public InventoryService(DataContext context)
        {
            _context = context;
        }
        // ====== NHẬP KHO  ======
        public bool ImportStock(int productId, int quantity, decimal costPrice, string reason)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return false;

            product.Quantity = (product.Quantity ?? 0) + quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductID = productId,
                QuantityChange = quantity,   // + nhập
                CostPrice = costPrice,
                Reason = reason,
                CreatedDate = DateTime.Now
            });

            _context.SaveChanges();
            return true;
        }
         
        // ====== XUẤT KHO ======
        public bool ExportStock(int productId, int quantity, string reason)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return false;

            int currentStock = product.Quantity ?? 0;
            if (currentStock < quantity)
            {
                return false; // Không đủ hàng
            }

            // Cập nhật tồn kho
            product.Quantity = currentStock - quantity;

            // Lấy giá vốn gần nhất đã nhập kho
            var lastImport = _context.InventoryTransactions
                .Where(t => t.ProductID == productId && t.QuantityChange > 0)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefault();

            decimal costPrice = lastImport?.CostPrice ?? 0;

            // Thêm giao dịch xuất kho với giá vốn
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductID = productId,
                QuantityChange = -quantity, // Xuất kho
                CostPrice = costPrice,      // Lấy từ lô nhập gần nhất
                Reason = reason,
                CreatedDate = DateTime.Now
            });

            _context.SaveChanges();
            return true;
        }
    }
}
