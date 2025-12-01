using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "Video")]
    public class VideoComponent : ViewComponent
    {
        private readonly DataContext _context;
        public VideoComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfVideo = (from vi in _context.Videos
                               where (vi.IsActive == true && vi.IsFeatured == true)
                               select vi).Take(6).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfVideo));
        }
    }
}
