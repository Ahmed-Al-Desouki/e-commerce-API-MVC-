using ECommerce.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireSessionAttribute : ActionFilterAttribute
    {
        public bool AdminOnly { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            if (!SessionHelper.IsLoggedIn(session))
            {
                var request = context.HttpContext.Request;
                var returnUrl = $"{request.Path}{request.QueryString}";
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
                return;
            }

            if (AdminOnly && !SessionHelper.IsAdmin(session))
            {
                context.Result = new RedirectToActionResult("Index", "Products", null);
            }
        }
    }
}
