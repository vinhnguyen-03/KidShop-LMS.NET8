using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "FeaturedProduct")]
    public class FeaturedProductComponet : ViewComponent
    {
        private readonly DataContext _context;
        public FeaturedProductComponet(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var featuredProducts = (from p in _context.Products
                                    where (p.Is_featured == true && p.IsActive == true)
                                    orderby p.ProductID descending
                                    select p).Take(8).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", featuredProducts));
        }
    }
}
