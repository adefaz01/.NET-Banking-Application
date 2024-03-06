using Admin.Data;
using Microsoft.Extensions.DependencyInjection;
using Admin.Models;
using Xunit;
using Admin.AdminAPI;
using Microsoft.EntityFrameworkCore;
using System;

namespace AdminTest.RepositoryTests;

public class CustomerRepositoryTests : IDisposable
{
    private readonly BankingApplicationContext _context;

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    public CustomerRepositoryTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<BankingApplicationContext>(options =>
            options.UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared"));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _context = serviceProvider.GetRequiredService<BankingApplicationContext>();

        _context.Database.EnsureCreated();

        SeedData.InitDb(serviceProvider);
    }

    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    [InlineData(2300)]
    public void GetGetCustomerByIDTestSuccess(int id)
    {
        var repository = new CustomerRepository(_context);

        var customer = repository.GetCustomerByID(id);

        Assert.NotNull(customer);
        Assert.Equal(id,customer.CustomerID);
    }

    [Theory]
    [InlineData(21200)]
    [InlineData(223200)]
    public void GetGetCustomerByIDTestFail(int id)
    {
        var repository = new CustomerRepository(_context);

        var customer = repository.GetCustomerByID(id);

        Assert.Null(customer);
    }


    [Fact]
    public void GetCustomersTest()
    {
        var repository = new CustomerRepository(_context);

        var customers = repository.GetCustomers();

        Assert.NotNull(customers);
        Assert.Equal(3, customers.Count());

    }

    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    [InlineData(2300)]
    public void LockCustomerLoginTest(int id)
    {
        var repository = new CustomerRepository(_context);

        repository.LockCustomerLogin(id);

        var customer = _context.Customers.Find(id);

        Assert.True(customer.Islocked);
    }

    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    [InlineData(2300)]
    public void UnlockCustomerLoginTest(int id)
    {
        var repository = new CustomerRepository(_context);

        repository.UnlockCustomerLogin(id);

        var customer = _context.Customers.Find(id);

        Assert.False(customer.Islocked);
    }

    [Theory]
    [InlineData(2100, "Adam De Fazio")]
    [InlineData(2200, "Derek Tek")]
    public void UpdateCustomerTest(int id, string name)
    {
        var repository = new CustomerRepository(_context);

        var customer = _context.Customers.Find(id);
        customer.Name = name;

        repository.UpdateCustomer(customer);

        Assert.Equal(name, customer.Name);

    }

}

