using AutoMapper;
using MediatR;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;
namespace WebApiPortfolioApp.API.Handlers
{
    public class AddProductsToNewsLetterHandler(IApiCall apiCall, IMapper mapper, IUserIdService userIdService,
        ISaveToProductSubscriptionService productSaveService, IHttpContextAccessor httpContextAccessor,
        IDeserializeService deserializeService, IUserNameClaimService userNameClaimService,
        IEmailService emailService, ViewRender viewRenderer, IGetEmailService getEmailService,
        IProductFilterService productFilterService, IShopNameValidator shopNameValidator) : IRequestHandler<AddProductsToNewsLetterRequest, RawJsonDtoResponse>
    {
        private readonly IShopNameValidator _shopNameValidator = shopNameValidator;

        public async Task<RawJsonDtoResponse> Handle(AddProductsToNewsLetterRequest request, CancellationToken cancellationToken)
        {
            var emailContent = await viewRenderer.RenderToStringAsync("SendEmail/NewsLetter/WelcomToNewsLetter", new { ProductName = request.SearchProduct });
            var restRequest = apiCall.CreateProductSearchRequest(request.SearchProduct, 50);
            var response = await apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
            Console.WriteLine($"Response Content: {response.Content}");
            if (response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new FailedToFetchDataExeption("Failed to fetch data");
            }
            try
            {
                var rawProductResponse = deserializeService.Deserialize<RawJsonDtoResponse>(response.Content);

                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    throw new CantDeserializeExeption(response.Content);
                }

                var mappedProducts = mapper.Map<List<RawJsonDto>>(rawProductResponse);
                var filterNullValues = productFilterService.FilterNullValues(mappedProducts);
                if (filterNullValues.Count == 0)
                {
                    throw new NoMatchingFiltredProductsExeption("To many null values in data");
                }
                List<RawJsonDto> filteredByStoreName = filterNullValues;
                if (!string.IsNullOrEmpty(request.Shop))
                {
                    var shopNameValidatorTask = _shopNameValidator.ValidateShopName(request.Shop);
                    var shopNameValidator = await shopNameValidatorTask;
                    filteredByStoreName = productFilterService.FilterByStoreName(filterNullValues, shopNameValidator);
                }
                var outOfStockFilter = productFilterService.OutOfStockFilter(filteredByStoreName);
                if (outOfStockFilter == null)
                {
                    throw new OutOFStockExeption("Last date in price history is older than 25 days");
                }
                var groupedProducts = productFilterService.GroupByLowestPrice(outOfStockFilter);
                var newsletterProducts = mapper.Map<List<AddProductsToNewsLetterDto>>(groupedProducts);
                var userIdClaim = userIdService.GetUserId();
                var userNameClaim = userNameClaimService.GetUserName();
                var subscribedEmails = await getEmailService.GetMailingList();
                foreach (var email in subscribedEmails)
                {
                    await emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = email,
                        Subject = $"You have been subscribed to the {request.SearchProduct} newsletter",
                        Body = emailContent
                    });
                }
                try
                {
                    await productSaveService.SaveToProductSubscriptionAsync(newsletterProducts, userIdClaim, userNameClaim);
                }
                catch (FailedToSaveExeption)
                {
                    throw new FailedToSaveExeption("Error occurred while saving products");
                }
                
                return new RawJsonDtoResponse { Data = groupedProducts };
                
            }

            catch (CantDeserializeExeption)
            {
                throw new CantDeserializeExeption(response.Content);
            }
            
        }
    }
}

