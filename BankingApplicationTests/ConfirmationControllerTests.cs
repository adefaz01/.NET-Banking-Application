using BankingApplication.Controllers;
using BankingApplication.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BankingApplicationTests;

public class ConfirmationControllerTests : TestBase
{
    private readonly int customerID = 4100;

    public ConfirmationControllerTests()
    {
        InitializeContextAndSession(customerID);
    }

    [Fact]
    public void Index_ConfirmationTest()
    {
        var transaction = new Transaction
        {
            TransactionType = "D",
            AccountNumber = 4100,
            Amount = 30,
            Comment = "test"
        };

        // Arrange
        var controller = new ConfirmationController(_context, _session);

        // Act
        var result = controller.Index(transaction);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Theory]
    [InlineData("D", 4100, 100.23, "Deposit")] 
    [InlineData("W", 4101, 50, "Withdrawal")] 
    public void ConfirmTransactionTestSuccess(string TransactionType, int accountNumber, decimal amount, string comment)
    {
        var transaction = new Transaction
        {
            TransactionType = TransactionType,
            AccountNumber = accountNumber,
            Amount = amount,
            Comment = comment
        };

        var controller = new ConfirmationController(_context, _session);

        var result = controller.ConfirmTransaction(transaction) as RedirectToActionResult;


        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Customer", result.ControllerName);


    }
}
