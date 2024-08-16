using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.API.Handlers;

namespace WebApiPortfolioTest
{
    [TestFixture]
    public class GenerateJwtTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;
        private LoginHandler _loginHandler;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();

            var user = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                user.Object,
                null,
                new PasswordHasher<ApplicationUser>(),
                null,
                null,
                null,
                null,
                null,
                null
            );

            
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("MySecretKey8r3rwefhsjkafui3yufsafbjksafb");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("MyIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("MyAudience");

            
            _loginHandler = new LoginHandler(_mockMapper.Object, _mockUserManager.Object, _mockConfiguration.Object);
        }

        [Test]
        public void GenerateJwtToken_ShouldReturnExpectedToken()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "TestUser",
                Email = "test@test.com"
            };
            var roles = new List<string> { "user", "admin" };

            // Act
            var result = _loginHandler.GenerateJwtToken(user, roles);

            // Assert
            Assert.IsNotNull(result);

            
        }
    }
}
