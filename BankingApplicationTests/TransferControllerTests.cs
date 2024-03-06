using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BankingApplicationTests;

public class TransferControllerTests : TestBase

{
    private readonly int customerID = 4100;

    public TransferControllerTests()
    {
        InitializeContextAndSession(customerID);
    }


    [Fact]
    public void Index_TransferTest()
    {
        // Arrange
        var controller = new TransferController(_context, _session);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }




    [Fact]
    public void Transfer_Test()
    {

        // Arrange
        var controller = new TransferController(_context, _session);

        // Act
        var result = controller.Transfer(customerID);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Theory]
    [InlineData(4100, 4.20, "test1", 4101)] 
    [InlineData(4101, 32.56, "test2", 4200)] 
    [InlineData(4101, 312.3, null, 4300)]    
    public void TransferSuccessTest(int accountNumber, decimal amount, string comment, int destinationAccountNumber)
    {
        
        var controller = new TransferController(_context, _session);

        
        var result = controller.Transfer(accountNumber, amount, destinationAccountNumber, comment) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Confirmation", result.ControllerName);
        Assert.Equal("T", result.RouteValues["TransactionType"]);
    }


    [Theory]
    [InlineData(4100, -4.20, "test1", 4101)]
    [InlineData(4101, 5000, "test2", 4102)]
    [InlineData(4101, 32.56, "test3", 9999)]
    [InlineData(4101, 32.56, null, 4101)]
    public void TransferFailureTest(int accountNumber, decimal amount, string comment, int destinationAccountNumber)
    {
        var controller = new TransferController(_context, _session);

        var result = controller.Transfer(accountNumber, amount, destinationAccountNumber, comment) as ViewResult;

        Assert.False(controller.ModelState.IsValid); 
    }

}
