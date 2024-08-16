using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

[TestFixture]
public class SaveProductServiceTests : IDisposable
{
    private AppDbContext _context;
    private SaveProductService _saveProductService;
    private Mock<IUserNameClaimService> _mockUserNameClaimService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _mockUserNameClaimService = new Mock<IUserNameClaimService>();
        _mockUserNameClaimService.Setup(s => s.GetUserName()).Returns("TestUser");

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        })));

        _context = new AppDbContext(options, _mockUserNameClaimService.Object);
        _saveProductService = new SaveProductService(_context, _mockHttpContextAccessor.Object);
    }


    [Test]
    public async Task SaveProductsAsync_ShouldAddProductsToContext()
    {
        // Arrange
        var products = new List<RawJsonDto>
        {
            new RawJsonDto
            {
                Name = "Product1",
                Price_History = new List<Price_History>
                {
                    new Price_History { Price = 100 }
                },
                Store = new StoreName { Name = "Store1" }
            }
        };

        var userId = "user123";
        var isJob = true;
        var currentDateTime = DateTime.UtcNow;

        // Act
        await _saveProductService.SaveProductsAsync<SearchHistory>(products, userId, isJob);

        // Assert
        var searchHistories = _context.SearchHistory.ToList();
        Assert.AreEqual(1, searchHistories.Count);
        var searchHistory = searchHistories.First();

        // Assert each field in the SearchHistory entity
        Assert.AreEqual(isJob ? "-1" : userId, searchHistory.UserId);
        Assert.AreEqual(isJob, searchHistory.IsJob);
        Assert.AreEqual("Product1", searchHistory.SearchString);
        Assert.AreEqual("Store1", searchHistory.Store);
        Assert.AreEqual(100, searchHistory.Price);

        // DateTime comparisons
        var tolerance = TimeSpan.FromSeconds(1);
        Assert.IsTrue(Math.Abs((searchHistory.SearchDate - currentDateTime).TotalSeconds) < tolerance.TotalSeconds);
        Assert.IsTrue(Math.Abs((searchHistory.Created - currentDateTime).TotalSeconds) < tolerance.TotalSeconds);

        // Assert auditing fields
        Assert.AreEqual("TestUser", searchHistory.CreatedBy);
        Assert.AreEqual("TestUser", searchHistory.LastModifiedBy);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
