using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using BankingApplication.Models;
using BankingApplication.ViewModel;


namespace BankingApplicationTests;

public class BillPayControllerTests : TestBase
{
    private readonly int customerID = 4100;

    public BillPayControllerTests()
    {
        InitializeContextAndSession(customerID);

        var payee = new Payee
        {
            Name = "Adam De Fazio",
            Address = "1 Fake Street",
            City = "Melbourne",
            State = "Vic",
            Postcode = "3000",
            Phone = "(04) 9783 4534"
        };

        _context.Add(payee);
        _context.SaveChanges();
    }


    [Fact]
    public void Index_BillPayTest()
    {
        // Arrange
        var controller = new BillPayController(_context, _session);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }



    [Fact]
    public void BillPay_ViewModelSuccess()
    {
        var controller = new BillPayController(_context, _session);

        // Act
        var result = controller.BillPay() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Model);
        Assert.IsType<BillPayViewModel>(result.Model);
    }


    [Theory]
    [InlineData(4100, 10.30, "2024-01-28", "O")]
    [InlineData(4101, 20, "2024-02-15", "M")]
    [InlineData(4300, 65.78, "2024-03-10", "O")]
    public void BillPay_Success(int accountNumber, int amount, DateTime date, string period)
    {
        var billpaycount = _context.BillPays.Count();

        var controller = new BillPayController(_context, _session);


        var billPay = new BillPay
        {
            AccountNumber = accountNumber,
            PayeeID = _context.Payees.First().PayeeID,
            Amount = amount,
            ScheduleTimeUtc = date,
            Period = period
        };

        var result = controller.BillPay(billPay) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal(billpaycount + 1, _context.BillPays.Count());

    }

    [Theory]
    [InlineData(4100, 10.306, "2024-01-28", "O")]
    [InlineData(4101, 20.039403, "2024-02-15", "M")]
    [InlineData(4300, 65.78, "2024-03-10", "sfdsf")]
    public void BillPay_Failure(int accountNumber, decimal amount, DateTime date, string period)
    {
        var billpaycount = _context.BillPays.Count();

        var controller = new BillPayController(_context, _session);


        var billPay = new BillPay
        {
            AccountNumber = accountNumber,
            PayeeID = _context.Payees.First().PayeeID,
            Amount = amount,
            ScheduleTimeUtc = date,
            Period = period
        };

        var result = controller.BillPay(billPay) as ViewResult;

        Assert.False(controller.ModelState.IsValid);

    }

    [Fact]
    public void CancelBillPay_ReturnIndex()
    {
        var billPay = new BillPay
        {
            AccountNumber = 4100,
            PayeeID = _context.Payees.First().PayeeID,
            Amount = 30m,
            ScheduleTimeUtc = DateTime.UtcNow,
            Period = "O"
        };

        _context.Add(billPay);
        _context.SaveChanges();


        var controller = new BillPayController(_context, _session);

        // Act
        var result = controller.CancelBillPay(_context.BillPays.First().BillPayID);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(0, _context.BillPays.Count());
    }


    [Fact]
    public void PayeeReturnsViewTest()
    {
        var controller = new BillPayController(_context, _session);

        var result = controller.Payee();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Theory]
    [InlineData("Test Name", "123 Test Address", "Test City", "Test State", "12345", "1234567890")]
    public void AddPayeeTestSuccess(string name, string address, string city, string state, string postcode, string phone)
    {
        var payee = new Payee
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Phone = phone
        };

        var controller = new BillPayController(_context, _session);

        var result = controller.Payee(payee) as RedirectToActionResult;
        Assert.Equal("Index", result.ActionName);
        Assert.Equal(2, _context.Payees.Count());

    }

}

