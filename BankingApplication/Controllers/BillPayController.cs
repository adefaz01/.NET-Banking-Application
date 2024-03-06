using BankingApplication.CustomAttribute;
using BankingApplication.Data;
using BankingApplication.Models;
using BankingApplication.ViewModel;
using BankingApplication.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingApplication.Controllers;

[CustomerAuthentication]
public class BillPayController : Controller
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    private int CustomerID => _session.GetInt32(nameof(Customer.CustomerID));
    public BillPayController(BankingApplicationContext context, ISessionWrapper session)
    {
        _context = context;
        _session = session;
    }

    // Action to display list of billpays
    public IActionResult Index()
    {
        // Fetch and display billpays that the current customer has
        var billpays = _context.Customers
            .Where(c => c.CustomerID == CustomerID)
            .Include(c => c.Accounts)
                .ThenInclude(a => a.BillPays)
            .SelectMany(c => c.Accounts.SelectMany(a => a.BillPays))
            .OrderBy(x => x.ScheduleTimeUtc)
            .ToList();

        return View(billpays);
    }

    // Action to add billpay
    // Shows a list of accounts that the customer has to be selected
    // Shows a list of Payees to be selected
    public IActionResult BillPay()
    {
        return View(
            new BillPayViewModel
            {   
                BillPay = new BillPay(),
                accounts = _context.Accounts.Where(x => x.CustomerID == CustomerID).ToList(),
                Payees = _context.Payees.ToList()
            }); 
    }

    // Action to handle new billpay
    [HttpPost]
    public IActionResult BillPay(BillPay billPay)
    {
        // error handling for fields
        // checks if billpayid has been selected
        if (billPay.PayeeID == 0)
            ModelState.AddModelError("Payee", "Payee ID field is required");
        // Check if periiod has been inputed
        if (billPay.Period != "O" && billPay.Period != "M")
            ModelState.AddModelError("period", "Period must be O or M.");
        // Check if amount is valid
        if (billPay.Amount <= 0)
            ModelState.AddModelError("amount", "Amount must be positive.");
            // Round to 2 decimal places if it is valid
        else if (decimal.Round(billPay.Amount, 2) != billPay.Amount)
            ModelState.AddModelError("amount", "Amount cannot have more than 2 decimal places.");
        // Ensure time is selected
        if(billPay.ScheduleTimeUtc == DateTime.MinValue || billPay.ScheduleTimeUtc == DateTime.MaxValue)
            ModelState.AddModelError("Date", "Date Field Required");

        // if all fields are correct, redirect to index after adding to the db
        if (ModelState.IsValid)
        {
            // Convert time to utc time
            billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.ToUniversalTime();
            // set failed payment to false (init value)
            billPay.FailedPayment = false;
            // add billpay to table
            _context.BillPays.Add(billPay);
            // save
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        // if failed, return to the form view
        return View(
                    new BillPayViewModel
                    {
                        BillPay = billPay,
                        accounts = _context.Accounts.Where(x => x.CustomerID == CustomerID).ToList(),
                        Payees = _context.Payees.ToList()
                    });
    }

    // Action to cancel a billpay
    public IActionResult CancelBillPay(int id)
    {
        // get the billpay where the BillPayID matches id
        var billpay = _context.BillPays.FirstOrDefault(x => x.BillPayID == id);
        // remove billpay and save
        _context.BillPays.Remove(billpay);
        _context.SaveChanges();
        // redirect to index
        return RedirectToAction(nameof(Index));
    }


    // Action to add new Payee
    public IActionResult Payee() => View(new Payee());

    // Action to handle new Payee
    [HttpPost]
    public IActionResult Payee(Payee payee)
    {
        // if all details are valid
        if (ModelState.IsValid)
        {
            // add payee to db and save
             _context.Payees.Add(payee);
             _context.SaveChanges();

            // redirect to index
            return RedirectToAction(nameof(Index));
        }
        // if failed, return to form view
        return View(payee);
    }
}