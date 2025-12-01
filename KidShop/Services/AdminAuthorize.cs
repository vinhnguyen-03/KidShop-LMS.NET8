using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KidShop.Services
{
    public class AdminAuthorize : ActionFilterAttribute
    { 
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var session = context.HttpContext.Session.GetString("AdminID");
                if (string.IsNullOrEmpty(session))
                {
                    // Nếu chưa login, redirect về trang Login
                    context.Result = new RedirectToRouteResult(
                        new Microsoft.AspNetCore.Routing.RouteValueDictionary(
                            new { area = "Admin", controller = "AdminAuth", action = "Login" }
                        )
                    );
                }
            }
         
    }
}
