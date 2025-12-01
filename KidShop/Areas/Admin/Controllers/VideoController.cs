using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class VideoController : Controller
    {
        private DataContext _context;
        public VideoController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.Videos.OrderBy(v => v.VideoID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            var mnList = (from m in _context.CategoryVideos
                          select new SelectListItem()
                          {
                              Text = m.Title,
                              Value = m.Category_VideoID.ToString(),
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
        public IActionResult Create(tbl_Video ab)
        {
            if (ModelState.IsValid)
            {
                _context.Videos.Add(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Videos.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            var mnList = (from m in _context.CategoryVideos
                          select new SelectListItem()
                          {
                              Text = m.Title,
                              Value = m.Category_VideoID.ToString(),
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
        public IActionResult Edit(tbl_Video ab)
        {
            if (ModelState.IsValid)
            {
                _context.Videos.Update(ab);
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

            var ab = _context.Videos.Find(id);
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
            var ab = _context.Videos.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Videos.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
