using BankingApplication.CustomAttribute;
using BankingApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApplication.Models;
using BankingApplication.Wrapper;


namespace BankingApplication.Controllers;

[CustomerAuthentication]
public class TransferController : Controller
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    private int CustomerID => _session.GetInt32(nameof(Customer.CustomerID));

    public TransferController(BankingApplicationContext context, ISessionWrapper session)
    {
        _context = context;
        _session = session;
    }

    // Action to display list of accounts for customer
    public IActionResult Index()
    {
        var customer = _context.Customers.Include(x => x.Accounts).
            FirstOrDefault(x => x.CustomerID == CustomerID);

        return View(customer);
    }

    // Action to display transfer transaction page
    public IActionResult Transfer(int id) => View(_context.Accounts.Find(id));

    // Action to handle transfer transaction
    [HttpPost]
    public IActionResult Transfer(int id,decimal amount,int destination, string comment)
    {
        // get all accounts associated with the transaction
        var account = _context.Accounts.Find(id);
        var allAccounts = _context.Accounts.ToList();
        // set balance
        decimal balance = account.Balance;

        // Check if amount is valid, round to 2 decimals if true
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if (decimal.Round(amount, 2) != amount)
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        // check free transactions
        if (!FreeTransactions(account))
            balance -= 0.10m;

        // Validate balance in account
        if (account.AccountType == "S" && balance - amount < 0)
            ModelState.AddModelError(nameof(amount), "Cannot transfer more than available balance.");
        else if (account.AccountType == "C" && balance - amount < 300)
            ModelState.AddModelError(nameof(amount), "Cannot transfer more than available balance. (Checking account min balance : $300)");

        if (destination == account.AccountNumber || !(allAccounts.Any(x => x.AccountNumber == destination)))
            ModelState.AddModelError(nameof(destination), "Not a valid destination account.");

        // check string length
        if (comment != null)
        {
            if (comment.Length > 30)
                ModelState.AddModelError(nameof(comment), "Comment length Exceeded 30 characters ");
        }

        // If not valid, return to index
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }
        
        // create new transaction object
        var transaction = new Transaction
        {
            TransactionType = "T",
            AccountNumber = account.AccountNumber,
            DestinationAccountNumber = destination,
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