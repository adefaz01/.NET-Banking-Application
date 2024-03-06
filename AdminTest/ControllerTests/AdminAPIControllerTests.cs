using Admin.AdminAPI;
using Admin.Controllers;
using Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace AdminTest.ControllerTests;
public class AdminAPIControllerTests
{
    private readonly ICustomerRepository _repo;

    public AdminAPIControllerTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<BankingApplicationContext>(options =>
            options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var _context = serviceProvider.GetRequiredService<BankingApplicationContext>();

        _context.Database.EnsureCreated();

        SeedData.InitDb(serviceProvider);

        _repo = new CustomerRepository(_context);
    }


    [Fact]
    public void GetTest()
    {
        var controller = new AdminAPIController(_repo);

        var customers = _repo.GetCustomers();
        Assert.NotNull(customers);
        Assert.Equal(3, customers.Count());
    }



}
