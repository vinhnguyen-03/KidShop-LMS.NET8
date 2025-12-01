using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class LecturerController : Controller
    {
        private DataContext _context;
        public LecturerController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.Lecturers.OrderBy(g => g.LecturerID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_Lecturer ab)
        {
            if (ModelState.IsValid)
            {
                _context.Lecturers.Add(ab);
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

            var ab = _context.Lecturers.Find(id);
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
            var ab = _context.Lecturers.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Lecturers.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Lecturers.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_Lecturer ab)
        {
            if (ModelState.IsValid)
            {
                _context.Lecturers.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
