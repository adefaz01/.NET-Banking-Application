using System.Transactions;
using BankingApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingApplication.BackgroundServices;

public class BillPayService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BillPayService> _logger;

    public BillPayService(IServiceProvider services, ILogger<BillPayService> logger)
    {
        _services = services;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bill Pay Service is running.");

        while(!cancellationToken.IsCancellationRequested)
        {
            await DoWorkAsync(cancellationToken);

            _logger.LogInformation("Bill Pay Service is waiting a minute.");

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bill Pay Service is working.");

        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BankingApplicationContext>();

        // get list of BillPays from BillPay table
        var billpays = await context.BillPays.ToListAsync(cancellationToken);
        // iterate through each billpay
        foreach(var billpay in billpays)
        {
            // check if the currently utc time has passed the scheduled utc time
            if (DateTime.UtcNow >= billpay.ScheduleTimeUtc)
            {
                // get the account from AccountNumber
                var account = context.Accounts.FirstOrDefault(x => x.AccountNumber == billpay.AccountNumber);
                // set the new balance
                var balance = account.Balance - billpay.Amount;

                // Business rules apply for Savings and Checking account
                if ((account.AccountType == "S" && balance >= 0) || (account.AccountType == "C" && balance >= 300))
                {
                    // set the account balance to new balance
                    account.Balance = balance;
                    // add transaction
                    context.Transactions.Add(
                        new Models.Transaction
                        {
                            TransactionType = "B",
                            AccountNumber = billpay.AccountNumber,
                            Amount = billpay.Amount,
                            Comment = "BillPay Service",
                            TransactionTimeUtc = DateTime.UtcNow
                        }
                    );

                    // Remove the billpay from the list if it is a one time payment
                    if (billpay.Period == "O")
                    {
                        context.BillPays.Remove(billpay);
                    } 
                    // Change the scheduled time to one month ahead so it is processed in a months time
                    else if (billpay.Period == "M")
                    {
                        billpay.ScheduleTimeUtc = billpay.ScheduleTimeUtc.AddMonths(1);
                    }
                } 
                else 
                {
                    // Set failed payment to display to the client that it has failed
                    billpay.FailedPayment = true;
                    _logger.LogInformation("PAYMENT HAS FAILED");
                }
            }
        }
        // Save changes
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bill Pay Service work complete.");
    }
}
