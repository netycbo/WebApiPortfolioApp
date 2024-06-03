using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Quartz;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;
using AutoMapper;
using RestSharp;
using Moq.EntityFrameworkCore;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.Providers;

namespace WebApiPortfolioApp.Tests
{
    [TestFixture]
    public class PriceCheckJobTests : IDisposable
    {
        private Mock<IApiCall> _apiCallMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IComparePrices> _comparePricesMock;
        private AppDbContext _contextDb;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ISaveProductService> _productSaveServiceMock;
        private Mock<IUserIdService> _userIdServiceMock;
        private Mock<IProductFilterService> _productFilterServiceMock;
        private Mock<ViewRender> _viewRendererMock;
        private PriceCheckJob _job;

        [SetUp]
        public void Setup()
        {
            _apiCallMock = new Mock<IApiCall>();
            _mapperMock = new Mock<IMapper>();
            _comparePricesMock = new Mock<IComparePrices>();
            _emailServiceMock = new Mock<IEmailService>();
            _productSaveServiceMock = new Mock<ISaveProductService>();
            _userIdServiceMock = new Mock<IUserIdService>();
            _productFilterServiceMock = new Mock<IProductFilterService>();
            _viewRendererMock = new Mock<ViewRender>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var userResolverService = new UserResolverService(new HttpContextAccessor());
            _contextDb = new AppDbContext(options, userResolverService);

            _job = new PriceCheckJob(
                _apiCallMock.Object,
                _mapperMock.Object,
                _comparePricesMock.Object,
                _contextDb,
                _productFilterServiceMock.Object,
                _emailServiceMock.Object,
                _userIdServiceMock.Object,
                _productSaveServiceMock.Object,
                _viewRendererMock.Object
            );
        }

        [Test]
        public async Task Execute_SendsEmail_WhenPriceIsBelowAverage()
        {
            // Arrange
            var jobExecutionContextMock = new Mock<IJobExecutionContext>();
            jobExecutionContextMock.Setup(x => x.JobDetail.JobDataMap.GetBoolean("IsJob")).Returns(true);

            var productResponse = new RawJsonDtoResponse
            {
                Data = new List<RawJsonDto>
                {
                    new RawJsonDto { Name = "Hansa Mango Ipa" }
                }
            };

            _apiCallMock.Setup(x => x.CreateProductSearchRequest(It.IsAny<string>()))
                        .Returns(new RestRequest());

            _apiCallMock.Setup(x => x.ExecuteRequestAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new RestResponse
                        {
                            Content = "{\"Data\": [{\"Name\": \"Hansa Mango Ipa\"}]}"
                        });

            _mapperMock.Setup(x => x.Map<List<RawJsonDto>>(It.IsAny<object>()))
                       .Returns(productResponse.Data);

            _comparePricesMock.Setup(x => x.ComparePricesAsync(It.IsAny<string>()))
                              .ReturnsAsync(10.0m);

            _productFilterServiceMock.Setup(x => x.FilterProducts(It.IsAny<List<RawJsonDto>>()))
                                     .Returns(productResponse.Data);

            _contextDb.SearchHistory.Add(new SearchHistory { SearchString = "Hansa Mango Ipa", Price = 15.0m });
            _contextDb.Users.Add(new ApplicationUser { IsSubscribedToLowBeerPriceAletr = true, Email = "test@example.com" });
            await _contextDb.SaveChangesAsync();

            _viewRendererMock.Setup(x => x.RenderToStringAsync(It.IsAny<string>(), null))
                             .ReturnsAsync("Email Content");

            // Act
            await _job.Execute(jobExecutionContextMock.Object);

            // Assert
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.Is<EmailRequest>(req =>
                req.ToEmail == "test@example.com" &&
                req.Subject == "Spadła cena piwa" &&
                req.Body == "Email Content"
            )), Times.Once);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
