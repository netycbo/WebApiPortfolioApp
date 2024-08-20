using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Handlers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;


namespace WebApiPortfolioTest
{
    [TestFixture]
    public class UpdatePriceProduktHandlerTests 
    {
        private AppDbContext _context;
        private Mock<IMapper> _mockMapper;
        private Mock<IApiCall> _mockApiCall;
        private Mock<ISaveProductService> _mockSaveProductService;
        private Mock<IProductFilterService> _mockProductFilterService;
        private Mock <IDeserializeService> _mockDeserializeService;
        private UpdatePriceProduktHandler _updatePriceProduktHandler;
        private Mock<IUserNameClaimService> _mockUserNameClaimService;

        [SetUp]
        public void Setup()
        {
            _mockUserNameClaimService = new Mock<IUserNameClaimService>();
            _mockUserNameClaimService.Setup(s => s.GetUserName()).Returns("TestUser");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options, _mockUserNameClaimService.Object);

            _mockMapper = new Mock<IMapper>();
            _mockApiCall = new Mock<IApiCall>();
            _mockSaveProductService = new Mock<ISaveProductService>();
            _mockProductFilterService = new Mock<IProductFilterService>();
            _mockDeserializeService = new Mock<IDeserializeService>();

            _updatePriceProduktHandler = new UpdatePriceProduktHandler(
                _context,
                _mockApiCall.Object,
                _mockMapper.Object,
                _mockDeserializeService.Object,
                _mockSaveProductService.Object,
                _mockProductFilterService.Object
            );

            _mockSaveProductService.Setup(x => x.SaveTemporaryProductsAsync(It.IsAny<List<TemporaryProductsDto>>()))
                .Returns(Task.CompletedTask);
        }
        [Test]
        public async Task UpdatePriceProduktHandler_ShouldUpdatePriceOfProducts()
        {
            // Arrange
            var request = new UpdatePriceProduktRequest
            {
                UserName = "TestUser"
            };

            var productsInDb = new List<ProductSubscription>
    {
        new ProductSubscription { UserName = "TestUser", ProductName = "Product1", Price = 10, Store = "Store1", UserId = "testUserId1", Id = 1 },
        new ProductSubscription { UserName = "TestUser", ProductName = "Product2", Price = 10, Store = "Store2", UserId = "testUserId2", Id = 2 }
    };
            await _context.ProductSubscriptions.AddRangeAsync(productsInDb);
            await _context.SaveChangesAsync();

            var apiResponse = new RawJsonDtoResponse
            {
                Data = new List<RawJsonDto>
                {
            new RawJsonDto { Name = "Product1", Current_Price = 20, Store = new StoreName { Name = "Store3" }, Id = 1 },
            new RawJsonDto { Name = "Product2", Current_Price = 40, Store = new StoreName { Name = "Store4" }, Id = 2 }
                }
            };

            _mockApiCall.Setup(x => x.ExecuteRequestAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = JsonConvert.SerializeObject(apiResponse) });

            _mockDeserializeService.Setup(x => x.Deserialize<RawJsonDtoResponse>(It.IsAny<string>()))
                .Returns(apiResponse);

            _mockMapper.Setup(x => x.Map<List<RawJsonDto>>(It.IsAny<object>()))
                .Returns(apiResponse.Data);

            _mockProductFilterService.Setup(x => x.FilterNullValues(It.IsAny<List<RawJsonDto>>()))
                .Returns(apiResponse.Data);

            _mockProductFilterService.Setup(x => x.GroupByLowestPrice(It.IsAny<List<RawJsonDto>>()))
                .Returns(apiResponse.Data);

            var tempProducts = new List<TemporaryProductsDto>();

            _mockMapper.Setup(x => x.Map<List<UpdatePriceProduktDto>>(It.IsAny<List<RawJsonDto>>()))
                .Returns(new List<UpdatePriceProduktDto>
                {
            new UpdatePriceProduktDto { ProductName = "Product1", Price = 20, Store = new StoreName { Name = "Store3" } },
            new UpdatePriceProduktDto { ProductName = "Product2", Price = 40, Store = new StoreName { Name = "Store4" } }
                });

            _mockMapper.Setup(x => x.Map<TemporaryProductsDto>(It.IsAny<UpdatePriceProduktDto>()))
                .Returns((UpdatePriceProduktDto source) =>
                    new TemporaryProductsDto { Name = source.ProductName, Price = source.Price.Value, Store = source.Store });

            _mockSaveProductService.Setup(x => x.SaveTemporaryProductsAsync(It.IsAny<List<TemporaryProductsDto>>()))
                .Callback<List<TemporaryProductsDto>>(products =>
                {
                    tempProducts.AddRange(products);
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _updatePriceProduktHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Data.Count); 
            Assert.AreEqual(20, result.Data.First(x => x.ProductName == "Product1").Price);
            Assert.AreEqual(40, result.Data.First(x => x.ProductName == "Product2").Price);

            var product1FromDb = await _context.ProductSubscriptions.FirstOrDefaultAsync(p => p.ProductName == "Product1");
            var product2FromDb = await _context.ProductSubscriptions.FirstOrDefaultAsync(p => p.ProductName == "Product2");

            Assert.IsNotNull(product1FromDb);
            Assert.IsNotNull(product2FromDb);

            var tempProduct1 = tempProducts.First(p => p.Name == "Product1");
            if (product1FromDb.Price > tempProduct1.Price)
            {
                product1FromDb.Price = tempProduct1.Price;
                product1FromDb.Store = tempProduct1.Store.Name;
            }

            var tempProduct2 = tempProducts.First(p => p.Name == "Product2");
            if (product2FromDb.Price > tempProduct2.Price)
            {
                product2FromDb.Price = tempProduct2.Price;
                product2FromDb.Store = tempProduct2.Store.Name;
            }

            Assert.AreEqual(20, tempProduct1.Price);
            Assert.AreEqual(40, tempProduct2.Price);
            Assert.AreEqual("Store3", tempProduct1.Store.Name);
            Assert.AreEqual("Store4", tempProduct2.Store.Name);

        }
        [TearDown]
        public void Dispose()
        {
            _context?.Dispose();
        }

    }


}
