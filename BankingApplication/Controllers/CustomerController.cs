
using BankingApplication.Data;
using BankingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApplication.CustomAttribute;
using System;
using X.PagedList;
using SimpleHashing.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using BankingApplication.ViewModel;
using BankingApplication.Wrapper;

namespace BankingApplication.Controllers;

[CustomerAuthentication]
public class CustomerController : Controller
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    // get customerID from session
    private int CustomerID => _session.GetInt32(nameof(Customer.CustomerID));


    public CustomerController(BankingApplicationContext context, ISessionWrapper session)
    {
        _context = context;
        _session = session;

    }

    // Action to display the customer accounts
    public IActionResult Index()
    {
        var customer = _context.Customers.Include(x => x.Accounts).
            FirstOrDefault(x => x.CustomerID == CustomerID);

        return View(customer);
    }

    // Action to display customer profile
    public IActionResult Profile() => View(_context.Customers.Find(CustomerID));
    
    // Action to handle customer profile updates
    [HttpPost]
    public IActionResult Profile(Customer updateCustomer)
    {   
        // if all details are valid
        if (ModelState.IsValid)
        {
            var existingCustomer = _context.Customers.Find(updateCustomer.CustomerID);

            // update customer details
            existingCustomer.Name = updateCustomer.Name;
            existingCustomer.TFN = updateCustomer.TFN;
            existingCustomer.Address = updateCustomer.Address;
            existingCustomer.City = updateCustomer.City;
            existingCustomer.State = updateCustomer.State?.ToUpper();
            existingCustomer.Postcode = updateCustomer.Postcode;
            existingCustomer.Mobile = updateCustomer.Mobile;

            // change DB
            _context.SaveChanges();

            // return to home
            return RedirectToAction(nameof(Index));
        }
        else
            return View("Profile", _context.Customers.Find(CustomerID));

    }

    ISimpleHash simpleHash = new SimpleHash();

    // Action to display new password page
    public IActionResult Password() => View("Password");


    // Action to handle password change
    [HttpPost]
    public IActionResult Password(string Password)
    {
        // validation to ensure password is not empty
        if(string.IsNullOrEmpty(Password))
        {
            ModelState.AddModelError(nameof(Password), "Password field is required");
            return View("Password");
        }

         // hash password
        var simpleHash = new SimpleHash();
        string hashedPassword = simpleHash.Compute(Password);

        // get login table from DB based on Customer ID 
        var login = _context.Logins.FirstOrDefault(x => x.CustomerID == CustomerID);

        // update password hash in DB 
        login.PasswordHash = hashedPassword;

        _context.SaveChanges();

        // return to home
        return RedirectToAction(nameof(Index));

    }

}