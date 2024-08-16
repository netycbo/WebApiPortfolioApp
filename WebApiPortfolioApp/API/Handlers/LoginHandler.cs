using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

using WebApiPortfolioApp.Data.Entinities.Identity;


namespace WebApiPortfolioApp.API.Handlers
{
    public class LoginHandler(IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration) : IRequestHandler<LoginRequest, LoginResponse>
    {
        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
               throw new UserNotFoundException("User not found.");
            }
            var result = await userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                throw new InvalidPasswordException("Invalid password.");
            }
            
            var roles = await userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            var userDto = mapper.Map<LoginDto>(user);
            userDto.UserRole = roles.ToList();
            return new LoginResponse
            {
                Data = userDto,
                Token = token
            };
        }
        public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
