using BankingApplication.CustomAttribute;
using BankingApplication.Data;
using BankingApplication.Models;
using BankingApplication.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingApplication.Controllers;

[CustomerAuthentication]
public class WithdrawController : Controller
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    private int CustomerID => _session.GetInt32(nameof(Customer.CustomerID));


    public WithdrawController(BankingApplicationContext context, ISessionWrapper session)
    {
        _context = context;
        _session = session;
    }

    // Action to display the accounts that the customer has
    public IActionResult Index()
    {
        var customer = _context.Customers.Include(x => x.Accounts).
            FirstOrDefault(x => x.CustomerID == CustomerID);

        return View(customer);
    }

    // Action to display the withdraw page
    public IActionResult Withdraw(int id) => View(_context.Accounts.Find(id));

    // Action to handle new withdraw transaction
    [HttpPost]
    public IActionResult Withdraw(int id, decimal amount, string comment)
    {
        // get the account and balance
        var account = _context.Accounts.Find(id);
        var balance = account.Balance;

        Console.WriteLine(amount);
        // check if amount is valid
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        // check free transactions
        if (!FreeTransactions(account))
            balance -= 0.05m;

        // Validate balance in account
        if (account.AccountType == "S" && balance - amount < 0)
            ModelState.AddModelError(nameof(amount), "Cannot transfer more than available balance.");
        else if (account.AccountType == "C" && balance - amount < 300)
            ModelState.AddModelError(nameof(amount), "Cannot transfer more than available balance. (Checking account min balance : $300)");

        // Check if comments follow business rules
        if (comment != null)
        {
            if (comment.Length > 30)
                ModelState.AddModelError(nameof(comment), "Comment length Exceeded 30 characters ");
        }

        // Return to index if not valid
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }
        
        // Create new transaction object if valid
        var transaction = new Transaction
        {
            TransactionType = "W",
            AccountNumber = account.AccountNumber,
            Amount = amount,
            Comment = comment,
        };

        // redirect to confirmation page
        return RedirectToAction("Index", "Confirmation", transaction);

    }

    // Check for free transactions
    public bool FreeTransactions(Account account)
    {
        var transactions = _context.Transactions
                .Where(x => x.AccountNumber == account.AccountNumber && (x.TransactionType == "W" || x.DestinationAccountNumber != null))
                .ToList();

        return transactions.Count <= 1;
    }
}
