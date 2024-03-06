using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Admin.CustomAttribute
{
    public class AdminAuthentication: Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loggedIn = context.HttpContext.Session.GetInt32("admin");
            if (!loggedIn.HasValue)
                context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}
