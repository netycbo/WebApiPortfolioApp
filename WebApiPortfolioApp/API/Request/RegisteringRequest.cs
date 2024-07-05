using MediatR;
using System.ComponentModel.DataAnnotations;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Validation;

namespace WebApiPortfolioApp.API.Request
{
    public class RegisteringRequest : IRequest<RegisteringResponse>
    {
        [Required]
        [NoSpecialCharactersAttribute]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool SubscribeToMailingList { get; set; }   
    }
}
