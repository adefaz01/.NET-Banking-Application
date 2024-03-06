using BankingApplication.Data;
using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;

namespace BankingApplicationTests;

public class LoginControllerTests
{
    private readonly BankingApplicationContext _context;
    public LoginControllerTests()
    {

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<BankingApplicationContext>(options =>
            options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _context = serviceProvider.GetRequiredService<BankingApplicationContext>();

        _context.Database.EnsureCreated();

        SeedData.InitDb(serviceProvider);
    }

    [Fact]
    public void Login_ReturnsView()
    {
        // Arrange
        var controller = new LoginController(_context);

        // Act
        var result = controller.Login();

        // Assert
        Assert.IsType<ViewResult>(result);

    }


    [Theory]
    [InlineData("12345678", "abc123")]
    [InlineData("38074569", "ilovermit2020")]
    [InlineData("17963428", "youWill_n0tGuess-This!")]

    public void Login_Success(string id, string password)
    {
        var controller = new LoginController(_context);

        // mock HttpContext session;
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        // set http context
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            
        };

        var result = controller.Login(id, password) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Customer", result.ControllerName);
    }


    [Theory]
    [InlineData("12345678", "weqwe")]
    [InlineData("38074569", "weqwqeqwe")]
    [InlineData("17963428", "youWi234234ll_n0tGuess-This!")]
    public void Login_Failed(string id, string password)
    {
        var controller = new LoginController(_context);

        // mock HttpContext session;
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        // set http context
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = controller.Login(id, password);

        Assert.IsType<ViewResult>(result);
    }


    [Fact]
    public void LogoutTest()
    {
        var controller = new LoginController(_context);

        // Mock the HttpContext property
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        // set the mocked http into the controller
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = controller.Logout() as RedirectToActionResult;

        Assert.Equal("Login", result.ActionName);


    }

}

