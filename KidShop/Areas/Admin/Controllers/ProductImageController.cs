using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductImageController : Controller
    {
        private DataContext _context;
        public ProductImageController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            // Lấy dữ liệu + Include Product, nhưng không ToList() (để còn phân trang)
            var query = _context.ProductImages
                .Include(p => p.Product)
                .AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Product.ProductName.Contains(search));
            }

            // Sắp xếp
            query = query.OrderBy(c => c.ProductImageID);

            // Phân trang
            var pagedList = query.ToPagedList(page, pageSize);

            ViewBag.Search = search;

            return View(pagedList);
        }
        public IActionResult Create()
        {
            var mnList = (from m in _context.Products
                          select new SelectListItem()
                          {
                              Text = m.ProductName,
                              Value = m.ProductID.ToString(),
                          }).ToList();

            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnList = mnList;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_ProductImage ab)
        {
            if (ModelState.IsValid)
            {
                _context.ProductImages.Add(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ab = _context.ProductImages.Find(id);
            if (ab == null)
            {
                return NotFound();
            }
            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ab = _context.ProductImages.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.ProductImages.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.ProductImages.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            var mnList = (from m in _context.Products
                          select new SelectListItem()
                          {
                              Text = m.ProductName,
                              Value = m.ProductID.ToString(),
                          }).ToList();

            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnList = mnList;

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_ProductImage ab)
        {
            if (ModelState.IsValid)
            {
                _context.ProductImages.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
