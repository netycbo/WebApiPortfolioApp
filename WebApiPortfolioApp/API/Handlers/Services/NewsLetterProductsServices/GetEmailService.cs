
using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.Data;

namespace WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices
{
    public class GetEmailService : IGetEmailService
    {
        private readonly AppDbContext _context;
        public GetEmailService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetMailingList()
        {
            return await _context.Users
                                 .Where(user => user.IsSubscribedToNewsLetter)
                                 .Select(user => user.Email!)
                                 .ToListAsync();
        }
    }
}
