using KidShop.Models;
using KidShop.Utilities;
using KidShop.ViewModel;
using KidShop.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Headers;

namespace KidShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId = null, string sortBy = "default", decimal? minPrice = null, decimal? maxPrice = null, string? brand = null, int page = 1, int pageSize = 12, string keyword = "")
        {
            var products = _context.Products
                .Where(p => p.IsActive)
                .AsQueryable();

            // --- Tìm kiếm ---
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                products = products.Where(p => p.ProductName != null && p.ProductName.ToLower().Contains(lowerKeyword));
            }

            // ---Danh muc
            if (categoryId.HasValue)
                products = products.Where(p => p.Category_ProductID == categoryId.Value);

            // --- Lọc theo giá ---
            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            // --- Lọc theo thương hiệu ---
            if (!string.IsNullOrEmpty(brand))
                products = products.Where(p => p.Brand != null && p.Brand == brand);

            // --- Sắp xếp theo tiêu chí ---
            switch (sortBy)
            {
                case "price_asc": // Giá tăng dần
                    products = products.OrderBy(p => p.PriceSale ?? p.Price);
                    break;

                case "price_desc": // Giá giảm dần
                    products = products.OrderByDescending(p => p.PriceSale ?? p.Price);
                    break;

                case "oldest": // Cũ nhất
                    products = products.OrderBy(p => p.CreatedDate);
                    break;

                default: // Mặc định mới nhất
                    products = products.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            //  tong ban ghi, Phân trang
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            // Danh sách thương hiệu để hiển thị ra View
            ViewBag.Brands = _context.Products
                .Where(p => p.Brand != null)
                .Select(p => p.Brand)
                .Distinct()
                .ToList();
            // Tính tổng sản phẩm
            ViewBag.TotalProducts = products.Count();

            ViewBag.Categories = _context.CategorieProducts.ToList();

            ViewBag.SortBy = sortBy;

            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            //giữ lại giá trị bộ lọc
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedBrand = brand;
            //
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(data);
        }
        [Route("/sanpham-{slug}-{id:int}.html", Name = "ProductDetail")]
        // GET: Product/ProductDetail/5
        public IActionResult ProductDetail(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm liên quan trong cùng danh mục, không bao gồm sản phẩm hiện tại
            var relatedProducts = _context.Products
                .Where(p => p.Category_ProductID == product.Category_ProductID && p.ProductID != product.ProductID && p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Take(4) // Giới hạn số sản phẩm liên quan hiển thị
                .ToList();
            ViewBag.RelatedProducts = relatedProducts;

            // Lấy danh sách ảnh của sản phẩm
            var images = _context.ProductImages
                .Where(ImageUrl => ImageUrl.ProductID == product.ProductID)
                .ToList();
            // Lấy comment + tên người dùng
            var comments = (from c in _context.Comments
                            join u in _context.Users on c.UserID equals u.UserID into userJoin
                            from u in userJoin.DefaultIfEmpty()
                            where c.TargetID == id && c.TargetType == "Product" && c.IsActive
                            orderby c.CreatedDate descending
                            select new CommentWithUserVM
                            {
                                CommentID = c.CommentID,
                                Contents = c.Contents,
                                CreatedDate = c.CreatedDate,
                                FullName = u != null ? u.FullName : "Ẩn danh"
                            }).ToList();

            var viewModel = new ProductDetailVM
            {
                Product = product,
                Images = images,
                Comments = comments ?? new List<CommentWithUserVM>()
            };
            return View(viewModel);
        }
        // Thêm bình luận
        [HttpPost]
        public IActionResult AddComment(int productId, string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                TempData["CommentError"] = "Nội dung bình luận không được để trống.";
                return RedirectToProductDetail(productId);
            }

            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                TempData["CommentError"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToProductDetail(productId);
            }

            int userId = int.Parse(userIdClaim.Value);

            var comment = new tbl_Comment
            {
                TargetID = productId,
                TargetType = "Product",
                UserID = userId,
                Contents = contents,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            TempData["CommentSuccess"] = "Bình luận của bạn đã được gửi!";
            return RedirectToProductDetail(productId);
        }
        private IActionResult RedirectToProductDetail(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Tạo slug đúng như khi bạn sinh URL
            string slug = Functions.ToSlug(product.ProductName ?? "no-title");

            // Redirect đúng route có tên "ProductDetail"
            return RedirectToRoute("ProductDetail", new { slug = slug, id = productId });
        }
    }
}

