using BankingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingApplication.CustomAttribute
{
    public class CustomerAuthentication: Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (!customerID.HasValue)
                context.Result = new RedirectToActionResult("Login", "Login", null);
        }
    }
}
