using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using X.PagedList; // thư viện phân trang
using X.PagedList.Extensions;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductController : Controller
    {
        private DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Products.AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Sắp xếp theo ProductID
            query = query.OrderBy(p => p.ProductID);

            // Phân trang
            var pagedList = query.ToPagedList(page, pageSize);

            ViewBag.Search = search;  

            return View(pagedList);
        }
        public IActionResult Create(string? search)
        {
            var mnList = (from m in _context.CategorieProducts
                          select new SelectListItem()
                          {
                              Text = m.CategoryName,
                              Value = m.Category_ProductID.ToString(),
                          }).ToList();

            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnList = mnList;
            ViewBag.Search = search;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_Product ab, string? search)
        {
            // Kiểm tra tên sản phẩm đã tồn tại chưa (không phân biệt hoa thường)
            bool isDuplicate = _context.Products
                .Any(p => p.ProductName.ToLower().Trim() == ab.ProductName.ToLower().Trim());

            if (isDuplicate)
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại, vui lòng nhập tên khác.");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(ab);
                _context.SaveChanges();
                // Redirect giữ search
                return RedirectToAction("Index", new { search });
            }

            // Nếu lỗi (ví dụ trùng tên) → nạp lại danh sách loại sản phẩm để form không bị rỗng
            var mnList = (from m in _context.CategorieProducts
                          select new SelectListItem()
                          {
                              Text = m.CategoryName,
                              Value = m.Category_ProductID.ToString(),
                          }).ToList();

            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });

            ViewBag.mnList = mnList;

            // Trả lại view kèm lỗi hiển thị
            return View(ab);
        }
   
        public IActionResult Edit(int? id, string? search)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Products.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            var mnList = (from m in _context.CategorieProducts
                          select new SelectListItem()
                          {
                              Text = m.CategoryName,
                              Value = m.Category_ProductID.ToString(),
                          }).ToList();

            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnList = mnList;
            ViewBag.Search = search;
            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_Product ab, string? search)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(ab);
                _context.SaveChanges();
                // Redirect giữ search
                return RedirectToAction("Index", new { search });
            }
            return View(ab);
        }
        public IActionResult Delete(int? id, string? search)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ab = _context.Products.Find(id);
            if (ab == null)
            {
                return NotFound();
            }
            ViewBag.Search = search;
            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string? search)
        {
            var ab = _context.Products.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Products.Remove(ab);
            _context.SaveChanges();
            // Redirect giữ search
            return RedirectToAction("Index", new { search });
        }
    }
}
