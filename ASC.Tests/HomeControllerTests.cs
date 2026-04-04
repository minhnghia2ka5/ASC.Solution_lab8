using ASC.Tests.TestUtilities;
using ASC.Utilities;
using ASC.Web.Configuration;
using ASC.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASC.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> loggerMock;
        private readonly Mock<IOptions<ApplicationSettings>> optionsMock;
        private readonly Mock<HttpContext> mockHttpContext;

        public HomeControllerTests()
        {
            // Setup mocks
            loggerMock = new Mock<ILogger<HomeController>>();
            optionsMock = new Mock<IOptions<ApplicationSettings>>();
            mockHttpContext = new Mock<HttpContext>();

            optionsMock.Setup(ap => ap.Value).Returns(new ApplicationSettings
            {
                ApplicationTitle = "ASC"
            });

            mockHttpContext.Setup(p => p.Session).Returns(new FakeSession());
        }

        [Fact]
        public void HomeController_Index_View_Test()
        {
            // Arrange
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result); // ✅ Test kiểu trả về ViewResult
        }

        [Fact]
        public void HomeController_Index_NoModel_Test()
        {
            // Arrange
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result); // ✅ loại bỏ null dereference

            // Assert
            Assert.Null(viewResult.ViewData.Model); // Model phải null
        }

        [Fact]
        public void HomeController_Index_Validation_Test()
        {
            // Arrange
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            // Assert
            Assert.Equal(0, viewResult.ViewData.ModelState.ErrorCount); // ModelState phải rỗng
        }

        [Fact]
        public void HomeController_Index_Session_Test()
        {
            // Arrange
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            controller.Index();

            // Assert
            Assert.NotNull(controller.HttpContext.Session.GetSession<ApplicationSettings>("Test")); // Session phải có giá trị
        }
    }
}