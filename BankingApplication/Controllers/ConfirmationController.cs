using BankingApplication.Data;
using BankingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using BankingApplication.CustomAttribute;
using Microsoft.EntityFrameworkCore;
using BankingApplication.Wrapper;

namespace BankingApplication.Controllers;

[CustomerAuthentication]
public class ConfirmationController : Controller
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    // Retrieve customerID from session
    private int CustomerID => _session.GetInt32(nameof(Customer.CustomerID));

    public ConfirmationController(BankingApplicationContext context, ISessionWrapper session)
    {
        _context = context;
        _session = session;
    }

    // Action to display confirmation view for a transaction
    [HttpGet]
    public IActionResult Index(Transaction transaction) => View(transaction);

    // Action to handle the confirmation of a transaction
    [HttpPost]
    public IActionResult ConfirmTransaction(Transaction transaction)
    {
        // Find the account that made the transaction
        var account = _context.Accounts.Find(transaction.AccountNumber);

        // Deposit transaction
        if (transaction.TransactionType == "D")
        {  
            // add amount to account balance and update
            account.Balance += transaction.Amount;
            transaction.TransactionTimeUtc = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            
        }
        // Withdraw transaction
        else if(transaction.TransactionType == "W")
        {
            // deduct account
            account.Balance -= transaction.Amount;

            //add transaction to DB 
            transaction.TransactionTimeUtc = DateTime.UtcNow;
            _context.Transactions.Add(transaction);

            // appy service charge fee 
            if (!FreeTransactions(account))
            {
                account.Balance -= 0.05m;
                _context.Transactions.Add(
                new Transaction
                {
                    TransactionType = "S",
                    AccountNumber = account.AccountNumber,
                    Amount = 0.05m,
                    Comment = "Withdraw Fee",
                    TransactionTimeUtc = DateTime.UtcNow
                });
            }


            _context.SaveChanges();
            
        }
        // Transfer transaction
        else if (transaction.TransactionType == "T")
        {
            // get destination account
            var destinationAccount = _context.Accounts.Find(transaction.DestinationAccountNumber);

            // remove balance from source account
            account.Balance -= transaction.Amount;
            transaction.TransactionTimeUtc = DateTime.UtcNow;
            _context.Transactions.Add(transaction);

            // add balance to destination account
            // add incoming transaction
            destinationAccount.Balance += transaction.Amount;
            _context.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    AccountNumber = transaction.DestinationAccountNumber.Value,
                    Amount = transaction.Amount,
                    Comment = transaction.Comment,
                    TransactionTimeUtc = DateTime.UtcNow
                });


            //check for service fee
            if(!FreeTransactions(account))
            {
                account.Balance -= 0.10m;
                _context.Transactions.Add(
                new Transaction
                {
                    TransactionType = "S",
                    AccountNumber = account.AccountNumber,
                    Amount = 0.10m,
                    Comment = "Transfer Fee",
                    TransactionTimeUtc = DateTime.UtcNow
                });
            }

            _context.SaveChanges();

        }

        // redirect to index method from customer controller
        return RedirectToAction("Index", "Customer");

    }

    // Check if the account has free transactions remaining
    public bool FreeTransactions(Account account)
    {
        var transactions = _context.Transactions
                .Where(x => x.AccountNumber == account.AccountNumber && (x.TransactionType == "W" || x.DestinationAccountNumber != null))
                .ToList();

        return transactions.Count <= 1;
    }
}