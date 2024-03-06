using BankingApplication.Data;
using BankingApplication.Wrapper;
using BankingApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using SimpleHashing.Net;

namespace BankingApplicationTests;

public class CustomerControllerTests
{
    private readonly BankingApplicationContext _context;
    private readonly ISessionWrapper _session;

    public CustomerControllerTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<BankingApplicationContext>(options =>
            options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _context = serviceProvider.GetRequiredService<BankingApplicationContext>();

        _context.Database.EnsureCreated();

        SeedData.InitDb(serviceProvider);

        // mock customer session
        var sessionMock = new Mock<ISessionWrapper>();
        sessionMock.Setup(s => s.GetInt32(It.IsAny<string>())).Returns(2100);

        _session = sessionMock.Object;

    }


    [Fact]
    public void Index_ReturnsCustomerFromHttpSession()
    {
        // Arrange
        var controller = new CustomerController(_context,_session);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Profile_ReturnCustomerView()
    {

        // Arrange
        var controller = new CustomerController(_context, _session);

        // Act
        var result = controller.Profile();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Theory]
    [InlineData(2100,"Adam De Fazio")]
    [InlineData(2100,"Derek Tek")]

    public void Profile_UpdateCustomerName(int id, string name)
    {
        var controller = new CustomerController(_context, _session);

        var customer = _context.Customers.Find(id);
        customer.Name = name;

        var result = controller.Profile(customer);

        Assert.Equal(name, customer.Name);
    }


    [Fact]
    public void Password_Test()
    {
        // Arrange
        var controller = new CustomerController(_context, _session);

        // Act
        var result = controller.Password();

        // Assert
        Assert.IsType<ViewResult>(result);
    }


    [Theory]
    [InlineData("newpassword")]
    [InlineData("qwerty")]
    public void Change_Password(string password)
    {
        var simpleHash = new SimpleHash();

        var controller = new CustomerController(_context, _session);

        var result = controller.Password(password);

        var newpassword = _context.Logins.Find("12345678").PasswordHash;

        Assert.True(simpleHash.Verify(password, newpassword));
    }

}
