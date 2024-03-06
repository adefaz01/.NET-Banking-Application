
using BankingApplication.Data;
using BankingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;

namespace BankingApplication.Controllers;

[Route("/Banking/Login")]
public class LoginController : Controller
{
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();
    private readonly BankingApplicationContext _context;

    public LoginController(BankingApplicationContext context)
    {
        _context = context;
    }

    // Action to display login page
    public IActionResult Login() => View();

    // Action to handle login
    [HttpPost]
    public IActionResult Login(string loginID, string password)
    {
        // get customer from loginID
        var login = _context.Logins.Include(x => x.Customer).FirstOrDefault(y => y.LoginID == loginID);
        var customer = _context.Customers.Find(login.CustomerID);

        // Check if the login details are valid
        if(login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        {
            // display login page again and show error message
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }

        // Check if the account is locked
        if(customer.Islocked)
        {
            // display login page again and show error message
            ModelState.AddModelError("LoginFailed", "Account locked by Admin");
            return View(new Login { LoginID = loginID });
        }
  

        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }

    // Action to handle logout
    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // clear session
        HttpContext.Session.Clear();
        // redirect to login view
        return RedirectToAction("Login");
    }
}