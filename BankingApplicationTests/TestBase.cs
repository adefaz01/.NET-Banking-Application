using BankingApplication.Data;
using BankingApplication.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BankingApplicationTests;
public class TestBase
{
    protected BankingApplicationContext _context;
    protected ISessionWrapper _session;

    protected void InitializeContextAndSession(int customerID)
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
        sessionMock.Setup(s => s.GetInt32(It.IsAny<string>())).Returns(customerID);

        _session = sessionMock.Object;
    }
}