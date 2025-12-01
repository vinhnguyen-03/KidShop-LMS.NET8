using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoryProductController : Controller
    {
        private readonly DataContext _context;
        public CategoryProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.CategorieProducts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CategoryName.Contains(search));
            }

            query = query.OrderBy(c => c.Category_ProductID);

            var pagedList = query.ToPagedList(page, pageSize);

            ViewBag.Search = search;

            return View(pagedList);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_CategoryProduct ab)
        {
            if (ModelState.IsValid)
            {
                _context.CategorieProducts.Add(ab);
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

            var ab = _context.CategorieProducts.Find(id);
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
            var ab = _context.CategorieProducts.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.CategorieProducts.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.CategorieProducts.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_CategoryProduct ab)
        {
            if (ModelState.IsValid)
            {
                _context.CategorieProducts.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
