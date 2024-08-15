using Moq;
using RestSharp;
using WebApiPortfolioApp.API.Handlers.Services;

[TestFixture]
public class ApiCallTests
{
    private IApiCall _apiCall;
    private const string _apiKey = "test-api-key";
    private Mock<IRestClient> _mockRestClient;

    [SetUp]
    public void SetUp()
    {
        var restClient = new RestClient();
        _apiCall = new ApiCall(restClient, _apiKey);
        _mockRestClient = new Mock<IRestClient>();
        _apiCall = new ApiCall(_mockRestClient.Object, _apiKey);

    }

    [Test]
    public void CreateProductSearchRequest_ShouldCreateValidRequest()
    {
        // Arrange
        var searchProduct = "TestProduct";
        var numberOfResults = 10;

        // Act
        var request = _apiCall.CreateProductSearchRequest(searchProduct, numberOfResults);

        // Assert

        Assert.AreEqual("/https://kassal.app/api/v1/products", request.Resource);
        Assert.AreEqual(Method.Get, request.Method);

        var searchParam = request.Parameters.FirstOrDefault(p => p.Name == "search");
        Assert.IsNotNull(searchParam);
        Assert.AreEqual(searchProduct, searchParam?.Value);

        var sizeParam = request.Parameters.FirstOrDefault(p => p.Name == "size");
        Assert.IsNotNull(sizeParam);
        Assert.AreEqual(numberOfResults.ToString(), sizeParam?.Value);

        var sortParam = request.Parameters.FirstOrDefault(p => p.Name == "sort");
        Assert.IsNotNull(sortParam);
        Assert.AreEqual("price_desc", sortParam?.Value);

        var authHeader = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.HttpHeader && p.Name == "Authorization");
        Assert.IsNotNull(authHeader, "Authorization header is missing.");
        Assert.AreEqual($"Bearer {_apiKey}", authHeader?.Value, "Authorization header value is incorrect.");
    }
    [Test]
    public async Task ExecuteRequestAsync_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new RestRequest("/test-endpoint", Method.Get);
        var expectedResponse = new RestResponse
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = "{\"success\":true}"
        };

        _mockRestClient
        .Setup(client => client.ExecuteAsync(request, It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

        // Act
        var response = await _apiCall.ExecuteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("{\"success\":true}", response.Content);
    }
}

