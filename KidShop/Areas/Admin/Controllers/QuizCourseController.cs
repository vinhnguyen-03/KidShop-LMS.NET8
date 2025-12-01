using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class QuizCourseController : Controller
    {
        private DataContext _context;
        public QuizCourseController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.Quizzes.OrderBy(g => g.QuizID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            var mnList = (from m in _context.CourseVideos
                          select new SelectListItem()
                          {
                              Text = m.Title,
                              Value = m.CourseVideoID.ToString(),
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
        public IActionResult Create(tbl_Quiz ab)
        {
            if (ModelState.IsValid)
            {
                _context.Quizzes.Add(ab);
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

            var ab = _context.Quizzes.Find(id);
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
            var ab = _context.Quizzes.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Quizzes.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            var mnList = (from m in _context.CourseVideos
                          select new SelectListItem()
                          {
                              Text = m.Title,
                              Value = m.CourseVideoID.ToString(),
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
        public IActionResult Edit(tbl_Quiz ab)
        {
            if (ModelState.IsValid)
            {
                _context.Quizzes.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
