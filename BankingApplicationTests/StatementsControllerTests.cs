using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Xunit;
using BankingApplication.Models;

namespace BankingApplicationTests;

public class StatementsControllerTests : TestBase
{
    private readonly int customerID = 4100;

    public StatementsControllerTests()
    {
        InitializeContextAndSession(customerID);
    }

    [Fact]
    public void Index_StatementsTest()
    {
        var controller = new StatementsController(_context, _session);

        var result = controller.Index(customerID);

        Assert.IsType<ViewResult>(result);
    }


    [Fact]
    public void ViewStatementsTest()
    {
        var controller = new StatementsController(_context, _session);

        var result = controller.ViewStatements(customerID) as ViewResult;

        Assert.NotNull(result);
        Assert.NotNull(result.Model);
        Assert.IsType<PagedList<Transaction>>(result.Model); 
    }

}
