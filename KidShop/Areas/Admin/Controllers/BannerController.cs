using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BannerController : Controller
    {
        private readonly DataContext _context;
        public BannerController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.Banners.OrderBy(g => g.BannerID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_Banner ab)
        {
            if (ModelState.IsValid)
            {
                _context.Banners.Add(ab);
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

            var ab = _context.Banners.Find(id);
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
            var ab = _context.Banners.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Banners.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Banners.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_Banner ab)
        {
            if (ModelState.IsValid)
            {
                _context.Banners.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
