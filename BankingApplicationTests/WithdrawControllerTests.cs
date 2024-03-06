﻿using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BankingApplicationTests;

public class WithdrawControllerTests : TestBase
{
    private readonly int customerID = 4100;


    public WithdrawControllerTests()
    {
        InitializeContextAndSession(customerID);
    }


    [Fact]
    public void Index_WithdrawTest()
    {
        // Arrange
        var controller = new WithdrawController(_context, _session);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }




    [Fact]
    public void Withdraw_Test()
    {

        // Arrange
        var controller = new WithdrawController(_context, _session);

        // Act
        var result = controller.Withdraw(customerID);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Theory]
    [InlineData(4100, 4.20, "test1")]
    [InlineData(4101, 32.56, "test2")]
    [InlineData(4101, 312.3, null)]
    public void WithdrawSuccessTest(int accountNumber, decimal amount, string comment)
    {

        var controller = new WithdrawController(_context, _session);

        var result = controller.Withdraw(accountNumber, amount, comment) as RedirectToActionResult;


        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Confirmation", result.ControllerName);
        Assert.Equal("W", result.RouteValues["TransactionType"]);
    }


    [Theory]
    [InlineData(4100, -4.20, "test1")]
    [InlineData(4101, 32.567, "test2")]
    [InlineData(4101, 1000, "ladsldasdasdsadsadsak;dsak;ldsal;ksdak;ldsalk;asd30 characters")]
    public void WithdrawtFailureTest(int accountNumber, decimal amount, string comment)
    {
        var controller = new DepositController(_context, _session);

        var result = controller.Deposit(accountNumber, amount, comment) as ViewResult;

        Assert.NotNull(result);
        Assert.False(controller.ModelState.IsValid);

    }
}
