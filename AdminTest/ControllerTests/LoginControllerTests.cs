using Admin.Data;
using Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AdminTest.ControllerTests;

public  class LoginControllerTests
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
    public void Index_ReturnsView()
    {
        // Arrange
        var controller = new LoginController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);

    }

    [Fact]
    public void Login_Success()
    {
        var controller = new LoginController();

        // Mock the HttpContext property
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;
         
        // set the mocked http into the controller
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = controller.Login("admin", "password") as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Admin", result.ControllerName);
    }

    
    [Theory]
    [InlineData("admin", "qwerty")]
    [InlineData("12345678", "password")]
    [InlineData("asdasd", "adasdasd")]
    public void Login_Failed(string username, string password)
    {


        // Arrange
        // Mock the HttpContext property
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        tempData["ErrorMessage"] = "Login failed please try again";

        // Create controller and pass in mocked temp data
        var controller = new LoginController()
        {
            TempData = tempData,
        };


        // set the mocked http into the controller
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            
        };

        // Act
        var result = controller.Login(username, password) as RedirectToActionResult;

        Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public void LogoutTest()
    {
        var controller = new LoginController();

        // Mock the HttpContext property
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        // set the mocked http into the controller
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = controller.Logout() as RedirectToActionResult;

        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Login", result.ControllerName);

    }
}


