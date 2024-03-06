using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class LoginController : Controller
    {   
        // Main page for admin which is the login page
        public IActionResult Index()
        {
            if (TempData != null && TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View();
        }

        // Login validation
        public IActionResult Login(string Login, string Password)
        {
            if(Login == "admin" && Password == "password")
            {
                HttpContext.Session.SetInt32("admin", 1);

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["ErrorMessage"] = "Login failed please try again";
                return RedirectToAction("Index");

            }
        }

        // logout returns to home page which is login screen
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}
