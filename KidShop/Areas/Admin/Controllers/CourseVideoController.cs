using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CourseVideoController : Controller
    {
        private DataContext _context;
        public CourseVideoController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.CourseVideos.OrderBy(m => m.CourseVideoID).ToList();
            return View(mnList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var mn = _context.CourseVideos.Find(id);
            if (mn == null)
            {
                return NotFound();
            }

            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleMenu = _context.CourseVideos.Find(id);
            if (deleMenu == null)
            {
                return NotFound();
            }

            _context.CourseVideos.Remove(deleMenu);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            var mnList = (from m in _context.Courses
                          select new SelectListItem()
                          {
                              Text = m.CourseName,
                              Value = m.CourseID.ToString(),
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
        public IActionResult Create(tbl_CourseVideo mn)
        {
            if (ModelState.IsValid)
            {
                _context.CourseVideos.Add(mn);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mn);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _context.CourseVideos.Find(id);
            if (mn == null)
            {
                return NotFound();
            }
            var mnList = (from m in _context.Courses
                          select new SelectListItem()
                          {
                              Text = m.CourseName,
                              Value = m.CourseID.ToString(),
                          }).ToList();
            mnList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnList = mnList;
            return View(mn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_CourseVideo mn)
        {
            if (ModelState.IsValid)
            {
                _context.CourseVideos.Update(mn);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mn);
        }
    }
}
