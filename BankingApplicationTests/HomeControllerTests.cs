namespace BankingApplicationTests;
using BankingApplication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsHomePage()
    {
        var controller = new HomeController();

        // mock HttpContext session;
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;
       
        // set http context
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,

        };

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }


}
