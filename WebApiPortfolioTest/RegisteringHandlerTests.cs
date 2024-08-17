using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using WebApiPortfolioApp.API.Handlers;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;

namespace WebApiPortfolioTest
{
    [TestFixture]
    public class RegisteringHandlerTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<RegisteringHandler> _mockRegisteringHandler;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IViewRender> _mockViewRender;
        private RegisteringHandler _registeringHandler;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockEmailService = new Mock<IEmailService>();
            _mockViewRender = new Mock<IViewRender>();
            _registeringHandler = new RegisteringHandler(_mockMapper.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object, _mockEmailService.Object, _mockViewRender.Object);
        }
        [Test]
        public async Task Handle_UserRegistersSuccessfully_ReturnsRegisteringResponse()
        {
            // Arrange
            var request = new RegisteringRequest
            {
                Name = "TestUser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new ApplicationUser { UserName = request.Name, Email = request.Email };
            _mockUserManager.Setup(x => x.FindByNameAsync(request.Name)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _mockMapper.Setup(m => m.Map<ApplicationUser>(request)).Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, request.Password)).ReturnsAsync(IdentityResult.Success);
            _mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("dummyToken");

            _mockConfiguration.Setup(x => x["FrontendUrl"]).Returns("http://localhost");

            var renderedEmail = "Rendered Email Content";
            _mockViewRender.Setup(v => v.RenderToStringAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(renderedEmail);

            // Act
            var result = await _registeringHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RegisteringResponse>(result);
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Admin"), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailRequest>()), Times.Once);
        }
        [Test]
        public void Handle_UserWithNameAlreadyExists_ThrowsUsernameAlreadyTakenException()
        {
            // Arrange
            var request = new RegisteringRequest
            {
                Name = "TestUser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var existingUser = new ApplicationUser { UserName = request.Name };
            _mockUserManager.Setup(x => x.FindByNameAsync(request.Name)).ReturnsAsync(existingUser);

            // Act & Assert
            Assert.ThrowsAsync<UsernameAlreadyTakenException>(() => _registeringHandler.Handle(request, CancellationToken.None));
        }
        [Test]
        public void Handle_UserWithEmailAlreadyExists_ThrowsEmailNotUniqueException()
        {
            // Arrange
            var request = new RegisteringRequest
            {
                Name = "TestUser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var existingUser = new ApplicationUser { Email = request.Email };
            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            Assert.ThrowsAsync<EmailNotUniqueException>(() => _registeringHandler.Handle(request, CancellationToken.None));
        }
        [Test]
        public void Handle_UserCreationFails_ThrowsCantCreateUserExeption()
        {
            // Arrange
            var request = new RegisteringRequest
            {
                Name = "TestUser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(request.Name)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password)).ReturnsAsync(IdentityResult.Failed());

            // Act & Assert
            Assert.ThrowsAsync<CantCreateUserExeption>(() => _registeringHandler.Handle(request, CancellationToken.None));
        }


    }
}
