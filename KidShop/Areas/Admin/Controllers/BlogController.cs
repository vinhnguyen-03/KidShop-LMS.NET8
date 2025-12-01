using KidShop.Models;
using KidShop.Services;
using KidShop.ViewModel.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions; // thư viện phân trang

namespace KidShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BlogController : Controller
    {
        private DataContext _context;
        public BlogController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Blogs.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search));
            }
            query = query.OrderBy(b => b.BlogID);
            // Phân trang
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
        public IActionResult Create(tbl_Blog ab)
        {
            if (ModelState.IsValid)
            {
                _context.Blogs.Add(ab);
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
            var ab = _context.Blogs.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tbl_Blog ab)
        {
            if (ModelState.IsValid)
            {
                _context.Blogs.Update(ab);
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

            var ab = _context.Blogs.Find(id);
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
            var ab = _context.Blogs.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Pending()
        {
            var blogs = _context.Blogs.Include(b => b.User)
                .Where(b => b.IsActive == false)
                .OrderByDescending(b => b.CreatedDate)
                .ToList();
            return View(blogs);
        }
        public IActionResult Approve(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null) return NotFound();

            blog.IsActive = true; // duyệt
         

            _context.SaveChanges();
            TempData["Success"] = "Bài viết đã được duyệt!";
            return RedirectToAction("Pending");
        }
        public IActionResult Reject(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null) return NotFound();

            _context.Blogs.Remove(blog); // hoặc đánh dấu từ chối
            _context.SaveChanges();

            TempData["Success"] = "Bài viết đã bị từ chối!";
            return RedirectToAction("Pending");
        }
        public IActionResult Statistics()
        {
            // Tổng số bài viết
            int totalBlogs = _context.Blogs.Count();

            // Bài viết có lượt xem nhiều nhất
            var mostViewedBlog = _context.Blogs
                .OrderByDescending(b => b.ViewCount)
                .FirstOrDefault();

            // Người đăng nhiều bài nhất (Role = "User")
            var topUser = (from b in _context.Blogs
                           join u in _context.Users on b.UserID equals u.UserID
                           where u.Role == "User" // chỉ tính user bình thường
                           group b by new { b.UserID, u.FullName } into g
                           orderby g.Count() descending
                           select new
                           {
                               UserID = g.Key.UserID,
                               FullName = g.Key.FullName,
                               BlogCount = g.Count()
                           })
                           .FirstOrDefault();

            string topUserName = "Không có";
            int topUserBlogCount = 0;

            if (topUser != null)
            {
                topUserName = topUser.FullName ?? "Không có";
                topUserBlogCount = topUser.BlogCount;
            }

            var vm = new BlogStatsVM
            {
                TotalBlogs = totalBlogs,
                MostViewedBlog = mostViewedBlog,
                TopUserName = topUserName,
                TopUserBlogCount = topUserBlogCount
            };

            return View(vm);
        }
    }
}
