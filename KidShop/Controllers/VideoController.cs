using KidShop.Models;
using KidShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Reflection.Metadata;

namespace KidShop.Controllers
{
    public class VideoController : Controller
    {
        private DataContext _context;
        public VideoController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId = null, int page = 1, int pageSize = 12, string keyword = "")
        {
            var videos = _context.Videos
               .Where(v => v.IsActive)
               .AsQueryable();
            // --- Tìm kiếm ---
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                videos = videos.Where(p => p.Title != null && p.Title.ToLower().Contains(lowerKeyword));
            }

            // ---Danh muc
            if (categoryId.HasValue)
                videos = videos.Where(p => p.Category_VideoID == categoryId.Value);

            //  tong ban ghi, Phân trang
            var totalRecords = videos.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = videos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 5. Truyền dữ liệu sang View
            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewBag.Categories = _context.CategoryVideos.ToList();
            ViewBag.SelectedCategory = categoryId;
            return View(data);
        }
        public IActionResult VideoDetail(int id)
        {
            // Lấy video chi tiết
            var video = _context.Videos
                .FirstOrDefault(v => v.VideoID == id && v.IsActive);

            if (video == null)
            {
                return NotFound();
            }

            //   Tăng lượt xem mỗi khi người dùng vào trang chi tiết
            video.ViewCount++;
            _context.SaveChanges();

            // Lấy video liên quan cùng Category_VideoID (ngoại trừ video hiện tại)
            var relatedVideos = _context.Videos
                .Where(v => v.Category_VideoID == video.Category_VideoID && v.VideoID != id && v.IsActive)
                .OrderByDescending(v => v.CreatedDate)
                .Take(7) // lấy   video liên quan
                .ToList();

            // Lấy comment + tên người dùng
            var comments = (from c in _context.Comments
                            join u in _context.Users on c.UserID equals u.UserID into userJoin
                            from u in userJoin.DefaultIfEmpty()
                            where c.TargetID == id && c.TargetType == "Video" && c.IsActive
                            orderby c.CreatedDate descending
                            select new CommentWithUserVM
                            {
                                CommentID = c.CommentID,
                                Contents = c.Contents,
                                CreatedDate = c.CreatedDate,
                                FullName = u != null ? u.FullName : "Ẩn danh"
                            }).ToList();

            var viewModel = new VideoDetailVM
            {
                Video = video,
                RelatedVideos = relatedVideos,
                Comments = comments ?? new List<CommentWithUserVM>()
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddComment(int videoId, string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                TempData["CommentError"] = "Nội dung bình luận không được để trống.";
                return RedirectToAction("VideoDetail", new { id = videoId });
            }

            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                TempData["CommentError"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToAction("VideoDetail", new { id = videoId });
            }

            int userId = int.Parse(userIdClaim.Value);

            var comment = new tbl_Comment
            {
                TargetID = videoId,
                TargetType = "Video",
                UserID = userId,
                Contents = contents,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            TempData["CommentSuccess"] = "Bình luận của bạn đã được gửi!";
            return RedirectToAction("VideoDetail", new { id = videoId });
        }
    }
}
