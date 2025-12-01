using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "CategoryProduct")]
    public class CategoryProductComponent : ViewComponent
    {
        private readonly DataContext _context;
        public CategoryProductComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfCat = (from c in _context.CategorieProducts
                              where (c.IsActive == true) 
                              select c).Take(6).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfCat));
        }
    }
}
