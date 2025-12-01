using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "RecentBlog")]
    public class RecentBlogComponent : ViewComponent
    {
        private readonly DataContext _context;
        public RecentBlogComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listofBlog = (from b in _context.Blogs
                              where b.IsActive == true
                              orderby b.CreatedDate descending // sắp xếp theo ngày đăng
                              select b)
                  .Take(3)
                  .ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listofBlog));
        }
    }
}
