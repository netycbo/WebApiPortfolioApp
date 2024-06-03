﻿using System.Security.Claims;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.Providers;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class SaveProductService : ISaveProductService
    {
        private readonly AppDbContext _context;
       

        public SaveProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            
        }
        public async Task SaveProductsAsync(List<RawJsonDto> products, int userId, bool isJob)
        {
            var searchHistory = products.Select(product =>
            {
                var priceHistory = product.Price_History.FirstOrDefault();
                return new SearchHistory
                {
                    UserId = isJob ? -1 : userId,
                    IsJob = isJob,
                    SearchString = product.Name,
                    SearchDate = DateTime.UtcNow,
                    Shop = product?.Store.Name ?? null,
                    Price = priceHistory?.Price ?? 0,
                    Created = DateTime.UtcNow,
                };
            }).ToList();

            _context.SearchHistory.AddRange(searchHistory);
            await _context.SaveChangesAsync();
        }
    }
}