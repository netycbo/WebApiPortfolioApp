using AutoMapper;
using AutoMapper.Internal;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;

namespace WebApiPortfolioApp.API.Handlers
{
    public class RegisteringHandler : IRequestHandler<RegisteringRequest, RegisteringResponse>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ViewRender _viewRenderer;

        public RegisteringHandler(IMapper mapper, UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager, IConfiguration configuration,
                                  IEmailService emailService, ViewRender viewRenderer)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _viewRenderer = viewRenderer;
        }

        public async Task<RegisteringResponse> Handle(RegisteringRequest request, CancellationToken cancellationToken)
        {
            var emailContent = await _viewRenderer.RenderToStringAsync("SendEmail/ConfirmationEmail/ConfirmationEmail", new { UserName = request.Name });

            var userExist = await _userManager.FindByNameAsync(request.Name);
            if (userExist == null)
            {
                var user = _mapper.Map<ApplicationUser>(request);
                IdentityResult result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    string roleName = "Admin";
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                    await _userManager.AddToRoleAsync(user, roleName);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = $"{_configuration["FrontendUrl"]}/confirm-email?user={user.Id}&code={Uri.EscapeDataString(code)}";

                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = user.Email,
                        Subject = "Confirm your email",
                        Body = emailContent.Replace("{{callbackUrl}}", callbackUrl) 
                    });
                    var registeringDto = _mapper.Map<RegisteringDto>(user);
                   
                    return new RegisteringResponse
                    {
                        Data = _mapper.Map<RegisteringDto>(user),
                        
                    };
                }
                else
                {
                    return new RegisteringResponse
                    {
                        // Error handling or additional info could be added here
                    };
                }
            }
            return new RegisteringResponse
            {
                // Error handling or additional info could be added here
            };
        }
    }
}
