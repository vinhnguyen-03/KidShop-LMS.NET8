using KidShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Components
{
    [ViewComponent(Name = "Introduce")]
    public class IntroduceComponent : ViewComponent
    {
        private readonly DataContext _context;
        public IntroduceComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfIntro = (from i in _context.Introduces
                             where (i.IsActive == true)
                             select i).Take(3).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfIntro));
        }
    }
}
