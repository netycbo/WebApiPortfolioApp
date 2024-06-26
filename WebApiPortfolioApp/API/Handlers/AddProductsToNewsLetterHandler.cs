﻿using AutoMapper;
using MediatR;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;
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
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;
namespace WebApiPortfolioApp.API.Handlers
{
    public class AddProductsToNewsLetterHandler : IRequestHandler<AddProductsToNewsLetterRequest, RawJsonDto>
    {
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IUserIdService _userIdService;
        private readonly ISaveToProductSubscriptionService _productSaveService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeserializeService _deserializeService;
        private readonly IUserNameClaimService _userNameClaimService;
        private readonly IEmailService _emailService;
        private readonly ViewRender _viewRenderer;
        private readonly IGetEmailService _getEmailService;
        private readonly IProductFilterService _productFilterService;

        public AddProductsToNewsLetterHandler(IApiCall apiCall, IMapper mapper, IUserIdService userIdService,
            ISaveToProductSubscriptionService productSaveService, IHttpContextAccessor httpContextAccessor,
            IDeserializeService deserializeService, IUserNameClaimService userNameClaimService,
            IEmailService emailService, ViewRender viewRenderer, IGetEmailService getEmailService,
            IProductFilterService productFilterService)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _userIdService = userIdService;
            _productSaveService = productSaveService;
            _httpContextAccessor = httpContextAccessor;
            _deserializeService = deserializeService;
            _userNameClaimService = userNameClaimService;
            _emailService = emailService;
            _viewRenderer = viewRenderer;
            _getEmailService = getEmailService;
            _productFilterService = productFilterService;
        }

        public async Task<RawJsonDto> Handle(AddProductsToNewsLetterRequest request, CancellationToken cancellationToken)
        {
            var emailContent = await _viewRenderer.RenderToStringAsync("SendEmail/NewsLetter/WelcomToNewsLetter", new { ProductName = request.SearchProduct });
            var restRequest = _apiCall.CreateProductSearchRequest(request.SearchProduct, 50);
            var response = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
            Console.WriteLine($"Response Content: {response.Content}");
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new FailedToFetchDataExeption("Failed to fetch data");
            }
            try
            {
                var rawProductResponse = _deserializeService.Deserialize<RawJsonDtoResponse>(response.Content);

                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    throw new CantDeserializeExeption(response.Content);
                }
                
                var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse);
                var filterNullValues = _productFilterService.FilterNullValues(mappedProducts); 
                if (filterNullValues.Count == 0)
                {
                    throw new NoMatchingFiltredProductsExeption("No matching filtered products");
                }
                var groupedProducts = _productFilterService.GroupByLowestPrice(filterNullValues);
                var newsletterProducts = _mapper.Map<AddProductsToNewsLetterDto>(groupedProducts);
                var userIdClaim = _userIdService.GetUserId();
                var userNameClaim = _userNameClaimService.GetUserName();
                var subscribedEmails = await _getEmailService.GetMailingList();
                foreach (var email in subscribedEmails)
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = email,
                        Subject = $"You have been subscribed to the {request.SearchProduct} newsletter",
                        Body = emailContent
                    });
                }
                try
                {
                    await _productSaveService.SaveToProductSubscriptionAsync(newsletterProducts, userIdClaim, userNameClaim);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
                return new RawJsonDto();
                
            }
            catch (CantDeserializeExeption)
            {
                throw new CantDeserializeExeption(response.Content);
            }
        }
    }
}

