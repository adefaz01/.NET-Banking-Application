using BankingApplication.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BankingApplication.Data;

public static class SeedData
{

    public static void InitDb(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<BankingApplicationContext>();

        // check if DB is already seeded
        if (context.Customers.Any())
            return;

        // Deserialise JSON file
        var customers = JSONDeserialise();


        // insert customers in DB 
        foreach (var customer in customers)
        {
            context.Customers.Add(
                new Customer
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    Address = customer.Address,
                    City = customer.City,
                    Postcode = customer.Postcode,
                    Islocked = false
                }
            ); ;
            // insert accounts in DB
            foreach (var account in customer.Accounts)
            {
                context.Accounts.Add(
                    new Account
                    {
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType,
                        CustomerID = account.CustomerID,
                        Balance = getInitialBalance(account)
                    }
                ); ;
                // insert transactions into DB
                foreach (var transaction in account.Transactions)
                {

                    context.Transactions.Add(
                        new Models.Transaction
                        {   
                            TransactionType = "D",
                            AccountNumber = account.AccountNumber,
                            Amount = transaction.Amount,
                            Comment = transaction.Comment,
                            TransactionTimeUtc = transaction.TransactionTimeUtc
                        }
                    );
                }
            }
            // insert login into DB
            context.Logins.Add(
                new Login
                {
                    LoginID = customer.Login.LoginID,
                    CustomerID = customer.CustomerID,
                    PasswordHash = customer.Login.PasswordHash
                }
            );

            // commit to DB 
            context.SaveChanges();

        }

        
    }

    // get the initial balance for each account 
    public static decimal getInitialBalance(Account account)
    {
        decimal balance = 0;

        foreach(var transaction in account.Transactions)
        {
            balance += transaction.Amount;
        }

        return balance;
    }




    

    public static List<Customer> JSONDeserialise()
    {
        // JSON String
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        // get connection
        using var client = new HttpClient();
        var json = client.GetStringAsync(Url).Result;

        //Deserialise JSON String
        List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt"

        });

        return customers;
    }


}