using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoryVideoController : Controller
    {
        private readonly DataContext _context;
        public CategoryVideoController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.CategoryVideos.OrderBy(g => g.Category_VideoID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_CategoryVideo ab)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryVideos.Add(ab);
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

            var ab = _context.CategoryVideos.Find(id);
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
            var ab = _context.CategoryVideos.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.CategoryVideos.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.CategoryVideos.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_CategoryVideo ab)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryVideos.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
