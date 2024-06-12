﻿using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
namespace WebApiPortfolioApp
{
    public class AddProductsToNewsLetterHandler : IRequestHandler<AddProductsToNewsLetterRequest, AddProductsToNewsLetterRespons>
    {
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IUserIdService _userIdService;
        private readonly ISaveToProductSubscriptionService _productSaveService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeserializeService _deserializeService;

        public AddProductsToNewsLetterHandler(IApiCall apiCall, IMapper mapper, IUserIdService userIdService,
            ISaveToProductSubscriptionService productSaveService, IHttpContextAccessor httpContextAccessor, IDeserializeService deserializeService)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _userIdService = userIdService;
            _productSaveService = productSaveService;
            _httpContextAccessor = httpContextAccessor;
            _deserializeService = deserializeService;
        }

        public async Task<AddProductsToNewsLetterRespons> Handle(AddProductsToNewsLetterRequest request, CancellationToken cancellationToken)
        {
            var restRequest = _apiCall.CreateProductSearchRequest(request.SearchProduct, 1);
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

                var mappedProducts = _mapper.Map<List<AddProductsToNewsLetterDto>>(rawProductResponse);
                var userIdClaim = _userIdService.GetUserId();
                var userName = _httpContextAccessor.HttpContext.User.Claims
                             .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                await _productSaveService.SaveToProductSubscriptionAsync(mappedProducts, userIdClaim, userName);
                return new AddProductsToNewsLetterRespons { Data = mappedProducts };
            }
            catch (CantDeserializeExeption)
            {
                throw new CantDeserializeExeption(response.Content);
            }
        }
    }
}

