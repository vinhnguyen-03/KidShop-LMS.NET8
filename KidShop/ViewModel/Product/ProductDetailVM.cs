using KidShop.Models;

namespace KidShop.ViewModel.Product
{
    public class ProductDetailVM
    {
        public tbl_Product Product { get; set; }
       
        public List<tbl_ProductImage> Images { get; set; } = new List<tbl_ProductImage>();
        public List<CommentWithUserVM> Comments { get; set; } = new List<CommentWithUserVM>();
    }
   
}
