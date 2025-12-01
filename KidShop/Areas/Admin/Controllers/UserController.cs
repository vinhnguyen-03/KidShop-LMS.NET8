using KidShop.Models;
using KidShop.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using X.PagedList;  
using X.PagedList.Extensions;

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class UserController : Controller
    {
        private DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? searchEmail, int page = 1, int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            // Tìm kiếm theo Email hoặc Username
            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(u => u.Email.Contains(searchEmail) || u.Username.Contains(searchEmail));
            }

            // Sắp xếp theo UserID
            query = query.OrderBy(u => u.UserID);

            // Phân trang
            var pagedList = query.ToPagedList(page, pageSize);

            ViewBag.SearchEmail = searchEmail; // giữ giá trị tìm kiếm trong ô input

            return View(pagedList);
        }
         
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tbl_User ab)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(ab);
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

            var ab = _context.Users.Find(id);
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
            var ab = _context.Users.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Users.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Users.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_User ab)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
