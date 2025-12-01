using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidShop.Models
{
    [Table("InventoryTransaction")]
    public class InventoryTransaction
    {
        [Key]
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public tbl_Product? Product { get; set; }
        // + nhập, - xuất
        public int QuantityChange { get; set; }
        public decimal? CostPrice { get; set; }
        [StringLength(200)]
        public string? Reason { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
