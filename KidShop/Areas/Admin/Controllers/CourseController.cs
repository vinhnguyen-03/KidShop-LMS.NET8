using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;
using X.PagedList;
namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CourseController : Controller
    {
        private DataContext _context;
        public CourseController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Courses.AsQueryable();
             
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CourseName.Contains(search));
            }
             
            query = query.OrderBy(c => c.CourseID);
             
            var pagedList = query.ToPagedList(page, pageSize);

            ViewBag.Search = search;  

            return View(pagedList);
        }
        public IActionResult Create()
        {
            var mnList = (from m in _context.Lecturers
                          select new SelectListItem()
                          {
                              Text = m.LecturerName,
                              Value = m.LecturerID.ToString(),
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
        public IActionResult Create(tbl_Course ab)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(ab);
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

            var ab = _context.Courses.Find(id);
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
            var ab = _context.Courses.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Courses.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            var mnList = (from m in _context.Lecturers
                          select new SelectListItem()
                          {
                              Text = m.LecturerName,
                              Value = m.LecturerID.ToString(),
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
        public IActionResult Edit(tbl_Course ab)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
