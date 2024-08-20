using AutoMapper;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities;

namespace WebApiPortfolioTest
{
    [TestFixture]
    public class ProductSearchHandlerTests
    {
        private Mock<IProductFilterService> _mockProductFilterService;
        private Mock<ISaveProductService> _mockSaveProductService;
        private Mock<IApiCall> _mockApiCall;
        private Mock<IUserIdService> _mockUserIdService;
        private Mock<IShopNameValidator> _mockShopNameValidator;
        private Mock<IMapper> _mockMapper;
        private Mock<IDeserializeService> _mockDeserializeService;

        private ProductSearchHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockProductFilterService = new Mock<IProductFilterService>();
            _mockSaveProductService = new Mock<ISaveProductService>();
            _mockApiCall = new Mock<IApiCall>();
            _mockUserIdService = new Mock<IUserIdService>();
            _mockShopNameValidator = new Mock<IShopNameValidator>();
            _mockMapper = new Mock<IMapper>();
            _mockDeserializeService = new Mock<IDeserializeService>();

            _handler = new ProductSearchHandler(
                _mockApiCall.Object,
                _mockMapper.Object,
                _mockProductFilterService.Object,
                _mockSaveProductService.Object,
                _mockUserIdService.Object,
                _mockDeserializeService.Object,
                _mockShopNameValidator.Object
            );

            var apiResponse = new RawJsonDtoResponse
            {
                Data = new List<RawJsonDto>
                {
                new RawJsonDto { Name = "TestProduct", Current_Price = 20, Vendor = "TestVendor", Store = new StoreName { Name = "Store3", Code = "Code3" }, Price_History = new List<Price_History> { new Price_History { Price = 20, Date = DateTime.Now }}, Id = 1 },
                new RawJsonDto { Name = "TestProduct", Current_Price = 23, Vendor = "TestVendor", Store = new StoreName { Name = "Store4", Code = "Code3" }, Price_History = new List<Price_History> { new Price_History { Price = 20, Date = DateTime.Now }}, Id = 2 },
                new RawJsonDto { Name = "TestProduct", Current_Price = 33, Vendor = "TestVendor", Store = new StoreName { Name = "Store5", Code = "Code3" }, Price_History = new List<Price_History> { new Price_History { Price = 20, Date = DateTime.Now }}, Id = 3 }
                }
            };
            var wrongApiResponse = new RawJsonDtoResponse
            {
                Data = new List<RawJsonDto>()
            };
           
            _mockApiCall.Setup(x => x.ExecuteRequestAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = JsonConvert.SerializeObject(apiResponse) });

            _mockDeserializeService.Setup(x => x.Deserialize<RawJsonDtoResponse>(It.IsAny<string>()))
                .Returns(apiResponse);

            _mockMapper.Setup(x => x.Map<List<RawJsonDto>>(It.IsAny<object>()))
                .Returns(apiResponse.Data);

            _mockProductFilterService.SetupSequence(x => x.FilterNullValues(It.IsAny<List<RawJsonDto>>()))
                .Returns(wrongApiResponse.Data)
                .Returns(wrongApiResponse.Data)
                .Returns(apiResponse.Data);

            _mockProductFilterService.Setup(x => x.GroupByLowestPrice(It.IsAny<List<RawJsonDto>>()))
                .Returns(apiResponse.Data);
            _mockProductFilterService.Setup(x => x.OutOfStockFilter(It.IsAny<List<RawJsonDto>>()))
                .Returns(apiResponse.Data);
            _mockProductFilterService.Setup(x => x.FilterByStoreName(It.IsAny<List<RawJsonDto>>(), It.IsAny<string>()))
                .Returns(apiResponse.Data);
        }
        [Test]
        public async Task Handle_SearchProduct_ReturnsListOfProducts()
        {
            // Arrange
            var request = new ProductSearchRequest
            {
                SearchProduct = "TestProduct"
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RawJsonDtoResponse>(result);
            Assert.AreEqual(3, result.Data.Count);
            Assert.AreEqual("TestProduct", result.Data.First().Name);
        }
        [Test]
        public async Task Handle_SearchProductWithShopName_ReturnsListOfProducts()
        {
            // Arrange
            var request = new ProductSearchRequest
            {
                SearchProduct = "TestProduct",
                Store = "Store3"
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RawJsonDtoResponse>(result);
            Assert.AreEqual("Store3", result.Data.First().Store.Name);
        }
        [Test]
        public async Task Handle_WhenResponseIsNotSuccessful_UseLoopToGetResponse()
        {
            // Arrange
            var request = new ProductSearchRequest
            {
                SearchProduct = "TestProduct",
                NumberOfResults = 1
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockProductFilterService.Verify(x => x.FilterNullValues(It.IsAny<List<RawJsonDto>>()), Times.Exactly(3));
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RawJsonDtoResponse>(result);
            Assert.AreEqual("TestProduct", result.Data.First().Name);
        }
    }
}
