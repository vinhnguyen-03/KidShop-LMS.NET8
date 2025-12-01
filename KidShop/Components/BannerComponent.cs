using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "Banner")]
    public class BannerComponent : ViewComponent
    {
        private readonly DataContext _context;
        public BannerComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var banner = (from b in _context.Banners
                             where (b.IsActive == true)
                             select b).Take(1).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", banner));
        }
    }
}
